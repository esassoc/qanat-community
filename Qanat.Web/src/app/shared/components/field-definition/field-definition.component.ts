import { Component, OnInit, Input, ChangeDetectorRef, ViewChild, AfterViewInit, OnDestroy } from "@angular/core";
import { Alert } from "../../models/alert";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "../../services/alert.service";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { PermissionEnum } from "../../generated/enum/permission-enum";
import { RightsEnum } from "../../models/enums/rights.enum";
import { CustomRichTextService } from "../../generated/api/custom-rich-text.service";
import { CustomRichTextDto } from "../../generated/model/custom-rich-text-dto";
import { CustomRichTextTypeEnum } from "../../generated/enum/custom-rich-text-type-enum";
import { CustomRichTextSimpleDto } from "../../generated/model/models";
import { PopperDirective } from "../../directives/popper.directive";
import { FormsModule } from "@angular/forms";
import { NgIf } from "@angular/common";

@Component({
    selector: "field-definition",
    templateUrl: "./field-definition.component.html",
    styleUrls: ["./field-definition.component.scss"],
    standalone: true,
    imports: [NgIf, EditorComponent, FormsModule, PopperDirective],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class FieldDefinitionComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() geographyID: number = null;
    @Input() fieldDefinitionType: string;
    @Input() labelOverride: string;
    @Input() inline: boolean = true;
    @Input() useBodyContainer: boolean = false;

    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    private currentUser: UserDto;

    public fieldDefinition: CustomRichTextDto;
    public isLoading: boolean = true;
    public isEditing: boolean = false;
    public emptyContent: boolean = false;

    public editedContent: string;

    constructor(
        private customRichTextService: CustomRichTextService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef,
        private alertService: AlertService
    ) {}

    ngAfterViewInit(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });
        this.customRichTextService.publicCustomRichTextCustomRichTextTypeIDGet(CustomRichTextTypeEnum[this.fieldDefinitionType]).subscribe((x) => this.loadFieldDefinition(x));
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    private loadFieldDefinition(fieldDefinition: CustomRichTextDto) {
        this.fieldDefinition = fieldDefinition;
        this.emptyContent = fieldDefinition.CustomRichTextContent?.length == 0;
        this.isLoading = false;
        this.cdr.detectChanges();
    }

    public getLabelText() {
        if (this.labelOverride !== null && this.labelOverride !== undefined) {
            return this.labelOverride;
        }

        return this.fieldDefinition.CustomRichTextType.CustomRichTextTypeDisplayName;
    }

    public showEditButton(): boolean {
        return this.authenticationService.hasPermission(this.currentUser, PermissionEnum.FieldDefinitionRights, RightsEnum.Update);
    }

    public enterEdit(event: any): void {
        event.preventDefault();

        this.editedContent = this.fieldDefinition.CustomRichTextContent;
        this.isEditing = true;
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    public saveEdit(): void {
        this.isEditing = false;
        this.isLoading = true;
        const updateDto = new CustomRichTextSimpleDto({
            CustomRichTextTitle: this.fieldDefinition.CustomRichTextTitle,
            CustomRichTextContent: this.editedContent,
            GeographyID: this.fieldDefinition.Geography?.GeographyID,
        });
        this.fieldDefinition.CustomRichTextContent = this.editedContent;
        this.customRichTextService.customRichTextCustomRichTextTypeIDPut(this.fieldDefinition.CustomRichTextType.CustomRichTextTypeID, updateDto).subscribe(
            (x) => this.loadFieldDefinition(x),
            (error) => {
                this.isLoading = false;
                this.alertService.pushAlert(new Alert("There was an error updating the field definition", AlertContext.Danger));
            }
        );
    }
}
