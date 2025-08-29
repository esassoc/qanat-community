import { Component, EventEmitter, inject, Output, ViewChild } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "../../forms/form-field/form-field.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "file-upload-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./file-upload-modal.component.html",
    styleUrl: "./file-upload-modal.component.scss",
})
export class FileUploadModalComponent {
    public ref: DialogRef<any, IFileResourceUpload> = inject(DialogRef);
    FormFieldType = FormFieldType;

    @ViewChild("fileUploadField") fileUploadField: any;
    @Output() fileChanged = new EventEmitter<File>();

    //pdf, jpg, png, csv, txt, xlsx, docx, doc
    public allowedFileTypes: string[] = [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "text/csv",
        "text/plain",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    ];

    public file: File;

    public formGroup: FormGroup<FileResourceUploadForm> = new FormGroup<FileResourceUploadForm>({
        File: new FormControl<File>(null),
        FileDescription: new FormControl<string>(""),
    });

    onClickFileUpload(event: any): void {
        const fileUploadInput = this.fileUploadField.nativeElement;
        fileUploadInput.click();
    }

    updateFile(event: any): void {
        this.file = event.target.files.item(0);
        this.fileChanged.emit(this.file);
    }

    submitFileResourceUpload(): void {
        let fileResourceUpload: IFileResourceUpload = {
            File: this.file,
            FileDescription: this.formGroup.get("FileDescription").value,
        };

        this.ref.close(fileResourceUpload);
    }

    close(): void {
        this.ref.close(null);
    }
}

export interface IFileResourceUpload {
    File: File;
    FileDescription: string;
}

export interface FileResourceUploadForm {
    File: FormControl<File>;
    FileDescription: FormControl<string>;
}
