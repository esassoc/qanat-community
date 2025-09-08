import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Subscription } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ReportingPeriodDto, ReportingPeriodUpsertDto, ReportingPeriodUpsertDtoForm, ReportingPeriodUpsertDtoFormControls } from "src/app/shared/generated/model/models";

@Component({
    selector: "upsert-reporting-period-modal",
    imports: [ReactiveFormsModule, FormFieldComponent, AlertDisplayComponent],
    templateUrl: "./upsert-reporting-period-modal.component.html",
    styleUrl: "./upsert-reporting-period-modal.component.scss"
})
export class UpsertReportingPeriodModalComponent implements OnInit, OnDestroy {
    public ref: DialogRef<UpsertReportingPeriodModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public isNewReportingPeriod: boolean = true;
    public endDate: string;
    public addOrEditSubscription: Subscription;

    public formGroup = new FormGroup<ReportingPeriodUpsertDtoForm>({
        Name: ReportingPeriodUpsertDtoFormControls.Name(),
        StartDate: ReportingPeriodUpsertDtoFormControls.StartDate(),
        ReadyForAccountHolders: ReportingPeriodUpsertDtoFormControls.ReadyForAccountHolders(),
        IsDefault: ReportingPeriodUpsertDtoFormControls.IsDefault(),
    });

    public endDateControl = new FormControl({}, []);
    public startDateChangeSubscription: Subscription;

    constructor(private reportingPeriodService: ReportingPeriodService) {}

    ngOnInit(): void {
        if (this.ref.data.ReportingPeriod) {
            this.ref.data.ReportingPeriod.StartDate = new Date(this.ref.data.ReportingPeriod.StartDate).toISOString().split("T")[0];
            this.formGroup.patchValue(this.ref.data.ReportingPeriod);

            this.endDate = new Date(this.ref.data.ReportingPeriod.EndDate).toISOString().split("T")[0];
            this.endDateControl.setValue(this.endDate);

            if (this.ref.data?.ReportingPeriod?.IsDefault) {
                this.formGroup.get("IsDefault").disable();
            }

            this.isNewReportingPeriod = false;
        }

        this.endDateControl.disable();

        this.startDateChangeSubscription = this.formGroup.get("StartDate").valueChanges.subscribe((startDate: string) => {
            this.calculateEndDate(startDate);
        });
    }

    ngOnDestroy(): void {
        if (this.addOrEditSubscription && this.addOrEditSubscription.unsubscribe) {
            this.addOrEditSubscription.unsubscribe();
        }

        if (this.startDateChangeSubscription && this.startDateChangeSubscription.unsubscribe) {
            this.startDateChangeSubscription.unsubscribe();
        }
    }

    calculateEndDate(startDate: string): void {
        let endDate = new Date(startDate);
        // set end date to the next year minus one day
        endDate.setFullYear(endDate.getFullYear() + 1);
        endDate.setDate(endDate.getDate() - 1);

        this.endDate = endDate.toISOString().split("T")[0];
        this.endDateControl.setValue(this.endDate);
    }

    save(): void {
        let upsertDto = this.formGroup.value as ReportingPeriodUpsertDto;
        if (this.formGroup.get("IsDefault").value) {
            upsertDto.IsDefault = true; //MK 3/3/2025 -- Need to set it explicity because if it's disabled its exlcuded from the formGroup.value.
        }

        if (this.isNewReportingPeriod) {
            this.addOrEditSubscription = this.reportingPeriodService.createReportingPeriod(this.ref.data.GeographyID, upsertDto).subscribe((response) => {
                this.ref.close(true);
            });
        } else {
            this.addOrEditSubscription = this.reportingPeriodService
                .updateReportingPeriod(this.ref.data.GeographyID, this.ref.data.ReportingPeriod.ReportingPeriodID, upsertDto)
                .subscribe((response) => {
                    this.ref.close(true);
                });
        }
    }

    cancel(): void {
        this.ref.close(false);
    }
}

export class UpsertReportingPeriodModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
}
