import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router } from "@angular/router";
import { map } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { GeographyEnum } from "src/app/shared/models/enums/geography.enum";
import { AlertService } from "src/app/shared/services/alert.service";

export function withGeographyFlagGuard(flagEnum: FlagEnum): CanActivateFn {
    return (route: ActivatedRouteSnapshot) => {
        const authenticationService = inject(AuthenticationService);
        const router = inject(Router);
        const alertService = inject(AlertService);

        const geographySlug = route.paramMap.get(routeParams.geographyName);

        return authenticationService.getCurrentUser().pipe(
            map((currentUser) => {
                const geographyID = GeographyEnum[geographySlug];

                if (AuthorizationHelper.hasFlag(FlagEnum.IsSystemAdmin, currentUser)) {
                    return true; //MK 2/5/2025: System Admins can access everything. Wanted to OR these in app.routes but they are AND'd. This is a workaround because System Admins won't have geography flags.
                }

                if (AuthorizationHelper.hasGeographyFlag(geographyID, flagEnum, currentUser)) {
                    return true;
                }

                alertService.pushNotFoundUnauthorizedAlert();
                return router.createUrlTree(["/"]);
            })
        );
    };
}
