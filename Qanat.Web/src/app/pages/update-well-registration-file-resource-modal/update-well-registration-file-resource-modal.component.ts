import { Component, inject } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldType, FormFieldComponent } from "src/app/shared/components/forms/form-field/form-field.component";
import { WellRegistrationFileResourceService } from "src/app/shared/generated/api/well-registration-file-resource.service";
import {
    WellRegistrationFileResourceDto,
    WellRegistrationFileResourceDtoForm,
    WellRegistrationFileResourceDtoFormControls,
} from "src/app/shared/generated/model/well-registration-file-resource-dto";

@Component({
    selector: "update-well-registration-file-resource-modal",
    imports: [FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./update-well-registration-file-resource-modal.component.html",
    styleUrl: "./update-well-registration-file-resource-modal.component.scss",
})
export class UpdateWellRegistrationFileResourceModalComponent {
    public ref: DialogRef<WellRegistrationContext, WellRegistrationFileResourceDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<WellRegistrationFileResourceDtoForm>({
        WellRegistrationFileResourceID: WellRegistrationFileResourceDtoFormControls.WellRegistrationFileResourceID(),
        WellRegistrationID: WellRegistrationFileResourceDtoFormControls.WellRegistrationID(),
        FileResourceID: WellRegistrationFileResourceDtoFormControls.FileResourceID(),
        FileDescription: WellRegistrationFileResourceDtoFormControls.FileDescription(),
        FileResource: WellRegistrationFileResourceDtoFormControls.FileResource(),
    });

    constructor(private wellRegistrationFileResourceService: WellRegistrationFileResourceService) {}

    ngOnInit() {
        this.formGroup.patchValue(this.ref.data.wellRegistrationFileResourceToUpdate);
    }

    public updateWellRegistrationFileResource() {
        this.wellRegistrationFileResourceService
            .updateWellFileResourceWellRegistrationFileResource(
                this.ref.data.wellID,
                this.ref.data.wellRegistrationFileResourceToUpdate.WellRegistrationFileResourceID,
                this.formGroup.getRawValue()
            )
            .subscribe(() => {
                this.ref.close(this.formGroup.getRawValue());
            });
    }

    cancel(): void {
        this.ref.close(null);
    }
}

export class WellRegistrationContext {
    public wellID: number;
    public wellRegistrationFileResourceToUpdate: WellRegistrationFileResourceDto;
}
