import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { inject } from "@angular/core";
import { RouteHelpers } from "../../shared/models/router-helpers";
import { Observable, map } from "rxjs";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { routeParams } from "src/app/app.routes";

export const wellRegistryEnabledGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const router: Router = inject(Router);
    const geographyService: GeographyService = inject(GeographyService);
    const currentRoute = RouteHelpers.getCurrentRouteFromActivatedRouteSnapshot(route);
    const geographySlug = currentRoute.paramMap.get(routeParams.geographyName);

    return geographyService.getByNameAsMinimalDtoGeography(geographySlug).pipe(
        map((geography) => {
            const enabled = geography.GeographyConfiguration.WellRegistryEnabled;
            if (enabled) return true;
            console.warn(`The '${geographySlug}' geography does not have the well registry enabled.`);
            return router.createUrlTree(["not-found"]);
        })
    );
};
