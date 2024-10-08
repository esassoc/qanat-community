import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { Observable } from "rxjs";
import { GeographyDto } from "../../generated/model/geography-dto";
import { GeographyService } from "../../generated/api/geography.service";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";

@Component({
    selector: "geography-selector",
    templateUrl: "./geography-selector.component.html",
    styleUrls: ["./geography-selector.component.scss"],
    standalone: true,
    imports: [NgIf, NgFor, GeographyLogoComponent, AsyncPipe],
})
export class GeographySelectorComponent implements OnInit {
    @Output() geographySelected = new EventEmitter<string>();

    public geographies$: Observable<GeographyDto[]>;

    constructor(private geographyService: GeographyService) {}

    ngOnInit(): void {
        this.geographies$ = this.geographyService.publicGeographiesGet();
    }

    public onGeographySelected(geography: GeographyDto) {
        this.geographySelected.emit(geography.GeographyName);
    }
}
