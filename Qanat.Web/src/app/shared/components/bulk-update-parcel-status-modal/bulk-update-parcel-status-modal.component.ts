import { Component, ComponentRef, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ParcelStatusEnum } from "src/app/shared/generated/enum/parcel-status-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import {
    ParcelBulkUpdateParcelStatusDto,
    ParcelBulkUpdateParcelStatusDtoForm,
    ParcelBulkUpdateParcelStatusDtoFormControls,
} from "src/app/shared/generated/model/parcel-bulk-update-parcel-status-dto";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { AlertContext } from "../../models/enums/alert-context.enum";

@Component({
    selector: "bulk-update-parcel-status-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, FieldDefinitionComponent],
    templateUrl: "./bulk-update-parcel-status-modal.component.html",
    styleUrls: ["./bulk-update-parcel-status-modal.component.scss"],
})
export class BulkUpdateParcelStatusModalComponent implements OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: ParcelUpdateContext;
    public FormFieldType = FormFieldType;
    public numberOfParcels: number;

    public formGroup: FormGroup<ParcelBulkUpdateParcelStatusDtoForm> = new FormGroup<ParcelBulkUpdateParcelStatusDtoForm>({
        ParcelStatusID: ParcelBulkUpdateParcelStatusDtoFormControls.ParcelStatusID(),
        ParcelIDs: ParcelBulkUpdateParcelStatusDtoFormControls.ParcelIDs(),
        EndYear: ParcelBulkUpdateParcelStatusDtoFormControls.EndYear(),
    });

    public statusOptions: SelectDropdownOption[] = [
        { Value: null, Label: "Select Parcel Status", Disabled: true },
        { Value: ParcelStatusEnum.Excluded, Label: "Excluded", Disabled: false },
        { Value: ParcelStatusEnum.Inactive, Label: "Inactive", Disabled: false },
        { Value: ParcelStatusEnum.Unassigned, Label: "Unassigned", Disabled: false },
    ];

    public yearOptions: SelectDropdownOption[] = [{ Value: null, Label: "Select End Year:", Disabled: true }];

    constructor(
        private modalService: ModalService,
        private parcelService: ParcelService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.numberOfParcels = this.modalContext.ParcelIDs.length;
        if (this.modalContext.Years) {
            this.modalContext.Years.forEach((year) => {
                this.yearOptions.push({ Value: year, Label: year.toString(), Disabled: false });
            });
        }
    }

    save(): void {
        const submitDto = new ParcelBulkUpdateParcelStatusDto(this.formGroup.value);
        submitDto.ParcelIDs = this.modalContext.ParcelIDs;
        if (!this.modalContext.Years && !submitDto.EndYear) {
            submitDto.EndYear = new Date().getFullYear(); // default to current year if no year picker
        }

        this.parcelService.geographiesGeographyIDParcelsBulkUpdateParcelStatusPost(this.modalContext.GeographyID, submitDto).subscribe(() => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Parcels successfully updated!", AlertContext.Success));
            this.modalService.close(this.modalComponentRef, true);
        });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef, false);
    }
}

export interface ParcelUpdateContext {
    ParcelIDs: number[];
    Years: number[];
    GeographyID: number;
}
