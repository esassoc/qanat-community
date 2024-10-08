import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "water-account-title",
    standalone: true,
    imports: [CommonModule, IconComponent],
    templateUrl: "./water-account-title.component.html",
    styleUrls: ["./water-account-title.component.scss"],
})
export class WaterAccountTitleComponent {
    @Input() waterAccountName: string;
    @Input() waterAccountNumber: string;

    @Input() nameLength: number = null;
    @Input() small: boolean = false;

    constructor() {}
}
