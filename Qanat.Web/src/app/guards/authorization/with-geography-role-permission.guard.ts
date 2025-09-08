import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router } from "@angular/router";
import { map } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { GeographyEnum } from "src/app/shared/models/enums/geography.enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { AlertService } from "src/app/shared/services/alert.service";

export function withGeographyRolePermissionGuard(permissionEnum: PermissionEnum, rightsEnum: RightsEnum): CanActivateFn {
    return (route: ActivatedRouteSnapshot) => {
        const authenticationService = inject(AuthenticationService);
        const router = inject(Router);
        const alertService = inject(AlertService);

        const geographySlug = route.paramMap.get(routeParams.geographyName);

        return authenticationService.getCurrentUser().pipe(
            map((currentUser) => {
                const geographyID = GeographyEnum[geographySlug];

                if (AuthorizationHelper.hasGeographyRolePermission(geographyID, permissionEnum, rightsEnum, currentUser)) {
                    return true;
                }

                alertService.pushNotFoundUnauthorizedAlert();
                return router.createUrlTree(["/"]);
            })
        );
    };
}
