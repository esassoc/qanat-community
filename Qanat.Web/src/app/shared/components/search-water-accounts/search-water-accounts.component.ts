import { Component, Input, OnDestroy, OnInit } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, of, timer } from "rxjs";
import { debounce, map, switchMap, tap } from "rxjs/operators";
import { SearchService } from "../../generated/api/search.service";
import { WaterAccountDto, WaterAccountSearchResultWithMatchedFieldsDto, WaterAccountSearchSummaryDto } from "../../generated/model/models";
import { CommaJoinPipe } from "../../pipes/comma-join.pipe";
import { SumPipe } from "../../pipes/sum.pipe";
import { HighlightDirective } from "../../directives/highlight.directive";
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from "@angular/common";

@Component({
    selector: "search-water-accounts",
    templateUrl: "./search-water-accounts.component.html",
    styleUrls: ["./search-water-accounts.component.scss"],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: SearchWaterAccountsComponent,
        },
    ],
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule, NgIf, NgFor, HighlightDirective, AsyncPipe, DecimalPipe, SumPipe, CommaJoinPipe],
})
export class SearchWaterAccountsComponent implements OnInit, OnDestroy {
    @Input() geographyID: number;
    @Input() excludedWaterAccountIDs: number[] = [];

    public searchString = new FormControl({ value: null, disabled: false });
    public searchResults$: Observable<WaterAccountSearchSummaryDto>;
    public allSearchResults: WaterAccountSearchResultWithMatchedFieldsDto[] = [];
    public val: WaterAccountDto = null;
    public isSearching: boolean = false;
    public highlightedSearchResult: WaterAccountSearchResultWithMatchedFieldsDto;

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
                if (!searchString) return of(new WaterAccountSearchSummaryDto());
                if (searchString && searchString.length > 1 && searchString != this.val?.WaterAccountName) {
                    return this.searchService.searchGeographyGeographyIDWaterAccountsGet(this.geographyID, searchString);
                }
                return of(new WaterAccountSearchSummaryDto());
            }),
            map((x: WaterAccountSearchSummaryDto) => {
                if (this.excludedWaterAccountIDs.length == 0) return x;
                x.WaterAccountSearchResults = x.WaterAccountSearchResults?.filter((y) => !this.excludedWaterAccountIDs.includes(y.WaterAccount.WaterAccountID) ?? null);
                return x;
            }),
            tap((x: WaterAccountSearchSummaryDto) => {
                this.isSearching = false;
                this.allSearchResults = x?.WaterAccountSearchResults ?? [];
                this.highlightedSearchResult = x?.WaterAccountSearchResults?.length > 0 ? x.WaterAccountSearchResults[0] : null;
            })
        );
    }

    selectNext(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex + 1 < this.allSearchResults.length) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex + 1];
            const listItemToScrollTo = document.getElementById("WaterAccount_" + this.highlightedSearchResult.WaterAccount.WaterAccountID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectPrevious(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex - 1 >= 0) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex - 1];
            const listItemToScrollTo = document.getElementById("WaterAccount_" + this.highlightedSearchResult.WaterAccount.WaterAccountID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectCurrent(event: Event): void {
        if (this.highlightedSearchResult) {
            this.selectWaterAccount(this.highlightedSearchResult);
        }
    }

    ngOnDestroy(): void {}

    clearSearch() {
        this.searchString.reset();
        this.value = null;
    }

    selectWaterAccount(result: WaterAccountSearchResultWithMatchedFieldsDto) {
        this.value = result.WaterAccount;
    }

    // begin ControlValueAccessor
    public disabled = false;
    public touched = false;
    onChange: any = () => {};
    onTouch: any = () => {};

    set value(val: WaterAccountDto) {
        // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
        this.val = val;
        this.onChange(val);
        this.onTouch(val);
        if (val) {
            this.searchString.patchValue(val.WaterAccountNameAndNumber, { emitEvent: false }); // dont emit, to prevent another search from being done
        } else {
            // if falsy, patch to empty string and emit so that the search valueChanges returns an empty array which clears the search results
            this.searchString.patchValue("", { emitEvent: true });
        }
    }

    writeValue(obj: WaterAccountDto) {
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
