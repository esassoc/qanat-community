import { Component, Input, OnInit } from "@angular/core";
import { ParcelMinimalDto } from "src/app/shared/generated/model/models";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "parcel-icon-with-number",
    templateUrl: "./parcel-icon-with-number.component.html",
    styleUrls: ["./parcel-icon-with-number.component.scss"],
    imports: [IconComponent]
})
export class ParcelIconWithNumberComponent implements OnInit {
    @Input() parcel: ParcelDisplayDto | ParcelMinimalDto;
    @Input() parcelNumber: string;

    public internalParcelNumber: string;

    constructor() {}

    ngOnInit(): void {
        this.internalParcelNumber = this.parcel ? this.parcel.ParcelNumber : this.parcelNumber;
    }
}
