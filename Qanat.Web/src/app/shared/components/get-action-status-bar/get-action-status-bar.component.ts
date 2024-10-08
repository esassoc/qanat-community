import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { GETActionStatusEnum } from "src/app/shared/generated/enum/g-e-t-action-status-enum";

@Component({
    selector: "get-action-status-bar",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./get-action-status-bar.component.html",
    styleUrls: ["./get-action-status-bar.component.scss"],
})
export class GetActionStatusBarComponent implements OnInit {
    public statusColor: string;

    private _getAction: GETActionDto;
    get getAction(): GETActionDto {
        return this._getAction;
    }
    @Input() set getAction(value: GETActionDto) {
        this._getAction = value;

        switch (value.GETActionStatus.GETActionStatusID) {
            case GETActionStatusEnum.Complete:
                this.statusColor = "#2CB92A"; // success green
                break;
            case GETActionStatusEnum.Created:
            case GETActionStatusEnum.CreatedInGET:
            case GETActionStatusEnum.Queued:
            case GETActionStatusEnum.Processing:
            case GETActionStatusEnum.AnalysisSuccess:
            case GETActionStatusEnum.ProcesingInputs:
            case GETActionStatusEnum.RunningAnalysis:
                this.statusColor = "#E9B93F"; // in progress yellow
                break;
            case GETActionStatusEnum.InvalidOutput:
            case GETActionStatusEnum.InvalidInput:
            case GETActionStatusEnum.HasDryCells:
            case GETActionStatusEnum.SystemError:
            case GETActionStatusEnum.GETIntegrationFailure:
            case GETActionStatusEnum.AnalysisFailed:
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
