import { GridOptions } from "ag-grid-community";

export class AgGridHelper {
    public static gridSpinnerOverlay = `<div class="circle"><div class="wave"></div></div>`;

    public static defaultGridOptions: GridOptions = {
        enableCellTextSelection: true,
        ensureDomOrder: true,
    };
}
