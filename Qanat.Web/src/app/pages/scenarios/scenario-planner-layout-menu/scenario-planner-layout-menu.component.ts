import { Component, model, OnInit } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router, RouterLinkActive, RouterLink, RouterOutlet } from "@angular/router";
import { Observable, combineLatest, forkJoin, of } from "rxjs";
import { filter, map, startWith, tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { RouteHelpers } from "src/app/shared/models/router-helpers";
import { CurrentScenarioRunService } from "src/app/shared/services/viewing-get-action.service";
import { ScenarioEnum, Scenarios, ScenariosAsSelectDropdownOptions } from "src/app/shared/generated/enum/scenario-enum";
import { ModelSimpleDto, ScenarioRunDto, ScenarioSimpleDto } from "src/app/shared/generated/model/models";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { ModelService } from "src/app/shared/generated/api/model.service";
import { ScenarioService } from "src/app/shared/generated/api/scenario.service";
import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";

@Component({
    selector: "scenario-planner-layout-menu",
    templateUrl: "./scenario-planner-layout-menu.component.html",
    styleUrls: ["./scenario-planner-layout-menu.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent, RouterLinkActive, RouterLink, NgFor, RouterOutlet, LoadingDirective, AsyncPipe],
})
export class DashboardScenarioPlannerComponent implements OnInit {
    readonly ScenarioEnum = ScenarioEnum;

    public models$: Observable<ModelSimpleDto[]>;
    public scenarios: LookupTableEntry[];
    public scenarioRun$: Observable<ScenarioRunDto>;
    public currentModel$: Observable<ModelSimpleDto>;

    public modelsLinkActive: boolean = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modelService: ModelService,
        private currentScenarioRunService: CurrentScenarioRunService
    ) {}

    ngOnInit(): void {
        this.models$ = this.modelService.modelsGet();
        this.scenarios = Scenarios;
        this.scenarioRun$ = this.currentScenarioRunService.currentScenarioRunObservable;

        this.currentModel$ = combineLatest({ models: this.models$, routerEvent: this.router.events }).pipe(
            filter(({ routerEvent }) => routerEvent instanceof NavigationEnd),
            map(({ models, routerEvent }) => {
                const currentRoute = RouteHelpers.getCurrentRouteFromActivatedRouteSnapshot(this.route.snapshot);
                const modelShortName = currentRoute.paramMap.get(routeParams.modelShortName);
                return models.find((model) => model.ModelShortName == modelShortName);
            })
        );
    }
}
