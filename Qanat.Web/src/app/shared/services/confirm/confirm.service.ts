import { Injectable, ViewContainerRef } from "@angular/core";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { ConfirmOptions } from "./confirm-options";
import { ConfirmState } from "./confirm-state";
import { ConfirmModalComponent } from "../../components/confirm-modal/confirm-modal.component";

/**
 * A confirmation service, allowing to open a confirmation modal from anywhere and get back a promise.
 */
@Injectable({
    providedIn: "root",
})
export class ConfirmService {
    constructor(
        private modalService: ModalService,
        private state: ConfirmState
    ) {}
    /**
     * Opens a confirmation modal
     * @param options the options for the modal (title and message)
     * @returns {Promise<boolean>} a promise that is fulfilled when the user chooses to confirm
     * or closes the modal
     */
    confirm(options: ConfirmOptions, viewContainerRef: ViewContainerRef = null): Promise<boolean> {
        this.state.modal = this.modalService.open(ConfirmModalComponent, viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, options);
        return this.state.modal.instance.result;
    }
}
