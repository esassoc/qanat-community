import { Component, ComponentRef, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Observable } from "rxjs";
import { map, tap } from "rxjs/operators";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import {
    CreateWaterAccountFromSuggestionDtoForm,
    CreateWaterAccountFromSuggestionDtoFormControls,
    ParcelDisplayDto,
    ParcelWithGeoJSONDto,
    ReportingPeriodDto,
    WaterAccountMinimalDto,
} from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomGeoJSONLayer, ParcelMapComponent } from "../../../parcel/parcel-map/parcel-map.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MapPipe } from "../../../../pipes/map.pipe";
import { RouterLink } from "@angular/router";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { AsyncPipe } from "@angular/common";
import { ParcelTypeaheadComponent } from "../../../parcel/parcel-typeahead/parcel-typeahead.component";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "review-water-account-suggestion",
    templateUrl: "./review-water-account-suggestion.component.html",
    styleUrls: ["./review-water-account-suggestion.component.scss"],
    imports: [
        IconComponent,
        CustomRichTextComponent,
        FormsModule,
        ReactiveFormsModule,
        ParcelTypeaheadComponent,
        ParcelIconWithNumberComponent,
        FormFieldComponent,
        RouterLink,
        ParcelMapComponent,
        AsyncPipe,
        MapPipe,
    ],
})
export class ReviewWaterAccountSuggestionComponent implements OnInit {
    public ref: DialogRef<WaterAccountSuggestionContext, WaterAccountMinimalDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalReviewWaterAccountSuggestion;

    public formGroup = new FormGroup<CreateWaterAccountFromSuggestionDtoForm>({
        ParcelIDList: CreateWaterAccountFromSuggestionDtoFormControls.ParcelIDList(),
        ReportingPeriodID: CreateWaterAccountFromSuggestionDtoFormControls.ReportingPeriodID(),
        WaterAccountName: CreateWaterAccountFromSuggestionDtoFormControls.WaterAccountName(),
        ContactName: CreateWaterAccountFromSuggestionDtoFormControls.WaterAccountName(),
        ContactAddress: CreateWaterAccountFromSuggestionDtoFormControls.ContactAddress(),
    });

    public reportingPeriodDropdownOptions$: Observable<SelectDropdownOption[]>;
    public reportingPeriod: ReportingPeriodDto;
    public effectiveYear$: Observable<number>;
    public parcelsWithGeoJSON$: Observable<ParcelWithGeoJSONDto[]>;

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private parcelByGeographyService: ParcelByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.WaterAccountName.setValue(this.ref.data.WaterAccountName);
        this.formGroup.controls.ContactName.setValue(this.ref.data.ContactName);
        this.formGroup.controls.ContactAddress.setValue(this.ref.data.ContactAddress);
        this.parcelsWithGeoJSON$ = this.parcelByGeographyService.getParcelGeoJsonsParcelByGeography(this.ref.data.GeographyID, this.ref.data.ParcelIDList).pipe(
            tap((parcels) => {
                this.originalWaterAccountParcels = [...parcels];
                this.updateParcelsTo([...parcels]);
            })
        );

        this.reportingPeriodDropdownOptions$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.ref.data.GeographyID).pipe(
            map((x) => {
                let options = x.map((y) => (({
                    Value: y.ReportingPeriodID,
                    Label: y.Name
                }) as SelectDropdownOption));
                return options;
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;
        this.waterAccountByGeographyService
            .createWaterAccountFromSuggestionWaterAccountByGeography(this.ref.data.GeographyID, this.formGroup.getRawValue())
            .subscribe((response) => {
                if (response) {
                    this.alertService.pushAlert(new Alert(`Successfully approved suggested Water Account ${response.WaterAccountNameAndNumber}.`, AlertContext.Success));
                    this.ref.close(response);
                }
                this.isLoadingSubmit = false;
            });
    }

    reject() {
        this.isLoadingSubmit = true;
        this.waterAccountByGeographyService.rejectWaterAccountSuggestionsWaterAccountByGeography(this.ref.data.GeographyID, this.ref.data.ParcelIDList).subscribe((response) => {
            this.alertService.pushAlert(new Alert(`Successfully rejected suggested Water Account ${this.ref.data.WaterAccountName}.`, AlertContext.Success));
            this.ref.close(response);
            this.isLoadingSubmit = false;
        });
    }

    addSelectedParcel() {
        if (!this.selectedParcel) return;

        this.parcelByGeographyService.getParcelGeoJsonsParcelByGeography(this.ref.data.GeographyID, [this.selectedParcel.ParcelID]).subscribe((parcelWithGeoJson) => {
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
        this.formGroup.controls.ParcelIDList.setValue(parcels.map((x) => x.ParcelID));

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
            this.parcelByGeographyService.getParcelGeoJsonsParcelByGeography(this.ref.data.GeographyID, [parcelIDClicked]).subscribe((parcelDisplayDto) => {
                this.updateParcelsTo([...this.waterAccountParcels, ...parcelDisplayDto]);
            });
        }
    }

    getReviewSuggestionTitle(): string {
        const wellIDList = this.ref.data.WellIDList ?? [];
        const wellPart = wellIDList.length > 0 ? ` & ${wellIDList.length} Well${wellIDList.length > 1 ? "s" : ""}` : "";
        const parcelPart = `${this.ref.data.ParcelIDList.length} APN${this.ref.data.ParcelIDList.length > 1 ? "s" : ""}`;
        return `${this.ref.data.WaterAccountName} [${parcelPart}${wellPart}]`;
    }
}

export interface WaterAccountSuggestionContext {
    WaterAccountName: string;
    WaterAccountNumber: number;
    GeographyID: number;
    GeographyName: string;
    ParcelIDList: number[];
    WellIDList: number[];
    ContactName: string;
    ContactAddress: string;
}
