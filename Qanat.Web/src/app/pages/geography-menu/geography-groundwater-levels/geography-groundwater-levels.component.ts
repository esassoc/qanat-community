import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { BoundingBoxDto } from "src/app/shared/generated/model/bounding-box-dto";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { MonitoringWellService } from "src/app/shared/generated/api/monitoring-well.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { MonitoringWellMeasurementChartComponent } from "src/app/shared/components/monitoring-wells/modal/monitoring-well-measurement-chart/monitoring-well-measurement-chart.component";
import { ButtonRendererComponent } from "src/app/shared/components/ag-grid/button-renderer/button-renderer.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MonitoringWellDataDto } from "src/app/shared/generated/model/monitoring-well-data-dto";
import { GeographyEnum } from "src/app/shared/models/enums/geography.enum";
import { GeographyWithBoundingBoxDto } from "src/app/shared/generated/model/geography-with-bounding-box-dto";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { MonitoringWellsLayerComponent } from "src/app/shared/components/leaflet/layers/monitoring-wells-layer/monitoring-wells-layer.component";
import * as L from "leaflet";
import { Observable, switchMap, tap } from "rxjs";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "geography-groundwater-levels",
    templateUrl: "./geography-groundwater-levels.component.html",
    styleUrls: ["./geography-groundwater-levels.component.scss"],
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        ButtonLoadingDirective,
        CustomRichTextComponent,
        QanatGridComponent,
        QanatMapComponent,
        GsaBoundariesComponent,
        MonitoringWellsLayerComponent,
        AsyncPipe,
    ]
})
export class GeographyGroundwaterLevelsComponent implements OnInit, OnDestroy {
    @ViewChild("monitoringWellsGrid") monitoringWellsGrid: AgGridAngular;

    private currentUser: UserDto;

    public geography$: Observable<GeographyWithBoundingBoxDto>;
    public geography: GeographyWithBoundingBoxDto;

    public customRichTextTypeID = CustomRichTextTypeEnum.GeographyWaterLevels;
    public monitoringWellsCustomRichTextTypeID = CustomRichTextTypeEnum.MonitoringWellsGrid;
    public isLoading = true;
    public columnDefs: ColDef[];

    public monitoringWellData$: Observable<MonitoringWellDataDto[]>;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public boundingBox: BoundingBoxDto;
    public overlayLayers: { [key: string]: any } = {};

    public isTriggeringYoloWellJob: boolean = false;

    public _highlightedMonitoringWellID: number;
    private gridApi: GridApi;
    public set highlightedMonitoringWellID(value: number) {
        if (value != this._highlightedMonitoringWellID) {
            this._highlightedMonitoringWellID = value;
            this.selectHighlightedMonitoringWellIDRowNode();
        }
    }

    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private monitoringWellService: MonitoringWellService,
        private publicService: PublicService,
        private cdr: ChangeDetectorRef,
        private utilityFunctionsService: UtilityFunctionsService,
        private authenticationService: AuthenticationService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        // used to add some page-specific css in the shame/index.css file
        document.body.classList.add("geography-groundwater-levels");

        this.authenticationService.getCurrentUser().subscribe((currentUser) => (this.currentUser = currentUser));
        this.createColumnDefs();
        this.geography$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(() => {
                let geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
                return this.publicService.getGeographyByNameWithBoundingBoxPublic(geographyName);
            }),
            tap((geography) => {
                this.isLoading = false;
                this.geography = geography;
                this.boundingBox = geography.BoundingBox;
                this.fitGeographyBounds();
            })
        );

        this.monitoringWellData$ = this.geography$.pipe(
            tap(() => {
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap((geography) => {
                return this.publicService.getAllMonitoringWellsForGeographyForGridPublic(geography.GeographyID);
            }),
            tap(() => {
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", false);
                }
            })
        );
    }

    ngOnDestroy(): void {
        document.body.classList.remove("geography-groundwater-levels");
        this.cdr.detach();
    }

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.fitGeographyBounds();
        this.cdr.detectChanges();
    }

    onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    fitGeographyBounds() {
        if (!this.mapIsReady) return;
        if (!this.geography) return;

        if (this.geography.BoundingBox?.Left && this.geography.BoundingBox.Right && this.geography.BoundingBox.Top && this.geography.BoundingBox.Bottom) {
            this.map.fitBounds([
                [this.geography.BoundingBox.Bottom, this.geography.BoundingBox.Left],
                [this.geography.BoundingBox.Top, this.geography.BoundingBox.Right],
            ]);
        }
    }

    createColumnDefs() {
        this.columnDefs = [
            {
                headerName: "",
                field: "",
                valueGetter: (params) => {
                    return {
                        ActionName: "View Data",
                        CssClasses: "btn btn-primary-outline btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.openWellMeasurementModal(params.data.GeographyID, params.data.SiteCode, params.data.MonitoringWellName),
                    };
                },
                cellRenderer: ButtonRendererComponent,
                filter: false,
            },
            {
                headerName: "Site Code",
                field: "SiteCode",
            },
            { headerName: "Well Name", field: "MonitoringWellName" },
            this.utilityFunctionsService.createBasicColumnDef("Well Source", "MonitoringWellSourceTypeDisplayName", {
                CustomDropdownFilterField: "MonitoringWellSourceTypeDisplayName",
            }),
            this.utilityFunctionsService.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctionsService.createLatLonColumnDef("Longitude", "Longitude"),
            this.utilityFunctionsService.createDecimalColumnDef("# of Measurements", "NumberOfMeasurements", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("Earliest Elevation (ft)", "EarliestMeasurement"),
            this.utilityFunctionsService.createDateColumnDef("Earliest Measurement Date", "EarliestMeasurementDate", "short"),
            this.utilityFunctionsService.createDecimalColumnDef("Last Measurement (Depth)", "LastMeasurement"),
            this.utilityFunctionsService.createDateColumnDef("Last Measurement Date", "LastMeasurementDate", "short"),
        ];
    }

    public openWellMeasurementModal(geographyID: number, siteCode: string, monitoringWellName: string) {
        const dialogRef = this.dialogService.open(MonitoringWellMeasurementChartComponent, {
            data: {
                GeographyID: geographyID,
                SiteCode: siteCode,
                MonitoringWellName: monitoringWellName,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }

    public selectHighlightedMonitoringWellIDRowNode() {
        this.monitoringWellsGrid.api.forEachNodeAfterFilterAndSort((rowNode, index) => {
            if (rowNode.data.MonitoringWellID == this.highlightedMonitoringWellID) {
                rowNode.setSelected(true);
                this.monitoringWellsGrid.api.ensureIndexVisible(index, "top");
            }
        });
    }

    public isYoloGeography(): boolean {
        return this.geography.GeographyID == GeographyEnum.yolo;
    }

    public currentUserHasMonitoringWellUpdatePermissions() {
        if (!this.currentUser) return false;
        if (AuthorizationHelper.isSystemAdministrator(this.currentUser)) return true;

        return this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.MonitoringWellRights, RightsEnum.Update, this.geography.GeographyID);
    }

    public retrieveYoloWells() {
        if (!this.isYoloGeography() || !this.currentUserHasMonitoringWellUpdatePermissions()) return;

        this.confirmService
            .confirm({
                title: "Pull Scada Well Data",
                message: "Are you sure you want to pull Yolo WRID Scada Well data? This action will run in the background, and may take some time to complete.",
                buttonTextYes: "Pull",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.isTriggeringYoloWellJob = true;
                    this.monitoringWellService.retrieveYoloWRIDWellsAndMeasurementsMonitoringWell().subscribe(
                        () => {
                            this.isTriggeringYoloWellJob = false;
                            this.alertService.pushAlert(new Alert("Yolo WRID Scada Wells Job has been triggered!", AlertContext.Success, true));
                        },
                        (error) => {
                            this.isTriggeringYoloWellJob = false;
                        }
                    );
                }
            });
    }
}
