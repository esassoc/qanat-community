import { Component, inject } from "@angular/core";
import { NoteComponent } from "../../../shared/components/note/note.component";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "cover-crop-submit-attestation-modal",
    imports: [NoteComponent, CustomRichTextComponent, FormsModule, IconComponent, FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./cover-crop-submit-attestation-modal.component.html",
    styleUrl: "./cover-crop-submit-attestation-modal.component.scss",
})
export class CoverCropSubmitAttestationModalComponent {
    public ref: DialogRef<CoverCropSubmitAttestationModalContext, boolean> = inject(DialogRef);

    public customRichTextTypeID = CustomRichTextTypeEnum.CoverCropSelfReportingSubmitAttestation;

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
