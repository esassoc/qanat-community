import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ClearGridFiltersButtonComponent } from '../clear-grid-filters-button/clear-grid-filters-button.component';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AgGridAngular } from 'ag-grid-angular';
import { Subscription } from 'rxjs';

@Component({
  selector: 'qanat-grid-header',
  standalone: true,
  imports: [ClearGridFiltersButtonComponent, FormsModule, NgIf],
  templateUrl: './qanat-grid-header.component.html',
  styleUrl: './qanat-grid-header.component.scss'
})
export class QanatGridHeaderComponent implements OnInit, OnDestroy {
  @Input() grid: AgGridAngular;
  @Input() rowDataCount: number;
  @Input() hideGlobalFilter: boolean = false;
  @Input() multiSelectEnabled: boolean = false;
  @Input() leftAlignClearFiltersButton: boolean = false;
  @Input() disableGlobalFilter: boolean = false;

  public quickFilterText: string;
  public selectedRowsCount: number = 0;
  public allRowsSelected: boolean = false;
  public anyFilterPresent: boolean = false;
  public recordCount: number = 0;

  private selectionChangedSubscription: Subscription = Subscription.EMPTY;
  private filterChangedSubscription: Subscription = Subscription.EMPTY;

  ngOnInit(): void {
    this.selectionChangedSubscription = this.grid.selectionChanged.subscribe(() => {
      this.selectedRowsCount = this.grid.api.getSelectedNodes().length;
      this.allRowsSelected = this.selectedRowsCount == this.grid.api.getDisplayedRowCount();
    });

    this.filterChangedSubscription = this.grid.filterChanged.subscribe(() => {
      this.anyFilterPresent = this.grid.api.isAnyFilterPresent();
    });
  }

  ngOnDestroy(): void {
    this.selectionChangedSubscription.unsubscribe();
    this.filterChangedSubscription.unsubscribe();
  }

  onSelectAll() {
    this.grid.api.selectAllFiltered();
  }

  onDeselectAll() {
    this.grid.api.deselectAllFiltered();
  }

  public onFiltersCleared() {
    if (this.hideGlobalFilter) return;
    this.quickFilterText = '';
    this.onQuickFilterTextChanged();
  }

  public onQuickFilterTextChanged() {
    this.grid.api.setGridOption('quickFilterText', this.quickFilterText);
  }
}
