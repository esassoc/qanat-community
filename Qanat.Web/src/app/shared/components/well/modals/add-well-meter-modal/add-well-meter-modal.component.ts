import { Component, inject, OnInit } from "@angular/core";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AddWellMeterRequestDtoForm, AddWellMeterRequestDtoFormControls } from "src/app/shared/generated/model/add-well-meter-request-dto";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { Observable, map } from "rxjs";
import { AsyncPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { MeterByGeographyService } from "src/app/shared/generated/api/meter-by-geography.service";
import { MeterByWellService } from "src/app/shared/generated/api/meter-by-well.service";
import { DialogRef } from "@ngneat/dialog";
import { MeterGridDto } from "src/app/shared/generated/model/meter-grid-dto";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

@Component({
    selector: "add-well-meter-modal",
    templateUrl: "./add-well-meter-modal.component.html",
    styleUrl: "./add-well-meter-modal.component.scss",
    imports: [IconComponent, AlertDisplayComponent, FormsModule, NoteComponent, AsyncPipe, FormFieldComponent, ReactiveFormsModule],
})
export class AddWellMeterModalComponent implements OnInit {
    public ref: DialogRef<AddWellMeterContext, MeterGridDto> = inject(DialogRef);

    public meters$: Observable<FormInputOption[]>;
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<AddWellMeterRequestDtoForm>({
        MeterID: AddWellMeterRequestDtoFormControls.MeterID(),
        WellID: AddWellMeterRequestDtoFormControls.WellID(),
        StartDate: AddWellMeterRequestDtoFormControls.StartDate(),
    });
    public isLoadingSubmit: boolean = false;

    constructor(
        private meterByWellService: MeterByWellService,
        private meterByGeographyService: MeterByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.meters$ = this.meterByGeographyService.listUnassignedMetersMeterByGeography(this.ref.data.GeographyID).pipe(
            map((meters) => {
                return meters.map((x) => {
                    return {
                        Value: x.MeterID,
                        Label: x.LinkDisplay,
                    } as FormInputOption;
                });
            })
        );

        this.formGroup.patchValue({ WellID: this.ref.data.WellID });
    }

    save() {
        this.isLoadingSubmit = true;
        this.meterByWellService.addWellMeterMeterByWell(this.ref.data.WellID, this.formGroup.getRawValue()).subscribe({
            next: (response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Meter successfully assigned to well.", AlertContext.Success));
                this.ref.close(response);
                this.isLoadingSubmit = false;
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close() {
        this.ref.close(null);
    }
}

export class AddWellMeterContext {
    WellID: number;
    WellName: string;
    GeographyID: number;
}
