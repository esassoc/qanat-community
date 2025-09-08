import { Component, inject, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormGroup } from "@angular/forms";
import { SupportTicketResponseContext } from "src/app/pages/support-ticket-admin-pages/support-ticket-detail/support-ticket-detail.component";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { SupportTicketNoteSimpleDtoForm, SupportTicketNoteSimpleDtoFormControls } from "src/app/shared/generated/model/support-ticket-note-simple-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { SupportTicketService } from "src/app/shared/generated/api/support-ticket.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

@Component({
    selector: "support-ticket-response-modal",
    imports: [FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./support-ticket-response-modal.component.html",
    styleUrl: "./support-ticket-response-modal.component.scss",
})
export class SupportTicketResponseModal implements OnInit {
    public ref: DialogRef<SupportTicketResponseContext, boolean> = inject(DialogRef);

    public formGroup = new FormGroup<SupportTicketNoteSimpleDtoForm>({
        SupportTicketID: SupportTicketNoteSimpleDtoFormControls.SupportTicketID(),
        InternalNote: SupportTicketNoteSimpleDtoFormControls.InternalNote(),
        Message: SupportTicketNoteSimpleDtoFormControls.Message(),
    });
    public FormFieldType = FormFieldType;

    constructor(
        private alertService: AlertService,
        private supportTicketService: SupportTicketService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.SupportTicketID.setValue(this.ref.data.SupportTicketID);
        this.formGroup.controls.InternalNote.setValue(this.ref.data.IsInternalNote);
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.supportTicketService.createSupportTicketNoteSupportTicket(this.ref.data.SupportTicketID, this.formGroup.getRawValue()).subscribe({
            next: (response) => {
                this.alertService.pushAlert(new Alert("Support request successfully updated.", AlertContext.Success));
                this.ref.close(true);
            },
            error: (error) => {
                this.alertService.pushAlert(new Alert("An error occurred while attempting to update support ticket.", AlertContext.Danger));
                this.ref.close(null);
            },
        });
    }
}
