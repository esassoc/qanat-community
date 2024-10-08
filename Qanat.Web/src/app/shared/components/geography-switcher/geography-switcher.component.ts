import { Component, Input, OnInit } from "@angular/core";
import { Observable, map, mergeMap } from "rxjs";
import { GeographyDto, UserDto } from "../../generated/model/models";
import { SelectedGeographyService } from "../../services/selected-geography.service";
import { DropdownToggleDirective } from "../../directives/dropdown-toggle.directive";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "geography-switcher",
    templateUrl: "./geography-switcher.component.html",
    styleUrls: ["./geography-switcher.component.scss"],
    standalone: true,
    imports: [NgIf, DropdownToggleDirective, IconComponent, NgFor, AsyncPipe],
})
export class GeographySwitcherComponent implements OnInit {
    @Input() currentUser: UserDto[];

    public geography$: Observable<{ currentSelectedGeography: GeographyDto; currentUserAvailableGeographies: GeographyDto[] }>;
    public currentUserAvailableGeographies$: Observable<GeographyDto[]>;

    constructor(private selectedGeographyService: SelectedGeographyService) {}

    ngOnInit(): void {
        this.geography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.pipe(
            mergeMap((changedGeography) => {
                return this.selectedGeographyService.curentUserGeographiesObservable.pipe(
                    map((allGeographies) => {
                        return {
                            currentSelectedGeography: changedGeography,
                            currentUserAvailableGeographies: allGeographies.filter((geography) => geography.GeographyID !== changedGeography.GeographyID),
                        };
                    })
                );
            })
        );
    }

    changeGeography(geographyName: string) {
        this.selectedGeographyService.selectGeography(geographyName);
    }
}
