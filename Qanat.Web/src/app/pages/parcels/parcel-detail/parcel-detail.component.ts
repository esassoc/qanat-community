import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef } from "ag-grid-community";
import { Observable, Subscription } from "rxjs";
import { tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import {
    AllocationPlanManageDto,
    ExternalMapLayerSimpleDto,
    GeographyDto,
    ParcelDetailDto,
    ParcelSupplyDetailDto,
    UsageLocationDto,
    UserDto,
} from "src/app/shared/generated/model/models";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomDropdownFilterComponent } from "src/app/shared/components/custom-dropdown-filter/custom-dropdown-filter.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AsyncPipe, DecimalPipe } from "@angular/common";
import { DashboardMenu } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { WaterAccountTitleComponent } from "src/app/shared/components/water-account/water-account-title/water-account-title.component";
import { WaterAccountParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/water-account-parcels-layer/water-account-parcels-layer.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { UsageLocationLayerComponent } from "src/app/shared/components/leaflet/layers/usage-location-layer/usage-location-layer.component";
import { AllocationPlanTableComponent } from "src/app/shared/components/allocation-plan-table/allocation-plan-table.component";
import { ButtonGroupComponent } from "src/app/shared/components/button-group/button-group.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { VegaParcelUsageChartComponent } from "src/app/shared/components/vega/vega-parcel-usage-chart/vega-parcel-usage-chart.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ParcelEditZoneAssignmentsModalComponent } from "src/app/shared/components/parcel/modals/parcel-edit-zone-assignments-modal/parcel-edit-zone-assignments-modal.component";
import { ParcelUpdateOwnershipInfoModalComponent } from "src/app/shared/components/parcel/modals/parcel-update-ownership-info-modal/parcel-update-ownership-info-modal.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { ParcelAcreOverrideModalComponent } from "src/app/shared/components/parcel/modals/parcel-acre-override-modal/parcel-acre-override-modal.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { UsageLocationService } from "src/app/shared/generated/api/usage-location.service";
import { ParcelModifyParcelStatusModalComponent } from "src/app/shared/components/parcel/modals/parcel-modify-parcel-status-modal/parcel-modify-parcel-status-modal.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "parcel-detail",
    templateUrl: "./parcel-detail.component.html",
    styleUrls: ["./parcel-detail.component.scss"],
    imports: [
        RouterLink,
        IconComponent,
        PageHeaderComponent,
        WaterAccountTitleComponent,
        QanatMapComponent,
        WaterAccountParcelsLayerComponent,
        GsaBoundariesComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        UsageLocationLayerComponent,
        QanatGridComponent,
        VegaParcelUsageChartComponent,
        ButtonGroupComponent,
        AllocationPlanTableComponent,
        NoteComponent,
        LoadingDirective,
        AsyncPipe,
        DecimalPipe,
    ],
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
    @ViewChild("parcelSupplyGrid") parcelSupplyGrid: AgGridAngular;

    private parcelIDSubscription: Subscription = Subscription.EMPTY;
    public parcelSupplyGridColumnDefs: ColDef[];
    public usageLocationColumnDefs: ColDef<UsageLocationDto>[];
    public supplyTypeColDefInsertIndex = 3;
    public parcelID: number;
    public geographyID: number;

    public parcel$: Observable<ParcelDetailDto>;
    public parcelSupplies$: Observable<ParcelSupplyDetailDto[]>;
    public geography$: Observable<GeographyDto>;
    public usageLocations$: Observable<UsageLocationDto[]>;

    public currentUser$: Observable<UserDto>;
    public currentUserIsAdminOrWaterManager: boolean = false;

    public userHasOneGeography = false;

    public allocationPlans$: Observable<AllocationPlanManageDto[]>;
    public selectedAllocationPlan: AllocationPlanManageDto;
    public showAllocationPlan: boolean;
    public dashboardMenu: DashboardMenu;
    public externalMapLayers$: Observable<ExternalMapLayerSimpleDto[]>;

    constructor(
        private route: ActivatedRoute,
        private parcelService: ParcelService,
        private parcelByGeographyService: ParcelByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private externalMapLayerService: ExternalMapLayerService,
        private geographyService: GeographyService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef,
        private usageLocationService: UsageLocationService,
        private dialogService: DialogService
    ) {}

    ngOnDestroy() {
        this.parcelIDSubscription.unsubscribe();
        this.cdr.detach();
    }

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                this.parcelIDSubscription = this.route.paramMap.subscribe((paramMap) => {
                    this.mapIsReady = false;
                    this.parcelID = parseInt(paramMap.get(routeParams.parcelID));
                    this.parcelSupplies$ = this.parcelService.getAllSupplyEntriesByParcelIDParcel(this.parcelID);
                    this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcelID).pipe(
                        tap((parcel) => {
                            this.geographyID = parcel.GeographyID;

                            this.geography$ = this.geographyService.getGeographyByIDGeography(parcel.GeographyID).pipe(
                                tap((geography) => {
                                    this.currentUserIsAdminOrWaterManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(currentUser, geography.GeographyID);
                                })
                            );

                            this.usageLocations$ = this.usageLocationService.listUsageLocation(parcel.GeographyID, parcel.ParcelID).pipe(
                                tap((x) => {
                                    this.usageLocationColumnDefs = [
                                        this.utilityFunctionsService.createBasicColumnDef("Name", "Name"),
                                        this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriod.Name"),
                                        this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "Area"),
                                        this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationType.Name", {
                                            CustomDropdownFilterField: "UsageLocationType.Name",
                                        }),
                                        {
                                            headerName: "Crops",
                                            field: "Crops",
                                            valueGetter: (params) => {
                                                return params.data.Crops.map((x) => x.Name);
                                            },
                                            valueFormatter: (params) => {
                                                return params.value.join(", ");
                                            },
                                            filter: CustomDropdownFilterComponent,
                                            filterParams: {
                                                useDownloadDisplayValue: false,
                                                columnContainsMultipleValues: true,
                                            },
                                        },
                                    ];
                                })
                            );

                            this.allocationPlans$ = this.parcelByGeographyService.getParcelAllocationPlansByParcelIDParcelByGeography(parcel.GeographyID, parcel.ParcelID).pipe(
                                tap((x) => {
                                    this.selectedAllocationPlan = x[0];

                                    this.showAllocationPlan = x[0]?.GeographyAllocationPlanConfiguration.IsVisibleToLandowners ?? false;
                                })
                            );
                            this.externalMapLayers$ = this.externalMapLayerService.getActiveExternalMapLayersExternalMapLayer(parcel.GeographyID);
                            this.initializeSupplyGrid();

                            //MK 12/2/2024 -- Without this timeout, the map will not render correctly when swapping between parcels.
                            setTimeout(() => {
                                this.mapIsReady = true;
                                this.cdr.markForCheck();
                            });
                        })
                    );
                });
            })
        );
    }

    getZoneColorStyle(color) {
        return "background-color: " + color;
    }

    initializeSupplyGrid() {
        this.parcelSupplyGridColumnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Supply Type", "WaterType.WaterTypeName", {
                FieldDefinitionType: "SupplyType",
                CustomDropdownFilterField: "WaterType.WaterTypeName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Effective Date", "EffectiveDate", "M/d/yyyy", { FieldDefinitionType: "EffectiveDate" }),
            this.utilityFunctionsService.createDateColumnDef("Transaction Date", "TransactionDate", "short"),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Volume (ac-ft)", "TransactionAmount"),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Depth (ac-ft / ac)", "", {
                ValueGetter: (params) => this.utilityFunctionsService.customDecimalValueGetter(params.data.TransactionAmount / params.data.Parcel.ParcelArea),
            }),
            {
                headerName: "Comment",
                field: "UserComment",
                filter: false,
                sortable: false,
            },
        ];
    }

    editZoneAssignments(): void {
        const dialogRef = this.dialogService.open(ParcelEditZoneAssignmentsModalComponent, {
            data: { ParcelID: this.parcelID, GeographyID: this.geographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcelID);
            }
        });
    }

    updateOwnershipInfo(): void {
        const dialogRef = this.dialogService.open(ParcelUpdateOwnershipInfoModalComponent, {
            data: { ParcelID: this.parcelID, GeographyID: this.geographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcelID);
            }
        });
    }

    editAcres(): void {
        const dialogRef = this.dialogService.open(ParcelAcreOverrideModalComponent, {
            data: { ParcelID: this.parcelID, GeographyID: this.geographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcelID);
            }
        });
    }

    modifyParcelStatus(): void {
        const dialogRef = this.dialogService.open(ParcelModifyParcelStatusModalComponent, {
            data: { ParcelID: this.parcelID, GeographyID: this.geographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcelID);
            }
        });
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
    }
}
