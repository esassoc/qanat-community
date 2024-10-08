import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterOutlet } from "@angular/router";
import { Observable, share, startWith, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyFlagCheck } from "src/app/shared/directives/with-geography-flag.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { NgIf, AsyncPipe } from "@angular/common";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";

@Component({
    selector: "geography-menu-layout",
    templateUrl: "./geography-menu-layout.component.html",
    styleUrls: ["./geography-menu-layout.component.scss"],
    standalone: true,
    imports: [NgIf, LoadingDirective, GeographyLogoComponent, DashboardMenuComponent, RouterOutlet, AsyncPipe],
})
export class GeographyMenuLayoutComponent implements OnInit {
    public geography$: Observable<GeographyDto>;
    public FlagEnum = FlagEnum;
    public isLoading: boolean = true;
    public geographyDashboardMenu: DashboardMenu;
    public currentUser: UserDto;
    public withGeographyFlag: GeographyFlagCheck;

    constructor(
        private geographyService: GeographyService,
        private route: ActivatedRoute,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.authenticationService.getCurrentUser().pipe(
            startWith(null),
            tap((x) => {
                this.currentUser = x;
            }),
            switchMap((x) => this.geographyService.publicGeographyNameGeographyNameGet(geographyName)),
            tap((x) => {
                this.isLoading = false;
                this.geographyDashboardMenu = this.buildGeographyMenu(x);
            }),
            share()
        );
    }

    buildGeographyMenu(geography: GeographyDto): DashboardMenu {
        if (geography == null) return null;
        const geographySlug = geography.GeographyName.toLowerCase();
        const menu = {
            menuItems: [
                {
                    title: geography.GeographyDisplayName,
                    icon: "Geography",
                    routerLink: ["/geographies", geographySlug],
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    isDropdown: true,
                    preventCollapse: true,
                    isExpanded: true,
                    menuItems: [
                        {
                            title: "Overview",
                            routerLink: ["/geographies", geographySlug, "overview"],
                        },
                        {
                            title: "Allocation Plans",
                            routerLink: ["/geographies", geographySlug, "allocation-plans"],
                            routerLinkActiveOptions: {
                                matrixParams: "ignored",
                                queryParams: "ignored",
                                fragment: "exact",
                                paths: "subset",
                            },
                            isDisabled: !geography.AllocationPlansVisibleToLandowners || (!this.currentUser && !geography.AllocationPlansVisibleToPublic),
                        },
                        {
                            title: "Groundwater Levels",
                            routerLink: ["/geographies", geographySlug, "groundwater-levels"],
                            withGeographyFlag: {
                                currentUser: this.currentUser,
                                flag: FlagEnum.HasManagerDashboard,
                                geographyID: geography.GeographyID,
                            },
                        },
                        {
                            title: " Accountholder Sign-up",
                            routerLink: ["/", geographySlug],
                            routerLinkActiveOptions: {
                                matrixParams: "ignored",
                                queryParams: "ignored",
                                fragment: "exact",
                                paths: "subset",
                            },
                            isDisabled: !geography.LandingPageEnabled || (!this.currentUser && !geography.LandingPageEnabled),
                        },
                        {
                            title: "Support & Contact",
                            routerLink: ["/geographies", geographySlug, "support"],
                        },
                        {
                            title: "Back to All Geographies",
                            icon: "ArrowLeft",
                            routerLink: ["/geographies"],
                            cssClasses: "border-top",
                        },
                    ],
                },
            ],
        } as DashboardMenu;

        return menu;
    }
}
