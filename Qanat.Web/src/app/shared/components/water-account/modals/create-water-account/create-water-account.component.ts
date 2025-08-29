import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { Observable, map } from "rxjs";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { AsyncPipe } from "@angular/common";
import { WaterAccountUpsertDtoForm, WaterAccountUpsertDtoFormControls } from "src/app/shared/generated/model/water-account-upsert-dto";
import { DialogRef } from "@ngneat/dialog";
import { WaterAccountSimpleDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "create-water-account",
    templateUrl: "./create-water-account.component.html",
    styleUrls: ["./create-water-account.component.scss"],
    imports: [CustomRichTextComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, AsyncPipe],
})
export class CreateWaterAccountComponent implements OnInit {
    public ref: DialogRef<GeographyContext, WaterAccountSimpleDto> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public selectedParcel: ParcelDisplayDto;
    public waterAccountParcels: ParcelDisplayDto[] = [];
    public statesDropdownOptions$: Observable<SelectDropdownOption[]>;

    public showCommunicationPreferenceDropdown: boolean = false;
    public communicationPreferenceDropdownOptions: SelectDropdownOption[] = [
        { Label: "Email", Value: false, disabled: false },
        { Label: "Physical Mail", Value: true, disabled: false },
    ];

    public formGroup = new FormGroup<WaterAccountUpsertDtoForm>({
        WaterAccountName: WaterAccountUpsertDtoFormControls.WaterAccountName(),
        ContactName: WaterAccountUpsertDtoFormControls.ContactName(),
        ContactEmail: WaterAccountUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: WaterAccountUpsertDtoFormControls.ContactPhoneNumber(),
        Address: WaterAccountUpsertDtoFormControls.Address(),
        SecondaryAddress: WaterAccountUpsertDtoFormControls.SecondaryAddress(),
        City: WaterAccountUpsertDtoFormControls.City(),
        State: WaterAccountUpsertDtoFormControls.State(),
        ZipCode: WaterAccountUpsertDtoFormControls.ZipCode(),
        PrefersPhysicalCommunication: WaterAccountUpsertDtoFormControls.PrefersPhysicalCommunication(),
    });

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalCreateNewWaterAccount;

    constructor(
        private alertService: AlertService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.geographyID = this.ref.data.GeographyID;
        this.statesDropdownOptions$ = this.publicService.statesListPublic().pipe(
            map((states) => {
                return states.map((x) => (({
                    Value: x.StatePostalCode,
                    Label: x.StatePostalCode
                }) as SelectDropdownOption));
            })
        );

        this.formGroup.controls.ContactEmail.valueChanges.subscribe((contactEmail) => {
            if (contactEmail) {
                this.showCommunicationPreferenceDropdown = true;
                this.formGroup.controls.PrefersPhysicalCommunication.patchValue(false);
            } else {
                this.showCommunicationPreferenceDropdown = false;
                this.formGroup.controls.PrefersPhysicalCommunication.patchValue(true);
            }
        });
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.waterAccountByGeographyService.createWaterAccountWaterAccountByGeography(this.geographyID, this.formGroup.getRawValue()).subscribe({
            next: (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Water account successfully created.", AlertContext.Success));
                this.ref.close(result);
            },
            error: (error) => {
                this.alertService.pushAlert(new Alert("An error occurred while attempting to create a water account.", AlertContext.Danger));
                this.ref.close(null);
            },
        });
    }
}

export class GeographyContext {
    public GeographyID: number;
}
