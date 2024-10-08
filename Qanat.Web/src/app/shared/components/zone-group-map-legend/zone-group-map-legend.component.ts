import { Component, Input, OnInit } from "@angular/core";
import { ZoneGroupMinimalDto } from "../../generated/model/models";
import { NgIf, NgFor } from "@angular/common";

@Component({
    selector: "zone-group-map-legend",
    templateUrl: "./zone-group-map-legend.component.html",
    styleUrls: ["./zone-group-map-legend.component.scss"],
    standalone: true,
    imports: [NgIf, NgFor],
})
export class ZoneGroupMapLegendComponent implements OnInit {
    @Input() zoneGroup: ZoneGroupMinimalDto;

    constructor() {}

    ngOnInit(): void {}
}
