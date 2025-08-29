import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { MeterGridDto, MeterGridDtoForm, MeterGridDtoFormControls } from "src/app/shared/generated/model/meter-grid-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { MeterStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/meter-status-enum";
import { MeterByGeographyService } from "src/app/shared/generated/api/meter-by-geography.service";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

@Component({
    selector: "update-meter-modal",
    imports: [ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent, AsyncPipe],
    templateUrl: "./update-meter-modal.component.html",
    styleUrl: "./update-meter-modal.component.scss",
})
export class UpdateMeterModalComponent implements OnInit {
    public ref: DialogRef<MeterContext, MeterGridDto> = inject(DialogRef);
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
        WellID: MeterGridDtoFormControls.WellID(),
    });

    public isLoadingSubmit = false;

    constructor(
        private meterByGeographyService: MeterByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.meter$ = this.meterByGeographyService.getByIDMeterByGeography(this.ref.data.GeographyID, this.ref.data.MeterID).pipe(
            tap((meter) => {
                this.formGroup.setValue(meter);
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterByGeographyService.updateMeterMeterByGeography(this.ref.data.GeographyID, this.ref.data.MeterID, this.formGroup.getRawValue()).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Updated meter successfully.", AlertContext.Success));
            this.ref.close(response);
        });
    }
}

export class MeterContext {
    public MeterID: number;
    public GeographyID: number;
}
