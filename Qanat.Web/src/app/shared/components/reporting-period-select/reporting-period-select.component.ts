import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { ReportingPeriodService } from "../../generated/api/reporting-period.service";
import { Observable } from "rxjs";
import { ReportingPeriodSimpleDto } from "../../generated/model/reporting-period-simple-dto";
import { FormsModule } from "@angular/forms";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";

@Component({
    selector: "reporting-period-select",
    templateUrl: "./reporting-period-select.component.html",
    styleUrl: "./reporting-period-select.component.scss",
    standalone: true,
    imports: [NgIf, FormsModule, NgFor, AsyncPipe],
})
export class ReportingPeriodSelectComponent implements OnInit, OnChanges {
    @Input() geographyID: number;
    @Input() defaultDisplayYear?: number;

    @Output() selectionChanged = new EventEmitter<number>();

    public selectedYear: number;
    public $reportingPeriod: Observable<ReportingPeriodSimpleDto>;
    public $reportingPeriodYears: Observable<number[]>;

    constructor(private reportingPeriodService: ReportingPeriodService) {}

    ngOnInit(): void {
        if (!this.geographyID) {
            console.error("No geographyID specified for the Reporting Period selector.");
            return;
        }

        this.selectedYear = this.defaultDisplayYear ?? new Date().getFullYear();

        this.refreshReportingPeriod();
    }

    private refreshReportingPeriod() {
        this.$reportingPeriod = this.reportingPeriodService.geographiesGeographyIDReportingPeriodGet(this.geographyID);
        this.$reportingPeriodYears = this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(this.geographyID);
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (Object.keys(changes).includes("geographyID")) {
            this.refreshReportingPeriod();
        }
    }

    public onSelectionChanged() {
        this.selectionChanged.emit(this.selectedYear);
    }
}
