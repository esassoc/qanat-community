import { Component, OnInit } from "@angular/core";

@Component({
    selector: "unauthenticated",
    templateUrl: "./unauthenticated.component.html",
    styleUrls: ["./unauthenticated.component.scss"],
    standalone: true,
})
export class UnauthenticatedComponent implements OnInit {
    constructor() {}

    ngOnInit() {}
}
