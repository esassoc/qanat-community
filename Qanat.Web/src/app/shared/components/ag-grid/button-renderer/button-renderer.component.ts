import { Component, NgZone } from "@angular/core";
import { Router } from "@angular/router";
import { AgRendererComponent } from "ag-grid-angular";
import { NgIf } from "@angular/common";

@Component({
    selector: "button-renderer",
    templateUrl: "./button-renderer.component.html",
    styleUrls: ["./button-renderer.component.scss"],
    standalone: true,
    imports: [NgIf],
})
export class ButtonRendererComponent implements AgRendererComponent {
    params: any;

    constructor(
        private ngZone: NgZone,
        private router: Router
    ) {}

    agInit(params: any): void {
        if (params.value === null) {
            this.params.value = [{ ActionName: "", ActionHandler: null }];
        } else {
            this.params = params;
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
