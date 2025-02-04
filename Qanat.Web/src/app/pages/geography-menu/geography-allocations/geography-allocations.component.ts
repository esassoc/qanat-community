import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AllocationPlanMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { Control, Map } from "leaflet";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AllocationPlanSelectComponent } from "src/app/shared/components/allocation-plan-select/allocation-plan-select.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ZoneGroupMapLegendComponent } from "src/app/shared/components/zone-group-map-legend/zone-group-map-legend.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "geography-allocations",
    templateUrl: "./geography-allocations.component.html",
    styleUrls: ["./geography-allocations.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        AllocationPlanSelectComponent,
        ZoneGroupMapLegendComponent,
        QanatMapComponent,
        ZoneGroupLayerComponent,
        AsyncPipe,
    ],
})
export class GeographyAllocationsComponent implements OnInit {
    public geography: GeographyDto;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    public zoneGroup$: Observable<ZoneGroupMinimalDto>;

    public customRichTextTypeID = CustomRichTextTypeEnum.GeographyAllocations;
    public isLoading = true;

    constructor(private publicService: PublicService, private route: ActivatedRoute) {}

    ngOnInit(): void {
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.publicService.publicGeographiesNameGeographyNameGet(geographyName).subscribe((geography) => {
            this.geography = geography;
            this.isLoading = false;

            this.allocationPlans$ = this.publicService.publicGeographiesGeographyIDAllocationPlansGet(geography.GeographyID).pipe(
                tap((allocationPlans) => {
                    if (allocationPlans.length > 0) {
                        this.zoneGroup$ = this.publicService.publicGeographiesGeographyIDZoneGroupZoneGroupSlugGet(this.geography.GeographyID, allocationPlans[0].ZoneGroupSlug);
                    }
                })
            );
        });
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
