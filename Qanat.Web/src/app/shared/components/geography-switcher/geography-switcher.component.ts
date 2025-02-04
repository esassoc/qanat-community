import { Component, OnInit } from "@angular/core";
import { Observable, of, switchMap } from "rxjs";
import { GeographyDto, GeographyMinimalDto } from "../../generated/model/models";
import { DropdownToggleDirective } from "../../directives/dropdown-toggle.directive";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { IconComponent } from "../icon/icon.component";
import { GeographyService } from "../../generated/api/geography.service";
import { CurrentGeographyService } from "../../services/current-geography.service";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";

@Component({
    selector: "geography-switcher",
    templateUrl: "./geography-switcher.component.html",
    styleUrls: ["./geography-switcher.component.scss"],
    standalone: true,
    imports: [NgIf, DropdownToggleDirective, IconComponent, NgFor, AsyncPipe],
})
export class GeographySwitcherComponent implements OnInit {
    public currentUserAvailableGeographies$: Observable<GeographyMinimalDto[]>;
    public geography$: Observable<GeographyMinimalDto>;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService
    ) {}

    ngOnInit(): void {
        this.currentUserAvailableGeographies$ = this.geographyService.geographiesCurrentUserGet();
        this.geography$ = this.currentGeographyService.getCurrentGeography();
    }

    public changeGeography(geography: GeographyDto) {
        this.currentGeographyService.setCurrentGeography(geography);

        // If the current route has the geographyName route param
        const firstChild = this.router.routerState.snapshot.root.firstChild;
        if (firstChild && Object.keys(firstChild.params).includes(routeParams.geographyName)) {
            const geographyNameNavigatingFrom = firstChild.paramMap.get(routeParams.geographyName)?.toLowerCase();

            const geographyName = geography.GeographyName.toLowerCase();
            if (geographyNameNavigatingFrom !== geographyName) {
                // Replace only the geography name in the path
                const currentPath = this.router.routerState.snapshot.url.split("?")[0]; // Remove query string
                const newPath = currentPath.replace(geographyNameNavigatingFrom, geographyName);

                this.router.navigate([newPath], {
                    relativeTo: this.route,
                    queryParamsHandling: "preserve", // Preserve existing query parameters
                });
            }
        }
    }
}
