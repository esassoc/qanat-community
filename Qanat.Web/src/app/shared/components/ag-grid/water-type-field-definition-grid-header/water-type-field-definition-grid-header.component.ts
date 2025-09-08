import { Component, ElementRef, ViewChild } from "@angular/core";
import { IAfterGuiAttachedParams, IHeaderParams } from "ag-grid-community";

import { WaterTypeFieldDefinitionComponent } from "../../water-type-field-definition/water-type-field-definition.component";

interface MyParams extends IHeaderParams {
    menuIcon: string;
}

@Component({
    selector: "water-type-field-definition-grid-header",
    templateUrl: "./water-type-field-definition-grid-header.component.html",
    styleUrls: ["./water-type-field-definition-grid-header.component.scss"],
    imports: [WaterTypeFieldDefinitionComponent],
})
export class WaterTypeFieldDefinitionGridHeaderComponent {
    @ViewChild("header") header: ElementRef;

    public params: any;
    public sorted: string;
    public showMenu: boolean = false;
    public filtered: boolean = false;
    private elementRef: ElementRef;

    constructor(elementRef: ElementRef) {
        this.elementRef = elementRef;
    }

    refresh(params: IHeaderParams): boolean {
        return true;
    }

    afterGuiAttached?(params?: IAfterGuiAttachedParams): void {}

    agInit(params: MyParams): void {
        this.params = params;
    }
}
