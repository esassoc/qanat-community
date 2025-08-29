import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, Subject, concat, of } from "rxjs";
import { catchError, distinctUntilChanged, switchMap, tap } from "rxjs/operators";
import { SearchService } from "../../generated/api/search.service";
import { WaterAccountSearchDto, WaterAccountSearchResultWithMatchedFieldsDto, WaterAccountSearchSummaryDto } from "../../generated/model/models";
import { CommaJoinPipe } from "../../pipes/comma-join.pipe";
import { SumPipe } from "../../pipes/sum.pipe";
import { HighlightDirective } from "../../directives/highlight.directive";
import { AsyncPipe, DecimalPipe } from "@angular/common";
import { NgSelectModule } from "@ng-select/ng-select";

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
    imports: [FormsModule, ReactiveFormsModule, AsyncPipe, NgSelectModule, HighlightDirective, SumPipe, DecimalPipe, CommaJoinPipe]
})
export class SearchWaterAccountsComponent implements AfterViewInit, OnChanges {
    @Input() geographyID: number;
    @Input() excludedWaterAccountIDs: number[] = [];
    @Input() formControl: FormControl;
    @Input() isPartOfForm: boolean = true;
    @Input() isOpen: boolean;
    @Input() initialWaterAccountID: number;
    @Output() change = new EventEmitter<number>();

    public searchString = new Subject<string>();
    public searchResults$: Observable<WaterAccountSearchSummaryDto>;
    public allSearchResults: WaterAccountSearchResultWithMatchedFieldsDto[] = [];
    public selectedValue: number = null;
    public isSearching: boolean = false;
    public highlightedSearchResult: WaterAccountSearchResultWithMatchedFieldsDto;
    public currentWaterAccountID: number;

    public isDisabled: boolean = false;
    private internalFormControl = new FormControl();

    get activeFormControl(): FormControl {
        return this.formControl || this.internalFormControl;
    }

    constructor(private searchService: SearchService) {}

    ngAfterViewInit(): void {
        // Initialize the search results observable
        this.initializeSearch();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.geographyID && changes.geographyID.currentValue) {
            this.initializeSearch();
        }
    }

    private initializeSearch(): void {
        this.searchResults$ = concat(
            this.searchWaterAccounts("", this.val ?? this.initialWaterAccountID),
            this.searchString.pipe(
                distinctUntilChanged(),
                tap(() => (this.isSearching = true)),
                switchMap((term) => this.searchWaterAccounts(term, this.val))
            )
        );
    }

    private searchWaterAccounts(term: string, selectedWaterAccountID?: number): Observable<WaterAccountSearchSummaryDto> {
        const waterAccountSearchDto = new WaterAccountSearchDto();
        waterAccountSearchDto.GeographyID = this.geographyID;
        waterAccountSearchDto.SearchString = term;
        waterAccountSearchDto.WaterAccountID = selectedWaterAccountID;

        return this.searchService.searchWaterAccountsSearch(waterAccountSearchDto).pipe(
            catchError(() => of(new WaterAccountSearchSummaryDto())),
            tap(() => {
                this.isSearching = false;
            })
        );
    }

    public onClear() {
        this.val = null;
        // Reset search term and ensure dropdown is refreshed
        this.searchString.next("");
    }

    public onSelectionChange(event: any) {
        // Extract just the WaterAccountID from the selected item
        // This ensures we emit only the ID value, not the full object
        const selectedValue = event?.WaterAccount?.WaterAccountID;
        this.value = selectedValue;

        // Reset the search term to ensure fresh results on next open
        this.searchString.next("");

        // We don't need to manually close the dropdown here
        // ng-select will handle closing automatically after selection
        // and the parent dropdown toggle directive will manage reopening
    }

    public val: any;
    set value(val) {
        this.val = val;

        // Update form control if provided
        if (this.formControl && this.formControl.value !== val) {
            this.formControl.setValue(val, { emitEvent: false });
        }

        this.change.emit(val);
        this.onChange(val);
        this.onTouch(val);
    }

    onChange: any = () => {};
    onTouch: any = () => {};

    writeValue(value: any): void {
        this.val = value;
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouch = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }
}
