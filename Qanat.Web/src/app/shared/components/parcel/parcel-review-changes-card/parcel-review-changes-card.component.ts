import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelChangesGridItemDto } from "src/app/shared/generated/model/parcel-changes-grid-item-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ParcelUpdateWaterAccountModalComponent } from "../modals/parcel-update-water-account-modal/parcel-update-water-account-modal.component";
import { ParcelEditZoneAssignmentsModalComponent } from "../modals/parcel-edit-zone-assignments-modal/parcel-edit-zone-assignments-modal.component";
import { ParcelModifyParcelStatusModalComponent } from "../modals/parcel-modify-parcel-status-modal/parcel-modify-parcel-status-modal.component";
import { ParcelUpdateOwnershipInfoModalComponent } from "../modals/parcel-update-ownership-info-modal/parcel-update-ownership-info-modal.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "parcel-review-changes-card",
    imports: [IconComponent, CommonModule],
    templateUrl: "./parcel-review-changes-card.component.html",
    styleUrl: "./parcel-review-changes-card.component.scss"
})
export class ParcelReviewChangesCardComponent {
    @Input() parcel: ParcelChangesGridItemDto;
    @Input() nextButtonDisabled: boolean = false;

    @Output() parcelReviewed = new EventEmitter();
    @Output() parcelUpdated = new EventEmitter();
    @Output() nextParcel = new EventEmitter();

    public isLoadingSubmit: boolean = false;

    constructor(
        private parcelService: ParcelService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    public selectNextParcel() {
        this.nextParcel.emit();
    }

    public markParcelAsReviewed() {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();
        this.parcelService.markParcelAsReviewedParcel(this.parcel.GeographyID, [this.parcel.ParcelID]).subscribe({
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
        const dialogRef = this.dialogService.open(ParcelUpdateWaterAccountModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcelUpdated.emit();
            }
        });
    }

    public editZoneAssignments(): void {
        const dialogRef = this.dialogService.open(ParcelEditZoneAssignmentsModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }

    public modifyParcelStatus(): void {
        const dialogRef = this.dialogService.open(ParcelModifyParcelStatusModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcelUpdated.emit();
            }
        });
    }
    public updateParcelOwnershipInfo(): void {
        const dialogRef = this.dialogService.open(ParcelUpdateOwnershipInfoModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcelUpdated.emit();
            }
        });
    }
}
