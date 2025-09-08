import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { NgClass } from "@angular/common";

@Component({
    selector: "fresca-button",
    templateUrl: "./button.component.html",
    styleUrls: ["./button.component.scss"],
    imports: [NgClass]
})
export class ButtonComponent implements OnInit {
    @Input() type: string = "button";
    @Input() disabled: boolean = false;
    @Input() iconClass: string = "fa fa-long-arrow-right icon-right";
    @Input() cssClass: string = "btn btn-primary";
    @Output() onClick = new EventEmitter<any>();

    constructor() {}

    ngOnInit(): void {}

    onClickButton(event) {
        this.onClick.emit(event);
    }
}
