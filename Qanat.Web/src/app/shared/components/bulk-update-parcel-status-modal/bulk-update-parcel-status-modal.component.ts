import { Component, ComponentRef, inject, OnInit } from "@angular/core";

import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ParcelStatusEnum } from "src/app/shared/generated/enum/parcel-status-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import {
    ParcelBulkUpdateParcelStatusDto,
    ParcelBulkUpdateParcelStatusDtoForm,
    ParcelBulkUpdateParcelStatusDtoFormControls,
} from "src/app/shared/generated/model/parcel-bulk-update-parcel-status-dto";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { ParcelByGeographyService } from "../../generated/api/parcel-by-geography.service";
import { AlertDisplayComponent } from "../alert-display/alert-display.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "bulk-update-parcel-status-modal",
    imports: [ReactiveFormsModule, FormFieldComponent, FieldDefinitionComponent, AlertDisplayComponent],
    templateUrl: "./bulk-update-parcel-status-modal.component.html",
    styleUrls: ["./bulk-update-parcel-status-modal.component.scss"],
})
export class BulkUpdateParcelStatusModalComponent implements OnInit {
    public ref: DialogRef<ParcelUpdateContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public numberOfParcels: number;

    public formGroup: FormGroup<ParcelBulkUpdateParcelStatusDtoForm> = new FormGroup<ParcelBulkUpdateParcelStatusDtoForm>({
        ParcelStatusID: ParcelBulkUpdateParcelStatusDtoFormControls.ParcelStatusID(),
        ParcelIDs: ParcelBulkUpdateParcelStatusDtoFormControls.ParcelIDs(),
    });

    public statusOptions: SelectDropdownOption[] = [
        { Value: ParcelStatusEnum.Excluded, Label: "Excluded", disabled: false },
        { Value: ParcelStatusEnum.Inactive, Label: "Inactive", disabled: false },
        { Value: ParcelStatusEnum.Unassigned, Label: "Unassigned", disabled: false },
    ];

    public yearOptions: SelectDropdownOption[] = [{ Value: null, Label: "Select End Year:", disabled: true }];

    constructor(
        private parcelByGeographyService: ParcelByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.numberOfParcels = this.ref.data.ParcelIDs.length;
        if (this.ref.data.Years) {
            this.ref.data.Years.forEach((year) => {
                this.yearOptions.push({ Value: year, Label: year.toString(), disabled: false });
            });
        }
    }

    save(): void {
        const submitDto = new ParcelBulkUpdateParcelStatusDto(this.formGroup.value);
        submitDto.ParcelIDs = this.ref.data.ParcelIDs;

        this.parcelByGeographyService.bulkUpdateParcelStatusParcelByGeography(this.ref.data.GeographyID, submitDto).subscribe(() => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Parcels successfully updated!", AlertContext.Success));
            this.ref.close(true);
        });
    }

    close(): void {
        this.ref.close(false);
    }
}

export interface ParcelUpdateContext {
    ParcelIDs: number[];
    Years: number[];
    GeographyID: number;
}
