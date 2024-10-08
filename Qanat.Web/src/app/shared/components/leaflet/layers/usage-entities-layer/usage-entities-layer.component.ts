import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
@Component({
    selector: "usage-entities-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./usage-entities-layer.component.html",
    styleUrls: ["./usage-entities-layer.component.scss"],
})
export class UsageEntitiesLayerComponent extends MapLayerBase {
    constructor(
        private wfsService: WfsService,
        private geographyService: GeographyService
    ) {
        super();
    }
    @Input({ required: true }) geographyID: number;
    @Input() parcelIDs: number[];
    public wmsOptions: L.WMSOptions;
    public layer;
    public fieldLayerStyle = { color: "#7ee556", fillOpacity: 0.05 };

    ngAfterViewInit(): void {
        this.geographyService.geographiesGeographyIDGet(this.geographyID).subscribe((geography) => {
            if (!geography.DisplayUsageGeometriesAsField) return;
            this.layer = new L.LayerGroup();
            this.initLayer();
            this.updateLayer();
        });
    }

    updateLayer() {
        let cql_filter = `GeographyID = ${this.geographyID}`;
        if (this.parcelIDs?.length > 0) {
            cql_filter += ` and ParcelID in(${this.parcelIDs.join(",")})`;
        }

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllUsageEntities", cql_filter).subscribe((response) => {
            if (response.length > 0) {
                const geoJSON = new L.geoJson(response, { style: this.fieldLayerStyle })
                    .bindPopup(
                        (layer) => {
                            return `<usage-entity-popup-custom-element usage-entity-id="${layer.feature.properties.UsageEntityID}"></usage-entity-popup-custom-element>`;
                        },
                        {
                            maxWidth: "400px",
                        }
                    )

                    .addTo(this.layer);
                geoJSON.eachLayer((layer) => {
                    layer.on("mouseover", (e) => {
                        layer.setStyle({ fillOpacity: 0.2 });
                    });
                    layer.on("mouseout", (e) => {
                        layer.setStyle({ fillOpacity: 0.05 });
                    });
                });
            }
        });
    }
}
