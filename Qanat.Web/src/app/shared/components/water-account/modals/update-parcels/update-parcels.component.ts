import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { Observable, combineLatest, of } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { ParcelDisplayDto, ParcelMinimalDto, ParcelWithGeoJSONDto, ReportingPeriodDto, WaterAccountParcelsUpdateDtoForm } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomGeoJSONLayer, ParcelMapComponent } from "../../../parcel/parcel-map/parcel-map.component";
import { MapPipe } from "../../../../pipes/map.pipe";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { ParcelTypeaheadComponent } from "../../../parcel/parcel-typeahead/parcel-typeahead.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AsyncPipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "update-parcels",
    templateUrl: "./update-parcels.component.html",
    styleUrls: ["./update-parcels.component.scss"],
    imports: [
        CustomRichTextComponent,
        IconComponent,
        FormsModule,
        ReactiveFormsModule,
        ParcelTypeaheadComponent,
        ParcelIconWithNumberComponent,
        FormFieldComponent,
        ParcelMapComponent,
        AsyncPipe,
        MapPipe,
    ]
})
export class UpdateParcelsComponent implements OnInit {
    public ref: DialogRef<WaterAccountContext, UpdateParcelsReturnContext> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWaterAccountParcels;
    public waterAccount$: Observable<WaterAccountDto>;
    private waterAccount: WaterAccountDto;

    public formGroup = new FormGroup<WaterAccountParcelsUpdateDtoForm>({
        ReportingPeriodID: new FormControl<number>(null, [Validators.required]),
        ParcelIDs: new FormControl<number[]>(null, [Validators.minLength(0)]),
    });

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public reportingPeriods: ReportingPeriodDto[];
    public defaultReportingPeriod$: Observable<ReportingPeriodDto>;
    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;

    public existingWaterAccountParcels$: Observable<ParcelMinimalDto[]>;

    public parcelsWithGeoJSON$: Observable<ParcelWithGeoJSONDto[]>;

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private waterAccountService: WaterAccountService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private reportingPeriodService: ReportingPeriodService,
        private parcelByGeographyService: ParcelByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.getByIDWaterAccount(this.ref.data.WaterAccountID).pipe(
            tap((waterAccount) => {
                this.waterAccount = waterAccount;
            })
        );

        this.reportingPeriods$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(waterAccount.Geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                this.reportingPeriods = reportingPeriods;
            })
        );

        this.reportingPeriodSelectOptions$ = this.reportingPeriods$.pipe(
            switchMap((reportingPeriods) => {
                let selectOptions = reportingPeriods.map((x) => {
                    return { Value: x.ReportingPeriodID, Label: x.Name } as SelectDropdownOption;
                });

                return of(selectOptions);
            })
        );

        this.existingWaterAccountParcels$ = this.formGroup.get("ReportingPeriodID").valueChanges.pipe(
            switchMap((reportingPeriodID) => {
                let reportingPeriod = this.reportingPeriods.find((x) => x.ReportingPeriodID == reportingPeriodID);
                let reportingPeriodEndDate = new Date(reportingPeriod.EndDate);
                return this.waterAccountParcelByWaterAccountService.getCurrentParcelsFromAccountIDWaterAccountParcelByWaterAccount(
                    this.waterAccount.WaterAccountID,
                    reportingPeriodEndDate.getUTCFullYear()
                );
            })
        );

        this.parcelsWithGeoJSON$ = this.existingWaterAccountParcels$.pipe(
            switchMap((parcels) =>
                this.parcelByGeographyService
                    .getParcelGeoJsonsParcelByGeography(
                        this.waterAccount.Geography.GeographyID,
                        parcels.map((x) => x.ParcelID)
                    )
                    .pipe(
                        tap((parcels) => {
                            this.originalWaterAccountParcels = [...parcels];
                            this.updateParcelsTo([...parcels]);
                        })
                    )
            )
        );

        this.defaultReportingPeriod$ = combineLatest({ reportingPeriods: this.reportingPeriods$, options: this.reportingPeriodSelectOptions$ }).pipe(
            switchMap(({ reportingPeriods, options }) => {
                let defaultReportingPeriod = reportingPeriods.find((x) => x.IsDefault);
                if (!defaultReportingPeriod) {
                    defaultReportingPeriod = reportingPeriods[0];
                }

                this.formGroup.controls.ReportingPeriodID.setValue(defaultReportingPeriod.ReportingPeriodID);

                return of(defaultReportingPeriod);
            })
        );
    }

    close() {
        this.ref.close({ success: false } as UpdateParcelsReturnContext);
    }

    save() {
        this.isLoadingSubmit = true;

        this.waterAccountParcelByWaterAccountService
            .updateWaterAccountParcelsWaterAccountParcelByWaterAccount(this.waterAccount.WaterAccountID, this.formGroup.getRawValue())
            .subscribe((response) => {
                if (response) {
                    this.alertService.pushAlert(new Alert(`Successfully saved Water Account Parcels for ${this.waterAccount.WaterAccountNameAndNumber}.`, AlertContext.Success));
                    this.ref.close({ success: true, result: response } as UpdateParcelsReturnContext);
                }
                this.isLoadingSubmit = false;
            });
    }

    addSelectedParcel() {
        if (!this.selectedParcel) return;

        this.parcelByGeographyService.getParcelGeoJsonsParcelByGeography(this.waterAccount.Geography.GeographyID, [this.selectedParcel.ParcelID]).subscribe((parcelWithGeoJson) => {
            this.updateParcelsTo([...this.waterAccountParcels, ...parcelWithGeoJson]);
            this.selectedParcel = null;
        });
    }

    removeParcel(parcel: ParcelWithGeoJSONDto) {
        const parcels = this.waterAccountParcels.filter((x) => x != parcel);
        this.updateParcelsTo([...parcels]);
    }

    // this should hopefully be the only place we're actually setting a formcontrol
    updateParcelsTo(parcels: ParcelWithGeoJSONDto[]) {
        this.waterAccountParcels = parcels;
        this.formGroup.controls.ParcelIDs.setValue(parcels.map((x) => x.ParcelID));

        const removedParcels = this.originalWaterAccountParcels.filter((x) => !this.waterAccountParcels.map((y) => y.ParcelID).includes(x.ParcelID));

        this.customGeoJSONLayers = [
            {
                name: "<span style='width:15px; height:15px; display:inline-block; background:#ff000040; border:3px solid #ed6969;'></span> Removed Parcels",
                style: {
                    color: "#ed6969",
                    weight: 2,
                    fillOpacity: 0.25,
                    className: "hover-feature",
                    dashArray: "5",
                },
                geometries: removedParcels.map((x) => JSON.parse(x.GeoJSON)),
            },
            {
                name: "<span style='width:15px; height:15px; display:inline-block; background:#0000ff40; border:3px solid blue;'></span> Selected Parcels",
                style: {
                    color: "blue",
                    weight: 3,
                    fillOpacity: 0.25,
                    className: "hover-feature",
                },
                geometries: this.waterAccountParcels.map((x) => JSON.parse(x.GeoJSON)),
            },
        ];

        // determining which parcels should be used to fetch valid Effective Years
        let parcelIDs = parcels.map((x) => x.ParcelID);

        const originalParcelIDs = this.originalWaterAccountParcels.map((x) => x.ParcelID);
        parcelIDs = parcelIDs.filter((x) => !originalParcelIDs.includes(x));

        const removedParcelIDs = removedParcels.map((x) => x.ParcelID);
        parcelIDs = [...parcelIDs, ...removedParcelIDs];
    }

    selectedParcelChanged(parcel: ParcelDisplayDto) {
        this.selectedParcel = parcel;
    }

    clickedOnParcel(event): any {
        const parcelIDClicked = event.ParcelID;

        // remove if it exists
        if (this.waterAccountParcels.map((x) => x.ParcelID).includes(parcelIDClicked)) {
            const newParcels = this.waterAccountParcels.filter((x) => x.ParcelID != parcelIDClicked);
            this.updateParcelsTo([...newParcels]);
        } else {
            // else add it
            this.parcelByGeographyService.getParcelGeoJsonsParcelByGeography(this.waterAccount.Geography.GeographyID, [parcelIDClicked]).subscribe((parcelDisplayDto) => {
                this.updateParcelsTo([...this.waterAccountParcels, ...parcelDisplayDto]);
            });
        }
    }
}

export class UpdateParcelsReturnContext {
    success: boolean;
    result: ParcelMinimalDto[];
}
