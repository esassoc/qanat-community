import { Component, ElementRef, ViewChild } from "@angular/core";
import { IAfterGuiAttachedParams, IHeaderParams, SortDirection } from "ag-grid-community";
import { IHeaderAngularComp } from "ag-grid-angular";
import { NgIf } from "@angular/common";
import { FieldDefinitionComponent } from "../field-definition/field-definition.component";

interface MyParams extends IHeaderParams {
    menuIcon: string;
}

@Component({
    selector: "field-definition-grid-header",
    templateUrl: "./field-definition-grid-header.component.html",
    styleUrls: ["./field-definition-grid-header.component.scss"],
    standalone: true,
    imports: [FieldDefinitionComponent, NgIf],
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
        //because of the way the popover sits and how it's triggered, it's best to just prevent the column from covering it
        //TODO make the css here more act more like the default ag-grid css
        this.params.column.minWidth = this.params.column.actualWidth;
        this.params.column.addEventListener("sortChanged", this.onSortChanged.bind(this));
        this.params.column.addEventListener("filterChanged", this.onFilterChanged.bind(this));
        this.onSortChanged();
    }

    onMenuClick(event: Event) {
        event.stopPropagation();
        this.params.showColumnMenu(this.querySelector(".custom-filter-button"));
    }

    onSortRequested(event) {
        this.params.progressSort(event.shiftKey);
    }

    onFilterChanged(event) {
        this.filtered = event.column.isFilterActive();
    }

    onSortChanged() {
        if (this.params.column.isSortAscending()) {
            this.sorted = "asc";
        } else if (this.params.column.isSortDescending()) {
            this.sorted = "desc";
        } else {
            this.sorted = null;
        }
    }

    private querySelector(selector: string) {
        return <HTMLElement>this.elementRef.nativeElement.querySelector(".custom-filter-button", selector);
    }
}
