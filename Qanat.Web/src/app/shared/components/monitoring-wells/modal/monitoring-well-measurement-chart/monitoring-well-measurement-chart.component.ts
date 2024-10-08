import { Component, ComponentRef, OnInit, ViewContainerRef } from "@angular/core";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { VegaMonitoringWellsMeasurementChartComponent } from "../../../vega/vega-monitoring-wells-measurement-chart/vega-monitoring-wells-measurement-chart.component";

@Component({
    selector: "monitoring-well-measurement-chart",
    templateUrl: "./monitoring-well-measurement-chart.component.html",
    styleUrls: ["./monitoring-well-measurement-chart.component.scss"],
    standalone: true,
    imports: [VegaMonitoringWellsMeasurementChartComponent],
})
export class MonitoringWellMeasurementChartComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: MonitoringWellContext;

    geographyID: number;
    siteCode: string;
    monitoringWellName: string;

    constructor(
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit(): void {
        this.geographyID = this.modalContext.GeographyID;
        this.siteCode = this.modalContext.SiteCode;
        this.monitoringWellName = this.modalContext.MonitoringWellName;
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }
}
export interface MonitoringWellContext {
    GeographyID: number;
    SiteCode: string;
    MonitoringWellName: string;
}
