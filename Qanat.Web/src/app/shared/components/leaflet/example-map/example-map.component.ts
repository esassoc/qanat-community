import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { QanatMapComponent, QanatMapInitEvent } from "../qanat-map/qanat-map.component";
import { Control, Map } from "leaflet";
import { GsaBoundariesComponent } from "../layers/gsa-boundaries/gsa-boundaries.component";

@Component({
    selector: "example-map",
    standalone: true,
    imports: [CommonModule, QanatMapComponent, GsaBoundariesComponent],
    template: `
        <qanat-map (onMapLoad)="handleMapReady($event)" mapHeight="800px">
            <ng-container *ngIf="mapIsReady">
                <gsa-boundaries [displayOnLoad]="true" [geographyID]="geographyID" [map]="map" [layerControl]="layerControl"> </gsa-boundaries>
            </ng-container>
        </qanat-map>
    `,
    styles: [],
})
export class ExampleMapComponent implements OnInit {
    public map: Map;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    public geographyID = 1;

    constructor() {}

    ngOnInit(): void {}

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
