import { Component, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { MeterReadingMonthlyInterpolationSimpleDto } from "src/app/shared/generated/model/meter-reading-monthly-interpolation-simple-dto";
import { vega, default as vegaEmbed, VisualizationSpec } from "vega-embed";

@Component({
    selector: "vega-meter-reading-monthly-chart",
    imports: [LoadingDirective],
    templateUrl: "./vega-meter-reading-monthly-chart.component.html",
    styleUrl: "./vega-meter-reading-monthly-chart.component.scss"
})
export class VegaMeterReadingMonthlyChartComponent implements OnInit, OnChanges {
    @Input() monthlyInterpolations: MeterReadingMonthlyInterpolationSimpleDto[];

    public isLoading: boolean = true;
    ngOnInit(): void {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.monthlyInterpolations && changes.monthlyInterpolations.currentValue) {
            this.buildChart();
            this.isLoading = false;
        }
    }

    public buildChart() {
        const dateObjects = this.monthlyInterpolations.map((x) => new Date(x.Date));
        const maxDate = new Date(Math.max.apply(null, dateObjects));
        maxDate.setMonth(maxDate.getMonth() + 1);

        const startDate = new Date(maxDate);
        startDate.setFullYear(maxDate.getFullYear() - 2);
        startDate.setMonth(startDate.getMonth() - 1);

        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: { values: this.monthlyInterpolations.map((d) => ({ ...d, LegendLabel: "Monthly Values" })) },
            config: {
                legend: {
                    orient: "top",
                    direction: "horizontal",
                    columns: 4, // Set this to the number of legend items you want per row
                    title: null,
                    labelFontSize: 12, // Adjust font size if needed
                    labelLimit: 0, // Disable truncation
                    labelAlign: "left", // Align labels to the left for readability
                },
            },
            vconcat: [
                // Main Chart
                {
                    width: "container",
                    height: 300,
                    mark: {
                        type: "line",
                        point: { size: 100 },
                        tooltip: true,
                        color: "#71c9e9",
                    },
                    params: [
                        {
                            name: "InterpolatedVolumeInAcreFeet",
                            select: { type: "point", fields: ["InterpolatedVolumeInAcreFeet"] },
                            bind: "legend",
                        },
                    ],
                    encoding: {
                        x: {
                            field: "Date",
                            type: "temporal",
                            timeUnit: "utcyearmonth",
                            title: "",
                            axis: { labelFontSize: 12 },
                            scale: { domain: { param: "brush" } }, // Sync with brush
                        },
                        y: {
                            field: "InterpolatedVolumeInAcreFeet",
                            type: "quantitative",
                            title: "Volume (ac-ft)",
                            axis: { labelFontSize: 12, titleFontSize: 12 },
                        },
                        color: {
                            field: "LegendLabel",
                            type: "nominal",
                            scale: { domain: ["Monthly Values"], range: ["#71c9e9"] },
                            legend: {
                                title: null,
                            },
                        },
                        tooltip: [
                            {
                                field: "Date",
                                type: "temporal",
                                title: "Date",
                                format: "%b %Y",
                                scale: { type: "utc" },
                            },
                            { field: "InterpolatedVolumeInAcreFeet", type: "quantitative", format: ",.4f", title: "Volume (ac-ft)" },
                            { field: "InterpolatedVolume", type: "quantitative", format: ",.4f", title: "Volume" },
                        ],
                    },
                },
                // Scrubber Chart
                {
                    view: {
                        fill: "#eee",
                    },
                    name: "scrubber",
                    title: {
                        text: "Click and drag chart beneath to zoom selection",
                        align: "left",
                        anchor: "start",
                    },
                    width: "container",
                    height: 75,
                    mark: {
                        type: "line",
                        point: false,
                    },
                    params: [
                        {
                            name: "brush",
                            select: { type: "interval", encodings: ["x"] },
                        },
                    ],
                    encoding: {
                        x: {
                            field: "Date",
                            type: "temporal",
                            timeUnit: "utcyearmonth",
                            axis: {
                                title: null,
                                tickCount: "month",
                                expr: "timeParse(datum.Date, '%Y-%m-%dT%H:%M:%S%Z')",
                            },
                        },
                        y: {
                            field: "InterpolatedVolumeInAcreFeet",
                            type: "quantitative",
                            axis: { title: null, labels: false, ticks: false, domain: false },
                        },
                    },
                },
            ],
        } as VisualizationSpec;

        vegaEmbed("#monthly-vis", vegaSpec, { renderer: "svg" });
    }
}
