import { Component, inject } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";

import { DialogRef } from "@ngneat/dialog";
import { TrustHtmlPipe } from "../../pipes/trust-html.pipe";

@Component({
    selector: "confirm-modal",
    templateUrl: "./confirm-modal.component.html",
    styleUrls: ["./confirm-modal.component.scss"],
    standalone: true,
    imports: [TrustHtmlPipe],
})
export class ConfirmModalComponent {
    public ref: DialogRef<ConfirmOptions, boolean> = inject(DialogRef);
    modalContext: ConfirmOptions;

    constructor(public sanitizer: DomSanitizer) {
        this.modalContext = this.ref.data;
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.ref.close(true);
    }
}
