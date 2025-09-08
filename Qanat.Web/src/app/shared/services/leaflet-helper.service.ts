import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { BoundingBoxDto } from "../generated/model/bounding-box-dto";
import * as L from "leaflet";

@Injectable({
    providedIn: "root",
})
export class LeafletHelperService {
    constructor() {}

    public readonly tileLayers = LeafletHelperService.GetDefaultTileLayers();

    public readonly defaultMapOptions: L.MapOptions = {
        minZoom: 6,
        maxZoom: 20,
        layers: [this.tileLayers.Aerial],
        gestureHandling: true,
    } as L.MapOptions;

    public static readonly defaultBoundingBox = new BoundingBoxDto({
        Left: environment.parcelBoundingBoxLeft,
        Bottom: environment.parcelBoundingBoxBottom,
        Right: environment.parcelBoundingBoxRight,
        Top: environment.parcelBoundingBoxTop,
    });

    public readonly blueIcon = L.icon({
        iconUrl: "/assets/main/map-icons/blue-pin.png",
        shadowUrl: "/assets/main/map-icons/shadow-skew.png",
        iconSize: [22, 35],
        iconAnchor: [12, 34],
        shadowAnchor: [4, 26],
        popupAnchor: [1, -34],
        tooltipAnchor: [16, -28],
        shadowSize: [28, 28],
    });

    public readonly blueIconLarge = L.icon({
        iconUrl: "/assets/main/map-icons/blue-pin.png",
        shadowUrl: "/assets/main/map-icons/shadow-skew.png",
        iconSize: [28, 45],
        iconAnchor: [15, 45],
        shadowAnchor: [5, 34],
        popupAnchor: [1, -45],
        tooltipAnchor: [16, -28],
        shadowSize: [35, 35],
    });

    public readonly yellowIcon = L.icon({
        iconUrl: "/assets/main/map-icons/yellow-pin.png",
        shadowUrl: "/assets/main/map-icons/shadow-skew.png",
        iconSize: [22, 35],
        iconAnchor: [12, 34],
        shadowAnchor: [4, 26],
        popupAnchor: [1, -34],
        tooltipAnchor: [16, -28],
        shadowSize: [28, 28],
    });

    public readonly yellowIconLarge = L.icon({
        iconUrl: "/assets/main/map-icons/yellow-pin.png",
        shadowUrl: "/assets/main/map-icons/shadow-skew.png",
        iconSize: [28, 45],
        iconAnchor: [15, 45],
        shadowAnchor: [5, 34],
        popupAnchor: [1, -45],
        tooltipAnchor: [16, -28],
        shadowSize: [35, 35],
    });

    public markerColors: string[] = ["#7F3C8D", "#11A579", "#3969AC", "#F2B701", "#E73F74", "#80BA5A", "#E68310", "#008695", "#CF1C90", "#f97b72", "#4b4b8f", "#A5AA99"];

    public createDivIcon(color: string, dash: boolean = false) {
        return L.divIcon({
            className: "qanat-div-icon",
            html: `<svg width="100%" viewbox="0 0 30 42">
              <path fill="${color}" stroke="#fff" stroke-width="1.5" ${dash ? 'stroke-dasharray="4"' : ""}
                    d="M15 3
                      Q16.5 6.8 25 18
                      A12.8 12.8 0 1 1 5 18
                      Q13.5 6.8 15 3z" />
            </svg>`,
            iconSize: new L.Point(30, 42),
            iconAnchor: [15, 42],
        });
    }

    public fitMapToDefaultBoundingBox(map: L.map) {
        const defaultBoundingBox = LeafletHelperService.defaultBoundingBox;
        this.fitMapToBoundingBox(map, defaultBoundingBox);
    }

    public fitMapToBoundingBox(map: L.map, boundingBox: BoundingBoxDto) {
        map.fitBounds([
            [boundingBox.Bottom, boundingBox.Left],
            [boundingBox.Top, boundingBox.Right],
        ]);
    }

    public clusterIconCreateFunction(cluster) {
        const childCount = cluster.getChildCount();

        // currently we aren't using these small medium large sizes, but keeping because it doesn't hurt
        let c = " qanat-cluster-";
        if (childCount < 10) {
            c += "small";
        } else if (childCount < 100) {
            c += "medium";
        } else {
            c += "large";
        }

        return new L.DivIcon({
            html: "<div><span>" + childCount + "</span></div>",
            className: "marker-cluster qanat-cluster" + c,
            iconSize: new L.Point(40, 40),
        });
    }

    public zoomToMarker(map: L.Map, marker: L.Marker, zoomLevel?: number) {
        map.setView(marker.getLatLng(), zoomLevel ?? 18);
    }

    public static GetDefaultTileLayers(): { [key: string]: any } {
        return {
            Aerial: L.tileLayer("https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}", {
                attribution: "Aerial",
            }),
            Street: L.tileLayer("https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}", {
                attribution: "Aerial",
            }),
            Terrain: L.tileLayer("https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}", {
                attribution: "Terrain",
            }),
        };
    }
}
