import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import { NgFor } from "@angular/common";
@Component({
    selector: "btn-group-radio-input",
    templateUrl: "./btn-group-radio-input.component.html",
    styleUrls: ["./btn-group-radio-input.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: BtnGroupRadioInputComponent,
            multi: true,
        },
    ],
    standalone: true,
    imports: [NgFor],
})
export class BtnGroupRadioInputComponent implements OnInit {
    public uniqueName: string = crypto.randomUUID();
    @Input() label: string;
    @Input() options: IBtnGroupRadioInputOption[] = [];
    @Output() change = new EventEmitter<string>();
    @Input() required: boolean = false;

    public val: any;
    set value(val) {
        // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
        this.val = val;
        this.change.emit(val);

        this.onChange(val);
        this.onTouch(val);
    }

    public isDisabled: boolean = false;

    onChange: any = () => {};
    onTouch: any = () => {};

    constructor() {}

    writeValue(value: any): void {
        this.val = value;
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouch = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    ngOnInit(): void {}
}

export interface IBtnGroupRadioInputOption {
    label: string;
    value: any;
}
