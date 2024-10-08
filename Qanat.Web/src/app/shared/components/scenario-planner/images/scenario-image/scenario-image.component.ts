import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { IconComponent } from "../../../icon/icon.component";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { ScenarioSimpleDto } from "src/app/shared/generated/model/models";

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
    constructor(private getActionService: GETActionService) {}

    ngOnInit(): void {
        this.image$ = this.getActionService.scenariosScenarioIDImageGet(this.scenario.ScenarioID);
    }
}
