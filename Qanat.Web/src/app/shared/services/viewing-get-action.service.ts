import { Injectable } from "@angular/core";
import { ReplaySubject } from "rxjs";
import { GETActionDto } from "../generated/model/get-action-dto";

@Injectable({
    providedIn: "root",
})
export class ViewingGETActionService {
    private _currentGETAction = new ReplaySubject<GETActionDto>(1);
    public currentGetActionObservable = this._currentGETAction.asObservable();

    constructor() {}

    ngOnInit(): void {}

    public loaded(getAction: GETActionDto): void {
        this._currentGETAction.next(getAction);
    }

    public unLoaded(): void {
        this._currentGETAction.next(null);
    }
}
