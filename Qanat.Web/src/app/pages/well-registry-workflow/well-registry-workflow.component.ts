import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/models";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";
import { RouterOutlet } from "@angular/router";
import { GeographyLogoComponent } from "../../shared/components/geography-logo/geography-logo.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { WellRegistryWorkflowNavComponent } from "src/app/shared/components/well-registry-workflow-nav/well-registry-workflow-nav.component";
import { WellRegistryWorkflowProgressComponent } from "src/app/shared/components/well-registry-workflow-progress/well-registry-workflow-progress.component";
import { WellRegistryReviewBannerComponent } from "src/app/shared/components/well-registry-review-banner/well-registry-review-banner.component";

@Component({
    selector: "well-registry-workflow",
    templateUrl: "./well-registry-workflow.component.html",
    styleUrls: ["./well-registry-workflow.component.scss"],
    standalone: true,
    imports: [NgIf, GeographyLogoComponent, WellRegistryWorkflowNavComponent, WellRegistryWorkflowProgressComponent, RouterOutlet, WellRegistryReviewBannerComponent, AsyncPipe],
})
export class WellRegistryWorkflowComponent implements OnInit {
    public geography$: Observable<GeographyDto>;

    constructor(private geographyRouteService: GeographyRouteService) {}

    ngOnInit(): void {
        this.geography$ = this.geographyRouteService.geography$;
    }
}
