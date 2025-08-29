import { Component } from "@angular/core";
import { AgFilterComponent } from "ag-grid-angular";
import { IDoesFilterPassParams, RowNode } from "ag-grid-community";
import { FormsModule } from "@angular/forms";

@Component({
    selector: "qanat-custom-dropdown-filter",
    templateUrl: "./custom-dropdown-filter.component.html",
    styleUrls: ["./custom-dropdown-filter.component.scss"],
    imports: [FormsModule]
})
export class CustomDropdownFilterComponent implements AgFilterComponent {
    params;
    field: string;
    dropdownValues = [];
    columnContainsMultipleValues: boolean = false;
    useDownloadDisplayValue: boolean = false;

    state = {
        selectAll: true,
        deselectAll: false,
        strict: false,
        filterOptions: {},
    };

    agInit(params): void {
        this.params = params;

        if (params.colDef.filterParams) {
            this.field = params.colDef.filterParams.field;
            this.useDownloadDisplayValue = params.colDef.filterParams.useDownloadDisplayValue;
            this.columnContainsMultipleValues = params.colDef.filterParams.columnContainsMultipleValues;
        }

        if (this.columnContainsMultipleValues) {
            this.initMultipleValueColumnFilter();
        } else {
            this.initSingleValueColumnFilter();
        }
    }

    initSingleValueColumnFilter(): void {
        this.params.api.forEachNode((rowNode, i) => {
            const columnValue = this.getNodeValue(rowNode);
            if (!this.dropdownValues.includes(columnValue)) {
                this.dropdownValues.push(columnValue);
            }
        });

        // Initialize the checked state for each option.
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[element] = true;
        });
    }

    initMultipleValueColumnFilter(): void {
        this.state.selectAll = false;
        this.state.deselectAll = true;

        this.params.api.forEachNode((rowNode, i) => {
            const columnValue = this.getNodeValue(rowNode);
            if (!Array.isArray(columnValue)) {
                throw "Value getter for multiple column filter needs to return an array";
            }
            columnValue.forEach((x) => {
                if (!this.dropdownValues.includes(x)) {
                    this.dropdownValues.push(x);
                }
            });
        });

        // Initialize the unchecked state for multiple value columns
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[element] = false;
        });
    }

    // If the filter is NOT active, the filter will pass for every row.
    // the filter paradigm for a multiple value column
    isFilterActive(): boolean {
        return this.columnContainsMultipleValues ? !this.state.deselectAll : !this.state.selectAll;
    }

    doesFilterPass(filterParams: IDoesFilterPassParams): boolean {
        if (this.columnContainsMultipleValues) {
            return this.doesfilterPassMultipleValues(filterParams);
        } else {
            return this.doesFilterPassSingleValue(filterParams);
        }
    }

    doesfilterPassMultipleValues(filterParams: IDoesFilterPassParams): boolean {
        const valueArray = this.getNodeValue(filterParams.node as RowNode<any>);
        let filterPasses;

        // if strict we need to compare all true filter options and see if the valueArray contains them all
        if (this.state.strict) {
            const checkedOptions = [];
            for (const [key, value] of Object.entries(this.state.filterOptions)) {
                if (value) {
                    checkedOptions.push(key);
                }
            }
            filterPasses = checkedOptions.every((name) => valueArray.includes(name));
        } else {
            // if not strict we can just see if the valueArray contains at least one of the selected filter options
            filterPasses = valueArray.some((name) => this.state.filterOptions[name]);
        }

        return filterPasses;
    }

    doesFilterPassSingleValue(filterParams: IDoesFilterPassParams): boolean {
        const value = this.getNodeValue(filterParams.node as RowNode<any>);
        if (this.state.filterOptions[value] == null) {
            return false;
        }
        return this.state.filterOptions[value] ? true : false;
    }

    private getNodeValue(rowNode: RowNode) {
        if (this.field) {
            return this.getPropertyValue(rowNode.data, this.field, "");
        } else if (this.params.colDef.valueGetter) {
            const value = this.params.colDef.valueGetter(rowNode);
            return this.useDownloadDisplayValue && value.downloadDisplay ? value.downloadDisplay : value;
        }
    }

    private getPropertyValue(object, path, defaultValue) {
        return path.split(".").reduce((o, p) => (o ? o[p] : defaultValue), object);
    }

    getModel() {
        return { filtersActive: this.state };
    }

    // one place this gets called is when the clear-grid-filters-button component clears the filter model
    setModel(model: any) {
        if (model === null) {
            // when we reset the model, for a multiple values filter we need to deselect all instead of selecting all.
            this.columnContainsMultipleValues ? this.onDeselectAll() : this.onSelectAll();
        } else {
            this.state = model.filtersActive;
        }
    }

    getDropdownValues() {
        return this.dropdownValues.sort();
    }

    updateFilter() {
        this.state.selectAll = true;
        this.state.deselectAll = true;
        for (const element of this.dropdownValues) {
            if (this.state.filterOptions[element]) {
                this.state.deselectAll = false;
            } else {
                this.state.selectAll = false;
            }

            if (!this.state.selectAll && !this.state.deselectAll) {
                break;
            }
        }

        this.params.filterChangedCallback();
    }

    onSelectAll() {
        this.state.selectAll = true;
        this.state.deselectAll = false;

        this.updateFilterSelection();
    }

    onDeselectAll() {
        this.state.selectAll = false;
        this.state.deselectAll = true;

        this.updateFilterSelection();
    }

    onSelectStrict() {
        this.state.strict = true;
        this.params.filterChangedCallback();
    }

    onSelectLoose() {
        this.state.strict = false;
        this.params.filterChangedCallback();
    }

    private updateFilterSelection() {
        this.dropdownValues.forEach((element) => {
            this.state.filterOptions[element] = this.state.selectAll;
        });

        this.params.filterChangedCallback();
    }
}
