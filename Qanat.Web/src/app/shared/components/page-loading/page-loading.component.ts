import { Component, OnInit } from "@angular/core";
import { LoadingDirective } from "../../directives/loading.directive";

@Component({
    selector: "page-loading",
    templateUrl: "./page-loading.component.html",
    styleUrls: ["./page-loading.component.scss"],
    standalone: true,
    imports: [LoadingDirective],
})
export class PageLoadingComponent implements OnInit {
    constructor() {}

    ngOnInit(): void {}
}
