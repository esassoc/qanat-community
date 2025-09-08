import { Component, OnInit, Input, OnChanges, SimpleChanges } from "@angular/core";
import { GeographyDto, WaterAccountDto, ZoneGroupMinimalDto, ZoneMinimalDto } from "../../generated/model/models";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { Control, Map } from "leaflet";
import { RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { ZoneGroupLayerComponent } from "../leaflet/layers/zone-group-layer/zone-group-layer.component";
import { Observable } from "rxjs";
import { GeographyService } from "../../generated/api/geography.service";

@Component({
    selector: "account-zone-card",
    templateUrl: "./account-zone-card.component.html",
    styleUrls: ["./account-zone-card.component.scss"],
    imports: [AsyncPipe, RouterLink, QanatMapComponent, ZoneGroupLayerComponent]
})
export class AccountZoneCardComponent implements OnInit, OnChanges {
    @Input() waterAccount: WaterAccountDto;
    @Input() zoneGroup: ZoneGroupMinimalDto;
    @Input() zoneID: number;

    public zone: ZoneMinimalDto;

    public geography$: Observable<GeographyDto>;

    constructor(private geographyService: GeographyService) {}

    ngOnInit(): void {
        this.zone = this.zoneGroup.ZoneList?.find((x) => x.ZoneID == this.zoneID);
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes && changes.waterAccount && changes.waterAccount.currentValue) {
            this.geography$ = this.geographyService.getGeographyByIDGeography(changes.waterAccount.currentValue.Geography.GeographyID);
        }
    }

    public map: Map;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
