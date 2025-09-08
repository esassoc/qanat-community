import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { map } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { AlertService } from "src/app/shared/services/alert.service";

export function withFlagGuard(flagEnum: FlagEnum): CanActivateFn {
    return () => {
        const authenticationService = inject(AuthenticationService);
        const router = inject(Router);
        const alertService = inject(AlertService);

        return authenticationService.getCurrentUser().pipe(
            map((currentUser) => {
                if (AuthorizationHelper.hasFlag(flagEnum, currentUser)) {
                    return true;
                }

                alertService.pushNotFoundUnauthorizedAlert();
                return router.createUrlTree(["/"]);
            })
        );
    };
}
