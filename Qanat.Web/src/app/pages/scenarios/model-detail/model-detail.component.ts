import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ModelEnum } from "src/app/shared/generated/enum/model-enum";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { ModelSimpleDto } from "src/app/shared/generated/model/models";
import { environment } from "src/environments/environment";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ScenarioActionsHeroComponent } from "src/app/shared/components/scenario-planner/scenario-actions-hero/scenario-actions-hero.component";
import { ModelRunCardComponent } from "src/app/shared/components/scenario-planner/model-run-card/model-run-card.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";

@Component({
    selector: "model-detail",
    templateUrl: "./model-detail.component.html",
    styleUrls: ["./model-detail.component.scss"],
    standalone: true,
    imports: [LoadingDirective, NgIf, PageHeaderComponent, ScenarioActionsHeroComponent, NgFor, ModelRunCardComponent, QanatGridComponent, AsyncPipe],
})
export class ModelDetailComponent implements OnInit {
    public model$: Observable<ModelSimpleDto>;
    public isLoading: boolean = false;
    public allModelRuns$: Observable<GETActionDto[]>;
    public latestModelRuns: GETActionDto[] = [];
    public getDashboardUrl: string;
    public customRichTextID: number;

    public columnDefs: ColDef[];

    constructor(
        private route: ActivatedRoute,
        private getActionService: GETActionService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.isLoading = true;
        this.model$ = this.route.paramMap.pipe(
            switchMap((params) => {
                const modelShortName = params.get(routeParams.modelShortName);
                this.allModelRuns$ = this.getActionService.modelsModelShortNameActionsGet(modelShortName).pipe(
                    tap((x) => {
                        this.latestModelRuns = x.slice(0, 3);
                        switch (x[0].Model.ModelID) {
                            case ModelEnum.KernC2VSimFGKern:
                                this.customRichTextID = CustomRichTextTypeEnum.KernScenarioModel;
                                break;
                            case ModelEnum.MercedWaterResourcesModel:
                                this.customRichTextID = CustomRichTextTypeEnum.MercedWaterResourcesModel;
                                break;
                            case ModelEnum.YSGAWaterResourcesModel:
                                this.customRichTextID = CustomRichTextTypeEnum.YoloScenarioModel;
                        }
                    })
                );
                return this.getActionService.modelsModelShortNameGet(modelShortName);
            }),
            tap((x) => {
                this.getDashboardUrl = environment.getDashboardUrl + "/Model/ModelDetails?modelID=" + x.GETModelID;
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
