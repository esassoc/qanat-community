import { inject } from "@angular/core";
import { CanActivateFn, Router, UrlTree } from "@angular/router";
import { Observable, catchError, forkJoin, map, of, switchMap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { FlagEnum } from "../../shared/generated/enum/flag-enum";
import { PublicService } from "src/app/shared/generated/api/public.service";

export const landingPageEnabledGuard: CanActivateFn = (route, state): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const router: Router = inject(Router);
    const publicService: PublicService = inject(PublicService);
    const geographyName = route.paramMap.get(routeParams.geographyName);
    const notFoundUrlTree = router.createUrlTree(["/"]);
    const authService: AuthenticationService = inject(AuthenticationService);

    return authService.guardInitObservable().pipe(
        switchMap((x) => {
            return forkJoin([publicService.getGeographyByNamePublic(geographyName), authService.isAuthenticated() ? authService.getCurrentUser() : of(null)]);
        }),
        map(([geography, userDto]) => {
            const landingPageEnabled = geography?.LandingPageEnabled ?? false;
            if (landingPageEnabled) {
                return true;
            } else if (
                authService.hasGeographyFlagForGeographyID(userDto, FlagEnum.HasManagerDashboard, geography.GeographyID) ||
                authService.hasFlag(userDto, FlagEnum.HasManagerDashboard)
            ) {
                return true;
            }
            return notFoundUrlTree;
        }),
        catchError((err) => {
            return of(notFoundUrlTree);
        })
    );
};
