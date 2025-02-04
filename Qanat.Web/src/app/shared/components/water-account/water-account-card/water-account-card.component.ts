import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, Output, SimpleChange, SimpleChanges, ViewChild, ViewContainerRef } from "@angular/core";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { DeleteWaterAccountComponent } from "../modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "../modals/merge-water-accounts/merge-water-accounts.component";
import { UpdateParcelsComponent } from "../modals/update-parcels/update-parcels.component";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "../modals/update-water-account-info/update-water-account-info.component";
import { Observable, tap } from "rxjs";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { ParcelMapComponent } from "../../parcel/parcel-map/parcel-map.component";
import { ParcelIconWithNumberComponent } from "../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { RouterLink } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from "@angular/common";
import { QanatMapComponent, QanatMapInitEvent } from "../../leaflet/qanat-map/qanat-map.component";
import { Map, layerControl } from "leaflet";
import { GsaBoundariesComponent } from "../../leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { ParcelLayerComponent } from "../../leaflet/layers/parcel-layer/parcel-layer.component";
import { WellsLayerComponent } from "../../leaflet/layers/wells-layer/wells-layer.component";
import { ZoneGroupLayerComponent } from "../../leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyParcelsLayerComponent } from "../../leaflet/layers/geography-parcels-layer/geography-parcels-layer.component";
import { WellMinimalDto } from "src/app/shared/generated/model/well-minimal-dto";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/zone-group-minimal-dto";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { WellService } from "src/app/shared/generated/api/well.service";

@Component({
    selector: "water-account-card",
    templateUrl: "./water-account-card.component.html",
    styleUrls: ["./water-account-card.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        IconComponent,
        RouterLink,
        NgFor,
        ParcelIconWithNumberComponent,
        ParcelMapComponent,
        AsyncPipe,
        DecimalPipe,
        QanatMapComponent,
        GsaBoundariesComponent,
        ParcelLayerComponent,
        WellsLayerComponent,
        ZoneGroupLayerComponent,
        GeographyParcelsLayerComponent,
    ],
})
export class WaterAccountCardComponent implements OnChanges {
    @ViewChild("modalContainer") modalContainer;

    @Input() waterAccountID: number;
    @Input() displayActions: boolean = true;

    @Output() waterAccountIDChange = new EventEmitter<WaterAccountDto>();

    public waterAccount: WaterAccountDto;
    public waterAccount$: Observable<WaterAccountDto>;
    public selectedParcelIDs: number[];
    public totalAcres: number;
    public totalIrrigatedAcres: number;
    public wells$: Observable<WellMinimalDto[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;

    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private waterAccountService: WaterAccountService,
        private cdr: ChangeDetectorRef,
        private wellService: WellService,
        private zoneGroupService: ZoneGroupService
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.waterAccountID.currentValue && changes.waterAccountID.currentValue !== changes.waterAccountID.previousValue) {
            this.mapIsReady = false;

            this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountID).pipe(
                tap((waterAccount) => {
                    this.waterAccount = waterAccount;

                    this.wells$ = this.wellService.waterAccountsWaterAccountIDWellsGet(waterAccount.WaterAccountID);
                    this.zoneGroups$ = this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.waterAccount.Geography.GeographyID);

                    this.updateSelectedParcels();
                })
            );
        }
    }

    updateSelectedParcels(): void {
        this.selectedParcelIDs = this.waterAccount.Parcels.map((x) => x.ParcelID);
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
    }

    openUpdateInfoModal(): void {
        this.modalService
            .open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.waterAccountID = result;
                    this.waterAccountIDChange.emit(result);
                }
            });
    }

    openMergeModal(): void {
        this.modalService
            .open(MergeWaterAccountsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.waterAccountID = { ...result };
                    this.waterAccountIDChange.emit(result);
                    this.updateSelectedParcels();
                }
            });
    }

    openUpdateParcelsModal(): void {
        this.modalService
            .open(UpdateParcelsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.waterAccountID = { ...result };
                    this.updateSelectedParcels();
                    this.waterAccountIDChange.emit(result);
                }
            });
    }

    openDeleteModal(): void {
        this.modalService
            .open(DeleteWaterAccountComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.waterAccountIDChange.emit(null);
                }
            });
    }
}
