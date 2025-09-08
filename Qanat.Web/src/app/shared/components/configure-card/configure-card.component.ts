import { Component, EventEmitter, Input, Output } from "@angular/core";
import { CustomRichTextComponent } from "../custom-rich-text/custom-rich-text.component";
import { ModelNameTagComponent } from "../name-tag/name-tag.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "configure-card",
    templateUrl: "./configure-card.component.html",
    styleUrls: ["./configure-card.component.scss"],
    imports: [RouterLink, ModelNameTagComponent, CustomRichTextComponent]
})
export class ConfigureCardComponent {
    @Input() title: string;
    @Input() routerLinkValue: string;
    @Input() customRichTextTypeID: number;

    @Input() toggleChecked: boolean = false;
    @Input() hideToggle: boolean = false;
    @Input() linkDisabled: boolean = false;

    @Output() toggle = new EventEmitter<boolean>();

    constructor() {}

    public onToggle() {
        this.toggleChecked = !this.toggleChecked;
        this.toggle.emit(this.toggleChecked);
    }
}
