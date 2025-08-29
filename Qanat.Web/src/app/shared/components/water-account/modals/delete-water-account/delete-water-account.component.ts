import { Component, inject, OnInit } from "@angular/core";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { Observable } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "delete-water-account",
    templateUrl: "./delete-water-account.component.html",
    styleUrls: ["./delete-water-account.component.scss"],
    imports: [IconComponent, AsyncPipe]
})
export class DeleteWaterAccountComponent implements OnInit {
    public ref: DialogRef<WaterAccountContext, boolean> = inject(DialogRef);
    public waterAccount$: Observable<WaterAccountDto>;

    constructor(
        private alertService: AlertService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.getByIDWaterAccount(this.ref.data.WaterAccountID);
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.waterAccountService.deleteWaterAccount(this.ref.data.WaterAccountID).subscribe(
            (results) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Deleted water account successfully.", AlertContext.Success));
                this.ref.close(true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("There was an issue deleting this water account."));
                this.ref.close(false);
            }
        );
    }
}
