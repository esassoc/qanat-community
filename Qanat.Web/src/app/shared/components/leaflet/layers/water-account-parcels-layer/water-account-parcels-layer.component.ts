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
    @Input() parcelID: number;

    @Input() highlightedParcelIDs: number[];

    public parcelStyle = { color: "#3388FF", dashArray: "4 8", opacity: 0.5, fillOpacity: 0.1 };
    public parcelHighlightedStyle = { color: "#fcfc12" };
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

        const cql_filter = !this.parcelID //MK 11/21/2024: If we pass in a parcelID we can assume it does not have a water account, we still want the parcel to show up on the map in this case.
            ? `GeographyID = ${this.geographyID} and WaterAccountID = ${this.waterAccountID}`
            : `GeographyID = ${this.geographyID} and ParcelID = ${this.parcelID}`;

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
                        return `This Parcel (<b>${layer.feature.properties.ParcelNumber}</b>)`;
                    }
                    return `Parcel <b>${layer.feature.properties.ParcelNumber}</b> belongs to the same water account as this parcel.<a href="/parcels/${layer.feature.properties.ParcelID}">View</a>`;
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
