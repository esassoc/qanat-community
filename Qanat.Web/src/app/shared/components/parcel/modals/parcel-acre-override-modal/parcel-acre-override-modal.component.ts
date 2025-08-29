import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelAcreUpdateDto, ParcelAcreUpdateDtoForm, ParcelAcreUpdateDtoFormControls } from "src/app/shared/generated/model/parcel-acre-update-dto";
import { Observable, tap } from "rxjs";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { ParcelContext } from "../../../water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { NoteComponent } from "../../../note/note.component";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "parcel-acre-override-modal",
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent, AsyncPipe],
    templateUrl: "./parcel-acre-override-modal.component.html",
    styleUrl: "./parcel-acre-override-modal.component.scss",
})
export class ParcelAcreOverrideModalComponent implements OnInit {
    public ref: DialogRef<ParcelContext, boolean> = inject(DialogRef);
    public parcel$: Observable<ParcelMinimalDto>;

    public isLoadingSubmit: boolean = false;

    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<ParcelAcreUpdateDtoForm> = new FormGroup({
        Acres: ParcelAcreUpdateDtoFormControls.Acres(null, {
            validators: [Validators.required, Validators.min(0)],
        }),
    });

    constructor(
        private parcelService: ParcelService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.getByIDParcel(this.ref.data.ParcelID).pipe(
            tap((parcel) => {
                this.formGroup.setValue({ Acres: parcel.ParcelArea });
            })
        );
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;

        let parcelAcreUpdateDto = new ParcelAcreUpdateDto({
            Acres: this.formGroup.get("Acres").value,
        });

        this.parcelService.updateParcelAcresParcel(this.ref.data.ParcelID, parcelAcreUpdateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Parcel acres successfully updated!", AlertContext.Success));
                this.ref.close(true);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
