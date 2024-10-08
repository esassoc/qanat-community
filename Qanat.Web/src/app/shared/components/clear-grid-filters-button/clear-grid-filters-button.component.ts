import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { GridApi } from 'ag-grid-community';
import { Subscription } from 'rxjs';

@Component({
  selector: 'qanat-clear-grid-filters-button',
  templateUrl: './clear-grid-filters-button.component.html',
  styleUrls: ['./clear-grid-filters-button.component.scss'],
  standalone: true,
})
export class ClearGridFiltersButtonComponent implements OnInit, OnDestroy {
  @Input() grid: AgGridAngular = null;

  @Output() filtersCleared = new EventEmitter();

  private gridReadySubscription: Subscription = Subscription.EMPTY;
  private filtersChangedSubscription: Subscription = Subscription.EMPTY;
  public gridApi: GridApi = null;
  public gridFiltersApplied: boolean = false;

  ngOnInit(): void {
    this.gridApi = this.grid?.api;

    this.gridReadySubscription = this.grid.gridReady.subscribe(event => {
      this.gridApi = event.api;
    });

    this.filtersChangedSubscription = this.grid.filterChanged.subscribe((event) => {
      this.gridFiltersApplied = event.api.isAnyFilterPresent();
    });
  }

  ngOnDestroy(): void {
    this.gridReadySubscription.unsubscribe();
    this.filtersChangedSubscription.unsubscribe();
  }

  public onClick() {
    this.gridApi.setFilterModel(null);
    this.filtersCleared.emit();
  }
}
