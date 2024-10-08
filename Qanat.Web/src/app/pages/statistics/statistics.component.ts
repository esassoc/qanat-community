import { Component, OnDestroy, OnInit } from "@angular/core";
import { forkJoin, Subscription } from "rxjs";
import { ParcelSupplyService } from "src/app/shared/generated/api/parcel-supply.service";
import { WaterTypeService } from "src/app/shared/generated/api/water-type.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { MonthlyUsageSummaryDto } from "src/app/shared/generated/model/monthly-usage-summary-dto";
import { WaterAccountParcelWaterMeasurementDto } from "src/app/shared/generated/model/water-account-parcel-water-measurement-dto";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { WaterTypeSupplyDto } from "src/app/shared/generated/model/water-type-supply-dto";
import { GroupByPipe } from "src/app/shared/pipes/group-by.pipe";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { WaterTypeFieldDefinitionComponent } from "../../shared/components/water-type-field-definition/water-type-field-definition.component";
import { RouterLink } from "@angular/router";
import { VegaMonthlyUsageChartComponent } from "../../shared/components/vega/vega-monthly-usage-chart/vega-monthly-usage-chart.component";
import { VegaCumulativeUsageChartComponent } from "../../shared/components/vega/vega-cumulative-usage-chart/vega-cumulative-usage-chart.component";
import { ReportingPeriodSelectComponent } from "../../shared/components/reporting-period-select/reporting-period-select.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { NgIf, NgClass, NgFor, DecimalPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "statistics",
    templateUrl: "./statistics.component.html",
    styleUrls: ["./statistics.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        NgIf,
        AlertDisplayComponent,
        ReportingPeriodSelectComponent,
        NgClass,
        VegaCumulativeUsageChartComponent,
        VegaMonthlyUsageChartComponent,
        RouterLink,
        NgFor,
        WaterTypeFieldDefinitionComponent,
        DecimalPipe,
        DatePipe,
        LoadingDirective,
    ],
})
export class StatisticsComponent implements OnInit, OnDestroy {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    public geographyID: number;
    public geographySlug: string;
    public geography: GeographyDto;

    public selectedYear: number;
    public usageToDate: number;
    public totalSupply: number;
    public barStyling: string;
    public waterTypes: WaterTypeSimpleDto[];
    public waterTypesSupply: WaterTypeSupplyDto[];
    public waterSupplyBar: string;
    public currentAvailable: number;
    public mostRecentSupplyDate: string;
    public mostRecentUsageDate: string;
    public mostRecentEffectiveDate: string;
    public totalAcreage: number;
    public showCumulativeWaterUsageChart: boolean = true;
    public showAcresFeet: boolean = false;
    public acresFeetUnits: string = "ac-ft";
    public acresFeetAcreUnits: string = "ac-ft/ac";

    public waterAccountParcelWaterMeasurements: WaterAccountParcelWaterMeasurementDto[];
    public sourceOfRecordWaterMeasurements: WaterAccountParcelWaterMeasurementDto[];
    public monthlyUsageSummaries: MonthlyUsageSummaryDto[];
    public isLoading: boolean = true;

    constructor(
        private parcelSupplyService: ParcelSupplyService,
        private waterTypeService: WaterTypeService,
        private selectedGeographyService: SelectedGeographyService,
        private sumPipe: SumPipe,
        private groupByPipe: GroupByPipe
    ) {}

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geography = geography;
            this.geographyID = geography.GeographyID;
            this.geographySlug = geography.GeographyName.replace(" ", "-").toLowerCase();
            this.selectedYear = geography.DefaultDisplayYear;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    getDataForGeographyID(geographyID: number): void {
        forkJoin({
            parcelData: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearMonthlyUsageSummaryGet(geographyID, this.selectedYear),
            waterTypeData: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearSupplyGet(geographyID, this.selectedYear),
            waterTypes: this.waterTypeService.geographiesGeographyIDWaterTypesActiveGet(geographyID),
            mostRecentEffectiveDates: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearRecentEffectiveDatesGet(geographyID, this.selectedYear),
        }).subscribe(({ parcelData, waterTypeData, waterTypes, mostRecentEffectiveDates }) => {
            this.waterAccountParcelWaterMeasurements = parcelData;
            this.waterTypes = waterTypes;
            this.waterTypesSupply = waterTypeData;
            this.mostRecentSupplyDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentSupplyEffectiveDate);
            this.mostRecentUsageDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentUsageEffectiveDate);
            this.mostRecentEffectiveDate = this.mostRecentSupplyDate > this.mostRecentUsageDate ? this.mostRecentSupplyDate : this.mostRecentUsageDate;
            this.setSupplyAndUsageValues();
            this.isLoading = false;
        });
    }

    getSourceOfRecordWaterMeasurementRollups(measurements: WaterAccountParcelWaterMeasurementDto[]): MonthlyUsageSummaryDto[] {
        const rollupData = [];
        const usageEntityArea = this.sumPipe.transform(measurements, "UsageEntityArea");

        const groupedByEffectiveDate = this.groupByPipe.transform(
            measurements.flatMap((x) => x.WaterMeasurementMonthlyValues),
            "EffectiveDate"
        );
        Object.keys(groupedByEffectiveDate).forEach((effectiveDate) => {
            const monthlyUsageSummary = new MonthlyUsageSummaryDto();
            const group = groupedByEffectiveDate[effectiveDate];
            monthlyUsageSummary.EffectiveDate = effectiveDate;
            monthlyUsageSummary.CurrentUsageAmount = this.sumPipe.transform(group, "CurrentUsageAmount");
            monthlyUsageSummary.CurrentUsageAmountDepth = this.sumPipe.transform(group, "CurrentUsageAmountDepth");
            monthlyUsageSummary.AverageUsageAmount = this.sumPipe.transform(group, "AverageUsageAmount");
            monthlyUsageSummary.AverageUsageAmountDepth = this.sumPipe.transform(group, "AverageUsageAmountDepth");
            monthlyUsageSummary.CurrentCumulativeUsageAmount = this.sumPipe.transform(group, "CurrentCumulativeUsageAmount");
            monthlyUsageSummary.CurrentCumulativeUsageAmountDepth = this.sumPipe.transform(group, "CurrentCumulativeUsageAmount") / usageEntityArea;
            monthlyUsageSummary.AverageCumulativeUsageAmount = this.sumPipe.transform(group, "AverageCumulativeUsageAmount");
            monthlyUsageSummary.AverageCumulativeUsageAmountDepth = this.sumPipe.transform(group, "AverageCumulativeUsageAmount") / usageEntityArea;
            monthlyUsageSummary.TotalSupply = this.sumPipe.transform(this.waterTypesSupply, "TotalSupply");
            monthlyUsageSummary.TotalSupplyDepth = this.convertToAcresFeetAcre(monthlyUsageSummary.TotalSupply);
            rollupData.push(monthlyUsageSummary);
        });
        return rollupData;
    }

    setSupplyAndUsageValues() {
        this.sourceOfRecordWaterMeasurements = this.waterAccountParcelWaterMeasurements.filter(
            (x) => x.WaterMeasurementTypeID === this.geography.SourceOfRecordWaterMeasurementType.WaterMeasurementTypeID
        );

        if (this.sourceOfRecordWaterMeasurements.length > 0) {
            this.usageToDate = this.sumPipe.transform(this.sourceOfRecordWaterMeasurements, "WaterMeasurementTotalValue");
            this.totalAcreage = this.sumPipe.transform(this.sourceOfRecordWaterMeasurements, "UsageEntityArea");
        } else {
            this.usageToDate = null;
        }

        if (this.waterTypesSupply.length > 0) {
            this.totalSupply = this.sumPipe.transform(this.waterTypesSupply, "TotalSupply");
        } else {
            this.totalSupply = null;
        }

        this.currentAvailable = this.totalSupply - this.usageToDate;
        this.barStyling = "width: " + this.getPercentageOfWaterUsed() + "%";

        this.monthlyUsageSummaries = this.getSourceOfRecordWaterMeasurementRollups(this.sourceOfRecordWaterMeasurements);
    }

    getPercentageOfWaterUsed() {
        if (this.totalSupply > 0) {
            return ((this.usageToDate / this.totalSupply) * 100).toFixed(2);
        }
        return 0;
    }

    getWaterTypeUsage(waterType: WaterTypeSimpleDto) {
        const waterUse = this.waterTypesSupply.filter((x) => x.WaterTypeID == waterType.WaterTypeID);
        if (waterUse.length == 1) {
            return waterUse[0].TotalSupply.toFixed(2);
        }
        return 0;
    }
    setWaterSupplyBar(waterTypeTotalUse) {
        return "width: " + (waterTypeTotalUse / this.totalSupply) * 100 + "%";
    }

    updateDashboardForSelectedYear(selectedYear: number) {
        this.selectedYear = selectedYear;

        forkJoin({
            parcelData: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearMonthlyUsageSummaryGet(this.geographyID, this.selectedYear),
            waterTypeData: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearSupplyGet(this.geographyID, this.selectedYear),
            mostRecentEffectiveDates: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearRecentEffectiveDatesGet(this.geographyID, this.selectedYear),
        }).subscribe(({ parcelData, waterTypeData, mostRecentEffectiveDates }) => {
            this.waterAccountParcelWaterMeasurements = parcelData;
            this.waterTypesSupply = waterTypeData;
            this.mostRecentSupplyDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentSupplyEffectiveDate);
            this.mostRecentUsageDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentUsageEffectiveDate);
            this.mostRecentEffectiveDate = this.mostRecentSupplyDate > this.mostRecentUsageDate ? this.mostRecentSupplyDate : this.mostRecentUsageDate;
            this.setSupplyAndUsageValues();
        });
    }
    getDateFromString(dateString: string) {
        if (dateString != null) return dateString.substring(0, 10);
    }
    convertToAcresFeetAcre(num: number) {
        return num / this.totalAcreage;
    }
    changeUnits(temp) {
        this.showAcresFeet = temp;
    }
    getShowAcresFeet() {
        return this.showAcresFeet;
    }

    public updateShowCumulativeWaterUsageChart(value: boolean) {
        this.showCumulativeWaterUsageChart = value;
    }
}
