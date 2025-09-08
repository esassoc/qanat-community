import { Component, inject } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldType, FormFieldComponent } from "src/app/shared/components/forms/form-field/form-field.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import {
    ReportingPeriodCoverCropSelfReportMetadataUpdateDtoForm,
    ReportingPeriodCoverCropSelfReportMetadataUpdateDtoFormControls,
} from "src/app/shared/generated/model/reporting-period-cover-crop-self-report-metadata-update-dto";
import { ReportingPeriodDto } from "src/app/shared/generated/model/reporting-period-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "edit-cover-crop-self-report-visibility-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./edit-cover-crop-self-report-visibility-modal.component.html",
    styleUrl: "./edit-cover-crop-self-report-visibility-modal.component.scss",
})
export class EditCoverCropSelfReportVisibilityModalComponent {
    public ref: DialogRef<EditCoverCropSelfReportVisibilityModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<ReportingPeriodCoverCropSelfReportMetadataUpdateDtoForm>({
        CoverCropSelfReportStartDate: ReportingPeriodCoverCropSelfReportMetadataUpdateDtoFormControls.CoverCropSelfReportStartDate(),
        CoverCropSelfReportEndDate: ReportingPeriodCoverCropSelfReportMetadataUpdateDtoFormControls.CoverCropSelfReportEndDate(),
        CoverCropSelfReportReadyForAccountHolders: ReportingPeriodCoverCropSelfReportMetadataUpdateDtoFormControls.CoverCropSelfReportReadyForAccountHolders(),
    });

    public constructor(
        private reportingPeriodService: ReportingPeriodService,
        private alertService: AlertService
    ) {
        let startDate = this.ref.data.ReportingPeriod.CoverCropSelfReportStartDate
            ? new Date(this.ref.data.ReportingPeriod.CoverCropSelfReportStartDate).toISOString().split("T")[0]
            : null;
        let endDate = this.ref.data.ReportingPeriod.CoverCropSelfReportEndDate
            ? new Date(this.ref.data.ReportingPeriod.CoverCropSelfReportEndDate).toISOString().split("T")[0]
            : null;

        this.formGroup.patchValue({
            CoverCropSelfReportStartDate: startDate,
            CoverCropSelfReportEndDate: endDate,
            CoverCropSelfReportReadyForAccountHolders: this.ref.data.ReportingPeriod.CoverCropSelfReportReadyForAccountHolders,
        });
    }

    public save(): void {
        let updateDto = this.formGroup.getRawValue();

        if (updateDto.CoverCropSelfReportStartDate === "") {
            updateDto.CoverCropSelfReportStartDate = null;
        }

        if (updateDto.CoverCropSelfReportEndDate === "") {
            updateDto.CoverCropSelfReportEndDate = null;
        }

        this.reportingPeriodService
            .updateCoverCropSelfReportMetadataReportingPeriod(this.ref.data.GeographyID, this.ref.data.ReportingPeriod.ReportingPeriodID, updateDto)
            .subscribe({
                next: () => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Updated cover crop self-report visibility successfully.", AlertContext.Success));
                    this.ref.close(true);
                },
                error: (error) => {},
            });
    }

    public cancel(): void {
        this.ref.close(false);
    }
}

export interface EditCoverCropSelfReportVisibilityModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
}
