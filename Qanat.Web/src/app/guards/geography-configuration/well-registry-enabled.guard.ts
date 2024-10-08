import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { inject } from "@angular/core";
import { RouteHelpers } from "../../shared/models/router-helpers";
import { Observable, map } from "rxjs";
import { GeographyConfigurationService } from "../../shared/generated/api/geography-configuration.service";
import { routeParams } from "src/app/app.routes";

export const wellRegistryEnabledGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const router: Router = inject(Router);
    const geographyConfigurationService: GeographyConfigurationService = inject(GeographyConfigurationService);
    const currentRoute = RouteHelpers.getCurrentRouteFromActivatedRouteSnapshot(route);
    const geographySlug = currentRoute.paramMap.get(routeParams.geographyName);

    return geographyConfigurationService.geographyConfigurationsGet().pipe(
        map((configurations) => {
            const enabled = configurations.find((x) => x.GeographySlug == geographySlug).WellRegistryEnabled;
            if (enabled) return true;
            console.warn(`The '${geographySlug}' geography does not have the well registry enabled.`);
            return router.createUrlTree(["not-found"]);
        })
    );
};
