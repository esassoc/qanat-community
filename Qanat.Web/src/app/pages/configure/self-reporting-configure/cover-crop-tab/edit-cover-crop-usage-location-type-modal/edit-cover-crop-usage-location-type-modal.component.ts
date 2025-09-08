import { Component, inject } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldType, FormFieldComponent } from "src/app/shared/components/forms/form-field/form-field.component";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";
import {
    UsageLocationTypeDto,
    UsageLocationTypeUpdateCoverCropMetadataDtoForm,
    UsageLocationTypeUpdateCoverCropMetadataDtoFormControls,
} from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "edit-cover-crop-usage-location-type-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./edit-cover-crop-usage-location-type-modal.component.html",
    styleUrl: "./edit-cover-crop-usage-location-type-modal.component.scss",
})
export class EditCoverCropUsageLocationTypeModalComponent {
    public ref: DialogRef<EditCoverCropUsageLocationTypeModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<UsageLocationTypeUpdateCoverCropMetadataDtoForm>({
        CanBeSelectedInCoverCropForm: UsageLocationTypeUpdateCoverCropMetadataDtoFormControls.CanBeSelectedInCoverCropForm(),
        CountsAsCoverCropped: UsageLocationTypeUpdateCoverCropMetadataDtoFormControls.CountsAsCoverCropped(),
    });

    public constructor(
        private usageLocationTypeService: UsageLocationTypeService,
        private alertService: AlertService
    ) {
        this.formGroup.patchValue({
            CanBeSelectedInCoverCropForm: this.ref.data.UsageLocationType.CanBeSelectedInCoverCropForm,
            CountsAsCoverCropped: this.ref.data.UsageLocationType.CountsAsCoverCropped,
        });
    }

    public save(): void {
        this.usageLocationTypeService
            .updateCoverCropMetadataUsageLocationType(this.ref.data.GeographyID, this.ref.data.UsageLocationType.UsageLocationTypeID, this.formGroup.getRawValue())
            .subscribe({
                next: () => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Updated usage location type successfully.", AlertContext.Success));
                    this.ref.close(true);
                },
                error: (error) => {},
            });
    }

    public cancel(): void {
        this.ref.close(false);
    }
}

export interface EditCoverCropUsageLocationTypeModalContext {
    GeographyID: number;
    UsageLocationType: UsageLocationTypeDto;
}
