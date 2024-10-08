import { Component } from "@angular/core";
import { Observable, map, of, switchMap, tap } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyLandingPageDto } from "src/app/shared/generated/model/geography-landing-page-dto";
import { GeographyDto, UserDto } from "src/app/shared/generated/model/models";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";
import { RouterLink } from "@angular/router";
import { CustomRichTextComponent } from "../../shared/components/custom-rich-text/custom-rich-text.component";
import { NgIf, NgClass, AsyncPipe } from "@angular/common";
import { GeographyLandingPageHeaderComponent } from "src/app/shared/components/geography-landing-page-header/geography-landing-page-header.component";
import { GeographyPromoCardComponent } from "src/app/shared/components/geography-promo-card/geography-promo-card.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { GeographyWidePromoCardComponent } from "src/app/shared/components/geography-wide-promo-card/geography-wide-promo-card.component";
import { RichLinkComponent } from "src/app/shared/components/rich-link/rich-link.component";

@Component({
    selector: "geography-landing-page",
    templateUrl: "./geography-landing-page.component.html",
    styleUrls: ["./geography-landing-page.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        GeographyLandingPageHeaderComponent,
        CustomRichTextComponent,
        GeographyPromoCardComponent,
        NgClass,
        IconComponent,
        RouterLink,
        GeographyWidePromoCardComponent,
        RichLinkComponent,
        AsyncPipe,
    ],
})
export class GeographyLandingPageComponent {
    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;
    geography$: Observable<GeographyDto>;
    public landingPageDto$: Observable<GeographyLandingPageDto>;
    currentUser: UserDto = null;
    public hasUserCompletedSetUp: boolean;
    public geography: GeographyDto;

    constructor(
        private geographyRouteService: GeographyRouteService,
        private authenticationService: AuthenticationService,
        private userService: UserService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.geographyRouteService.geography$.pipe(map((geography) => (this.geography = geography)));

        this.landingPageDto$ = of(this.authenticationService.isAuthenticated()).pipe(
            switchMap((authenticated) => {
                if (authenticated) {
                    return this.authenticationService.currentUserSetObservable.pipe(
                        switchMap((user) => {
                            this.currentUser = user;
                            return this.userService.geographiesGeographyIDLandingPageGet(this.geography.GeographyID);
                        }),
                        tap((x) => {
                            this.hasUserCompletedSetUp = this.geography.WellRegistryEnabled
                                ? this.currentUser && x.NumberOfWaterAccounts > 0 && x.NumberOfWellRegistrations > 0
                                : this.currentUser && x.NumberOfWaterAccounts > 0;
                        })
                    );
                }
                return of(new GeographyLandingPageDto());
            })
        );
    }

    public signUp(): void {
        this.authenticationService.signUp();
    }

    public login(): void {
        this.authenticationService.login();
    }
}
