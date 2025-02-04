import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, IsActiveMatchOptions, Router, RouterLink, RouterOutlet } from "@angular/router";
import { combineLatest, Observable, of } from "rxjs";
import { map, switchMap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { UserDto, WaterAccountDto, WaterAccountSearchResultDto } from "src/app/shared/generated/model/models";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { NgIf, AsyncPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { GeographyLogoComponent } from "src/app/shared/components/geography-logo/geography-logo.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { SearchWaterAccountsComponent } from "../../../shared/components/search-water-accounts/search-water-accounts.component";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";

@Component({
    selector: "water-account-detail-layout",
    templateUrl: "./water-account-detail-layout.component.html",
    styleUrls: ["./water-account-detail-layout.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        RouterLink,
        GeographyLogoComponent,
        IconComponent,
        SelectDropDownModule,
        DashboardMenuComponent,
        RouterOutlet,
        LoadingDirective,
        AsyncPipe,
        SearchWaterAccountsComponent,
        DropdownToggleDirective,
    ],
})
export class WaterAccountDetailLayoutComponent implements OnInit {
    public currentWaterAccount$: Observable<WaterAccountDto>;
    public pageMenu$: Observable<DashboardMenu>;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private geographyService: GeographyService,
        private waterAccountsService: WaterAccountService
    ) {}

    ngOnInit(): void {
        this.currentWaterAccount$ = this.route.params.pipe(
            switchMap((params) => {
                return this.waterAccountsService.waterAccountsWaterAccountIDGet(params[routeParams.waterAccountID]);
            })
        );

        this.pageMenu$ = this.currentWaterAccount$.pipe(
            switchMap((waterAccount) => {
                return combineLatest({
                    parcel: of(waterAccount),
                    currentUser: this.authenticationService.getCurrentUser(),
                    geography: this.geographyService.geographiesGeographyIDGet(waterAccount.Geography.GeographyID),
                });
            }),
            map((value) => {
                return this.buildMenu(value.parcel, value.geography, value.currentUser);
            })
        );
    }
    changedWaterAccount(waterAccount: WaterAccountSearchResultDto, currentWaterAccount: WaterAccountDto): void {
        if (waterAccount && waterAccount.WaterAccountID !== undefined && waterAccount.WaterAccountID !== currentWaterAccount.WaterAccountID) {
            // Preserve the current route structure dynamically
            const currentRoute = this.router.url.split("?")[0]; // Extract the current route path without query params
            const updatedRoute = currentRoute.split("/").map(
                (segment, index) => (index === 2 ? waterAccount.WaterAccountID.toString() : segment) // Replace only the water account ID segment
            );

            this.router.navigate(updatedRoute, {
                queryParams: this.route.snapshot.queryParams, // Preserve the current query parameters
                replaceUrl: true, // Replace the current history entry
            });
        }
    }

    buildMenu(waterAccount: WaterAccountDto, geography: GeographyDto, currentUser: UserDto): DashboardMenu {
        const waterAccountID = waterAccount.WaterAccountID;
        const isSystemAdministratorOrGeographyManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(currentUser, geography.GeographyID);
        const menu = {
            menuItems: [
                {
                    title: `# ${waterAccount.WaterAccountNumber}`,
                    icon: "WaterAccounts",
                    routerLink: ["/water-accounts", waterAccountID],
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    isDropdown: true,
                    hidden: true,
                    menuItems: [
                        {
                            title: "Water Budget",
                            routerLink: ["/water-accounts", waterAccountID, "water-budget"],
                        },
                        {
                            title: "Parcels",
                            routerLink: ["/water-accounts", waterAccountID, "parcels"],
                        },
                        {
                            title: "Wells",
                            routerLink: ["/water-accounts", waterAccountID, "wells"],
                        },
                        {
                            title: "Account Activity",
                            routerLink: ["/water-accounts", waterAccountID, "activity"],
                        },
                        {
                            title: "Allocation Plans",
                            routerLink: ["/water-accounts", waterAccountID, "allocation-plans"],
                            routerLinkActiveOptions: {
                                matrixParams: "ignored",
                                queryParams: "ignored",
                                fragment: "exact",
                                paths: "subset",
                            },
                            isDisabled: !geography.AllocationPlansVisibleToLandowners,
                        },
                        {
                            title: "Users & Settings",
                            routerLink: ["/water-accounts", waterAccountID, "users-and-settings"],
                        },
                        {
                            title: "Admin Panel",
                            routerLink: ["/water-accounts", waterAccountID, "admin-panel"],
                            isDisabled: !isSystemAdministratorOrGeographyManager,
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

                // {
                //     title: "Support & Contact",
                //     icon: "Question",
                //     routerLink: ["/request-support"],
                //     queryParams: { GeographyName: this.currentWaterAccount.Geography.GeographyName },
                // },
            ],
        } as DashboardMenu;

        if (geography.FeeCalculatorEnabled) {
            let indexOfWaterBudgetMenuItem = menu.menuItems[0].menuItems.findIndex((x) => x.title == "Water Budget");
            let feeCalculatorMenuItem = {
                title: "Fee Calculator",
                routerLink: ["/fee-calculator", geography.GeographyName.toLowerCase()],
                queryParams: { selectedWaterAccountID: waterAccount.WaterAccountID },
            } as any;

            menu.menuItems[0].menuItems.splice(indexOfWaterBudgetMenuItem + 1, 0, feeCalculatorMenuItem);
        }

        //MK 12/18/2024 -- Currently holders have the create permission and viewers do not. Might want to change this to an explicit flag in the future.
        const isWaterAccountHolder = AuthorizationHelper.hasWaterAccountRolePermission(
            geography.GeographyID,
            waterAccount.WaterAccountID,
            PermissionEnum.WaterAccountRights,
            RightsEnum.Create,
            currentUser
        );

        const canViewSelfReportMenuItem = isSystemAdministratorOrGeographyManager || isWaterAccountHolder;

        if (geography.AllowWaterMeasurementSelfReporting && canViewSelfReportMenuItem) {
            let indexOfAccountActivity = menu.menuItems[0].menuItems.findIndex((x) => x.title == "Account Activity");
            let selfReportMenuItem = {
                title: "Self-Report",
                routerLink: ["/water-accounts", waterAccountID, "water-measurement-self-reports"],
            } as any;

            menu.menuItems[0].menuItems.splice(indexOfAccountActivity + 1, 0, selfReportMenuItem);
        }

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
        return menu;
    }
}
