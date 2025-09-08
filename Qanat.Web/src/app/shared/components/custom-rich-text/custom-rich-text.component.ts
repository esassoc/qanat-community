import { Component, OnInit, Input, ViewChild, AfterViewChecked, ChangeDetectorRef, OnDestroy, OnChanges, SimpleChanges, ElementRef } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "../../services/alert.service";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../../helpers/tiny-mce-helpers";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { CustomRichTextDto } from "src/app/shared/generated/model/custom-rich-text-dto";
import { PermissionEnum } from "../../generated/enum/permission-enum";
import { RightsEnum } from "../../models/enums/rights.enum";
import { ContentTypeEnum } from "../../generated/enum/content-type-enum";
import { CustomRichTextSimpleDto } from "../../generated/model/custom-rich-text-simple-dto";
import { UserDto } from "../../generated/model/models";
import { CustomRichTextService } from "../../generated/api/custom-rich-text.service";
import { GeographyEnum } from "../../models/enums/geography.enum";
import { FormsModule } from "@angular/forms";

import { LoadingDirective } from "../../directives/loading.directive";
import { IconComponent } from "../icon/icon.component";
import { PublicService } from "../../generated/api/public.service";

@Component({
    selector: "custom-rich-text",
    templateUrl: "./custom-rich-text.component.html",
    styleUrls: ["./custom-rich-text.component.scss"],
    imports: [LoadingDirective, IconComponent, FormsModule, EditorComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
})
export class CustomRichTextComponent implements OnInit, AfterViewChecked, OnDestroy, OnChanges {
    @ViewChild("displayContent") private displayContent?: ElementRef<HTMLElement>;

    @Input() geographyID: number = null;
    public GeographyEnum = GeographyEnum;
    @Input() customRichTextTypeID: number;
    @Input() showLoading: boolean = true;
    @Input() showInfoIcon: boolean = true;
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    private currentUser: UserDto;

    public customRichTextTitle: string;
    public editedTitle: string;
    public showTitle: boolean = false;

    public customRichTextContent: SafeHtml;
    public editedContent: string;
    public isEmptyContent: boolean = false;

    public isLoading: boolean = true;
    public isEditing: boolean = false;

    constructor(
        private customRichTextService: CustomRichTextService,
        private publicService: PublicService,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private sanitizer: DomSanitizer,
        private cdr: ChangeDetectorRef
    ) {}

    ngAfterViewChecked(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });

        this.publicService.getCustomRichTextPublic(this.customRichTextTypeID, this.geographyID).subscribe((x) => {
            this.loadCustomRichText(x);
        });
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.geographyID && changes.geographyID.currentValue) {
            this.isLoading = true;
            this.publicService.getCustomRichTextPublic(this.customRichTextTypeID, changes.geographyID.currentValue).subscribe((x) => {
                this.loadCustomRichText(x);
            });
        }
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    private loadCustomRichText(customRichText: CustomRichTextDto) {
        this.customRichTextTitle = customRichText.CustomRichTextTitle;
        this.editedTitle = this.customRichTextTitle;

        this.customRichTextContent = this.sanitizer.bypassSecurityTrustHtml(customRichText.CustomRichTextContent);
        this.editedContent = customRichText.CustomRichTextContent;
        this.isEmptyContent = customRichText.IsEmptyContent;

        if (customRichText.CustomRichTextType?.ContentTypeID == ContentTypeEnum.FormInstructions) {
            this.showTitle = true;
        }

        this.runCodeHighlighting();
        this.isLoading = false;
        this.cdr.detectChanges();
    }

    private runCodeHighlighting() {
        if (this.isEditing) {
            return;
        }
        const prism = (window as any).Prism;
        const host = this.displayContent?.nativeElement;
        if (!prism || !host) {
            return;
        }

        // Defer so [innerHTML] has committed
        setTimeout(() => prism.highlightAllUnder(host), 0);
    }

    public showEditButton(): boolean {
        return this.authenticationService.hasPermission(this.currentUser, PermissionEnum.CustomRichTextRights, RightsEnum.Update);
    }

    public enterEdit(): void {
        this.isEditing = true;
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    public saveEdit(): void {
        this.isEditing = false;
        this.isLoading = true;
        const updateDto = new CustomRichTextSimpleDto({ CustomRichTextTitle: this.editedTitle, CustomRichTextContent: this.editedContent, GeographyID: this.geographyID });
        this.customRichTextService.updateCustomRichTextCustomRichText(this.customRichTextTypeID, updateDto).subscribe(
            (x) => {
                this.loadCustomRichText(x);
            },
            (error) => {
                this.isLoading = false;
                this.alertService.pushAlert(new Alert("There was an error updating the rich text content", AlertContext.Danger, true));
            }
        );
    }
}
