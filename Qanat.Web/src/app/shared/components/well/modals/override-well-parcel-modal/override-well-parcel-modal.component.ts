import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Observable, Subscription } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ParcelDisplayDto, ParcelMinimalDto, WellLocationDto, WellMinimalDto, WellParcelDtoForm, WellParcelDtoFormControls } from "src/app/shared/generated/model/models";
import { AsyncPipe } from "@angular/common";
import { WellService } from "src/app/shared/generated/api/well.service";
import * as L from "leaflet";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { DialogRef } from "@ngneat/dialog";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelTypeaheadComponent } from "src/app/shared/components/parcel/parcel-typeahead/parcel-typeahead.component";
import { ParcelIconWithNumberComponent } from "src/app/shared/components/parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { WellContext } from "src/app/shared/components/well/modals/delete-well-modal/delete-well-modal.component";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

@Component({
    selector: "override-well-parcel-modal",
    templateUrl: "./override-well-parcel-modal.component.html",
    styleUrls: ["./override-well-parcel-modal.component.scss"],
    imports: [
        CustomRichTextComponent,
        FormsModule,
        ReactiveFormsModule,
        ParcelTypeaheadComponent,
        ParcelIconWithNumberComponent,
        AsyncPipe,
        QanatMapComponent,
        ParcelLayerComponent,
        ParcelIconWithNumberComponent,
        WellsLayerComponent,
    ],
})
export class OverrideWellParcelComponent implements OnInit, OnDestroy {
    public ref: DialogRef<WellContext, WellMinimalDto> = inject(DialogRef);
    public well$: Observable<WellMinimalDto>;
    public wellLocation$: Observable<WellLocationDto>;

    public selectedParcel: ParcelDisplayDto;
    public selectedParcelSubscription = Subscription.EMPTY;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWellParcel;
    public isLoadingSubmit = false;

    public FormFieldType = FormFieldType;
    public formGroup = new FormGroup<WellParcelDtoForm>({
        WellID: WellParcelDtoFormControls.WellID(),
        ParcelID: WellParcelDtoFormControls.ParcelID(),
    });

    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private wellService: WellService,
        private parcelService: ParcelService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.well$ = this.wellService.getWellAsMinimalDtoWell(this.ref.data.WellID).pipe(
            tap((well) => {
                this.formGroup.controls.WellID.setValue(well.WellID);
                this.formGroup.controls.ParcelID.setValue(well.ParcelID);

                if (!well.ParcelID) return;
                this.selectedParcel = new ParcelMinimalDto({ ParcelID: well.ParcelID, ParcelNumber: well.ParcelNumber });
            })
        );

        this.wellLocation$ = this.well$.pipe(switchMap((well) => this.wellService.getLocationByWellIDWell(well.WellID)));
    }

    ngOnDestroy(): void {
        this.selectedParcelSubscription.unsubscribe();
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;

        this.formGroup.controls.ParcelID.patchValue(this.selectedParcel.ParcelID);
        this.wellService.overrideWellParcelWell(this.ref.data.WellID, this.formGroup.getRawValue()).subscribe((response) => {
            if (response) {
                this.alertService.pushAlert(new Alert(`Parcel successfully updated`, AlertContext.Success));
                this.ref.close(response);
            }
            this.isLoadingSubmit = false;
        });
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    selectedParcelChanged(parcel: ParcelDisplayDto) {
        this.selectedParcel = parcel;
    }

    onParcelSelectedFromMap(selectedParcelID: number) {
        this.selectedParcelSubscription = this.parcelService.getByIDParcel(selectedParcelID).subscribe((parcel) => {
            this.selectedParcel = parcel;
        });
    }
}
