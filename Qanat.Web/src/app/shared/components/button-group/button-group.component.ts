import { Component, HostBinding, Input, OnInit, ViewEncapsulation } from "@angular/core";

@Component({
    selector: "button-group",
    templateUrl: "./button-group.component.html",
    styleUrls: ["./button-group.component.scss"],
    encapsulation: ViewEncapsulation.None,
    standalone: true,
})
export class ButtonGroupComponent implements OnInit {
    @HostBinding("class.button-group-component") containerClass: boolean = true;

    @HostBinding("class.wrap")
    @Input()
    wrap: boolean = true;

    constructor() {}

    ngOnInit(): void {}
}
