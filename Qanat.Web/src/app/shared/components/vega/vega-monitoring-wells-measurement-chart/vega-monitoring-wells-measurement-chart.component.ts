import { Component, Input, OnInit } from "@angular/core";
import { MonitoringWellService } from "src/app/shared/generated/api/monitoring-well.service";
import { MonitoringWellMeasurementDataDto } from "src/app/shared/generated/model/models";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";
import { NgIf, DatePipe } from "@angular/common";
import { LoadingDirective } from "../../../directives/loading.directive";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "vega-monitoring-wells-measurement-chart",
    templateUrl: "./vega-monitoring-wells-measurement-chart.component.html",
    styleUrls: ["./vega-monitoring-wells-measurement-chart.component.scss"],
    standalone: true,
    imports: [LoadingDirective, NgIf, DatePipe],
})
export class VegaMonitoringWellsMeasurementChartComponent implements OnInit {
    @Input() geographyID: number;
    @Input() siteCode: string;

    public chartData: MonitoringWellMeasurementDataDto[];
    public lastMeasurementDate: Date;
    private startDate: Date;
    private maxDate: Date;

    public showAcreFeet: boolean = false;
    public isLoading: boolean = true;

    constructor(private publicService: PublicService) {}

    ngOnInit(): void {
        this.getChartData();
    }

    private getChartData() {
        this.publicService.publicGeographiesGeographyIDMonitoringWellSiteCodeGet(this.geographyID, this.siteCode).subscribe((monitoringWellMeasurements) => {
            this.chartData = monitoringWellMeasurements;

            if (monitoringWellMeasurements.length == 0) {
                this.isLoading = false;
                return;
            }

            const dateObjects = monitoringWellMeasurements.map((x) => new Date(x.MeasurementDate));
            this.maxDate = new Date(Math.max.apply(null, dateObjects));
            this.lastMeasurementDate = new Date(Math.max.apply(null, dateObjects));
            this.maxDate.setMonth(this.maxDate.getMonth() + 1);

            this.startDate = new Date(this.maxDate);
            this.startDate.setFullYear(this.maxDate.getFullYear() - 2);
            this.startDate.setMonth(this.startDate.getMonth() - 1);

            this.setupChart();
        });
    }

    public changeUnits(value: boolean) {
        this.showAcreFeet = value;
        this.setupChart();
    }

    private setupChart() {
        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: { values: this.chartData },
            config: {
                legend: { orient: "top", title: "", labelFontSize: 12, symbolSize: 250 },
            },
            vconcat: [
                {
                    width: "container",
                    height: 300,
                    mark: {
                        type: "line",
                        point: { size: 50 },
                    },
                    encoding: {
                        x: {
                            timeUnit: "utcyearmonthdate",
                            field: "MeasurementDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "month",
                                labelFontSize: 12,
                            },
                            scale: { domain: { param: "brush" } },
                        },
                        y: { field: "Measurement", type: "quantitative", axis: { title: "Measurement (depth)", labelFontSize: 12 }, scale: { zero: false } },
                        tooltip: [
                            { field: "MeasurementDate", type: "quantitative", title: "Date", timeUnit: "utcyearmonthdate", scale: { type: "utc" } },
                            { field: "Measurement", type: "nominal", title: "Measurement (depth)" },
                        ],
                    },
                },
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
                            value: {
                                x: [
                                    { year: this.startDate.getFullYear(), month: this.startDate.getMonth() },
                                    { year: this.maxDate.getFullYear(), month: this.maxDate.getMonth() },
                                ],
                            },
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "utcyearmonthdate",
                            field: "MeasurementDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "year",
                                labelExpr: "year(datum.value)",
                            },
                        },
                        y: { field: "Measurement", type: "quantitative", axis: { title: null, labels: false, ticks: false, domain: false } },
                    },
                },
            ],
        } as VisualizationSpec;

        vegaEmbed("#monitoringWellMeasurementChart", vegaSpec, { renderer: "svg" }).then((res) => {});
        this.isLoading = false;
    }
}
