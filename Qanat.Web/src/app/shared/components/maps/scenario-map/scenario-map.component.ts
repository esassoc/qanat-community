import { AfterViewInit, Component, Input, Output, OnInit, EventEmitter, OnDestroy, ChangeDetectorRef, NgZone, AfterViewChecked } from "@angular/core";
import * as L from "leaflet";
import GestureHandling from "leaflet-gesture-handling";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ModelService } from "src/app/shared/generated/api/model.service";

@Component({
    selector: "scenario-map",
    templateUrl: "./scenario-map.component.html",
    styleUrls: ["./scenario-map.component.scss"],
    standalone: true,
})
export class ScenarioMapComponent implements OnInit, AfterViewInit, OnDestroy, AfterViewChecked {
    @Input() modelShortName: string;
    @Input() selecting: boolean = false;

    @Output() mapReady = new EventEmitter<QanatMapInitEvent>();
    @Output() locationSelected = new EventEmitter<L.latlng>();

    public map: L.map;
    public mapID: string = crypto.randomUUID();
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public layerControl: L.Control;

    constructor(
        private modelService: ModelService,
        private leafletHelperService: LeafletHelperService,
        private cdr: ChangeDetectorRef,
        private zone: NgZone
    ) {}

    ngOnDestroy(): void {
        this.map.off();
        this.map.remove();
        this.map = null;
    }

    ngOnInit(): void {}

    ngAfterViewInit(): void {
        this.modelService.modelsModelShortNameBoundaryGet(this.modelShortName).subscribe((modelBoundary) => {
            const mapOptions: L.MapOptions = {
                minZoom: 6,
                maxZoom: 17,
                layers: [this.tileLayers.Aerial],
                fullscreenControl: true,
                gestureHandling: true,
            } as L.MapOptions;

            if (!this.map) {
                this.map = L.map(this.mapID, mapOptions);
            }

            L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

            this.layerControl = new L.Control.Layers(this.tileLayers, null, { collapsed: false });
            this.layerControl.addTo(this.map);

            this.mapReady.emit(new QanatMapInitEvent(this.map, this.layerControl, this.mapID));
            const geoJsonObject = JSON.parse(modelBoundary.GeoJson);
            if (geoJsonObject) {
                const leafletGeoJson = L.geoJson(geoJsonObject, { interactive: true, style: { className: "boundary" } })
                    .addTo(this.map)
                    .on("click", (event: L.LeafletEvent) => this.onMapSelect(event.latlng));

                let bounds = leafletGeoJson.getBounds();
                if (bounds && bounds.isValid()) {
                    this.map.fitBounds(bounds);
                }
            } else {
                this.leafletHelperService.fitMapToDefaultBoundingBox(this.map);
            }
        });
    }

    ngAfterViewChecked(): void {
        if (this.map) {
            this.map.invalidateSize();
        }
    }

    private onMapSelect(latlng: L.latlng) {
        if (!this.selecting) return;
        this.locationSelected.emit(latlng);
    }
}
