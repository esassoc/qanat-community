import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { WaterAccountCreateDtoForm, WaterAccountCreateDtoFormControls } from "src/app/shared/generated/model/water-account-create-dto";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";

@Component({
    selector: "create-water-account",
    templateUrl: "./create-water-account.component.html",
    styleUrls: ["./create-water-account.component.scss"],
    standalone: true,
    imports: [CustomRichTextComponent, FormsModule, ReactiveFormsModule, FormFieldComponent],
})
export class CreateWaterAccountComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: GeographyContext;

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public selectedParcel: ParcelDisplayDto;
    public waterAccountParcels: ParcelDisplayDto[] = [];

    public formGroup = new FormGroup<WaterAccountCreateDtoForm>({
        WaterAccountName: WaterAccountCreateDtoFormControls.WaterAccountName(),
        ContactName: WaterAccountCreateDtoFormControls.ContactName(),
        ContactAddress: WaterAccountCreateDtoFormControls.ContactAddress(),
    });

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalCreateNewWaterAccount;

    constructor(private modalService: ModalService, private alertService: AlertService, private waterAccountByGeographyService: WaterAccountByGeographyService) {}

    ngOnInit(): void {
        this.geographyID = this.modalContext.geographyID;
    }

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    save() {
        this.alertService.clearAlerts();
        this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsPost(this.geographyID, this.formGroup.getRawValue()).subscribe({
            next: (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Water account successfully created.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, result);
            },
            error: (error) => {
                this.alertService.pushAlert(new Alert("An error occurred while attempting to create a water account.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, null);
            },
        });
    }
}

export class GeographyContext {
    public geographyID: number;
}
