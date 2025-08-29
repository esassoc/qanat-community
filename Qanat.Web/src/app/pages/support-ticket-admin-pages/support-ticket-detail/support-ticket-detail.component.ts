import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, IsActiveMatchOptions, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, Observable, share, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { SupportTicketService } from "src/app/shared/generated/api/support-ticket.service";
import { SupportTicketGridDto } from "src/app/shared/generated/model/support-ticket-grid-dto";
import { PageHeaderComponent } from "../../../shared/components/page-header/page-header.component";
import { KeyValuePairListComponent } from "../../../shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "../../../shared/components/key-value-pair/key-value-pair.component";
import { PhonePipe } from "src/app/shared/pipes/phone.pipe";
import { ReactiveFormsModule } from "@angular/forms";
import { SupportTicketNoteFeedDto } from "src/app/shared/generated/model/support-ticket-note-feed-dto";
import { DomSanitizer } from "@angular/platform-browser";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { SupportTicketResponseModal } from "src/app/shared/components/support-ticket/modals/support-ticket-response-modal/support-ticket-response-modal.component";
import { SupportTicketUpdateModalComponent } from "src/app/shared/components/support-ticket/modals/support-ticket-update-modal/support-ticket-update-modal.component";
import { TimeElapsedPipe } from "src/app/shared/pipes/time-elapsed.pipe";
import { SupportTicketStatusEnum } from "src/app/shared/generated/enum/support-ticket-status-enum";
import { DialogService } from "@ngneat/dialog";
import { SupportTicketUpdateStatusModalComponent } from "src/app/shared/components/support-ticket/modals/support-ticket-update-status-modal/support-ticket-update-status-modal.component";
import { TrustHtmlPipe } from "src/app/shared/pipes/trust-html.pipe";

@Component({
    selector: "support-ticket-detail",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        DatePipe,
        RouterLink,
        PhonePipe,
        ReactiveFormsModule,
        IconComponent,
        DashboardMenuComponent,
        AlertDisplayComponent,
        TimeElapsedPipe,
        TrustHtmlPipe,
    ],
    templateUrl: "./support-ticket-detail.component.html",
    styleUrl: "./support-ticket-detail.component.scss",
})
export class SupportTicketDetailComponent {
    public supportTicket$: Observable<SupportTicketGridDto>;
    public refreshSupportTicket$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public supportTicketNotes$: Observable<SupportTicketNoteFeedDto[]>;

    public supportTicketID: number;
    public supportTicketClosed: boolean;
    public pageMenu: DashboardMenu;
    public dateFormatString: string = "MMM d, y, h:mm a";

    constructor(
        private supportTicketService: SupportTicketService,
        private route: ActivatedRoute,
        private dialogService: DialogService,
        public sanitizer: DomSanitizer,
        private router: Router
    ) {}

    ngOnInit() {
        this.supportTicketID = parseInt(this.route.snapshot.paramMap.get(routeParams.supportTicketID));
        this.buildMenu(this.supportTicketID);

        this.supportTicket$ = this.refreshSupportTicket$.pipe(
            switchMap((supportTicketID) => {
                if (!supportTicketID) return [];
                return this.supportTicketService
                    .getSupportTicketByIDSupportTicket(this.supportTicketID)
                    .pipe(tap((supportTicket) => (this.supportTicketClosed = supportTicket.SupportTicketStatus.SupportTicketStatusID == SupportTicketStatusEnum.Closed)));
            })
        );

        this.supportTicketNotes$ = this.refreshSupportTicket$.pipe(
            switchMap((supportTicketID) => {
                if (!supportTicketID) return [];
                return this.supportTicketService.getSupportTicketNotesBySupportTicketIDSupportTicket(supportTicketID).pipe(share());
            })
        );

        this.refreshSupportTicket$.next(this.supportTicketID);
    }

    private buildMenu(supportTicketID: number) {
        var menu = {
            menuItems: [
                {
                    title: `Support Request: #${supportTicketID}`,
                    icon: "Inbox",
                    routerLink: ["/support-tickets", supportTicketID],
                    isDropdown: true,
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    isExpanded: true,
                    preventCollapse: true,
                    menuItems: [
                        {
                            title: "Details",
                            routerLink: ["/support-tickets", supportTicketID],
                            fragment: "details",
                        },
                        {
                            title: "Responses & Notes",
                            routerLink: ["/support-tickets", supportTicketID],
                            fragment: "responses-and-notes",
                        },
                    ],
                },
                {
                    title: "Inbox",
                    icon: "Inbox",
                    routerLink: ["/support-tickets"],
                },
            ],
        } as DashboardMenu;

        menu.menuItems.forEach((menuItem) => {
            menuItem.menuItems?.forEach((childItem) => {
                const urltree = this.router.createUrlTree(childItem.routerLink as any[]);
                const childRouteIsActive = this.router.isActive(
                    urltree,
                    childItem.routerLinkActiveOptions
                        ? childItem.routerLinkActiveOptions
                        : ({ paths: "exact", queryParams: "ignored", matrixParams: "ignored" } as IsActiveMatchOptions)
                );
                if (childRouteIsActive) {
                    menuItem.isExpanded = true;
                }
            });
        });

        this.pageMenu = menu;
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
                this.refreshSupportTicket$.next(this.supportTicketID);
            }
        });
    }

    public openUpdateDetailsModal(supportTicket: SupportTicketGridDto) {
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
                this.refreshSupportTicket$.next(this.supportTicketID);
            }
        });
    }

    public openResponseModal(supportTicket: SupportTicketGridDto, isInternalNote: boolean) {
        const dialogRef = this.dialogService.open(SupportTicketResponseModal, {
            data: {
                SupportTicketID: supportTicket.SupportTicketID,
                IsInternalNote: isInternalNote,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshSupportTicket$.next(this.supportTicketID);
            }
        });
    }
}

export class SupportTicketContext {
    SupportTicketID: number;
}

export class SupportTicketUpdateContext extends SupportTicketContext {
    WaterAccountID: number;
    Description: string;
    GeographyID: number;
    ContactFirstName: string;
    ContactLastName: string;
    ContactEmail: string;
    ContactPhoneNumber: string;
    SupportTicketPriorityID: number;
    SupportTicketQuestionTypeID: number;
}

export class SupportTicketUpdateStatusContext extends SupportTicketContext {
    GeographyID: number;
    SupportTicketPriorityID: number;
    SupportTicketStatusID: number;
    AssignedUserID: number;
}

export class SupportTicketResponseContext extends SupportTicketContext {
    IsInternalNote: boolean;
}
