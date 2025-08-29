import { Component, inject, OnInit } from "@angular/core";
import { FormFieldComponent, FormFieldType } from "../forms/form-field/form-field.component";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "basic-rich-text-editor-modal",
    imports: [FormFieldComponent, FormsModule, ReactiveFormsModule],
    templateUrl: "./basic-rich-text-editor-modal.component.html",
    styleUrl: "./basic-rich-text-editor-modal.component.scss",
})
export class BasicRichTextEditorModalComponent implements OnInit {
    public ref: DialogRef<BasicRichTextEditorModalContext, string> = inject(DialogRef);

    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<{ HtmlContent: FormControl<string> }> = new FormGroup<{ HtmlContent: FormControl<string> }>({
        HtmlContent: new FormControl<string>(null),
    });

    constructor() {}

    public ngOnInit(): void {
        this.formGroup.controls.HtmlContent.patchValue(this.ref.data.HtmlContent);
    }

    save(): void {
        this.ref.close(this.formGroup.controls.HtmlContent.getRawValue());
    }

    cancel(): void {
        this.ref.close(null);
    }
}

export class BasicRichTextEditorModalContext {
    Header: string;
    HtmlContent: string;
}
