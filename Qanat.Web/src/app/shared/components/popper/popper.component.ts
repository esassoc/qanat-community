import { Component, Input, OnInit, TemplateRef } from "@angular/core";
import { NgIf, NgTemplateOutlet } from "@angular/common";

@Component({
    selector: "popper",
    templateUrl: "./popper.component.html",
    styleUrls: ["./popper.component.scss"],
    standalone: true,
    imports: [NgIf, NgTemplateOutlet],
})
export class PopperComponent implements OnInit {
    @Input() context: TemplateRef<any>;
    @Input() title: TemplateRef<any> | string;

    constructor() {}

    ngOnInit(): void {}

    titleIsTemplate(): boolean {
        return this.title instanceof TemplateRef;
    }
}
