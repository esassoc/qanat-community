import { Injectable } from "@angular/core";

import { Observable } from "rxjs";
import { ConfirmService } from "../shared/services/confirm/confirm.service";

export interface IDeactivateComponent {
    canExit: () => Observable<boolean> | Promise<boolean> | boolean;
}

@Injectable({
    providedIn: "root",
})
export class UnsavedChangesGuard {
    constructor(private confirmService: ConfirmService) {}

    public canDeactivate(component: IDeactivateComponent): Observable<boolean> | Promise<boolean> | boolean {
        if (component.canExit && !component.canExit()) {
            return this.confirmService.confirm({
                buttonClassYes: "btn-danger",
                title: "Warning: There are unsaved changes",
                message: "You have unsaved changes on this page. Are you sure you want to leave this page?",
                buttonTextYes: "Proceed",
                buttonTextNo: "Stay on page",
            });
        }

        return Promise.resolve(true);
    }
}
