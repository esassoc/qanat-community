import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { ReportingPeriodService } from "../../generated/api/reporting-period.service";
import { Observable, of, switchMap } from "rxjs";
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
    public $reportingPeriodYears: Observable<number[]>;

    constructor(private reportingPeriodService: ReportingPeriodService) {}

    ngOnInit(): void {
        if (!this.geographyID) {
            console.error("No geographyID specified for the Reporting Period selector.");
            return;
        }

        this.selectedYear = this.defaultDisplayYear ?? new Date().getUTCFullYear();
        this.refreshReportingPeriod();
    }

    private refreshReportingPeriod() {
        this.$reportingPeriodYears = this.reportingPeriodService.geographiesGeographyIDReportingPeriodsGet(this.geographyID).pipe(
            switchMap((reportingPeriods) => {
                return of(reportingPeriods.map((x) => new Date(x.StartDate).getUTCFullYear()));
            })
        );
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (Object.keys(changes).includes("geographyID")) {
            this.refreshReportingPeriod();
            this.selectedYear = this.defaultDisplayYear ?? new Date().getUTCFullYear();
        }
    }

    public onSelectionChanged() {
        this.selectionChanged.emit(this.selectedYear);
    }
}
