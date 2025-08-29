import { Component, ContentChild, ElementRef, HostBinding, Input } from "@angular/core";

import { CopyToClipboardDirective } from "src/app/shared/directives/copy-to-clipboard.directive";

@Component({
    selector: "key-value-pair",
    imports: [CopyToClipboardDirective],
    templateUrl: "./key-value-pair.component.html",
    styleUrls: ["./key-value-pair.component.scss"]
})
export class KeyValuePairComponent {
    @Input() key: string;
    @Input() keyValue: string;
    @Input() horizontal: boolean = false;
    @Input() copyValueToClipboard: boolean = false;

    @ContentChild("key") keyElement: ElementRef;
    @ContentChild("value") valueElement: ElementRef;

    @HostBinding("class.horizontal") get isHorizontal() {
        return this.horizontal;
    }
}
