import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
    selector: "no-selected-item-box",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./no-selected-item-box.component.html",
    styleUrls: ["./no-selected-item-box.component.scss"],
})
export class NoSelectedItemBoxComponent {
    @Input() boxText: string = "No Item Selected";
    @Input() boxHeight: number = 300;
}
