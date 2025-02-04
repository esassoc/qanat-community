import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, IsActiveMatchOptions, Router, RouterLink, RouterOutlet } from "@angular/router";
import { Observable, of } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { filter, share, switchMap, tap } from "rxjs/operators";
import { OpenETConfigurationService } from "src/app/shared/generated/api/open-et-configuration.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GeographySwitcherComponent } from "../../../shared/components/geography-switcher/geography-switcher.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";

@Component({
    selector: "qanat-supply-and-usage-menu-layout",
    templateUrl: "./supply-and-usage-menu-layout.component.html",
    styleUrls: ["./supply-and-usage-menu-layout.component.scss"],
    standalone: true,
    imports: [NgIf, RouterLink, GeographyLogoComponent, IconComponent, GeographySwitcherComponent, RouterOutlet, PageHeaderComponent, AsyncPipe, DashboardMenuComponent],
})
export class SupplyAndUsageMenuLayoutComponent implements OnInit {
    public isOpenETActive$: Observable<boolean>;
    public supplyAndUsageMenu: DashboardMenu;

    public geography$: Observable<GeographyMinimalDto>;
    public supplyAndUsageMenu$: Observable<DashboardMenu>;

    public routerLinkActiveOptions = {
        exact: false,
    };

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                if (geographyName) {
                    return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName).pipe(
                        tap((geography) => {
                            this.currentGeographyService.setCurrentGeography(geography);
                            this.buildMenu(geography);
                        })
                    );
                } else {
                    return this.currentGeographyService.getCurrentGeography().pipe(
                        tap((geography) => {
                            if (geography) {
                                this.buildMenu(geography);
                                this.redirectToGeography(geography.GeographyName);
                            }
                        })
                    );
                }
            })
        );

        this.supplyAndUsageMenu$ = this.geography$.pipe(
            switchMap((geography) => {
                if (geography) {
                    this.buildMenu(geography);
                }
                return of(this.supplyAndUsageMenu);
            })
        );
    }

    redirectToGeography(geographyName: string) {
        this.router.navigate(["/supply-and-usage", geographyName.toLowerCase(), "activity-center"]);
    }

    buildMenu(geography: GeographyMinimalDto) {
        const geographySlug = geography.GeographyName.toLowerCase();
        const menu = {
            menuItems: [
                {
                    title: "Activity Center",
                    icon: "ActivityCenter",
                    routerLink: ["/supply-and-usage", geographySlug, "activity-center"],
                    isDropdown: false,
                },
                {
                    title: "Water Account Budgets",
                    icon: "Budget",
                    routerLink: ["/supply-and-usage", geographySlug, "water-account-budgets-report"],
                },
                {
                    title: "Statistics",
                    icon: "Statistics",
                    routerLink: ["/supply-and-usage", geographySlug, "statistics"],
                },
                {
                    title: "Water Measurements",
                    icon: "Measurements",
                    routerLink: ["/supply-and-usage", geographySlug, "water-measurements"],
                },
                {
                    title: "Water Supply",
                    icon: "Transactions",
                    routerLink: ["/supply-and-usage", geographySlug, "water-supply"],
                },
                {
                    title: "Allocation Plans",
                    icon: "Allocations",
                    routerLink: ["/supply-and-usage", geographySlug, "parcels", "allocation-plans"],
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    isDisabled: !geography.AllocationPlansEnabled,
                },
                {
                    title: "Zones",
                    icon: "Zones",
                    routerLink: ["/supply-and-usage", geographySlug, "zones"],
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

        this.supplyAndUsageMenu = menu;
    }
}
