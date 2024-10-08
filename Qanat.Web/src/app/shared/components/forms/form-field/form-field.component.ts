import { Component, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { TinyMceConfigPipe } from "../../../pipes/tiny-mce-config.pipe";
import { RequiredPipe } from "../../../pipes/required.pipe";
import { InputErrorsComponent } from "../../inputs/input-errors/input-errors.component";
import { FieldDefinitionComponent } from "../../field-definition/field-definition.component";
import { SelectDropdownComponent } from "../../inputs/select-dropdown/select-dropdown.component";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { NgSwitch, NgSwitchCase, NgIf, NgFor } from "@angular/common";
import { NgxMaskDirective, provideNgxMask } from "ngx-mask";

@Component({
    selector: "form-field",
    templateUrl: "./form-field.component.html",
    styleUrls: ["./form-field.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: FormFieldComponent,
            multi: true,
        },
        {
            provide: TINYMCE_SCRIPT_SRC,
            useValue: "tinymce/tinymce.min.js",
        },
        provideNgxMask(),
    ],
    standalone: true,
    imports: [
        NgSwitch,
        NgSwitchCase,
        NgxMaskDirective,
        FormsModule,
        ReactiveFormsModule,
        NgIf,
        EditorComponent,
        SelectDropdownComponent,
        NgFor,
        FieldDefinitionComponent,
        InputErrorsComponent,
        RequiredPipe,
        TinyMceConfigPipe,
    ],
})
export class FormFieldComponent {
    public FormFieldType = FormFieldType;
    @Output() change = new EventEmitter<any>();
    @Input() formControl: FormControl;
    @Input() fieldLabel: string = "";
    @Input() placeholder: string = "";
    @Input() type: FormFieldType = FormFieldType.Text;
    @Input() formInputOptions: FormInputOption[];
    @Input() toggleTrue: string = "On";
    @Input() toggleFalse: string = "Off";
    @Input() checkLabel: string;
    @Input() units: string;
    @Input() name: string;
    @Input() fieldDefinitionName: string;
    @Input() toggleHeight: string = "";
    @Input() mask: string;
    /**
     * comma separated list of file types: https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#accept
     */
    @Input() uploadFileAccepts: string;

    @ViewChild("fileUploadField") fileUploadField: any;

    public val: any;
    set value(val) {
        this.val = val;
        this.change.emit(val);

        this.onChange(val);
        this.onTouch(val);
    }

    public isDisabled: boolean = false;

    onChange: any = () => {};
    onTouch: any = () => {};

    writeValue(value: any): void {
        this.val = value;
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouch = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    onFileChange(event: any): void {
        this.value = event.target.files[0];
    }

    onClickFileUpload(event: any): void {
        const fileUploadInput = this.fileUploadField.nativeElement;
        fileUploadInput.click();
    }
}

export enum FormFieldType {
    Text = "text",
    Textarea = "textarea",
    Check = "check",
    Toggle = "toggle",
    Date = "date",
    Select = "select",
    Number = "number",
    Radio = "radio",
    RTE = "rte",
    File = "file",
}

export interface FormInputOption {
    Value: any;
    Label: string;
    Disabled: boolean | null | undefined;
}
