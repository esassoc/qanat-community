import { AsyncPipe, DecimalPipe } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterLink, ActivatedRoute } from "@angular/router";
import { GridApi, ColDef, GridReadyEvent, RowSelectionOptions } from "ag-grid-community";
import { Observable, BehaviorSubject, switchMap, tap, combineLatest } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterAccountFallowStatusReviewService } from "src/app/shared/generated/api/water-account-fallow-status-review.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { WaterAccountFallowStatusDto } from "src/app/shared/generated/model/water-account-fallow-status-dto";
import { WaterMeasurementSelfReportSimpleDto } from "src/app/shared/generated/model/water-measurement-self-report-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "fallow-self-report-review",
    imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, QanatGridComponent, FormsModule, AsyncPipe, DecimalPipe, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./fallow-self-report-review.component.html",
    styleUrl: "./fallow-self-report-review.component.scss"
})
export class FallowSelfReportReviewComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public selfReports$: Observable<WaterAccountFallowStatusDto[]>;
    public refreshSelfReports$: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public gridApi: GridApi;
    public columnDefs: ColDef<WaterMeasurementSelfReportSimpleDto>[];
    public rowSelection: RowSelectionOptions = {
        mode: "multiRow",
    };

    public submittedCount: number = 0;
    public approvedCount: number = 0;

    public customRichTextTypeID = CustomRichTextTypeEnum.FallowSelfReportingReview;

    public selectedFallowStatusIDs: number[] = [];
    public isLoadingSubmit: boolean = false;

    public constructor(
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterAccountFallowStatusReviewService: WaterAccountFallowStatusReviewService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
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

        this.selfReports$ = combineLatest({ geography: this.geography$, _: this.refreshSelfReports$ }).pipe(
            switchMap(({ geography }) =>
                this.waterAccountFallowStatusReviewService.listWaterAccountFallowStatusReview(geography.GeographyID).pipe(
                    tap((reports) => {
                        this.submittedCount = reports.filter((report) => report.SelfReportStatus.SelfReportStatusID == SelfReportStatusEnum.Submitted).length;
                        this.approvedCount = reports.filter((report) => report.SelfReportStatus.SelfReportStatusID == SelfReportStatusEnum.Approved).length;
                    })
                )
            )
        );

        this.buildColumnDefs();
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;

        this.gridApi.sizeColumnsToFit();

        this.gridApi.setColumnFilterModel("SelfReportStatus.SelfReportStatusDisplayName", {
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

    public onSelectionChanged() {
        this.selectedFallowStatusIDs = this.gridApi.getSelectedNodes().map((x) => x.data.WaterAccountFallowStatusID);
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [];
                actions.push({
                    ActionName: "Review Report",
                    ActionIcon: "fas fa-review",
                    ActionLink: `/geographies/${params.data.Geography.GeographyID}/reporting-periods/${params.data.ReportingPeriod.ReportingPeriodID}/fallow-self-reports/${params.data.WaterAccountFallowStatusID}`,
                });
                return actions.length == 0 ? null : actions;
            }),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriod.Name"),
            this.utilityFunctionsService.createBasicColumnDef("Status", "SelfReportStatus.SelfReportStatusDisplayName", {
                CustomDropdownFilterField: "SelfReportStatus.SelfReportStatusDisplayName",
            }),
            this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccount.WaterAccountNameAndNumber", "WaterAccount.WaterAccountID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccount.WaterAccountID}/water-budget`, LinkDisplay: params.data.WaterAccount.WaterAccountNameAndNumber };
                },
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Usage Locations Fallowed", "CountOfFallowedUsageLocations", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Acres", "AcresFallowed"),
            this.utilityFunctionsService.createBasicColumnDef("Created by User", "CreateUser.FullName", {
                CustomDropdownFilterField: "CreateUser.FullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Created Date", "CreateDate", "M/d/yyyy", {}),
            this.utilityFunctionsService.createBasicColumnDef("Updated by User", "UpdateUser.FullName", {
                CustomDropdownFilterField: "UpdateUser.FullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Updated Date", "UpdateDate", "M/d/yyyy", {}),
            this.utilityFunctionsService.createBasicColumnDef("Submitted by User", "SubmittedByUser.FullName", {
                CustomDropdownFilterField: "SubmittedByUser.FullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Submitted Date", "SubmittedDate", "M/d/yyyy", {}),
            this.utilityFunctionsService.createBasicColumnDef("Approved By User", "ApprovedByUser.FullName", {
                CustomDropdownFilterField: "ApprovedByUser.FullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Approved Date", "ApprovedDate", "M/d/yyyy", {}),
            this.utilityFunctionsService.createBasicColumnDef("Returned By User", "ReturnedByUser.FullName", {
                CustomDropdownFilterField: "ReturnedByUser.FullName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Returned Date", "ReturnedDate", "M/d/yyyy", {}),
        ];
    }

    approve(geography: GeographyMinimalDto, fallowStatusIDs: number[]): void {
        this.isLoadingSubmit = true;
        this.waterAccountFallowStatusReviewService.approveWaterAccountFallowStatusReview(geography.GeographyID, fallowStatusIDs).subscribe({
            next: (result) => {
                this.alertService.pushAlert(new Alert(`Successfully approved fallow self reviews.`, AlertContext.Success));
                this.selectedFallowStatusIDs = [];
                this.buildColumnDefs(); // Rebuild column definitions to reflect changes
                this.refreshSelfReports$.next();
                this.isLoadingSubmit = false;
            },
            error: (error) => {
                this.alertService.pushAlert(new Alert(`There was an error approving fallow self reviews.`, AlertContext.Success));
                this.isLoadingSubmit = false;
            },
        });
    }

    return(geography: GeographyMinimalDto, fallowStatusIDs: number[]): void {
        this.isLoadingSubmit = true;
        this.waterAccountFallowStatusReviewService.returnWaterAccountFallowStatusReview(geography.GeographyID, fallowStatusIDs).subscribe({
            next: (result) => {
                this.alertService.pushAlert(new Alert(`Successfully returned fallow self reviews.`, AlertContext.Success));
                this.selectedFallowStatusIDs = [];
                this.gridApi.deselectAll();
                this.refreshSelfReports$.next();
                this.isLoadingSubmit = false;
            },
            error: (error) => {
                this.alertService.pushAlert(new Alert(`There was an error returning fallow self reviews.`, AlertContext.Danger));
                this.isLoadingSubmit = false;
            },
        });
    }
}
