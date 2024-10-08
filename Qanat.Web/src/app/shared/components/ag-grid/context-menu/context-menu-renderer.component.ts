import { Component, NgZone } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { AgRendererComponent } from "ag-grid-angular";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DropdownToggleDirective } from "../../../directives/dropdown-toggle.directive";
import { NgIf, NgFor } from "@angular/common";

@Component({
    selector: "grid-context-menu-renderer",
    templateUrl: "./context-menu-renderer.component.html",
    styleUrls: ["./context-menu-renderer.component.scss"],
    standalone: true,
    imports: [NgIf, DropdownToggleDirective, IconComponent, NgFor, RouterLink],
})
export class ContextMenuRendererComponent implements AgRendererComponent {
    params: any;

    constructor(
        private ngZone: NgZone,
        private router: Router
    ) {}

    agInit(params: any): void {
        if (params.value) {
            this.params = params;
            this.params.title = params.title || "Actions";
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
