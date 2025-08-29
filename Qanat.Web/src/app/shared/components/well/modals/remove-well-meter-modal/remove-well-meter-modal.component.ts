import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RemoveWellMeterRequestDtoForm, RemoveWellMeterRequestDtoFormControls } from "src/app/shared/generated/model/remove-well-meter-request-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MeterByWellService } from "src/app/shared/generated/api/meter-by-well.service";
import { DialogRef } from "@ngneat/dialog";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "remove-well-meter-modal",
    templateUrl: "./remove-well-meter-modal.component.html",
    styleUrl: "./remove-well-meter-modal.component.scss",
    imports: [AlertDisplayComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
})
export class RemoveWellMeterModalComponent implements OnInit {
    public ref: DialogRef<RemoveWellMeterContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<RemoveWellMeterRequestDtoForm>({
        WellID: RemoveWellMeterRequestDtoFormControls.WellID(),
        MeterID: RemoveWellMeterRequestDtoFormControls.MeterID(),
        EndDate: RemoveWellMeterRequestDtoFormControls.EndDate(),
    });
    public isLoadingSubmit: boolean = false;

    constructor(
        private meterByWellService: MeterByWellService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({ WellID: this.ref.data.WellID, MeterID: this.ref.data.MeterID });
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterByWellService.removeWellMeterMeterByWell(this.ref.data.WellID, this.ref.data.MeterID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Meter successfully removed from well.", AlertContext.Success));
                this.ref.close(true);
                this.isLoadingSubmit = false;
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close() {
        this.ref.close(false);
    }
}

export class RemoveWellMeterContext {
    WellID: number;
    WellName: string;
    MeterID: number;
    DeviceName: string;
}
