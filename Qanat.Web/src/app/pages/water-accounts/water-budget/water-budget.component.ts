import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, filter, map, Observable, of, shareReplay, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import {
    AllocationPlanMinimalDto,
    ExternalMapLayerSimpleDto,
    GeographySimpleDto,
    MonthlyUsageSummaryDto,
    MostRecentEffectiveDatesDto,
    ParcelMinimalDto,
    ReportingPeriodDto,
    UsageLocationDto,
    WaterAccountBudgetStatDto,
    WaterAccountContactDto,
    WaterAccountMinimalDto,
    WaterAccountParcelWaterMeasurementDto,
    WaterAccountWaterTypeMonthlySupplyDto,
    ZoneGroupMinimalDto,
} from "src/app/shared/generated/model/models";
import { GeographyEnum } from "src/app/shared/models/enums/geography.enum";
import { GroupByPipe } from "src/app/shared/pipes/group-by.pipe";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { NgClass, DecimalPipe, PercentPipe, DatePipe, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { ButtonGroupComponent } from "src/app/shared/components/button-group/button-group.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";
import { VegaCumulativeUsageChartComponent } from "src/app/shared/components/vega/vega-cumulative-usage-chart/vega-cumulative-usage-chart.component";
import { VegaMonthlyUsageChartComponent } from "src/app/shared/components/vega/vega-monthly-usage-chart/vega-monthly-usage-chart.component";
import { WaterSupplyTypeComponent } from "src/app/shared/components/water-supply-type/water-supply-type.component";
import { WaterAccountParcelWaterMeasurementsGridComponent } from "./components/water-account-parcel-water-measurements-grid/water-account-parcel-water-measurements-grid.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { Map, layerControl } from "leaflet";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { UsageLocationLayerComponent } from "src/app/shared/components/leaflet/layers/usage-location-layer/usage-location-layer.component";
import { UsageLocationByWaterAccountService } from "src/app/shared/generated/api/usage-location-by-water-account.service";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterAccountContactService } from "src/app/shared/generated/api/water-account-contact.service";

@Component({
    selector: "water-budget",
    templateUrl: "./water-budget.component.html",
    styleUrls: ["./water-budget.component.scss"],
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        ReportingPeriodSelectComponent,
        ButtonGroupComponent,
        NgClass,
        IconComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        VegaCumulativeUsageChartComponent,
        VegaMonthlyUsageChartComponent,
        WaterAccountParcelWaterMeasurementsGridComponent,
        WaterSupplyTypeComponent,
        DecimalPipe,
        PercentPipe,
        DatePipe,
        QanatMapComponent,
        ParcelLayerComponent,
        GsaBoundariesComponent,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        UsageLocationLayerComponent,
        SumPipe,
        LoadingDirective,
    ],
})
export class WaterBudgetComponent implements OnInit {
    public currentWaterAccount$: Observable<WaterAccountMinimalDto>;
    public waterAccountContacts$: Observable<WaterAccountContactDto>;
    private selectedYearSubject = new BehaviorSubject<number | null>(null);
    public selectedYear$ = this.selectedYearSubject.asObservable();
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerSimpleDto[]>;
    public waterTypeSupplies$: Observable<WaterAccountWaterTypeMonthlySupplyDto[]>;
    public parcels$: Observable<ParcelMinimalDto[]>;
    public waterAccountParcelWaterMeasurements$: Observable<WaterAccountParcelWaterMeasurementDto[]>;
    public usageLocations$: Observable<UsageLocationDto[]>;
    public usageLocationsByReportingPeriod$: Observable<UsageLocationDto[]>;
    public waterAccountBudgetStats$: Observable<WaterAccountBudgetStatDto>;
    public mostRecentEffectiveDates$: Observable<MostRecentEffectiveDatesDto>;
    public selectedReportingPeriodSubject = new BehaviorSubject<ReportingPeriodDto | null>(null);
    public selectedReportingPeriod$: Observable<ReportingPeriodDto> = this.selectedReportingPeriodSubject.asObservable();
    public selectedReportingPeriod: ReportingPeriodDto;

    public currentGeographySlug: string;

    public parcelIDs: number[];

    public totalSupply: number;
    public usageToDate: number;
    public usageBar: string;
    public totalAcreage: number;
    public currentAvailable: number;

    public totalET: number;
    public totalEffectivePrecip: number;

    public showAcresFeet: boolean = false;
    public acresFeetUnits: string = "ac-ft";
    public acresFeetAcreUnits: string = "ac-ft/ac";

    public showCumulativeWaterUsageChart = true;
    public showWaterAccountRollup = true;

    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;

    public monthlyUsageSummaries: MonthlyUsageSummaryDto[];

    constructor(
        private route: ActivatedRoute,
        private waterAccountService: WaterAccountService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private usageLocationByWaterAccountService: UsageLocationByWaterAccountService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private sumPipe: SumPipe,
        private groupByPipe: GroupByPipe
    ) {}

    ngOnInit(): void {
        this.currentWaterAccount$ = this.route.params.pipe(
            switchMap((params) => {
                return this.waterAccountService.getByIDWaterAccount(params[routeParams.waterAccountID]);
            }),
            tap((waterAccount) => {
                this.currentGeographySlug = waterAccount.Geography.GeographyName.replace(" ", "-").toLowerCase();
            }),
            shareReplay(1)
        );

        this.waterAccountContacts$ = this.currentWaterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.waterAccountService.getWaterAccountContactByWaterAccountIDWaterAccount(waterAccount.WaterAccountID);
            })
        );

        this.usageLocations$ = this.currentWaterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.usageLocationByWaterAccountService.listUsageLocationByWaterAccount(waterAccount.Geography.GeographyID, waterAccount.WaterAccountID);
            })
        );

        this.usageLocationsByReportingPeriod$ = combineLatest({ usageLocations: this.usageLocations$, selectedReportingPeriod: this.selectedReportingPeriod$ }).pipe(
            switchMap(({ usageLocations, selectedReportingPeriod }) => {
                return of(usageLocations.filter((x) => x.ReportingPeriod.ReportingPeriodID == selectedReportingPeriod.ReportingPeriodID));
            })
        );

        this.zoneGroups$ = this.currentWaterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.zoneGroupService.listZoneGroup(waterAccount.Geography.GeographyID);
            })
        );

        this.externalMapLayers$ = this.currentWaterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.externalMapLayerService.getExternalMapLayer(waterAccount.Geography.GeographyID);
            })
        );

        this.mostRecentEffectiveDates$ = combineLatest([this.currentWaterAccount$, this.selectedYear$]).pipe(
            filter(([waterAccount, selectedYear]) => selectedYear != null),
            switchMap(([waterAccount, selectedYear]) => {
                return this.waterAccountService.getMostRecentSupplyAndUsageDateByAccountIDWaterAccount(waterAccount.WaterAccountID, selectedYear);
            })
        );

        this.parcels$ = combineLatest([this.currentWaterAccount$, this.selectedYear$]).pipe(
            switchMap(([waterAccount, selectedYear]) => {
                return this.waterAccountParcelByWaterAccountService.getCurrentParcelsFromAccountIDWaterAccountParcelByWaterAccount(waterAccount.WaterAccountID, selectedYear);
            }),
            tap((parcels) => {
                this.parcelIDs = parcels.length > 0 ? parcels.map((x) => x.ParcelID) : [-1]; // in this case we want no hits
            })
        );

        this.waterAccountBudgetStats$ = combineLatest([this.currentWaterAccount$, this.selectedYear$]).pipe(
            switchMap(([waterAccount, selectedYear]) => {
                return waterAccount.Geography.ShowSupplyOnWaterBudgetComponent
                    ? of(null)
                    : this.waterAccountService.getWaterMeasurementStatsForWaterBudgetWaterAccount(waterAccount.WaterAccountID, selectedYear);
            })
        );

        this.waterTypeSupplies$ = combineLatest([this.currentWaterAccount$, this.selectedYear$]).pipe(
            switchMap(([waterAccount, selectedYear]) => {
                return this.waterAccountService.getWaterTypeSupplyFromAccountIDWaterAccount(waterAccount.WaterAccountID, selectedYear);
            }),
            tap((data) => {
                if (data.length > 0) {
                    this.totalSupply = this.sumPipe.transform(data, "TotalSupply");
                } else {
                    this.totalSupply = null;
                }
            }),
            shareReplay(1)
        );

        this.waterAccountParcelWaterMeasurements$ = combineLatest([this.currentWaterAccount$, this.selectedYear$, this.waterTypeSupplies$]).pipe(
            switchMap(([waterAccount, selectedYear, _]) => {
                return combineLatest({
                    waterAccountParcelWaterMeasurements: this.waterAccountService.getMonthlyUsageSummaryForWaterAccountAndReportingPeriodWaterAccount(
                        waterAccount.WaterAccountID,
                        selectedYear
                    ),
                    geography: of(waterAccount.Geography),
                });
            }),
            tap((data) => {
                this.setSupplyAndUsageValues(data.geography, data.waterAccountParcelWaterMeasurements);
            }),
            map((data) => {
                return data.waterAccountParcelWaterMeasurements;
            }),
            shareReplay(1)
        );
    }

    public onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        let endDate = new Date(selectedReportingPeriod.EndDate);
        let endYear = endDate.getUTCFullYear();
        this.selectedYearSubject.next(endYear);

        this.selectedReportingPeriodSubject.next(selectedReportingPeriod);
        this.selectedReportingPeriod = selectedReportingPeriod;
    }

    private getSourceOfRecordWaterMeasurementRollups(measurements: WaterAccountParcelWaterMeasurementDto[], parcelArea: number): MonthlyUsageSummaryDto[] {
        const rollupData = [];
        const groupedByEffectiveDate = this.groupByPipe.transform(
            measurements.flatMap((x) => x.WaterMeasurementMonthlyValues),
            "EffectiveDate"
        );

        Object.keys(groupedByEffectiveDate).forEach((effectiveDate) => {
            const monthlyUsageSummary = new MonthlyUsageSummaryDto();
            const group = groupedByEffectiveDate[effectiveDate];

            monthlyUsageSummary.EffectiveDate = effectiveDate;

            const currentUsageAmount = this.sumPipe.transform(group, "CurrentUsageAmount");
            monthlyUsageSummary.CurrentUsageAmount = currentUsageAmount;
            monthlyUsageSummary.CurrentUsageAmountDepth = currentUsageAmount / parcelArea;

            const averageUsageAmount = this.sumPipe.transform(group, "AverageUsageAmount");
            monthlyUsageSummary.AverageUsageAmount = averageUsageAmount;
            monthlyUsageSummary.AverageUsageAmountDepth = averageUsageAmount / parcelArea;

            const currentCumulativeUsageAmount = group.map((value) => value.CurrentCumulativeUsageAmount).reduce((a, b) => (a == null ? null : a + b));
            monthlyUsageSummary.CurrentCumulativeUsageAmount = currentCumulativeUsageAmount == null ? null : currentCumulativeUsageAmount > 0 ? currentCumulativeUsageAmount : 0;
            monthlyUsageSummary.CurrentCumulativeUsageAmountDepth =
                currentCumulativeUsageAmount == null ? null : currentCumulativeUsageAmount > 0 ? currentCumulativeUsageAmount / parcelArea : 0;

            const averageCumulativeUsageAmount = group.map((value) => value.AverageCumulativeUsageAmount).reduce((a, b) => (a == null ? null : a + b));
            monthlyUsageSummary.AverageCumulativeUsageAmount = averageCumulativeUsageAmount == null ? null : averageCumulativeUsageAmount > 0 ? averageCumulativeUsageAmount : 0;
            monthlyUsageSummary.AverageCumulativeUsageAmountDepth =
                averageCumulativeUsageAmount == null ? null : averageCumulativeUsageAmount > 0 ? averageCumulativeUsageAmount / parcelArea : 0;

            rollupData.push(monthlyUsageSummary);
        });
        return rollupData;
    }

    private setSupplyAndUsageValues(geography: GeographySimpleDto, waterAccountParcelWaterMeasurements: WaterAccountParcelWaterMeasurementDto[]) {
        const sourceOfRecordWaterMeasurements = waterAccountParcelWaterMeasurements.filter((x) => x.WaterMeasurementTypeID === geography.SourceOfRecordWaterMeasurementTypeID);

        const parcelArea = this.getParcelAreaFromWaterMeasurements(sourceOfRecordWaterMeasurements);

        if (sourceOfRecordWaterMeasurements.length > 0) {
            this.usageToDate = this.sumPipe.transform(sourceOfRecordWaterMeasurements, "WaterMeasurementTotalValue");
            this.totalAcreage = parcelArea;
        } else {
            this.usageToDate = null;
        }

        this.currentAvailable = this.totalSupply - this.usageToDate;

        this.monthlyUsageSummaries = this.getSourceOfRecordWaterMeasurementRollups(sourceOfRecordWaterMeasurements, parcelArea);

        if (geography.GeographyID == GeographyEnum.etsgsa) {
            const etValues = waterAccountParcelWaterMeasurements.filter((x) => x.WaterMeasurementTypeName === "Land IQ ETa");
            this.totalET = this.sumPipe.transform(etValues, "WaterMeasurementTotalValue");
            const effectivePrecipValues = waterAccountParcelWaterMeasurements.filter((x) => x.WaterMeasurementTypeName === "Effective Precip");
            this.totalEffectivePrecip = this.sumPipe.transform(effectivePrecipValues, "WaterMeasurementTotalValue");
        }
    }

    private getParcelAreaFromWaterMeasurements(sourceOfRecordWaterMeasurements: WaterAccountParcelWaterMeasurementDto[]) {
        var parcelArea = 0;
        const groupedByParcelID = this.groupByPipe.transform(sourceOfRecordWaterMeasurements, "ParcelID");
        Object.keys(groupedByParcelID).forEach((parcelID) => {
            const group = groupedByParcelID[parcelID];
            parcelArea += group[0].ParcelArea;
        });
        return parcelArea;
    }

    public getPercentageOfWaterUsed(): number {
        if (this.totalSupply > 0) {
            return this.usageToDate / this.totalSupply;
        }
        return 0;
    }

    public convertToAcresFeetAcre(num: number) {
        return num / this.totalAcreage;
    }

    changeUnits(temp: boolean) {
        this.showAcresFeet = temp;
    }

    public getShowAcresFeet() {
        return this.showAcresFeet;
    }

    public updateShowCumulativeWaterUsageChart(value: boolean) {
        this.showCumulativeWaterUsageChart = value;
    }

    public updateShowWaterAccountRollup(value: boolean) {
        this.showWaterAccountRollup = value;
    }

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public layerLoading: boolean = true;
    onLayerLoadStarted() {
        this.layerLoading = true;
    }

    onLayerLoadFinished() {
        this.layerLoading = false;
    }
}
