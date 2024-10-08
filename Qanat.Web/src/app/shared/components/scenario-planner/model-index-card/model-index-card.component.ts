import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ModelImageComponent } from "../images/model-image/model-image.component";
import { IconComponent } from "../../icon/icon.component";
import { ModelSimpleDto } from "src/app/shared/generated/model/models";
import { RouterLink } from "@angular/router";

@Component({
    selector: "model-index-card",
    standalone: true,
    imports: [CommonModule, ModelImageComponent, IconComponent, RouterLink],
    templateUrl: "./model-index-card.component.html",
    styleUrls: ["./model-index-card.component.scss"],
})
export class ModelIndexCardComponent {
    @Input() model: ModelSimpleDto;

    constructor() {}
}
