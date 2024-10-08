import { Component, Input } from "@angular/core";
import { IconComponent, IconInterface } from "../icon/icon.component";
import { CustomRichTextComponent } from "../custom-rich-text/custom-rich-text.component";

@Component({
    selector: "geography-promo-card",
    templateUrl: "./geography-promo-card.component.html",
    styleUrls: ["./geography-promo-card.component.scss"],
    standalone: true,
    imports: [IconComponent, CustomRichTextComponent],
})
export class GeographyPromoCardComponent {
    @Input() icon: typeof IconInterface;
    @Input() customRichTextTypeID: number;
    @Input() number: number;
    @Input() cardTitle: string;
}
