import { Component, Input, OnInit } from "@angular/core";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";
import * as vega from "vega";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { ParcelMinimalDto, ParcelWaterMeasurementChartDatumDto } from "src/app/shared/generated/model/models";
import { LoadingDirective } from "../../../directives/loading.directive";

import { ButtonComponent } from "../../button/button.component";

@Component({
    selector: "vega-parcel-usage-chart",
    templateUrl: "./vega-parcel-usage-chart.component.html",
    styleUrls: ["./vega-parcel-usage-chart.component.scss"],
    imports: [ButtonComponent, LoadingDirective]
})
export class VegaParcelUsageChartComponent implements OnInit {
    @Input() parcel: ParcelMinimalDto;

    public allChartData: ParcelWaterMeasurementChartDatumDto[];
    public isLoading: boolean = true;

    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(private waterMeasurementService: WaterMeasurementService) {}

    ngOnInit(): void {
        this.getChartData();
    }

    getChartData() {
        this.waterMeasurementService.listWaterMeasurementChartDataForParcelWaterMeasurement(this.parcel.GeographyID, this.parcel.ParcelID).subscribe((response) => {
            this.allChartData = response;
            this.isLoading = false;
            if (this.allChartData.length != 0) {
                this.setupChart();
            }
        });
    }

    setupChart(): void {
        const dateObjects = this.allChartData.map((x) => new Date(x.ReportedDate));
        const maxDate = new Date(Math.max.apply(null, dateObjects));
        maxDate.setMonth(maxDate.getMonth() + 1);

        const startDate = new Date(maxDate);
        startDate.setFullYear(maxDate.getFullYear() - 2);
        startDate.setMonth(startDate.getMonth() - 1);

        const colors = ["#7F3C8D", "#11A579", "#3969AC", "#F2B701", "#E73F74", "#80BA5A", "#E68310", "#008695", "#CF1C90", "#f97b72", "#4b4b8f", "#A5AA99"];
        vega.scheme("qanat", colors);

        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: {
                values: this.allChartData,
            },
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
                {
                    width: "container",
                    mark: {
                        type: "line",
                        point: { size: 100 },
                    },
                    params: [
                        {
                            name: "WaterMeasurementType",
                            select: { type: "point", fields: ["WaterMeasurementTypeName"] },
                            bind: "legend",
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "yearmonth",
                            field: "ReportedDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "month",
                            },
                            scale: { domain: { param: "brush" } },
                        },
                        y: {
                            field: "ReportedValueInAcreFeet",
                            type: "quantitative",
                            axis: { title: "Acre-feet per month" },
                        },
                        color: {
                            field: "WaterMeasurementTypeName",
                            type: "nominal",
                            axis: { title: "Water Measurement Type" },
                            scale: { scheme: "qanat" },
                        },
                        tooltip: [
                            { field: "ReportedDate", type: "quantitative", title: "Date", timeUnit: "yearmonth" },
                            { field: "ReportedValueInAcreFeet", type: "quantitative", title: "Acre-feet" },
                            { field: "WaterMeasurementTypeName", type: "nominal", title: "Water Measurement Type" },
                        ],
                        opacity: {
                            condition: [{ param: "WaterMeasurementType", value: 1 }],
                            value: 0.2,
                        },
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
                                    { year: startDate.getFullYear(), month: startDate.getMonth() },
                                    { year: maxDate.getFullYear(), month: maxDate.getMonth() },
                                ],
                            },
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "yearmonth",
                            field: "ReportedDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "year",
                                labelExpr: "year(datum.value)",
                            },
                        },
                        y: { field: "ReportedValueInAcreFeetSum", type: "quantitative", axis: { title: null, labels: false, ticks: false, domain: false } },
                        color: {
                            field: "WaterMeasurementTypeName",
                            type: "nominal",
                            axis: { title: "Water Measurement Type" },
                        },
                    },
                },
            ],
        } as unknown as VisualizationSpec;

        vegaEmbed("#vis", vegaSpec, { renderer: "svg" }).then((res) => {});
    }

    public downloadWaterMeasurements() {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;

        this.waterMeasurementService.listWaterMeasurementChartDataForParcelWaterMeasurement(this.parcel.GeographyID, this.parcel.ParcelID).subscribe((result) => {
            this.handleDownloadSuccess(result, `${this.parcel.ParcelNumber}_waterMeasurements`, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                (error) => this.handleDownloadError(error);
        });
    }

    private handleDownloadSuccess(result, fileName, contentType) {
        const blob = new Blob([result], {
            type: contentType,
        });

        //Create a fake object to trigger downloading the zip file that was returned
        const a: any = document.createElement("a");
        document.body.appendChild(a);

        a.style = "display: none";
        const url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isDownloading = false;
    }

    private handleDownloadError(error) {
        this.downloadError = true;
        //Because our return type is ArrayBuffer, the message will be ugly. Convert it and display
        const decodedString = String.fromCharCode.apply(null, new Uint8Array(error.error) as any);
        this.downloadErrorMessage = decodedString;
        this.isDownloading = false;
    }
}
