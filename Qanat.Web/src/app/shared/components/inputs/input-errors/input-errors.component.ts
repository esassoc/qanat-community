import { Component, Input, OnInit } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { Observable } from "rxjs";
import { map, startWith } from "rxjs/operators";
import { NoteComponent } from "../../note/note.component";
import { NgFor, NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "input-errors",
    templateUrl: "./input-errors.component.html",
    styleUrls: ["./input-errors.component.scss"],
    standalone: true,
    imports: [NgFor, NgIf, NoteComponent, AsyncPipe],
})
export class InputErrorsComponent implements OnInit {
    @Input() validateFormControl: FormControl<any>;
    public errors$: Observable<string[]>;

    constructor() {}

    ngOnInit(): void {
        // listen in the input value changes and map any errors
        this.errors$ = this.validateFormControl.valueChanges.pipe(
            startWith((x) => null),
            map((x) => {
                // errors is null when there are no errors
                if (!this.validateFormControl.errors) return [];
                const errors = Object.keys(this.validateFormControl.errors).map((key) => {
                    switch (key) {
                        case Validators.min.name.toLowerCase(): {
                            const min = this.validateFormControl.errors[key].min;
                            return `This field requires a minimum value of ${min}.`;
                        }
                        case Validators.max.name.toLowerCase(): {
                            const max = this.validateFormControl.errors[key].max;
                            return `This field requires a max value of ${max}.`;
                        }
                        case Validators.minLength.name.toLowerCase(): {
                            const requiredLength = this.validateFormControl.errors[key].requiredLength;
                            return `This field requires a minimum length of ${requiredLength}.`;
                        }
                        case Validators.maxLength.name.toLowerCase(): {
                            const requiredLength = this.validateFormControl.errors[key].requiredLength;
                            return `This field has a maximum length of ${requiredLength}.`;
                        }
                        case Validators.required.name.toLowerCase(): {
                            return `This field is required.`;
                        }
                        case Validators.requiredTrue.name.toLowerCase(): {
                            return `This field must be selected.`;
                        }
                        case Validators.email.name.toLowerCase(): {
                            return `This field requires a valid Email Address.`;
                        }
                        case Validators.pattern.name.toLowerCase(): {
                            const requiredPattern = this.validateFormControl.errors[key].requiredPattern;
                            return `This field did not match the requred pattern "${requiredPattern}".`;
                        }
                    }
                });

                return errors;
            })
        );
    }
}
