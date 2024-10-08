import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";

@Component({
    selector: "qanat-create-user-callback",
    templateUrl: "./create-user-callback.component.html",
    styleUrls: ["./create-user-callback.component.scss"],
    standalone: true,
})
export class CreateUserCallbackComponent implements OnInit {
    constructor(private authenticationService: AuthenticationService) {}

    ngOnInit() {
        //this.authenticationService.login();
    }
}
