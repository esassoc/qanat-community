import { Component, OnInit } from "@angular/core";
import { RouterOutlet } from "@angular/router";

@Component({
    selector: "user-profile",
    templateUrl: "./user-profile.component.html",
    styleUrls: ["./user-profile.component.scss"],
    standalone: true,
    imports: [RouterOutlet],
})
export class UserProfileComponent implements OnInit {
    constructor() {}

    ngOnInit(): void {}
}
