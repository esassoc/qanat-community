import { DOCUMENT, NgIf } from "@angular/common";
import { ChangeDetectorRef, Component, Inject, OnInit, QueryList, ViewChildren } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { WellService } from "src/app/shared/generated/api/well.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";

@Component({
    selector: "reference-wells-upload",
    templateUrl: "./reference-wells-upload.component.html",
    styleUrls: ["./reference-wells-upload.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, FormsModule, ReactiveFormsModule, NgIf, ButtonComponent, CustomRichTextComponent],
})
export class ReferenceWellsUploadComponent implements OnInit {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    private geographyID: number;
    @ViewChildren("fileInput") public fileInput: QueryList<any>;

    public isLoadingSubmit: boolean;
    public userID: number;
    public customRichTextType: number = CustomRichTextTypeEnum.ReferenceWellsUploader;
    public allowableFileTypes = ["gdb"];
    public maximumFileSizeMB = 30;
    public newParcelLayerForm = new UntypedFormGroup({
        gdbUploadForParcelLayer: new UntypedFormControl("", [Validators.required]),
    });
    public gdbInputFile: any = null;
    public uploadedGdbID: number;
    public currentWaterYear: number;
    public previousWaterYear: number;
    waterYearsNotPresentError: boolean;
    private fileUploadElement: HTMLElement;
    public fileUploadElementID = "gdbUploadForParcelLayer";

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
        private route: ActivatedRoute,
        private selectedGeographyService: SelectedGeographyService,
        private wellService: WellService,
        @Inject(DOCUMENT) private document: Document
    ) {}

    ngOnInit() {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.authenticationService.getCurrentUser().subscribe((currentUser) => {
                this.userID = currentUser.UserID;
            });
        });
    }

    get f() {
        return this.newParcelLayerForm.controls;
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    public onClickFileUpload() {
        if (!this.fileUploadElement) {
            this.fileUploadElement = document.getElementById(this.fileUploadElementID);
        }

        this.fileUploadElement.click();
    }

    private getSelectedFile(event: any) {
        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            //returns bytes, but I'd rather not handle a constant that's a huge value
            return event.target.files.item(0);
        }
        return null;
    }

    public onGDBFileChange(event: any) {
        this.gdbInputFile = this.getSelectedFile(event);
        this.newParcelLayerForm.get("gdbUploadForParcelLayer").setValue(this.gdbInputFile);
    }

    public getInputFileForGDB() {
        return this.gdbInputFile ? this.gdbInputFile.name : "No file chosen...";
    }

    public onSubmitGDB() {
        this.alertService.clearAlerts();
        if (!this.newParcelLayerForm.valid) {
            Object.keys(this.newParcelLayerForm.controls).forEach((field) => {
                const control = this.newParcelLayerForm.get(field);
                control.markAsTouched({ onlySelf: true });
            });
            return;
        }

        this.isLoadingSubmit = true;
        this.wellService.geographiesGeographyIDUploadReferenceWellsPost(this.geographyID, this.gdbInputFile).subscribe(
            (response) => {
                this.isLoadingSubmit = false;
                this.router.navigate(["../"], { relativeTo: this.route });
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Failed to upload GDB! If available, error details are below.", AlertContext.Danger));
                this.isLoadingSubmit = false;
            }
        );
    }
}
