import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { Observable, filter, map } from "rxjs";
import { GeographyDto } from "../../generated/model/geography-dto";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { AsyncPipe } from "@angular/common";
import { PublicService } from "../../generated/api/public.service";

@Component({
    selector: "geography-selector",
    templateUrl: "./geography-selector.component.html",
    styleUrls: ["./geography-selector.component.scss"],
    imports: [GeographyLogoComponent, AsyncPipe]
})
export class GeographySelectorComponent implements OnInit {
    @Output() geographySelected = new EventEmitter<string>();

    public geographies$: Observable<GeographyDto[]>;

    constructor(private publicService: PublicService) {}

    ngOnInit(): void {
        this.geographies$ = this.publicService.geographiesListPublic().pipe(
            map((geographies) => {
                return geographies.filter((g) => g.LandingPageEnabled).sort((a, b) => a.GeographyName.localeCompare(b.GeographyName));
            })
        );
    }

    public onGeographySelected(geography: GeographyDto) {
        this.geographySelected.emit(geography.GeographyName);
    }
}
