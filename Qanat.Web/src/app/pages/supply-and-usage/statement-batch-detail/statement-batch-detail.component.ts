import { AsyncPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable, forkJoin, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { PageHeaderComponent } from "../../../shared/components/page-header/page-header.component";
import { KeyValuePairListComponent } from "../../../shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "../../../shared/components/key-value-pair/key-value-pair.component";
import { ReactiveFormsModule } from "@angular/forms";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { StatementBatchDto } from "src/app/shared/generated/model/statement-batch-dto";
import { StatementBatchByGeographyService } from "src/app/shared/generated/api/statement-batch-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { StatementBatchWaterAccountDto } from "src/app/shared/generated/model/statement-batch-water-account-dto";
import { QanatGridComponent } from "../../../shared/components/qanat-grid/qanat-grid.component";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import saveAs from "file-saver";

@Component({
    selector: "statement-batch-detail",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        ReactiveFormsModule,
        AlertDisplayComponent,
        DatePipe,
        QanatGridComponent,
        ButtonLoadingDirective,
        RouterLink,
    ],
    templateUrl: "./statement-batch-detail.component.html",
    styleUrl: "./statement-batch-detail.component.scss"
})
export class StatementBatchDetailComponent {
    public currentGeography$: Observable<GeographyMinimalDto>;
    public geographyID: number;
    public statementBatchData$: Observable<[StatementBatchDto, StatementBatchWaterAccountDto[]]>;

    public statementBatch: StatementBatchDto;
    public statementBatchWaterAccounts: StatementBatchWaterAccountDto[];
    public statementsGeneratedCount: number;

    public columnDefs: ColDef<StatementBatchWaterAccountDto>[];
    public gridApi: GridApi;

    public isGenerating: boolean = false;
    public isDownloading: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private statementBatchByGeographyService: StatementBatchByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private fileResourceService: FileResourceService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit() {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;

                this.statementBatchData$ = this.route.params.pipe(
                    switchMap((params) => {
                        const statementBatchID = params[routeParams.statementBatchID];
                        return forkJoin([
                            this.statementBatchByGeographyService.getStatementBatchByIDStatementBatchByGeography(geography.GeographyID, statementBatchID),
                            this.statementBatchByGeographyService.listStatementBatchWaterAccountsStatementBatchByGeography(geography.GeographyID, statementBatchID),
                        ]).pipe(
                            tap(([statementBatch, statementBatchWaterAccounts]) => {
                                this.statementBatch = statementBatch;
                                this.statementBatchWaterAccounts = statementBatchWaterAccounts;

                                if (!statementBatch.StatementsGenerated) {
                                    this.isGenerating = true;
                                    this.statementsGeneratedCount = statementBatchWaterAccounts.filter((x) => x.FileResourceGuid != null).length;
                                }

                                this.createColumnDefs();
                            })
                        );
                    })
                );
            })
        );
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [
                    {
                        ActionName: "Download PDF",
                        ActionHandler: () => this.downloadSingleStatement(params.data.WaterAccountID, params.data.FileResourceGuid),
                    },
                    {
                        ActionName: "Regenerate Statement",
                        ActionHandler: () => this.regenerateSingleStatement(params.data.WaterAccountID),
                    },
                ];
                return actions;
            }, !this.statementBatch.StatementsGenerated),
            this.utilityFunctionsService.createLinkColumnDef("Water Account #", "WaterAccountNumber", "WaterAccountID", { InRouterLink: "/water-accounts/" }),
            this.utilityFunctionsService.createBasicColumnDef("Account Name", "WaterAccountName"),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "ContactName"),
            this.utilityFunctionsService.createBasicColumnDef("Contact Address", "FullAddress"),
        ];
    }

    public regenerateAllStatements() {
        this.isGenerating = true;

        const options = {
            title: "Confirm: Regenerate All Statements",
            message: "Are you sure you want to regenerate all statements?",
            buttonClassYes: "btn-primary",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.statementBatchByGeographyService.generateBatchStatementsStatementBatchByGeography(this.geographyID, this.statementBatch.StatementBatchID).subscribe({
                    next: () => {
                        this.statementBatch.StatementsGenerated = false;
                        this.statementsGeneratedCount = 0;
                        this.gridApi.setColumnsVisible(["0"], false);
                    },
                    error: () => (this.isGenerating = false),
                });
            }
        });
    }

    public bulkDownloadStatements() {
        this.isDownloading = true;
        this.statementBatchByGeographyService.downloadStatementsStatementBatchByGeography(this.geographyID, this.statementBatch.StatementBatchID).subscribe({
            next: (response) => {
                this.isDownloading = false;
                saveAs(response, `${this.statementBatch.StatementBatchName}.zip`);

                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Statements successfully downloaded.", AlertContext.Success));
            },
            error: () => (this.isDownloading = false),
        });
    }

    public regenerateSingleStatement(waterAccountID: number) {
        if (!this.statementBatch.StatementsGenerated) return;

        this.alertService.pushAlert(new Alert("Regenerating statement...", AlertContext.Info));
        this.statementBatchByGeographyService
            .generateWaterAccountStatementStatementBatchByGeography(this.geographyID, this.statementBatch.StatementBatchID, waterAccountID)
            .subscribe((result) => {
                this.statementBatchWaterAccounts.find((x) => x.WaterAccountID == waterAccountID).FileResourceGuid = result.FileResourceGuid;

                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Statement successfully regenerated.", AlertContext.Success));
            });
    }

    public downloadSingleStatement(waterAccountID: number, fileResourceGuid: string) {
        if (!this.statementBatch.StatementsGenerated) return;
        this.alertService.pushAlert(new Alert("Downloading statement...", AlertContext.Info));

        const waterAccount = this.statementBatchWaterAccounts.find((x) => x.WaterAccountID == waterAccountID);
        var shortenedContactName = waterAccount.ContactName.length > 21 ? waterAccount.ContactName.substring(0, 21) : waterAccount.ContactName;

        this.fileResourceService.downloadFileResourceFileResource(fileResourceGuid).subscribe((response) => {
            saveAs(response, `${shortenedContactName}_${waterAccount.WaterAccountNumber}_${this.statementBatch.StatementBatchName}.pdf`);

            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Statement successfully downloaded.", AlertContext.Success));
        });
    }
}

export class SupportTicketContext {
    SupportTicketID: number;
}

export class SupportTicketUpdateContext extends SupportTicketContext {
    UpdateType: "Details" | "Status" = "Details";
    WaterAccountID: number;
    Description: string;
    SupportTicketPriorityID: number;
    GeographyID: number;
    ContactFirstName: string;
    ContactLastName: string;
    ContactEmail: string;
    ContactPhoneNumber: string;
    AssignedUserID: number;
}

export class SupportTicketResponseContext extends SupportTicketContext {
    IsInternalNote: boolean;
}
