import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { Observable } from "rxjs";
import { GeographyDto } from "../../generated/model/geography-dto";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { PublicService } from "../../generated/api/public.service";

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

    constructor(private publicService: PublicService) {}

    ngOnInit(): void {
        this.geographies$ = this.publicService.publicGeographiesGet();
    }

    public onGeographySelected(geography: GeographyDto) {
        this.geographySelected.emit(geography.GeographyName);
    }
}
