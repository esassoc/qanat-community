import { Component, ComponentRef, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ParcelUpdateOwnershipRequestDtoForm, ParcelUpdateOwnershipRequestDtoFormControls } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "../../../icon/icon.component";

@Component({
    selector: "parcel-update-ownership-info-modal",
    standalone: true,
    imports: [CommonModule, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
    templateUrl: "./parcel-update-ownership-info-modal.component.html",
    styleUrls: ["./parcel-update-ownership-info-modal.component.scss"],
})
export class ParcelUpdateOwnershipInfoModalComponent implements IModal, OnInit {
    public modalComponentRef: ComponentRef<ModalComponent>;
    public FormFieldType = FormFieldType;
    public modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;
    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup<ParcelUpdateOwnershipRequestDtoForm> = new FormGroup<ParcelUpdateOwnershipRequestDtoForm>({
        ParcelID: ParcelUpdateOwnershipRequestDtoFormControls.ParcelID(),
        OwnerName: ParcelUpdateOwnershipRequestDtoFormControls.OwnerName(),
        OwnerAddress: ParcelUpdateOwnershipRequestDtoFormControls.OwnerAddress(),
    });

    constructor(private modalService: ModalService, private alertService: AlertService, private parcelService: ParcelService) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDGet(this.modalContext.ParcelID).pipe(
            tap((parcel) => {
                this.formGroup.setValue({ ParcelID: parcel.ParcelID, OwnerName: parcel.OwnerName, OwnerAddress: parcel.OwnerAddress });
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;

        this.parcelService.parcelsParcelIDOwnershipPut(this.modalContext.ParcelID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Parcel ownership successfully updated!", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
