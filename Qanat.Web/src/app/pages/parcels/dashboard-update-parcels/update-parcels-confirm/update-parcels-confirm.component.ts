import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, switchMap, tap } from "rxjs";
import { ParcelLayerUpdateDto } from "src/app/shared/generated/model/parcel-layer-update-dto";
import { ParcelUpdateExpectedResultsDto } from "src/app/shared/generated/model/parcel-update-expected-results-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { DecimalPipe, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyPublicDto } from "src/app/shared/generated/model/geography-public-dto";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

@Component({
    selector: "update-parcels-confirm",
    templateUrl: "./update-parcels-confirm.component.html",
    styleUrls: ["./update-parcels-confirm.component.scss"],
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
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
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private cdr: ChangeDetectorRef,
        private confirmService: ConfirmService
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

        this.resultsPreview$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelByGeographyService.getExpectedResultsParcelByGeography(geography.GeographyID);
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
        this.isLoadingSubmit = true;
        this.parcelByGeographyService.enactGDBChangesParcelByGeography(geography.GeographyID).subscribe(
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

    public launchModal(geography: GeographyMinimalDto): void {
        const confirmOptions = {
            title: "Finalize Water Account and Parcel Changes",
            message: `Are you sure you want to finalize these changes? This action cannot be undone.`,
            buttonClassYes: "btn btn-secondary",
            buttonTextYes: "Save",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.onSubmitChanges(geography);
            }
        });
    }
}
