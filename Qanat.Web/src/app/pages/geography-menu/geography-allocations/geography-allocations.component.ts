import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { combineLatest, Observable } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AllocationPlanMinimalDto, GeographyPublicDto, GeographyWithBoundingBoxDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { Control, Map } from "leaflet";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AllocationPlanSelectComponent } from "src/app/shared/components/allocation-plan-select/allocation-plan-select.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ZoneGroupMapLegendComponent } from "src/app/shared/components/zone-group-map-legend/zone-group-map-legend.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";

@Component({
    selector: "geography-allocations",
    templateUrl: "./geography-allocations.component.html",
    styleUrls: ["./geography-allocations.component.scss"],
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        AllocationPlanSelectComponent,
        ZoneGroupMapLegendComponent,
        QanatMapComponent,
        ZoneGroupLayerComponent,
        AsyncPipe,
    ]
})
export class GeographyAllocationsComponent implements OnInit {
    public geography$: Observable<GeographyWithBoundingBoxDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    public zoneGroup$: Observable<ZoneGroupMinimalDto>;

    public customRichTextTypeID = CustomRichTextTypeEnum.GeographyAllocations;
    public isLoading = true;

    constructor(
        private publicService: PublicService,
        private authenticationService: AuthenticationService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params.geographyName;
                return combineLatest({
                    geography: this.publicService.getGeographyByNameWithBoundingBoxPublic(geographyName),
                    user: this.authenticationService.getCurrentUser(),
                });
            }),
            tap((x) => {
                if (!x.geography.AllocationPlansVisibleToLandowners || (!x.user && !x.geography.AllocationPlansVisibleToPublic)) {
                    this.router.navigate(["/geographies", x.geography.GeographyName.toLowerCase(), "overview"]);
                } else {
                    this.router.navigate(["/geographies", x.geography.GeographyName.toLowerCase(), "allocation-plans"]);
                }
            }),
            map((x) => {
                return x.geography;
            })
        );

        this.allocationPlans$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.publicService.listAllocationPlansByGeographyIDPublic(geography.GeographyID);
            })
        );

        this.zoneGroup$ = combineLatest({ geography: this.geography$, allocationPlans: this.allocationPlans$ }).pipe(
            switchMap(({ geography, allocationPlans }) => {
                if (allocationPlans.length > 0) {
                    return this.publicService.getZoneGroupBySlugPublic(geography.GeographyID, allocationPlans[0].ZoneGroupSlug).pipe(
                        tap(() => {
                            this.isLoading = false;
                        })
                    );
                } else {
                    return new Observable<ZoneGroupMinimalDto>().pipe(
                        tap(() => {
                            this.isLoading = false;
                        })
                    );
                }
            })
        );
    }

    public map: Map;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
