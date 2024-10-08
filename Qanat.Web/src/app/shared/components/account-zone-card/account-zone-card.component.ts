import { Component, OnInit, Input } from "@angular/core";
import { WaterAccountDto, ZoneGroupMinimalDto, ZoneMinimalDto } from "../../generated/model/models";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { Control, Map } from "leaflet";
import { RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { ZoneGroupLayerComponent } from "../leaflet/layers/zone-group-layer/zone-group-layer.component";

@Component({
    selector: "account-zone-card",
    templateUrl: "./account-zone-card.component.html",
    styleUrls: ["./account-zone-card.component.scss"],
    standalone: true,
    imports: [NgIf, RouterLink, QanatMapComponent, ZoneGroupLayerComponent],
})
export class AccountZoneCardComponent implements OnInit {
    @Input() waterAccount: WaterAccountDto;
    @Input() zoneGroup: ZoneGroupMinimalDto;
    @Input() zoneID: number;

    public zone: ZoneMinimalDto;

    constructor() {}

    ngOnInit(): void {
        console.log(this.waterAccount);
        this.zone = this.zoneGroup.ZoneList?.find((x) => x.ZoneID == this.zoneID);
    }

    public map: Map;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    handleMapReady(event: QanatMapInitEvent): void {
        console.log("test");
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
