import { Component, EventEmitter, Input, Output } from "@angular/core";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WaterAccountRequestChangesParcelDto } from "../../../generated/model/water-account-request-changes-parcel-dto";

@Component({
    selector: "parcel-list-item",
    standalone: true,
    imports: [IconComponent],
    templateUrl: "./parcel-list-item.component.html",
    styleUrl: "./parcel-list-item.component.scss",
})
export class ParcelListItemComponent {
    @Input() parcel: WaterAccountRequestChangesParcelDto;
    @Input() titleText: string = "Remove Parcel";
    @Output() removed = new EventEmitter();

    public onRemove() {
        this.removed.emit();
    }
}
