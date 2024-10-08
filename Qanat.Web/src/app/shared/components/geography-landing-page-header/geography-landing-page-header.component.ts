import { Component, Input } from "@angular/core";
import { GeographyDto } from "src/app/shared/generated/model/models";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { PhonePipe } from "../../pipes/phone.pipe";

@Component({
    selector: "geography-landing-page-header",
    templateUrl: "./geography-landing-page-header.component.html",
    styleUrls: ["./geography-landing-page-header.component.scss"],
    standalone: true,
    imports: [GeographyLogoComponent, PhonePipe],
})
export class GeographyLandingPageHeaderComponent {
    @Input() geography: GeographyDto;
}
