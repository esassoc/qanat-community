import { Component, inject, OnInit } from "@angular/core";
import { ManagerWellUpdateRequestDtoForm, ManagerWellUpdateRequestDtoFormControls } from "src/app/shared/generated/model/manager-well-update-request-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WellContext } from "../delete-well-modal/delete-well-modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WellMinimalDto } from "src/app/shared/generated/model/well-minimal-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AsyncPipe } from "@angular/common";
import { WellStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/well-status-enum";
import { DialogRef } from "@ngneat/dialog";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "update-well-info-modal",
    templateUrl: "./update-well-info-modal.component.html",
    styleUrl: "./update-well-info-modal.component.scss",
    imports: [CustomRichTextComponent, AlertDisplayComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent, AsyncPipe],
})
export class UpdateWellInfoModalComponent implements OnInit {
    public ref: DialogRef<WellContext, WellMinimalDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public well$: Observable<WellMinimalDto>;

    public isLoadingSubmit = false;
    public customRichTextTypeID = CustomRichTextTypeEnum.UpdateWellInfo;
    public WellStatusOptions = WellStatusesAsSelectDropdownOptions;

    public formGroup = new FormGroup<ManagerWellUpdateRequestDtoForm>({
        WellID: ManagerWellUpdateRequestDtoFormControls.WellID(),
        StateWCRNumber: ManagerWellUpdateRequestDtoFormControls.StateWCRNumber(),
        CountyWellPermitNumber: ManagerWellUpdateRequestDtoFormControls.CountyWellPermitNumber(),
        DateDrilled: ManagerWellUpdateRequestDtoFormControls.DateDrilled(),
        WellStatusID: ManagerWellUpdateRequestDtoFormControls.WellStatusID(),
        Notes: ManagerWellUpdateRequestDtoFormControls.Notes(),
        WellDepth: ManagerWellUpdateRequestDtoFormControls.WellDepth(),
        CasingDiameter: ManagerWellUpdateRequestDtoFormControls.CasingDiameter(),
        TopOfPerforations: ManagerWellUpdateRequestDtoFormControls.TopOfPerforations(),
        BottomOfPerforations: ManagerWellUpdateRequestDtoFormControls.BottomOfPerforations(),
        ElectricMeterNumber: ManagerWellUpdateRequestDtoFormControls.ElectricMeterNumber(),
    });

    constructor(
        private wellService: WellService,
        private alertService: AlertService
    ) {}

    public ngOnInit(): void {
        this.well$ = this.wellService.getWellAsMinimalDtoWell(this.ref.data.WellID).pipe(
            tap((well) => {
                this.formGroup.setValue({
                    WellID: well.WellID,
                    StateWCRNumber: well.StateWCRNumber,
                    CountyWellPermitNumber: well.CountyWellPermitNumber,
                    DateDrilled: well.DateDrilled,
                    WellStatusID: well.WellStatusID,
                    Notes: well.Notes,
                    WellDepth: well.WellDepth,
                    CasingDiameter: well.CasingDiameter,
                    TopOfPerforations: well.TopOfPerforations,
                    BottomOfPerforations: well.BottomOfPerforations,
                    ElectricMeterNumber: well.ElectricMeterNumber,
                });
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;
        this.wellService.updateWellDetailsWell(this.ref.data.WellID, this.formGroup.getRawValue()).subscribe({
            next: (response) => {
                this.alertService.clearAlerts();
                this.ref.close(response);
                this.alertService.pushAlert(new Alert("Updated well successfully.", AlertContext.Success));
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
