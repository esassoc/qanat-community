import { ChangeDetectorRef, Component, ViewChild } from "@angular/core";
import { Router, ActivatedRoute, IsActiveMatchOptions, RouterLink, RouterOutlet } from "@angular/router";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef } from "ag-grid-community";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { CustomDropdownFilterComponent } from "src/app/shared/components/custom-dropdown-filter/custom-dropdown-filter.component";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { UsageEntityService } from "src/app/shared/generated/api/usage-entity.service";
import { GeographyRoleEnum } from "src/app/shared/generated/enum/geography-role-enum";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { AllocationPlanManageDto } from "src/app/shared/generated/model/allocation-plan-manage-dto";
import { ExternalMapLayerDto } from "src/app/shared/generated/model/external-map-layer-dto";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { ParcelDetailDto } from "src/app/shared/generated/model/parcel-detail-dto";
import { ParcelSupplyDetailDto } from "src/app/shared/generated/model/parcel-supply-detail-dto";
import { UsageEntityListItemDto } from "src/app/shared/generated/model/usage-entity-list-item-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { IconComponent } from "../../../shared/components/icon/icon.component";
import { PageHeaderComponent } from "../../../shared/components/page-header/page-header.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { WaterAccountTitleComponent } from "src/app/shared/components/water-account/water-account-title/water-account-title.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";

@Component({
    selector: "parcel-detail-layout",
    standalone: true,
    imports: [
        NgIf,
        RouterLink,
        GeographyLogoComponent,
        IconComponent,
        DashboardMenuComponent,
        PageHeaderComponent,
        WaterAccountTitleComponent,
        LoadingDirective,
        AsyncPipe,
        RouterOutlet,
    ],
    templateUrl: "./parcel-detail-layout.component.html",
    styleUrl: "./parcel-detail-layout.component.scss",
})
export class ParcelDetailLayoutComponent {
    @ViewChild("parcelSupplyGrid") parcelSupplyGrid: AgGridAngular;
    public currentUser: UserDto;
    public currentUser$: Observable<UserDto>;
    public parcel$: Observable<ParcelDetailDto>;
    public userHasOneGeography = false;
    public geographyID: number;

    public allocationPlans$: Observable<AllocationPlanManageDto[]>;
    public showAllocationPlan: boolean;
    public dashboardMenu: DashboardMenu;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private parcelService: ParcelService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                this.currentUser = currentUser;
                const parcelID = parseInt(this.route.snapshot.paramMap.get(routeParams.parcelID));

                this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(parcelID).pipe(
                    tap((parcel) => {
                        this.geographyID = parcel.GeographyID;
                        this.allocationPlans$ = this.parcelService.geographiesGeographyIDParcelsParcelIDAllocationPlansGet(parcel.GeographyID, parcel.ParcelID).pipe(
                            tap((x) => {
                                this.showAllocationPlan = x[0]?.GeographyAllocationPlanConfiguration.IsVisibleToLandowners ?? false;
                                this.dashboardMenu = this.buildMenu(parcel);
                            })
                        );
                    })
                );
            })
        );
    }

    buildMenu(parcel: ParcelDetailDto): DashboardMenu {
        const parcelID = parcel.ParcelID;
        const menu = {
            menuItems: [
                {
                    title: parcel.ParcelNumber,
                    icon: "Parcels",
                    routerLink: ["/water-dashboard", "parcels", parcelID],
                    isDropdown: true,
                    menuItems: [
                        {
                            title: "Parcel Details",
                            routerLink: ["/water-dashboard", "parcels", parcelID, "detail"],
                        },
                        {
                            title: "Water Measurements",
                            routerLink: ["/water-dashboard", "parcels", parcelID, "detail"],
                            fragment: "water-measurements",
                        },
                        {
                            title: "Allocation Plan",
                            routerLink: ["/water-dashboard", "parcels", parcelID, "detail"],
                            fragment: "allocation-plan",
                            isDisabled: !this.showAllocationPlan,
                        },
                        {
                            title: "Supply Activity",
                            routerLink: ["/water-dashboard", "parcels", parcelID, "detail"],
                            fragment: "ledger-activity",
                        },
                        {
                            title: "Admin Panel",
                            routerLink: ["/water-dashboard", "parcels", parcelID, "admin-panel"],
                            isDisabled: !(
                                this.authenticationService.isUserAnAdministrator(this.currentUser) ||
                                this.authenticationService.hasGeographyFlagForGeographyID(this.currentUser, FlagEnum.HasManagerDashboard, this.geographyID)
                            ),
                        },
                        {
                            title: "Back to All Parcels",
                            icon: "ArrowLeft",
                            routerLink: ["/water-dashboard/parcels"],
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
                    title: "Support & Contact",
                    icon: "Question",
                    routerLink: ["/geographies", parcel.GeographyName.toLowerCase(), "support"], //TODO: fix geo slug
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

        return menu;
    }

    redirectToAccount(parcelID: number) {
        if (parcelID) {
            this.router.navigateByUrl(`/water-dashboard/${parcelID}`, {});
        } else {
            this.router.navigateByUrl(`/water-dashboard`);
        }
    }
}
