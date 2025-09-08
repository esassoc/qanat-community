import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ScenarioRunStatusEnum } from "../../generated/enum/scenario-run-status-enum";
import { ScenarioRunDto } from "../../generated/model/models";

@Component({
    selector: "scenario-run-status-bar",
    imports: [CommonModule],
    templateUrl: "./scenario-run-status-bar.component.html",
    styleUrls: ["./scenario-run-status-bar.component.scss"]
})
export class ScenarioRunStatusBarComponent implements OnInit {
    public statusColor: string;

    private _scenarioRun: ScenarioRunDto;
    get scenarioRun(): ScenarioRunDto {
        return this._scenarioRun;
    }

    @Input() set scenarioRun(value: ScenarioRunDto) {
        this._scenarioRun = value;

        switch (value.ScenarioRunStatus.ScenarioRunStatusID) {
            case ScenarioRunStatusEnum.Complete:
                this.statusColor = "#2CB92A"; // success green
                break;
            case ScenarioRunStatusEnum.Created:
            case ScenarioRunStatusEnum.CreatedInGET:
            case ScenarioRunStatusEnum.Queued:
            case ScenarioRunStatusEnum.Processing:
            case ScenarioRunStatusEnum.AnalysisSuccess:
            case ScenarioRunStatusEnum.ProcesingInputs:
            case ScenarioRunStatusEnum.RunningAnalysis:
                this.statusColor = "#E9B93F"; // in progress yellow
                break;
            case ScenarioRunStatusEnum.InvalidOutput:
            case ScenarioRunStatusEnum.InvalidInput:
            case ScenarioRunStatusEnum.HasDryCells:
            case ScenarioRunStatusEnum.SystemError:
            case ScenarioRunStatusEnum.GETIntegrationFailure:
            case ScenarioRunStatusEnum.AnalysisFailed:
                this.statusColor = "#ED6969"; // error red
                break;
            default:
                this.statusColor = "#000";
                break;
        }
    }

    constructor() {}

    ngOnInit(): void {}
}
