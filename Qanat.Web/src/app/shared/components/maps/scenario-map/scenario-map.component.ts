import { AfterViewInit, Component, Input, Output, OnInit, EventEmitter, OnDestroy } from "@angular/core";
import * as L from "leaflet";
import GestureHandling from "leaflet-gesture-handling";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";

@Component({
    selector: "scenario-map",
    templateUrl: "./scenario-map.component.html",
    styleUrls: ["./scenario-map.component.scss"],
    standalone: true,
})
export class ScenarioMapComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() modelShortName: string;
    @Input() selecting: boolean = false;

    @Output() mapReady = new EventEmitter<QanatMapInitEvent>();
    @Output() locationSelected = new EventEmitter<L.latlng>();

    public map: L.map;
    public mapID: string = crypto.randomUUID();
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public layerControl: L.Control;

    constructor(
        private getActionService: GETActionService,
        private leafletHelperService: LeafletHelperService
    ) {}

    ngOnDestroy(): void {
        this.map.off();
        this.map.remove();
        this.map = null;
    }

    ngOnInit(): void {
        this.getActionService.modelsModelShortNameBoundaryGet(this.modelShortName).subscribe((modelBoundary) => {
            const geoJsonObject = JSON.parse(modelBoundary.GeoJson);

            if (geoJsonObject) {
                const leafletGeoJson = L.geoJson(geoJsonObject, { interactive: true, style: { className: "boundary" } })
                    .addTo(this.map)
                    .on("click", (event: L.LeafletEvent) => this.onMapSelect(event.latlng));
                this.map.fitBounds(leafletGeoJson.getBounds());
            } else {
                this.leafletHelperService.fitMapToDefaultBoundingBox(this.map);
            }
        });
    }

    ngAfterViewInit(): void {
        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            fullscreenControl: true,
            gestureHandling: true,
        } as L.MapOptions;

        this.map = L.map(this.mapID, mapOptions);
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        this.layerControl = new L.Control.Layers(this.tileLayers, null, { collapsed: false });
        this.layerControl.addTo(this.map);

        this.mapReady.emit(new QanatMapInitEvent(this.map, this.layerControl, this.mapID));
    }

    private onMapSelect(latlng: L.latlng) {
        if (!this.selecting) return;
        this.locationSelected.emit(latlng);
    }
}
