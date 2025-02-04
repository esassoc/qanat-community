import { Component } from "@angular/core";
import { IconComponent } from "../../shared/components/icon/icon.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "help",
    standalone: true,
    imports: [IconComponent, RouterLink],
    templateUrl: "./help.component.html",
    styleUrl: "./help.component.scss",
})
export class HelpComponent {
    icon: any;
    geography: any;
}
