import { Injectable } from "@angular/core";
import { ReplaySubject } from "rxjs";
import { ScenarioRunDto } from "../generated/model/models";

@Injectable({
    providedIn: "root",
})
export class CurrentScenarioRunService {
    private _currentScenarioRun = new ReplaySubject<ScenarioRunDto>(1);
    public currentScenarioRunObservable = this._currentScenarioRun.asObservable();

    constructor() {}

    ngOnInit(): void {}

    public loaded(scenarioRun: ScenarioRunDto): void {
        this._currentScenarioRun.next(scenarioRun);
    }

    public unloaded(): void {
        this._currentScenarioRun.next(null);
    }
}
