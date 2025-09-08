import { AsyncPipe } from "@angular/common";
import { ChangeDetectorRef, Component, OnDestroy, OnInit, QueryList, ViewChildren } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, switchMap, tap } from "rxjs";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FeatureClassInfo } from "src/app/shared/generated/model/feature-class-info";
import { AlertService } from "src/app/shared/services/alert.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "update-parcels-upload",
    templateUrl: "./update-parcels-upload.component.html",
    styleUrls: ["./update-parcels-upload.component.scss"],
    imports: [AsyncPipe, PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ReactiveFormsModule, CustomRichTextComponent, ButtonComponent],
})
export class UpdateParcelsUploadComponent implements OnInit, OnDestroy {
    public geography$: Observable<GeographyMinimalDto>;

    @ViewChildren("fileInput") public fileInput: QueryList<any>;

    public isLoadingSubmit: boolean;
    public userID: number;
    public customRichTextType: number = 40;
    public allowableFileTypes = ["gdb"];
    public maximumFileSizeMB = 30;
    public newParcelLayerForm = new UntypedFormGroup({
        gdbUploadForParcelLayer: new UntypedFormControl("", [Validators.required]),
    });
    public submitForPreviewForm = new UntypedFormGroup({
        waterYearSelection: new UntypedFormControl("", [Validators.required]),
    });
    public gdbInputFile: any = null;
    public featureClass: FeatureClassInfo;
    public uploadedGdbID: number;
    public currentWaterYear: number;
    public previousWaterYear: number;
    public waterYearsNotPresentError: boolean;
    private fileUploadElement: HTMLElement;
    public fileUploadElementID = "gdbUploadForParcelLayer";

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
        private parcelByGeographyService: ParcelByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService
    ) {}

    ngOnInit() {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );
    }

    get f() {
        return this.newParcelLayerForm.controls;
    }

    get submitForPreviewFormControls() {
        return this.submitForPreviewForm.controls;
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

    public onSubmitGDB(geography: GeographyMinimalDto) {
        this.alertService.clearAlerts();
        if (!this.newParcelLayerForm.valid) {
            Object.keys(this.newParcelLayerForm.controls).forEach((field) => {
                const control = this.newParcelLayerForm.get(field);
                control.markAsTouched({ onlySelf: true });
            });
            return;
        }

        this.isLoadingSubmit = true;
        this.parcelByGeographyService.uploadGDBAndParseFeatureClassesParcelByGeography(geography.GeographyID, this.gdbInputFile).subscribe(
            () => {
                this.isLoadingSubmit = false;
                this.router.navigate(["../review-parcels"], { relativeTo: this.route });
            },
            () => {
                this.alertService.pushAlert(new Alert("Failed to upload GDB! If available, error details are above.", AlertContext.Danger));
                this.isLoadingSubmit = false;
            }
        );
    }
}
