import { DatePipe, DecimalPipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { CellClassFunc, CellStyle, CellStyleFunc, ColDef, CsvExportParams, SortDirection, ValueFormatterFunc, ValueGetterFunc, ValueGetterParams } from "ag-grid-community";
import { CustomDropdownFilterComponent } from "../components/custom-dropdown-filter/custom-dropdown-filter.component";
import { FieldDefinitionGridHeaderComponent } from "../components/field-definition-grid-header/field-definition-grid-header.component";
import { CustomAttributeSimpleDto, WaterTypeSimpleDto, ZoneGroupMinimalDto } from "../generated/model/models";
import { LinkRendererComponent } from "../components/ag-grid/link-renderer/link-renderer.component";
import { ContextMenuRendererComponent } from "../components/ag-grid/context-menu/context-menu-renderer.component";
import { MultiLinkRendererComponent } from "../components/ag-grid/multi-link-renderer/multi-link-renderer.component";
import { WaterTypeFieldDefinitionGridHeaderComponent } from "../components/ag-grid/water-type-field-definition-grid-header/water-type-field-definition-grid-header.component";
import { PhonePipe } from "../pipes/phone.pipe";
import { ZonesRendererComponent } from "../components/ag-grid/zones-renderer/zones-renderer.component";

@Injectable({
    providedIn: "root",
})
export class UtilityFunctionsService {
    public static readonly months: string[] = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    public static readonly millisecondsInADay = 86400000;

    public static readonly actionsColumnID = "actions";

    constructor(
        private datePipe: DatePipe,
        private decimalPipe: DecimalPipe,
        private phonePipe: PhonePipe
    ) {}

    public getMonthName(monthNumber) {
        return UtilityFunctionsService.months[monthNumber - 1];
    }

    public getNumberFromMonth(month: string) {
        return UtilityFunctionsService.months.indexOf(month) + 1;
    }

    public booleanValueGetter(value: boolean, allowNullValues: boolean = true) {
        if (allowNullValues && value == null) return null;

        return value ? "Yes" : "No";
    }

    public stringToKebabCase(string: string): string {
        return string.replace(/[A-Z]+(?![a-z])|[A-Z]/g, ($, ofs) => (ofs ? "-" : "") + $.toLowerCase());
    }

    public formatDate(date: Date, format: string): string {
        const _datePipe = this.datePipe;
        return _datePipe.transform(date, format);
    }

    public minDate(): Date {
        return new Date("1900-01-01T00:00:00");
    }

    public maxDate(): Date {
        return new Date("2100-01-01T00:00:00");
    }

    public createActionsColumnDef(actionsValueGetter: ValueGetterFunc, hide: boolean = false): ColDef {
        return {
            headerName: "Actions",
            valueGetter: actionsValueGetter,
            cellRenderer: ContextMenuRendererComponent,
            colId: UtilityFunctionsService.actionsColumnID,
            cellClass: "context-menu-container",
            pinned: true,
            sortable: false,
            filter: false,
            suppressSizeToFit: true,
            suppressAutoSize: true,
            width: 100,
            maxWidth: 100,
            hide: hide,
        };
    }

    public defaultValueGetter(params: ValueGetterParams, fieldName: string, containingFieldName: string = "data") {
        const path = fieldName.split(".");
        return path.reduce((obj, key) => (obj != null ? obj[key] : null), containingFieldName ? params[containingFieldName] : params);
    }

    public createBasicColumnDef(headerName: string, fieldName: string, colDefParams?: QanatColumnDefParams): ColDef {
        const colDef: ColDef = {
            field: fieldName,
            headerName: headerName,
            valueGetter: (params) => this.defaultValueGetter(params, fieldName),
        };

        this.applyDefaultQanatColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public customDecimalValueGetter(value: number, decimalPlacesToDisplay: number = 2) {
        const _decimalPipe = this.decimalPipe;
        const formatString = `1.${decimalPlacesToDisplay}-${decimalPlacesToDisplay}`;

        return value != null ? _decimalPipe.transform(value, formatString) : null;
    }

    public decimalValueGetter(params: any, fieldName: string): number {
        const fieldNames = fieldName.split(".");

        // checks that each part of a nested field is not null
        let fieldValue = params.data;
        fieldNames.forEach((x) => {
            fieldValue = fieldValue[x];
            if (!fieldValue) {
                fieldValue = 0;
                return;
            }
        });

        return fieldValue;
    }

    public decimalComparator(id1: string, id2: string) {
        if (!id1) return -1;
        if (!id2) return 1;

        const value1 = parseFloat(id1.replace(",", ""));
        const value2 = parseFloat(id2.replace(",", ""));
        return value1 == value2 ? 0 : value1 > value2 ? 1 : -1;
    }

    public convertStringToDecimal(value: string): number {
        if (!value) return null;

        // accounting for parseFloat() function treating commas as decimals
        return parseFloat(value.replace(",", ""));
    }

    public createDecimalColumnDef(headerName: string, fieldName: string, decimalColumnDefParams?: DecimalColumnDefParams) {
        const _decimalPipe = this.decimalPipe;

        const decimalPlacesToDisplay = decimalColumnDefParams?.MaxDecimalPlacesToDisplay ?? 2;
        const minDecimalPlaces = decimalColumnDefParams?.MinDecimalPlacesToDisplay ?? decimalPlacesToDisplay;
        const decimalFormatString = "1." + minDecimalPlaces + "-" + decimalPlacesToDisplay;

        const decimalColDef: ColDef = {
            headerName: headerName,
            cellStyle: { "justify-content": "flex-end" },
            valueGetter: (params) => {
                const value = this.defaultValueGetter(params, fieldName);
                return value != null
                    ? _decimalPipe.transform(value, decimalFormatString)
                    : decimalColumnDefParams?.ZeroFillNullValues
                      ? _decimalPipe.transform(0, decimalFormatString)
                      : decimalColumnDefParams?.StringForNullValues
                        ? decimalColumnDefParams?.StringForNullValues
                        : null;
            },
            filter: "agNumberColumnFilter",
            filterValueGetter: (params) => this.convertStringToDecimal(_decimalPipe.transform(this.defaultValueGetter(params, fieldName), decimalFormatString)),
            comparator: this.decimalComparator,
        };

        this.applyDefaultQanatColumnDefParams(decimalColDef, decimalColumnDefParams);
        return decimalColDef;
    }

    public createLatLonColumnDef(headerName: "Latitude" | "Longitude", fieldName: string) {
        return this.createDecimalColumnDef(headerName, fieldName, { MaxDecimalPlacesToDisplay: 5 });
    }

    public createYearColumnDef(headerName: string, fieldName: string): ColDef {
        return {
            headerName: headerName,
            valueGetter: (params) => this.decimalValueGetter(params, fieldName),
            comparator: this.decimalComparator,
            filter: "agNumberColumnFilter",
            cellStyle: { "justify-content": "flex-end" },
        };
    }

    public createPhoneNumberColumnDef(headerName: string, fieldName: string): ColDef {
        return {
            headerName: headerName,
            field: fieldName,
            valueFormatter: (params) => this.phonePipe.transform(params.value),
            filterParams: {
                textFormatter: this.phonePipe.gridFilterTextFormatter,
            },
        };
    }

    public linkRendererComparator(id1: any, id2: any) {
        if (id1.LinkDisplay == id2.LinkDisplay) {
            return 0;
        }
        return id1.LinkDisplay > id2.LinkDisplay ? 1 : -1;
    }

    public createLinkColumnDef(headerName: string, fieldName: string, linkValueField: string, linkColumnDefParams?: LinkColumnDefParams) {
        const colDef: ColDef = {
            headerName: headerName,
            field: fieldName,
            valueGetter: (params) => {
                return {
                    LinkValue: this.defaultValueGetter(params, linkValueField),
                    LinkDisplay: this.defaultValueGetter(params, linkColumnDefParams?.LinkDisplayField ?? fieldName),
                };
            },
            filterValueGetter: (params) => this.defaultValueGetter(params, fieldName),
            comparator: this.linkRendererComparator,
            cellRenderer: LinkRendererComponent,
            cellRendererParams: { inRouterLink: linkColumnDefParams?.InRouterLink },
        };

        this.applyDefaultQanatColumnDefParams(colDef, linkColumnDefParams);
        return colDef;
    }

    public multiLinkRendererComparator(id1: any, id2: any) {
        if (id1.downloadDisplay == id2.downloadDisplay) {
            return 0;
        }
        return id1.downloadDisplay > id2.downloadDisplay ? 1 : -1;
    }

    public createMultiLinkColumnDef(
        headerName: string,
        listField: string,
        linkValueField: string,
        linkDisplayField: string,
        multiLinkColumnDefParams?: MultiLinkColumnDefParams
    ): ColDef {
        const colDef: ColDef = {
            headerName: headerName,
            valueGetter: (params) => {
                const names = this.defaultValueGetter(params, listField)?.map((x) => {
                    return { LinkValue: this.defaultValueGetter(x, linkValueField, ""), LinkDisplay: this.defaultValueGetter(x, linkDisplayField, "") };
                });
                const downloadDisplay = names?.map((x) => x.LinkDisplay).join(", ");
                return { links: names, downloadDisplay: downloadDisplay };
            },
            filterValueGetter: (params) =>
                this.defaultValueGetter(params, listField)
                    ?.map((x) => this.defaultValueGetter(x, linkDisplayField, ""))
                    .join(", "),
            comparator: this.multiLinkRendererComparator,
            cellRenderer: MultiLinkRendererComponent,
            cellRendererParams: { inRouterLink: multiLinkColumnDefParams?.InRouterLink },
        };

        this.applyDefaultQanatColumnDefParams(colDef, multiLinkColumnDefParams);
        return colDef;
    }

    private dateFilterComparator(filterLocalDateAtMidnight, cellValue) {
        const filterDate = Date.parse(filterLocalDateAtMidnight);
        const cellDate = Date.parse(cellValue);

        return cellDate == filterDate ? 0 : cellDate < filterDate ? -1 : 1;
    }

    public dateSortComparator(id1: any, id2: any) {
        const date1 = id1 ? Date.parse(id1) : Date.parse("1/1/1900");
        const date2 = id2 ? Date.parse(id2) : Date.parse("1/1/1900");

        return date1 == date2 ? 0 : date1 > date2 ? 1 : -1;
    }

    public createDateColumnDef(headerName: string, fieldName: string, dateFormat: string, dateColumnDefParams?: DateColumnDefParams): ColDef {
        const _datePipe = this.datePipe;
        const timezone = dateColumnDefParams?.IgnoreLocalTimezone ? "+0000" : null;

        const dateColDef: ColDef = {
            headerName: headerName,
            valueGetter: (params) => {
                const value = this.defaultValueGetter(params, fieldName);
                return _datePipe.transform(value, dateFormat, timezone);
            },
            comparator: this.dateSortComparator,
            filter: "agDateColumnFilter",
            filterParams: {
                filterOptions: ["inRange"],
                comparator: this.dateFilterComparator,
            },
            sort: dateColumnDefParams?.Sort,
        };

        this.applyDefaultQanatColumnDefParams(dateColDef, dateColumnDefParams);
        return dateColDef;
    }

    public createDaysPassedColumnDef(headerName: string, dateFieldName: string, colDefParams?: QanatColumnDefParams) {
        const colDef: ColDef = {
            field: "Days Open",
            valueGetter: (params) => {
                const currentDate = new Date();
                const dateCreated = new Date(params.data[dateFieldName]);

                const currentTime = currentDate.getTime();
                const startTime = dateCreated.getTime();

                const daysPassed = Math.round((currentTime - startTime) / UtilityFunctionsService.millisecondsInADay);
                return `${daysPassed} day${daysPassed > 1 ? "s" : ""}`;
            },
            cellStyle: { "justify-content": "flex-end" },
            filter: "agNumberColumnFilter",
        };

        this.applyDefaultQanatColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public createZoneGroupColumnDef(zoneGroup: ZoneGroupMinimalDto, fieldName: string, hide: boolean = false): ColDef {
        return {
            headerName: zoneGroup.ZoneGroupName,
            valueGetter: (params) => {
                if (params.data[fieldName]) {
                    const zoneIDs = params.data[fieldName].split(",");
                    const zones = zoneGroup.ZoneList.filter((x) => zoneIDs.includes(x.ZoneID.toString()));
                    return { zones: zones, downloadDisplay: zones?.length > 0 ? zones.map((x) => x.ZoneName).join(", ") : "N/A" };
                }
                return { zones: [], downloadDisplay: "N/A" };
            },
            filter: CustomDropdownFilterComponent,
            filterParams: {
                useDownloadDisplayValue: true,
            },
            cellRenderer: ZonesRendererComponent,
            hide: hide,
            comparator: (a: any, b: any) => {
                if (a.downloadDisplay == b.downloadDisplay) {
                    return 0;
                }

                return a.downloadDisplay > b.downloadDisplay ? 1 : -1;
            },
        };
    }

    public createCustomAttributeColumnDefs(customAttributes: CustomAttributeSimpleDto[], hide: boolean = false): ColDef[] {
        return customAttributes.map((x) => {
            return {
                headerName: x.CustomAttributeName,
                valueGetter: (params) => (params.data.CustomAttributes ? params.data.CustomAttributes[x.CustomAttributeName] : null),
                hide: hide,
            };
        });
    }

    public applyDefaultQanatColumnDefParams(colDef: ColDef, params: QanatColumnDefParams) {
        if (!params) return;

        if (params.FieldDefinitionType) {
            colDef.headerComponentParams = {
                innerHeaderComponent: FieldDefinitionGridHeaderComponent,
                innerHeaderComponentParams: {
                    fieldDefinitionType: params.FieldDefinitionType,
                    labelOverride: params.FieldDefinitionLabelOverride,
                },
            };
        }

        if (params.UseCustomDropdownFilter || params.CustomDropdownFilterField) {
            colDef.filter = CustomDropdownFilterComponent;
            colDef.filterParams = {
                field: params.CustomDropdownFilterField,
                columnContainsMultipleValues: params.ColumnContainsMultipleValues,
                filterPopup: true,
            };
            colDef.floatingFilter = false;
        }

        if (params.WaterType) {
            colDef.headerComponentParams = {
                innerHeaderComponent: WaterTypeFieldDefinitionGridHeaderComponent,
                innerHeaderComponentParams: { waterType: params.WaterType },
            };
        }
        if (params.Width) colDef.width = params.Width;
        if (params.MaxWidth) colDef.maxWidth = params.MaxWidth;
        if (params.Hide) colDef.hide = params.Hide;
        if (params.ValueGetter) colDef.valueGetter = params.ValueGetter;
        if (params.FilterValueGetter) colDef.filterValueGetter = params.FilterValueGetter;
        if (params.ValueFormatter) colDef.valueFormatter = params.ValueFormatter;
        if (params.CellClass) colDef.cellClass = params.CellClass;
        if (params.CellStyle) colDef.cellStyle = params.CellStyle;
        if (params.Sort) colDef.sort = params.Sort;
    }

    public exportGridToCsv(grid: AgGridAngular, fileName: string, columnKeys: Array<string>) {
        const params = {
            skipHeader: false,
            columnGroups: false,
            skipFooters: true,
            skipRowGroups: true,
            skipPinnedTop: true,
            skipPinnedBottom: true,
            allColumns: true,
            onlySelected: false,
            suppressQuotes: false,
            fileName: fileName,
            processCellCallback: function (p) {
                if (p.column.getColDef().cellRenderer) {
                    if (p.value.downloadDisplay) {
                        return p.value.downloadDisplay;
                    } else {
                        return p.value.LinkDisplay;
                    }
                } else {
                    return p.value;
                }
            },
        } as CsvExportParams;
        if (columnKeys) {
            // exclude actions column from export
            params.columnKeys = columnKeys.filter((x) => x !== UtilityFunctionsService.actionsColumnID);
        }
        grid.api.exportDataAsCsv(params);
    }

    public deepEqual(obj1: any, obj2: any): boolean {
        if (obj1 === obj2) {
            return true;
        }

        if (typeof obj1 !== "object" || typeof obj2 !== "object" || obj1 == null || obj2 == null) {
            return false;
        }

        const keys1 = Object.keys(obj1);
        const keys2 = Object.keys(obj2);
        if (keys1.length !== keys2.length) {
            return false;
        }

        return keys1.every((key) => this.deepEqual(obj1[key], obj2[key]));
    }
}

export interface QanatColumnDefParams {
    Width?: number;
    MaxWidth?: number;
    Hide?: boolean;
    FieldDefinitionType?: string;
    FieldDefinitionLabelOverride?: string;
    UseCustomDropdownFilter?: boolean; // use to enable CustomDropdownFilter without specifying a field
    CustomDropdownFilterField?: string;
    ColumnContainsMultipleValues?: boolean;
    WaterType?: WaterTypeSimpleDto;
    ValueGetter?: ValueGetterFunc;
    FilterValueGetter?: ValueGetterFunc;
    ValueFormatter?: ValueFormatterFunc;
    CellClass?: string | string[] | CellClassFunc;
    CellStyle?: CellStyle | CellStyleFunc;
    Sort?: SortDirection;
}

export interface LinkColumnDefParams extends QanatColumnDefParams {
    Width?: number;
    InRouterLink?: string;
    LinkDisplayField?: string;
}

export interface MultiLinkColumnDefParams extends QanatColumnDefParams {
    InRouterLink?: string;
}

export interface DecimalColumnDefParams extends QanatColumnDefParams {
    MinDecimalPlacesToDisplay?: number;
    MaxDecimalPlacesToDisplay?: number;
    ZeroFillNullValues?: boolean;
    StringForNullValues?: string;
}

export interface DateColumnDefParams extends QanatColumnDefParams {
    Sort?: SortDirection;
    IgnoreLocalTimezone?: boolean;
}
