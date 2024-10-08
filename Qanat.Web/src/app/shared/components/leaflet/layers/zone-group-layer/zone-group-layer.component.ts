import { CommonModule } from "@angular/common";
import { AfterViewInit, Component, Input, OnChanges, SimpleChange, SimpleChanges } from "@angular/core";
import { environment } from "src/environments/environment";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
@Component({
    selector: "zone-group-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./zone-group-layer.component.html",
    styleUrls: ["./zone-group-layer.component.scss"],
})
export class ZoneGroupLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    constructor(private wfsService: WfsService) {
        super();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)) {
            this.ngAfterViewInit();
        }
    }

    @Input({ required: true }) zoneGroupID: number;
    @Input({ required: true }) zoneGroupName: string;
    @Input() fitToBounds: boolean = false;
    public wmsOptions: L.WMSOptions;
    public layer;

    ngAfterViewInit(): void {
        if (this.layer) {
            this.map.removeLayer(this.layer);
            this.layerControl.removeLayer(this.layer);
        }
        const cql_filter = `ZoneGroupID = ${this.zoneGroupID}`;

        if (this.fitToBounds) {
            this.wfsService.getGeoserverWFSLayer(null, "Qanat:ZoneGroups", cql_filter).subscribe((x) => {
                const geoJson = L.geoJSON(x);
                this.map.fitBounds(geoJson.getBounds());
            });
        }

        this.wmsOptions = {
            layers: "Qanat:ZoneGroups",
            transparent: true,
            format: "image/png",
            tiled: true,
            styles: "zone_group",
            cql_filter: cql_filter,
        };

        this.layer = L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", this.wmsOptions);
        this.initLayer();
    }
}
