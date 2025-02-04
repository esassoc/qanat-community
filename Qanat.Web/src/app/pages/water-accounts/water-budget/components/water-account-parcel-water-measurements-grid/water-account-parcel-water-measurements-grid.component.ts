import { AsyncPipe, DecimalPipe, NgIf } from "@angular/common";
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { MonthlyUsageSummaryDto, WaterAccountParcelWaterMeasurementDto } from "src/app/shared/generated/model/models";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";

@Component({
    selector: "water-account-parcel-water-measurements-grid",
    standalone: true,
    imports: [AsyncPipe, NgIf, QanatGridComponent, LoadingDirective],
    templateUrl: "./water-account-parcel-water-measurements-grid.component.html",
    styleUrl: "./water-account-parcel-water-measurements-grid.component.scss",
})
export class WaterAccountParcelWaterMeasurementsGridComponent implements OnChanges {
    @Input() geographyID: number;
    @Input() showAcreFeet: boolean;
    @Input() showWaterAccountRollup: boolean;
    @Input() waterAccountParcelWaterMeasurements: WaterAccountParcelWaterMeasurementDto[] = [];

    @Input() filterToWaterMeasurementTypeIDs: number[] = [];

    @Output() waterAccountParcelWaterMeasurementsChange = new EventEmitter<WaterAccountParcelWaterMeasurementDto[]>();

    public isLoading: boolean = true;
    public colDefs: ColDef<WaterAccountParcelWaterMeasurementDto>[];

    public measurements: WaterAccountParcelWaterMeasurementDto[] = [];

    private gridApi: GridApi;

    constructor(
        private decimalPipe: DecimalPipe,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (this.showWaterAccountRollup) {
            this.measurements = this.getWaterAccountRollups(this.waterAccountParcelWaterMeasurements);
        } else {
            this.measurements = this.waterAccountParcelWaterMeasurements;
        }

        if (this.filterToWaterMeasurementTypeIDs.length > 0) {
            this.measurements = this.measurements.filter((x) => this.filterToWaterMeasurementTypeIDs.includes(x.WaterMeasurementTypeID));
        }

        this.refreshGridColumns();
    }

    private refreshGridColumns() {
        this.colDefs = [
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", { InRouterLink: "../../../parcels/", CustomDropdownFilterField: "ParcelNumber" }),
            this.utilityFunctionsService.createBasicColumnDef("Measurement", "WaterMeasurementTypeName", {
                CustomDropdownFilterField: "WaterMeasurementTypeName",
                Width: 400,
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Total", "WaterMeasurementTotalValue", {
                ValueFormatter: (params) => {
                    if (params.data.WaterMeasurementTotalValue === null) return "-";
                    const value = this.showAcreFeet ? params.data.WaterMeasurementTotalValue : params.data.WaterMeasurementTotalValue / params.data.ParcelArea;
                    return this.decimalPipe.transform(value, "1.2-2");
                },
                CellStyle: { "font-weight": "bold" },
            }),
        ];

        if (this.measurements.length > 0) {
            this.measurements[0].WaterMeasurementMonthlyValues.forEach((x, index) => {
                const colDef = {
                    headerName: this.utilityFunctionsService.getMonthName(x.EffectiveMonth).substring(0, 3),
                    cellStyle: { "justify-content": "flex-end" },
                    filter: "agNumberColumnFilter",
                    valueGetter: (params) => {
                        return params.data.WaterMeasurementMonthlyValues[index].CurrentUsageAmount;
                    },
                    valueFormatter: (params) => {
                        if (params.data.WaterMeasurementMonthlyValues[index].CurrentUsageAmount == null) return "-";
                        const value = this.showAcreFeet
                            ? params.data.WaterMeasurementMonthlyValues[index].CurrentUsageAmount
                            : params.data.WaterMeasurementMonthlyValues[index].CurrentUsageAmount / params.data.ParcelArea;
                        return this.decimalPipe.transform(value, "1.2-2");
                    },
                } as ColDef<WaterAccountParcelWaterMeasurementDto>;

                this.colDefs.push(colDef);
            });

            if (this.gridApi) {
                setTimeout(() => {
                    this.gridApi.autoSizeColumns(["ParcelNumber"]);
                }, 1);
            }
        }

        this.colDefs.push(
            this.utilityFunctionsService.createBasicColumnDef("Measurement Type", "WaterMeasurementCategoryTypeName", {
                CustomDropdownFilterField: "WaterMeasurementCategoryTypeName",
            })
        );

        this.colDefs.push(
            this.utilityFunctionsService.createBasicColumnDef("Area", "ParcelArea", { ValueFormatter: (params) => this.decimalPipe.transform(params.value, "1.2-2") })
        );

        if (this.showWaterAccountRollup) {
            this.colDefs = this.colDefs.filter((colDef) => colDef.field !== "ParcelNumber");
        }

        this.isLoading = false;
    }

    getWaterAccountRollups(measurements: WaterAccountParcelWaterMeasurementDto[]): WaterAccountParcelWaterMeasurementDto[] {
        const rollupData = [];

        if (measurements) {
            const groupedByMeasurementTypeName = measurements.reduce((acc, curr) => {
                if (!acc[curr.WaterMeasurementTypeName]) {
                    acc[curr.WaterMeasurementTypeName] = { ...curr, WaterMeasurementTotalValue: null, WaterMeasurementMonthlyValues: [], UsageEntityArea: 0, ParcelArea: 0 };
                }

                acc[curr.WaterMeasurementTypeName].ParcelID = null; // We are summing parcels together so removing the parcel ID to prevent confusion.
                acc[curr.WaterMeasurementTypeName].ParcelNumber = null; // We are summing parcels together so removing the parcel number to prevent confusion.

                if (curr.WaterMeasurementTotalValue !== null) {
                    acc[curr.WaterMeasurementTypeName].WaterMeasurementTotalValue += curr.WaterMeasurementTotalValue;
                }

                acc[curr.WaterMeasurementTypeName].UsageEntityArea += curr.UsageEntityArea;
                acc[curr.WaterMeasurementTypeName].ParcelArea += curr.ParcelArea;

                curr.WaterMeasurementMonthlyValues.forEach((month, index) => {
                    if (!acc[curr.WaterMeasurementTypeName].WaterMeasurementMonthlyValues[index]) {
                        const monthlyUsageSummary = new MonthlyUsageSummaryDto();
                        monthlyUsageSummary.EffectiveDate = month.EffectiveDate;
                        monthlyUsageSummary.EffectiveMonth = month.EffectiveMonth;
                        monthlyUsageSummary.CurrentUsageAmount = null;
                        acc[curr.WaterMeasurementTypeName].WaterMeasurementMonthlyValues[index] = monthlyUsageSummary;
                    }
                    if (curr.WaterMeasurementMonthlyValues[index].CurrentUsageAmount !== null) {
                        acc[curr.WaterMeasurementTypeName].WaterMeasurementMonthlyValues[index].CurrentUsageAmount += curr.WaterMeasurementMonthlyValues[index].CurrentUsageAmount;
                    }
                });

                return acc;
            }, {});

            for (const key in groupedByMeasurementTypeName) {
                rollupData.push(groupedByMeasurementTypeName[key]);
            }
        }
        return rollupData;
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        setTimeout(() => {
            this.gridApi.autoSizeColumns(["ParcelNumber"]);
        });
    }
}
