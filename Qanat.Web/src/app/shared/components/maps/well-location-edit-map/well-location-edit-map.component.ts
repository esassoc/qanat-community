import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, Output } from "@angular/core";
import * as L from "leaflet";
import { GestureHandling } from "leaflet-gesture-handling";
import { BoundingBoxDto } from "src/app/shared/generated/model/bounding-box-dto";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { environment } from "src/environments/environment";
import { DecimalPipe } from "@angular/common";

@Component({
    selector: "well-location-edit-map",
    templateUrl: "./well-location-edit-map.component.html",
    styleUrl: "./well-location-edit-map.component.scss",
    imports: [DecimalPipe]
})
export class WellLocationEditMapComponent implements AfterViewInit {
    @Input() wellLatLng: L.latLng;
    @Input() parcelGeoJson: string;
    @Input() boundingBox: BoundingBoxDto;

    @Output() locationChanged = new EventEmitter<L.latLng>();

    public mapID: string = crypto.randomUUID();
    public mapHeight: string = "500px";
    public selectedParcelStyle: string = "parcel_yellow";
    public wellMarker: L.Layer;

    public map: L.Map;
    public layerControl: L.Control.Layers;
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public overlayLayers: { [key: string]: any } = {};

    private wellIcon = L.icon({
        iconUrl: "/assets/main/map-icons/marker-icon-selected.png",
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        tooltipAnchor: [16, -28],
        shadowSize: [41, 41],
    });

    public isLoadingSubmit = false;

    constructor(private cdr: ChangeDetectorRef) {}

    ngAfterViewInit(): void {
        const defaultParcelsWMSOptions = {
            layers: "Qanat:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true,
        } as L.WMSOptions;

        const parcelsWMSOptions = Object.assign({ styles: "parcel" }, defaultParcelsWMSOptions);

        this.overlayLayers = Object.assign(
            {
                "All Parcels": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", parcelsWMSOptions),
            },
            this.overlayLayers
        );

        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            gestureHandling: true,
        } as L.MapOptions;
        this.map = L.map(this.mapID, mapOptions);
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        this.updateMapBoundingBox();
        this.setControl();

        if (this.parcelGeoJson) {
            this.addParcelLayer(this.parcelGeoJson);
        }
        if (this.wellLatLng) {
            this.placeWellMarker(this.wellLatLng);
        }

        // register click events
        this.map.on("click", (event: L.LeafletEvent) => this.placeWellMarker(event.latlng));
    }

    private updateMapBoundingBox(boundingBox?: BoundingBoxDto) {
        if (!this.boundingBox) {
            this.boundingBox = new BoundingBoxDto();
        }

        this.boundingBox.Left = boundingBox ? boundingBox.Left : environment.parcelBoundingBoxLeft;
        this.boundingBox.Bottom = boundingBox ? boundingBox.Bottom : environment.parcelBoundingBoxBottom;
        this.boundingBox.Right = boundingBox ? boundingBox.Right : environment.parcelBoundingBoxRight;
        this.boundingBox.Top = boundingBox ? boundingBox.Top : environment.parcelBoundingBoxTop;

        this.map.fitBounds(
            [
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ],
            {}
        );
    }

    private setControl(): void {
        this.layerControl = new L.Control.Layers(this.tileLayers, this.overlayLayers).addTo(this.map);
    }

    private addParcelLayer(geoJsonString: string) {
        const parcelGeoJson = JSON.parse(geoJsonString);
        L.geoJSON(parcelGeoJson, { style: { color: "#FFFF85" }, interactive: false }).addTo(this.map);
    }

    private placeWellMarker(latlng: L.latLng) {
        if (this.wellMarker) {
            this.map.removeLayer(this.wellMarker);
        }

        this.wellMarker = new L.marker(latlng, {
            icon: this.wellIcon,
            zIndexOffset: 1000,
            draggable: true,
        }).on("dragend", (e, m) => {
            const latLng = e.target.getLatLng();
            this.onLocationChanged(latLng);
        });

        this.wellMarker.addTo(this.map);
        this.onLocationChanged(latlng);
        this.cdr.detectChanges();
    }

    private onLocationChanged(latLng: L.latLng) {
        this.wellMarker.Latitude = latLng.lat;
        this.wellMarker.Longitude = latLng.lng;
        this.map.setView(latLng, 17);

        this.locationChanged.emit(latLng);
    }
}
