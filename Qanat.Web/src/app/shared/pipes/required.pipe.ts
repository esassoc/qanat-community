import { Pipe, PipeTransform } from "@angular/core";
import { AbstractControl, FormControl } from "@angular/forms";

@Pipe({
    name: "isRequired",
    standalone: true,
})
export class RequiredPipe implements PipeTransform {
    transform(formControl: FormControl): boolean {
        //  Return when no control or a control without a validator is provided
        if (!formControl || !formControl.validator) {
            return false;
        }

        //  Return the required state of the validator
        const validator = formControl.validator({} as AbstractControl);
        return validator && validator.required;
    }
}
