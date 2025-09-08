import { AfterViewInit, ChangeDetectorRef, Component } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { BehaviorSubject, combineLatest, filter, Observable, of, shareReplay, switchMap, tap } from "rxjs";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    ExternalMapLayerSimpleDto,
    GeographyMinimalDto,
    ReportingPeriodDto,
    UsageLocationByReportingPeriodIndexGridDto,
    UserDto,
    ZoneGroupMinimalDto,
} from "src/app/shared/generated/model/models";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { Map, layerControl } from "leaflet";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AsyncPipe, CommonModule, DecimalPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UsageLocationLayerComponent } from "../../../shared/components/leaflet/layers/usage-location-layer/usage-location-layer.component";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { UsageLocationByGeographyService } from "src/app/shared/generated/api/usage-location-by-geography.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { NgSelectModule } from "@ng-select/ng-select";

@Component({
    selector: "usage-location-list",
    imports: [
        PageHeaderComponent,
        LoadingDirective,
        AsyncPipe,
        FormsModule,
        CommonModule,
        LoadingDirective,
        WaterDashboardNavComponent,
        GsaBoundariesComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        AlertDisplayComponent,
        HybridMapGridComponent,
        UsageLocationLayerComponent,
        NgSelectModule,
    ],
    templateUrl: "./usage-location-list.component.html",
    styleUrl: "./usage-location-list.component.scss",
})
export class UsageLocationListComponent implements AfterViewInit {
    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public geography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;

    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public currentReportingPeriodID$: BehaviorSubject<number> = new BehaviorSubject(null);
    public currentReportingPeriodID: number;

    public gridApi: GridApi;
    public columnDefs$: Observable<ColDef[]>;

    public usageLocations$: Observable<UsageLocationByReportingPeriodIndexGridDto[]>;
    public usageLocationIDs: number[] = [];

    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerSimpleDto[]>;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public currentUserHasOverallPermission: boolean = false;
    public currentUserHasNoGeographies: boolean = false;

    public selectedUsageLocationID: number;

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardUsageLocations;
    public isLoading: boolean = true;
    public layerLoading: boolean = true;
    public firstLoad: boolean = true;

    constructor(
        private geographyService: GeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private usageLocationByGeographyService: UsageLocationByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private authenticationService: AuthenticationService,
        private utilityFunctionService: UtilityFunctionsService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private decimalPipe: DecimalPipe,
        private cdr: ChangeDetectorRef
    ) {}

    ngAfterViewInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
                this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(user, PermissionEnum.WaterAccountRights, RightsEnum.Read);
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

        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                if (geography?.GeographyID == this.currentGeography?.GeographyID) {
                    return;
                }

                this.currentGeography = geography;
                this.onGeographySelected(geography);
            })
        );

        this.reportingPeriods$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                let defaultReportingPeriod = reportingPeriods.find((x) => x.IsDefault);
                if (!defaultReportingPeriod) {
                    defaultReportingPeriod = reportingPeriods[0];
                }

                this.currentReportingPeriodID = defaultReportingPeriod?.ReportingPeriodID;
                this.currentReportingPeriodID$.next(this.currentReportingPeriodID);
            })
        );

        this.usageLocations$ = combineLatest({ geography: this.geography$, reportingPeriodID: this.currentReportingPeriodID$ }).pipe(
            filter(({ geography, reportingPeriodID }) => !!geography && !!reportingPeriodID),
            tap(() => {
                this.isLoading = true;
                this.layerLoading = true;
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap(({ geography, reportingPeriodID }) => {
                return this.usageLocationByGeographyService.listByGeographyAndReportingPeriodUsageLocationByGeography(geography.GeographyID, reportingPeriodID);
            }),
            tap((usageLocations) => {
                this.isLoading = false;
                this.firstLoad = false;
                this.usageLocationIDs = usageLocations.map((ul) => ul.UsageLocationID);

                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", false);

                    setTimeout(() => {
                        this.gridApi.sizeColumnsToFit();
                    });
                }
            }),
            shareReplay(1)
        );

        this.columnDefs$ = combineLatest({ user: this.currentUser$, geography: this.geography$ }).pipe(
            switchMap(({ user, geography }) => {
                const colDefs = this.createColumnDefs();
                return of(colDefs);
            })
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

    onReportingPeriodSelected($event: number) {
        this.currentReportingPeriodID$.next($event);
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;

        setTimeout(() => {
            this.gridApi.sizeColumnsToFit();
        });
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        setTimeout(() => {
            this.cdr.detectChanges();
        }, 1000);
    }

    public onSelectedUsageLocationIDChanged(selectedUsageLocationID): number {
        if (this.selectedUsageLocationID == selectedUsageLocationID) {
            return;
        }

        this.selectedUsageLocationID = selectedUsageLocationID;
        return this.selectedUsageLocationID;
    }

    private createColumnDefs(): ColDef[] {
        let colDefs: ColDef[] = [
            this.utilityFunctionService.createBasicColumnDef("Usage Location", "Name", { FieldDefinitionType: "UsageLocation" }),
            this.utilityFunctionService.createDecimalColumnDef("Area (Acres)", "Area"),
            this.utilityFunctionService.createBasicColumnDef("Usage Location Type", "UsageLocationTypeName", {
                CustomDropdownFilterField: "UsageLocationTypeName",
            }),
            this.utilityFunctionService.createLinkColumnDef("Water Account", "WaterAccountNumberAndName", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
            }),

            this.utilityFunctionService.createLinkColumnDef("Parcel", "ParcelNumber", "ParcelID", {
                InRouterLink: "/parcels/",
            }),
        ];

        return colDefs;
    }

    public onLayerLoadStarted() {
        this.layerLoading = true;
    }

    public onLayerLoadFinished() {
        this.layerLoading = false;
    }
}
