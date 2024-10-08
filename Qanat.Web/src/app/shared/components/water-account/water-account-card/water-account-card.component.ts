import { Component, EventEmitter, Input, OnChanges, Output, ViewChild, ViewContainerRef } from "@angular/core";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { DeleteWaterAccountComponent } from "../modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "../modals/merge-water-accounts/merge-water-accounts.component";
import { UpdateParcelsComponent } from "../modals/update-parcels/update-parcels.component";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "../modals/update-water-account-info/update-water-account-info.component";
import { Observable, tap } from "rxjs";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { ParcelMapComponent } from "../../parcel-map/parcel-map.component";
import { ParcelIconWithNumberComponent } from "../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { RouterLink } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";

@Component({
    selector: "water-account-card",
    templateUrl: "./water-account-card.component.html",
    styleUrls: ["./water-account-card.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent, RouterLink, NgFor, ParcelIconWithNumberComponent, ParcelMapComponent, AsyncPipe],
})
export class WaterAccountCardComponent implements OnChanges {
    @Input() waterAccountID: number;
    public waterAccount: WaterAccountDto;
    public waterAccount$: Observable<WaterAccountDto>;
    @Input() displayActions: boolean = true;
    @Output() changedAccount = new EventEmitter<WaterAccountDto>();
    @ViewChild("modalContainer") modalContainer;
    public mapID = crypto.randomUUID();
    public selectedParcelIDs: number[];

    constructor(
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnChanges(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountID).pipe(
            tap((waterAccount) => {
                this.waterAccount = waterAccount;
                this.updateSelectedParcels();
            })
        );
    }

    updateSelectedParcels(): void {
        this.selectedParcelIDs = this.waterAccount.Parcels.map((x) => x.ParcelID);
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
                    this.changedAccount.emit(result);
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
                    this.changedAccount.emit(result);
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
                    this.changedAccount.emit(result);
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
                    this.changedAccount.emit(null);
                }
            });
    }
}
