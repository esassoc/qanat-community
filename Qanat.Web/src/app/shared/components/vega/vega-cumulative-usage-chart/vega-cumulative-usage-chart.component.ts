import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { MonthlyUsageSummaryDto } from "src/app/shared/generated/model/monthly-usage-summary-dto";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";

@Component({
    selector: "vega-cumulative-usage-chart",
    standalone: true,
    imports: [LoadingDirective],
    templateUrl: "./vega-cumulative-usage-chart.component.html",
    styleUrls: ["./vega-cumulative-usage-chart.component.scss"],
})
export class VegaCumulativeUsageChartComponent implements OnChanges {
    @Input() year: number;
    @Input() showAcreFeet: boolean = false;
    @Input() supplyLabel: string = "Supply";
    @Input() usageLabel: string = "Usage";

    @Input() monthlyUsageSummaries: MonthlyUsageSummaryDto[];
    public isLoading: boolean = true;

    constructor() {}

    ngOnChanges(changes: SimpleChanges): void {
        if (!this.year) return;

        this.isLoading = true;
        this.setupChart();
        this.isLoading = false;
    }

    private setupChart() {
        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: {
                name: "cumulativeWaterUsages",
                values: this.monthlyUsageSummaries,
            },
            transform: [
                { calculate: this.showAcreFeet ? "datum.AverageCumulativeUsageAmount" : "datum.AverageCumulativeUsageAmountDepth", as: "AverageCumulativeUsageValue" },
                { calculate: this.showAcreFeet ? "datum.CurrentCumulativeUsageAmount" : "datum.CurrentCumulativeUsageAmountDepth", as: "CurrentCumulativeUsageValue" },
                { calculate: this.showAcreFeet ? "datum.TotalSupply" : "datum.TotalSupplyDepth", as: "TotalSupplyValue" },
            ],
            config: {
                legend: { orient: "bottom", labelFontSize: 12, symbolSize: 250, symbolStrokeWidth: 4, labelLimit: 300 },
            },
            width: "container",
            height: 400,
            padding: 0,
            layer: [
                {
                    mark: { type: "line", strokeWidth: 3, strokeDash: [3, 3] },
                    encoding: {
                        x: {
                            field: "EffectiveDate",
                            type: "temporal",
                            timeUnit: "utcyearmonth",
                            title: "",
                            axis: { labelFontSize: 12, labelAngle: -45 },
                        },
                        y: { field: "AverageCumulativeUsageValue", type: "quantitative", axis: { labelFontSize: 12, titleFontSize: 12 } },
                        color: { datum: `Average ${this.usageLabel} (All Years)`, scale: { range: ["#aaa"] }, legend: { symbolDash: [3, 3] } },
                    },
                },
                {
                    mark: {
                        type: "area",
                        color: {
                            gradient: "linear",
                            x1: 1,
                            y1: 1,
                            x2: 1,
                            y2: 0,
                            stops: [
                                { offset: 0, color: "white" },
                                { offset: 1, color: "#71c9e9" },
                            ],
                        },
                        point: { size: 100, color: "#71c9e9" },
                    },
                    encoding: {
                        x: { field: "EffectiveDate", type: "temporal", timeUnit: "utcyearmonth" },
                        y: { field: "CurrentCumulativeUsageValue", type: "quantitative", title: this.showAcreFeet ? "Volume (ac-ft)" : "Depth (ac-ft/ac)" },
                    },
                },
                {
                    mark: { type: "area", opacity: 0 },
                    encoding: {
                        color: { datum: `${this.year} Cumulative ${this.usageLabel}`, scale: { range: ["#71c9e9"] } },
                    },
                },
                {
                    mark: { type: "rule", strokeWidth: 3 },
                    encoding: {
                        y: { field: this.showAcreFeet ? "TotalSupply" : "TotalSupplyDepth", aggregate: "max" },
                        color: { datum: `Total ${this.supplyLabel}`, scale: { range: ["#ed6969"] } },
                    },
                },
                {
                    mark: "rule",
                    encoding: {
                        x: { field: "EffectiveDate" },
                        opacity: {
                            condition: { value: 0.4, param: "hover", empty: false },
                            value: 0.1,
                        },
                        tooltip: [
                            { field: "EffectiveDate", type: "temporal", timeUnit: "utcyearmonth", scale: { type: "utc" }, title: "Date" },
                            { field: "CurrentCumulativeUsageValue", type: "quantitative", title: `Cumulative ${this.usageLabel}`, format: ",.2f" },
                            { field: "AverageCumulativeUsageValue", type: "quantitative", title: `Average ${this.usageLabel}`, format: ",.2f" },
                            { field: "TotalSupplyValue", type: "quantitative", title: `Total ${this.supplyLabel}`, format: ",.2f" },
                        ],
                    },
                    params: [
                        {
                            name: "hover",
                            select: {
                                type: "point",
                                fields: ["EffectiveDate"],
                                nearest: true,
                                on: "mouseover",
                                clear: "mouseout",
                            },
                        },
                    ],
                },
            ],
            resolve: {
                scale: { color: "independent" },
            },
        } as VisualizationSpec;

        vegaEmbed("#vis", vegaSpec, { renderer: "svg" });
        this.isLoading = false;
    }
}
