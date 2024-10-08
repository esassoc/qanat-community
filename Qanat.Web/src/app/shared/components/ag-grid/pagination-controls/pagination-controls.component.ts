import { Component, Input, OnInit } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { Observable, combineLatest, map, startWith } from "rxjs";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "pagination-controls",
    templateUrl: "./pagination-controls.component.html",
    styleUrl: "./pagination-controls.component.scss",
    standalone: true,
    imports: [NgIf, AsyncPipe],
})
export class PaginationControlsComponent implements OnInit {
    @Input() grid: AgGridAngular;

    public paginationValues$: Observable<QanatPaginationValues>;

    public ngOnInit(): void {
        // Since combineLatest will only emit when all observables have emitted at least once, we need to start with a null value
        // to get the combineLatest to react to either a filter change or a pagination change
        const observables = [this.grid.filterChanged.pipe(startWith(null)), this.grid.paginationChanged.pipe(startWith(null))];

        this.paginationValues$ = combineLatest(observables).pipe(
            map(() => {
                return {
                    currentPage: this.grid.api.paginationGetCurrentPage() + 1,
                    totalPages: this.grid.api.paginationGetTotalPages(),
                    onFirstPage: this.grid.api.paginationGetCurrentPage() == 0,
                    onLastPage: this.grid.api.paginationGetCurrentPage() + 1 == this.grid.api.paginationGetTotalPages(),
                } as QanatPaginationValues;
            })
        );
    }

    public goToNextPage() {
        this.grid.api?.paginationGoToNextPage();
    }

    public goToPreviousPage() {
        this.grid.api?.paginationGoToPreviousPage();
    }

    public goToFirstPage() {
        this.grid.api?.paginationGoToFirstPage();
    }

    public goToLastPage() {
        this.grid.api?.paginationGoToLastPage();
    }
}

export interface QanatPaginationValues {
    currentPage: number;
    totalPages: number;
    onFirstPage: boolean;
    onLastPage: boolean;
}
