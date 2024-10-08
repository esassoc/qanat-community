import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { ScenarioImageComponent } from "../images/scenario-image/scenario-image.component";
import { IconComponent } from "../../icon/icon.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { RouterLink } from "@angular/router";
import { GetActionStatusBarComponent } from "../../get-action-status-bar/get-action-status-bar.component";

@Component({
    selector: "model-run-card",
    standalone: true,
    imports: [CommonModule, ScenarioImageComponent, IconComponent, ModelNameTagComponent, GetActionStatusBarComponent, RouterLink],
    templateUrl: "./model-run-card.component.html",
    styleUrls: ["./model-run-card.component.scss"],
})
export class ModelRunCardComponent {
    @Input() GETAction: GETActionDto;

    constructor() {}
}
