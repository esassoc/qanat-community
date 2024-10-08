import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { NgFor, NgSwitch, NgSwitchCase, AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "geographies",
    templateUrl: "./geographies.component.html",
    styleUrls: ["./geographies.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgFor, NgSwitch, NgSwitchCase, AsyncPipe],
})
export class GeographiesComponent implements OnInit {
    public geographies$: Observable<GeographyDto[]>;
    public richTextTypeID: number = CustomRichTextTypeEnum.OurGeographies;

    constructor(
        private selectedGeographyService: SelectedGeographyService,
        private geographyService: GeographyService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geographies$ = this.geographyService.publicGeographiesGet();
    }

    navigateToGeographyDashboard(geography: GeographyDto) {
        const geographyName = geography.GeographyName.toLowerCase();
        this.selectedGeographyService.selectGeography(geographyName);
        this.router.navigateByUrl(`geographies/${geographyName}`);
    }
}
