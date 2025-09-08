import { Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, combineLatest, Observable, of, switchMap, tap } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WaterAccountBudgetReportDto } from "src/app/shared/generated/model/water-account-budget-report-dto";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { ReportingPeriodSelectComponent } from "../../../shared/components/reporting-period-select/reporting-period-select.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto, ReportingPeriodDto, ZoneGroupDto } from "src/app/shared/generated/model/models";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";

@Component({
    selector: "water-account-budgets-report",
    templateUrl: "./water-account-budgets-report.component.html",
    styleUrls: ["./water-account-budgets-report.component.scss"],
    imports: [AsyncPipe, LoadingDirective, PageHeaderComponent, AlertDisplayComponent, ReportingPeriodSelectComponent, QanatGridComponent],
})
export class WaterAccountBudgetsReportComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;

    private selectedYearSubject = new BehaviorSubject<number | null>(null);
    public selectedYear$ = this.selectedYearSubject.asObservable();

    public waterTypes$: Observable<WaterTypeSimpleDto[]>;
    public waterAccountBudgetReports$: Observable<WaterAccountBudgetReportDto[]>;

    public zoneGroups$: Observable<ZoneGroupDto[]>;

    public columnDefs$: Observable<ColDef[]>;

    public richTextTypeID = CustomRichTextTypeEnum.WaterAccountBudgetReport;
    public isLoading = true;

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private zoneGroupService: ZoneGroupService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.listZoneGroup(geography.GeographyID);
            })
        );

        this.waterTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterTypeByGeographyService.getActiveWaterTypesWaterTypeByGeography(geography.GeographyID);
            }),
            tap((waterTypes) => {
                this.isLoading = false;
            })
        );

        this.waterAccountBudgetReports$ = combineLatest([this.geography$, this.selectedYear$]).pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(([geography, selectedYear]) => {
                return this.waterAccountByGeographyService.getWaterAccountsBudgetWaterAccountByGeography(geography.GeographyID, selectedYear);
            }),
            tap(() => {
                this.isLoading = false;
            })
        );

        this.columnDefs$ = combineLatest({ waterTypes: this.waterTypes$, zoneGroups: this.zoneGroups$ }).pipe(
            switchMap(({ waterTypes, zoneGroups }) => {
                let colDefs = this.createColumnDefs(waterTypes, zoneGroups);
                return of(colDefs);
            })
        );
    }

    private createColumnDefs(waterTypes: WaterTypeSimpleDto[], zoneGroups: ZoneGroupDto[]): ColDef[] {
        let columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Water Account #", "WaterAccountNumber", "WaterAccountUrl", {
                FieldDefinitionType: "WaterAccount",
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccountID}/water-budget`, LinkDisplay: params.data.WaterAccountNumber };
                },
                InRouterLink: "../../../water-accounts/",
            }),
            { headerName: "Water Account Name", field: "WaterAccountName", width: 155 },
            this.utilityFunctionsService.createDecimalColumnDef("Total Supply (ac-ft)", "TotalSupply"),
            ...waterTypes.map((waterType) => {
                const fieldName = "WaterSupplyByWaterType." + waterType.WaterTypeID;
                return this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, fieldName, { WaterType: waterType });
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Total Usage (ac-ft)", "UsageToDate"),
            this.utilityFunctionsService.createDecimalColumnDef("Current Available (ac-ft)", "CurrentAvailable"),
            this.utilityFunctionsService.createBasicColumnDef("Parcel Count", "ParcelCount", {
                CellStyle: { "justify-content": "flex-end" },
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Parcel Acres (ac)", "ParcelArea"),
            this.utilityFunctionsService.createDecimalColumnDef("Usage Location Acres(ac)", "UsageLocationArea"),
        ];

        zoneGroups.forEach((zoneGroup) => {
            columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "ZoneIDs", false));
        });

        return columnDefs;
    }

    onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        let endDate = new Date(selectedReportingPeriod.EndDate);
        let selectedYear = endDate.getUTCFullYear();
        this.selectedYearSubject.next(selectedYear);
    }
}
