import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AgGridAngular, AgGridModule } from "ag-grid-angular";
import {
    ColDef,
    FilterChangedEvent,
    FirstDataRenderedEvent,
    GetRowIdFunc,
    GridApi,
    GridColumnsChangedEvent,
    GridOptions,
    GridReadyEvent,
    RowDataUpdatedEvent,
    RowSelectionMode,
    RowSelectionOptions,
    SelectionChangedEvent,
    SelectionColumnDef,
    Theme,
    themeBalham,
    iconOverrides,
    ModuleRegistry,
    AllCommunityModule,
} from "ag-grid-community";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { TooltipComponent } from "src/app/shared/components/ag-grid/tooltip/tooltip.component";
import { FormsModule } from "@angular/forms";
import { CsvDownloadButtonComponent } from "src/app/shared/components/csv-download-button/csv-download-button.component";
import { PaginationControlsComponent } from "src/app/shared/components/ag-grid/pagination-controls/pagination-controls.component";
import { QanatGridHeaderComponent } from "../qanat-grid-header/qanat-grid-header.component";

@Component({
    selector: "qanat-grid",
    imports: [CommonModule, AgGridModule, FormsModule, CsvDownloadButtonComponent, PaginationControlsComponent, QanatGridHeaderComponent],
    templateUrl: "./qanat-grid.component.html",
    styleUrls: ["./qanat-grid.component.scss"],
})
export class QanatGridComponent implements OnInit, OnChanges {
    @ViewChild(AgGridAngular) gridref: AgGridAngular;

    // ag grid stuff
    @Output() selectionChanged: EventEmitter<SelectionChangedEvent<any>> = new EventEmitter<SelectionChangedEvent<any>>();
    @Output() filterChanged: EventEmitter<FilterChangedEvent<any>> = new EventEmitter<FilterChangedEvent<any>>();
    @Output() gridReady: EventEmitter<GridReadyEvent> = new EventEmitter<GridReadyEvent>();
    @Output() gridRefReady: EventEmitter<AgGridAngular> = new EventEmitter<AgGridAngular>();
    @Output() firstDataLoaded: EventEmitter<FirstDataRenderedEvent> = new EventEmitter<FirstDataRenderedEvent>();

    @Input() rowData: any[];
    @Input() columnDefs: any[];
    @Input() defaultColDef: ColDef = {
        sortable: true,
        filter: true,
        resizable: true,
        tooltipComponent: TooltipComponent,
        tooltipValueGetter: (params) => params.value,
    };

    @Input() pagination: boolean = false;
    @Input() paginationPageSize: number = 100;
    @Input() getRowId: GetRowIdFunc;
    @Input() gridOptions: GridOptions;

    @Input() rowSelection: RowSelectionOptions;
    // setting default will override passed rowSelectionOptions
    @Input() defaultRowSelection: RowSelectionMode;

    // our stuff
    @Input() width: string = "100%";
    @Input() height: string = "720px";
    @Input() downloadFileName: string = "grid-data";
    @Input() colIDsToExclude: string[] = [];
    @Input() hideDownloadButton: boolean = false;
    @Input() hideTooltips: boolean = false;
    @Input() hideGlobalFilter: boolean = false;
    @Input() disableGlobalFilter: boolean = false;
    @Input() sizeColumnsToFitGrid: boolean = false;
    @Input() suppressColumnSizing: boolean = false;
    @Input() overrideDefaultGridHeader: boolean = false;
    @Input() unsetHeaderGridActionWidth: boolean = false;

    private gridApi: GridApi;
    public gridLoaded: boolean = false;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;
    public quickFilterText: string;
    public selectedRowsCount: number = 0;
    public allRowsSelected: boolean = false;
    public multiSelectEnabled: boolean;
    public selectionColumnDef: SelectionColumnDef = { pinned: true };
    public anyFilterPresent: boolean = false;
    public filteredRowsCount: number;
    public autoSizeStrategy: { type: "fitCellContents" | "fitGridWidth" };

    private fontAwesomeIcons = iconOverrides({
        type: "font",
        family: "FontAwesome",
        icons: {
            filter: "\u{f0b0}",
            filterActive: "\u{f0b0}",
        },
    });

    public gridTheme: Theme = themeBalham.withPart(this.fontAwesomeIcons);

    ngOnInit(): void {
        ModuleRegistry.registerModules([AllCommunityModule]);

        this.autoSizeStrategy = this.suppressColumnSizing ? null : { type: this.sizeColumnsToFitGrid ? "fitGridWidth" : "fitCellContents" };

        if (this.defaultRowSelection == "singleRow") {
            this.rowSelection = AgGridHelper.defaultSingleRowSelectionOptions;
        } else if (this.defaultRowSelection == "multiRow") {
            this.rowSelection = AgGridHelper.defaultMultiRowSelectionOptions;
        }

        this.multiSelectEnabled = this.rowSelection?.mode == "multiRow";
        if (this.multiSelectEnabled) {
            this.selectionColumnDef = {
                pinned: true,
                sortable: true,
                resizable: true,
                width: 70,
                sort: "desc",
            };
        }

        if (this.hideTooltips) {
            this.defaultColDef.tooltipValueGetter = null;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.rowData) {
            this.gridApi?.setGridOption("loading", true);
            this.gridApi?.updateGridOptions({ rowData: this.rowData });
            this.gridApi?.setGridOption("loading", false);
        }

        if (changes.columnDefs) {
            this.gridApi?.setGridOption("loading", true);
            this.gridApi?.updateGridOptions({ columnDefs: this.columnDefs });
            this.gridApi?.setGridOption("loading", false);
        }
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridReady.emit(event);

        this.gridApi = event.api;
    }

    public onFirstDataRendered(event: FirstDataRenderedEvent) {
        this.firstDataLoaded.emit(event);
        this.gridRefReady.emit(this.gridref);
        this.resizeGridColumns();
        this.gridLoaded = true;
    }

    public onGridColumnsChanged(event: GridColumnsChangedEvent) {
        this.resizeGridColumns();
    }

    public resizeGridColumns() {
        if (this.suppressColumnSizing) return;

        if (this.autoSizeStrategy.type == "fitCellContents") {
            this.gridApi?.autoSizeAllColumns();
        } else if (this.autoSizeStrategy.type == "fitGridWidth") {
            // This will size columns to fit the grid width, but it may not be perfect
            // as it doesn't account for the number of columns and their widths.
            this.gridApi?.sizeColumnsToFit();
        }
    }

    public onSelectionChanged(event: SelectionChangedEvent) {
        this.selectionChanged.emit(event);

        if (this.multiSelectEnabled) {
            this.selectedRowsCount = this.gridApi.getSelectedNodes().length;
            this.allRowsSelected = this.selectedRowsCount == this.rowData.length;
        }
    }

    public onFilterChanged(event: FilterChangedEvent) {
        this.filterChanged.emit(event);

        this.anyFilterPresent = event.api.isAnyFilterPresent();

        let filteredRowsCount = 0;
        this.gridApi.forEachNodeAfterFilter(() => {
            filteredRowsCount++;
        });
        this.filteredRowsCount = filteredRowsCount;
    }

    public onRowDataUpdated(event: RowDataUpdatedEvent) {
        event.api.autoSizeAllColumns();
        if (event.api.isRowDataEmpty()) {
            event.api.showNoRowsOverlay();
            if (!this.gridLoaded) {
                this.gridRefReady.emit(this.gridref);
                this.gridLoaded = true;
            }
        } else {
            event.api.hideOverlay();
        }
    }

    onSelectAll() {
        // todo: ensure only selecting filtered
        this.gridApi.selectAll();
    }

    onDeselectAll() {
        // todo: ensure only deselecting filtered
        this.gridApi.deselectAll();
    }

    public onFiltersCleared() {
        if (this.hideGlobalFilter) return;
        this.quickFilterText = "";
    }
}
