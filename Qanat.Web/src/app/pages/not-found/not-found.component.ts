import { Component, OnInit } from "@angular/core";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "not-found",
    templateUrl: "./not-found.component.html",
    styleUrls: ["./not-found.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent]
})
export class NotFoundComponent implements OnInit {
    constructor() {}

    ngOnInit() {}
}
