import { Component, Input } from "@angular/core";
import { UserGeographySummaryDto } from "../../generated/model/user-geography-summary-dto";
import { RouterLink } from "@angular/router";
import { NgIf, NgFor, DecimalPipe } from "@angular/common";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "large-geography-card",
    templateUrl: "./large-geography-card.component.html",
    styleUrls: ["./large-geography-card.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent, RouterLink, NgFor, DecimalPipe],
})
export class LargeGeographyCardComponent {
    @Input() userGeographySummary: UserGeographySummaryDto;

    constructor() {}
}
