import { Component, Input, OnInit } from "@angular/core";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ZoneGroupMonthlyUsageDto } from "src/app/shared/generated/model/zone-group-monthly-usage-dto";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";
import { CustomRichTextComponent } from "../../custom-rich-text/custom-rich-text.component";
import { LoadingDirective } from "../../../directives/loading.directive";
import { NgClass } from "@angular/common";

@Component({
    selector: "vega-zone-group-usage-chart",
    templateUrl: "./vega-zone-group-usage-chart.component.html",
    styleUrls: ["./vega-zone-group-usage-chart.component.scss"],
    imports: [NgClass, LoadingDirective, CustomRichTextComponent],
})
export class VegaZoneGroupUsageChartComponent implements OnInit {
    @Input() geographyID: number;
    @Input() zoneGroupSlug: string;

    public chartData: ZoneGroupMonthlyUsageDto[];
    private startDate: Date;
    private maxDate: Date;

    public showAcreFeet: boolean = false;
    public isLoading: boolean = true;
    public richTextTypeID = CustomRichTextTypeEnum.ZoneGroupUsageChart;

    constructor(private zoneGroupService: ZoneGroupService) {}

    ngOnInit(): void {
        if (!this.chartData) {
            this.getChartData();
            return;
        }

        this.setupChart();
    }

    public changeUnits(value: boolean) {
        this.showAcreFeet = value;
        this.setupChart();
    }

    private getChartData() {
        this.zoneGroupService.listWaterUsageByZoneGroupSlugZoneGroup(this.geographyID, this.zoneGroupSlug).subscribe((zoneGroupUsages) => {
            this.chartData = zoneGroupUsages;

            const dateObjects = zoneGroupUsages.map((x) => new Date(x.EffectiveDate));
            this.maxDate = new Date(Math.max.apply(null, dateObjects));
            this.maxDate.setMonth(this.maxDate.getMonth() + 1);

            this.startDate = new Date(this.maxDate);
            this.startDate.setFullYear(this.maxDate.getFullYear() - 2);
            this.startDate.setMonth(this.startDate.getMonth() - 1);

            this.setupChart();
        });
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
                    transform: [
                        {
                            calculate: this.showAcreFeet ? "format(datum.UsageAmount, ',.2f') + ' ac-ft'" : "format(datum.UsageAmountDepth, ',.2f') + ' ac-ft/ac'",
                            as: "UsageAmountWithUnits",
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "utcyearmonth",
                            field: "EffectiveDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "month",
                                labelFontSize: 12,
                            },
                            scale: { domain: { param: "brush" } },
                        },
                        y: this.showAcreFeet
                            ? { field: "UsageAmount", type: "quantitative", axis: { title: "Volume (ac-ft)", labelFontSize: 12 } }
                            : { field: "UsageAmountDepth", type: "quantitative", axis: { title: "Depth (ac-ft/ac)" } },
                        color: {
                            field: "ZoneName",
                            type: "nominal",
                            scale: { range: { field: "ZoneColor" } },
                        },
                        tooltip: [
                            { field: "EffectiveDate", type: "quantitative", title: "Date", timeUnit: "utcyearmonth", scale: { type: "utc" } },
                            { field: "UsageAmountWithUnits", type: "nominal", title: "Usage" },
                            { field: "ZoneName", type: "nominal", title: "Zone" },
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
                            timeUnit: "utcyearmonth",
                            field: "EffectiveDate",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "year",
                                labelExpr: "year(datum.value)",
                            },
                        },
                        y: {
                            field: this.showAcreFeet ? "UsageAmount" : "UsageAmountDepth",
                            type: "quantitative",
                            axis: { title: null, labels: false, ticks: false, domain: false },
                        },
                        color: {
                            field: "ZoneName",
                            type: "nominal",
                        },
                    },
                },
            ],
        } as VisualizationSpec;

        vegaEmbed("#zoneGroupUsageChart", vegaSpec, { renderer: "svg" }).then((res) => {});
        this.isLoading = false;
    }
}
