import { Component, OnInit, Input } from "@angular/core";

@Component({
    selector: "geography-logo",
    templateUrl: "./geography-logo.component.html",
    styleUrls: ["./geography-logo.component.scss"],
    imports: []
})
export class GeographyLogoComponent implements OnInit {
    @Input() geographyID: number;

    constructor() {}

    ngOnInit(): void {}
}
