import { Component } from "@angular/core";
import { IconComponent } from "../../shared/components/icon/icon.component";

@Component({
    selector: "acknowledgements",
    templateUrl: "./acknowledgements.component.html",
    styleUrl: "./acknowledgements.component.scss",
    standalone: true,
    imports: [IconComponent],
})
export class AcknowledgementsComponent {}
