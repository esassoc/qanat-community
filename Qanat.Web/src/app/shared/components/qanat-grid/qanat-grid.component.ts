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
    GridReadyEvent,
    RowDataUpdatedEvent,
    SelectionChangedEvent,
} from "ag-grid-community";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { TooltipComponent } from "src/app/shared/components/ag-grid/tooltip/tooltip.component";
import { IconComponent } from "../icon/icon.component";
import { ClearGridFiltersButtonComponent } from "src/app/shared/components/clear-grid-filters-button/clear-grid-filters-button.component";
import { FormsModule } from "@angular/forms";
import { CsvDownloadButtonComponent } from "src/app/shared/components/csv-download-button/csv-download-button.component";
import { PaginationControlsComponent } from "src/app/shared/components/ag-grid/pagination-controls/pagination-controls.component";
import { QanatGridHeaderComponent } from "../qanat-grid-header/qanat-grid-header.component";

@Component({
    selector: "qanat-grid",
    standalone: true,
    imports: [
        CommonModule,
        AgGridModule,
        TooltipComponent,
        IconComponent,
        FormsModule,
        ClearGridFiltersButtonComponent,
        CsvDownloadButtonComponent,
        PaginationControlsComponent,
        QanatGridHeaderComponent,
    ],
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

    @Input() rowData: any[];
    @Input() columnDefs: any[];
    @Input() defaultColDef: ColDef = {
        sortable: true,
        filter: true,
        resizable: true,
        tooltipComponent: TooltipComponent,
        tooltipValueGetter: (params) => params.value,
    };
    @Input() rowSelection: "single" | "multiple";
    @Input() suppressRowClickSelection: boolean = false;
    @Input() rowMultiSelectWithClick: boolean = false;
    @Input() pagination: boolean = false;
    @Input() paginationPageSize: number = 100;
    @Input() getRowId: GetRowIdFunc;

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
    @Input() overrideDefaultGridHeader: boolean = false;

    private gridApi: GridApi;
    public gridLoaded: boolean = false;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;
    public quickFilterText: string;
    public selectedRowsCount: number = 0;
    public allRowsSelected: boolean = false;
    public multiSelectEnabled: boolean;
    public anyFilterPresent: boolean = false;
    public filteredRowsCount: number;

    public autoSizeStrategy: { type: "fitCellContents" | "fitGridWidth" };

    ngOnInit(): void {
        this.autoSizeStrategy = { type: this.sizeColumnsToFitGrid ? "fitGridWidth" : "fitCellContents" };
        this.multiSelectEnabled = this.rowSelection == "multiple";

        if (this.hideTooltips) {
            this.defaultColDef.tooltipValueGetter = null;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.rowData) {
            this.gridApi?.updateGridOptions({ rowData: this.rowData });
            this.gridApi?.hideOverlay();
        }

        if (changes.columnDefs) {
            this.gridApi?.updateGridOptions({ columnDefs: this.columnDefs });
            this.gridApi?.hideOverlay();
        }
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridReady.emit(event);
        this.gridApi = event.api;
    }

    public onFirstDataRendered(event: FirstDataRenderedEvent) {
        event.api.sizeColumnsToFit();
        this.gridLoaded = true;

        this.gridRefReady.emit(this.gridref);
    }

    public onGridColumnsChanged(event: GridColumnsChangedEvent) {
        event.api.sizeColumnsToFit();
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
    }

    onSelectAll() {
        this.gridApi.selectAllFiltered();
    }

    onDeselectAll() {
        this.gridApi.deselectAllFiltered();
    }

    public onFiltersCleared() {
        if (this.hideGlobalFilter) return;
        this.quickFilterText = "";
    }
}
