import { Injectable, OnDestroy } from "@angular/core";
import { filter, map, ReplaySubject, startWith, Subscription, switchMap, tap } from "rxjs";
import { GeographyDto } from "../generated/model/models";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { PublicService } from "../generated/api/public.service";

@Injectable({
    providedIn: "root",
})
export class GeographyRouteService implements OnDestroy {
    private geographySubject = new ReplaySubject<GeographyDto>(1);
    geography$ = this.geographySubject.asObservable();

    private geographies: GeographyDto[];

    private geographiesSub: Subscription = Subscription.EMPTY;
    private routerSub: Subscription = Subscription.EMPTY;

    constructor(private route: ActivatedRoute, private publicService: PublicService, private router: Router) {
        this.fetchGeographies();
    }

    fetchGeographies(): void {
        this.geographiesSub = this.publicService.publicGeographiesGet().subscribe((geographies) => {
            this.geographies = geographies;
            this.routerSub = this.router.events
                .pipe(
                    filter((event) => event instanceof NavigationEnd),
                    startWith(null as any),
                    switchMap((e) => {
                        if (this.route.firstChild) {
                            return this.route.firstChild.paramMap;
                        }
                        return this.route.paramMap;
                    }),
                    map((paramMap) => {
                        if (paramMap.has(routeParams.geographyName)) {
                            return this.getGeographyBySlug(paramMap.get(routeParams.geographyName));
                        }
                    }),
                    tap((geography) => this.geographySubject.next(geography))
                )
                .subscribe();
        });
    }

    getGeographyBySlug(slug: string): GeographyDto {
        return this.geographies.find((x) => x.GeographyName.toLowerCase() === slug);
    }

    ngOnDestroy(): void {
        this.geographiesSub.unsubscribe();
        this.routerSub.unsubscribe();
    }
}
