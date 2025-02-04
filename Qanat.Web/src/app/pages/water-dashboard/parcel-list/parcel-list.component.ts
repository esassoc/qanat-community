import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { BehaviorSubject, Observable, combineLatest, of } from "rxjs";
import { filter, switchMap, tap } from "rxjs/operators";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AsyncPipe, CommonModule } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { ParcelIndexGridDto } from "src/app/shared/generated/model/parcel-index-grid-dto";
import { WaterAccountIndexGridDto } from "src/app/shared/generated/model/water-account-index-grid-dto";
import { AgGridAngular } from "ag-grid-angular";
import { Map, layerControl } from "leaflet";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { CustomAttributeTypeEnum } from "src/app/shared/generated/enum/custom-attribute-type-enum";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/zone-group-minimal-dto";
import { CustomAttributeSimpleDto } from "src/app/shared/generated/model/custom-attribute-simple-dto";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { FormsModule } from "@angular/forms";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ExternalMapLayerDto } from "src/app/shared/generated/model/external-map-layer-dto";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { GeographyDto, GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";

@Component({
    selector: "parcel-list",
    templateUrl: "./parcel-list.component.html",
    styleUrls: ["./parcel-list.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        LoadingDirective,
        CommonModule,
        FormsModule,
        AsyncPipe,
        WaterDashboardNavComponent,
        QanatGridHeaderComponent,
        QanatGridComponent,
        QanatMapComponent,
        GsaBoundariesComponent,
        ParcelLayerComponent,
        IconComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        AlertDisplayComponent,
    ],
})
export class ParcelListComponent implements OnInit, OnDestroy {
    public geography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;

    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerDto[]>;
    public customAttributes$: Observable<CustomAttributeSimpleDto[]>;

    public columnDefs$: Observable<ColDef<WaterAccountIndexGridDto>[]>;

    public refreshGeographyData$ = new BehaviorSubject(null);
    public parcels$: Observable<ParcelIndexGridDto[]>;
    public parcelIDs$: Observable<number[]>;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public currentUserHasOverallPermission: boolean = false;
    public currentUserHasNoGeographies: boolean = false;

    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public selectedPanel: "Grid" | "Hybrid" | "Map" = "Hybrid";
    public selectedParcelID: number;

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;

    public zoneGroups: ZoneGroupMinimalDto[];
    public externalMapLayers: ExternalMapLayerDto[];
    public customAttributes: CustomAttributeSimpleDto[];

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardParcels;
    public isLoading: boolean = true;
    public firstLoad: boolean = true;

    constructor(
        private parcelByGeographyService: ParcelByGeographyService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private customAttributeService: CustomAttributeService,
        private authenticationService: AuthenticationService,
        private utilityFunctionsService: UtilityFunctionsService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
                this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(user, PermissionEnum.ParcelRights, RightsEnum.Read);
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

        this.currentUserGeographies$ = combineLatest({ currentUser: this.currentUser$ }).pipe(
            switchMap(({ currentUser }) => {
                return this.geographyService.geographiesCurrentUserGet();
            }),
            tap((geographies) => {
                this.currentUserHasNoGeographies = geographies.length == 0;
            })
        );

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geography.GeographyID);
            })
        );

        this.columnDefs$ = combineLatest({ user: this.currentUser$, geography: this.geography$, zoneGroups: this.zoneGroups$ }).pipe(
            switchMap(({ user, geography, zoneGroups }) => {
                const colDefs = this.createColumnDefs(user, geography, zoneGroups);
                return of(colDefs);
            })
        );

        this.externalMapLayers$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.externalMapLayerService.geographiesGeographyIDExternalMapLayersGet(geography.GeographyID);
            })
        );

        this.customAttributes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.customAttributeService.geographiesGeographyIDCustomAttributesCustomAttributeTypeIDGet(geography.GeographyID, CustomAttributeTypeEnum.Parcel);
            })
        );

        this.parcels$ = combineLatest({ currentUser: this.currentUser$, geography: this.geography$, _: this.refreshGeographyData$ }).pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(({ currentUser, geography, _ }) => {
                return this.parcelByGeographyService.geographiesGeographyIDParcelsCurrentUserGet(geography.GeographyID);
            }),
            tap((parcels) => {
                this.isLoading = false;
                this.firstLoad = false;
            })
        );

        this.parcelIDs$ = this.parcels$.pipe(
            switchMap((parcels) => {
                return of(parcels.map((x) => x.ParcelID));
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public toggleSelectedPanel(selectedPanel: "Grid" | "Hybrid" | "Map") {
        this.selectedPanel = selectedPanel;

        setTimeout(() => {
            this.map.invalidateSize(true);

            if (this.layerControl && this.bounds) {
                this.map.fitBounds(this.bounds);
            }
        }, 300);

        // if no map is visible, turn of grid selection
        if (selectedPanel == "Grid") {
            this.gridApi.setGridOption("rowSelection", null);
            this.selectedParcelID = undefined;
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
            this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read, this.currentGeography.GeographyID);

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
        this.selectedParcelID = selectedNodes.length > 0 ? selectedNodes[0].data.ParcelID : null;
    }

    public onMapSelectionChanged(selectedParcelID: number) {
        if (this.selectedParcelID == selectedParcelID) return;

        this.selectedParcelID = selectedParcelID;
        this.gridApi.forEachNode((node, index) => {
            if (node.data.ParcelID == selectedParcelID) {
                node.setSelected(true, true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }

    private createColumnDefs(user: UserDto, geography: GeographyDto, zoneGroups: ZoneGroupMinimalDto[]) {
        const userHasOverAllPermission = this.authenticationService.hasOverallPermission(user, PermissionEnum.ParcelRights, RightsEnum.Read);
        const userHasGeographyPermission = this.authenticationService.hasGeographyPermission(
            this.currentUser,
            PermissionEnum.ParcelRights,
            RightsEnum.Read,
            this.currentGeography.GeographyID
        );
        const userHasPermission = userHasOverAllPermission || userHasGeographyPermission;

        const columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                InRouterLink: "/parcels/",
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.ParcelID}/detail`, LinkDisplay: params.data.ParcelNumber };
                },
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (Acres)", "ParcelArea"),
            this.utilityFunctionsService.createLinkColumnDef("Account #", "WaterAccountNumber", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Water Account Name", "WaterAccountName"),
            this.utilityFunctionsService.createMultiLinkColumnDef("Wells on Parcel", "WellsOnParcel", "WellID", "WellID", {
                InRouterLink: "../wells",
                MaxWidth: 300,
            }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Irrigated By", "IrrigatedByWells", "WellID", "WellID", {
                InRouterLink: "../wells",
                MaxWidth: 300,
            }),
            { headerName: "Owner Name", field: "OwnerName" },
            { headerName: "Owner Address", field: "OwnerAddress" },
            this.utilityFunctionsService.createBasicColumnDef("Parcel Status", "ParcelStatusDisplayName", {
                FieldDefinitionType: "ParcelStatus",
                CustomDropdownFilterField: "ParcelStatusDisplayName",
                Hide: !userHasPermission,
            }),
        ];

        zoneGroups.forEach((zoneGroup) => {
            columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "ZoneIDs", !userHasPermission));
        });

        if (!this.customAttributes) {
            return columnDefs;
        }

        columnDefs.push(...this.utilityFunctionsService.createCustomAttributeColumnDefs(this.customAttributes, !userHasPermission));

        return columnDefs;
    }
}
