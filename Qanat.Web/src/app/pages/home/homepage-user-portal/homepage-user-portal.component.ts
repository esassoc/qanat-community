import { Component, OnInit } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { Observable, tap } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { UserGeographySummaryDto } from "src/app/shared/generated/model/user-geography-summary-dto";
import { GeographySelectorComponent } from "../../../shared/components/geography-selector/geography-selector.component";
import { DropdownToggleDirective } from "../../../shared/directives/dropdown-toggle.directive";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { LargeGeographyCardComponent } from "../../../shared/components/large-geography-card/large-geography-card.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { RichLinkComponent } from "src/app/shared/components/rich-link/rich-link.component";

@Component({
    selector: "homepage-user-portal",
    templateUrl: "./homepage-user-portal.component.html",
    styleUrls: ["./homepage-user-portal.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        RichLinkComponent,
        RouterLink,
        LoadingDirective,
        AlertDisplayComponent,
        NgFor,
        LargeGeographyCardComponent,
        CustomRichTextComponent,
        DropdownToggleDirective,
        GeographySelectorComponent,
        AsyncPipe,
    ],
})
export class HomepageUserPortalComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public geographySummaries$: Observable<UserGeographySummaryDto[]>;

    public updateProfileRichTextTypeID = CustomRichTextTypeEnum.HomepageUpdateProfileLink;
    public growerGuideRichTextTypeID = CustomRichTextTypeEnum.HomepageGrowerGuideLink;
    public geographiesRichTextTypeID = CustomRichTextTypeEnum.HomepageGeographiesLink;
    public claimWaterAccountsRichTextTypeID = CustomRichTextTypeEnum.HomepageClaimWaterAccountsPanel;

    public noWaterAccountsText: string;
    public isLoading: boolean = true;

    constructor(
        private authenticationService: AuthenticationService,
        private userService: UserService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(tap(() => (this.noWaterAccountsText = this.getNoWaterAccountsText())));

        this.geographySummaries$ = this.userService.userGeographySummaryGet().pipe(
            tap((x) => {
                this.isLoading = false;
            })
        );
    }

    private getNoWaterAccountsText(): string {
        if (this.authenticationService.currentUserHasFlag(FlagEnum.HasAdminDashboard)) {
            return "As a Platform Admin, you have access to all Water Accounts within the platform.";
        } else if (this.authenticationService.currentUserHasFlag(FlagEnum.HasManagerDashboard)) {
            return "As a Water Manager, you have access to all Water Accounts within your managed geographies.";
        } else {
            return "You do not currently have Water Accounts in any geographies.";
        }
    }

    public logout() {
        this.authenticationService.logout();
    }

    public onGeographySelected(geographyName: string) {
        this.router.navigateByUrl(`/${geographyName.toLowerCase()}/claim-water-accounts`);
    }
}
