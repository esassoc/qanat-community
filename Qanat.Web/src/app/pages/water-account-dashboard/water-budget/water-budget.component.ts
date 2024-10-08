import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { forkJoin, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { ParcelSupplyService } from "src/app/shared/generated/api/parcel-supply.service";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { UsageEntityService } from "src/app/shared/generated/api/usage-entity.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterTypeService } from "src/app/shared/generated/api/water-type.service";
import {
    AllocationPlanMinimalDto,
    GeographySimpleDto,
    MonthlyUsageSummaryDto,
    UserDto,
    WaterAccountDto,
    WaterAccountMinimalDto,
    WaterAccountParcelWaterMeasurementDto,
    WaterAccountWaterTypeSupplyDto,
    WaterTypeSimpleDto,
} from "src/app/shared/generated/model/models";
import { GeographyEnum } from "src/app/shared/models/enums/geography.enum";
import { GroupByPipe } from "src/app/shared/pipes/group-by.pipe";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { NgIf, NgClass, NgFor, DecimalPipe, PercentPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { ButtonGroupComponent } from "src/app/shared/components/button-group/button-group.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";
import { StaticWaterAccountMapComponent } from "src/app/shared/components/static-water-account-map/static-water-account-map.component";
import { VegaCumulativeUsageChartComponent } from "src/app/shared/components/vega/vega-cumulative-usage-chart/vega-cumulative-usage-chart.component";
import { VegaMonthlyUsageChartComponent } from "src/app/shared/components/vega/vega-monthly-usage-chart/vega-monthly-usage-chart.component";
import { WaterSupplyTypeComponent } from "src/app/shared/components/water-supply-type/water-supply-type.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterDashboardParcelWaterMeasurementsGridComponent } from "../components/water-dashboard-parcel-water-measurements-grid/water-dashboard-parcel-water-measurements-grid.component";

@Component({
    selector: "water-budget",
    templateUrl: "./water-budget.component.html",
    styleUrls: ["./water-budget.component.scss"],
    standalone: true,
    imports: [
        LoadingDirective,
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        ReportingPeriodSelectComponent,
        ButtonGroupComponent,
        NgClass,
        IconComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        StaticWaterAccountMapComponent,
        VegaCumulativeUsageChartComponent,
        VegaMonthlyUsageChartComponent,
        WaterDashboardParcelWaterMeasurementsGridComponent,
        NgFor,
        WaterSupplyTypeComponent,
        DecimalPipe,
        PercentPipe,
        DatePipe,
    ],
})
export class WaterBudgetComponent implements OnInit, OnDestroy {
    public currentUser: UserDto;

    public waterAccountID: number;
    private accountIDSubscription: Subscription = Subscription.EMPTY;
    public currentWaterAccount: WaterAccountMinimalDto;
    public currentGeography: GeographySimpleDto;
    public currentGeographySlug: string;

    public selectedYear: number;
    public waterTypes: WaterTypeSimpleDto[];
    public waterTypesSupply: WaterAccountWaterTypeSupplyDto[];
    public parcelIDs: number[];
    public allocationPlans: AllocationPlanMinimalDto[];

    public totalSupply: number;
    public usageToDate: number;
    public currentAvailable: number;
    public usageBar: string;
    public totalAcreage: number;
    public parcelArea: number;
    public usageEntitiesArea: number;
    public totalET: number;
    public totalEffectivePrecip: number;
    public showAcresFeet: boolean = false;
    public acresFeetUnits: string = "ac-ft";
    public acresFeetAcreUnits: string = "ac-ft/ac";
    public mostRecentSupplyDate: string;
    public mostRecentUsageDate: string;
    public mostRecentEffectiveDate: string;

    public showCumulativeWaterUsageChart = true;

    public showWaterAccountRollup = true;

    public isLoading: boolean = true;

    public waterAccountParcelWaterMeasurements: WaterAccountParcelWaterMeasurementDto[];
    public sourceOfRecordWaterMeasurements: WaterAccountParcelWaterMeasurementDto[];
    public monthlyUsageSummaries: MonthlyUsageSummaryDto[];

    constructor(
        private parcelSupplyService: ParcelSupplyService,
        private waterTypeService: WaterTypeService,
        private route: ActivatedRoute,
        private waterAccountService: WaterAccountService,
        private parcelService: ParcelService,
        private usageEntityService: UsageEntityService,
        private sumPipe: SumPipe,
        private groupByPipe: GroupByPipe
    ) {}

    ngOnDestroy() {
        this.accountIDSubscription.unsubscribe();
    }

    ngOnInit(): void {
        this.accountIDSubscription = this.route.paramMap.subscribe((paramMap) => {
            this.isLoading = true;
            this.waterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
            this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountID).subscribe((waterAccount) => {
                this.currentWaterAccount = waterAccount;
                this.currentGeography = waterAccount.Geography;
                this.currentGeographySlug = waterAccount.Geography.GeographyName.replace(" ", "-").toLowerCase();
                this.selectedYear = waterAccount.Geography.DefaultDisplayYear;
                this.getDataFromWaterAccountAndGeographyID(this.currentWaterAccount, this.currentGeography.GeographyID);
            });
        });
    }

    getDataFromWaterAccountAndGeographyID(waterAccount: WaterAccountDto, geographyID: number) {
        forkJoin({
            waterTypes: this.waterTypeService.geographiesGeographyIDWaterTypesActiveGet(geographyID),
            parcels: this.parcelService.waterAccountsWaterAccountIDParcelMinimalsGet(waterAccount.WaterAccountID),
            usageEntities: this.usageEntityService.waterAccountsWaterAccountIDUsageEntitiesGet(waterAccount.WaterAccountID),
            allocationPlans: this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(waterAccount.WaterAccountID),
        }).subscribe(({ waterTypes, parcels, usageEntities, allocationPlans }) => {
            this.parcelArea = this.sumPipe.transform(parcels, "ParcelArea");
            this.usageEntitiesArea = this.sumPipe.transform(usageEntities, "UsageEntityArea");
            this.parcelIDs = parcels.map((x) => x.ParcelID);
            this.allocationPlans = allocationPlans;

            forkJoin({
                parcelWaterMeasurements: this.waterAccountService.waterAccountsWaterAccountIDParcelSuppliesYearMonthlyUsageSummaryGet(
                    waterAccount.WaterAccountID,
                    this.selectedYear
                ),
                totalWaterTypeSupplyData: this.waterAccountService.waterAccountsWaterAccountIDWaterTypeSupplyYearGet(waterAccount.WaterAccountID, this.selectedYear),
                mostRecentEffectiveDates: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearWaterAccountsWaterAccountIDRecentEffectiveDatesGet(
                    geographyID,
                    this.selectedYear,
                    waterAccount.WaterAccountID
                ),
            }).subscribe(({ parcelWaterMeasurements, totalWaterTypeSupplyData, mostRecentEffectiveDates }) => {
                this.isLoading = false;
                this.waterAccountParcelWaterMeasurements = parcelWaterMeasurements;
                this.waterTypes = waterTypes;
                this.waterTypesSupply = totalWaterTypeSupplyData;
                this.mostRecentSupplyDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentSupplyEffectiveDate);
                this.mostRecentUsageDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentUsageEffectiveDate);
                this.mostRecentEffectiveDate = this.mostRecentSupplyDate > this.mostRecentUsageDate ? this.mostRecentSupplyDate : this.mostRecentUsageDate;
                this.setSupplyAndUsageValues();
            });
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
            const currentUsageAmount = this.sumPipe.transform(group, "CurrentUsageAmount");
            monthlyUsageSummary.CurrentUsageAmount = currentUsageAmount;
            monthlyUsageSummary.CurrentUsageAmountDepth = currentUsageAmount / usageEntityArea;
            const averageUsageAmount = this.sumPipe.transform(group, "AverageUsageAmount");
            monthlyUsageSummary.AverageUsageAmount = averageUsageAmount;
            monthlyUsageSummary.AverageUsageAmountDepth = averageUsageAmount / usageEntityArea;
            const currentCumulativeUsageAmount = group.map((value) => value.CurrentCumulativeUsageAmount).reduce((a, b) => (a == null ? null : a + b));
            monthlyUsageSummary.CurrentCumulativeUsageAmount = currentCumulativeUsageAmount == null ? null : currentCumulativeUsageAmount > 0 ? currentCumulativeUsageAmount : 0;
            monthlyUsageSummary.CurrentCumulativeUsageAmountDepth =
                currentCumulativeUsageAmount == null ? null : currentCumulativeUsageAmount > 0 ? currentCumulativeUsageAmount / usageEntityArea : 0;
            const averageCumulativeUsageAmount = group.map((value) => value.AverageCumulativeUsageAmount).reduce((a, b) => (a == null ? null : a + b));
            monthlyUsageSummary.AverageCumulativeUsageAmount = averageCumulativeUsageAmount == null ? null : averageCumulativeUsageAmount > 0 ? averageCumulativeUsageAmount : 0;
            monthlyUsageSummary.AverageCumulativeUsageAmountDepth =
                averageCumulativeUsageAmount == null ? null : averageCumulativeUsageAmount > 0 ? averageCumulativeUsageAmount / usageEntityArea : 0;
            monthlyUsageSummary.TotalSupply = this.sumPipe.transform(this.waterTypesSupply, "TotalSupply");
            monthlyUsageSummary.TotalSupplyDepth = this.convertToAcresFeetAcre(monthlyUsageSummary.TotalSupply);
            rollupData.push(monthlyUsageSummary);
        });
        return rollupData;
    }

    setSupplyAndUsageValues() {
        this.sourceOfRecordWaterMeasurements = this.waterAccountParcelWaterMeasurements.filter(
            (x) => x.WaterMeasurementTypeID === this.currentGeography.SourceOfRecordWaterMeasurementTypeID
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
        this.usageBar = "width: " + this.getPercentageOfWaterUsed() * 100 + "%";

        this.monthlyUsageSummaries = this.getSourceOfRecordWaterMeasurementRollups(this.sourceOfRecordWaterMeasurements);

        if (this.currentGeography.GeographyID == GeographyEnum.etsgsa) {
            const etValues = this.waterAccountParcelWaterMeasurements.filter((x) => x.WaterMeasurementTypeName === "Land IQ ETa");
            this.totalET = this.sumPipe.transform(etValues, "WaterMeasurementTotalValue");
            const effectivePrecipValues = this.waterAccountParcelWaterMeasurements.filter((x) => x.WaterMeasurementTypeName === "Effective Precip");
            this.totalEffectivePrecip = this.sumPipe.transform(effectivePrecipValues, "WaterMeasurementTotalValue");
        }
    }

    getWaterTypeUsage(waterType: WaterTypeSimpleDto) {
        let returnValue = 0;

        if (!this.waterTypesSupply) {
            return returnValue;
        }

        const waterUse = this.waterTypesSupply.filter((x) => x.WaterTypeID == waterType.WaterTypeID);

        if (waterUse.length == 1) {
            returnValue = waterUse[0].TotalSupply;
        }

        if (!this.showAcresFeet) {
            returnValue = this.convertToAcresFeetAcre(returnValue);
        }

        return returnValue;
    }

    setWaterSupplyBar(waterTypeTotalUse) {
        return "width: " + (waterTypeTotalUse / this.totalSupply) * 100 + "%";
    }

    getPercentageOfWaterUsed(): number {
        if (this.totalSupply > 0) {
            return this.usageToDate / this.totalSupply;
        }
        return 0;
    }

    updateDashboardForSelectedYear(selectedYear: number) {
        this.selectedYear = selectedYear;

        forkJoin({
            parcelWaterMeasurements: this.waterAccountService.waterAccountsWaterAccountIDParcelSuppliesYearMonthlyUsageSummaryGet(this.waterAccountID, this.selectedYear),
            totalWaterTypeSupplyData: this.waterAccountService.waterAccountsWaterAccountIDWaterTypeSupplyYearGet(this.waterAccountID, this.selectedYear),
            mostRecentEffectiveDates: this.parcelSupplyService.geographiesGeographyIDParcelSuppliesYearWaterAccountsWaterAccountIDRecentEffectiveDatesGet(
                this.currentGeography.GeographyID,
                this.selectedYear,
                this.waterAccountID
            ),
        }).subscribe(({ parcelWaterMeasurements, totalWaterTypeSupplyData, mostRecentEffectiveDates }) => {
            this.waterAccountParcelWaterMeasurements = parcelWaterMeasurements;
            this.waterTypesSupply = totalWaterTypeSupplyData;
            this.mostRecentSupplyDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentSupplyEffectiveDate);
            this.mostRecentUsageDate = this.getDateFromString(mostRecentEffectiveDates.MostRecentUsageEffectiveDate);
            this.mostRecentEffectiveDate = this.mostRecentSupplyDate > this.mostRecentUsageDate ? this.mostRecentSupplyDate : this.mostRecentUsageDate;
            this.setSupplyAndUsageValues();
        });
    }

    convertToAcresFeetAcre(num) {
        return num / this.totalAcreage;
    }

    changeUnits(temp) {
        this.showAcresFeet = temp;
    }

    getShowAcresFeet() {
        return this.showAcresFeet;
    }

    getDateFromString(dateString: string) {
        if (dateString != null) return dateString.substring(0, 10);
    }

    public updateShowCumulativeWaterUsageChart(value: boolean) {
        this.showCumulativeWaterUsageChart = value;
    }

    public updateShowWaterAccountRollup(value: boolean) {
        this.showWaterAccountRollup = value;
    }
}
