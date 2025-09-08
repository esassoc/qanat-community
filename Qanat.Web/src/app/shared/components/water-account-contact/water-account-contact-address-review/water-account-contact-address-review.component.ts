import { Component, Input, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { WaterAccountContactUpsertDto, WaterAccountContactUpsertDtoForm } from "src/app/shared/generated/model/water-account-contact-upsert-dto";
import { NoteComponent } from "../../note/note.component";
import { IconComponent } from "../../icon/icon.component";
import { MapboxResponseDto } from "src/app/shared/generated/model/mapbox-response-dto";

@Component({
    selector: "water-account-contact-address-review",
    templateUrl: "./water-account-contact-address-review.component.html",
    styleUrls: ["./water-account-contact-address-review.component.scss"],
    imports: [FormsModule, ReactiveFormsModule, NoteComponent, IconComponent],
})
export class WaterAccountContactAddressReviewComponent implements OnInit {
    @Input({ required: true }) formGroup: FormGroup<WaterAccountContactUpsertDtoForm>;
    @Input({ required: true }) mapboxResponseDto: MapboxResponseDto;

    public waterAccountContactUpsertDto: WaterAccountContactUpsertDto;

    public isUsingValidatedAddressFormControl: FormControl<boolean> = new FormControl<boolean>(null);
    public showUnvalidatedWarning: boolean = false;
    public confidenceDescriptor: string;

    public noAddressMatch: boolean = false;

    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.WaterAccountContactUpdateModal;
    public isLoadingSubmit: boolean = false;

    constructor() {}

    ngOnInit(): void {
        this.waterAccountContactUpsertDto = this.formGroup.getRawValue();

        this.registerFormGroupChanges();
        this.isUsingValidatedAddressFormControl.setValue(true);

        if (!this.mapboxResponseDto) {
            this.noAddressMatch = true;
            this.isUsingValidatedAddressFormControl.patchValue(false);
            return;
        }

        switch (this.mapboxResponseDto.Confidence) {
            case "Exact":
                this.confidenceDescriptor = "an exact";
                break;
            case "High":
                this.confidenceDescriptor = "a high-confidence";
                break;
            case "Medium":
                this.confidenceDescriptor = "a medium-confidence";
                break;
            case "Low":
                this.confidenceDescriptor = "a low-confidence";
                break;
            default:
                this.confidenceDescriptor = null;
        }
    }

    private registerFormGroupChanges() {
        this.isUsingValidatedAddressFormControl.valueChanges.subscribe((isUsingValidatedAddress: boolean) => {
            if (isUsingValidatedAddress) {
                this.formGroup.controls.Address.patchValue(this.mapboxResponseDto.Address);
                this.formGroup.controls.SecondaryAddress.patchValue(this.mapboxResponseDto.SecondaryAddress);
                this.formGroup.controls.City.patchValue(this.mapboxResponseDto.City);
                this.formGroup.controls.State.patchValue(this.mapboxResponseDto.StatePostalCode);
                this.formGroup.controls.ZipCode.patchValue(this.mapboxResponseDto.ZipCode);
                this.formGroup.controls.AddressValidationJson.patchValue(this.mapboxResponseDto.ResponseJsonString);
                this.formGroup.controls.AddressValidated.patchValue(true);

                this.showUnvalidatedWarning = false;
            } else {
                this.formGroup.controls.Address.patchValue(this.waterAccountContactUpsertDto.Address);
                this.formGroup.controls.SecondaryAddress.patchValue(this.waterAccountContactUpsertDto.SecondaryAddress);
                this.formGroup.controls.City.patchValue(this.waterAccountContactUpsertDto.City);
                this.formGroup.controls.State.patchValue(this.waterAccountContactUpsertDto.State);
                this.formGroup.controls.ZipCode.patchValue(this.waterAccountContactUpsertDto.ZipCode);
                this.formGroup.controls.AddressValidationJson.patchValue(null);
                this.formGroup.controls.AddressValidated.patchValue(false);

                this.showUnvalidatedWarning = true;
            }
        });
    }
}
