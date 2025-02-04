import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, NavigationEnd, RouterLink } from "@angular/router";
import { combineLatest, debounceTime, distinctUntilChanged, filter, Observable, of, switchMap, tap } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { VegaZoneGroupUsageChartComponent } from "../../../../shared/components/vega/vega-zone-group-usage-chart/vega-zone-group-usage-chart.component";
import { ParcelMapComponent } from "../../../../shared/components/parcel/parcel-map/parcel-map.component";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "zone-group-detail",
    templateUrl: "./zone-group-detail.component.html",
    styleUrls: ["./zone-group-detail.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, RouterLink, AlertDisplayComponent, ParcelMapComponent, VegaZoneGroupUsageChartComponent, AsyncPipe],
})
export class ZoneGroupDetailComponent implements OnInit {
    public geography$: Observable<GeographyDto>;
    public zoneGroupSlug$: Observable<string>;
    public zoneGroup$: Observable<ZoneGroupMinimalDto>;
    public isLoading = true;

    constructor(
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.zoneGroupSlug$ = this.route.params.pipe(
            switchMap((params) => {
                return of(params.zoneGroupSlug);
            })
        );

        this.zoneGroup$ = combineLatest({ geography: this.geography$, zoneGroupSlug: this.zoneGroupSlug$ }).pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(({ geography, zoneGroupSlug }) => {
                return this.publicService.publicGeographiesGeographyIDZoneGroupZoneGroupSlugGet(geography.GeographyID, zoneGroupSlug);
            }),
            tap(() => {
                this.isLoading = false;
            })
        );
    }
}
