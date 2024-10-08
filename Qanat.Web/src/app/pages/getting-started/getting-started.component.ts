import { Component } from "@angular/core";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "getting-started",
    templateUrl: "./getting-started.component.html",
    styleUrls: ["./getting-started.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent],
})
export class GettingStartedComponent {
    constructor() {}
}
