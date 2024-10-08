import { Directive } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { AgGridHelper } from "../helpers/ag-grid-helper";

@Directive({
    selector: "[defaultGridOptions]",
    standalone: true,
})
export class DefaultGridOptionsDirective {
    constructor(private grid: AgGridAngular) {
        if (!grid.gridOptions) {
            grid.gridOptions = AgGridHelper.defaultGridOptions;
        }
    }
}
