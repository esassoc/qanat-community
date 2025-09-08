import { Component, Input, Output, EventEmitter } from "@angular/core";
import * as L from "leaflet";
import { ScenarioObservationPointDto, ScenarioPumpingWellDto, ScenarioRechargeSiteDto } from "../../generated/model/models";

@Component({
    selector: "scenario-map-marker-layer",
    templateUrl: "./scenario-map-marker-layer.component.html",
    styleUrls: ["./scenario-map-marker-layer.component.scss"],
    standalone: true,
})
export class ScenarioMapMarkerLayerComponent {
    @Input() map: L.map;
    @Input() markerIcon: object;
    @Input() selectedMarkerIcon: object;

    @Input()
    set markerObjects(value: Array<object>) {
        this._markerObjects = value;
        this.updateMarkerLayers();
    }

    @Input()
    set selectedMarkerObject(value: object) {
        this._selectedMarkerObject = value;
        this.updateMarkerLayers();
    }

    @Output() markerSelected = new EventEmitter<ScenarioRechargeSiteDto | ScenarioObservationPointDto | ScenarioPumpingWellDto>();

    private _markerObjects: Array<any>;
    private _selectedMarkerObject: any;
    private markerLayer: L.LayerGroup;
    private selectedMarkerLayer: L.Layer;

    constructor() {}

    private updateMarkerLayers() {
        if (this.markerLayer) {
            this.map.removeLayer(this.markerLayer);
            this.markerLayer = null;
        }
        this.markerLayer = L.layerGroup();

        this._markerObjects.forEach((object, i) => {
            if (!object.Latitude || !object.Longitude) return;
            if (object.Latitude == this._selectedMarkerObject?.Latitude && object.Longitude == this._selectedMarkerObject?.Longitude) return;

            const latitude = object.Latitude;
            const longitude = object.Longitude;

            new L.marker([latitude, longitude], { index: i, icon: this.markerIcon, draggable: true })
                .on("click", function (e) {
                    const object = this._markerObjects[e.target.options.index];
                    this.markerSelected.emit(object);
                })
                .on("dragend", function (e) {
                    const latlng = e.target.getLatLng();

                    const object = this._markerObjects[e.target.options.index];
                    object.Latitude = latlng.lat;
                    object.Longitude = latlng.lng;
                })
                .addTo(this.markerLayer);
        });
        this.markerLayer.addTo(this.map);

        this.updateSelectedMarkerLayer();
    }

    private updateSelectedMarkerLayer() {
        if (this.selectedMarkerLayer) {
            this.map.removeLayer(this.selectedMarkerLayer);
            this.selectedMarkerLayer = null;
        }

        if (!this._selectedMarkerObject || !this._selectedMarkerObject.Latitude || !this._selectedMarkerObject.Longitude) return;

        this.selectedMarkerLayer = new L.marker([this._selectedMarkerObject.Latitude, this._selectedMarkerObject.Longitude], { icon: this.selectedMarkerIcon, draggable: true })
            .on("dragend", function (e) {
                const latlng = e.target.getLatLng();
                this._selectedMarkerObject.Latitude = latlng.lat;
                this._selectedMarkerObject.Longitude = latlng.lng;
            })
            .addTo(this.map);
    }
}
