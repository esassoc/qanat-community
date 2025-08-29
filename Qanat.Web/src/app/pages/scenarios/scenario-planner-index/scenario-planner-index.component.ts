import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ModelSimpleDto } from "src/app/shared/generated/model/models";
import { GroupByPipe } from "src/app/shared/pipes/group-by.pipe";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AsyncPipe, KeyValuePipe } from "@angular/common";
import { RouterOutlet } from "@angular/router";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ModelIndexCardComponent } from "src/app/shared/components/scenario-planner/model-index-card/model-index-card.component";
import { ModelService } from "src/app/shared/generated/api/model.service";

@Component({
    selector: "scenario-planner-index",
    templateUrl: "./scenario-planner-index.component.html",
    styleUrls: ["./scenario-planner-index.component.scss"],
    imports: [RouterOutlet, PageHeaderComponent, ModelIndexCardComponent, CustomRichTextComponent, AsyncPipe, KeyValuePipe]
})
export class ScenarioPlannerIndexComponent implements OnInit {
    public scenarioPlannerRichTextTypeID = CustomRichTextTypeEnum.ScenarioPlanner;
    public scenarioPlannerGETRichTextTypeID = CustomRichTextTypeEnum.ScenarioPlannerGET;
    public modelGroups$: Observable<ReadonlyMap<string, ModelSimpleDto[]>>;

    constructor(
        private modelService: ModelService,
        private groupByPipe: GroupByPipe
    ) {}

    ngOnInit(): void {
        this.modelGroups$ = this.modelService.listModelsModel().pipe(
            map((x) => {
                return this.groupByPipe.transform<ModelSimpleDto>(x, "ModelSubbasin");
            })
        );
    }
}
