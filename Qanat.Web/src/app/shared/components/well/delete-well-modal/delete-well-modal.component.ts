import { Component, ComponentRef } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "delete-well-modal",
    templateUrl: "./delete-well-modal.component.html",
    styleUrls: ["./delete-well-modal.component.scss"],
    standalone: true,
    imports: [IconComponent],
})
export class DeleteWellModalComponent {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WellContext;

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private wellRegistrationService: WellRegistrationService
    ) {}

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.wellRegistrationService.wellRegistrationsWellRegistrationIDDelete(this.modalContext.WellID).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Deleted well successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            error: () => {
                this.alertService.pushAlert(new Alert("There was an issue deleting this well."));
                this.modalService.close(this.modalComponentRef, false);
            },
        });
    }
}

export interface WellContext {
    WellID: number;
}
