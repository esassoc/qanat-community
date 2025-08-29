import { Component, OnInit, HostListener, ChangeDetectorRef, OnDestroy } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { environment } from "src/environments/environment";
import { UserDto } from "../../generated/model/user-dto";
import { FlagEnum } from "../../generated/enum/flag-enum";
import { Observable, tap } from "rxjs";
import { UserService } from "../../generated/api/user.service";
import { UserGeographySummaryDto } from "../../generated/model/user-geography-summary-dto";
import { GeographyRoleEnum } from "../../generated/enum/geography-role-enum";
import { RoleEnum } from "../../generated/enum/role-enum";
import { WaterAccountSummaryDto } from "../../generated/model/water-account-summary-dto";
import { WithGeographyFlagDirective } from "../../directives/with-geography-flag.directive";
import { DropdownToggleDirective } from "../../directives/dropdown-toggle.directive";
import { WithFlagDirective } from "../../directives/with-flag.directive";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { DropdownToggleCloseDirective } from "../../directives/dropdown-toggle-close.directive";
import { AsyncPipe, DecimalPipe } from "@angular/common";
import { IconComponent } from "../icon/icon.component";
import { AuthorizationHelper } from "../../helpers/authorization-helper";
import { WithScenarioPlannerRolePermissionDirective } from "../../directives/with-scenario-planner-role-permission.directive";
import { RightsEnum } from "../../models/enums/rights.enum";
import { PermissionEnum } from "../../generated/enum/permission-enum";

@Component({
    selector: "header-nav",
    templateUrl: "./header-nav.component.html",
    styleUrls: ["./header-nav.component.scss"],
    imports: [
        DropdownToggleCloseDirective,
        RouterLink,
        RouterLinkActive,
        IconComponent,
        WithFlagDirective,
        DropdownToggleDirective,
        WithScenarioPlannerRolePermissionDirective,
        AsyncPipe,
        DecimalPipe,
    ],
})
export class HeaderNavComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    public RightsEnum = RightsEnum;
    public PermissionEnum = PermissionEnum;
    public FlagEnum = FlagEnum;
    currentUser: UserDto;

    public windowWidth: number;
    public geographySummaries$: Observable<UserGeographySummaryDto[]>;
    public numberOfWaterAccounts: number;
    public numberOfParcels: number;
    public numberOfWells: number;
    public showWaterDashboardDropdown: boolean = false;
    public waterAccounts: WaterAccountSummaryDto[];

    @HostListener("window:resize", ["$event"])
    resize() {
        this.windowWidth = window.innerWidth;
    }

    constructor(
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef,
        private userService: UserService
    ) {}

    ngOnInit() {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
            this.currentUser = currentUser;
            this.showWaterDashboardDropdown = !(
                (currentUser.GeographyUser.length > 0 && currentUser.GeographyUser.find((x) => x.GeographyRoleID == GeographyRoleEnum.WaterManager) != undefined) ||
                AuthorizationHelper.isSystemAdministrator(this.currentUser)
            );
        });

        this.geographySummaries$ = this.userService.getGeographySummaryUser().pipe(
            tap((geographySummaries) => {
                this.waterAccounts = geographySummaries.flatMap((x) => x.WaterAccounts).sort((x) => x.Area);
                this.numberOfWaterAccounts = geographySummaries.reduce((x, { WaterAccounts }) => x + WaterAccounts.length, 0);
                this.numberOfParcels = geographySummaries.reduce((x, { ParcelsCount }) => x + ParcelsCount, 0);
                this.numberOfWells = geographySummaries.reduce((x, { WellsCount }) => x + WellsCount, 0);
            })
        );
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();

        this.cdr.detach();
    }

    public isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    public getUserName() {
        return this.currentUser ? this.currentUser.FullName : null;
    }

    public login(): void {
        this.authenticationService.login();
    }

    public logout(): void {
        this.authenticationService.logout();

        setTimeout(() => {
            this.cdr.detectChanges();
        });
    }

    public showTestingWarning(): boolean {
        return environment.staging || environment.dev;
    }

    public testingWarningText(): string {
        return environment.staging ? "QA Environment" : "Development Environment";
    }

    public editProfile(): void {
        this.authenticationService.editProfile();
    }

    public isCurrentUserBeingImpersonated(): boolean {
        return this.authenticationService.isCurrentUserBeingImpersonated(this.currentUser);
    }

    public hasManageMenu(): boolean {
        const hasMenu =
            this.authenticationService.hasFlag(this.currentUser, FlagEnum.HasManagerDashboard) ||
            this.authenticationService.hasGeographyFlag(this.currentUser, FlagEnum.HasManagerDashboard);
        return hasMenu;
    }
}
