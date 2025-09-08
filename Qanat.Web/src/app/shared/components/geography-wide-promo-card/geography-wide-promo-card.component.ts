import { Component, Input } from "@angular/core";
import { IconComponent, IconInterface } from "../icon/icon.component";
import { CustomRichTextComponent } from "../custom-rich-text/custom-rich-text.component";

@Component({
    selector: "geography-wide-promo-card",
    templateUrl: "./geography-wide-promo-card.component.html",
    styleUrls: ["./geography-wide-promo-card.component.scss"],
    imports: [IconComponent, CustomRichTextComponent]
})
export class GeographyWidePromoCardComponent {
    @Input() icon: IconInterface;
    @Input() customRichTextTypeID: number;
    @Input() cardTitle: string;
}
