import { Component, inject } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldType, FormFieldComponent } from "src/app/shared/components/forms/form-field/form-field.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import {
    ReportingPeriodFallowSelfReportMetadataUpdateDtoForm,
    ReportingPeriodFallowSelfReportMetadataUpdateDtoFormControls,
    ReportingPeriodDto,
} from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "edit-fallowing-self-report-visibility-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./edit-fallowing-self-report-visibility-modal.component.html",
    styleUrl: "./edit-fallowing-self-report-visibility-modal.component.scss",
})
export class EditFallowingSelfReportVisibilityModalComponent {
    public ref: DialogRef<EditFallowSelfReportVisibilityModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<ReportingPeriodFallowSelfReportMetadataUpdateDtoForm>({
        FallowSelfReportStartDate: ReportingPeriodFallowSelfReportMetadataUpdateDtoFormControls.FallowSelfReportStartDate(),
        FallowSelfReportEndDate: ReportingPeriodFallowSelfReportMetadataUpdateDtoFormControls.FallowSelfReportEndDate(),
        FallowSelfReportReadyForAccountHolders: ReportingPeriodFallowSelfReportMetadataUpdateDtoFormControls.FallowSelfReportReadyForAccountHolders(),
    });

    public constructor(
        private reportingPeriodService: ReportingPeriodService,
        private alertService: AlertService
    ) {
        let startDate = this.ref.data.ReportingPeriod.FallowSelfReportStartDate
            ? new Date(this.ref.data.ReportingPeriod.FallowSelfReportStartDate).toISOString().split("T")[0]
            : null;
        let endDate = this.ref.data.ReportingPeriod.FallowSelfReportEndDate ? new Date(this.ref.data.ReportingPeriod.FallowSelfReportEndDate).toISOString().split("T")[0] : null;

        this.formGroup.patchValue({
            FallowSelfReportStartDate: startDate,
            FallowSelfReportEndDate: endDate,
            FallowSelfReportReadyForAccountHolders: this.ref.data.ReportingPeriod.FallowSelfReportReadyForAccountHolders,
        });
    }

    public save(): void {
        let updateDto = this.formGroup.getRawValue();

        if (updateDto.FallowSelfReportStartDate === "") {
            updateDto.FallowSelfReportStartDate = null;
        }

        if (updateDto.FallowSelfReportEndDate === "") {
            updateDto.FallowSelfReportEndDate = null;
        }

        this.reportingPeriodService.updateFallowSelfReportMetadataReportingPeriod(this.ref.data.GeographyID, this.ref.data.ReportingPeriod.ReportingPeriodID, updateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Updated fallowing self-report visibility successfully.", AlertContext.Success));
                this.ref.close(true);
            },
            error: (error) => {},
        });
    }

    public cancel(): void {
        this.ref.close(false);
    }
}

export interface EditFallowSelfReportVisibilityModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
}
