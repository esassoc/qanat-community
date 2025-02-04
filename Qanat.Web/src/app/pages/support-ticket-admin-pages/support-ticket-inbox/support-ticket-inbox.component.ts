import { AsyncPipe, CommonModule, DatePipe, NgClass, NgFor, NgIf } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink, RouterLinkActive } from "@angular/router";
import { Observable, share, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { SupportTicketService } from "src/app/shared/generated/api/support-ticket.service";
import { SupportTicketGridDto } from "src/app/shared/generated/model/support-ticket-grid-dto";
import { PageHeaderComponent } from "../../../shared/components/page-header/page-header.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { KeyValuePairListComponent } from "../../../shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "../../../shared/components/key-value-pair/key-value-pair.component";
import { PhonePipe } from "src/app/shared/pipes/phone.pipe";
import { SupportTicketNoteSimpleDtoForm, SupportTicketNoteSimpleDtoFormControls } from "src/app/shared/generated/model/support-ticket-note-simple-dto";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "../../../shared/components/forms/form-field/form-field.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { SupportTicketNoteFeedDto } from "src/app/shared/generated/model/support-ticket-note-feed-dto";
import { UpdateSupportTicketModalComponent } from "src/app/shared/components/update-support-ticket-modal/update-support-ticket-modal.component";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { SupportTicketContext } from "../../support-ticket-list/support-ticket-list.component";
import { DomSanitizer } from "@angular/platform-browser";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";

@Component({
    selector: "support-ticket-inbox",
    standalone: true,
    imports: [
        NgIf,
        AsyncPipe,
        PageHeaderComponent,
        ModelNameTagComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        DatePipe,
        RouterLink,
        PhonePipe,
        ReactiveFormsModule,
        FormFieldComponent,
        LoadingDirective,
        ButtonLoadingDirective,
        NgFor,
        NgClass,
        RouterLinkActive,
        IconComponent,
        AlertDisplayComponent,
    ],
    templateUrl: "./support-ticket-inbox.component.html",
    styleUrl: "./support-ticket-inbox.component.scss",
})
export class SupportTicketInboxComponent {
    public supportTicket$: Observable<SupportTicketGridDto>;
    public supportTicketNotes$: Observable<SupportTicketNoteFeedDto[]>;
    public isLoadingSubmit: boolean = false;
    public supportTicketID: number;
    public currentTab: string = "Feed";

    public formGroup = new FormGroup<SupportTicketNoteSimpleDtoForm>({
        SupportTicketID: SupportTicketNoteSimpleDtoFormControls.SupportTicketID(),
        InternalNote: SupportTicketNoteSimpleDtoFormControls.InternalNote(),
        Message: SupportTicketNoteSimpleDtoFormControls.Message(),
    });
    public FormFieldType = FormFieldType;

    constructor(private supportTicketService: SupportTicketService, private route: ActivatedRoute, private modalService: ModalService, private sanitizer: DomSanitizer) {}

    ngOnInit() {
        this.supportTicketID = parseInt(this.route.snapshot.paramMap.get(routeParams.supportTicketID));
        this.formGroup.controls.SupportTicketID.setValue(this.supportTicketID);
        this.loadSupportTicketNotes();
    }

    changeTab(tabName: string) {
        this.currentTab = tabName;
    }

    onSubmit(internalNote: boolean) {
        this.isLoadingSubmit = true;
        this.formGroup.controls.InternalNote.setValue(internalNote);
        this.supportTicketService.supportTicketsSupportTicketIDNotesPost(1, this.formGroup.getRawValue()).subscribe((response) => {
            this.isLoadingSubmit = false;
            this.formGroup.reset();
            this.formGroup.controls.SupportTicketID.setValue(this.supportTicketID);
            this.loadSupportTicketNotes();
        });
    }

    loadSupportTicketNotes() {
        this.supportTicket$ = this.supportTicketService.supportTicketsSupportTicketIDGet(this.supportTicketID);
        this.supportTicketNotes$ = this.supportTicketService.supportTicketsSupportTicketIDNotesGet(this.supportTicketID).pipe(share());
    }

    loadHTML(html) {
        return this.sanitizer.bypassSecurityTrustHtml(html);
    }

    getInitials(firstName: string, lastName: string): string {
        if (!firstName || !lastName) {
            return ""; // Handle cases where the name is missing
        }
        return firstName.charAt(0).toUpperCase() + lastName.charAt(0).toUpperCase();
    }

    public openUpdateModal(supportTicket: SupportTicketGridDto) {
        this.modalService
            .open(UpdateSupportTicketModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                SupportTicketID: supportTicket.SupportTicketID,
                WaterAccountID: supportTicket.WaterAccountID,
                Description: supportTicket.Description,
                SupportTicketPriorityID: supportTicket.SupportTicketPriority.SupportTicketPriorityID,
                GeographyID: supportTicket.GeographyID,
                ContactFirstName: supportTicket.ContactFirstName,
                ContactLastName: supportTicket.ContactLastName,
                ContactEmail: supportTicket.ContactEmail,
                ContactPhoneNumber: supportTicket.ContactPhoneNumber,
                AssignedUserID: supportTicket.AssignedUserID,
            } as SupportTicketContext)
            .instance.result.then((result) => {
                if (result) {
                    this.loadSupportTicketNotes();
                }
            });
    }
}
