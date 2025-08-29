import { DialogRef } from "@ngneat/dialog";
import { AsyncPipe } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgSelectModule } from "@ng-select/ng-select";
import { Observable, switchMap, tap } from "rxjs";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { GeographyMinimalDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";

@Component({
    selector: "refresh-water-measurement-calculations-modal",
    standalone: true,
    imports: [FormsModule, NgSelectModule, ButtonLoadingDirective, AsyncPipe],
    templateUrl: "./refresh-water-measurement-calculations-modal.component.html",
    styleUrl: "./refresh-water-measurement-calculations-modal.component.scss",
})
export class RefreshWaterMeasurementCalculationsModalComponent implements OnInit {
    public ref: DialogRef<RefreshWaterMeasurementCalculationsContext, boolean> = inject(DialogRef);

    public geography$: Observable<GeographyMinimalDto>;

    public selectedMonth: number;
    public selectedYear: number;
    public years: number[];
    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public monthOffset: number = 0; // Used to adjust the month index for the dropdown
    public months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public isLoadingSubmit: boolean = false;

    constructor(
        private geographyService: GeographyService,
        private waterMeasurementService: WaterMeasurementService,
        private alertService: AlertService,
        private reportingPeriodService: ReportingPeriodService
    ) {}

    public ngOnInit(): void {
        this.geography$ = this.geographyService.getByNameAsMinimalDtoGeography(this.ref.data.GeographyName);

        this.reportingPeriods$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                let defaultReportingPeriod = reportingPeriods.find((rp) => rp.IsDefault);
                if (!defaultReportingPeriod) {
                    defaultReportingPeriod = reportingPeriods[0];
                }

                this.years = [];

                reportingPeriods.forEach((reportingPeriod) => {
                    let year = new Date(reportingPeriod.EndDate).getFullYear();
                    this.years.push(year);
                });

                let reportingPeriod = this.ref.data.ReportingPeriodID
                    ? reportingPeriods.find((rp) => rp.ReportingPeriodID === this.ref.data.ReportingPeriodID)
                    : defaultReportingPeriod;

                let startDate = new Date(reportingPeriod.StartDate);
                let startMonth = startDate.getUTCMonth();
                this.monthOffset = startMonth;
                if (startMonth != 0) {
                    //Reorder months to start from the reporting period's start month
                    this.months = [...this.months.slice(startMonth), ...this.months.slice(0, startMonth)];
                }

                this.selectedYear = new Date(reportingPeriod.EndDate).getFullYear();

                let now = new Date();
                let currentMonth = now.getUTCMonth();
                this.selectedMonth = (currentMonth - this.monthOffset + 12) % 12;
            })
        );
    }

    public close() {
        this.ref.close(false);
    }

    save() {
        this.alertService.clearAlerts();
        this.isLoadingSubmit = true;
        let month = (this.selectedMonth + this.monthOffset) % 12; // Adjust month index to match the original month array
        this.waterMeasurementService
            .runAllCalculationsForGeographyWaterMeasurement(this.ref.data.GeographyID, this.selectedYear, month + 1, {
                UsageLocationIDs: this.ref.data.UsageLocationIDs,
            }) //Passing null for UsageLocationIDs to recalculate measurements for all usage locations
            .subscribe({
                next: () => {
                    this.isLoadingSubmit = false;
                    this.ref.close(null);
                    this.alertService.pushAlert(
                        new Alert(
                            `${this.ref.data.GeographyName} water measurements successfully recalculated for ${this.months[this.selectedMonth]} ${this.selectedYear}`,
                            AlertContext.Success
                        )
                    );
                },
                error: () => {
                    this.isLoadingSubmit = false;
                    this.ref.close();
                },
            });
    }
}

export class RefreshWaterMeasurementCalculationsContext {
    public GeographyID: number;
    public GeographyName: string;
    public GeographyStartYear: number;
    public UsageLocationIDs: number[] | null; // If null, recalculate measurements for all usage locations
    public ReportingPeriodID: number | null; // If null, use the default reporting period
}
