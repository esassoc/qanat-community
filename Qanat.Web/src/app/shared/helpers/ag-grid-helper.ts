import { GridOptions, RowSelectionOptions } from "ag-grid-community";

export class AgGridHelper {
    public static gridSpinnerOverlay = `<div class="circle"><div class="wave"></div></div>`;

    public static defaultGridOptions: GridOptions = {
        enableCellTextSelection: true,
        ensureDomOrder: true,
    };

    public static readonly defaultSingleRowSelectionOptions: RowSelectionOptions = {
        mode: "singleRow",
        enableClickSelection: true,
        checkboxes: false,
    };

    public static readonly defaultMultiRowSelectionOptions: RowSelectionOptions = {
        mode: "multiRow",
        enableClickSelection: true,
        enableSelectionWithoutKeys: true,
        selectAll: "filtered",
    };
}
