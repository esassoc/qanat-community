import { ChangeDetectorRef, Component, ComponentRef, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Subscription } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ParcelLayerUpdateDto } from "src/app/shared/generated/model/parcel-layer-update-dto";
import { ParcelUpdateExpectedResultsDto } from "src/app/shared/generated/model/parcel-update-expected-results-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NgIf, DecimalPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";

@Component({
    selector: "update-parcels-confirm",
    templateUrl: "./update-parcels-confirm.component.html",
    styleUrls: ["./update-parcels-confirm.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, NgIf, FormsModule, ReactiveFormsModule, CustomRichTextComponent, ButtonComponent, DecimalPipe],
})
export class UpdateParcelsConfirmComponent implements OnInit, OnDestroy {
    private selectedGeography$: Subscription = Subscription.EMPTY;

    private geographyID: number;
    public modalReference: ComponentRef<ModalComponent>;

    public currentUserID: number;
    public parcelLayerUpdateDto: ParcelLayerUpdateDto;
    public resultsPreview: ParcelUpdateExpectedResultsDto;
    public submitForPreviewForm = new UntypedFormGroup({
        waterYearSelection: new UntypedFormControl("", [Validators.required]),
    });

    public customRichTextType: CustomRichTextTypeEnum = CustomRichTextTypeEnum.UpdateParcelsConfirm;
    public expectedResultsRetrievedSuccessfully: boolean = false;
    public isLoadingSubmit: boolean;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
        private parcelService: ParcelService,
        private modalService: ModalService,
        private selectedGeographyService: SelectedGeographyService,
        private route: ActivatedRoute,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit() {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    getDataForGeographyID(geographyID: number) {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUserID = currentUser.UserID;

            this.parcelService.geographiesGeographyIDParcelsGetExpectedResultsGet(geographyID).subscribe((expectedResults) => {
                this.resultsPreview = expectedResults;
                this.expectedResultsRetrievedSuccessfully = true;
            });
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    get submitForPreviewFormControls() {
        return this.submitForPreviewForm.controls;
    }

    public onSubmitChanges() {
        if (this.modalReference) {
            this.modalService.close(this.modalReference);
            this.modalReference = null;
        }

        this.isLoadingSubmit = true;
        this.parcelService.geographiesGeographyIDParcelsEnactGDBChangesPost(this.geographyID).subscribe(
            () => {
                this.isLoadingSubmit = false;
                this.router.navigate(["../../parcels/update"], { relativeTo: this.route }).then(() => {
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
