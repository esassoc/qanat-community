import { Component, OnInit } from "@angular/core";
import { BehaviorSubject, combineLatest, Observable, of, switchMap, map, tap, concat, shareReplay } from "rxjs";
import { MonthlyUsageSummaryDto } from "src/app/shared/generated/model/monthly-usage-summary-dto";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { WaterTypeSupplyDto } from "src/app/shared/generated/model/water-type-supply-dto";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { WaterTypeFieldDefinitionComponent } from "../../../shared/components/water-type-field-definition/water-type-field-definition.component";
import { RouterLink } from "@angular/router";
import { VegaMonthlyUsageChartComponent } from "../../../shared/components/vega/vega-monthly-usage-chart/vega-monthly-usage-chart.component";
import { VegaCumulativeUsageChartComponent } from "../../../shared/components/vega/vega-cumulative-usage-chart/vega-cumulative-usage-chart.component";
import { ReportingPeriodSelectComponent } from "../../../shared/components/reporting-period-select/reporting-period-select.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { NgClass, DecimalPipe, DatePipe, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { GeographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto } from "src/app/shared/generated/model/geography-source-of-record-water-measurement-type-monthly-usage-summary-dto";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto, MostRecentEffectiveDatesDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "statistics",
    templateUrl: "./statistics.component.html",
    styleUrls: ["./statistics.component.scss"],
    imports: [
        PageHeaderComponent,
        AlertDisplayComponent,
        ReportingPeriodSelectComponent,
        NgClass,
        VegaCumulativeUsageChartComponent,
        VegaMonthlyUsageChartComponent,
        RouterLink,
        WaterTypeFieldDefinitionComponent,
        DecimalPipe,
        LoadingDirective,
        AsyncPipe,
        DatePipe,
    ]
})
export class StatisticsComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    private selectedYearSubject = new BehaviorSubject<number | null>(null);
    public selectedYear$ = this.selectedYearSubject.asObservable();

    public waterTypes$: Observable<WaterTypeSimpleDto[]>;
    public mostRecentEffectiveDates$: Observable<MostRecentEffectiveDatesDto>;
    public usageData$: Observable<GeographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto>;
    public supplyData$: Observable<WaterTypeSupplyDto[]>;

    public viewModel$: Observable<StatisticsViewModel>;
    public showCumulativeWaterUsageChart = true;
    public showAcresFeet = false;
    public acresFeetUnits = "ac-ft";
    public acresFeetAcreUnits = "ac-ft/ac";
    public isLoading = true;

    constructor(
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private sumPipe: SumPipe
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.waterTypes$ = this.geography$.pipe(switchMap((geography) => this.waterTypeByGeographyService.getActiveWaterTypesWaterTypeByGeography(geography.GeographyID)));
        this.viewModel$ = combineLatest([this.geography$, this.selectedYear$]).pipe(
            switchMap(([geography, selectedYear]) => {
                const loading$ = of(null); // clears view immediately

                if (!geography || !selectedYear) {
                    return loading$; // show loading if either is missing
                }

                const data$ = combineLatest([
                    this.parcelSupplyByGeographyService.getMostRecentSupplyAndUsageDateParcelSupplyByGeography(geography.GeographyID, selectedYear).pipe(shareReplay(1)),
                    this.parcelSupplyByGeographyService
                        .getMonthlyUsageSummaryForGeographyAndReportingPeriodParcelSupplyByGeography(geography.GeographyID, selectedYear)
                        .pipe(shareReplay(1)),
                    this.parcelSupplyByGeographyService.getSupplyOfWaterTypesParcelSupplyByGeography(geography.GeographyID, selectedYear).pipe(shareReplay(1)),
                ]).pipe(
                    map(([mostRecentDates, usageData, supplyData]) => {
                        const totalSupply = this.sumPipe.transform(supplyData, "TotalSupply") || 0;
                        const usageToDate = usageData.WaterMeasurementTotalValue || 0;
                        const totalAcreage = usageData.TotalParcelArea || 0;

                        const monthlyUsageSummaries = usageData.WaterMeasurementMonthlyValues.map((monthlyUsageSummary) => ({
                            ...monthlyUsageSummary,
                            CurrentCumulativeUsageAmountDepth: monthlyUsageSummary.CurrentCumulativeUsageAmount / totalAcreage,
                            AverageCumulativeUsageAmountDepth: monthlyUsageSummary.AverageCumulativeUsageAmount / totalAcreage,
                            TotalSupply: totalSupply,
                            TotalSupplyDepth: this.convertToAcresFeetAcre(totalSupply, totalAcreage),
                        }));

                        const currentAvailable = totalSupply - usageToDate;
                        const barStyling = `width: ${(usageToDate / totalSupply) * 100}%`;

                        const mostRecentSupplyDate = this.getDateFromString(mostRecentDates.MostRecentSupplyEffectiveDate);
                        const mostRecentUsageDate = this.getDateFromString(mostRecentDates.MostRecentUsageEffectiveDate);
                        const mostRecentEffectiveDate = mostRecentSupplyDate > mostRecentUsageDate ? mostRecentSupplyDate : mostRecentUsageDate;

                        return {
                            mostRecentSupplyDate,
                            mostRecentUsageDate,
                            mostRecentEffectiveDate,
                            usageToDate,
                            totalSupply,
                            currentAvailable,
                            barStyling,
                            monthlyUsageSummaries,
                            supplyData,
                            totalAcreage,
                        } as StatisticsViewModel;
                    })
                );

                // emit null first to trigger @else block before actual data comes in
                return concat(loading$, data$);
            }),
            tap((vm) => {
                this.isLoading = vm === null;
            })
        );
    }

    onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        const endDate = new Date(selectedReportingPeriod.EndDate);
        const selectedYear = endDate.getUTCFullYear();
        this.selectedYearSubject.next(selectedYear);
    }

    getPercentageOfWaterUsed(viewModel: StatisticsViewModel) {
        if (viewModel.totalSupply > 0) {
            return ((viewModel.usageToDate / viewModel.totalSupply) * 100).toFixed(2);
        }
    }

    getWaterTypeUsage(viewModel: StatisticsViewModel, waterType: WaterTypeSimpleDto): number {
        const waterUse = viewModel.supplyData.filter((x) => x.WaterTypeID === waterType.WaterTypeID);
        return waterUse.length === 1 ? waterUse[0].TotalSupply : 0;
    }

    setWaterSupplyBar(viewModel: StatisticsViewModel, waterTypeTotalUse: number) {
        return "width: " + (waterTypeTotalUse / viewModel.totalSupply) * 100 + "%";
    }

    getDateFromString(dateString: string) {
        return dateString?.substring(0, 10);
    }

    convertToAcresFeetAcre(num: number, acreage: number) {
        if (!acreage || acreage === 0) {
            return null;
        }
        return num / acreage;
    }

    changeUnits(useAcreFeet: boolean) {
        this.showAcresFeet = useAcreFeet;
    }

    getShowAcresFeet() {
        return this.showAcresFeet;
    }

    updateShowCumulativeWaterUsageChart(value: boolean) {
        this.showCumulativeWaterUsageChart = value;
    }
}

class StatisticsViewModel {
    mostRecentSupplyDate: string;
    mostRecentUsageDate: string;
    mostRecentEffectiveDate: string;
    usageToDate: number;
    totalAcreage: number;
    totalSupply: number;
    supplyData: WaterTypeSupplyDto[];
    currentAvailable: number;
    barStyling: string;
    monthlyUsageSummaries: MonthlyUsageSummaryDto[];
}
