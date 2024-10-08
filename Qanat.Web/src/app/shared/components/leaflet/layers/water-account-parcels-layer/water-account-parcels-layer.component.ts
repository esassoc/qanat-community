import { CommonModule } from "@angular/common";
import { AfterViewInit, Component, Input, OnChanges, SimpleChange } from "@angular/core";
import * as L from "leaflet";

import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
@Component({
    selector: "water-account-parcels-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./water-account-parcels-layer.component.html",
    styleUrls: ["./water-account-parcels-layer.component.scss"],
})
export class WaterAccountParcelsLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    public isLoading: boolean = false;
    @Input() controlTitle: string = "My Parcels";
    @Input({ required: true }) geographyID: number;
    @Input({ required: true }) waterAccountID: number;
    @Input() highlightedParcelIDs: number[];

    public parcelStyle = { color: "#3388FF", dashArray: "4 8", opacity: 0.5, fillOpacity: 0.1 };
    public parcelHighlightedStyle = { color: "#3388FF" };
    public layer;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngOnChanges(changes: any): void {
        if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)) {
            this.updateLayer();
        }
    }

    ngAfterViewInit(): void {
        this.setupLayer();
        this.updateLayer();
    }

    updateLayer() {
        this.layer.clearLayers();

        const cql_filter = `GeographyID = ${this.geographyID} and WaterAccountID = ${this.waterAccountID}`;

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllParcels", cql_filter).subscribe((response) => {
            if (response.length > 0) {
                const geoJson = L.geoJSON(response, {
                    style: (feature) => {
                        if (this.highlightedParcelIDs?.includes(feature.properties.ParcelID)) {
                            return this.parcelHighlightedStyle;
                        }
                        return this.parcelStyle;
                    },
                }).bindPopup((layer) => {
                    if (this.highlightedParcelIDs?.includes(layer.feature.properties.ParcelID)) {
                        return `This Parcel (${layer.feature.properties.ParcelNumber})`;
                    }
                    return `This parcel (${layer.feature.properties.ParcelNumber}) belongs to this parcel's water account.<a href="/water-dashboard/parcels/${layer.feature.properties.ParcelID}">View</a>`;
                });
                geoJson.addTo(this.layer);
                const bounds = this.layer.getBounds();
                this.map.fitBounds(bounds);
            }
            this.isLoading = false;
        });
    }

    setupLayer() {
        this.layer = L.geoJSON();
        this.initLayer();
    }
}
