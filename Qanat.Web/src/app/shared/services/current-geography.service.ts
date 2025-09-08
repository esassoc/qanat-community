import { Injectable } from "@angular/core";
import { BehaviorSubject, filter, Observable, ReplaySubject } from "rxjs";
import { GeographyService } from "../generated/api/geography.service";
import { GeographyMinimalDto } from "../generated/model/geography-minimal-dto";

@Injectable({
    providedIn: "root",
})
export class CurrentGeographyService {
    private geography$ = new BehaviorSubject<GeographyMinimalDto>(null);
    public currentGeography: GeographyMinimalDto = null;

    constructor(private geographyService: GeographyService) {}

    public setCurrentGeography(geography: GeographyMinimalDto): void {
        if (this.currentGeography?.GeographyID === geography?.GeographyID && JSON.stringify(this.currentGeography) !== JSON.stringify(geography)) {
            this.currentGeography = geography;
            this.geography$.next(this.currentGeography);
        } else {
            this.currentGeography = geography;
            this.geography$.next(this.currentGeography);
        }
    }

    public getCurrentGeography(): Observable<GeographyMinimalDto> {
        if (!this.currentGeography) {
            this.geographyService.listForCurrentUserGeography().subscribe((geographies) => {
                this.setCurrentGeography(geographies[0]);
            });
        }

        return this.geography$.asObservable().pipe(filter((geography) => geography !== null));
    }
}
