import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { IconComponent } from "../../../icon/icon.component";
import { ScenarioSimpleDto } from "src/app/shared/generated/model/models";
import { ScenarioService } from "src/app/shared/generated/api/scenario.service";

@Component({
    selector: "scenario-image",
    standalone: true,
    imports: [CommonModule, IconComponent],
    templateUrl: "./scenario-image.component.html",
    styleUrls: ["./scenario-image.component.scss"],
})
export class ScenarioImageComponent implements OnInit {
    @Input() scenario: ScenarioSimpleDto;
    readonly ScenarioEnum = ScenarioEnum;

    public image$: Observable<string>;
    constructor(private scenarioService: ScenarioService) {}

    ngOnInit(): void {
        this.image$ = this.scenarioService.scenariosScenarioIDImageGet(this.scenario.ScenarioID);
    }
}
