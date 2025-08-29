import { Component, OnInit } from "@angular/core";
import { RouterLink } from "@angular/router";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { Observable } from "rxjs";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { AsyncPipe, CommonModule } from "@angular/common";

@Component({
    selector: "activity-center",
    templateUrl: "./activity-center.component.html",
    styleUrls: ["./activity-center.component.scss"],
    imports: [AsyncPipe, PageHeaderComponent, AlertDisplayComponent, RouterLink, CommonModule]
})
export class ActivityCenterComponent implements OnInit {
    public customRichTextTypeID: number = CustomRichTextTypeEnum.ActivityCenter;

    public geography$: Observable<GeographyMinimalDto>;

    constructor(private currentGeographyService: CurrentGeographyService) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
    }
}
