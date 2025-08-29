import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from "@angular/core";
import { FormControl, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from "@angular/forms";
import { Observable, timer, switchMap, of, map, tap, debounce } from "rxjs";
import { SearchService } from "../../generated/api/search.service";
import { ParcelSearchDto, ParcelSearchResultDto, ParcelSearchResultWithMatchedFieldsDto, ParcelSearchSummaryDto } from "../../generated/model/models";
import { AsyncPipe } from "@angular/common";
import { HighlightDirective } from "../../directives/highlight.directive";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "search-parcels",
    templateUrl: "./search-parcels.component.html",
    styleUrl: "./search-parcels.component.scss",
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: SearchParcelsComponent,
        },
    ],
    imports: [FormsModule, ReactiveFormsModule, HighlightDirective, AsyncPipe, IconComponent],
})
export class SearchParcelsComponent implements OnInit, OnDestroy {
    @Input() geographyID: number;
    @Input() excludedParcelIDs: number[] = [];
    @Input() isPartOfForm: boolean = true;
    @Output() change = new EventEmitter<ParcelSearchResultDto>();

    public searchString = new FormControl({ value: null, disabled: false });
    public searchResults$: Observable<ParcelSearchSummaryDto>;
    public allSearchResults: ParcelSearchResultWithMatchedFieldsDto[] = [];
    public val: ParcelSearchResultDto = null;
    public isSearching: boolean = false;
    public highlightedSearchResult: ParcelSearchResultWithMatchedFieldsDto;
    public currentParcel: ParcelSearchResultDto;
    private searchCleared: boolean = false;

    constructor(private searchService: SearchService) {}

    ngOnInit(): void {
        this.searchResults$ = this.searchString.valueChanges.pipe(
            debounce((x) => {
                // debounce search to 500ms when the user is typing in the search
                this.isSearching = true;
                if (this.searchString.value) {
                    return timer(500);
                } else {
                    // don't debounce when the user has cleared the search
                    return timer(500);
                }
            }),
            switchMap((searchString) => {
                this.isSearching = true;
                if (this.searchCleared && !searchString) {
                    return of(new ParcelSearchSummaryDto());
                }
                this.searchCleared = false;
                if (searchString != this.val?.ParcelNumber) {
                    const waterAccountSearchDto = new ParcelSearchDto();
                    waterAccountSearchDto.GeographyID = this.geographyID;
                    waterAccountSearchDto.SearchString = searchString;
                    return this.searchService.searchParcelsSearch(waterAccountSearchDto);
                }
                return of(new ParcelSearchSummaryDto());
            }),
            map((x: ParcelSearchSummaryDto) => {
                if (this.excludedParcelIDs.length == 0) return x;
                x.ParcelSearchResults = x.ParcelSearchResults?.filter((y) => !this.excludedParcelIDs.includes(y.Parcel.ParcelID));
                return x;
            }),
            tap((x: ParcelSearchSummaryDto) => {
                this.isSearching = false;
                this.allSearchResults = x?.ParcelSearchResults ?? [];
                this.highlightedSearchResult = x?.ParcelSearchResults?.length > 0 ? x.ParcelSearchResults[0] : null;
            })
        );
    }

    toggleDropdown() {
        this.searchCleared = false;
        if (this.val == this.currentParcel) {
            this.val = null;
            this.searchString.patchValue("");
        } else {
            this.val = this.currentParcel;
        }

        this.change.emit(this.val);
    }

    selectNext(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex + 1 < this.allSearchResults.length) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex + 1];
            const listItemToScrollTo = document.getElementById("Parcel_" + this.highlightedSearchResult.Parcel.ParcelID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectPrevious(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex - 1 >= 0) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex - 1];
            const listItemToScrollTo = document.getElementById("Parcel_" + this.highlightedSearchResult.Parcel.ParcelID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectCurrent(event: Event): void {
        if (this.highlightedSearchResult) {
            this.selectParcel(this.highlightedSearchResult);
        }
    }

    ngOnDestroy(): void {}

    clearSearch() {
        this.searchString.reset();
        this.value = null;
        this.currentParcel = null;
        this.searchCleared = true;
    }

    selectParcel(result: ParcelSearchResultWithMatchedFieldsDto) {
        this.value = result.Parcel;
        this.change.emit(result.Parcel);
    }

    // begin ControlValueAccessor
    public disabled = false;
    public touched = false;
    onChange: any = () => {};
    onTouch: any = () => {};

    set value(val: ParcelSearchResultDto) {
        // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
        this.val = val;
        if (this.val) {
            this.currentParcel = val;
        }
        this.onChange(val);
        this.onTouch(val);
        if (val) {
            this.searchString.patchValue(val.ParcelNumber, { emitEvent: false }); // dont emit, to prevent another search from being done
        } else {
            // if falsy, patch to empty string and emit so that the search valueChanges returns an empty array which clears the search results
            this.searchString.patchValue("", { emitEvent: true });
        }
    }

    writeValue(obj: ParcelSearchResultDto) {
        this.value = obj;
    }
    registerOnChange(onChange: any) {
        this.onChange = onChange;
    }
    registerOnTouched(onTouched: any) {
        this.onTouch = onTouched;
    }
    setDisabledState?(isDisabled: boolean) {
        this.disabled = isDisabled;
        if (isDisabled) {
            this.searchString.disable();
        } else {
            this.searchString.enable();
        }
    }

    markAsTouched() {
        if (!this.touched) {
            this.onTouch();
            this.touched = true;
        }
    }
    // end ControlValueAccessor
}
