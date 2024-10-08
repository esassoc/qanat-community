import { ComponentRef, Injectable } from "@angular/core";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ConfirmOptions } from "./confirm-options";

@Injectable({
    providedIn: "root",
})
export class ConfirmState {
    /**
     * The last options passed ConfirmService.confirm()
     */
    options: ConfirmOptions;

    /**
     * The last opened confirmation modal
     */
    modal: ComponentRef<ModalComponent>;
}
