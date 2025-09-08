import { Component, ComponentRef, OnInit } from "@angular/core";
import { ModalComponent } from "../../modal/modal.component";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RemoveWellMeterRequestDtoForm, RemoveWellMeterRequestDtoFormControls } from "src/app/shared/generated/model/remove-well-meter-request-dto";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { WellService } from "src/app/shared/generated/api/well.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldType, FormFieldComponent } from "../../forms/form-field/form-field.component";
import { NoteComponent } from "../../note/note.component";
import { AlertDisplayComponent } from "../../alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "remove-well-meter-modal",
    templateUrl: "./remove-well-meter-modal.component.html",
    styleUrl: "./remove-well-meter-modal.component.scss",
    standalone: true,
    imports: [IconComponent, AlertDisplayComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
})
export class RemoveWellMeterModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: RemoveWellMeterContext;
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<RemoveWellMeterRequestDtoForm>({
        WellID: RemoveWellMeterRequestDtoFormControls.WellID(),
        MeterID: RemoveWellMeterRequestDtoFormControls.MeterID(),
        EndDate: RemoveWellMeterRequestDtoFormControls.EndDate(),
    });
    public isLoadingSubmit: boolean = false;

    constructor(
        private modalService: ModalService,
        private wellService: WellService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({ WellID: this.modalContext.WellID, MeterID: this.modalContext.MeterID });
    }

    save() {
        this.isLoadingSubmit = true;
        this.wellService.wellsWellIDMetersPut(this.modalContext.WellID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Meter successfully removed from well.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
                this.isLoadingSubmit = false;
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }
}

export class RemoveWellMeterContext {
    WellID: number;
    WellName: string;
    MeterID: number;
    DeviceName: string;
}
