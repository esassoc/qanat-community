import { AfterViewInit, Component, Input, OnDestroy, OnInit } from "@angular/core";
import { LeafletHelperService } from "../../services/leaflet-helper.service";
import * as L from "leaflet";
import GestureHandling from "leaflet-gesture-handling";
import { WaterAccountService } from "../../generated/api/water-account.service";
import { WaterAccountGeoJSONDto } from "../../generated/model/water-account-geo-json-dto";
import { GeographyDto } from "../../generated/model/geography-dto";

@Component({
    selector: "static-water-account-map",
    templateUrl: "./static-water-account-map.component.html",
    styleUrls: ["./static-water-account-map.component.scss"],
    standalone: true,
})
export class StaticWaterAccountMapComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() mapID: string = "staticWaterAccountMap";
    @Input() mapHeight: string = "500px";
    @Input() geography: GeographyDto;

    private _waterAccountID: number = null;
    @Input() set waterAccountID(value: number) {
        if (this.waterAccountID != value) {
            this._waterAccountID = value;
            this.changedWaterAccount(value);
        }
    }
    private waterAccountGeoJSON: WaterAccountGeoJSONDto;

    private map: L.map;
    private waterAccountParcelsLayer: L.LayerGroup = L.layerGroup();
    private usageEntityLayer: L.LayerGroup;
    private tileLayers = this.leafletHelperService.tileLayers;
    private parcelStyle: "parcel_blue";

    private parcelsLayer;
    private usageLayer;

    constructor(
        private leafletHelperService: LeafletHelperService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnDestroy(): void {
        this.map.remove();
    }

    ngOnInit(): void {}

    private layerControl: L.Control.Layers;
    private changedWaterAccount(waterAccountID: number) {
        this.waterAccountService.waterAccountsWaterAccountIDGeojsonGet(waterAccountID).subscribe((waterAccountGeoJSON) => {
            if (!this.layerControl) {
                this.layerControl = new L.Control.Layers(this.tileLayers).addTo(this.map);
            }
            if (this.parcelsLayer) {
                this.layerControl.removeLayer(this.parcelsLayer);
            }
            if (this.usageLayer) {
                this.layerControl.removeLayer(this.usageLayer);
            }

            this.waterAccountGeoJSON = waterAccountGeoJSON;
            this.parcelsLayer = this.createWaterAccountParcelsLayer();
            this.parcelsLayer.addTo(this.map);
            this.layerControl.addOverlay(this.parcelsLayer, "Parcels");

            if (this.geography.DisplayUsageGeometriesAsField) {
                this.usageLayer = this.createUsageEntityLayer();
                this.usageLayer.addTo(this.map);
                this.layerControl.addOverlay(this.usageLayer, "Fields");
            } else {
                this.usageLayer = null;
            }
            this.leafletHelperService.fitMapToBoundingBox(this.map, this.waterAccountGeoJSON.BoundingBox);
        });
    }

    ngAfterViewInit(): void {
        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            fullscreenControl: true,
            zoomControl: true,
            dragging: true,
            gestureHandling: true,
        } as L.MapOptions;

        this.map = L.map(this.mapID, mapOptions);
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);
    }

    private createUsageEntityLayer(): L.LayerGroup {
        this.usageEntityLayer = L.layerGroup();

        this.waterAccountGeoJSON.UsageEntities.forEach((usageEntity) => {
            const geoJSON = new L.geoJson(JSON.parse(usageEntity.GeoJSON), { style: { color: "#7ee556", fillOpacity: 0.05 } })
                .bindPopup(`<usage-entity-popup-custom-element usage-entity-id="${usageEntity.UsageEntityID}"></usage-entity-popup-custom-element>`, {
                    maxWidth: "400px",
                })
                .addTo(this.usageEntityLayer);

            geoJSON.on("mouseover", (e) => {
                geoJSON.setStyle({ fillOpacity: 0.2 });
                geoJSON.on("mouseout", (e) => {
                    geoJSON.setStyle({ fillOpacity: 0.05 });
                });
            });
        });
        return this.usageEntityLayer;
    }

    private createWaterAccountParcelsLayer(): L.LayerGroup {
        this.waterAccountParcelsLayer.clearLayers();

        this.waterAccountGeoJSON.Parcels.forEach((parcel) => {
            const geoJSON = new L.geoJson(JSON.parse(parcel.GeoJSON), { style: this.parcelStyle })
                .bindPopup(`<parcel-popup-custom-element parcel-id="${parcel.ParcelID}"></parcel-popup-custom-element>`, {
                    maxWidth: "400px",
                })
                .addTo(this.waterAccountParcelsLayer);

            geoJSON.on("mouseover", (e) => {
                geoJSON.setStyle({ fillOpacity: 0.5 });
                geoJSON.on("mouseout", (e) => {
                    geoJSON.setStyle({ fillOpacity: 0.1 });
                });
            });
        });
        return this.waterAccountParcelsLayer;
    }
}
