import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, FormsModule, FormControl, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "../../forms/form-field/form-field.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "file-description-update-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./file-description-update-modal.component.html",
    styleUrl: "./file-description-update-modal.component.scss",
})
export class FileDescriptionUpdateModalComponent implements OnInit {
    public ref: DialogRef<any, any> = inject(DialogRef);
    FormFieldType = FormFieldType;

    public formGroup: FormGroup<FileDescriptionUpdateForm> = new FormGroup<FileDescriptionUpdateForm>({
        FileDescription: new FormControl<string>(""),
    });

    ngOnInit(): void {
        this.formGroup.setValue({
            FileDescription: this.ref.data.FileResource.FileDescription,
        });
    }

    submitFileUpdate(): void {
        this.ref.data.FileResource.FileDescription = this.formGroup.get("FileDescription").value;
        this.ref.close(this.ref.data.FileResource);
    }

    close(): void {
        this.ref.close(null);
    }
}

export interface FileDescriptionUpdateForm {
    FileDescription?: FormControl<string>;
}
