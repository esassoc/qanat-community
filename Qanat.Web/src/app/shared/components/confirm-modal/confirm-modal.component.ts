import { Component, ComponentRef } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { IModal, ModalService } from "../../services/modal/modal.service";
import { ModalComponent } from "../modal/modal.component";
import { NgIf } from "@angular/common";
import { ConfirmState } from "../../services/confirm/confirm-state";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "confirm-modal",
    templateUrl: "./confirm-modal.component.html",
    styleUrls: ["./confirm-modal.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent],
})
export class ConfirmModalComponent implements IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: ConfirmOptions;

    constructor(
        private state: ConfirmState,
        public sanitizer: DomSanitizer,
        private modalService: ModalService
    ) {
        this.modalContext = state.options;
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.modalService.close(this.modalComponentRef, true);
    }
}
