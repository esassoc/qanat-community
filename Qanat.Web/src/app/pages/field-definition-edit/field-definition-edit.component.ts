import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit, Input, OnDestroy } from "@angular/core";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { Router, ActivatedRoute, RouterLink } from "@angular/router";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { CustomRichTextDto } from "src/app/shared/generated/model/custom-rich-text-dto";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";
import { CustomRichTextSimpleDto } from "src/app/shared/generated/model/models";
import { FormsModule } from "@angular/forms";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "qanat-field-definition-edit",
    templateUrl: "./field-definition-edit.component.html",
    styleUrls: ["./field-definition-edit.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, AlertDisplayComponent, EditorComponent, FormsModule, RouterLink],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class FieldDefinitionEditComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() geographyID: number = null;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;
    private currentUser: UserDto;

    public fieldDefinition: CustomRichTextDto;
    public originalFieldDefinitionValue: string;

    isLoadingSubmit: boolean;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private customRichTextService: CustomRichTextService,
        private publicService: PublicService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef
    ) {}

    ngAfterViewInit(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
            const id = parseInt(this.route.snapshot.paramMap.get(routeParams.fieldDefinitionID));
            if (id) {
                this.publicService.publicCustomRichTextsCustomRichTextTypeIDGet(id).subscribe((fieldDefinition) => {
                    this.fieldDefinition = fieldDefinition;
                    this.originalFieldDefinitionValue = fieldDefinition.CustomRichTextContent;
                });
            }
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    canExit(): boolean {
        return this.fieldDefinition.CustomRichTextContent == this.originalFieldDefinitionValue;
    }

    saveDefinition(): void {
        this.isLoadingSubmit = true;

        const updateDto = new CustomRichTextSimpleDto({
            CustomRichTextTitle: this.fieldDefinition.CustomRichTextTitle,
            CustomRichTextContent: this.fieldDefinition.CustomRichTextContent,
            GeographyID: this.fieldDefinition.Geography?.GeographyID,
        });
        this.customRichTextService.customRichTextCustomRichTextTypeIDPut(this.fieldDefinition.CustomRichTextType.CustomRichTextTypeID, updateDto).subscribe({
            next: (response) => {
                this.isLoadingSubmit = false;
                this.fieldDefinition = response;
                this.originalFieldDefinitionValue = this.fieldDefinition.CustomRichTextContent;
                this.router.navigate(["/platform-admin/labels-and-definitions"]).then(() => {
                    this.alertService.pushAlert(
                        new Alert(`The definition for ${this.fieldDefinition.CustomRichTextType.CustomRichTextTypeDisplayName} was successfully updated.`, AlertContext.Success)
                    );
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }
}
