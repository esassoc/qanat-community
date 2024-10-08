import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/models";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf } from "@angular/common";
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";

@Component({
    selector: "dashboard-admin",
    templateUrl: "./dashboard-admin.component.html",
    styleUrls: ["./dashboard-admin.component.scss"],
    standalone: true,
    imports: [RouterLink, RouterLinkActive, RouterOutlet, NgIf, PageHeaderComponent],
})
export class DashboardAdminComponent implements OnInit {
    private currentUser: UserDto;
    public routerLinkActiveOptions = {
        exact: true,
    };

    constructor(private authenticationService: AuthenticationService) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });
    }
}
