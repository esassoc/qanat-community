import { Component, ComponentRef, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
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
    modalContext: WaterAccountContext;

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

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit(): void {
        this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
        });
    }

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    save() {
        this.alertService.clearAlerts();
        this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsPost(this.geographyID, this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Created new water account successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, result);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create the water account.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }
}
