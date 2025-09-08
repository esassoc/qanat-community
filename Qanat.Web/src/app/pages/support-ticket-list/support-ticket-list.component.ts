import { Component, OnInit } from "@angular/core";
import { SupportTicketService } from "../../shared/generated/api/support-ticket.service";
import { Observable, tap } from "rxjs";
import { UtilityFunctionsService } from "../../shared/services/utility-functions.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { PageHeaderComponent } from "../../shared/components/page-header/page-header.component";
import { QanatGridComponent } from "../../shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe } from "@angular/common";
import { CreateSupportModalComponent } from "../../shared/components/support-ticket/modals/create-support-modal/create-support-modal.component";
import { SupportTicketStatusEnum } from "../../shared/generated/enum/support-ticket-status-enum";
import { SupportTicketGridDto } from "../../shared/generated/model/support-ticket-grid-dto";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { SupportTicketUpdateModalComponent } from "src/app/shared/components/support-ticket/modals/support-ticket-update-modal/support-ticket-update-modal.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { DialogService } from "@ngneat/dialog";
import { SupportTicketUpdateContext, SupportTicketUpdateStatusContext } from "../support-ticket-admin-pages/support-ticket-detail/support-ticket-detail.component";
import { SupportTicketUpdateStatusModalComponent } from "src/app/shared/components/support-ticket/modals/support-ticket-update-status-modal/support-ticket-update-status-modal.component";

@Component({
    selector: "support-ticket-list",
    imports: [PageHeaderComponent, QanatGridComponent, AsyncPipe, AlertDisplayComponent, IconComponent, LoadingDirective],
    templateUrl: "./support-ticket-list.component.html",
    styleUrl: "./support-ticket-list.component.scss",
})
export class SupportTicketListComponent implements OnInit {
    public activeTab: "Inbox" | "Active" | "On Hold" | "Closed" = "Inbox";
    public activeTabIcon = {
        "Inbox": "Inbox",
        "Active": "CircleCheckmark",
        "On Hold": "Info",
        "Closed": "CircleX",
    };

    public supportTicketDtos$: Observable<SupportTicketGridDto[]>;
    public rowData: SupportTicketGridDto[];
    public columnDefs: ColDef[];

    public inboxSupportTickets: SupportTicketGridDto[];
    public activeSupportTickets: SupportTicketGridDto[];
    public onHoldSupportTickets: SupportTicketGridDto[];
    public closedSupportTickets: SupportTicketGridDto[];

    public inboxColumnDefs: ColDef[] = [];
    public activeColumnDefs: ColDef[] = [];
    public onHoldColumnDefs: ColDef[] = [];
    public closedColumnDefs: ColDef[] = [];

    public gridApi: GridApi;
    public isLoading = true;

    constructor(
        private supportTicketService: SupportTicketService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.loadSupportTickets();
        this.createColumnDefs();
    }

    public changeActiveTab(tab: "Inbox" | "Active" | "On Hold" | "Closed") {
        this.activeTab = tab;

        switch (tab) {
            case "Inbox":
                this.rowData = this.inboxSupportTickets;
                this.columnDefs = this.inboxColumnDefs;
                break;
            case "Active":
                this.rowData = this.activeSupportTickets;
                this.columnDefs = this.activeColumnDefs;
                break;
            case "On Hold":
                this.rowData = this.onHoldSupportTickets;
                this.columnDefs = this.onHoldColumnDefs;
                break;
            case "Closed":
                this.rowData = this.closedSupportTickets;
                this.columnDefs = this.closedColumnDefs;
                break;
        }
    }

    private loadSupportTickets() {
        this.supportTicketDtos$ = this.supportTicketService.listAllSupportTicketsSupportTicket().pipe(
            tap((supportTickets) => {
                this.inboxSupportTickets = supportTickets
                    .filter((x) => x.SupportTicketStatus.SupportTicketStatusID == SupportTicketStatusEnum.Unassigned)
                    .sort((a, b) => this.utilityFunctionsService.dateSortComparator(a.DateCreated, b.DateCreated));

                this.activeSupportTickets = supportTickets
                    .filter((x) => x.SupportTicketStatus.SupportTicketStatusID == SupportTicketStatusEnum.Active)
                    .sort((a, b) => -1 * this.utilityFunctionsService.dateSortComparator(a.DateUpdated, b.DateUpdated));

                this.onHoldSupportTickets = supportTickets
                    .filter((x) => x.SupportTicketStatus.SupportTicketStatusID == SupportTicketStatusEnum.OnHold)
                    .sort((a, b) => -1 * this.utilityFunctionsService.dateSortComparator(a.DateUpdated, b.DateUpdated));

                this.closedSupportTickets = supportTickets
                    .filter((x) => x.SupportTicketStatus.SupportTicketStatusID == SupportTicketStatusEnum.Closed)
                    .sort((a, b) => -1 * this.utilityFunctionsService.dateSortComparator(a.DateClosed, b.DateClosed));

                this.changeActiveTab("Inbox");
                this.isLoading = false;
            })
        );
    }

    private createColumnDefs(): void {
        const baseColumnDefs: ColDef[] = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [];
                actions.push({ ActionName: "View Ticket", ActionIcon: "fas fa-map", ActionLink: `${params.data.SupportTicketID}` });
                if (params.data.SupportTicketStatus.SupportTicketStatusID != SupportTicketStatusEnum.Closed) {
                    actions.push({ ActionName: "Update Details", ActionIcon: "fas fa-edit", ActionHandler: () => this.openUpdateDetailsModal(params.data) });
                }
                actions.push({ ActionName: "Update Status", ActionIcon: "fas fa-edit", ActionHandler: () => this.openUpdateStatusModal(params.data) });

                return actions.length == 0 ? null : actions;
            }),
            { headerName: "Ticket ID", field: "SupportTicketID", cellRenderer: (params) => `<a href="/support-tickets/${params.value}" target="_blank">${params.value}</a>` },
            this.utilityFunctionsService.createBasicColumnDef("Geography", "GeographyName", { CustomDropdownFilterField: "GeographyName" }),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "ContactFirstName", {
                ValueFormatter: (params) => {
                    return params.data.ContactFirstName + " " + params.data.ContactLastName;
                },
            }),
            this.utilityFunctionsService.createBasicColumnDef("Priority", "SupportTicketPriority.SupportTicketPriorityDisplayName", {
                CustomDropdownFilterField: "SupportTicketPriority.SupportTicketPriorityDisplayName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Date Updated", "DateUpdated", "M/d/yyyy"),
            this.utilityFunctionsService.createDateColumnDef("Date Created", "DateCreated", "M/d/yyyy"),
            this.utilityFunctionsService.createBasicColumnDef("Question Type", "SupportTicketQuestionType.SupportTicketQuestionTypeDisplayName", {
                CustomDropdownFilterField: "SupportTicketQuestionType.SupportTicketQuestionTypeDisplayName",
            }),
        ];

        // inbox column defs
        this.inboxColumnDefs = baseColumnDefs.slice();
        this.inboxColumnDefs.splice(7, 0, this.utilityFunctionsService.createDaysPassedColumnDef("Days Open", "DateCreated"));
        this.inboxColumnDefs.splice(
            9,
            0,
            ...[
                this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumber", "WaterAccountID", {
                    FieldDefinitionType: "WaterAccount",
                    InRouterLink: "/water-accounts/",
                }),
                this.utilityFunctionsService.createBasicColumnDef("Contact Email", "ContactEmail"),
                this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone Number", "ContactPhoneNumber"),
                this.utilityFunctionsService.createBasicColumnDef("Created By", "CreateUserFullName", { CustomDropdownFilterField: "CreateUserFullName" }),
            ]
        );

        // active column defs
        this.activeColumnDefs = baseColumnDefs.slice();
        this.activeColumnDefs.splice(
            4,
            0,
            this.utilityFunctionsService.createBasicColumnDef("Assigned To", "AssignedUserFullName", { CustomDropdownFilterField: "AssignedUserFullName" })
        );
        this.activeColumnDefs.splice(
            6,
            0,
            ...[
                this.utilityFunctionsService.createDaysPassedColumnDef("Days Open", "DateCreated"),
                { headerName: "Most Recent Note", field: "MostRecentNoteMessage", width: 200, maxWidth: 200, suppressSizeToFit: true, suppressAutoSize: true },
            ]
        );
        this.activeColumnDefs.splice(
            11,
            0,
            ...[
                this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumber", "WaterAccountID", {
                    FieldDefinitionType: "WaterAccount",
                    InRouterLink: "/water-accounts/",
                }),
                this.utilityFunctionsService.createBasicColumnDef("Contact Email", "ContactEmail"),
                this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone Number", "ContactPhoneNumber"),
            ]
        );

        // on hold column defs
        this.onHoldColumnDefs = baseColumnDefs.slice();
        this.onHoldColumnDefs.splice(
            4,
            0,
            ...[this.utilityFunctionsService.createBasicColumnDef("Assigned To", "AssignedUserFullName", { CustomDropdownFilterField: "AssignedUserFullName" })]
        );
        this.onHoldColumnDefs.splice(
            8,
            0,
            ...[
                this.utilityFunctionsService.createDaysPassedColumnDef("Days Open", "DateCreated"),
                { headerName: "Most Recent Note", field: "MostRecentNoteMessage", width: 200, maxWidth: 200, suppressSizeToFit: true, suppressAutoSize: true },
                this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumber", "WaterAccountID", {
                    FieldDefinitionType: "WaterAccount",
                    InRouterLink: "/water-accounts/",
                }),
                this.utilityFunctionsService.createBasicColumnDef("Contact Email", "ContactEmail"),
                this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone Number", "ContactPhoneNumber"),
            ]
        );

        // closed column defs
        this.closedColumnDefs = baseColumnDefs.slice();
        this.closedColumnDefs.splice(
            4,
            0,
            this.utilityFunctionsService.createBasicColumnDef("Assigned To", "AssignedUserFullName", { CustomDropdownFilterField: "AssignedUserFullName" })
        );
        this.closedColumnDefs.splice(6, 0, this.utilityFunctionsService.createDateColumnDef("Date Closed", "DateClosed", "M/d/yyyy"));
        this.closedColumnDefs.splice(9, 0, {
            headerName: "Most Recent Note",
            field: "MostRecentNoteMessage",
            width: 200,
            maxWidth: 200,
            suppressSizeToFit: true,
            suppressAutoSize: true,
        });
        this.closedColumnDefs.splice(
            11,
            0,
            ...[
                this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumber", "WaterAccountID", {
                    FieldDefinitionType: "WaterAccount",
                    InRouterLink: "/water-accounts/",
                }),
                this.utilityFunctionsService.createBasicColumnDef("Contact Email", "ContactEmail"),
                this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone Number", "ContactPhoneNumber"),
            ]
        );
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public openCreateNewModal() {
        const dialogRef = this.dialogService.open(CreateSupportModalComponent, {
            data: {
                GeographyID: null,
                WaterAccountID: null,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadSupportTickets();
            }
        });
    }

    public openUpdateStatusModal(supportTicket: SupportTicketGridDto) {
        const dialogRef = this.dialogService.open(SupportTicketUpdateStatusModalComponent, {
            data: {
                SupportTicketID: supportTicket.SupportTicketID,
                GeographyID: supportTicket.GeographyID,
                SupportTicketStatusID: supportTicket.SupportTicketStatus.SupportTicketStatusID,
                SupportTicketPriorityID: supportTicket.SupportTicketPriority.SupportTicketPriorityID,
                AssignedUserID: supportTicket.AssignedUserID,
            } as SupportTicketUpdateStatusContext,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadSupportTickets();
            }
        });
    }

    private openUpdateDetailsModal(supportTicket: SupportTicketGridDto) {
        const dialogRef = this.dialogService.open(SupportTicketUpdateModalComponent, {
            data: {
                SupportTicketID: supportTicket.SupportTicketID,
                WaterAccountID: supportTicket.WaterAccountID,
                Description: supportTicket.Description,
                SupportTicketPriorityID: supportTicket.SupportTicketPriority.SupportTicketPriorityID,
                SupportTicketQuestionTypeID: supportTicket.SupportTicketQuestionType.SupportTicketQuestionTypeID,
                GeographyID: supportTicket.GeographyID,
                ContactFirstName: supportTicket.ContactFirstName,
                ContactLastName: supportTicket.ContactLastName,
                ContactEmail: supportTicket.ContactEmail,
                ContactPhoneNumber: supportTicket.ContactPhoneNumber,
                AssignedUserID: supportTicket.AssignedUserID,
            } as SupportTicketUpdateContext,
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadSupportTickets();
            }
        });
    }
}
