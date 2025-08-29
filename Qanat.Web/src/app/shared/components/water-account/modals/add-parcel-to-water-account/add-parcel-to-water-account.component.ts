import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable } from "rxjs";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/models";
import { SearchWaterAccountsComponent } from "../../../search-water-accounts/search-water-accounts.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { AsyncPipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "add-parcel-to-water-account",
    templateUrl: "./add-parcel-to-water-account.component.html",
    styleUrls: ["./add-parcel-to-water-account.component.scss"],
    imports: [CustomRichTextComponent, ParcelIconWithNumberComponent, FormsModule, ReactiveFormsModule, SearchWaterAccountsComponent, AsyncPipe],
})
export class AddParcelToWaterAccountComponent implements OnInit {
    public ref: DialogRef<ParcelContext, boolean> = inject(DialogRef);

    public parcel$: Observable<ParcelMinimalDto>;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalAddParcelToWaterAccount;
    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup<{
        waterAccountID: FormControl<number>;
    }> = new FormGroup({
        waterAccountID: new FormControl<number>(null, [Validators.required]),
    });

    constructor(
        private alertService: AlertService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private parcelService: ParcelService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.getByIDParcel(this.ref.data.ParcelID);
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;

        const waterAccountID = this.formGroup.controls.waterAccountID.value;
        this.waterAccountParcelByWaterAccountService.addOrphanedParcelToWaterAccountWaterAccountParcelByWaterAccount(waterAccountID, this.ref.data.ParcelID).subscribe(
            () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully added parcel to water account", AlertContext.Success));
                this.ref.close(true);
            },
            (error: any) => {
                this.isLoadingSubmit = false;
            },
            () => {
                this.isLoadingSubmit = false;
            }
        );
    }
}

export interface ParcelContext {
    ParcelID: number;
    GeographyID: number;
}
