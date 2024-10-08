import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import * as L from "leaflet";
import "leaflet.fullscreen";

@Component({
    selector: "parcel-minimap",
    templateUrl: "./parcel-minimap.component.html",
    styleUrls: ["./parcel-minimap.component.scss"],
    standalone: true,
})
export class ParcelMinimapComponent implements OnInit {
    @ViewChild("mapElement") mapElement: ElementRef;
    @Input() height: string;
    @Input() width: string;
    @Input() geoJson: any;

    private map: L.Map;
    private layerControl: L.Control.Layers;
    private tileLayers: { [key: string]: any } = {};
    private leafletGeoJson: any;

    constructor() {}

    ngOnDestroy(): void {
        this.map.off();
        this.map.remove();
        this.map = null;
    }

    ngOnInit(): void {}

    ngAfterViewInit(): void {
        let geoJsonObject;
        switch (typeof this.geoJson) {
            case "string":
                geoJsonObject = JSON.parse(this.geoJson);
                break;
            case "object":
                geoJsonObject = this.geoJson;
                break;
            default:
                console.error('The "geoJson" input provided to the parcel minimap component must be either a string or geoJson object');
        }

        this.leafletGeoJson = L.geoJSON(geoJsonObject, {});

        const worldImagery = L.tileLayer("https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}", {
            attribution: "Aerial",
            maxZoom: 22,
            maxNativeZoom: 18,
        });

        const streetMap = L.tileLayer("https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}", {
            attribution: "Street",
            maxZoom: 22,
            maxNativeZoom: 18,
            opacity: 0.7,
            className: "street-map-overlay",
        });

        const baseLayer = L.layerGroup([worldImagery, streetMap]);

        const mapOptions: L.MapOptions = {
            zoomControl: false,
            dragging: false,
            scrollWheelZoom: false,
            doubleClickZoom: false,
            boxZoom: false,
            keyboard: false,
            touchZoom: false,
        } as L.MapOptions;

        this.map = L.map(this.mapElement.nativeElement, mapOptions);
        this.map.fitBounds(this.leafletGeoJson.getBounds());
        this.map.zoomOut(1);
        this.map.addLayer(baseLayer);

        this.leafletGeoJson.addTo(this.map);
    }
}
