import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyWithBoundingBoxDto } from "src/app/shared/generated/model/geography-with-bounding-box-dto";
import * as L from "leaflet";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { MonitoringWellsLayerComponent } from "src/app/shared/components/leaflet/layers/monitoring-wells-layer/monitoring-wells-layer.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "geography-about",
    templateUrl: "./geography-about.component.html",
    styleUrls: ["./geography-about.component.scss"],
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        QanatMapComponent,
        GsaBoundariesComponent,
        CustomRichTextComponent,
        AsyncPipe,
        MonitoringWellsLayerComponent,
    ]
})
export class GeographyAboutComponent implements OnInit {
    public geography$: Observable<GeographyWithBoundingBoxDto>;
    public customRichTextTypeID = CustomRichTextTypeEnum.GeographyAbout;
    public isLoading = true;

    constructor(
        private publicService: PublicService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.publicService.getGeographyByNameWithBoundingBoxPublic(geographyName);
            }),
            tap((geography) => {
                this.isLoading = false;
                if (this.map && geography.BoundingBox?.Left && geography.BoundingBox.Right && geography.BoundingBox.Top && geography.BoundingBox.Bottom) {
                    this.map.fitBounds([
                        [geography.BoundingBox.Bottom, geography.BoundingBox.Left],
                        [geography.BoundingBox.Top, geography.BoundingBox.Right],
                    ]);
                }
            })
        );
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
