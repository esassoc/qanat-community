import { Component, Input } from "@angular/core";
import { IconComponent, IconInterface } from "../icon/icon.component";
import { CustomRichTextComponent } from "../custom-rich-text/custom-rich-text.component";

@Component({
    selector: "rich-link",
    templateUrl: "./rich-link.component.html",
    styleUrls: ["./rich-link.component.scss"],
    standalone: true,
    imports: [IconComponent, CustomRichTextComponent],
})
export class RichLinkComponent {
    @Input() customRichTextTypeID: number;
    @Input() icon: typeof IconInterface;
    @Input() cardTitle: string;
}
