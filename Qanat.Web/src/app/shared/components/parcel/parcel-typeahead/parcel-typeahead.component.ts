import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges, SimpleChanges } from "@angular/core";
import { NgSelectComponent, NgSelectModule } from "@ng-select/ng-select";
import { Observable, of, Subject } from "rxjs";
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap } from "rxjs/operators";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { ParcelService } from "../../../generated/api/parcel.service";
import { ParcelMinimalDto } from "../../../generated/model/models";
import { AsyncPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { ParcelByGeographyService } from "../../../generated/api/parcel-by-geography.service";

@Component({
    selector: "parcel-typeahead",
    templateUrl: "./parcel-typeahead.component.html",
    styleUrls: ["./parcel-typeahead.component.scss"],
    standalone: true,
    imports: [NgSelectModule, FormsModule, AsyncPipe],
})
export class ParcelTypeaheadComponent implements OnInit, OnChanges {
    @ViewChild(NgSelectComponent) ngSelectComponent: NgSelectComponent;
    @Input() selectedParcel: ParcelMinimalDto | ParcelDisplayDto;
    @Input() geographyID: number;
    @Input() excludedParcelIDs: number[] = [];
    @Output() change = new EventEmitter<ParcelDisplayDto>();

    public parcels$: Observable<ParcelDisplayDto[]>;
    public parcelInputs$ = new Subject<string>();
    public searchLoading = false;

    constructor(private parcelService: ParcelService, private parcelByGeographyService: ParcelByGeographyService) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (Object.keys(changes).includes("geographyID") || Object.keys(changes).includes("excludedParcelIDs")) {
            this.initSearch();
        }
    }

    ngOnInit(): void {
        this.initSearch();
    }

    initSearch(): void {
        this.parcels$ = this.parcelInputs$.pipe(
            filter((searchTerm) => searchTerm != null),
            distinctUntilChanged(),
            tap(() => (this.searchLoading = true)),
            debounceTime(800),
            switchMap((searchTerm) =>
                this.geographyID
                    ? // search within geography
                      this.parcelByGeographyService.geographiesGeographyIDParcelsSearchSearchStringGet(this.geographyID, searchTerm).pipe(
                          map((x) => x.filter((y) => !this.excludedParcelIDs.includes(y.ParcelID))),
                          catchError(() => of([])),
                          tap(() => (this.searchLoading = false))
                      )
                    : // search all geographies
                      this.parcelService.parcelsSearchSearchStringGet(searchTerm).pipe(
                          map((x) => x.filter((y) => !this.excludedParcelIDs.includes(y.ParcelID))),
                          catchError(() => of([])),
                          tap(() => (this.searchLoading = false))
                      )
            )
        );
    }

    public onSelectedParcelChange(parcel: ParcelDisplayDto) {
        if (parcel) {
            this.selectedParcel = parcel;
            this.selectedParcel.ParcelID = parcel ? parcel.ParcelID : null;
            this.selectedParcel.ParcelNumber = parcel ? parcel.ParcelNumber : null;
        } else {
            this.selectedParcel = null;
        }
        this.change.emit(parcel);
    }
}
