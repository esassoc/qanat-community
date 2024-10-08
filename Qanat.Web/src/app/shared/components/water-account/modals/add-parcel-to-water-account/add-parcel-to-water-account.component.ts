import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable } from "rxjs";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto, WaterAccountDto } from "src/app/shared/generated/model/models";
import { SearchWaterAccountsComponent } from "../../../search-water-accounts/search-water-accounts.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { WaterAccountParcelService } from "src/app/shared/generated/api/water-account-parcel.service";

@Component({
    selector: "add-parcel-to-water-account",
    templateUrl: "./add-parcel-to-water-account.component.html",
    styleUrls: ["./add-parcel-to-water-account.component.scss"],
    standalone: true,
    imports: [CustomRichTextComponent, NgIf, ParcelIconWithNumberComponent, FormsModule, ReactiveFormsModule, SearchWaterAccountsComponent, AsyncPipe],
})
export class AddParcelToWaterAccountComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalAddParcelToWaterAccount;
    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup = new FormGroup({
        waterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
    });

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private waterAccountParcelService: WaterAccountParcelService,
        private parcelService: ParcelService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDGet(this.modalContext.ParcelID);
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;

        const waterAccountID = this.formGroup.controls.waterAccount.value.WaterAccountID;
        this.waterAccountParcelService.waterAccountsWaterAccountIDParcelsParcelIDPut(waterAccountID, this.modalContext.ParcelID).subscribe(
            () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully added parcel to water account", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
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
