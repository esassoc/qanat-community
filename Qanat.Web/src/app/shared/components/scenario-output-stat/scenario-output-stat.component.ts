import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
    selector: "scenario-output-stat",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./scenario-output-stat.component.html",
    styleUrls: ["./scenario-output-stat.component.scss"],
})
export class ScenarioOutputStatComponent implements OnInit {
    @Input() statTitle: string;
    @Input() value: number;
    @Input() units: string;
    @Input() description: string;

    constructor() {}

    ngOnInit(): void {}
}
