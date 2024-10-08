import { Component, OnInit, Input } from "@angular/core";
import { NgIf, NgSwitch, NgSwitchCase } from "@angular/common";

@Component({
    selector: "geography-logo",
    templateUrl: "./geography-logo.component.html",
    styleUrls: ["./geography-logo.component.scss"],
    standalone: true,
    imports: [NgIf, NgSwitch, NgSwitchCase],
})
export class GeographyLogoComponent implements OnInit {
    @Input() geographyID: number;

    constructor() {}

    ngOnInit(): void {}
}
