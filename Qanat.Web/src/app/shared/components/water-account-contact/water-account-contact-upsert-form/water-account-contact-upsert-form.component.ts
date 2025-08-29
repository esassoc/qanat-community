import { Component, Input } from "@angular/core";
import { map, Observable } from "rxjs";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "../../forms/form-field/form-field.component";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { WaterAccountContactUpsertDtoForm } from "src/app/shared/generated/model/water-account-contact-upsert-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { AsyncPipe } from "@angular/common";

@Component({
    selector: "water-account-contact-upsert-form",
    imports: [FormsModule, ReactiveFormsModule, AsyncPipe, FormFieldComponent],
    templateUrl: "./water-account-contact-upsert-form.component.html",
    styleUrl: "./water-account-contact-upsert-form.component.scss",
})
export class WaterAccountContactUpsertFormComponent {
    @Input() formGroup: FormGroup<WaterAccountContactUpsertDtoForm>;

    public stateDropdownOptions$: Observable<SelectDropdownOption[]>;
    public showCommunicationPreferenceDropdown: boolean = false;
    public communicationPreferenceDropdownOptions: SelectDropdownOption[] = [
        { Label: "Email", Value: false, disabled: false },
        { Label: "Physical Mail", Value: true, disabled: false },
    ];

    public FormFieldType = FormFieldType;

    constructor(private publicService: PublicService) {}

    ngOnInit(): void {
        this.stateDropdownOptions$ = this.publicService.statesListPublic().pipe(
            map((states) => {
                return states.map(
                    (x) =>
                        ({
                            Value: x.StatePostalCode,
                            Label: x.StatePostalCode,
                        }) as SelectDropdownOption
                );
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
}
