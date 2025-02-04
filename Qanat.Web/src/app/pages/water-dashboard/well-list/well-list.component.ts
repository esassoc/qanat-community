import { ChangeDetectorRef, Component, OnInit, OnDestroy } from "@angular/core";
import { BehaviorSubject, combineLatest, map, of, switchMap, tap } from "rxjs";
import { Observable } from "rxjs/internal/Observable";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NgIf, AsyncPipe, CommonModule } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { AgGridAngular } from "ag-grid-angular";
import { Map, layerControl } from "leaflet";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { WellMinimalDto } from "src/app/shared/generated/model/well-minimal-dto";
import { WellService } from "src/app/shared/generated/api/well.service";
import { FormsModule } from "@angular/forms";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { OpenedWellPopupEvent, WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { RouterLink } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/zone-group-minimal-dto";
import { ExternalMapLayerDto } from "src/app/shared/generated/model/external-map-layer-dto";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "well-list",
    templateUrl: "./well-list.component.html",
    styleUrls: ["./well-list.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        LoadingDirective,
        NgIf,
        RouterLink,
        IconComponent,
        AsyncPipe,
        WaterDashboardNavComponent,
        QanatGridComponent,
        QanatGridHeaderComponent,
        QanatMapComponent,
        WellsLayerComponent,
        CommonModule,
        FormsModule,
        ParcelLayerComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        GsaBoundariesComponent,
        AlertDisplayComponent,
    ],
})
export class WellListComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public geography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;

    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public columnDefs$: Observable<ColDef<WellMinimalDto>[]>;

    public wells$: Observable<WellMinimalDto[]>;

    public parcelIDs$: Observable<number[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerDto[]>;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public currentUserHasOverallPermission: boolean = false;
    public currentUserHasNoGeographies: boolean = false;

    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public gridRefReady$ = new BehaviorSubject<void>(null);

    public selectedPanel: "Grid" | "Hybrid" | "Map" = "Hybrid";

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;
    public selectedWellID: number;

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardWells;
    public isLoading: boolean = true;
    public firstLoad: boolean = true;

    constructor(
        private authenticationService: AuthenticationService,
        private wellService: WellService,
        public parcelService: ParcelService,
        public parcelByGeographyService: ParcelByGeographyService,
        public zoneGroupService: ZoneGroupService,
        public externalMapLayerService: ExternalMapLayerService,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                this.currentUser = currentUser;
                this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read);
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

        this.columnDefs$ = combineLatest({ currentUser: this.currentUser$, geography: this.geography$ }).pipe(
            switchMap(({ currentUser, geography }) => {
                const columnDefs = this.createColumnDefs();
                return of(columnDefs);
            })
        );

        this.currentUserGeographies$ = this.geographyService.geographiesCurrentUserGet().pipe(
            tap((geographies) => {
                this.currentUserHasNoGeographies = geographies.length == 0;
            })
        );

        this.parcelIDs$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelByGeographyService.geographiesGeographyIDParcelsCurrentUserGet(geography.GeographyID);
            }),
            map((parcels) => parcels.map((x) => x.ParcelID))
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

        this.wells$ = combineLatest({ currentUser: this.currentUser$, geography: this.geography$, _: this.gridRefReady$ }).pipe(
            tap(() => (this.isLoading = true)),
            switchMap(({ currentUser, geography }) => {
                return this.wellService.geographiesGeographyIDWellsCurrentUserGet(geography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
                this.firstLoad = false;
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
            this.selectedWellID = undefined;
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
        this.gridRefReady$.next();
    }

    public onGridSelectionChanged() {
        const selectedNodes = this.gridApi.getSelectedNodes();
        this.selectedWellID = selectedNodes.length > 0 ? selectedNodes[0].data.WellID : null;
    }

    public onMapSelectionChanged(event: OpenedWellPopupEvent) {
        if (this.selectedWellID == event.wellID) return;

        this.selectedWellID = event.wellID;
        this.gridApi.forEachNode((node, index) => {
            if (node.data.WellID == this.selectedWellID) {
                node.setSelected(true, true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }

    private createColumnDefs(): ColDef<WellMinimalDto>[] {
        const columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Well ID", "WellID", "WellID", { InRouterLink: "/wells/" }),
            this.utilityFunctionsService.createBasicColumnDef("Well Name", "WellName"),
            this.utilityFunctionsService.createLinkColumnDef("Default APN", "ParcelNumber", "ParcelID", { InRouterLink: "/parcels/" }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Irrigates", "IrrigatesParcels", "ParcelID", "ParcelNumber", {
                InRouterLink: "/parcels",
                MaxWidth: 300,
            }),
            this.utilityFunctionsService.createBasicColumnDef("County Well Permit #", "CountyWellPermitNumber", { FieldDefinitionType: "CountyWellPermitNo" }),
            this.utilityFunctionsService.createBasicColumnDef("State WCR #", "StateWCRNumber", { FieldDefinitionType: "StateWCRNo" }),
            this.utilityFunctionsService.createDateColumnDef("DateDrilled", "DateDrilled", "M/d/yyyy", { FieldDefinitionType: "DateDrilled" }),
            this.utilityFunctionsService.createDecimalColumnDef("Well Depth", "WellDepth", { FieldDefinitionType: "WellDepth" }),
            this.utilityFunctionsService.createBasicColumnDef("Well Status", "WellStatusDisplayName", {
                FieldDefinitionType: "WellStatus",
                CustomDropdownFilterField: "WellStatusDisplayName",
            }),
            this.utilityFunctionsService.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctionsService.createLatLonColumnDef("Longitude", "Longitude"),
            this.utilityFunctionsService.createBasicColumnDef("Notes", "Notes"),
        ];

        return columnDefs;
    }
}
