import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";

@Component({
    selector: "name-tag",
    templateUrl: "./name-tag.component.html",
    styleUrls: ["./name-tag.component.scss"],
    standalone: true,
    imports: [NgIf, RouterLink],
})
export class ModelNameTagComponent {
    @Input() name: string;
    @Input() routerLink: string | any[];
    @Input() color: string = "#5a9cb0";
    @Input() textColor: string = "#FFFFFF";
}
