import { ChangeDetectorRef, Component, OnDestroy, OnInit, QueryList, ViewChildren } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { forkJoin, Subscription } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelLayerUpdateDto } from "src/app/shared/generated/model/parcel-layer-update-dto";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { FeatureClassInfo } from "src/app/shared/generated/model/feature-class-info";
import { ParcelUpdateExpectedResultsDto } from "src/app/shared/generated/model/parcel-update-expected-results-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { NgIf, NgFor } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

@Component({
    selector: "update-parcels-review",
    templateUrl: "./update-parcels-review.component.html",
    styleUrls: ["./update-parcels-review.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        NgIf,
        FormsModule,
        ReactiveFormsModule,
        NgFor,
        CustomRichTextComponent,
        ButtonComponent,
        ButtonLoadingDirective,
    ],
})
export class UpdateParcelsReviewComponent implements OnInit, OnDestroy {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    private geographyID: number;
    @ViewChildren("fileInput") public fileInput: QueryList<any>;

    public isLoadingSubmit: boolean;
    public customRichTextType: number = 41;
    public rowData = [];
    public columnDefs: ColDef[];
    public userID: number;
    public newParcelLayerForm = new UntypedFormGroup({
        gdbUploadForParcelLayer: new UntypedFormControl("", [Validators.required]),
    });
    public submitForPreviewForm = new UntypedFormGroup({
        waterYearSelection: new UntypedFormControl("", [Validators.required]),
    });
    public featureClass: Array<FeatureClassInfo>;
    public resultsPreview: ParcelUpdateExpectedResultsDto;
    public parcelLayerUpdateDto: ParcelLayerUpdateDto;
    public nextAvailableEffectiveYear: number;
    public availableYears: number[];

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
        private parcelService: ParcelService,
        private route: ActivatedRoute,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit() {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    getDataForGeographyID(geographyID: number) {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.userID = currentUser.UserID;
            forkJoin([
                this.parcelService.geographiesGeographyIDParcelGetFeatureClassInfoGet(geographyID),
                this.parcelService.geographiesGeographyIDEffectiveYearsGet(geographyID),
            ]).subscribe(([uploadParcelLayerInfoDto, availableYears]) => {
                this.featureClass = uploadParcelLayerInfoDto.FeatureClasses;
                this.nextAvailableEffectiveYear = Math.min(...availableYears);
                this.availableYears = availableYears;

                this.parcelLayerUpdateDto = new ParcelLayerUpdateDto({
                    ParcelLayerNameInGDB: this.featureClass[0].LayerName,
                    //UploadedGDBID: 1
                });
            });
        });
    }

    get f() {
        return this.newParcelLayerForm.controls;
    }

    get submitForPreviewFormControls() {
        return this.submitForPreviewForm.controls;
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    public getColumns(): Array<string> {
        if (!this.featureClass) {
            return [];
        }

        return this.featureClass[0].Columns;
    }

    public onSubmitForPreview() {
        this.isLoadingSubmit = true;

        this.parcelLayerUpdateDto.EffectiveYear = this.submitForPreviewForm.get("waterYearSelection").value;

        this.parcelService.geographiesGeographyIDParcelsPreviewGDBChangesPost(this.geographyID, this.parcelLayerUpdateDto).subscribe(
            (response) => {
                this.isLoadingSubmit = false;
                this.router.navigate(["../confirm"], { relativeTo: this.route });
            },
            () => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Failed to generate preview of changes!", AlertContext.Danger));
            }
        );
    }

    public previewFormValid(): boolean {
        if (!this.parcelLayerUpdateDto) return false;

        return (
            this.parcelLayerUpdateDto.ParcelNumberColumn != null &&
            this.parcelLayerUpdateDto.ParcelNumberColumn != undefined &&
            this.parcelLayerUpdateDto.OwnerNameColumn != null &&
            this.parcelLayerUpdateDto.OwnerNameColumn != undefined &&
            this.parcelLayerUpdateDto.OwnerAddressColumn != null &&
            this.parcelLayerUpdateDto.OwnerAddressColumn != undefined
        );
    }
}
