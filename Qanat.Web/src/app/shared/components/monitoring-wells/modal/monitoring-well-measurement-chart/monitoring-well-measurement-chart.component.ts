import { Component, inject, OnInit } from "@angular/core";
import { VegaMonitoringWellsMeasurementChartComponent } from "../../../vega/vega-monitoring-wells-measurement-chart/vega-monitoring-wells-measurement-chart.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "monitoring-well-measurement-chart",
    templateUrl: "./monitoring-well-measurement-chart.component.html",
    styleUrls: ["./monitoring-well-measurement-chart.component.scss"],
    imports: [VegaMonitoringWellsMeasurementChartComponent]
})
export class MonitoringWellMeasurementChartComponent implements OnInit {
    public ref: DialogRef<MonitoringWellContext, boolean> = inject(DialogRef);

    geographyID: number;
    siteCode: string;
    monitoringWellName: string;

    constructor() {}

    ngOnInit(): void {
        this.geographyID = this.ref.data.GeographyID;
        this.siteCode = this.ref.data.SiteCode;
        this.monitoringWellName = this.ref.data.MonitoringWellName;
    }

    close() {
        this.ref.close(true);
    }
}
export interface MonitoringWellContext {
    GeographyID: number;
    SiteCode: string;
    MonitoringWellName: string;
}
