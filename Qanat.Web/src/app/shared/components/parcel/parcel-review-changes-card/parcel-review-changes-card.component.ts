import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelChangesGridItemDto } from "src/app/shared/generated/model/parcel-changes-grid-item-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ParcelUpdateWaterAccountModalComponent } from "../modals/parcel-update-water-account-modal/parcel-update-water-account-modal.component";
import { ParcelEditZoneAssignmentsModalComponent } from "../modals/parcel-edit-zone-assignments-modal/parcel-edit-zone-assignments-modal.component";
import { ParcelModifyParcelStatusModalComponent } from "../modals/parcel-modify-parcel-status-modal/parcel-modify-parcel-status-modal.component";
import { ParcelUpdateOwnershipInfoModalComponent } from "../modals/parcel-update-ownership-info-modal/parcel-update-ownership-info-modal.component";

@Component({
    selector: "parcel-review-changes-card",
    standalone: true,
    imports: [IconComponent, CommonModule],
    templateUrl: "./parcel-review-changes-card.component.html",
    styleUrl: "./parcel-review-changes-card.component.scss",
})
export class ParcelReviewChangesCardComponent {
    @Input() parcel: ParcelChangesGridItemDto;
    @Input() nextButtonDisabled: boolean = false;

    @Output() parcelReviewed = new EventEmitter();
    @Output() parcelUpdated = new EventEmitter();
    @Output() nextParcel = new EventEmitter();

    public isLoadingSubmit: boolean = false;

    constructor(private parcelService: ParcelService, private alertService: AlertService, private modalService: ModalService) {}

    public selectNextParcel() {
        this.nextParcel.emit();
    }

    public markParcelAsReviewed() {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();
        this.parcelService.parcelsReviewPut([this.parcel.ParcelID]).subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.parcel.IsReviewed = true;

                this.alertService.pushAlert(new Alert("Parcel successfully marked as reviewed.", AlertContext.Success));
                this.parcelReviewed.emit();
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    public updateWaterAccount(): void {
        this.modalService
            .open(
                ParcelUpdateWaterAccountModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.parcelUpdated.emit();
                }
            });
    }

    public editZoneAssignments(): void {
        this.modalService.open(
            ParcelEditZoneAssignmentsModalComponent,
            null,
            { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
            { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
        ).instance.result;
    }

    public modifyParcelStatus(): void {
        this.modalService
            .open(
                ParcelModifyParcelStatusModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.parcelUpdated.emit();
                }
            });
    }
    public updateParcelOwnershipInfo(): void {
        this.modalService
            .open(
                ParcelUpdateOwnershipInfoModalComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { ParcelID: this.parcel.ParcelID }
            )
            .instance.result.then((succeeded) => {
                if (succeeded) {
                    this.parcelUpdated.emit();
                }
            });
    }
}
