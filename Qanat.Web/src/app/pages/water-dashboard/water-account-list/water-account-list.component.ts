import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { BehaviorSubject, Observable, combineLatest, of } from "rxjs";
import { shareReplay, switchMap, tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UserDto, WaterAccountIndexGridDto, GeographyMinimalDto, WellMinimalDto, ZoneGroupMinimalDto, ExternalMapLayerSimpleDto } from "src/app/shared/generated/model/models";
import { AsyncPipe, CommonModule } from "@angular/common";
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
import { DeleteWaterAccountComponent } from "src/app/shared/components/water-account/modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "src/app/shared/components/water-account/modals/merge-water-accounts/merge-water-accounts.component";
import { UpdateWaterAccountInfoComponent } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { UpdateParcelsComponent } from "src/app/shared/components/water-account/modals/update-parcels/update-parcels.component";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { Map, layerControl } from "leaflet";
import { WaterAccountsLayerComponent } from "src/app/shared/components/leaflet/layers/water-accounts-layer/water-accounts-layer.component";
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
import { HybridMapGridComponent } from "../../../shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "water-account-list",
    templateUrl: "./water-account-list.component.html",
    styleUrls: ["./water-account-list.component.scss"],
    imports: [
        PageHeaderComponent,
        RouterLink,
        LoadingDirective,
        AsyncPipe,
        FormsModule,
        CommonModule,
        LoadingDirective,
        WaterDashboardNavComponent,
        GsaBoundariesComponent,
        WaterAccountsLayerComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        AlertDisplayComponent,
        HybridMapGridComponent,
        NgSelectModule,
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
    public externalMapLayers$: Observable<ExternalMapLayerSimpleDto[]>;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public currentUserHasOverallPermission: boolean = false;
    public currentUserHasNoGeographies: boolean = false;

    public selectedWaterAccountID: number;

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardWaterAccounts;
    public isLoading: boolean = true;
    public layerLoading: boolean = true;
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
        private cdr: ChangeDetectorRef,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
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
                return this.geographyService.listForCurrentUserGeography();
            }),
            tap((geographies) => {
                this.currentUserHasNoGeographies = geographies.length == 0;
            })
        );

        this.waterAccounts$ = combineLatest({ user: this.currentUser$, geography: this.geography$, _: this.refreshWaterAccounts$ }).pipe(
            tap(() => {
                this.isLoading = true;
                this.layerLoading = true;

                if (this.gridAPI) {
                    this.gridAPI.setGridOption("loading", true);
                }
            }),
            switchMap((data) => {
                return this.waterAccountByGeographyService.listByCurrentUserWaterAccountByGeography(data.geography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
                this.firstLoad = false;
                if (this.gridAPI) {
                    this.gridAPI.setGridOption("loading", false);
                }
            })
        );

        this.waterAccountIDs$ = this.waterAccounts$.pipe(
            switchMap((waterAccounts) => {
                return of(waterAccounts.map((x) => x.WaterAccountID));
            })
        );

        this.wells$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.wellService.listWellsByGeographyIDAndCurrentUserWell(geography.GeographyID);
            }),
            shareReplay(1)
        );

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.listZoneGroup(geography.GeographyID);
            })
        );

        this.externalMapLayers$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.externalMapLayerService.getExternalMapLayer(geography.GeographyID);
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
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

    public gridAPI: GridApi;
    public onGridReady(event: GridReadyEvent) {
        this.gridAPI = event.api;
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

    public onSelectedWaterAccountIDChanged(selectedWaterAccountID) {
        if (this.selectedWaterAccountID == selectedWaterAccountID) return;

        this.selectedWaterAccountID = selectedWaterAccountID;
        return this.selectedWaterAccountID;
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
            { headerName: "Contact Name", field: "WaterAccountContact.ContactName" },
            { headerName: "Email", field: "WaterAccountContact.Email" },
            this.utilityFunctionsService.createPhoneNumberColumnDef("Phone Number", "WaterAccountContact.PhoneNumber"),
            { headerName: "Address", field: "WaterAccountContact.Address" },
            { headerName: "Secondary Address", field: "WaterAccountContact.SecondaryAddress" },
            { headerName: "City", field: "WaterAccountContact.City" },
            { headerName: "State", field: "WaterAccountContact.State" },
            { headerName: "Zip", field: "WaterAccountContact.ZipCode" },
            this.utilityFunctionsService.createBasicColumnDef("Communication Preference", "", {
                ValueGetter: (params) => (params.data.PrefersPhysicalCommunication ? "Physical Mail" : "Email"),
                UseCustomDropdownFilter: true,
            }),
            { headerName: "Users", valueGetter: (params) => params.data.Users?.map((x) => x.UserFullName) },
            this.utilityFunctionsService.createDecimalColumnDef("# of Users", "Users.length", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Parcels", "Parcels.length", { MaxDecimalPlacesToDisplay: 0 }),
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
        const dialogRef = this.dialogService.open(DeleteWaterAccountComponent, {
            data: {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterAccounts$.next();
            }
        });
    }

    public mergeModal(waterAccountID: number) {
        const dialogRef = this.dialogService.open(MergeWaterAccountsComponent, {
            data: {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterAccounts$.next();
            }
        });
    }

    public updateInfoModal(waterAccountID: number, rowNode: RowNode) {
        const dialogRef = this.dialogService.open(UpdateWaterAccountInfoComponent, {
            data: {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                rowNode.setData(result);
            }
        });
    }

    public updateParcelsModal(waterAccountID: number, rowNode: RowNode) {
        const dialogRef = this.dialogService.open(UpdateParcelsComponent, {
            data: {
                WaterAccountID: waterAccountID,
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result.success) {
                var temp = rowNode.data;
                temp.Parcels = result.result;
                rowNode.setData(temp);
            }
        });
    }

    public onLayerLoadStarted() {
        this.layerLoading = true;
    }

    public onLayerLoadFinished() {
        this.layerLoading = false;
    }
}
