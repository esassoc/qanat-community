import { Component } from "@angular/core";
import { ActivatedRoute, IsActiveMatchOptions, Router } from "@angular/router";
import { BehaviorSubject, combineLatest, Observable, switchMap, tap } from "rxjs";
import { WaterAccountContactDto } from "src/app/shared/generated/model/water-account-contact-dto";
import { WaterAccountContactService } from "src/app/shared/generated/api/water-account-contact.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { routeParams } from "src/app/app.routes";
import { AsyncPipe } from "@angular/common";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { GeographyLogoComponent } from "src/app/shared/components/geography-logo/geography-logo.component";
import { WaterAccountContactUpdateComponent } from "src/app/shared/components/water-account-contact/modals/water-account-contact-update/water-account-contact-update.component";
import { DialogService } from "@ngneat/dialog";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ColDef } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountWaterAccountContactUpdateDto } from "src/app/shared/generated/model/water-account-water-account-contact-update-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { BasicJsonModalComponent, BasicJsonModalContext } from "src/app/shared/components/basic-json-modal/basic-json-modal.component";
import { PhonePipe } from "src/app/shared/pipes/phone.pipe";

@Component({
    selector: "water-account-contact-detail",
    standalone: true,
    imports: [
        PageHeaderComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        AlertDisplayComponent,
        AsyncPipe,
        DashboardMenuComponent,
        GeographyLogoComponent,
        IconComponent,
        QanatGridComponent,
        PhonePipe,
    ],
    templateUrl: "./water-account-contact-detail.component.html",
    styleUrl: "./water-account-contact-detail.component.scss",
})
export class WaterAccountContactDetailComponent {
    public waterAccountContact$: Observable<WaterAccountContactDto>;
    public waterAccountContact: WaterAccountContactDto;
    public refreshWaterAccountContact$: BehaviorSubject<number> = new BehaviorSubject(null);

    public columnDefs: ColDef[];

    public pageMenu: DashboardMenu;
    public isLoading = true;

    public customRichTextTypeID: number = CustomRichTextTypeEnum.WaterAccountContactUpdateModal;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private waterAccountContactService: WaterAccountContactService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private utilityFunctionsService: UtilityFunctionsService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnInit() {
        this.waterAccountContact$ = combineLatest([this.route.params, this.refreshWaterAccountContact$]).pipe(
            switchMap(([params, refresh]) => {
                const waterAccountContactID = params[routeParams.waterAccountContactID];
                return this.waterAccountContactService.getByIDWaterAccountContact(waterAccountContactID);
            }),
            tap((waterAccountContact) => {
                this.waterAccountContact = waterAccountContact;
                this.buildMenu(waterAccountContact);
                this.isLoading = false;
            })
        );

        this.buildColumnDefs();
    }

    public openUpdateModal(waterAccountContact: WaterAccountContactDto) {
        const dialogRef = this.dialogService.open(WaterAccountContactUpdateComponent, {
            data: {
                WaterAccountContactID: waterAccountContact.WaterAccountContactID,
                GeographyID: waterAccountContact.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.waterAccountContact = result;
            }
        });
    }

    public openDeleteModal(waterAccountContact: WaterAccountContactDto) {
        this.confirmService
            .confirm({
                title: "Delete Water Account Contact",
                message: `Are you sure you want to delete <b>${waterAccountContact.ContactName}</b>?`,
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.waterAccountContactService.deleteWaterAccountContact(waterAccountContact.WaterAccountContactID).subscribe(() => {
                        this.router.navigate(["/water-dashboard/contacts"]).then(() => {
                            this.alertService.pushAlert(new Alert("Water account contact successfully deleted.", AlertContext.Success));
                        });
                    });
                }
            });
    }

    public openRemoveFromWaterAccountModal(waterAccountID: number, waterAccountNumber: number) {
        this.confirmService
            .confirm({
                title: "Remove Contact from Water Account",
                message: `Are you sure you want to remove contact <b>${this.waterAccountContact.ContactName}</b> from water account <b>${waterAccountNumber}</b>?`,
                buttonTextYes: "Remove",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    const requestDto = new WaterAccountWaterAccountContactUpdateDto();
                    requestDto.WaterAccountContactID = null;

                    this.waterAccountService.updateWaterAccountContactWaterAccount(waterAccountID, requestDto).subscribe({
                        next: () => {
                            // const indexToRemove = this.waterAccountContact.WaterAccounts.findIndex((x) => x.WaterAccountID == waterAccountID);
                            // this.waterAccountContact.WaterAccounts.splice(indexToRemove, 1);
                            this.refreshWaterAccountContact$.next(null);

                            this.alertService.pushAlert(new Alert("Water account contact successfully removed from water account.", AlertContext.Success));
                        },
                    });
                }
            });
    }

    public openAddressValidationJsonModal() {
        const dialogRef = this.dialogService.open(BasicJsonModalComponent, {
            data: {
                title: "Address Validation JSON",
                json: this.waterAccountContact.AddressValidationJson,
            } as BasicJsonModalContext,
            size: "lg",
        });
    }

    private buildColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Remove",
                        ActionIcon: "fa fa-times-circle text-danger",
                        ActionHandler: () => this.openRemoveFromWaterAccountModal(params.data.WaterAccountID, params.data.WaterAccountNumber),
                    },
                ];
            }),
            this.utilityFunctionsService.createLinkColumnDef("Account Number", "WaterAccountNumber", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccountID}/users-and-settings`, LinkDisplay: params.data.WaterAccountNumber };
                },
            }),
            { headerName: "Account Name", field: "WaterAccountName" },
            { headerName: "Water Account PIN", field: "WaterAccountPIN" },
            { headerName: "Notes", field: "Notes" },
        ];
    }

    private buildMenu(waterAccountContact: WaterAccountContactDto) {
        var menu = {
            menuItems: [
                {
                    title: waterAccountContact.ContactName,
                    icon: "Contact",
                    routerLink: ["/contacts", waterAccountContact.WaterAccountContactID],
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
                            title: "Contact Details",
                            routerLink: ["/contacts", waterAccountContact.WaterAccountContactID],
                            fragment: "details",
                        },
                        {
                            title: "Water Accounts",
                            routerLink: ["/contacts", waterAccountContact.WaterAccountContactID],
                            fragment: "water-accounts",
                        },
                        {
                            title: "Back to All Contacts",
                            icon: "ArrowLeft",
                            routerLink: ["/water-dashboard/contacts"],
                            cssClasses: "border-top",
                        },
                    ],
                },
                {
                    title: "Water Accounts",
                    icon: "WaterAccounts",
                    routerLink: ["/water-dashboard/water-accounts"],
                },
                {
                    title: "Parcels",
                    icon: "Parcels",
                    routerLink: ["/water-dashboard/parcels"],
                },
                {
                    title: "Wells",
                    icon: "Wells",
                    routerLink: ["/water-dashboard/wells"],
                },
                {
                    title: "Water Account Contacts",
                    icon: "Contact",
                    routerLink: ["/water-dashboard/contacts"],
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
}
