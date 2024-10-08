import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { Observable } from "rxjs";
import { map, tap } from "rxjs/operators";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import {
    CreateWaterAccountFromSuggestionDtoForm,
    CreateWaterAccountFromSuggestionDtoFormControls,
    ParcelDisplayDto,
    ParcelWithGeoJSONDto,
    ReportingPeriodSimpleDto,
} from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "../../../inputs/select-dropdown/select-dropdown.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { CustomGeoJSONLayer, ParcelMapComponent } from "../../../parcel-map/parcel-map.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MapPipe } from "../../../../pipes/map.pipe";
import { RouterLink } from "@angular/router";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { NgFor, NgIf, AsyncPipe } from "@angular/common";
import { ParcelTypeaheadComponent } from "../../../parcel-typeahead/parcel-typeahead.component";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";

@Component({
    selector: "review-water-account-suggestion",
    templateUrl: "./review-water-account-suggestion.component.html",
    styleUrls: ["./review-water-account-suggestion.component.scss"],
    standalone: true,
    imports: [
        IconComponent,
        CustomRichTextComponent,
        FormsModule,
        ReactiveFormsModule,
        ParcelTypeaheadComponent,
        NgFor,
        ParcelIconWithNumberComponent,
        NgIf,
        FormFieldComponent,
        RouterLink,
        ParcelMapComponent,
        AsyncPipe,
        MapPipe,
    ],
})
export class ReviewWaterAccountSuggestionComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountSuggestionContext;
    public FormFieldType = FormFieldType;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalReviewWaterAccountSuggestion;

    public formGroup = new FormGroup<CreateWaterAccountFromSuggestionDtoForm>({
        ParcelIDList: CreateWaterAccountFromSuggestionDtoFormControls.ParcelIDList(),
        EffectiveYear: CreateWaterAccountFromSuggestionDtoFormControls.EffectiveYear(),
        WaterAccountName: CreateWaterAccountFromSuggestionDtoFormControls.WaterAccountName(),
        ContactName: CreateWaterAccountFromSuggestionDtoFormControls.WaterAccountName(),
        ContactAddress: CreateWaterAccountFromSuggestionDtoFormControls.ContactAddress(),
    });

    public reportingPeriods$: Observable<any>;
    public reportingPeriod: ReportingPeriodSimpleDto;
    public effectiveYear$: Observable<number>;
    public parcelsWithGeoJSON$: Observable<ParcelWithGeoJSONDto[]>;

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private modalService: ModalService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private parcelService: ParcelService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.WaterAccountName.setValue(this.modalContext.WaterAccountName);
        this.formGroup.controls.ContactName.setValue(this.modalContext.ContactName);
        this.formGroup.controls.ContactAddress.setValue(this.modalContext.ContactAddress);
        this.parcelsWithGeoJSON$ = this.parcelService.geographiesGeographyIDParcelsGeojsonPost(this.modalContext.GeographyID, this.modalContext.ParcelIDList).pipe(
            tap((parcels) => {
                this.originalWaterAccountParcels = [...parcels];
                this.updateParcelsTo([...parcels]);
            })
        );

        this.reportingPeriods$ = this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(this.modalContext.GeographyID).pipe(
            map((x) => {
                let options = x.map((y) => ({ Value: y, Label: y.toString() }) as SelectDropdownOption).reverse();
                // insert an empty option at the front
                options = [{ Value: null, Label: "Select an Option", Disabled: true }, ...options];
                return options;
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.waterAccountByGeographyService
            .geographiesGeographyIDWaterAccountsSuggestedCreatePost(this.modalContext.GeographyID, this.formGroup.getRawValue())
            .subscribe((response) => {
                if (response) {
                    this.alertService.pushAlert(new Alert(`Successfully approved suggested Water Account ${response.WaterAccountNameAndNumber}.`, AlertContext.Success));
                    this.modalService.close(this.modalComponentRef, response);
                }
                this.isLoadingSubmit = false;
            });
    }

    reject() {
        this.isLoadingSubmit = true;
        this.waterAccountByGeographyService
            .geographiesGeographyIDWaterAccountsSuggestedRejectPost(this.modalContext.GeographyID, this.modalContext.ParcelIDList)
            .subscribe((response) => {
                this.alertService.pushAlert(new Alert(`Successfully rejected suggested Water Account ${this.modalContext.WaterAccountName}.`, AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
                this.isLoadingSubmit = false;
            });
    }

    addSelectedParcel() {
        if (!this.selectedParcel) return;

        this.parcelService.geographiesGeographyIDParcelsGeojsonPost(this.modalContext.GeographyID, [this.selectedParcel.ParcelID]).subscribe((parcelWithGeoJson) => {
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
            this.parcelService.geographiesGeographyIDParcelsGeojsonPost(this.modalContext.GeographyID, [parcelIDClicked]).subscribe((parcelDisplayDto) => {
                this.updateParcelsTo([...this.waterAccountParcels, ...parcelDisplayDto]);
            });
        }
    }

    getReviewSuggestionTitle(): string {
        const wellIDList = this.modalContext.WellIDList ?? [];
        const wellPart = wellIDList.length > 0 ? ` & ${wellIDList.length} Well${wellIDList.length > 1 ? "s" : ""}` : "";
        const parcelPart = `${this.modalContext.ParcelIDList.length} APN${this.modalContext.ParcelIDList.length > 1 ? "s" : ""}`;
        return `${this.modalContext.WaterAccountName} [${parcelPart}${wellPart}]`;
    }
}

export interface WaterAccountSuggestionContext {
    WaterAccountName: string;
    GeographyID: number;
    GeographyName: string;
    ParcelIDList: number[];
    WellIDList: number[];
    ContactName: string;
    ContactAddress: string;
}
