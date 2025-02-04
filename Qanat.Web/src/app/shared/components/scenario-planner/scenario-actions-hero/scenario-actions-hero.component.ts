import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { IconComponent } from "../../icon/icon.component";
import { ScenarioSimpleDto } from "src/app/shared/generated/model/models";
import { RouterLink } from "@angular/router";
import { ScenarioService } from "src/app/shared/generated/api/scenario.service";

@Component({
    selector: "scenario-actions-hero",
    standalone: true,
    imports: [CommonModule, IconComponent, RouterLink],
    templateUrl: "./scenario-actions-hero.component.html",
    styleUrls: ["./scenario-actions-hero.component.scss"],
})
export class ScenarioActionsHeroComponent implements OnInit {
    readonly ScenarioEnum = ScenarioEnum;
    public scenarios$: Observable<ScenarioSimpleDto[]>;

    constructor(private scenarioService: ScenarioService) {}

    ngOnInit(): void {
        this.scenarios$ = this.scenarioService.scenariosGet();
    }
}
