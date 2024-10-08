import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";

@Component({
    selector: "activity-center",
    templateUrl: "./activity-center.component.html",
    styleUrls: ["./activity-center.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, ButtonComponent, RouterLink],
})
export class ActivityCenterComponent implements OnInit {
    public customRichTextTypeID: number = CustomRichTextTypeEnum.ActivityCenter;
    public geographySlug: string;

    constructor(private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.geographySlug = this.route.snapshot.paramMap.get(routeParams.geographyName);
    }
}
