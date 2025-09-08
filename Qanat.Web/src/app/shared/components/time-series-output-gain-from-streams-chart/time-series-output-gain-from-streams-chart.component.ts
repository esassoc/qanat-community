import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

import { default as vegaEmbed, VisualizationSpec, vega } from "vega-embed";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { ScenarioRunDto, ScenarioRunResult } from "../../generated/model/models";

@Component({
    selector: "time-series-output-gain-from-streams-chart",
    imports: [],
    templateUrl: "./time-series-output-gain-from-streams-chart.component.html",
    styleUrls: ["./time-series-output-gain-from-streams-chart.component.scss"]
})
export class TimeSeriesOutputGainFromStreamsChartComponent implements OnInit {
    @Input() scenarioRunResult: ScenarioRunResult;
    @Input() scenarioRun: ScenarioRunDto;

    private vegaView: any;

    // selected label two-way binding functionality
    @Output() selectedLabelChange: EventEmitter<string> = new EventEmitter<string>();
    private _selectedLabel: string = null;
    @Input()
    public set selectedLabel(value: string) {
        if (this._selectedLabel != value) {
            this._selectedLabel = value;
            this.vegaView.signal("selectedLabel_Name_legend", value).runAsync();
        }
        this.selectedLabelChange.emit(value);
    }
    get selectedLabel() {
        return this._selectedLabel;
    }

    constructor(private leafletHelperService: LeafletHelperService) {}

    ngOnInit(): void {
        this.setupChart();
    }

    private setupChart() {
        let allChartData = [];
        let currentColorIndex = 0;
        const colorRange = [];

        this.scenarioRunResult.PointsOfInterest.forEach((input) => {
            const rows = input.ScenarioRunResultTimeSeriesOutputs.map((x) => {
                return { Name: input.Name, Date: x.Date, Value: x.Value, Order: currentColorIndex };
            });

            colorRange.push(this.leafletHelperService.markerColors[currentColorIndex]);
            if (currentColorIndex > this.leafletHelperService.markerColors.length - 1) {
                currentColorIndex = 0;
            } else {
                currentColorIndex++;
            }

            allChartData = [...allChartData, ...rows];
        });
        const dateObjects = allChartData.map((x) => new Date(x.Date));
        const maxDate = new Date(Math.max.apply(null, dateObjects));
        maxDate.setMonth(maxDate.getMonth() + 1);

        const startDate = new Date(maxDate);
        startDate.setFullYear(maxDate.getFullYear() - 2);
        startDate.setMonth(startDate.getMonth() - 1);

        const colors = this.leafletHelperService.markerColors;
        vega.scheme("qanat", colors);

        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v5.json",
            description: "A simple bar chart with embedded data.",
            data: {
                values: allChartData,
            },
            config: {
                legend: { orient: "top" },
            },

            vconcat: [
                {
                    width: "container",
                    height: 600,
                    mark: {
                        type: "line",
                        point: { size: 100 },
                    },
                    params: [
                        {
                            name: "selectedLabel",
                            select: { type: "point", fields: ["Name"] },
                            bind: "legend",
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "year",
                            field: "Date",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "year",
                            },
                            scale: { domain: { param: "brush" } },
                        },
                        y: { field: "Value", type: "quantitative", axis: { title: "Feet" } },
                        color: {
                            field: "Name",
                            type: "nominal",
                            axis: { title: "Location" },
                            sort: { field: "Order", order: "ascending" },
                            scale: { scheme: "qanat" },
                        },
                        tooltip: [
                            { field: "Date", type: "quantitative", title: "Date", timeUnit: "year" },
                            { field: "Value", type: "quantitative", title: "Feet" },
                            { field: "Name", type: "nominal", title: "Location" },
                        ],
                        opacity: {
                            condition: [{ param: "selectedLabel", value: 1 }],
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
                            // value: {"x": [{"year": startDate.getFullYear(), "month": startDate.getMonth()}, {"year": maxDate.getFullYear(), "month": maxDate.getMonth()}]},
                        },
                    ],
                    encoding: {
                        x: {
                            timeUnit: "year",
                            field: "Date",
                            type: "temporal",
                            axis: {
                                title: null,
                                tickCount: "year",
                                labelExpr: "year(datum.value)",
                            },
                        },
                        y: { field: "Value", type: "quantitative", axis: { title: null, labels: false, ticks: false, domain: false } },
                        color: {
                            field: "Name",
                            type: "nominal",
                            sort: { field: "Order", order: "ascending" },
                            axis: { title: "" },
                        },
                    },
                },
            ],
        } as unknown as VisualizationSpec;

        vegaEmbed("#vis2", vegaSpec, { renderer: "svg" }).then((result) => {
            this.vegaView = result.view;
            // listen to label selected changes
            this.vegaView.addSignalListener("selectedLabel", (name, value) => {
                if (!value?.Name) {
                    this.selectedLabel = null;
                } else if (value.Name[0] != this.selectedLabel) {
                    this.selectedLabel = value.Name[0];
                }
            });
        });
    }
}
