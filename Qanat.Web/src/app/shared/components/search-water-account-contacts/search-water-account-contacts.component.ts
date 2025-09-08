import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from "@angular/core";
import { FormControl, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from "@angular/forms";
import { Observable, Subject, of, switchMap, tap, concat, distinctUntilChanged, catchError } from "rxjs";
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
import { NgSelectModule } from "@ng-select/ng-select";

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
    imports: [FormsModule, ReactiveFormsModule, HighlightDirective, AsyncPipe, IconComponent, NgSelectModule],
})
export class SearchWaterAccountContactsComponent implements OnChanges {
    @Input() geographyID: number;
    @Input() excludedWaterAccountContactIDs: number[] = [];
    @Input() formControl: FormControl;
    @Input() isPartOfForm: boolean = true;
    @Input() isOpen: boolean;
    @Input() initialWaterAccountContactID: number;

    @Output() change = new EventEmitter<WaterAccountContactSearchResultDto>();

    public searchString = new Subject<string>();
    public searchResults$: Observable<WaterAccountContactSearchSummaryDto>;
    public allSearchResults: WaterAccountContactSearchResultWithMatchedFieldsDto[] = [];
    public selectedValue: WaterAccountContactSearchResultDto = null;
    public isSearching: boolean = false;
    public highlightedSearchResult: WaterAccountContactSearchResultWithMatchedFieldsDto;
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
            this.searchWaterAccountContacts("", this.val?.WaterAccountContactID ?? this.initialWaterAccountContactID),
            this.searchString.pipe(
                distinctUntilChanged(),
                tap(() => (this.isSearching = true)),
                switchMap((term) => this.searchWaterAccountContacts(term, this.val?.WaterAccountContactID))
            )
        );
    }

    private searchWaterAccountContacts(term: string, selectedWaterAccountContactID?: number): Observable<WaterAccountContactSearchSummaryDto> {
        const waterAccountContactSearchDto = new WaterAccountContactSearchDto();
        waterAccountContactSearchDto.GeographyID = this.geographyID;
        waterAccountContactSearchDto.SearchString = term;
        waterAccountContactSearchDto.WaterAccountContactID = selectedWaterAccountContactID;

        return this.searchService.searchWaterAccountContactsSearch(waterAccountContactSearchDto).pipe(
            catchError(() => of(new WaterAccountContactSearchSummaryDto())),
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
        const selectedValue = event?.WaterAccountContact;
        this.value = selectedValue;

        // Reset the search term to ensure fresh results on next open
        this.searchString.next("");
    }

    public val: WaterAccountContactSearchResultDto;
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
