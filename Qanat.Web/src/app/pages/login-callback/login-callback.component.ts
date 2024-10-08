import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";

@Component({
    selector: "qanat-login-callback",
    templateUrl: "./login-callback.component.html",
    styleUrls: ["./login-callback.component.scss"],
    standalone: true,
})
export class LoginCallbackComponent implements OnInit, OnDestroy {
    constructor(
        private router: Router,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.router.navigate(["/"]);
        });
    }

    ngOnDestroy(): void {}
}
