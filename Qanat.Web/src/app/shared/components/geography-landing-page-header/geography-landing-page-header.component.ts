import { Component, Input } from "@angular/core";
import { GeographyDto } from "src/app/shared/generated/model/models";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { PhonePipe } from "../../pipes/phone.pipe";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
    selector: "geography-landing-page-header",
    templateUrl: "./geography-landing-page-header.component.html",
    styleUrls: ["./geography-landing-page-header.component.scss"],
    imports: [GeographyLogoComponent, PhonePipe],
})
export class GeographyLandingPageHeaderComponent {
    @Input() geography: GeographyDto;

    constructor(
        private router: Router,
        private route: ActivatedRoute
    ) {}

    public setRouteQueryParams() {
        this.router.navigate(["../request-support"], { relativeTo: this.route, queryParams: { GeographyID: this.geography.GeographyID } });
    }
}
