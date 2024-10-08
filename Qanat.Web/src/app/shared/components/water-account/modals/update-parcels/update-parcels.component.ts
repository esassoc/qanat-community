import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { Observable, forkJoin, of } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { UpdateWaterAccountParcelsDtoForm, UpdateWaterAccountParcelsDtoFormControls } from "src/app/shared/generated/model/update-water-account-parcels-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { ParcelDisplayDto, ParcelWithGeoJSONDto, ReportingPeriodSimpleDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "../../../inputs/select-dropdown/select-dropdown.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { CustomGeoJSONLayer, ParcelMapComponent } from "../../../parcel-map/parcel-map.component";
import { MapPipe } from "../../../../pipes/map.pipe";
import { NoteComponent } from "../../../note/note.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { ParcelTypeaheadComponent } from "../../../parcel-typeahead/parcel-typeahead.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";

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

    public reportingPeriod: ReportingPeriodSimpleDto;
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
        private parcelService: ParcelService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).pipe(
            tap((waterAccount) => {
                this.waterAccount = waterAccount;

                this.parcelsWithGeoJSON$ = this.parcelService
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

        // Define type for the emitted values
        type ReportingPeriodForkJoinResults = [number[], ReportingPeriodSimpleDto];

        // Replace generic type with specific type
        forkJoin<ReportingPeriodForkJoinResults>([
            this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(this.modalContext.GeographyID),
            this.reportingPeriodService.geographiesGeographyIDReportingPeriodGet(this.modalContext.GeographyID),
        ]).subscribe(([years, reportingPeriod]) => {
            this.reportingPeriodYears = years.reverse();
            this.updateEffectiveYearDropdownOptions(years);
            this.reportingPeriod = reportingPeriod;
        });

        this.effectiveYear$ = this.formGroup.controls.EffectiveYear.valueChanges.pipe(
            switchMap((x) => {
                if (!this.reportingPeriod || !this.formGroup.controls.EffectiveYear.value) return of(null);

                const date = new Date();
                date.setDate(1);
                date.setMonth(1);
                date.setFullYear(this.reportingPeriod.StartMonth == 1 ? this.formGroup.controls.EffectiveYear.value : this.formGroup.controls.EffectiveYear.value - 1);
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

        this.parcelService.geographiesGeographyIDParcelsGeojsonPost(this.waterAccount.Geography.GeographyID, [this.selectedParcel.ParcelID]).subscribe((parcelWithGeoJson) => {
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

        this.parcelService.geographiesGeographyIDParcelsLatestEffectiveYearPost(this.waterAccount.Geography.GeographyID, parcelIDs).subscribe((latestEffectiveYear) => {
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
            this.parcelService.geographiesGeographyIDParcelsGeojsonPost(this.waterAccount.Geography.GeographyID, [parcelIDClicked]).subscribe((parcelDisplayDto) => {
                this.updateParcelsTo([...this.waterAccountParcels, ...parcelDisplayDto]);
            });
        }
    }
}
