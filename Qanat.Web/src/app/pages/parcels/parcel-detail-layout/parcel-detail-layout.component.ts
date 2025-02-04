import { Component, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute, IsActiveMatchOptions, RouterLink, RouterOutlet } from "@angular/router";
import { AgGridAngular } from "ag-grid-angular";
import { Observable, combineLatest, map, of, switchMap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { IconComponent } from "../../../shared/components/icon/icon.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FormsModule } from "@angular/forms";
import { SearchParcelsComponent } from "../../../shared/components/search-parcels/search-parcels.component";

import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { GeographyAllocationPlanConfigurationDto } from "src/app/shared/generated/model/geography-allocation-plan-configuration-dto";

@Component({
    selector: "parcel-detail-layout",
    standalone: true,
    imports: [
        SearchParcelsComponent,
        NgIf,
        RouterLink,
        GeographyLogoComponent,
        IconComponent,
        FormsModule,
        DashboardMenuComponent,
        RouterOutlet,
        LoadingDirective,
        AsyncPipe,
        DropdownToggleDirective,
    ],

    templateUrl: "./parcel-detail-layout.component.html",
    styleUrl: "./parcel-detail-layout.component.scss",
})
export class ParcelDetailLayoutComponent implements OnInit {
    @ViewChild("parcelSupplyGrid") parcelSupplyGrid: AgGridAngular;
    public currentParcel$: Observable<ParcelMinimalDto>;
    public pageMenu$: Observable<DashboardMenu>;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private parcelService: ParcelService,
        private allocationPlanService: AllocationPlanService
    ) {}

    ngOnInit(): void {
        this.currentParcel$ = this.route.params.pipe(
            switchMap((params) => {
                return this.parcelService.parcelsParcelIDGet(params[routeParams.parcelID]);
            })
        );

        this.pageMenu$ = this.currentParcel$.pipe(
            switchMap((parcel) => {
                return combineLatest({
                    parcel: of(parcel),
                    currentUser: this.authenticationService.getCurrentUser(),
                    geographyConfig: this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationGet(parcel.GeographyID),
                });
            }),
            map((value) => {
                return this.buildMenu(value.parcel, value.geographyConfig, value.currentUser);
            })
        );
    }

    buildMenu(parcel: ParcelMinimalDto, geographyAllocationConfig: GeographyAllocationPlanConfigurationDto, currentUser: UserDto): DashboardMenu {
        const parcelID = parcel.ParcelID;
        const menu = {
            menuItems: [
                {
                    title: parcel.ParcelNumber,
                    icon: "Parcels",
                    routerLink: ["/water-dashboard", "parcels", parcelID],
                    isDropdown: true,
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    hidden: true,
                    menuItems: [
                        {
                            title: "Parcel Details",
                            routerLink: ["/parcels", parcelID, "detail"],
                        },
                        {
                            title: "Water Measurements",
                            routerLink: ["/parcels", parcelID, "detail"],
                            fragment: "water-measurements",
                        },
                        {
                            title: "Allocation Plan",
                            routerLink: ["/parcels", parcelID, "detail"],
                            fragment: "allocation-plan",
                            isDisabled: !geographyAllocationConfig.IsVisibleToLandowners,
                        },
                        {
                            title: "Supply Activity",
                            routerLink: ["/parcels", parcelID, "detail"],
                            fragment: "ledger-activity",
                        },
                        {
                            title: "Admin Panel",
                            routerLink: ["/parcels", parcelID, "admin-panel"],
                            isDisabled: !AuthorizationHelper.isSystemAdministratorOrGeographyManager(currentUser, parcel.GeographyID),
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
                //     queryParams: { GeographyID: parcel.GeographyID.toString() },
                // },
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

    changedParcel(parcel: ParcelMinimalDto, currentParcel: ParcelMinimalDto): void {
        if (parcel && parcel.ParcelID != undefined && parcel.ParcelID != currentParcel.ParcelID) {
            const url = this.router.url.split("#")[0].split("/");
            url[2] = parcel.ParcelID.toString();
            this.router.navigate(url, { preserveFragment: true, replaceUrl: true });
        }
    }
}
