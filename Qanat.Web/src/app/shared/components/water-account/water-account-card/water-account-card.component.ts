import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from "@angular/core";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { DeleteWaterAccountComponent } from "../modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "../modals/merge-water-accounts/merge-water-accounts.component";
import { UpdateParcelsComponent } from "../modals/update-parcels/update-parcels.component";
import { UpdateWaterAccountInfoComponent } from "../modals/update-water-account-info/update-water-account-info.component";
import { Observable, tap } from "rxjs";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { ParcelIconWithNumberComponent } from "../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { RouterLink } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AsyncPipe, DecimalPipe } from "@angular/common";
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
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "water-account-card",
    templateUrl: "./water-account-card.component.html",
    styleUrls: ["./water-account-card.component.scss"],
    imports: [
        IconComponent,
        RouterLink,
        ParcelIconWithNumberComponent,
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
    @Input() reportingPeriodID: number;

    @Output() waterAccountIDChange = new EventEmitter<WaterAccountDto>();
    @Output() reportingPeriodIDChange = new EventEmitter<number>();

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
        private waterAccountService: WaterAccountService,
        private cdr: ChangeDetectorRef,
        private wellService: WellService,
        private zoneGroupService: ZoneGroupService,
        private dialogService: DialogService
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
        var waterAccountIDChanged = changes.waterAccountID && changes.waterAccountID.currentValue !== changes.waterAccountID.previousValue;
        var reportingPeriodIDChanged = changes.reportingPeriodID && changes.reportingPeriodID.currentValue !== changes.reportingPeriodID.previousValue;
        if (waterAccountIDChanged || reportingPeriodIDChanged) {
            this.mapIsReady = false;

            this.waterAccount$ = this.reportingPeriodID
                ? this.waterAccountService.getByIDAndReportingPeriodIDWaterAccount(this.waterAccountID, this.reportingPeriodID)
                : this.waterAccountService.getByIDWaterAccount(this.waterAccountID);

            this.waterAccount$ = this.waterAccount$.pipe(
                tap((waterAccount) => {
                    this.waterAccount = waterAccount;

                    this.wells$ = this.wellService.listWellsByWaterAccountIDWell(waterAccount.WaterAccountID);
                    this.zoneGroups$ = this.zoneGroupService.listZoneGroup(this.waterAccount.Geography.GeographyID);

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
        const dialogRef = this.dialogService.open(UpdateWaterAccountInfoComponent, {
            data: {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.waterAccountID = result.WaterAccountID;
                this.waterAccountIDChange.emit(result);
            }
        });
    }

    openMergeModal(): void {
        const dialogRef = this.dialogService.open(MergeWaterAccountsComponent, {
            data: {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.waterAccountID = result.WaterAccountID;
                this.waterAccountIDChange.emit(result);
                this.updateSelectedParcels();
            }
        });
    }

    openUpdateParcelsModal(): void {
        const dialogRef = this.dialogService.open(UpdateParcelsComponent, {
            data: {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result.success) {
                this.waterAccountID = result.result[0].WaterAccountID;
                this.updateSelectedParcels();
                this.waterAccountIDChange.emit(result.result[0]);
            }
        });
    }

    openDeleteModal(): void {
        const dialogRef = this.dialogService.open(DeleteWaterAccountComponent, {
            data: {
                WaterAccountID: this.waterAccount.WaterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.waterAccountIDChange.emit(null);
            }
        });
    }
}
