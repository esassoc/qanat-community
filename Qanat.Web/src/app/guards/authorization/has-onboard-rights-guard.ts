import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { Injectable } from "@angular/core";
import { AlertService } from "../../shared/services/alert.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { FlagEnum } from "../../shared/generated/enum/flag-enum";

@Injectable({
    providedIn: "root",
})
export class HasOnboardRightsGuard {
    constructor(
        private router: Router,
        private alertService: AlertService,
        private authenticationService: AuthenticationService
    ) {}

    canActivate(): Observable<boolean> | Promise<boolean> | boolean {
        return this.authenticationService.getCurrentUser().pipe(
            map((user) => {
                if (this.authenticationService.hasFlag(user, FlagEnum.CanClaimWaterAccounts)) {
                    return true;
                }
                return this.returnUnauthorized();
            })
        );
    }

    private returnUnauthorized() {
        this.router.navigate(["/"]).then(() => {
            this.alertService.pushNotFoundUnauthorizedAlert();
        });
        return false;
    }
}
