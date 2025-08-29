import { Component, inject } from "@angular/core";

import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { Observable } from "rxjs";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ParcelStatusEnum } from "src/app/shared/generated/enum/parcel-status-enum";
import { ParcelBulkUpdateParcelStatusDto, ParcelBulkUpdateParcelStatusDtoForm } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { ParcelByGeographyService } from "../../../../generated/api/parcel-by-geography.service";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../alert-display/alert-display.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "parcel-modify-parcel-status-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent, AsyncPipe, AlertDisplayComponent],
    templateUrl: "./parcel-modify-parcel-status-modal.component.html",
    styleUrls: ["./parcel-modify-parcel-status-modal.component.scss"],
})
export class ParcelModifyParcelStatusModalComponent {
    public ref: DialogRef<ParcelContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public parcel$: Observable<ParcelMinimalDto>;
    public isLoadingSubmit: boolean = false;

    public statusOptions: SelectDropdownOption[] = [
        { Value: ParcelStatusEnum.Excluded, Label: "Excluded", disabled: false },
        { Value: ParcelStatusEnum.Inactive, Label: "Inactive", disabled: false },
        { Value: ParcelStatusEnum.Unassigned, Label: "Unassigned", disabled: false },
    ];

    public formGroup: FormGroup<ParcelBulkUpdateParcelStatusDtoForm> = new FormGroup<ParcelBulkUpdateParcelStatusDtoForm>({
        ParcelStatusID: new FormControl<ParcelStatusEnum>(null, [Validators.required]),
    });

    constructor(
        private alertService: AlertService,
        private parcelService: ParcelService,
        private parcelByGeographyService: ParcelByGeographyService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.getByIDParcel(this.ref.data.ParcelID);
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;

        const submitDto = new ParcelBulkUpdateParcelStatusDto(this.formGroup.value);
        submitDto.ParcelIDs = [this.ref.data.ParcelID];

        this.parcelByGeographyService.bulkUpdateParcelStatusParcelByGeography(this.ref.data.GeographyID, submitDto).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Parcels successfully updated!", AlertContext.Success));
            this.ref.close(true);
        });
    }
}
