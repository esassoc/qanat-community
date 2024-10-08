import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IconComponent } from "../../icon/icon.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "workflow-nav-item",
    standalone: true,
    imports: [CommonModule, IconComponent, RouterLink],
    templateUrl: "./workflow-nav-item.component.html",
    styleUrls: ["./workflow-nav-item.component.scss"],
})
export class WorkflowNavItemComponent {
    @Input() navRouterLink: string | string[];
    @Input() complete: boolean = false;
    @Input() disabled: boolean = false;
    @Input() required: boolean = true;

    constructor() {}
}
