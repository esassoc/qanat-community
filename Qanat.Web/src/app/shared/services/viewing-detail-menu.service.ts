import { Injectable } from "@angular/core";
import { ReplaySubject } from "rxjs";
import { ParcelDetailDto, WaterAccountDto, WellDetailDto } from "../generated/model/models";

@Injectable({
    providedIn: "root",
})
export class ViewingDetailMenuService {
    private _currentParcel = new ReplaySubject<ParcelDetailDto>(1);
    public currentParcel$ = this._currentParcel.asObservable();

    private _currentWaterAccount = new ReplaySubject<WaterAccountDto>(1);
    public currentWaterAccount$ = this._currentWaterAccount.asObservable();

    private _currentWell = new ReplaySubject<WellDetailDto>(1);
    public currentWell$ = this._currentWell.asObservable();

    constructor() {}

    public loadedParcel(parcel: ParcelDetailDto): void {
        this._currentParcel.next(parcel);
    }

    public unLoadedParcel(): void {
        this._currentParcel.next(null);
    }

    public loadedWaterAccount(waterAccount: WaterAccountDto): void {
        this._currentWaterAccount.next(waterAccount);
    }

    public unLoadedWaterAccount(): void {
        this._currentWaterAccount.next(null);
    }

    public loadedWell(well: WellDetailDto): void {
        this._currentWell.next(well);
    }

    public unLoadedWell(): void {
        this._currentWell.next(null);
    }
}
