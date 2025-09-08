import { AsyncPipe, CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import saveAs from "file-saver";
import { BehaviorSubject, Observable, of, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { StatementBatchByGeographyService } from "src/app/shared/generated/api/statement-batch-by-geography.service";
import { StatementTemplateByGeographyService } from "src/app/shared/generated/api/statement-template-by-geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { StatementBatchDto } from "src/app/shared/generated/model/statement-batch-dto";
import { StatementTemplateDto } from "src/app/shared/generated/model/statement-template-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "statement-batch-list",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, AsyncPipe, CommonModule, RouterLink],
    templateUrl: "./statement-batch-list.component.html",
    styleUrl: "./statement-batch-list.component.scss"
})
export class StatementBatchListComponent {
    public refreshStatementBatches$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public statementBatchDtos$: Observable<StatementTemplateDto[]>;
    public currentGeography$: Observable<GeographyDto>;
    public geographyID: number;

    public columnDefs: ColDef<StatementBatchDto>[];
    public gridApi: GridApi;

    public isLoading = true;
    public customRichTextTypeID = CustomRichTextTypeEnum.StatementList;

    constructor(
        private statementBatchByGeographyService: StatementBatchByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.refreshStatementBatches$.next(geography.GeographyID);
            })
        );

        this.statementBatchDtos$ = this.refreshStatementBatches$.pipe(
            switchMap((geographyID) => {
                if (!geographyID) return of([]);
                return this.statementBatchByGeographyService.listStatementBatchesByGeographyIDStatementBatchByGeography(geographyID).pipe(
                    tap(() => {
                        this.isLoading = false;
                    })
                );
            })
        );

        this.initializeGrid();
    }

    private initializeGrid(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [
                    {
                        ActionName: "Bulk Download PDFs",
                        ActionHandler: () => this.bulkDownloadStatements(params.data.StatementBatchID, params.data.StatementBatchName),
                    },
                    {
                        ActionName: "Regenerate All Statements",
                        ActionHandler: () => this.regenerateAllStatements(params.data.StatementBatchID),
                    },
                    {
                        ActionName: "Delete Batch",
                        ActionHandler: () => this.deleteStatementBatch(params.data.StatementBatchID),
                    },
                ];
                return actions;
            }),
            this.utilityFunctionsService.createLinkColumnDef("Name", "StatementBatchName", "StatementBatchID"),
            this.utilityFunctionsService.createDecimalColumnDef("# of Water Accounts", "NumberOfWaterAccounts", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createBasicColumnDef("Statement Template", "StatementTemplateTitle", {
                CustomDropdownFilterField: "StatementTemplateTitle",
            }),
            this.utilityFunctionsService.createDateColumnDef("Last Updated", "LastUpdated", "short"),
            this.utilityFunctionsService.createBasicColumnDef("Updated By", "UpdateUserFullName"),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public regenerateAllStatements(statementBatchID: number) {
        const options = {
            title: "Confirm: Regenerate All Statements",
            message: "Are you sure you want to regenerate all statements?",
            buttonClassYes: "btn-primary",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.statementBatchByGeographyService.generateBatchStatementsStatementBatchByGeography(this.geographyID, statementBatchID).subscribe(() => {
                    this.alertService.pushAlert(new Alert("Regenerating all statements. This may take several minutes to complete.", AlertContext.Success));
                    this.refreshStatementBatches$.next(this.geographyID);
                });
            }
        });
    }

    public bulkDownloadStatements(statementBatchID: number, statementBatchName: string) {
        this.alertService.pushAlert(new Alert("Downloading statements...", AlertContext.Info));
        this.statementBatchByGeographyService.downloadStatementsStatementBatchByGeography(this.geographyID, statementBatchID).subscribe((response) => {
            saveAs(response, `${statementBatchName}.zip`);
        });
    }

    public deleteStatementBatch(statementBatchID: number) {
        const options = {
            title: "Confirm: Delete Statement Batch",
            message: "Are you sure you want to delete this statement batch? This action cannot be undone.",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.statementBatchByGeographyService.deleteStatementBatchByIDStatementBatchByGeography(this.geographyID, statementBatchID).subscribe(() => {
                    this.alertService.pushAlert(new Alert("Statement batch successfully deleted.", AlertContext.Success));
                    this.refreshStatementBatches$.next(this.geographyID);
                });
            }
        });
    }
}
