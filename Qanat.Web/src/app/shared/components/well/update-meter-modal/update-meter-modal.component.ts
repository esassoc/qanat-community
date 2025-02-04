import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { MeterGridDto, MeterGridDtoForm, MeterGridDtoFormControls } from "src/app/shared/generated/model/meter-grid-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { CommonModule } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { MeterService } from "src/app/shared/generated/api/meter.service";
import { FormFieldComponent, FormFieldType } from "../../forms/form-field/form-field.component";
import { AlertDisplayComponent } from "../../alert-display/alert-display.component";
import { MeterStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/meter-status-enum";

@Component({
    selector: "update-meter-modal",
    standalone: true,
    imports: [IconComponent, CommonModule, ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent],
    templateUrl: "./update-meter-modal.component.html",
    styleUrl: "./update-meter-modal.component.scss",
})
export class UpdateMeterModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: MeterContext;
    public FormFieldType = FormFieldType;

    public MeterStatusSelectDropdownOptions = MeterStatusesAsSelectDropdownOptions;
    public meter$: Observable<MeterGridDto>;

    public formGroup = new FormGroup<MeterGridDtoForm>({
        MeterID: MeterGridDtoFormControls.MeterID(),
        SerialNumber: MeterGridDtoFormControls.SerialNumber(),
        DeviceName: MeterGridDtoFormControls.DeviceName(),
        Make: MeterGridDtoFormControls.Make(),
        ModelNumber: MeterGridDtoFormControls.ModelNumber(),
        MeterStatusID: MeterGridDtoFormControls.MeterID(),
        MeterStatus: MeterGridDtoFormControls.MeterStatus(),
        GeographyID: MeterGridDtoFormControls.GeographyID(),
        WellIDs: MeterGridDtoFormControls.WellIDs(),
    });

    public isLoadingSubmit = false;

    constructor(private modalService: ModalService, private meterService: MeterService, private alertService: AlertService) {}

    ngOnInit(): void {
        this.meter$ = this.meterService.geographiesGeographyIDMeterMeterIDGet(this.modalContext.GeographyID, this.modalContext.MeterID).pipe(
            tap((meter) => {
                this.formGroup.setValue(meter);
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterService.geographiesGeographyIDMeterMeterIDPost(this.modalContext.GeographyID, this.modalContext.MeterID, this.formGroup.getRawValue()).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Updated meter successfully.", AlertContext.Success));
            this.modalService.close(this.modalComponentRef, response);
        });
    }
}

export class MeterContext {
    public MeterID: number;
    public GeographyID: number;
}
