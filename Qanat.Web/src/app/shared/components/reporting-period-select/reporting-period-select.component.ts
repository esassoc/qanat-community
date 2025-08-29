import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { ReportingPeriodService } from "../../generated/api/reporting-period.service";
import { Observable, tap } from "rxjs";
import { FormsModule } from "@angular/forms";
import { AsyncPipe, CommonModule } from "@angular/common";
import { ReportingPeriodDto } from "../../generated/model/models";
import { NgSelectModule } from "@ng-select/ng-select";

@Component({
    selector: "reporting-period-select",
    templateUrl: "./reporting-period-select.component.html",
    styleUrl: "./reporting-period-select.component.scss",
    imports: [FormsModule, AsyncPipe, NgSelectModule, CommonModule],
})
export class ReportingPeriodSelectComponent implements OnInit, OnChanges {
    @Input() geographyID: number;
    @Input({ required: false }) initialReportingPeriodID: number | null = null;
    @Input({ required: false }) compact: boolean = false;

    @Output() selectionChanged = new EventEmitter<ReportingPeriodDto>();

    public selectedReportingPeriod: ReportingPeriodDto;
    public reportingPeriods$: Observable<ReportingPeriodDto[]>;

    constructor(private reportingPeriodService: ReportingPeriodService) {}

    ngOnInit(): void {
        if (!this.geographyID) {
            console.error("No geographyID specified for the Reporting Period selector.");
            return;
        }

        this.refreshReportingPeriod(this.initialReportingPeriodID);
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (Object.keys(changes).includes("geographyID")) {
            this.refreshReportingPeriod();
        }
    }

    public onSelectionChanged() {
        this.selectionChanged.emit(this.selectedReportingPeriod);
    }

    private refreshReportingPeriod(selectedReportingPeriodID?: number | null) {
        this.reportingPeriods$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.geographyID).pipe(
            tap((reportingPeriods) => {
                if (selectedReportingPeriodID) {
                    this.selectedReportingPeriod = reportingPeriods.find((rp) => rp.ReportingPeriodID === selectedReportingPeriodID);
                } else {
                    let defaultReportingPeriod = reportingPeriods.find((rp) => rp.IsDefault);
                    if (!defaultReportingPeriod) {
                        defaultReportingPeriod = reportingPeriods[0];
                    }

                    this.selectedReportingPeriod = defaultReportingPeriod;
                }

                this.onSelectionChanged();
            })
        );
    }
}
