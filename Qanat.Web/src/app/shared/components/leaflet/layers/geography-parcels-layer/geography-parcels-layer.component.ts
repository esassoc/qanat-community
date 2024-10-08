import { CommonModule } from "@angular/common";
import { Component, Input, OnChanges, OnInit } from "@angular/core";
import { environment } from "src/environments/environment";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
@Component({
    selector: "geography-parcels-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./geography-parcels-layer.component.html",
    styleUrls: ["./geography-parcels-layer.component.scss"],
})
export class GeographyParcelsLayerComponent extends MapLayerBase implements OnInit, OnChanges {
    constructor() {
        super();
    }

    @Input() geographyID: number;
    @Input() parcelStyle: string = "parcel";
    public wmsOptions: L.WMSOptions;
    public layer;

    ngOnInit(): void {}

    ngAfterViewInit(): void {
        const cql_filter = this.geographyID ? `GeographyID = ${this.geographyID}` : "";
        this.wmsOptions = {
            layers: "Qanat:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true,
            styles: `${this.parcelStyle}`,
        };

        if (this.geographyID) {
            this.wmsOptions.cql_filter = cql_filter;
        }
        this.layer = L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", this.wmsOptions);
        this.initLayer();
    }
}
