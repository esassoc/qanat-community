import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { NgFor, NgSwitch, NgSwitchCase, AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

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

    constructor(private publicService: PublicService, private router: Router) {}

    ngOnInit(): void {
        this.geographies$ = this.publicService.publicGeographiesGet();
    }

    navigateToGeographyDashboard(geography: GeographyDto) {
        const geographyName = geography.GeographyName.toLowerCase();
        this.router.navigateByUrl(`geographies/${geographyName}`);
    }
}
