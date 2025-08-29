import { Component, inject } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";
import { UsageLocationTypeDto, UsageLocationTypeUpdateFallowMetadataDtoForm, UsageLocationTypeUpdateFallowMetadataDtoFormControls } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "edit-fallowing-usage-location-type-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./edit-fallowing-usage-location-type-modal.component.html",
    styleUrl: "./edit-fallowing-usage-location-type-modal.component.scss",
})
export class EditFallowingUsageLocationTypeModalComponent {
    public ref: DialogRef<EditFallowUsageLocationTypeModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<UsageLocationTypeUpdateFallowMetadataDtoForm>({
        CanBeSelectedInFallowForm: UsageLocationTypeUpdateFallowMetadataDtoFormControls.CanBeSelectedInFallowForm(),
        CountsAsFallowed: UsageLocationTypeUpdateFallowMetadataDtoFormControls.CountsAsFallowed(),
    });

    public constructor(
        private usageLocationTypeService: UsageLocationTypeService,
        private alertService: AlertService
    ) {
        this.formGroup.patchValue({
            CanBeSelectedInFallowForm: this.ref.data.UsageLocationType.CanBeSelectedInFallowForm,
            CountsAsFallowed: this.ref.data.UsageLocationType.CountsAsFallowed,
        });
    }

    public save(): void {
        this.usageLocationTypeService
            .updateFallowMetadataUsageLocationType(this.ref.data.GeographyID, this.ref.data.UsageLocationType.UsageLocationTypeID, this.formGroup.getRawValue())
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

export interface EditFallowUsageLocationTypeModalContext {
    GeographyID: number;
    UsageLocationType: UsageLocationTypeDto;
}
