import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { WaterAccountUpdateDtoForm, WaterAccountUpdateDtoFormControls } from "src/app/shared/generated/model/water-account-update-dto";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "update-water-account-info",
    templateUrl: "./update-water-account-info.component.html",
    styleUrls: ["./update-water-account-info.component.scss"],
    standalone: true,
    imports: [NgIf, CustomRichTextComponent, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, AsyncPipe],
})
export class UpdateWaterAccountInfoComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWaterAccountInformation;
    public waterAccountName: string;
    public waterAccount$: Observable<WaterAccountDto>;
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<WaterAccountUpdateDtoForm>({
        WaterAccountName: WaterAccountUpdateDtoFormControls.WaterAccountName(),
        ContactName: WaterAccountUpdateDtoFormControls.ContactName(),
        ContactAddress: WaterAccountUpdateDtoFormControls.ContactAddress(),
        Notes: WaterAccountUpdateDtoFormControls.Notes(),
    });

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).pipe(
            tap((waterAccount) => {
                this.formGroup.patchValue(waterAccount);
                this.waterAccountName = waterAccount.WaterAccountName;
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.waterAccountService.waterAccountsWaterAccountIDPut(this.modalContext.WaterAccountID, this.formGroup.getRawValue()).subscribe((x) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Contact info successfully updated.", AlertContext.Success));
            this.modalService.close(this.modalComponentRef, x);
        });
    }
}

export interface WaterAccountContext {
    WaterAccountID: number;
    GeographyID: number;
}
