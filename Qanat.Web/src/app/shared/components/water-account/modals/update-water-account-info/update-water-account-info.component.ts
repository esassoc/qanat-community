import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable } from "rxjs";
import { map, tap } from "rxjs/operators";
import { WaterAccountUpdateDtoForm, WaterAccountUpdateDtoFormControls } from "src/app/shared/generated/model/water-account-update-dto";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { AsyncPipe } from "@angular/common";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "update-water-account-info",
    templateUrl: "./update-water-account-info.component.html",
    styleUrls: ["./update-water-account-info.component.scss"],
    imports: [CustomRichTextComponent, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, AsyncPipe]
})
export class UpdateWaterAccountInfoComponent implements OnInit {
    public ref: DialogRef<WaterAccountContext, WaterAccountDto> = inject(DialogRef);

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWaterAccountInformation;
    public waterAccountName: string;
    public waterAccount$: Observable<WaterAccountDto>;
    public FormFieldType = FormFieldType;
    public statesDropdownOptions$: Observable<SelectDropdownOption[]>;

    public formGroup = new FormGroup<WaterAccountUpdateDtoForm>({
        WaterAccountName: WaterAccountUpdateDtoFormControls.WaterAccountName(),
        Notes: WaterAccountUpdateDtoFormControls.Notes(),
    });

    constructor(
        private alertService: AlertService,
        private waterAccountService: WaterAccountService,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.getByIDWaterAccount(this.ref.data.WaterAccountID).pipe(
            tap((waterAccount) => {
                this.formGroup.patchValue(waterAccount);
                this.waterAccountName = waterAccount.WaterAccountName;
            })
        );
        this.statesDropdownOptions$ = this.publicService.statesListPublic().pipe(
            map((states) => {
                return states.map((x) => (({
                    Value: x.StatePostalCode,
                    Label: x.StatePostalCode
                }) as SelectDropdownOption));
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.waterAccountService.updateWaterAccount(this.ref.data.WaterAccountID, this.formGroup.getRawValue()).subscribe((x) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Water account info successfully updated.", AlertContext.Success));
            this.ref.close(x);
        });
    }
}

export interface WaterAccountContext {
    WaterAccountID: number;
    GeographyID: number;
}
