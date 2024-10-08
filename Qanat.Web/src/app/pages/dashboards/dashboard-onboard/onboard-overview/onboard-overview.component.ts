import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto, UserDto } from "src/app/shared/generated/model/models";
import { ButtonComponent } from "../../../../shared/components/button/button.component";
import { CustomRichTextComponent } from "../../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "onboard-overview",
    templateUrl: "./onboard-overview.component.html",
    styleUrls: ["./onboard-overview.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, NgIf, AlertDisplayComponent, CustomRichTextComponent, ButtonComponent, RouterLink, AsyncPipe],
})
export class OnboardOverviewComponent implements OnInit {
    private currentUserSubscription: Subscription;
    public currentUser: UserDto;

    public geography$: Observable<GeographyDto>;

    public mainCustomRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.OnboardOverviewContent;
    public sidebarCustomRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.OnboardOverview;

    constructor(
        private authenticationService: AuthenticationService,
        private route: ActivatedRoute,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.currentUserSubscription = this.authenticationService.currentUserSetObservable.subscribe((user) => {
            this.currentUser = user;
        });

        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.geographyService.publicGeographyNameGeographyNameGet(geographyName);
    }

    ngOnDestroy(): void {
        this.currentUserSubscription.unsubscribe();
    }

    isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    public login(): void {
        this.authenticationService.login();
    }

    public signUp(): void {
        this.authenticationService.signUp();
    }
}
