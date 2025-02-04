import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { Observable, combineLatest, forkJoin, of } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { UpdateWaterAccountParcelsDtoForm, UpdateWaterAccountParcelsDtoFormControls } from "src/app/shared/generated/model/update-water-account-parcels-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { ParcelDisplayDto, ParcelWithGeoJSONDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "../../../inputs/select-dropdown/select-dropdown.component";
import { CustomGeoJSONLayer, ParcelMapComponent } from "../../../parcel/parcel-map/parcel-map.component";
import { MapPipe } from "../../../../pipes/map.pipe";
import { NoteComponent } from "../../../note/note.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { ParcelTypeaheadComponent } from "../../../parcel/parcel-typeahead/parcel-typeahead.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";

@Component({
    selector: "update-parcels",
    templateUrl: "./update-parcels.component.html",
    styleUrls: ["./update-parcels.component.scss"],
    standalone: true,
    imports: [
        CustomRichTextComponent,
        NgIf,
        IconComponent,
        FormsModule,
        ReactiveFormsModule,
        ParcelTypeaheadComponent,
        NgFor,
        ParcelIconWithNumberComponent,
        NoteComponent,
        FormFieldComponent,
        ParcelMapComponent,
        AsyncPipe,
        DatePipe,
        MapPipe,
    ],
})
export class UpdateParcelsComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;
    public FormFieldType = FormFieldType;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWaterAccountParcels;
    public waterAccount$: Observable<WaterAccountDto>;
    private waterAccount: WaterAccountDto;

    public formGroup = new FormGroup<UpdateWaterAccountParcelsDtoForm>({
        EffectiveYear: UpdateWaterAccountParcelsDtoFormControls.EffectiveYear(),
        ParcelIDs: UpdateWaterAccountParcelsDtoFormControls.ParcelIDs(),
    });

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public defaultReportingPeriod$: Observable<ReportingPeriodDto>;
    public reportingPeriodYears: number[];

    public effectiveYearDropdownOptions: SelectDropdownOption[];
    public effectiveYear$: Observable<Date>;
    public parcelsWithGeoJSON$: Observable<ParcelWithGeoJSONDto[]>;

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private modalService: ModalService,
        private waterAccountService: WaterAccountService,
        private reportingPeriodService: ReportingPeriodService,
        private parcelByGeographyService: ParcelByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).pipe(
            tap((waterAccount) => {
                this.waterAccount = waterAccount;

                this.parcelsWithGeoJSON$ = this.parcelByGeographyService
                    .geographiesGeographyIDParcelsGeojsonPost(
                        waterAccount.Geography.GeographyID,
                        waterAccount.Parcels.map((x) => x.ParcelID)
                    )
                    .pipe(
                        tap((parcels) => {
                            this.originalWaterAccountParcels = [...parcels];
                            this.updateParcelsTo([...parcels]);
                        })
                    );
            })
        );

        this.reportingPeriods$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.reportingPeriodService.geographiesGeographyIDReportingPeriodsGet(waterAccount.Geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                this.reportingPeriodYears = reportingPeriods.map((x) => new Date(x.StartDate).getFullYear()).reverse();
            })
        );

        this.defaultReportingPeriod$ = this.reportingPeriods$.pipe(
            switchMap((reportingPeriods) => {
                return of(reportingPeriods.find((x) => x.IsDefaultReportingPeriod));
            })
        );

        this.effectiveYear$ = combineLatest([this.defaultReportingPeriod$, this.formGroup.controls.EffectiveYear.valueChanges]).pipe(
            switchMap(([reportingPeriod, effectiveYear]) => {
                if (!reportingPeriod || !effectiveYear) {
                    return of(null);
                }

                const date = new Date(reportingPeriod.StartDate);
                return of(date);
            })
        );
    }

    updateEffectiveYearDropdownOptions(years: number[]) {
        let options = years.map((x) => ({ Value: x, Label: x.toString() }) as SelectDropdownOption);
        // insert an empty option at the front
        options = [{ Value: null, Label: "Select an Option", Disabled: true }, ...options];

        this.effectiveYearDropdownOptions = options;

        if (this.formGroup.controls.EffectiveYear.value && !years.includes(this.formGroup.controls.EffectiveYear.value)) {
            this.formGroup.controls.EffectiveYear.setValue(null);
        }
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.waterAccountService.waterAccountsWaterAccountIDUpdateParcelsPatch(this.waterAccount.WaterAccountID, this.formGroup.getRawValue()).subscribe((response) => {
            if (response) {
                this.alertService.pushAlert(new Alert(`Successfully saved Water Account Parcels for ${this.waterAccount.WaterAccountNameAndNumber}.`, AlertContext.Success));
                this.modalService.close(this.modalComponentRef, response);
            }
            this.isLoadingSubmit = false;
        });
    }

    addSelectedParcel() {
        if (!this.selectedParcel) return;

        this.parcelByGeographyService
            .geographiesGeographyIDParcelsGeojsonPost(this.waterAccount.Geography.GeographyID, [this.selectedParcel.ParcelID])
            .subscribe((parcelWithGeoJson) => {
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
                name: "<span style='width:15px; height:15px; display:inline-block; background:#ff000040; border:3px solid red;'></span> Removed Parcels",
                style: {
                    color: "red",
                    weight: 2,
                    opacity: 0.65,
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
                    opacity: 0.65,
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

        this.parcelByGeographyService.geographiesGeographyIDParcelsLatestEffectiveYearPost(this.waterAccount.Geography.GeographyID, parcelIDs).subscribe((latestEffectiveYear) => {
            const years = this.reportingPeriodYears.filter((x) => x >= latestEffectiveYear);
            this.updateEffectiveYearDropdownOptions(years);
        });
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
            this.parcelByGeographyService.geographiesGeographyIDParcelsGeojsonPost(this.waterAccount.Geography.GeographyID, [parcelIDClicked]).subscribe((parcelDisplayDto) => {
                this.updateParcelsTo([...this.waterAccountParcels, ...parcelDisplayDto]);
            });
        }
    }
}
