import { Component } from "@angular/core";
import { IconComponent } from "../../shared/components/icon/icon.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "contact",
    imports: [IconComponent, RouterLink],
    templateUrl: "./contact.component.html",
    styleUrl: "./contact.component.scss"
})
export class ContactComponent {}
