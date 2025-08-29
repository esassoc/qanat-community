import { Component, ElementRef, ViewChild } from "@angular/core";
import { IAfterGuiAttachedParams, IHeaderParams, SortDirection } from "ag-grid-community";
import { IHeaderAngularComp } from "ag-grid-angular";

import { FieldDefinitionComponent } from "../field-definition/field-definition.component";

interface MyParams extends IHeaderParams {
    menuIcon: string;
}

@Component({
    selector: "field-definition-grid-header",
    templateUrl: "./field-definition-grid-header.component.html",
    styleUrls: ["./field-definition-grid-header.component.scss"],
    imports: [FieldDefinitionComponent],
})
export class FieldDefinitionGridHeaderComponent implements IHeaderAngularComp {
    @ViewChild("header") header: ElementRef;
    public params: any;
    public sorted: SortDirection;
    public filtered: boolean;
    private elementRef: ElementRef;
    public showMenu: boolean = false;

    constructor(elementRef: ElementRef) {
        this.elementRef = elementRef;
    }

    refresh(params: IHeaderParams): boolean {
        return true;
    }

    afterGuiAttached?(params?: IAfterGuiAttachedParams): void {}

    agInit(params: MyParams): void {
        this.params = params;
        this.params.column.minWidth = this.params.column.actualWidth;
    }
}
