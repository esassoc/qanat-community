import { Component, ComponentRef, inject } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

import { NoteComponent } from "src/app/shared/components/note/note.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";

@Component({
    selector: "fallow-submit-attestation-modal",
    imports: [NoteComponent, CustomRichTextComponent, FormsModule, IconComponent, FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./fallow-submit-attestation-modal.component.html",
    styleUrl: "./fallow-submit-attestation-modal.component.scss",
})
export class FallowSubmitAttestationModalComponent {
    public ref: DialogRef<CoverCropSubmitAttestationModalContext, boolean> = inject(DialogRef);

    public customRichTextTypeID = CustomRichTextTypeEnum.FallowSelfReportingSubmitAttestation;

    public FormFieldType = FormFieldType;
    public formControl = new FormControl<boolean>(false);
    public constructor() {}

    public confirm(): void {
        this.ref.close(true);
    }

    public close(): void {
        this.ref.close(false);
    }
}

export interface CoverCropSubmitAttestationModalContext {
    GeographyID: number;
}
