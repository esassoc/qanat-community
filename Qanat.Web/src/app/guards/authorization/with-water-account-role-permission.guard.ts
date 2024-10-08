import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router } from "@angular/router";
import { forkJoin, map } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { AlertService } from "src/app/shared/services/alert.service";

export function withWaterAccountRolePermissionGuard(permissionEnum: PermissionEnum, rightsEnum: RightsEnum): CanActivateFn {
    return (route: ActivatedRouteSnapshot) => {
        const authenticationService = inject(AuthenticationService);
        const router = inject(Router);
        const waterAccountService = inject(WaterAccountService);
        const alertService = inject(AlertService);

        const waterAccountID = route.paramMap.get(routeParams.waterAccountID);

        return forkJoin([authenticationService.getCurrentUser(), waterAccountService.waterAccountsWaterAccountIDGet(parseInt(waterAccountID))]).pipe(
            map(([currentUser, waterAccount]) => {
                if (AuthorizationHelper.hasWaterAccountRolePermission(waterAccount.Geography.GeographyID, waterAccount.WaterAccountID, permissionEnum, rightsEnum, currentUser)) {
                    return true;
                }

                alertService.pushNotFoundUnauthorizedAlert();
                return router.createUrlTree(["/"]);
            })
        );
    };
}
