import { Component, ComponentRef, OnInit } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { Observable } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "delete-water-account",
    templateUrl: "./delete-water-account.component.html",
    styleUrls: ["./delete-water-account.component.scss"],
    standalone: true,
    imports: [NgIf, IconComponent, AsyncPipe],
})
export class DeleteWaterAccountComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;
    public waterAccount$: Observable<WaterAccountDto>;

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID);
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.waterAccountService.waterAccountsWaterAccountIDDelete(this.modalContext.WaterAccountID).subscribe(
            (results) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Deleted water account successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("There was an issue deleting this water account."));
                this.modalService.close(this.modalComponentRef, false);
            }
        );
    }
}
