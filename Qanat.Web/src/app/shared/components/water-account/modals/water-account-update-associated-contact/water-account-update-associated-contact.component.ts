import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { DialogRef } from "@ngneat/dialog";
import { WaterAccountContactByGeographyService } from "src/app/shared/generated/api/water-account-contact-by-geography.service";
import { SearchWaterAccountContactsComponent } from "../../../search-water-account-contacts/search-water-account-contacts.component";
import { WaterAccountContactContext } from "../../../water-account-contact/modals/water-account-contact-update/water-account-contact-update.component";
import { WaterAccountContactUpsertFormComponent } from "../../../water-account-contact/water-account-contact-upsert-form/water-account-contact-upsert-form.component";
import { NoteComponent } from "../../../note/note.component";
import { IconComponent } from "../../../icon/icon.component";
import { WaterAccountContactUpsertDtoForm, WaterAccountContactUpsertDtoFormControls } from "src/app/shared/generated/model/water-account-contact-upsert-dto";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountWaterAccountContactUpdateDtoForm } from "src/app/shared/generated/model/water-account-water-account-contact-update-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WaterAccountContactSearchResultDto } from "src/app/shared/generated/model/water-account-contact-search-result-dto";
import { MapboxResponseDto } from "src/app/shared/generated/model/mapbox-response-dto";
import { WaterAccountContactAddressReviewComponent } from "src/app/shared/components/water-account-contact/water-account-contact-address-review/water-account-contact-address-review.component";

@Component({
    selector: "water-account-update-associated-contact",
    templateUrl: "./water-account-update-associated-contact.component.html",
    styleUrls: ["./water-account-update-associated-contact.component.scss"],
    imports: [
        CustomRichTextComponent,
        FormsModule,
        ReactiveFormsModule,
        SearchWaterAccountContactsComponent,
        WaterAccountContactUpsertFormComponent,
        NoteComponent,
        IconComponent,
        WaterAccountContactAddressReviewComponent,
    ],
})
export class WaterAccountUpdateAssociatedContactComponent implements OnInit {
    public ref: DialogRef<WaterAccountContactContext> = inject(DialogRef);

    public activeTab: "SelectExistingContact" | "CreateNewContact" = "SelectExistingContact";
    public currentFormGroup: FormGroup;

    public existingContactFormGroup = new FormGroup<WaterAccountWaterAccountContactUpdateDtoForm>({
        WaterAccountContactID: new FormControl<number>(null, Validators.required),
    });

    public newContactFormGroup = new FormGroup<WaterAccountContactUpsertDtoForm>({
        ContactName: WaterAccountContactUpsertDtoFormControls.ContactName(),
        ContactEmail: WaterAccountContactUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: WaterAccountContactUpsertDtoFormControls.ContactPhoneNumber(),
        Address: WaterAccountContactUpsertDtoFormControls.Address(),
        SecondaryAddress: WaterAccountContactUpsertDtoFormControls.SecondaryAddress(),
        City: WaterAccountContactUpsertDtoFormControls.City(),
        State: WaterAccountContactUpsertDtoFormControls.State(),
        ZipCode: WaterAccountContactUpsertDtoFormControls.ZipCode(),
        PrefersPhysicalCommunication: WaterAccountContactUpsertDtoFormControls.PrefersPhysicalCommunication(),
        WaterAccountID: WaterAccountContactUpsertDtoFormControls.WaterAccountID(),
        AddressValidated: WaterAccountContactUpsertDtoFormControls.AddressValidated(false),
        AddressValidationJson: WaterAccountContactUpsertDtoFormControls.AddressValidationJson(),
    });

    public isReviewingAddress: boolean;
    public mapboxResponseDto: MapboxResponseDto;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalUpdateWaterAccountContactInfo;
    public isLoadingSubmit: boolean = false;

    constructor(
        private alertService: AlertService,
        private waterAccountContactByGeographyService: WaterAccountContactByGeographyService,
        private waterAccountService: WaterAccountService
    ) {}

    public ngOnInit(): void {
        this.newContactFormGroup.controls.WaterAccountID.setValue(this.ref.data.WaterAccountID);
        this.currentFormGroup = this.existingContactFormGroup;
    }

    public setActiveTab(tab: "SelectExistingContact" | "CreateNewContact") {
        this.activeTab = tab;

        if (tab === "SelectExistingContact") {
            this.currentFormGroup = this.existingContactFormGroup;
        } else {
            this.currentFormGroup = this.newContactFormGroup;
        }
    }

    public updateSelectedContact(waterAccountContact: WaterAccountContactSearchResultDto) {
        if (!waterAccountContact) {
            return;
        }

        this.existingContactFormGroup.controls.WaterAccountContactID.setValue(waterAccountContact.WaterAccountContactID);
    }

    public close() {
        this.ref.close(null);
    }

    public save() {
        this.isLoadingSubmit = true;

        if (this.activeTab === "SelectExistingContact") {
            this.saveWithExistingContact();
        } else {
            this.saveWithNewContact();
        }
    }

    private saveWithExistingContact() {
        this.waterAccountService.updateWaterAccountContactWaterAccount(this.ref.data.WaterAccountID, this.existingContactFormGroup.getRawValue()).subscribe({
            next: (x) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Water account contact successfully updated!`, AlertContext.Success));
                this.ref.close(x);
            },
            error: () => {
                this.ref.close(null);
            },
        });
    }

    public saveWithNewContact() {
        if (this.isReviewingAddress) {
            this.saveWithNewContactAndValidatedAddress();
        } else {
            this.validateAddress();
        }
    }

    public validateAddress() {
        this.isLoadingSubmit = true;

        this.waterAccountContactByGeographyService.validateAddressWaterAccountContactByGeography(this.ref.data.GeographyID, this.newContactFormGroup.getRawValue()).subscribe({
            next: (response) => {
                this.isLoadingSubmit = false;

                this.mapboxResponseDto = response;
                this.isReviewingAddress = true;
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }

    public backToContactForm() {
        this.isReviewingAddress = false;
        this.mapboxResponseDto = null;
    }

    private saveWithNewContactAndValidatedAddress() {
        this.waterAccountContactByGeographyService.createWaterAccountContactByGeography(this.ref.data.GeographyID, this.newContactFormGroup.getRawValue()).subscribe({
            next: (x) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Water account contact successfully added!`, AlertContext.Success));
                this.ref.close(x);
            },
            error: () => {
                this.ref.close(null);
            },
        });
    }
}
