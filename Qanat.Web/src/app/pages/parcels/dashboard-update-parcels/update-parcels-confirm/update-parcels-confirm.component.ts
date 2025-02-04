import { ChangeDetectorRef, Component, ComponentRef, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, switchMap, tap } from "rxjs";
import { ParcelLayerUpdateDto } from "src/app/shared/generated/model/parcel-layer-update-dto";
import { ParcelUpdateExpectedResultsDto } from "src/app/shared/generated/model/parcel-update-expected-results-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NgIf, DecimalPipe, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyPublicDto } from "src/app/shared/generated/model/geography-public-dto";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";

@Component({
    selector: "update-parcels-confirm",
    templateUrl: "./update-parcels-confirm.component.html",
    styleUrls: ["./update-parcels-confirm.component.scss"],
    standalone: true,
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        NgIf,
        FormsModule,
        ReactiveFormsModule,
        CustomRichTextComponent,
        ButtonComponent,
        DecimalPipe,
        AlertDisplayComponent,
    ],
})
export class UpdateParcelsConfirmComponent implements OnInit, OnDestroy {
    public geography$: Observable<GeographyPublicDto>;
    public resultsPreview$: Observable<ParcelUpdateExpectedResultsDto>;

    public modalReference: ComponentRef<ModalComponent>;

    public parcelLayerUpdateDto: ParcelLayerUpdateDto;
    public submitForPreviewForm = new UntypedFormGroup({
        waterYearSelection: new UntypedFormControl("", [Validators.required]),
    });

    public customRichTextType: CustomRichTextTypeEnum = CustomRichTextTypeEnum.UpdateParcelsConfirm;
    public expectedResultsRetrievedSuccessfully: boolean = false;
    public isLoadingSubmit: boolean;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private parcelByGeographyService: ParcelByGeographyService,
        private modalService: ModalService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private viewContainerRef: ViewContainerRef,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit() {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );

        this.resultsPreview$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelByGeographyService.geographiesGeographyIDParcelsGetExpectedResultsGet(geography.GeographyID);
            }),
            tap((results) => {
                this.expectedResultsRetrievedSuccessfully = true;
            })
        );
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    get submitForPreviewFormControls() {
        return this.submitForPreviewForm.controls;
    }

    public onSubmitChanges(geography: GeographyMinimalDto) {
        if (this.modalReference) {
            this.modalService.close(this.modalReference);
            this.modalReference = null;
        }

        this.isLoadingSubmit = true;
        this.parcelByGeographyService.geographiesGeographyIDParcelsEnactGDBChangesPost(geography.GeographyID).subscribe(
            () => {
                this.isLoadingSubmit = false;
                this.router.navigate(["../../update"], { relativeTo: this.route }).then(() => {
                    this.alertService.pushAlert(new Alert(`Successfully updated parcels.`, AlertContext.Success));
                });
            },
            () => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Failed enact GDB changes", AlertContext.Danger));
            }
        );
    }

    public launchModal(template: TemplateRef<any>): void {
        this.modalReference = this.modalService.open(template, this.viewContainerRef);
    }

    public close(): void {
        if (!this.modalReference) return;
        this.modalService.close(this.modalReference);
    }
}
