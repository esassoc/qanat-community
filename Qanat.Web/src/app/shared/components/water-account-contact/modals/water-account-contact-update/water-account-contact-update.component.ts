import { Component, inject, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { DialogRef } from "@ngneat/dialog";
import { WaterAccountContactService } from "src/app/shared/generated/api/water-account-contact.service";
import { WaterAccountContactUpsertDtoForm, WaterAccountContactUpsertDtoFormControls } from "src/app/shared/generated/model/water-account-contact-upsert-dto";
import { WaterAccountContactByGeographyService } from "src/app/shared/generated/api/water-account-contact-by-geography.service";
import { WaterAccountContactUpsertFormComponent } from "../../water-account-contact-upsert-form/water-account-contact-upsert-form.component";
import { Observable, of, tap } from "rxjs";
import { WaterAccountContactDto } from "src/app/shared/generated/model/water-account-contact-dto";
import { AsyncPipe } from "@angular/common";
import { NoteComponent } from "../../../note/note.component";
import { IconComponent } from "../../../icon/icon.component";
import { MapboxResponseDto } from "src/app/shared/generated/model/mapbox-response-dto";
import { WaterAccountContactAddressReviewComponent } from "src/app/shared/components/water-account-contact/water-account-contact-address-review/water-account-contact-address-review.component";

@Component({
    selector: "water-account-contact-update",
    templateUrl: "./water-account-contact-update.component.html",
    styleUrls: ["./water-account-contact-update.component.scss"],
    imports: [FormsModule, ReactiveFormsModule, WaterAccountContactUpsertFormComponent, AsyncPipe, NoteComponent, IconComponent, WaterAccountContactAddressReviewComponent],
})
export class WaterAccountContactUpdateComponent implements OnInit {
    public ref: DialogRef<WaterAccountContactContext> = inject(DialogRef);

    public waterAccountContact$: Observable<WaterAccountContactDto>;
    public waterAccountContact: WaterAccountContactDto;

    public isCreating: boolean;

    public formGroup = new FormGroup<WaterAccountContactUpsertDtoForm>({
        ContactName: WaterAccountContactUpsertDtoFormControls.ContactName(),
        ContactEmail: WaterAccountContactUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: WaterAccountContactUpsertDtoFormControls.ContactPhoneNumber(),
        Address: WaterAccountContactUpsertDtoFormControls.Address(),
        SecondaryAddress: WaterAccountContactUpsertDtoFormControls.SecondaryAddress(),
        City: WaterAccountContactUpsertDtoFormControls.City(),
        State: WaterAccountContactUpsertDtoFormControls.State(),
        ZipCode: WaterAccountContactUpsertDtoFormControls.ZipCode(),
        PrefersPhysicalCommunication: WaterAccountContactUpsertDtoFormControls.PrefersPhysicalCommunication(),
        AddressValidated: WaterAccountContactUpsertDtoFormControls.AddressValidated(false),
        AddressValidationJson: WaterAccountContactUpsertDtoFormControls.AddressValidationJson(),
    });

    public isReviewingAddress: boolean;
    public mapboxResponseDto: MapboxResponseDto;

    public isLoadingSubmit: boolean = false;

    constructor(
        private alertService: AlertService,
        private waterAccountContactService: WaterAccountContactService,
        private waterAccountContactByGeographyService: WaterAccountContactByGeographyService
    ) {}

    ngOnInit(): void {
        this.isCreating = this.ref.data.WaterAccountContactID ? false : true;

        if (this.ref.data.WaterAccountContactID) {
            this.getWaterAccountContactData();
        } else {
            this.waterAccountContact$ = of(new WaterAccountContactDto());
            this.isCreating = true;
        }
    }

    public getWaterAccountContactData() {
        this.waterAccountContact$ = this.waterAccountContactService.getByIDWaterAccountContact(this.ref.data.WaterAccountContactID).pipe(
            tap((waterAccountContact) => {
                this.waterAccountContact = waterAccountContact;

                this.formGroup.setValue({
                    ContactName: waterAccountContact.ContactName,
                    ContactEmail: waterAccountContact.ContactEmail,
                    ContactPhoneNumber: waterAccountContact.ContactPhoneNumber,
                    Address: waterAccountContact.Address,
                    SecondaryAddress: waterAccountContact.SecondaryAddress,
                    City: waterAccountContact.City,
                    State: waterAccountContact.State,
                    ZipCode: waterAccountContact.ZipCode,
                    PrefersPhysicalCommunication: waterAccountContact.PrefersPhysicalCommunication,
                    AddressValidated: waterAccountContact.AddressValidated,
                    AddressValidationJson: waterAccountContact.AddressValidationJson,
                });
            })
        );
    }

    public close() {
        this.ref.close(null);
    }

    public backToContactForm() {
        this.isReviewingAddress = false;
        this.mapboxResponseDto = null;
    }

    public save() {
        if (this.isReviewingAddress) {
            this.saveWaterAccountContact();
        } else {
            this.validateAddress();
        }
    }

    public validateAddress() {
        this.isLoadingSubmit = true;

        var waterAccountContactUpsertDto = this.formGroup.getRawValue();
        if (
            this.waterAccountContact &&
            this.waterAccountContact.AddressValidated &&
            this.waterAccountContact.Address == waterAccountContactUpsertDto.Address &&
            this.waterAccountContact.SecondaryAddress == waterAccountContactUpsertDto.SecondaryAddress &&
            this.waterAccountContact.City == waterAccountContactUpsertDto.City &&
            this.waterAccountContact.State == waterAccountContactUpsertDto.State &&
            this.waterAccountContact.ZipCode == waterAccountContactUpsertDto.ZipCode
        ) {
            this.saveWaterAccountContact();
            return;
        }

        this.waterAccountContactByGeographyService.validateAddressWaterAccountContactByGeography(this.ref.data.GeographyID, waterAccountContactUpsertDto).subscribe({
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

    public saveWaterAccountContact() {
        this.isLoadingSubmit = true;

        var request = this.isCreating
            ? this.waterAccountContactByGeographyService.createWaterAccountContactByGeography(this.ref.data.GeographyID, this.formGroup.getRawValue())
            : this.waterAccountContactService.updateWaterAccountContact(this.ref.data.WaterAccountContactID, this.formGroup.getRawValue());

        request.subscribe({
            next: (x) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Water account contact successfully ${this.isCreating ? "created" : "updated"}!`, AlertContext.Success));
                this.ref.close(x);
            },
            error: () => {
                this.ref.close(null);
            },
        });
    }
}

export class WaterAccountContactContext {
    WaterAccountContactID?: number;
    WaterAccountID?: number;
    GeographyID?: number;
    WaterAccountContactName?: string = null;
}
