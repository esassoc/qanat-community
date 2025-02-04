import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto, OnboardingWaterAccountDto, UserDto } from "src/app/shared/generated/model/models";
import { ButtonComponent } from "../../../../shared/components/button/button.component";
import { CustomRichTextComponent } from "../../../../shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelMinimapComponent } from "../../../../shared/components/parcel/parcel-minimap/parcel-minimap.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "onboard-water-accounts",
    templateUrl: "./onboard-water-accounts.component.html",
    styleUrls: ["./onboard-water-accounts.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, NgFor, IconComponent, ParcelMinimapComponent, CustomRichTextComponent, ButtonComponent, RouterLink, AsyncPipe],
})
export class OnboardWaterAccountsComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public geography$: Observable<GeographyDto>;

    public currentUserID: number;
    public geographyID: number;
    public waterAccounts: OnboardingWaterAccountDto[];
    public waterAccountGeoJson: { [waterAccountID: number]: Object } = {};

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.OnboardClaimParcels;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private publicService: PublicService,
        private waterAccountUserService: WaterAccountUserService
    ) {}

    ngOnInit(): void {
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);

        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUserID = user.UserID;

                this.geography$ = this.publicService.publicGeographiesNameGeographyNameGet(geographyName).pipe(
                    tap((geography) => {
                        this.geographyID = geography.GeographyID;

                        this.waterAccountUserService.geographiesGeographyIDWaterAccountGet(this.geographyID).subscribe((waterAccounts) => {
                            this.waterAccounts = waterAccounts;
                            waterAccounts.forEach((x) => (this.waterAccountGeoJson[x.WaterAccountID] = this.getWaterAccountGeoJson(x)));
                        });
                    })
                );
            })
        );
    }

    getWaterAccountGeoJson(waterAccount: OnboardingWaterAccountDto): Object {
        return new Object({
            type: "Features Collection",
            features: waterAccount.ParcelGeoJson.map((x) => JSON.parse(x)),
        });
    }

    isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    formIsValid(): boolean {
        return this.waterAccounts?.every((x) => x.IsClaimed != null) ?? false;
    }

    claimAccount(account: OnboardingWaterAccountDto) {
        account.IsClaimed = true;
    }

    rejectAccount(account: OnboardingWaterAccountDto) {
        account.IsClaimed = false;
    }

    onSubmit() {
        this.waterAccountUserService.geographiesGeographyIDWaterAccountsClaimPost(this.currentUserID, this.waterAccounts).subscribe(() => {
            this.authenticationService.checkAndSetActiveAccount();
            this.authenticationService.updateActiveAccount(true);
            this.router.navigateByUrl("/water-dashboard");
        });
    }
}
