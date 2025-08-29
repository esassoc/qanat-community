import { Component, Input, OnInit } from "@angular/core";
import { WaterTypeSimpleDto } from "../../generated/model/models";
import { DecimalPipe } from "@angular/common";
import { WaterTypeFieldDefinitionComponent } from "../water-type-field-definition/water-type-field-definition.component";

@Component({
    selector: "water-supply-type",
    templateUrl: "./water-supply-type.component.html",
    styleUrls: ["./water-supply-type.component.scss"],
    imports: [WaterTypeFieldDefinitionComponent, DecimalPipe]
})
export class WaterSupplyTypeComponent implements OnInit {
    @Input() waterType: WaterTypeSimpleDto;
    @Input() value: number;
    @Input() totalSupply: number;
    @Input() unit: string;

    constructor() {}

    ngOnInit(): void {}

    barStyle() {
        return "width: " + (this.value / this.totalSupply) * 100 + "%";
    }
}
