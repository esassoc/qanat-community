import { Component, Input } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgFor, NgIf } from "@angular/common";
import { FormInputOption } from "../../forms/form-field/form-field.component";

@Component({
    selector: "select-dropdown",
    templateUrl: "./select-dropdown.component.html",
    styleUrls: ["./select-dropdown.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: SelectDropdownComponent,
            multi: true,
        },
    ],
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule, NgFor, NgIf],
})
export class SelectDropdownComponent {
    @Input() formInputOptions: FormInputOption[];
    @Input() formControl: FormControl;

    public groupedOptions: { GroupName: string; Options: FormInputOption[] }[] = [];

    constructor() {}

    ngOnChanges() {
        this.groupedOptions = null;

        if (this.formInputOptions?.length) {
            // Check if grouping is needed
            const hasGroups = this.formInputOptions.some((option) => option.Group);

            if (hasGroups) {
                this.groupedOptions = this.formInputOptions.reduce((acc, option) => {
                    const groupName = option.Group || "Select"; // Default to "Select" if the item has no group, seems like an okay default and is unlikely to be used as a group name.
                    const group = acc.find((g) => g.GroupName === groupName);
                    if (group) {
                        group.Options.push(option);
                    } else {
                        acc.push({ GroupName: groupName, Options: [option] });
                    }
                    return acc;
                }, [] as { GroupName: string; Options: FormInputOption[] }[]);
            }
        }
    }

    public disabled = false;
    public touched = false;
    onChange: any = () => {};
    onTouch: any = () => {};
    public val: boolean;

    set value(val: boolean) {
        this.val = val;
        this.onChange(val);
        this.onTouch(val);
    }

    writeValue(val: boolean) {
        this.value = val;
    }

    registerOnChange(onChange: any) {
        this.onChange = onChange;
    }

    registerOnTouched(onTouched: any) {
        this.onTouch = onTouched;
    }

    setDisabledState?(isDisabled: boolean) {
        this.disabled = isDisabled;
        if (isDisabled) {
            this.formControl.disable();
        } else {
            this.formControl.enable();
        }
    }

    markAsTouched() {
        if (!this.touched) {
            this.onTouch();
            this.touched = true;
        }
    }
}

export interface SelectDropdownOption extends FormInputOption {}
