import { Component, EventEmitter, Input, OnChanges, OnInit, Output } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { QanatGridComponent } from "../qanat-grid/qanat-grid.component";
import { QanatGridHeaderComponent } from "../qanat-grid-header/qanat-grid-header.component";
import { IconComponent } from "../icon/icon.component";
import { AgGridAngular } from "ag-grid-angular";
import { QanatMapInitEvent, QanatMapComponent } from "../leaflet/qanat-map/qanat-map.component";
import { Map, layerControl } from "leaflet";
import { AgGridHelper } from "../../helpers/ag-grid-helper";

@Component({
    selector: "hybrid-map-grid",
    imports: [QanatGridComponent, QanatGridHeaderComponent, IconComponent, QanatMapComponent],
    templateUrl: "./hybrid-map-grid.component.html",
    styleUrl: "./hybrid-map-grid.component.scss",
})
export class HybridMapGridComponent implements OnInit, OnChanges {
    @Input() rowData: any[];
    @Input() columnDefs: ColDef[];
    @Input() downloadFileName: string;
    @Input() selectedValue: number = null;
    @Input() entityIDField: string = "";
    @Input() mapHeight: string = "720px";

    @Output() gridReady: EventEmitter<GridReadyEvent> = new EventEmitter();
    @Output() onMapLoad: EventEmitter<QanatMapInitEvent> = new EventEmitter();
    @Output() selectedValueChange: EventEmitter<number> = new EventEmitter<number>();

    private selectedGridValue: number;

    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public selectedPanel: "Grid" | "Hybrid" | "Map" = "Hybrid";

    public map: Map;
    public layerControl: layerControl;
    public bounds: any;
    public mapIsReady: boolean = false;

    public isLoading: boolean = true;
    public firstLoad: boolean = true;

    ngOnInit(): void {
        this.selectedGridValue = this.selectedValue;
    }

    ngOnChanges(changes: any): void {
        if (changes.selectedValue) {
            if (changes.selectedValue.previousValue == changes.selectedValue.currentValue) {
                return;
            }

            this.selectedValue = changes.selectedValue.currentValue;
            this.onMapSelectionChanged(this.selectedValue);
        }
    }

    public toggleSelectedPanel(selectedPanel: "Grid" | "Hybrid" | "Map") {
        this.selectedPanel = selectedPanel;

        // resizing map to fit new container width; timeout needed to ensure new width has registered before running invalidtaeSize()
        setTimeout(() => {
            this.map.invalidateSize(true);

            if (this.layerControl && this.bounds) {
                this.map.fitBounds(this.bounds);
            }
        }, 300);

        // if no map is visible, turn of grid selection
        if (selectedPanel == "Grid") {
            this.gridApi.setGridOption("rowSelection", undefined);
            this.selectedValue = undefined;
        } else {
            this.gridApi.setGridOption("rowSelection", AgGridHelper.defaultSingleRowSelectionOptions);
        }
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.mapIsReady = true;

        this.onMapLoad.emit(event);
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridReady.emit(event);
    }

    public onGridRefReady(gridRef: AgGridAngular) {
        this.gridRef = gridRef;
    }

    public emitSelectedValue(value: number) {
        if (value == this.selectedValue) return;
        this.selectedValueChange.emit(value);
    }

    public onGridSelectionChanged() {
        const selectedNodes = this.gridApi.getSelectedNodes();
        const selectedValue = selectedNodes.length > 0 ? selectedNodes[0].data[this.entityIDField] : null;

        this.setGridSelection(selectedValue);
        this.emitSelectedValue(selectedValue);
    }

    public onMapSelectionChanged(selectedEntityID: number) {
        const selectedValue = selectedEntityID;

        this.setGridSelection(selectedValue, "top");
        this.emitSelectedValue(selectedValue);
    }

    private setGridSelection(selectedEntityID: number, position = null) {
        if (this.selectedGridValue == selectedEntityID) return;

        this.gridApi.forEachNode((node, index) => {
            if (node.data[this.entityIDField] == selectedEntityID) {
                node.setSelected(true, true);

                this.gridApi.ensureIndexVisible(index, position);
            }
        });

        this.selectedGridValue = selectedEntityID;
    }
}
