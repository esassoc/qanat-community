import { Component, EventEmitter, Input, Output, OnInit, OnDestroy } from "@angular/core";
import { FormControl, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from "@angular/forms";
import { Observable, timer, switchMap, of, map, tap, debounce } from "rxjs";
import { SearchService } from "../../generated/api/search.service";
import {
    WaterAccountContactSearchDto,
    WaterAccountContactSearchResultDto,
    WaterAccountContactSearchResultWithMatchedFieldsDto,
    WaterAccountContactSearchSummaryDto,
} from "../../generated/model/models";
import { AsyncPipe } from "@angular/common";
import { HighlightDirective } from "../../directives/highlight.directive";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "search-water-account-contacts",
    templateUrl: "./search-water-account-contacts.component.html",
    styleUrl: "./search-water-account-contacts.component.scss",
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: SearchWaterAccountContactsComponent,
        },
    ],
    imports: [FormsModule, ReactiveFormsModule, HighlightDirective, AsyncPipe, IconComponent],
})
export class SearchWaterAccountContactsComponent implements OnInit, OnDestroy {
    @Input() geographyID: number;
    @Input() isPartOfForm: boolean = true;
    @Input() excludedWaterAccountContactIDs: number[] = [];
    @Output() change = new EventEmitter<WaterAccountContactSearchResultDto>();

    public searchString = new FormControl({ value: null, disabled: false });

    public searchResults$: Observable<WaterAccountContactSearchSummaryDto>;
    public allSearchResults: WaterAccountContactSearchResultWithMatchedFieldsDto[] = [];
    public val: WaterAccountContactSearchResultDto = null;
    public isSearching: boolean = false;
    public highlightedSearchResult: WaterAccountContactSearchResultWithMatchedFieldsDto;
    public currentWaterAccountContact: WaterAccountContactSearchResultDto;
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
                    return of(new WaterAccountContactSearchSummaryDto());
                }
                this.searchCleared = false;
                if (searchString != this.val?.ContactName) {
                    const waterAccountContactSearchDto = new WaterAccountContactSearchDto();
                    waterAccountContactSearchDto.GeographyID = this.geographyID;
                    waterAccountContactSearchDto.SearchString = searchString;

                    return this.searchService.searchWaterAccountContactsSearch(waterAccountContactSearchDto);
                }
                return of(new WaterAccountContactSearchSummaryDto());
            }),
            map((x: WaterAccountContactSearchSummaryDto) => {
                if (this.excludedWaterAccountContactIDs.length == 0) return x;
                x.SearchResults = x.SearchResults?.filter((y) => !this.excludedWaterAccountContactIDs.includes(y.WaterAccountContact.WaterAccountContactID));
                return x;
            }),
            tap((x: WaterAccountContactSearchSummaryDto) => {
                this.isSearching = false;
                this.allSearchResults = x?.SearchResults ?? [];
                this.highlightedSearchResult = x?.SearchResults?.length > 0 ? x.SearchResults[0] : null;
            })
        );
    }

    toggleDropdown() {
        this.searchCleared = false;
        if (this.val == this.currentWaterAccountContact) {
            this.val = null;
            this.searchString.patchValue("");
        } else {
            this.val = this.currentWaterAccountContact;
        }

        this.change.emit(this.val);
    }

    selectNext(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex + 1 < this.allSearchResults.length) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex + 1];
            const listItemToScrollTo = document.getElementById("WaterAccountContact_" + this.highlightedSearchResult.WaterAccountContact.WaterAccountContactID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectPrevious(): void {
        const currentIndex = this.allSearchResults.indexOf(this.highlightedSearchResult);
        if (currentIndex - 1 >= 0) {
            this.highlightedSearchResult = this.allSearchResults[currentIndex - 1];
            const listItemToScrollTo = document.getElementById("WaterAccountContact_" + this.highlightedSearchResult.WaterAccountContact.WaterAccountContactID);
            listItemToScrollTo.scrollIntoView(false);
        }
    }

    selectCurrent(event: Event): void {
        if (this.highlightedSearchResult) {
            this.selectWaterAccountContact(this.highlightedSearchResult);
        }
    }

    ngOnDestroy(): void {}

    clearSearch() {
        this.searchString.reset();
        this.value = null;
        this.currentWaterAccountContact = null;
        this.searchCleared = true;
    }

    selectWaterAccountContact(result: WaterAccountContactSearchResultWithMatchedFieldsDto) {
        this.value = result.WaterAccountContact;
        this.change.emit(result.WaterAccountContact);
    }

    // begin ControlValueAccessor
    public disabled = false;
    public touched = false;
    onChange: any = () => {};
    onTouch: any = () => {};

    set value(val: WaterAccountContactSearchResultDto) {
        // this value is updated by programmatic changes if( val !== undefined && this.val !== val){
        this.val = val;
        if (this.val) {
            this.currentWaterAccountContact = val;
        }
        this.onChange(val);
        this.onTouch(val);
        if (val) {
            this.searchString.patchValue(val.ContactName, { emitEvent: false }); // dont emit, to prevent another search from being done
        } else {
            // if falsy, patch to empty string and emit so that the search valueChanges returns an empty array which clears the search results
            this.searchString.patchValue("", { emitEvent: true });
        }
    }

    writeValue(obj: WaterAccountContactSearchResultDto) {
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
