import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";

@Component({
    selector: "name-tag",
    templateUrl: "./name-tag.component.html",
    styleUrls: ["./name-tag.component.scss"],
    imports: [RouterLink]
})
export class ModelNameTagComponent {
    @Input() name: string;
    @Input() routerLink: string | any[];
    @Input() color: string = "#5a9cb0";
    @Input() textColor: string = "#FFFFFF";
}
