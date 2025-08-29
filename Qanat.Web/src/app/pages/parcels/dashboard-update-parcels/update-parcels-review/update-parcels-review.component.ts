import { ChangeDetectorRef, Component, OnDestroy, OnInit, QueryList, ViewChildren } from "@angular/core";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule, FormControl, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { combineLatest, Observable, of, shareReplay, switchMap, tap } from "rxjs";
import { ParcelLayerUpdateDto } from "src/app/shared/generated/model/parcel-layer-update-dto";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { FeatureClassInfo } from "src/app/shared/generated/model/feature-class-info";
import { ParcelUpdateExpectedResultsDto } from "src/app/shared/generated/model/parcel-update-expected-results-dto";
import { AsyncPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto, ReportingPeriodDto, UploadedGdbSimpleDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "update-parcels-review",
    templateUrl: "./update-parcels-review.component.html",
    styleUrls: ["./update-parcels-review.component.scss"],
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        FormsModule,
        ReactiveFormsModule,
        CustomRichTextComponent,
        ButtonComponent,
        ButtonLoadingDirective,
        FormFieldComponent,
        NgSelectModule,
        DatePipe,
        NoteComponent,
    ]
})
export class UpdateParcelsReviewComponent implements OnInit, OnDestroy {
    @ViewChildren("fileInput") public fileInput: QueryList<any>;

    public FormFieldType = FormFieldType;

    public geography$: Observable<GeographyMinimalDto>;
    public featureClass$: Observable<FeatureClassInfo[]>;

    public isLoadingSubmit: boolean;
    public customRichTextType: number = 41;
    public rowData = [];
    public columnDefs: ColDef[];
    public userID: number;

    public newParcelLayerForm = new UntypedFormGroup({
        gdbUploadForParcelLayer: new UntypedFormControl("", [Validators.required]),
    });

    public submitForPreviewForm: FormGroup<{ waterYearSelection: FormControl<any> }> = new FormGroup({
        waterYearSelection: new FormControl<any>("", [Validators.required]),
    });

    public featureClass: Array<FeatureClassInfo>;
    public resultsPreview: ParcelUpdateExpectedResultsDto;
    public parcelLayerUpdateDto: ParcelLayerUpdateDto;

    public latestFinalizedGDBUpload$: Observable<UploadedGdbSimpleDto>;
    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;

    constructor(
        private router: Router,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef,
        private geographyService: GeographyService,
        private parcelByGeographyService: ParcelByGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService
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

        this.featureClass$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelByGeographyService.getFeatureClassInfoParcelByGeography(geography.GeographyID).pipe(
                    switchMap((uploadParcelLayerInfoDto) => {
                        return of(uploadParcelLayerInfoDto.FeatureClasses);
                    })
                );
            }),
            tap((featureClass) => {
                this.featureClass = featureClass;
                this.parcelLayerUpdateDto = new ParcelLayerUpdateDto({
                    ParcelLayerNameInGDB: this.featureClass[0].LayerName,
                    //UploadedGDBID: 1
                });
            })
        );

        this.latestFinalizedGDBUpload$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelByGeographyService.getLatestEffectiveYearFinalizedGDBUploadForGeographyParcelByGeography(geography.GeographyID);
            }),
            shareReplay(1)
        );

        this.reportingPeriodSelectOptions$ = combineLatest({ geography: this.geography$, latestFinalizedGDBUpload: this.latestFinalizedGDBUpload$ }).pipe(
            switchMap(({ geography, latestFinalizedGDBUpload }) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID).pipe(
                    switchMap((reportingPeriods) => {
                        if (latestFinalizedGDBUpload != null) {
                            reportingPeriods = reportingPeriods.filter((reportingPeriod) => {
                                return new Date(reportingPeriod.EndDate).getUTCFullYear() >= latestFinalizedGDBUpload.EffectiveYear;
                            });
                        }
                        return of(
                            reportingPeriods.map((reportingPeriod) => {
                                return {
                                    Value: new Date(reportingPeriod.EndDate).getUTCFullYear(),
                                    Label: reportingPeriod.Name,
                                } as SelectDropdownOption;
                            })
                        );
                    })
                );
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

    public getColumns(): Array<string> {
        if (!this.featureClass) {
            return [];
        }

        return this.featureClass[0].Columns;
    }

    public onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        let endDate = new Date(selectedReportingPeriod.EndDate);
        let year = endDate.getUTCFullYear();
        this.submitForPreviewForm.get("waterYearSelection").setValue(year);
    }

    public onSubmitForPreview(geography: GeographyMinimalDto) {
        this.isLoadingSubmit = true;

        this.parcelLayerUpdateDto.EffectiveYear = this.submitForPreviewForm.get("waterYearSelection").value;

        this.parcelByGeographyService
            .previewParcelLayerGDBChangesViaGeoJsonFeatureCollectionAndUploadToStagingParcelByGeography(geography.GeographyID, this.parcelLayerUpdateDto)
            .subscribe(
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
        let reportingPeriod = this.submitForPreviewForm.get("waterYearSelection").value;
        return (
            this.parcelLayerUpdateDto.ParcelNumberColumn != null &&
            this.parcelLayerUpdateDto.ParcelNumberColumn != undefined &&
            this.parcelLayerUpdateDto.OwnerNameColumn != null &&
            this.parcelLayerUpdateDto.OwnerNameColumn != undefined &&
            this.parcelLayerUpdateDto.OwnerAddressColumn != null &&
            this.parcelLayerUpdateDto.OwnerAddressColumn != undefined &&
            this.parcelLayerUpdateDto.AcreColumn != null &&
            this.parcelLayerUpdateDto.AcreColumn != undefined &&
            reportingPeriod != null &&
            reportingPeriod != undefined &&
            reportingPeriod != ""
        );
    }
}
