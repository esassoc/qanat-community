import { Component, Input, OnInit } from "@angular/core";
import { ZoneGroupMinimalDto } from "../../generated/model/models";

@Component({
    selector: "zone-group-map-legend",
    templateUrl: "./zone-group-map-legend.component.html",
    styleUrls: ["./zone-group-map-legend.component.scss"],
    imports: []
})
export class ZoneGroupMapLegendComponent implements OnInit {
    @Input() zoneGroup: ZoneGroupMinimalDto;

    constructor() {}

    ngOnInit(): void {}
}
