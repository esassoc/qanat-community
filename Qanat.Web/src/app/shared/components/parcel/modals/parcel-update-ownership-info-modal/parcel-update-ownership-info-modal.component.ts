import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ParcelUpdateOwnershipRequestDtoForm, ParcelUpdateOwnershipRequestDtoFormControls } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "parcel-update-ownership-info-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent, AsyncPipe],
    templateUrl: "./parcel-update-ownership-info-modal.component.html",
    styleUrls: ["./parcel-update-ownership-info-modal.component.scss"],
})
export class ParcelUpdateOwnershipInfoModalComponent implements OnInit {
    public ref: DialogRef<ParcelContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public parcel$: Observable<ParcelMinimalDto>;
    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup<ParcelUpdateOwnershipRequestDtoForm> = new FormGroup<ParcelUpdateOwnershipRequestDtoForm>({
        ParcelID: ParcelUpdateOwnershipRequestDtoFormControls.ParcelID(),
        OwnerName: ParcelUpdateOwnershipRequestDtoFormControls.OwnerName(),
        OwnerAddress: ParcelUpdateOwnershipRequestDtoFormControls.OwnerAddress(),
    });

    constructor(
        private alertService: AlertService,
        private parcelService: ParcelService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.getByIDParcel(this.ref.data.ParcelID).pipe(
            tap((parcel) => {
                this.formGroup.setValue({ ParcelID: parcel.ParcelID, OwnerName: parcel.OwnerName, OwnerAddress: parcel.OwnerAddress });
            })
        );
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;

        this.parcelService.updateParcelOwnershipParcel(this.ref.data.ParcelID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Parcel ownership successfully updated!", AlertContext.Success));
                this.ref.close(true);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
