import { Component, OnInit } from "@angular/core";
import { SupportTicketService } from "../../shared/generated/api/support-ticket.service";
import { Observable } from "rxjs";
import { UtilityFunctionsService } from "../../shared/services/utility-functions.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { PageHeaderComponent } from "../../shared/components/page-header/page-header.component";
import { QanatGridComponent } from "../../shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe, NgIf } from "@angular/common";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "../../shared/services/modal/modal.service";
import { CreateSupportModalComponent } from "../../shared/components/create-support-modal/create-support-modal.component";
import { ConfirmService } from "../../shared/services/confirm/confirm.service";
import { Alert } from "../../shared/models/alert";
import { AlertContext } from "../../shared/models/enums/alert-context.enum";
import { ConfirmOptions } from "../../shared/services/confirm/confirm-options";
import { AlertService } from "../../shared/services/alert.service";
import { UpdateSupportTicketModalComponent } from "../../shared/components/update-support-ticket-modal/update-support-ticket-modal.component";
import { SupportTicketStatusEnum } from "../../shared/generated/enum/support-ticket-status-enum";
import { SupportTicketGridDto } from "../../shared/generated/model/support-ticket-grid-dto";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "support-ticket-list",
    standalone: true,
    imports: [PageHeaderComponent, QanatGridComponent, AsyncPipe, NgIf, AlertDisplayComponent, IconComponent],
    templateUrl: "./support-ticket-list.component.html",
    styleUrl: "./support-ticket-list.component.scss",
})
export class SupportTicketListComponent implements OnInit {
    public supportTicketDtos$: Observable<SupportTicketGridDto[]>;
    public columnDefs: ColDef[];
    public gridApi: GridApi;

    constructor(
        private supportTicketService: SupportTicketService,
        private utilityFunctionsService: UtilityFunctionsService,
        private modalService: ModalService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.loadSupportTickets();
        this.initializeGrid();
    }

    loadSupportTickets() {
        this.supportTicketDtos$ = this.supportTicketService.supportTicketsGet();
    }

    private initializeGrid(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [];
                actions.push({ ActionName: "View Ticket", ActionIcon: "fas fa-map", ActionLink: `${params.data.SupportTicketID}` });
                if (params.data.SupportTicketStatus.SupportTicketStatusID != SupportTicketStatusEnum.Closed) {
                    actions.push({ ActionName: "Update", ActionIcon: "fas fa-map", ActionHandler: () => this.openUpdateModal(params.data) });
                    actions.push({ ActionName: "Close", ActionIcon: "fas fa-info-circle", ActionHandler: () => this.openDeleteModal(params.data.SupportTicketID) });
                } else {
                    actions.push({ ActionName: "Reopen", ActionIcon: "fas fa-info-circle", ActionHandler: () => this.openReopenModal(params.data.SupportTicketID) });
                }

                return actions.length == 0 ? null : actions;
            }),
            this.utilityFunctionsService.createLinkColumnDef("Ticket ID", "SupportTicketID", "SupportTicketID"),
            this.utilityFunctionsService.createBasicColumnDef("Geography", "GeographyName"),
            this.utilityFunctionsService.createBasicColumnDef("Assigned To", "AssignedUserFullName"),
            this.utilityFunctionsService.createBasicColumnDef("Status", "SupportTicketStatus.SupportTicketStatusDisplayName", {
                CustomDropdownFilterField: "SupportTicketStatus.SupportTicketStatusDisplayName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Priority", "SupportTicketPriority.SupportTicketPriorityDisplayName", {
                CustomDropdownFilterField: "SupportTicketPriority.SupportTicketPriorityDisplayName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Question Type", "SupportTicketQuestionType.SupportTicketQuestionTypeDisplayName", {
                CustomDropdownFilterField: "SupportTicketQuestionType.SupportTicketQuestionTypeDisplayName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Water Account", "WaterAccountNumber", {
                FieldDefinitionType: "WaterAccount",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "ContactFirstName", {
                ValueFormatter: (params) => {
                    return params.data.ContactFirstName + " " + params.data.ContactLastName;
                },
            }),
            this.utilityFunctionsService.createBasicColumnDef("Contact Email", "ContactEmail"),
            this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone Number", "ContactPhoneNumber"),
            this.utilityFunctionsService.createDateColumnDef("Date Updated", "DateUpdated", "M/d/yyyy"),
            this.utilityFunctionsService.createBasicColumnDef("Created By", "CreateUserFullName"),
            this.utilityFunctionsService.createDateColumnDef("Date Created", "DateCreated", "M/d/yyyy"),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }
    public openCreateNewModal() {
        this.modalService
            .open(CreateSupportModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light })
            .instance.result.then((result) => {
                if (result) {
                    this.loadSupportTickets();
                }
            });
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
                    this.loadSupportTickets();
                }
            });
    }

    public openDeleteModal(supportTicketID) {
        const options = {
            title: "Confirm: Close Support Ticket",
            message: "Are you sure you want to close this support ticket?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.supportTicketService.supportTicketsSupportTicketIDClosePut(supportTicketID).subscribe((response) => {
                    this.alertService.pushAlert(new Alert("Sucessfully closed support ticket.", AlertContext.Success));
                    this.loadSupportTickets();
                });
            }
        });
    }
    public openReopenModal(supportTicketID) {
        const options = {
            title: "Confirm: Reopen Support Ticket",
            message: "Are you sure you want to reopen this support ticket?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.supportTicketService.supportTicketsSupportTicketIDReopenPut(supportTicketID).subscribe((response) => {
                    this.alertService.pushAlert(new Alert("Sucessfully reopened support ticket.", AlertContext.Success));
                    this.loadSupportTickets();
                });
            }
        });
    }
}

export class SupportTicketContext {
    SupportTicketID: number;
    WaterAccountID: number;
    Description: string;
    SupportTicketPriorityID: number;
    GeographyID: number;
    ContactFirstName: string;
    ContactLastName: string;
    ContactEmail: string;
    ContactPhoneNumber: string;
    AssignedUserID: number;
}
