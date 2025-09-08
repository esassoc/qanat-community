import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { BehaviorSubject, combineLatest, debounceTime, filter, map, Observable, shareReplay, Subscription, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "../../../../shared/components/page-header/page-header.component";
import { AsyncPipe } from "@angular/common";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ReportingPeriodSelectComponent } from "../../../../shared/components/reporting-period-select/reporting-period-select.component";
import { ReportingPeriodDto } from "src/app/shared/generated/model/reporting-period-dto";
import { SelectDropdownOption, FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { FormControl, FormGroup } from "@angular/forms";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { QanatMapComponent, QanatMapInitEvent } from "../../../../shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import { WaterMeasurementQualityAssuranceLayerComponent } from "../../../../shared/components/leaflet/layers/water-measurement-quality-assurance-layer/water-measurement-quality-assurance-layer.component";
import { ExternalMapLayerSimpleDto, ParcelDetailDto, WaterMeasurementQualityAssuranceDto, WellMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { GsaBoundariesComponent } from "../../../../shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ColDef, GridApi, GridOptions, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatGridComponent } from "../../../../shared/components/qanat-grid/qanat-grid.component";
import { DialogService } from "@ngneat/dialog";
import { RefreshWaterMeasurementCalculationsModalComponent } from "../../water-measurement-supply-and-usage-menu/refresh-water-measurement-calculations-modal/refresh-water-measurement-calculations-modal.component";
import { RecalculateOpenETSyncRasterDataModalComponent } from "src/app/shared/components/recalculate-open-et-sync-raster-data-modal/recalculate-open-et-sync-raster-data-modal.component";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { WellService } from "src/app/shared/generated/api/well.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { MigrateUsageLocationsModalComponent } from "./migrate-usage-locations-modal/migrate-usage-locations-modal.component";
import { BulkUpdateUsageLocationTypeModalComponent } from "./bulk-update-usage-location-type-modal/bulk-update-usage-location-type-modal.component";

@Component({
    selector: "usage-location-bulk-actions",
    imports: [
        AlertDisplayComponent,
        PageHeaderComponent,
        AsyncPipe,
        RouterModule,
        LoadingDirective,
        ReportingPeriodSelectComponent,
        FormFieldComponent,
        LoadingDirective,
        QanatMapComponent,
        WaterMeasurementQualityAssuranceLayerComponent,
        GsaBoundariesComponent,
        QanatGridComponent,
        ParcelLayerComponent,
        ZoneGroupLayerComponent,
        WellsLayerComponent,
    ],
    templateUrl: "./usage-location-bulk-actions.component.html",
    styleUrl: "./usage-location-bulk-actions.component.scss",
})
export class UsageLocationBulkActionsComponent implements OnInit, OnDestroy {
    public geography$: Observable<GeographyMinimalDto>;
    public currentUserGeographiesSelectOptions$: Observable<SelectDropdownOption[]>;
    public currentUserGeographies: GeographyMinimalDto[] = [];
    public reportingPeriodSelected$: BehaviorSubject<ReportingPeriodDto> = new BehaviorSubject<ReportingPeriodDto>(null);
    public selectedReportingPeriod: ReportingPeriodDto | null = null;
    public monthOptions$: Observable<SelectDropdownOption[]>;
    public monthsSelected$ = new BehaviorSubject<number[]>([0]); // Start with All
    public waterMeasurementTypeOptions$: Observable<SelectDropdownOption[]>;
    public waterMeasurementTypeOptions: SelectDropdownOption[] = [];
    public selectedWaterMeasurementType: SelectDropdownOption | null = null;
    public waterMeasurements$: Observable<WaterMeasurementQualityAssuranceDto[]>;
    public refreshWaterMeasurements$: BehaviorSubject<null> = new BehaviorSubject<null>(null);
    public wells$: Observable<WellMinimalDto[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerSimpleDto[]>;
    public colDefs$: Observable<ColDef[]>;

    public gridApi: GridApi;
    public gridOptions: GridOptions = {
        getRowId: (params) => params.data.UsageLocationID.toString(),
    } as GridOptions;

    public isLoading: boolean = true;

    public customRichTextTypeID = CustomRichTextTypeEnum.UsageLocationBulkActions;

    public filterFormGroup: FormGroup<{
        Geography: FormControl<SelectDropdownOption>;
        Months: FormControl<SelectDropdownOption[]>;
        WaterMeasurementType: FormControl<SelectDropdownOption>;
    }> = new FormGroup({
        Geography: new FormControl<SelectDropdownOption>(null, { nonNullable: true }),
        Months: new FormControl<SelectDropdownOption[]>([], { nonNullable: true }),
        WaterMeasurementType: new FormControl<SelectDropdownOption>(null, { nonNullable: true }),
    });

    public FormFieldType = FormFieldType;
    public subscriptions: Subscription[] = [];

    public selectedUsageLocationIDs: number[] = [];

    public constructor(
        private route: ActivatedRoute,
        private router: Router,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private waterMeasurementService: WaterMeasurementService,
        private wellService: WellService,
        private zoneGroupService: ZoneGroupService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {}

    public ngOnInit(): void {
        // Set up observables and initial data loading
        this.currentUserGeographiesSelectOptions$ = this.geographyService.listForCurrentUserGeography().pipe(
            tap((geographies) => {
                this.currentUserGeographies = geographies;
            }),
            map(
                (geographies) =>
                    geographies.map((g) => ({
                        Label: g.GeographyName,
                        Value: g.GeographyID,
                    })) as SelectDropdownOption[]
            )
        );

        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName).pipe(
                    tap((geography) => {
                        this.currentGeographyService.setCurrentGeography(geography);
                        this.filterFormGroup.get("Geography").setValue(geography.GeographyID as any, { emitEvent: false });
                    })
                );
            })
        );

        this.waterMeasurementTypeOptions$ = this.geography$.pipe(
            switchMap(
                (geography) => this.waterMeasurementTypeService.getWaterMeasurementTypesWaterMeasurementType(geography.GeographyID).pipe(map((types) => ({ geography, types }))) // preserve geography
            ),
            map(({ geography, types }) => {
                return {
                    geography,
                    options: types.map((type) => ({
                        Label: type.WaterMeasurementTypeName,
                        Value: type.WaterMeasurementTypeID,
                    })) as SelectDropdownOption[],
                };
            }),
            tap(({ geography, options }) => {
                this.filterFormGroup.get("WaterMeasurementType").setValue(geography.SourceOfRecordWaterMeasurementTypeID as any);
                this.waterMeasurementTypeOptions = options;
            }),
            map(({ options }) => options) // final stream is just the options for binding
        );

        this.monthOptions$ = this.reportingPeriodSelected$.pipe(
            filter((reportingPeriod) => !!reportingPeriod),
            map((reportingPeriod) => {
                let currentDate = new Date(reportingPeriod.StartDate);
                let currentMonth = currentDate.getUTCMonth();
                let result = [];
                for (let i = 0; i < 12; i++) {
                    let label = currentDate.toLocaleString("default", { month: "short", timeZone: "UTC" });
                    let month = { Label: label, Value: currentMonth + 1 } as SelectDropdownOption;
                    result.push(month);
                    currentDate.setMonth(currentDate.getMonth() + 1);
                    currentMonth = currentDate.getUTCMonth();
                }

                let allMonths = { Label: "All Months", Value: 0 } as SelectDropdownOption;
                return [allMonths, ...result];
            }),
            shareReplay(1)
        );

        this.colDefs$ = combineLatest({
            reportingPeriod: this.reportingPeriodSelected$,
            allMonths: this.monthOptions$, // sorted list based on start date
            selectedMonths: this.monthsSelected$,
        }).pipe(
            filter(({ reportingPeriod, selectedMonths }) => !!reportingPeriod && selectedMonths.length > 0),
            map(({ reportingPeriod, allMonths, selectedMonths }) => {
                const baseCols: ColDef[] = [
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location", "UsageLocationName"),
                    this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "UsageLocationArea"),
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationTypeName", { UseCustomDropdownFilter: true }),
                    this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumberAndName", "WaterAccountID", {
                        ValueGetter: (params) => ({
                            LinkValue: `${params.data.WaterAccountID}/water-budget`,
                            LinkDisplay: params.data.WaterAccountID ? params.data.WaterAccountNumberAndName : null,
                        }),
                        InRouterLink: "/water-accounts/",
                    }),
                    this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                        ValueGetter: (params) => ({
                            LinkValue: `${params.data.ParcelID}/detail`,
                            LinkDisplay: params.data.ParcelNumber,
                        }),
                        InRouterLink: "/parcels/",
                    }),
                    this.utilityFunctionsService.createDecimalColumnDef("Total (ac-ft/ac)", "SummedValueInFeet", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
                ];

                const dynamicMonthCols: ColDef[] = this.sortMonthsByStart(
                    reportingPeriod.StartDate ? new Date(reportingPeriod.StartDate).getUTCMonth() + 1 : 1,
                    this.normalizeMonths(selectedMonths as any, this.allMonthValues)
                ).map((monthNumber) => {
                    const label = allMonths.find((m) => m.Value === monthNumber)?.Label ?? `Month ${monthNumber}`;
                    return {
                        headerName: label,
                        field: `ReportedValueInFeetByMonth.${monthNumber}`,
                        cellStyle: { "justify-content": "flex-end" },
                        valueGetter: (params) => params.data.ReportedValueInFeetByMonth?.[monthNumber] ?? null,
                        type: "numericColumn",
                        filter: "agNumberColumnFilter",
                        width: 100,
                    };
                });

                let statusColumns: ColDef[] = [
                    this.utilityFunctionsService.createBasicColumnDef("Cover Crop Status", "CoverCropStatus", {
                        UseCustomDropdownFilter: true,
                    }),
                    this.utilityFunctionsService.createBasicColumnDef("Fallow Report Status", "FallowStatus", {
                        UseCustomDropdownFilter: true,
                    }),
                ];

                return [...baseCols, ...dynamicMonthCols, ...statusColumns];
            })
        );

        this.waterMeasurements$ = combineLatest({
            geography: this.geography$,
            reportingPeriod: this.reportingPeriodSelected$,
            waterMeasurementTypeID: this.filterFormGroup.get("WaterMeasurementType").valueChanges,
            months: this.monthsSelected$.pipe(
                debounceTime(1000),
                map((monthValues) => monthValues.map((m) => m)), // [0] or [1,2,3]
                map((values) => this.normalizeMonths(values as any, this.allMonthValues))
            ),
            refreshWaterMeasurements: this.refreshWaterMeasurements$, // trigger reload
        }).pipe(
            filter(({ geography, reportingPeriod, waterMeasurementTypeID }) => !!geography && !!reportingPeriod && !!waterMeasurementTypeID),
            tap(() => {
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap(({ geography, reportingPeriod, waterMeasurementTypeID, months }) => {
                return this.waterMeasurementService.listByGeographyIDReportingPeriodIDWaterMeasurementTypeIDAndMonthsWaterMeasurement(
                    geography.GeographyID,
                    reportingPeriod.ReportingPeriodID,
                    waterMeasurementTypeID as any,
                    months as any
                );
            }),
            tap(() => {
                this.isLoading = false;
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", false);
                }
            }),
            shareReplay(1)
        );

        // Set up subscriptions for form controls
        let geographySelectedSubscription = this.filterFormGroup
            .get("Geography")!
            .valueChanges.pipe(
                filter((geographyID) => !!geographyID) // Ensure geographyID is not null or undefined
            )
            .subscribe((geographyID) => {
                let geography = this.currentUserGeographies.find((g) => g.GeographyID === (geographyID as any));
                if (geography) {
                    this.currentGeographyService.setCurrentGeography(geography);
                    this.router.navigate(["/usage-location-bulk-actions", geography.GeographyName.toLowerCase()]);
                }
            });

        this.subscriptions.push(geographySelectedSubscription);

        this.wells$ = this.geography$.pipe(switchMap((geography) => this.wellService.listWellsWell(geography.GeographyID)));
        this.zoneGroups$ = this.geography$.pipe(switchMap((geography) => this.zoneGroupService.listZoneGroup(geography.GeographyID)));

        let previousSelectedMonths: number[] = [];
        let monthsSubscription = this.filterFormGroup.get("Months").valueChanges.subscribe((selected: any[]) => {
            let control = this.filterFormGroup.get("Months")!;
            let normalized = selected.map((s) => (typeof s === "object" ? s.Value : s));
            let currentSet = new Set(normalized);
            let prevSet = new Set(previousSelectedMonths);

            let hadAll = currentSet.has(0);
            let hasOthers = [...currentSet].some((v) => v !== 0);
            let justSelectedAll = !prevSet.has(0) && currentSet.has(0);

            let newVal: number[];

            if (hadAll && hasOthers && !justSelectedAll) {
                // All was selected and another month was added — remove All
                newVal = normalized.filter((v) => v !== 0);
            } else if (justSelectedAll || normalized.length === 0) {
                // All was just selected, or everything was cleared — select All
                newVal = [0];
            } else {
                // Standard selection
                newVal = [...normalized];
            }

            let startMonth = this.selectedReportingPeriod?.StartDate ? new Date(this.selectedReportingPeriod.StartDate).getUTCMonth() + 1 : 1;
            let getSortIndex = (month: number) => (month === 0 ? -1 : (month - startMonth + 12) % 12);
            let sorted = [...newVal].sort((a, b) => getSortIndex(a) - getSortIndex(b));

            // Only update the form control if the sorted value is different
            let prevSorted = [...(control.value ?? [])].map((v) => (typeof v === "object" ? v.Value : v)).sort((a, b) => a - b);

            let isDifferent = sorted.length !== prevSorted.length || sorted.some((v, i) => v !== prevSorted[i]);

            if (isDifferent) {
                control.setValue(sorted as any, { emitEvent: false });
            }

            this.monthsSelected$.next(sorted);
            previousSelectedMonths = sorted;
            control.setValue(sorted as any, { emitEvent: false }); // Update the form control without emitting an event
        });

        this.filterFormGroup.get("Months")!.setValue([0] as any);
        this.monthsSelected$.next([0]);
        this.subscriptions.push(monthsSubscription);

        let waterMeasurementTypeSubscription = combineLatest({
            waterMeasurementType: this.filterFormGroup.get("WaterMeasurementType").valueChanges.pipe(
                filter((value) => !!value),
                tap((selectedType) => {
                    let option = this.waterMeasurementTypeOptions.find((option) => option.Value === selectedType);
                    this.selectedWaterMeasurementType = option;
                })
            ),
            waterMeasurementTypes: this.waterMeasurementTypeOptions$,
        }).subscribe();

        this.subscriptions.push(waterMeasurementTypeSubscription);
    }

    private allMonthValues: number[] = Array.from({ length: 12 }, (_, i) => i + 1);
    private normalizeMonths(selected: number[], allMonths: number[]): number[] {
        return selected.includes(0) ? allMonths : selected;
    }
    private sortMonthsByStart(startMonth: number, months: number[]): number[] {
        let getSortIndex = (month: number) => (month === 0 ? -1 : (month - startMonth + 12) % 12);
        return [...months].sort((a, b) => getSortIndex(a) - getSortIndex(b));
    }

    ngOnDestroy(): void {
        if (this.subscriptions) {
            this.subscriptions.forEach((subscription) => subscription.unsubscribe());
            this.subscriptions = [];
        }
    }

    onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        this.reportingPeriodSelected$.next(selectedReportingPeriod);
        this.selectedReportingPeriod = selectedReportingPeriod;
    }

    onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridApi.sizeColumnsToFit();
    }

    onGridSelectionChanged($event: SelectionChangedEvent<any, any>) {
        let selectedRows = this.gridApi.getSelectedRows();
        this.selectedUsageLocationIDs = selectedRows.map((row) => row.UsageLocationID);
        this.gridApi.onSortChanged();
    }

    bulkUpdateUsageLocationTypes(geography: GeographyMinimalDto) {
        const dialogRef = this.dialogService.open(BulkUpdateUsageLocationTypeModalComponent, {
            data: {
                GeographyID: geography.GeographyID,
                ReportingPeriod: this.selectedReportingPeriod,
                UsageLocationIDs: this.selectedUsageLocationIDs,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterMeasurements$.next(null);
            }
        });
    }

    migrateUsageLocations(geography: GeographyMinimalDto) {
        const dialogRef = this.dialogService.open(MigrateUsageLocationsModalComponent, {
            data: {
                GeographyID: geography.GeographyID,
                ReportingPeriod: this.selectedReportingPeriod,
                UsageLocationIDs: this.selectedUsageLocationIDs,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterMeasurements$.next(null);
                document.querySelector("html")?.scrollTo({ top: 0, behavior: "smooth" });
            }
        });
    }

    refreshWaterMeasurementCalculations(geography: GeographyMinimalDto) {
        const dialogRef = this.dialogService.open(RefreshWaterMeasurementCalculationsModalComponent, {
            data: {
                GeographyID: geography.GeographyID,
                GeographyName: geography.GeographyName,
                GeographyStartYear: null,
                UsageLocationIDs: this.selectedUsageLocationIDs,
                ReportingPeriodID: this.selectedReportingPeriod.ReportingPeriodID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterMeasurements$.next(null);
                document.querySelector("html")?.scrollTo({ top: 0, behavior: "smooth" });
            }
        });
    }

    recalculateOpenETRasterData(parcel: ParcelDetailDto): void {
        const dialogRef = this.dialogService.open(RecalculateOpenETSyncRasterDataModalComponent, {
            data: {
                GeographyID: parcel.GeographyID,
                ParcelID: parcel.ParcelID,
                UsageLocationIDs: this.selectedUsageLocationIDs,
                ReportingPeriodID: this.selectedReportingPeriod.ReportingPeriodID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;
    public layerLoading: boolean = true;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public onUsageLocationFeatureSelected(usageLocationID: number) {
        // Toggle selection on the grid.
        if (this.gridApi) {
            const rowNode = this.gridApi.getRowNode(usageLocationID.toString());
            if (rowNode) {
                rowNode.setSelected(!rowNode.isSelected());
                this.gridApi.onSortChanged();
            }
        }
    }
}
