import { ActivatedRoute, ActivatedRouteSnapshot, Data } from "@angular/router";

export class RouteHelpers {
    static getRouteDataFromActivatedRoute(activatedRoute: ActivatedRoute): Data {
        return this.getCurrentRouteFromActivatedRoute(activatedRoute).data;
    }

    static getCurrentRouteFromActivatedRoute(activatedRoute: ActivatedRoute): ActivatedRouteSnapshot {
        let snapshot = activatedRoute.snapshot;
        while (snapshot.firstChild) {
            snapshot = snapshot.firstChild;
        }
        return snapshot;
    }

    static getCurrentRouteFromActivatedRouteSnapshot(activatedRoute: ActivatedRouteSnapshot): ActivatedRouteSnapshot {
        let snapshot = activatedRoute;
        while (snapshot.firstChild) {
            snapshot = snapshot.firstChild;
        }
        return snapshot;
    }
}
