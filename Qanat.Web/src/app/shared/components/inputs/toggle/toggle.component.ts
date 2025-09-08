import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Subscription } from "rxjs";

@Component({
    selector: "toggle",
    templateUrl: "./toggle.component.html",
    styleUrls: ["./toggle.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: ToggleComponent,
        },
    ],
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule],
})
export class ToggleComponent implements OnInit, OnDestroy {
    public isOn = new FormControl<boolean>({ value: false, disabled: false });
    public isOnSubscription: Subscription = Subscription.EMPTY;

    constructor() {}

    ngOnDestroy(): void {
        this.isOnSubscription.unsubscribe();
    }

    ngOnInit(): void {
        this.isOnSubscription = this.isOn.valueChanges.subscribe((val) => (this.value = val));
    }

    // begin ControlValueAccessor
    public disabled = false;
    public touched = false;
    onChange: any = () => {};
    onTouch: any = () => {};
    public val: boolean;

    set value(val: boolean) {
        // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
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
            this.isOn.disable();
        } else {
            this.isOn.enable();
        }
    }

    markAsTouched() {
        if (!this.touched) {
            this.onTouch();
            this.touched = true;
        }
    }
    // end ControlValueAccessor
}
