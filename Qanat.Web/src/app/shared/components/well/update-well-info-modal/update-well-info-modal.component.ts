import { Component, ComponentRef, OnInit } from "@angular/core";
import { ModalComponent } from "../../modal/modal.component";
import { FormFieldType, FormFieldComponent } from "../../forms/form-field/form-field.component";
import { ManagerWellUpdateRequestDtoForm, ManagerWellUpdateRequestDtoFormControls } from "src/app/shared/generated/model/manager-well-update-request-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WellContext } from "../delete-well-modal/delete-well-modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WellMinimalDto } from "src/app/shared/generated/model/well-minimal-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NoteComponent } from "../../note/note.component";
import { AlertDisplayComponent } from "../../alert-display/alert-display.component";
import { CustomRichTextComponent } from "../../custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { WellStatusesAsSelectDropdownOptions } from "src/app/shared/generated/enum/well-status-enum";

@Component({
    selector: "update-well-info-modal",
    templateUrl: "./update-well-info-modal.component.html",
    styleUrl: "./update-well-info-modal.component.scss",
    standalone: true,
    imports: [NgIf, IconComponent, CustomRichTextComponent, AlertDisplayComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent, AsyncPipe],
})
export class UpdateWellInfoModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: WellContext;
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
        WellDepth: ManagerWellUpdateRequestDtoFormControls.WellDepth(),
        WellStatusID: ManagerWellUpdateRequestDtoFormControls.WellStatusID(),
        Notes: ManagerWellUpdateRequestDtoFormControls.Notes(),
    });

    constructor(
        private wellService: WellService,
        private modalService: ModalService,
        private alertService: AlertService
    ) {}

    public ngOnInit(): void {
        this.well$ = this.wellService.wellsWellIDGet(this.modalContext.WellID).pipe(
            tap((well) => {
                this.formGroup.setValue({
                    WellID: well.WellID,
                    StateWCRNumber: well.StateWCRNumber,
                    CountyWellPermitNumber: well.CountyWellPermitNumber,
                    DateDrilled: well.DateDrilled,
                    WellDepth: well.WellDepth,
                    WellStatusID: well.WellStatusID,
                    Notes: well.Notes,
                });
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.wellService.wellsWellIDPut(this.modalContext.WellID, this.formGroup.getRawValue()).subscribe({
            next: (response) => {
                this.alertService.clearAlerts();
                this.modalService.close(this.modalComponentRef, response);
                this.alertService.pushAlert(new Alert("Updated well successfully.", AlertContext.Success));
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
