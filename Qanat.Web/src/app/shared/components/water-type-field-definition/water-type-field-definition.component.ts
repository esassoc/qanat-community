import { Component, Input, ViewChild, AfterViewInit } from "@angular/core";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { WaterTypeSimpleDto } from "../../generated/model/water-type-simple-dto";
import { PopperDirective } from "../../directives/popper.directive";
import { FormsModule } from "@angular/forms";
import { NgIf } from "@angular/common";

@Component({
    selector: "water-type-field-definition",
    templateUrl: "./water-type-field-definition.component.html",
    styleUrls: ["./water-type-field-definition.component.scss"],
    standalone: true,
    imports: [NgIf, EditorComponent, FormsModule, PopperDirective],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class WaterTypeFieldDefinitionComponent implements AfterViewInit {
    @Input() waterType: WaterTypeSimpleDto;
    @Input() labelOverride: string;
    @Input() editable: boolean = false;
    @Input() white: boolean = false;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;
    public editedContent: string;

    public isEditing: boolean = false;

    constructor() {}

    ngAfterViewInit(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    public enterEdit(event: any, waterType: WaterTypeSimpleDto): void {
        event.preventDefault();

        this.editedContent = waterType.WaterTypeDefinition;
        this.isEditing = true;
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    public saveEdit(waterType: WaterTypeSimpleDto): void {
        this.isEditing = false;
        waterType.WaterTypeDefinition = this.editedContent;
    }
}
