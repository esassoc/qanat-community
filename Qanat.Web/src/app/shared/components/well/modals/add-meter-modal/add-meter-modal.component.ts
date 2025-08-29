import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { MeterGridDto, MeterGridDtoForm, MeterGridDtoFormControls } from "src/app/shared/generated/model/meter-grid-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { MeterStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/meter-status-enum";
import { DialogRef } from "@ngneat/dialog";
import { MeterByGeographyService } from "src/app/shared/generated/api/meter-by-geography.service";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { MeterContext } from "src/app/shared/components/well/modals/update-meter-modal/update-meter-modal.component";

@Component({
    selector: "add-meter-modal",
    templateUrl: "./add-meter-modal.component.html",
    styleUrl: "./add-meter-modal.component.scss",
    imports: [ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent],
})
export class AddMeterModalComponent implements OnInit {
    public ref: DialogRef<MeterContext, MeterGridDto> = inject(DialogRef);
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
        private meterByGeographyService: MeterByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({ GeographyID: this.ref.data.GeographyID });
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterByGeographyService.addMeterMeterByGeography(this.ref.data.GeographyID, this.formGroup.getRawValue()).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Created new meter successfully.", AlertContext.Success));
            this.ref.close(response);
        });
    }
}
