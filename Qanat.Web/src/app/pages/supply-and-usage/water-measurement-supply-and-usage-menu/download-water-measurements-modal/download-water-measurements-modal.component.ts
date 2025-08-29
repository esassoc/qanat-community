import { Component, inject, OnInit } from "@angular/core";
import { ReportingPeriodSelectComponent } from "../../../../shared/components/reporting-period-select/reporting-period-select.component";
import { ReportingPeriodDto } from "src/app/shared/generated/model/reporting-period-dto";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "download-water-measurements-modal",
    imports: [ReportingPeriodSelectComponent],
    templateUrl: "./download-water-measurements-modal.component.html",
    styleUrl: "./download-water-measurements-modal.component.scss",
})
export class DownloadWaterMeasurementsModalComponent implements OnInit {
    public ref: DialogRef<DownloadWaterMeasurementsContext, number> = inject(DialogRef);

    public reportingYear: number;

    constructor() {}

    ngOnInit(): void {
        this.reportingYear = this.ref.data.GeographyStartYear;
    }

    public onSelectedReportingPeriodChange($event: ReportingPeriodDto) {
        let endDate = new Date($event.EndDate);
        this.reportingYear = endDate.getUTCFullYear();
    }

    public download() {
        this.ref.close(this.reportingYear);
    }

    public close() {
        this.ref.close(null);
    }
}

export class DownloadWaterMeasurementsContext {
    public GeographyID: number;
    public GeographyName: string;
    public GeographyStartYear: number;
}
