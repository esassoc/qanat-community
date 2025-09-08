import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { MonthlyUsageSummaryDto } from "src/app/shared/generated/model/models";
import { vega, default as vegaEmbed, VisualizationSpec } from "vega-embed";

@Component({
    selector: "vega-monthly-usage-chart",
    imports: [LoadingDirective],
    templateUrl: "./vega-monthly-usage-chart.component.html",
    styleUrls: ["./vega-monthly-usage-chart.component.scss"]
})
export class VegaMonthlyUsageChartComponent implements OnChanges {
    @Input() year: number;
    @Input() showAcreFeet: boolean = false;
    @Input() usageLabel: string = "Usage";
    @Input() monthlyUsageSummaries: MonthlyUsageSummaryDto[];

    public chartData: MonthlyUsageChartData[];
    public isLoading: boolean = true;

    constructor() {}

    ngOnChanges(changes: SimpleChanges): void {
        this.updateCumulativeWaterUsages();
    }

    private updateCumulativeWaterUsages() {
        this.chartData = [];
        this.monthlyUsageSummaries.forEach((x) => {
            const currentUsageValue = this.showAcreFeet ? x.CurrentUsageAmount : x.CurrentUsageAmountDepth;
            const currentUsage = new MonthlyUsageChartData(x.EffectiveDate, currentUsageValue > 0 ? currentUsageValue : 0, `${this.year} Current ${this.usageLabel}`, "#71c9e9");
            this.chartData.push(currentUsage);

            const averageUsageValue = this.showAcreFeet ? x.AverageUsageAmount : x.AverageUsageAmountDepth;
            const averageUsage = new MonthlyUsageChartData(x.EffectiveDate, averageUsageValue > 0 ? averageUsageValue : 0, `Average ${this.usageLabel} (All Years)`, "#c5c5c5");
            this.chartData.push(averageUsage);
        });

        this.setupChart();
        this.isLoading = false;
    }

    private setupChart() {
        vega.scheme("monthlyUsage", ["#c5c5c5", "#71c9e9"]);

        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: {
                name: "cumulativeWaterUsages",
                values: this.chartData,
            },
            width: "container",
            height: 400,
            config: {
                legend: { orient: "bottom", title: "", labelFontSize: 12, symbolSize: 250, labelLimit: 300 },
            },
            mark: { type: "bar", tooltip: true },
            encoding: {
                x: { field: "EffectiveDate", type: "nominal", timeUnit: "utcyearmonth", title: "", axis: { labelFontSize: 12 } },
                y: { field: "Value", type: "quantitative", title: this.showAcreFeet ? "Volume (ac-ft)" : "Depth (ac-ft/ac)", axis: { labelFontSize: 12, titleFontSize: 12 } },
                xOffset: { field: "Type", sort: "descending" },
                color: { field: "Type", type: "nominal", scale: { scheme: "monthlyUsage" }, sort: "descending" },
                tooltip: [
                    { field: "EffectiveDate", type: "temporal", timeUnit: "utcmonthyear", scale: { type: "utc" }, title: "Date" },
                    { field: "Value", type: "quantitative", format: ",.2f" },
                    { field: "Type", type: "nominal" },
                ],
            },
        } as VisualizationSpec;
        vegaEmbed("#monthly-vis", vegaSpec, { renderer: "svg" });
    }
}

class MonthlyUsageChartData {
    EffectiveDate: string;
    Value: number;
    Type: string;
    Color: string;

    constructor(effectiveDate: string, value: number, type: string, color: string) {
        this.EffectiveDate = effectiveDate;
        this.Value = value;
        this.Type = type;
        this.Color = color;
    }
}
