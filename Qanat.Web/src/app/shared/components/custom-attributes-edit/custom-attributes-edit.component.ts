import { Component, Input } from "@angular/core";
import { CustomAttributeSimpleDto } from "src/app/shared/generated/model/custom-attribute-simple-dto";
import { FormsModule } from "@angular/forms";
import { NgFor, NgIf } from "@angular/common";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "custom-attributes-edit",
    templateUrl: "./custom-attributes-edit.component.html",
    styleUrl: "./custom-attributes-edit.component.scss",
    standalone: true,
    imports: [NgFor, FormsModule, IconComponent, NgIf],
})
export class CustomAttributesEditComponent {
    @Input() customAttributes: CustomAttributeSimpleDto[];
    @Input() geographyID: number;
    @Input() customAttributeTypeID: number;
    @Input() readonly: boolean = false;

    public newCustomAttributeName: string;
    public isLoadingSubmit: boolean = false;

    public createNewCustomAttribute() {
        this.customAttributes.push({ GeographyID: this.geographyID, CustomAttributeName: this.newCustomAttributeName });
        this.newCustomAttributeName = "";
    }

    public removeCustomAttribute(index: number) {
        this.customAttributes.splice(index, 1);
    }
}
