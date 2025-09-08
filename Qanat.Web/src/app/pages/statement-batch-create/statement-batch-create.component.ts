import { Component, OnInit } from "@angular/core";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { BehaviorSubject, Observable, map, of, switchMap, tap } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { GeographyMinimalDto, WaterAccountIndexGridDto, StatementTemplateDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { StatementBatchEditModalComponent } from "src/app/shared/components/statement-batch-edit-modal/statement-batch-edit-modal.component";
import { StatementTemplateByGeographyService } from "src/app/shared/generated/api/statement-template-by-geography.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ReportingPeriodSelectComponent } from "../../shared/components/reporting-period-select/reporting-period-select.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "statement-batch-create",
    templateUrl: "./statement-batch-create.component.html",
    styleUrl: "./statement-batch-create.component.scss",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, RouterLink, AsyncPipe, ReportingPeriodSelectComponent],
})
export class StatementBatchCreateComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public geographyID: number;

    public isLoadingSubmit: boolean = false;

    public selectedYear: number;
    public selectedReportingPeriodID: number;

    public refreshWaterAccountsByYear$: BehaviorSubject<number> = new BehaviorSubject(null);

    public waterAccounts$: Observable<WaterAccountIndexGridDto[]>;
    public statementTemplates$: Observable<StatementTemplateDto[]>;
    public noStatementTemplatesConfigured: boolean = false;

    public columnDefs: ColDef<WaterAccountIndexGridDto>[];
    public selectedWaterAccountIDs: number[] = [];
    public gridApi: GridApi;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public richTextTypeID: number = CustomRichTextTypeEnum.NewStatementBatch;
    public geography: GeographyDto;
    public isLoading: boolean = true;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private statementTemplateByGeographyService: StatementTemplateByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    ngOnInit() {
        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;

                this.statementTemplates$ = this.statementTemplateByGeographyService.listStatementTemplatesByGeographyIDStatementTemplateByGeography(geography.GeographyID).pipe(
                    tap((statementTemplates) => {
                        if (statementTemplates.length == 0) {
                            this.noStatementTemplatesConfigured = true;
                            this.alertService.pushAlert(
                                new Alert(
                                    "No Statement Templates have been configured for this geography yet. Add a Statement Template from the Geography Configuration menu in order to create Statement Batches.",
                                    AlertContext.Warning,
                                    false
                                )
                            );
                            this.isLoading = false;
                        }
                    })
                );

                this.waterAccounts$ = this.refreshWaterAccountsByYear$.pipe(
                    tap(() => {
                        this.isLoading = true;
                    }),
                    switchMap((year) => {
                        if (!year) return of([]);
                        return this.waterAccountByGeographyService.listByGeographyIDAndYearWaterAccountByGeography(geography.GeographyID, year);
                    }),
                    map((waterAccounts) => waterAccounts.filter((x) => x.Parcels.length > 0)),
                    tap(() => {
                        this.isLoading = false;
                    })
                );
            })
        );

        this.createColumnDefs();
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Account Number", "WaterAccountNumber", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccountID}`, LinkDisplay: params.data.WaterAccountNumber };
                },
            }),
            { headerName: "Account Name", field: "WaterAccountName" },
            { headerName: "Contact Name", field: "WaterAccountContact.ContactName" },
            { headerName: "Contact Address", field: "WaterAccountContact.FullAddress" },
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onSelectionChanged() {
        this.selectedWaterAccountIDs = this.gridApi.getSelectedNodes().map((x) => x.data.WaterAccountID);
    }

    public onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        this.selectedReportingPeriodID = selectedReportingPeriod.ReportingPeriodID;

        let endDate = new Date(selectedReportingPeriod.EndDate);
        this.selectedYear = endDate.getUTCFullYear();

        this.refreshWaterAccountsByYear$.next(this.selectedYear);
    }

    public createStatementBatch() {
        const dialogRef = this.dialogService.open(StatementBatchEditModalComponent, {
            data: {
                GeographyID: this.geographyID,
                ReportingPeriodID: this.selectedReportingPeriodID,
                WaterAccountIDs: this.selectedWaterAccountIDs,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.router.navigate(["..", `${result.StatementBatchDto.StatementBatchID}`], { relativeTo: this.route });
            }
        });
    }
}
