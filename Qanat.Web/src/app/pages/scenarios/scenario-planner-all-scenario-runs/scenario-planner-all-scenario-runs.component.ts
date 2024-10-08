import { Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { ModelSimpleDto } from "src/app/shared/generated/model/models";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { ModelRunCardComponent } from "src/app/shared/components/scenario-planner/model-run-card/model-run-card.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "scenario-planner-all-scenario-runs",
    templateUrl: "./scenario-planner-all-scenario-runs.component.html",
    styleUrls: ["./scenario-planner-all-scenario-runs.component.scss"],
    standalone: true,
    imports: [LoadingDirective, PageHeaderComponent, NgIf, NgFor, ModelRunCardComponent, QanatGridComponent, AsyncPipe],
})
export class ScenarioPlannerAllScenarioRunsComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.ScenarioPlannerScenarioRuns;

    public model$: Observable<ModelSimpleDto>;
    public isLoading: boolean = false;
    public allModelRuns$: Observable<GETActionDto[]>;
    public latestModelRuns: GETActionDto[] = [];

    public columnDefs: ColDef[];

    constructor(
        private getActionService: GETActionService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.isLoading = true;

        this.allModelRuns$ = this.getActionService.actionsGet().pipe(
            tap((x) => {
                this.latestModelRuns = x.slice(0, 3);
                this.isLoading = false;
            })
        );

        this.buildColDef();
    }

    buildColDef() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Scenario Run Name", "", "", {
                ValueGetter: (params) => {
                    return {
                        LinkValue: params.data.Model.ModelShortName + "/" + params.data.Scenario.ScenarioShortName + "/" + params.data.GETActionID,
                        LinkDisplay: params.data.RunName,
                    };
                },
                FilterValueGetter: (params) => params.data.RunName,
                InRouterLink: "/scenario-planner/models/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Model", "Model.ModelName", { CustomDropdownFilterField: "Model.ModelName" }),
            this.utilityFunctionsService.createBasicColumnDef("Scenario", "Scenario.ScenarioName", { CustomDropdownFilterField: "Scenario.ScenarioName" }),
            this.utilityFunctionsService.createBasicColumnDef("Status", "GETActionStatus.GETActionStatusDisplayName", {
                CustomDropdownFilterField: "GETActionStatus.GETActionStatusDisplayName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Last Updated", "LastUpdateDate", "short"),
            this.utilityFunctionsService.createDateColumnDef("Created", "CreateDate", "short", { Sort: "desc" }),
            { headerName: "Created By", field: "User.FullName", width: 150 },
        ];
    }
}
