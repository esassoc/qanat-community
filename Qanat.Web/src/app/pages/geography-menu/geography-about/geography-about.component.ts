import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyWithBoundingBoxDto } from "src/app/shared/generated/model/geography-with-bounding-box-dto";
import * as L from "leaflet";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GsaBoundariesComponent } from "src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";

@Component({
    selector: "geography-about",
    templateUrl: "./geography-about.component.html",
    styleUrls: ["./geography-about.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, QanatMapComponent, GsaBoundariesComponent, CustomRichTextComponent, AsyncPipe],
})
export class GeographyAboutComponent {
    public geography$: Observable<GeographyWithBoundingBoxDto>;
    public customRichTextTypeID = CustomRichTextTypeEnum.GeographyAbout;
    public isLoading = true;

    constructor(
        private geographyService: GeographyService,
        private route: ActivatedRoute
    ) {}

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.geographyService.publicGeographyBoundingBoxGeographyNameGet(geographyName).pipe(
            tap((geography) => {
                this.isLoading = false;
                if (geography.BoundingBox?.Left && geography.BoundingBox.Right && geography.BoundingBox.Top && geography.BoundingBox.Bottom) {
                    this.map.fitBounds([
                        [geography.BoundingBox.Bottom, geography.BoundingBox.Left],
                        [geography.BoundingBox.Top, geography.BoundingBox.Right],
                    ]);
                }
            })
        );
    }
}
