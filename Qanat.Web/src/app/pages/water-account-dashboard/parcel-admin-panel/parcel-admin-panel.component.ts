import { Component, OnInit } from "@angular/core";
import { ParcelEditZoneAssignmentsModalComponent } from "src/app/shared/components/parcel-edit-zone-assignments-modal/parcel-edit-zone-assignments-modal.component";
import { ParcelModifyParcelStatusModalComponent } from "src/app/shared/components/parcel-modify-parcel-status-modal/parcel-modify-parcel-status-modal.component";
import { ParcelUpdateOwnershipInfoModalComponent } from "src/app/shared/components/parcel-update-ownership-info-modal/parcel-update-ownership-info-modal.component";
import { ParcelUpdateWaterAccountModalComponent } from "src/app/shared/components/parcel-update-water-account-modal/parcel-update-water-account-modal.component";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { Observable, Subscription, tap } from "rxjs";
import { AsyncPipe, DatePipe, DecimalPipe, KeyValuePipe, NgFor, NgIf } from "@angular/common";
import { ParcelDetailDto } from "src/app/shared/generated/model/parcel-detail-dto";
import { ParcelHistoryDto } from "src/app/shared/generated/model/parcel-history-dto";
import { WaterAccountParcelDto } from "src/app/shared/generated/model/water-account-parcel-dto";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterAccountTitleComponent } from "src/app/shared/components/water-account/water-account-title/water-account-title.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { KeyValuePairListComponent } from "../../../shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "../../../shared/components/key-value-pair/key-value-pair.component";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { EntityCustomAttributesDto } from "src/app/shared/generated/model/entity-custom-attributes-dto";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { QanatMapComponent, QanatMapInitEvent } from "../../../shared/components/leaflet/qanat-map/qanat-map.component";
import { HighlightedParcelsLayerComponent } from "../../../shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { WellRegistrationsLayerComponent } from "../../../shared/components/leaflet/layers/well-registrations-layer/well-registrations-layer.component";
import * as L from "leaflet";
import { WellsLayerComponent } from "../../../shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { WellLocationDto } from "src/app/shared/generated/model/well-location-dto";

@Component({
    selector: "parcel-admin-panel",
    standalone: true,
    imports: [
        IconComponent,
        PageHeaderComponent,
        WaterAccountTitleComponent,
        NgIf,
        AsyncPipe,
        RouterLink,
        DatePipe,
        NgFor,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        KeyValuePipe,
        LoadingDirective,
        ModelNameTagComponent,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellsLayerComponent,
        DecimalPipe,
    ],
    templateUrl: "./parcel-admin-panel.component.html",
    styleUrl: "./parcel-admin-panel.component.scss",
})
export class ParcelAdminPanelComponent implements OnInit {
    public parcel: ParcelDetailDto;
    public parcel$: Observable<ParcelDetailDto>;
    public parcelHistories$: Observable<ParcelHistoryDto[]>;
    public waterAccountParcels$: Observable<WaterAccountParcelDto[]>;
    public parcelCustomAttributes$: Observable<EntityCustomAttributesDto>;
    public wellLocations$: Observable<WellLocationDto[]>;

    public parcelID: number;
    private accountIDSubscription: Subscription = Subscription.EMPTY;

    public isLoading: boolean = false;

    constructor(
        private modalService: ModalService,
        private parcelService: ParcelService,
        private route: ActivatedRoute,
        private customAttributeService: CustomAttributeService
    ) {}

    ngOnInit(): void {
        this.accountIDSubscription = this.route.paramMap.subscribe((paramMap) => {
            this.isLoading = true;
            this.parcelID = parseInt(paramMap.get(routeParams.parcelID));
            this.wellLocations$ = this.parcelService.parcelsParcelIDWellsGet(this.parcelID);
            this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(this.parcelID).pipe(
                tap((parcel) => {
                    this.parcel = parcel;
                })
            );
            this.parcelHistories$ = this.parcelService.parcelsParcelIDHistoryGet(this.parcelID);
            this.waterAccountParcels$ = this.parcelService.parcelsParcelIDWaterAccountParcelsGet(this.parcelID);
            this.parcelCustomAttributes$ = this.customAttributeService.customAttributesParcelsParcelIDGet(this.parcelID);
            this.isLoading = false;
        });
    }

    updateOwnershipInfo(): void {
        this.modalService
            .open(
                ParcelUpdateOwnershipInfoModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(this.parcelID);
                }
            });
    }

    updateWaterAccount(): void {
        this.modalService
            .open(
                ParcelUpdateWaterAccountModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.waterAccountParcels$ = this.parcelService.parcelsParcelIDWaterAccountParcelsGet(this.parcelID);
                    this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(this.parcelID);
                }
            });
    }

    editZoneAssignments(): void {
        this.modalService
            .open(
                ParcelEditZoneAssignmentsModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                }
            });
    }

    modifyParcelStatus(): void {
        this.modalService
            .open(
                ParcelModifyParcelStatusModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(this.parcelID);
                }
            });
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
