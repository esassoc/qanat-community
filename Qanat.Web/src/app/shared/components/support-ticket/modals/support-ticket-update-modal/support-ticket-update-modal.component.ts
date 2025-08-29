import { Component, inject, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormGroup } from "@angular/forms";
import { Observable } from "rxjs";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { SupportTicketService } from "../../../../generated/api/support-ticket.service";
import { SupportTicketUpsertDtoForm, SupportTicketUpsertDtoFormControls } from "../../../../generated/model/support-ticket-upsert-dto";
import { Alert } from "../../../../models/alert";
import { AlertContext } from "../../../../models/enums/alert-context.enum";
import { AlertService } from "../../../../services/alert.service";
import { WaterAccountMinimalDto } from "../../../../generated/model/water-account-minimal-dto";
import { SupportTicketUpdateContext } from "src/app/pages/support-ticket-admin-pages/support-ticket-detail/support-ticket-detail.component";
import { DialogRef } from "@ngneat/dialog";
import { FlagEnum } from "../../../../generated/enum/flag-enum";

@Component({
    selector: "support-ticket-update-modal",
    imports: [FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./support-ticket-update-modal.component.html",
    styleUrl: "./support-ticket-update-modal.component.scss",
})
export class SupportTicketUpdateModalComponent implements OnInit {
    public ref: DialogRef<SupportTicketUpdateContext, boolean> = inject(DialogRef);

    public geographyID: number;
    public geographyOptions$: Observable<SelectDropdownOption[]>;

    public waterAccounts$: Observable<WaterAccountMinimalDto>;
    public waterAccountOptions = {};

    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<SupportTicketUpsertDtoForm>({
        WaterAccountID: SupportTicketUpsertDtoFormControls.WaterAccountID(),
        Description: SupportTicketUpsertDtoFormControls.Description(),
        GeographyID: SupportTicketUpsertDtoFormControls.GeographyID(),
        ContactFirstName: SupportTicketUpsertDtoFormControls.ContactFirstName(),
        ContactLastName: SupportTicketUpsertDtoFormControls.ContactLastName(),
        ContactEmail: SupportTicketUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: SupportTicketUpsertDtoFormControls.ContactPhoneNumber(),
        SupportTicketPriorityID: SupportTicketUpsertDtoFormControls.SupportTicketPriorityID(),
        SupportTicketQuestionTypeID: SupportTicketUpsertDtoFormControls.SupportTicketQuestionTypeID(),
    });

    constructor(
        private alertService: AlertService,
        private supportTicketService: SupportTicketService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.WaterAccountID.setValue(this.ref.data.WaterAccountID);
        this.formGroup.controls.Description.setValue(this.ref.data.Description);
        this.formGroup.controls.GeographyID.setValue(this.ref.data.GeographyID);
        this.formGroup.controls.ContactFirstName.setValue(this.ref.data.ContactFirstName);
        this.formGroup.controls.ContactLastName.setValue(this.ref.data.ContactLastName);
        this.formGroup.controls.ContactEmail.setValue(this.ref.data.ContactEmail);
        this.formGroup.controls.ContactPhoneNumber.setValue(this.ref.data.ContactPhoneNumber);
        this.formGroup.controls.SupportTicketPriorityID.setValue(this.ref.data.SupportTicketPriorityID);
        this.formGroup.controls.SupportTicketQuestionTypeID.setValue(this.ref.data.SupportTicketQuestionTypeID);
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.supportTicketService.editSupportTicketSupportTicket(this.ref.data.SupportTicketID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Support request successfully updated.", AlertContext.Success));
                this.ref.close(true);
            },
            error: () => {
                this.ref.close(null);
            },
        });
    }
}
