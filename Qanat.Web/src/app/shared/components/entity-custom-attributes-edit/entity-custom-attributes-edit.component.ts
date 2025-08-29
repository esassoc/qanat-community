import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType } from "../forms/form-field/form-field.component";
import { EntityCustomAttributesDto } from "../../generated/model/entity-custom-attributes-dto";
import { RouterLink } from "@angular/router";
import { ButtonLoadingDirective } from "../../directives/button-loading.directive";
import { KeyValuePipe } from "@angular/common";

@Component({
    selector: "entity-custom-attributes-edit",
    templateUrl: "./entity-custom-attributes-edit.component.html",
    styleUrl: "./entity-custom-attributes-edit.component.scss",
    imports: [FormsModule, ReactiveFormsModule, ButtonLoadingDirective, RouterLink, KeyValuePipe]
})
export class EntityCustomAttributesEditComponent implements OnInit {
    @Input() entityCustomAttributes: EntityCustomAttributesDto;
    @Input() isLoadingSubmit: boolean = false;

    @Output() save = new EventEmitter<{ [key: string]: string }>();

    public formGroup: FormGroup;
    public FormFieldType = FormFieldType;

    public formGroupValuesOnLoad: string;

    ngOnInit(): void {
        const formGroup = new FormGroup({});

        for (const key of Object.keys(this.entityCustomAttributes.CustomAttributes)) {
            formGroup.addControl(key, new FormControl(this.entityCustomAttributes.CustomAttributes[key]));
        }
        this.formGroup = formGroup;
        this.formGroupValuesOnLoad = JSON.stringify(this.formGroup?.getRawValue());
    }

    public canExit(): boolean {
        return JSON.stringify(this.formGroup?.getRawValue()) == this.formGroupValuesOnLoad;
    }

    public onSave() {
        this.formGroupValuesOnLoad = JSON.stringify(this.formGroup?.getRawValue());
        this.save.emit(this.formGroup.getRawValue());
    }
}
