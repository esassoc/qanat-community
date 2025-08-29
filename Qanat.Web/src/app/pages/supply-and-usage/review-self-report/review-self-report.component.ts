import { AsyncPipe, DecimalPipe } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterMeasurementReviewSelfReportService } from "src/app/shared/generated/api/water-measurement-review-self-report.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { WaterMeasurementSelfReportSimpleDto } from "src/app/shared/generated/model/water-measurement-self-report-simple-dto";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "review-self-report",
    imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, QanatGridComponent, FormsModule, AsyncPipe, DecimalPipe, LoadingDirective],
    templateUrl: "./review-self-report.component.html",
    styleUrl: "./review-self-report.component.scss"
})
export class ReviewSelfReportComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public selfReports$: Observable<WaterMeasurementSelfReportSimpleDto[]>;

    public gridApi: GridApi;
    public columnDefs: ColDef<WaterMeasurementSelfReportSimpleDto>[];

    public submittedCount: number = 0;
    public approvedCount: number = 0;

    public customRichTextTypeID = CustomRichTextTypeEnum.ReviewSelfReportList;

    public constructor(
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementReviewSelfReportService: WaterMeasurementReviewSelfReportService,
        private utilityFunctionsService: UtilityFunctionsService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                let geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => this.currentGeographyService.setCurrentGeography(geography))
        );

        this.selfReports$ = this.geography$.pipe(
            switchMap((geography) =>
                this.waterMeasurementReviewSelfReportService.readListWaterMeasurementReviewSelfReport(geography.GeographyID).pipe(
                    tap((reports) => {
                        this.submittedCount = reports.filter((report) => report.WaterMeasurementSelfReportStatusID == SelfReportStatusEnum.Submitted).length;
                        this.approvedCount = reports.filter((report) => report.WaterMeasurementSelfReportStatusID == SelfReportStatusEnum.Approved).length;
                    })
                )
            )
        );

        this.buildColumnDefs();
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;

        this.gridApi.sizeColumnsToFit();

        this.gridApi.setColumnFilterModel("WaterMeasurementSelfReportStatusDisplayName", {
            filtersActive: {
                selectAll: false,
                deselectAll: false,
                strict: false,
                filterOptions: {
                    Draft: false,
                    Submitted: true,
                    Approved: false,
                    Returned: true,
                },
            },
        });

        this.gridApi.onFilterChanged();
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [];
                actions.push({
                    ActionName: "Review Report",
                    ActionIcon: "fas fa-review",
                    ActionLink: `/water-accounts/${params.data.WaterAccountID}/water-measurement-self-reports/${params.data.WaterMeasurementSelfReportID}`,
                });
                return actions.length == 0 ? null : actions;
            }),
            {
                headerName: "Reporting Period",
                field: "ReportingPeriodName",
                filter: "agNumberColumnFilter",
            },
            this.utilityFunctionsService.createBasicColumnDef("Status", "WaterMeasurementSelfReportStatusDisplayName", {
                CustomDropdownFilterField: "WaterMeasurementSelfReportStatusDisplayName",
            }),
            this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccountNumberAndName", "WaterAccountID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccountID}/water-budget`, LinkDisplay: params.data.WaterAccountNumberAndName };
                },
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Water Measurement Type", "WaterMeasurementTypeName", {
                CustomDropdownFilterField: "WaterMeasurementTypeName",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume", "TotalVolume", {}),
            this.utilityFunctionsService.createBasicColumnDef("Document Count", "FileCount", {}),
            this.utilityFunctionsService.createBasicColumnDef("Created by User", "CreateUserFullName", {
                CustomDropdownFilterField: "CreateUserFullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Created Date", "CreateDate", "M/d/yyyy", {}),
            this.utilityFunctionsService.createBasicColumnDef("Updated by User", "UpdateUserFullName", {
                CustomDropdownFilterField: "UpdateUserFullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Updated Date", "UpdateDate", "M/d/yyyy", {}),
        ];
    }
}
