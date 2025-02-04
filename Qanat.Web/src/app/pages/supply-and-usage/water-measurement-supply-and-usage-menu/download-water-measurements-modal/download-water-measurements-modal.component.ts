import { Component, ComponentRef, OnInit } from "@angular/core";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ReportingPeriodSelectComponent } from "../../../../shared/components/reporting-period-select/reporting-period-select.component";

@Component({
    selector: "download-water-measurements-modal",
    standalone: true,
    imports: [ReportingPeriodSelectComponent],
    templateUrl: "./download-water-measurements-modal.component.html",
    styleUrl: "./download-water-measurements-modal.component.scss",
})
export class DownloadWaterMeasurementsModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: DownloadWaterMeasurementsContext;

    public reportingYear: number;

    constructor(private modalService: ModalService) {}

    ngOnInit(): void {
        this.reportingYear = this.modalContext.GeographyStartYear;
    }

    public updateReportingYear($event: number) {
        this.reportingYear = $event;
    }

    public download() {
        this.modalService.close(this.modalComponentRef, {
            year: this.reportingYear,
        });
    }

    public close() {
        this.modalService.close(this.modalComponentRef, null);
    }
}

export class DownloadWaterMeasurementsContext {
    public GeographyID: number;
    public GeographyName: string;
    public GeographyStartYear: number;
}
