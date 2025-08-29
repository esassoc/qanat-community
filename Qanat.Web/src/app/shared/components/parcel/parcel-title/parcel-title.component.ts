import { CommonModule } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "parcel-title",
    imports: [CommonModule, IconComponent],
    templateUrl: "./parcel-title.component.html",
    styleUrls: ["./parcel-title.component.scss"]
})
export class ParcelTitleComponent implements OnInit {
    @Input() parcelNumber: string;
    @Input() large: boolean = false;
    constructor() {}

    ngOnInit(): void {}
}
