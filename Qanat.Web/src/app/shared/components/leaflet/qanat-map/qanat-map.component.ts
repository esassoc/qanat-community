import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, Output } from "@angular/core";

import { Control, LeafletEvent, Map, MapOptions } from "leaflet";
import * as L from "leaflet";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { BoundingBoxDto } from "src/app/shared/generated/model/models";
import { environment } from "src/environments/environment";

@Component({
    selector: "qanat-map",
    template: `
        <div [id]="mapID" [style.height]="mapHeight">
            <ng-content></ng-content>
        </div>
    `,
    styles: [],
})
export class QanatMapComponent implements AfterViewInit, OnDestroy {
    @Input() boundingBox: BoundingBoxDto;
    @Input() mapHeight: string = "500px";
    @Input() showLayerControl: boolean = true;
    @Output() onMapLoad: EventEmitter<QanatMapInitEvent> = new EventEmitter();

    public mapID: string = crypto.randomUUID();
    public map: Map;
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public layerControl: Control.Layers;

    constructor() {}

    ngAfterViewInit(): void {
        const mapOptions: MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            fullscreenControl: true,
            gestureHandling: true,
        } as MapOptions;

        this.map = L.map(this.mapID, mapOptions);

        this.layerControl = new Control.Layers(this.tileLayers, null, { collapsed: true });
        if (this.showLayerControl) {
            this.layerControl.addTo(this.map);
        }

        this.map.on("load", (event: LeafletEvent) => {
            this.onMapLoad.emit(new QanatMapInitEvent(this.map, this.layerControl, this.mapID));
        });

        if (this.boundingBox == null) {
            this.boundingBox = new BoundingBoxDto();
            this.boundingBox.Left = environment.parcelBoundingBoxLeft;
            this.boundingBox.Bottom = environment.parcelBoundingBoxBottom;
            this.boundingBox.Right = environment.parcelBoundingBoxRight;
            this.boundingBox.Top = environment.parcelBoundingBoxTop;
        }

        this.map.fitBounds(
            [
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ],
            null
        );
    }

    ngOnDestroy(): void {
        if (this.map) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }
    }
}

export class QanatMapInitEvent {
    public map: Map;
    public layerControl: any;
    public mapID: string;

    constructor(map: Map, layerControl: any, mapID?: string) {
        this.map = map;
        this.layerControl = layerControl;
        this.mapID = mapID;
    }
}
