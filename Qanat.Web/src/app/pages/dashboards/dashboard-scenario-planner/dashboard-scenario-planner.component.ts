import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router, RouterLinkActive, RouterLink, RouterOutlet } from "@angular/router";
import { Observable, forkJoin } from "rxjs";
import { filter, map, startWith, tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { RouteHelpers } from "src/app/shared/models/router-helpers";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { ViewingGETActionService } from "src/app/shared/services/viewing-get-action.service";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { ModelSimpleDto } from "src/app/shared/generated/model/models";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";

@Component({
    selector: "dashboard-scenario-planner",
    templateUrl: "./dashboard-scenario-planner.component.html",
    styleUrls: ["./dashboard-scenario-planner.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent, RouterLinkActive, RouterLink, NgFor, RouterOutlet, LoadingDirective, AsyncPipe],
})
export class DashboardScenarioPlannerComponent implements OnInit {
    readonly ScenarioEnum = ScenarioEnum;
    public currentAction$: Observable<GETActionDto>;

    public currentModel$: Observable<ModelSimpleDto>;
    public modelsAndScenarios$: Observable<any[]>;
    public modelsLinkActive: boolean = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private getActionService: GETActionService,
        private viewingGETActionService: ViewingGETActionService
    ) {}

    ngOnInit(): void {
        this.currentAction$ = this.viewingGETActionService.currentGetActionObservable;

        this.modelsAndScenarios$ = forkJoin([
            this.getActionService.modelsGet().pipe(
                tap((allModels) => {
                    this.currentModel$ = this.router.events.pipe(
                        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
                        map((e) => this.route.snapshot),
                        startWith(this.route.snapshot),
                        map((routeSnapshot) => {
                            const currentRoute = RouteHelpers.getCurrentRouteFromActivatedRouteSnapshot(routeSnapshot);
                            const modelShortName = currentRoute.paramMap.get(routeParams.modelShortName);

                            const currentModel = allModels.find((model) => model.ModelShortName == modelShortName);
                            return currentModel;
                        })
                    );
                })
            ),
            this.getActionService.scenariosGet(),
        ]);
    }
}
