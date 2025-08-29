import { Component, Input } from "@angular/core";
import { UserGeographySummaryDto } from "../../generated/model/user-geography-summary-dto";
import { RouterLink } from "@angular/router";
import { DecimalPipe } from "@angular/common";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "large-geography-card",
    templateUrl: "./large-geography-card.component.html",
    styleUrls: ["./large-geography-card.component.scss"],
    imports: [IconComponent, RouterLink, DecimalPipe]
})
export class LargeGeographyCardComponent {
    @Input() userGeographySummary: UserGeographySummaryDto;

    constructor() {}
}
