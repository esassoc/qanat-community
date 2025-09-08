import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { map } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { AlertService } from "src/app/shared/services/alert.service";

export function withRolePermissionGuard(permissionEnum: PermissionEnum, rightsEnum: RightsEnum): CanActivateFn {
    return () => {
        const authenticationService = inject(AuthenticationService);
        const router = inject(Router);
        const alertService = inject(AlertService);

        return authenticationService.getCurrentUser().pipe(
            map((currentUser) => {
                if (AuthorizationHelper.hasRolePermission(permissionEnum, rightsEnum, currentUser)) {
                    return true;
                }

                alertService.pushNotFoundUnauthorizedAlert();
                return router.createUrlTree(["/"]);
            })
        );
    };
}
