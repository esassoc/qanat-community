import { Component, OnInit, OnDestroy } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { environment } from "src/environments/environment";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable } from "rxjs";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { HomepageUserPortalComponent } from "../homepage-user-portal/homepage-user-portal.component";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "app-home-index",
    templateUrl: "./home-index.component.html",
    styleUrls: ["./home-index.component.scss"],
    standalone: true,
    imports: [NgIf, HomepageUserPortalComponent, AlertDisplayComponent, CustomRichTextComponent, AsyncPipe],
})
export class HomeIndexComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;

    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute
    ) {}

    public ngOnInit(): void {
        this.route.queryParams.subscribe((params) => {
            //We're logging in
            if (params.hasOwnProperty("code")) {
                this.router.navigate(["/signin-oidc"], { queryParams: params });
                return;
            }

            if (localStorage.getItem("loginOnReturn")) {
                localStorage.removeItem("loginOnReturn");
                this.authenticationService.login();
            }

            //We were forced to logout or were sent a link and just finished logging in
            if (sessionStorage.getItem("authRedirectUrl")) {
                this.router.navigateByUrl(sessionStorage.getItem("authRedirectUrl")).then(() => {
                    sessionStorage.removeItem("authRedirectUrl");
                });
            }

            this.currentUser$ = this.authenticationService.getCurrentUser();
        });
    }

    ngOnDestroy(): void {}

    public login(): void {
        this.authenticationService.login();
    }

    public logout(): void {
        this.authenticationService.logout();
    }

    public signUp(): void {
        this.authenticationService.signUp();
    }

    public platformLongName(): string {
        return environment.platformLongName;
    }
}
