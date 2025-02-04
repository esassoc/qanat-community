import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewContainerRef } from "@angular/core";
import { BehaviorSubject, Observable, combineLatest, of } from "rxjs";
import { filter, switchMap, tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ExternalMapLayerDto, UserDto, WaterAccountIndexGridDto, GeographyMinimalDto, WellMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { NgIf, AsyncPipe, CommonModule } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { RouterLink } from "@angular/router";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FormsModule } from "@angular/forms";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { ColDef, GridApi, GridReadyEvent, RowNode } from "ag-grid-community";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { DeleteWaterAccountComponent } from "src/app/shared/components/water-account/modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "src/app/shared/components/water-account/modals/merge-water-accounts/merge-water-accounts.component";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { UpdateParcelsComponent } from "src/app/shared/components/water-account/modals/update-parcels/update-parcels.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AgGridAngular } from "ag-grid-angular";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { Map, layerControl } from "leaflet";
import { WaterAccountsLayerComponent } from "src/app/shared/components/leaflet/layers/water-accounts-layer/water-accounts-layer.component";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";

@Component({
    selector: "water-account-list",
    templateUrl: "./water-account-list.component.html",
    styleUrls: ["./water-account-list.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        RouterLink,
        LoadingDirective,
        NgIf,
        AsyncPipe,
        FormsModule,
        CommonModule,
        QanatGridComponent,
        QanatGridHeaderComponent,
        LoadingDirective,
        WaterDashboardNavComponent,
        IconComponent,
        QanatMapComponent,
        GsaBoundariesComponent,
        WaterAccountsLayerComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        AlertDisplayComponent,
    ],
})
export class WaterAccountListComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public geography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;

    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public columnDefs$: Observable<ColDef[]>;

    public refreshWaterAccounts$: BehaviorSubject<void> = new BehaviorSubject(null);
    public waterAccounts$: Observable<WaterAccountIndexGridDto[]>;
    public waterAccountIDs$: Observable<number[]>;

    public wells$: Observable<WellMinimalDto[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerDto[]>;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public currentUserHasOverallPermission: boolean = false;
    public currentUserHasNoGeographies: boolean = false;

    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public selectedPanel: "Grid" | "Hybrid" | "Map" = "Hybrid";
    public selectedWaterAccountID: number;

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardWaterAccounts;
    public isLoading: boolean = true;
    public firstLoad: boolean = true;

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private wellService: WellService,
        private utilityFunctionsService: UtilityFunctionsService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        //this.createColumnDefs();

        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
                this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(user, PermissionEnum.WaterAccountRights, RightsEnum.Read);
            })
        );

        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                if (geography?.GeographyID == this.currentGeography?.GeographyID) {
                    return;
                }

                this.currentGeography = geography;
                this.onGeographySelected(geography);
            })
        );

        this.columnDefs$ = combineLatest({ user: this.currentUser$, geography: this.geography$ }).pipe(
            switchMap(({ user, geography }) => {
                const colDefs = this.createColumnDefs();
                return of(colDefs);
            })
        );

        this.currentUserGeographies$ = this.currentUser$.pipe(
            switchMap(() => {
                return this.geographyService.geographiesCurrentUserGet();
            }),
            tap((geographies) => {
                this.currentUserHasNoGeographies = geographies.length == 0;
            })
        );

        this.waterAccounts$ = combineLatest({ user: this.currentUser$, geography: this.geography$, _: this.refreshWaterAccounts$ }).pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((data) => {
                return this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsCurrentUserGet(data.geography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
                this.firstLoad = false;
            })
        );

        this.waterAccountIDs$ = this.waterAccounts$.pipe(
            switchMap((waterAccounts) => {
                return of(waterAccounts.map((x) => x.WaterAccountID));
            })
        );

        this.wells$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.wellService.geographiesGeographyIDWellsCurrentUserGet(geography.GeographyID);
            })
        );

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geography.GeographyID);
            })
        );

        this.externalMapLayers$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.externalMapLayerService.geographiesGeographyIDExternalMapLayersGet(geography.GeographyID);
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public toggleSelectedPanel(selectedPanel: "Grid" | "Hybrid" | "Map") {
        this.selectedPanel = selectedPanel;

        // resizing map to fit new container width; timeout needed to ensure new width has registered before running invalidtaeSize()
        setTimeout(() => {
            this.map.invalidateSize(true);

            if (this.layerControl && this.bounds) {
                this.map.fitBounds(this.bounds);
            }
        }, 300);

        // if no map is visible, turn of grid selection
        if (selectedPanel == "Grid") {
            this.gridApi.setGridOption("rowSelection", null);
            this.selectedWaterAccountID = undefined;
        } else {
            this.gridApi.setGridOption("rowSelection", "single");
        }
    }

    public onGeographySelected(selectedGeography: GeographyMinimalDto) {
        if (!this.currentUser) {
            this.currentUserHasManagerPermissionsForSelectedGeography = false;
            return;
        }

        this.currentUserHasManagerPermissionsForSelectedGeography =
            this.currentUserHasOverallPermission ||
            this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read, selectedGeography.GeographyID);

        this.currentGeographyService.setCurrentGeography(selectedGeography);
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        this.cdr.detectChanges();
    }

    public handleLayerBoundsCalculated(bounds: any) {
        this.bounds = bounds;
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onGridRefReady(gridRef: AgGridAngular) {
        this.gridRef = gridRef;
    }

    public onGridSelectionChanged() {
        const selectedNodes = this.gridApi.getSelectedNodes();
        this.selectedWaterAccountID = selectedNodes.length > 0 ? selectedNodes[0].data.WaterAccountID : null;
    }

    public onMapSelectionChanged(selectedWaterAccountID: number) {
        if (this.selectedWaterAccountID == selectedWaterAccountID) return;

        this.selectedWaterAccountID = selectedWaterAccountID;
        this.gridApi.forEachNode((node, index) => {
            if (node.data.WaterAccountID == selectedWaterAccountID) {
                node.setSelected(true, true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }

    public onWaterAccountCreated() {
        this.refreshWaterAccounts$.next();
    }

    private createColumnDefs(): ColDef[] {
        const colDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    { ActionName: "Update Parcels", ActionIcon: "fas fa-map", ActionHandler: () => this.updateParcelsModal(params.data.WaterAccountID, params.node) },
                    { ActionName: "Update Info", ActionIcon: "fas fa-info-circle", ActionHandler: () => this.updateInfoModal(params.data.WaterAccountID, params.node) },
                    { ActionName: "Merge", ActionIcon: "fas fa-long-arrow-alt-right", ActionHandler: () => this.mergeModal(params.data.WaterAccountID) },
                    { ActionName: "Delete", ActionIcon: "fa fa-times-circle text-danger", ActionHandler: () => this.deleteModal(params.data.WaterAccountID) },
                ];
            }, !this.currentUserHasManagerPermissionsForSelectedGeography),
            this.utilityFunctionsService.createLinkColumnDef("Account Number", "WaterAccountNumber", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccountID}`, LinkDisplay: params.data.WaterAccountNumber };
                },
            }),
            { headerName: "Account Name", field: "WaterAccountName" },
            this.utilityFunctionsService.createMultiLinkColumnDef("APN List", "Parcels", "ParcelID", "ParcelNumber", {
                InRouterLink: "/parcels",
                MaxWidth: 300,
            }),
            { headerName: "Contact Name", field: "ContactName" },
            { headerName: "Contact Address", field: "ContactAddress" },
            { headerName: "Users", valueGetter: (params) => params.data.Users?.map((x) => x.UserFullName) },
            this.utilityFunctionsService.createDecimalColumnDef("# of Users", "Users.length", { DecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Parcels", "Parcels.length", { DecimalPlacesToDisplay: 0 }),
            { headerName: "Notes", field: "Notes" },
            this.utilityFunctionsService.createDateColumnDef("Create Date", "CreateDate", "short", { Hide: !this.currentUserHasManagerPermissionsForSelectedGeography }),
            this.utilityFunctionsService.createDateColumnDef("Last Update Date", "UpdateDate", "short", { Hide: !this.currentUserHasManagerPermissionsForSelectedGeography }),
            { headerName: "Water Account PIN", field: "WaterAccountPIN", hide: !this.currentUserHasManagerPermissionsForSelectedGeography },
            this.utilityFunctionsService.createDateColumnDef("Water Account PIN Last Used", "WaterAccountPINLastUsed", "short", {
                Hide: !this.currentUserHasManagerPermissionsForSelectedGeography,
            }),
        ];

        return colDefs;
    }

    public deleteModal(waterAccountID: number) {
        this.modalService
            .open(DeleteWaterAccountComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.refreshWaterAccounts$.next();
                }
            });
    }

    public mergeModal(waterAccountID: number) {
        this.modalService
            .open(MergeWaterAccountsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.refreshWaterAccounts$.next();
                }
            });
    }

    public updateInfoModal(waterAccountID: number, rowNode: RowNode) {
        this.modalService
            .open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    rowNode.setData(result);
                }
            });
    }

    public updateParcelsModal(waterAccountID: number, rowNode: RowNode) {
        this.modalService
            .open(UpdateParcelsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    rowNode.setData(result);
                }
            });
    }
}
