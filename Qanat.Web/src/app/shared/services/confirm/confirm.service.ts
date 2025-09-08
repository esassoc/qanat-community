import { Injectable, ViewContainerRef } from "@angular/core";
import { ConfirmOptions } from "./confirm-options";
import { ConfirmModalComponent } from "../../components/confirm-modal/confirm-modal.component";
import { DialogService } from "@ngneat/dialog";
import { firstValueFrom } from "rxjs";

/**
 * A confirmation service, allowing to open a confirmation modal from anywhere and get back a promise.
 */
@Injectable({
    providedIn: "root",
})
export class ConfirmService {
    constructor(private dialogService: DialogService) {}
    /**
     * Opens a confirmation modal
     * @param options the options for the modal (title and message)
     * @returns {Promise<boolean>} a promise that is fulfilled when the user chooses to confirm
     * or closes the modal
     */
    confirm(options: ConfirmOptions, viewContainerRef: ViewContainerRef = null): Promise<boolean> {
        const dialogRef = this.dialogService.open(ConfirmModalComponent, {
            data: {
                title: options.title,
                message: options.message,
                icon: options.icon,
                buttonTextYes: options.buttonTextYes,
                buttonTextNo: options.buttonTextNo,
                buttonClassYes: options.buttonClassYes,
            },
            size: "sm",
        });

        return firstValueFrom(dialogRef.afterClosed$).then((result) => {
            if (result) {
                return true;
            }
            return false;
        });
    }
}
