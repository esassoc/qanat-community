import { Component, OnInit } from "@angular/core";
import { Observable, switchMap, tap } from "rxjs";
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AsyncPipe } from "@angular/common";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "dashboard-update-parcels",
    templateUrl: "./dashboard-update-parcels.component.html",
    styleUrls: ["./dashboard-update-parcels.component.scss"],
    imports: [AsyncPipe, RouterLink, RouterLinkActive, RouterOutlet, PageHeaderComponent]
})
export class DashboardUpdateParcelsComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;

    constructor(
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );
    }
}
