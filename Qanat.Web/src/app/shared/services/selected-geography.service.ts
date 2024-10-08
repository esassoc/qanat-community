import { Injectable, OnDestroy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { forkJoin, ReplaySubject, Subscription } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyService } from "../generated/api/geography.service";
import { FlagEnum } from "../generated/enum/flag-enum";
import { GeographyDto } from "../generated/model/geography-dto";
import { UserDto } from "../generated/model/models";
import { routeParams } from "src/app/app.routes";
import { RouteHelpers } from "../models/router-helpers";
import { AlertService } from "./alert.service";

@Injectable({
    providedIn: "root",
})
export class SelectedGeographyService implements OnDestroy {
    private _currentUserGeographies = new ReplaySubject<GeographyDto[]>(1);
    public curentUserGeographiesObservable = this._currentUserGeographies.asObservable();

    private _currentUserSelectedGeography = new ReplaySubject<GeographyDto>(1);
    public curentUserSelectedGeographyObservable = this._currentUserSelectedGeography.asObservable();

    private currentUser: UserDto;
    private allGeographies: GeographyDto[];
    private currentUserSubscription: Subscription = Subscription.EMPTY;

    constructor(
        private geographyService: GeographyService,
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private alertService: AlertService
    ) {
        this.refreshGeographies();
    }

    public refreshGeographies(): void {
        this.currentUserSubscription = this.authenticationService.currentUserSetObservable.subscribe((user) => {
            this.currentUser = user;
            forkJoin({
                geographies: this.geographyService.publicGeographiesGet(),
            }).subscribe(({ geographies }) => {
                this.allGeographies = geographies;
                this.setCurrentUserGeographies();
            });
        });
    }

    ngOnDestroy(): void {
        this.currentUserSubscription.unsubscribe();
    }

    private setCurrentUserGeographies(): void {
        let userGeographies = [] as GeographyDto[];

        // if user has global manager dashboard they get all geographies to switch between
        if (this.authenticationService.hasFlag(this.currentUser, FlagEnum.HasManagerDashboard)) {
            userGeographies = [...this.allGeographies];
        }

        if (this.authenticationService.hasGeographyFlag(this.currentUser, FlagEnum.HasManagerDashboard)) {
            const geographiesWithRights = Object.keys(this.currentUser.GeographyRights);
            userGeographies = this.allGeographies.filter((x) => geographiesWithRights.includes(x.GeographyID.toString()));
        }

        this._currentUserGeographies.next(userGeographies);

        const currentRoute = RouteHelpers.getCurrentRouteFromActivatedRoute(this.route);
        const geographyNameFromLoad = currentRoute.paramMap.get(routeParams.geographyName);

        if (userGeographies.length > 0) {
            // select the first for now if the user doesn't navigate to a route with a geography on it
            const geographyID = this.geographyIDForGeographyName(geographyNameFromLoad);
            this.selectGeography(
                geographyNameFromLoad &&
                    (this.authenticationService.hasGeographyFlagForGeographyID(this.currentUser, FlagEnum.HasManagerDashboard, geographyID) ||
                        this.authenticationService.hasFlag(this.currentUser, FlagEnum.HasManagerDashboard))
                    ? geographyNameFromLoad
                    : userGeographies[0].GeographyName
            );
        }
    }

    public selectGeography(geographyName: string): void {
        const indexOfGeography = this.allGeographies.findIndex((x) => x.GeographyName.toLowerCase() == geographyName.toLowerCase());
        if (indexOfGeography < 0) {
            this.router.navigate(["/"]).then(() => {
                this.alertService.pushNotFoundUnauthorizedAlert();
            });
        }
        this._currentUserSelectedGeography.next(this.allGeographies[indexOfGeography]);

        // if they are on a page that has the geographyName route param, we want to redirect them to the right geography
        if (Object.keys(this.router.routerState.snapshot.root.firstChild.params).includes(routeParams.geographyName)) {
            const geographyNameNavigatingFrom = this.router.routerState.snapshot.root.firstChild.paramMap.get(routeParams.geographyName);

            if (geographyNameNavigatingFrom != geographyName.toLowerCase()) {
                const pathToGoTo = this.router.routerState.snapshot.root.firstChild.routeConfig.path
                    .replace(`:${routeParams.geographyName}`, geographyName.toString().toLowerCase())
                    .split("/");
                this.router.navigate(pathToGoTo);
            }
        }
    }

    public geographyIDForGeographyName(geographyName: string): number {
        if (!geographyName || !this.allGeographies) return;

        const indexOfGeography = this.allGeographies.findIndex((x) => x.GeographyName == geographyName || x.GeographyName.toLowerCase() == geographyName);
        if (indexOfGeography >= 0) {
            return this.allGeographies[indexOfGeography].GeographyID;
        }

        return;
    }
}
