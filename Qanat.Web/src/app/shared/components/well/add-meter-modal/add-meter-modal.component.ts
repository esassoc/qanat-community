import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CommonModule } from "@angular/common";
import { MeterGridDtoForm, MeterGridDtoFormControls } from "src/app/shared/generated/model/meter-grid-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MeterContext } from "../update-meter-modal/update-meter-modal.component";
import { MeterService } from "src/app/shared/generated/api/meter.service";
import { ModalComponent } from "../../modal/modal.component";
import { FormFieldComponent, FormFieldType } from "../../forms/form-field/form-field.component";
import { AlertDisplayComponent } from "../../alert-display/alert-display.component";
import { MeterStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/meter-status-enum";

@Component({
    selector: "add-meter-modal",
    standalone: true,
    templateUrl: "./add-meter-modal.component.html",
    styleUrl: "./add-meter-modal.component.scss",
    imports: [IconComponent, CommonModule, ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent],
})
export class AddMeterModalComponent implements OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: MeterContext;
    public FormFieldType = FormFieldType;

    public MeterStatusSelectDropdownOptions = MeterStatusesAsSelectDropdownOptions;

    public formGroup = new FormGroup<MeterGridDtoForm>({
        SerialNumber: MeterGridDtoFormControls.SerialNumber(),
        DeviceName: MeterGridDtoFormControls.DeviceName(),
        Make: MeterGridDtoFormControls.Make(),
        ModelNumber: MeterGridDtoFormControls.ModelNumber(),
        MeterStatusID: MeterGridDtoFormControls.MeterStatusID(),
        GeographyID: MeterGridDtoFormControls.GeographyID(),
    });

    public isLoadingSubmit = false;

    constructor(
        private modalService: ModalService,
        private meterService: MeterService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({ GeographyID: this.modalContext.GeographyID });
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterService.geographiesGeographyIDMeterPost(this.modalContext.GeographyID, this.formGroup.getRawValue()).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Created new meter successfully.", AlertContext.Success));
            this.modalService.close(this.modalComponentRef, response);
        });
    }
}
