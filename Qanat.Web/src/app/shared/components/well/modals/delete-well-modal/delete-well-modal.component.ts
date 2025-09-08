import { Component, ComponentRef, inject } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "delete-well-modal",
    templateUrl: "./delete-well-modal.component.html",
    styleUrls: ["./delete-well-modal.component.scss"],
    imports: [IconComponent],
})
export class DeleteWellModalComponent {
    public ref: DialogRef<WellContext, boolean> = inject(DialogRef);

    constructor(
        private alertService: AlertService,
        private wellRegistrationService: WellRegistrationService
    ) {}

    close() {
        this.ref.close(false);
    }

    save() {
        this.wellRegistrationService.deleteWellRegistrationWellRegistration(this.ref.data.WellID).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Deleted well successfully.", AlertContext.Success));
                this.ref.close(true);
            },
            error: () => {
                this.alertService.pushAlert(new Alert("There was an issue deleting this well."));
                this.ref.close(false);
            },
        });
    }
}

export interface WellContext {
    WellID: number;
    UpdatingTechnicalInfo: boolean;
}
