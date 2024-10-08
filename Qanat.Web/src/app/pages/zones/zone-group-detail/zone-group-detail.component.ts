import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { VegaZoneGroupUsageChartComponent } from "../../../shared/components/vega/vega-zone-group-usage-chart/vega-zone-group-usage-chart.component";
import { ParcelMapComponent } from "../../../shared/components/parcel-map/parcel-map.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "zone-group-detail",
    templateUrl: "./zone-group-detail.component.html",
    styleUrls: ["./zone-group-detail.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, RouterLink, AlertDisplayComponent, ParcelMapComponent, VegaZoneGroupUsageChartComponent, AsyncPipe],
})
export class ZoneGroupDetailComponent implements OnInit {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    public zoneGroupSlug: string;
    public geographyID: number;
    public geography: GeographyDto;
    public zoneGroup$: Observable<ZoneGroupMinimalDto>;
    public isLoading = true;

    constructor(
        private route: ActivatedRoute,
        private selectedGeographyService: SelectedGeographyService,
        private zoneGroupService: ZoneGroupService
    ) {}

    ngOnInit(): void {
        this.zoneGroupSlug = this.route.snapshot.paramMap.get(routeParams.zoneGroupSlug);

        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.geography = geography;
            this.zoneGroup$ = this.zoneGroupService.publicGeographyGeographyIDZoneGroupZoneGroupSlugGet(this.geographyID, this.zoneGroupSlug);
            this.isLoading = false;
        });
    }

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }
}
