import { CommonModule } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { IconComponent } from "../../../icon/icon.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { ParcelAcreUpdateDto, ParcelAcreUpdateDtoForm, ParcelAcreUpdateDtoFormControls } from "src/app/shared/generated/model/parcel-acre-update-dto";
import { Observable, tap } from "rxjs";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { ParcelContext } from "../../../water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { NoteComponent } from "../../../note/note.component";

@Component({
    selector: "parcel-acre-override-modal",
    standalone: true,
    imports: [CommonModule, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
    templateUrl: "./parcel-acre-override-modal.component.html",
    styleUrl: "./parcel-acre-override-modal.component.scss",
})
export class ParcelAcreOverrideModalComponent implements IModal, OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;

    public modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;

    public isLoadingSubmit: boolean = false;

    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<ParcelAcreUpdateDtoForm> = new FormGroup({
        Acres: ParcelAcreUpdateDtoFormControls.Acres(null, {
            validators: [Validators.required, Validators.min(0)],
        }),
    });

    constructor(private modalService: ModalService, private parcelService: ParcelService, private alertService: AlertService) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDGet(this.modalContext.ParcelID).pipe(
            tap((parcel) => {
                this.formGroup.setValue({ Acres: parcel.ParcelArea });
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;

        let parcelAcreUpdateDto = new ParcelAcreUpdateDto({
            Acres: this.formGroup.get("Acres").value,
        });

        this.parcelService.parcelsParcelIDAcresPut(this.modalContext.ParcelID, parcelAcreUpdateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Parcel acres successfully updated!", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
