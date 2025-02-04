import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ScenarioImageComponent } from "../images/scenario-image/scenario-image.component";
import { IconComponent } from "../../icon/icon.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { RouterLink } from "@angular/router";
import { ScenarioRunStatusBarComponent } from "../../scenario-run-status-bar/scenario-run-status-bar.component";
import { ScenarioRunDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "model-run-card",
    standalone: true,
    imports: [CommonModule, ScenarioImageComponent, IconComponent, ModelNameTagComponent, ScenarioRunStatusBarComponent, RouterLink],
    templateUrl: "./model-run-card.component.html",
    styleUrls: ["./model-run-card.component.scss"],
})
export class ModelRunCardComponent {
    @Input() scenarioRun: ScenarioRunDto;

    constructor() {}
}
