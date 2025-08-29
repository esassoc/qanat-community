import { AsyncPipe, CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { BehaviorSubject, Observable, of, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { StatementTemplateByGeographyService } from "src/app/shared/generated/api/statement-template-by-geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { StatementTemplateDto } from "src/app/shared/generated/model/statement-template-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "statement-template-list",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, AsyncPipe, CommonModule, RouterLink],
    templateUrl: "./statement-template-list.component.html",
    styleUrl: "./statement-template-list.component.scss"
})
export class StatementTemplateListComponent {
    public refreshStatementTemplates$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public statementTemplateDtos$: Observable<StatementTemplateDto[]>;
    public currentGeography$: Observable<GeographyDto>;

    public columnDefs: ColDef[];
    public gridApi: GridApi;

    public isLoading = true;
    public customRichTextTypeID = CustomRichTextTypeEnum.StatementTemplateList;

    constructor(
        private statementTemplateByGeographyService: StatementTemplateByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(tap((geography) => this.refreshStatementTemplates$.next(geography.GeographyID)));

        this.statementTemplateDtos$ = this.refreshStatementTemplates$.pipe(
            switchMap((geographyID) => {
                if (!geographyID) return of([]);
                return this.statementTemplateByGeographyService.listStatementTemplatesByGeographyIDStatementTemplateByGeography(geographyID);
            }),
            tap(() => (this.isLoading = false))
        );

        this.initializeGrid();
    }

    private initializeGrid(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [
                    {
                        ActionName: "Edit",
                        ActionIcon: "fa fa-edit",
                        ActionLink: `${params.data.StatementTemplateID}/edit`,
                    },
                    {
                        ActionName: "Duplicate",
                        ActionIcon: "far fa-copy",
                        ActionHandler: () => {
                            this.confirmService
                                .confirm({
                                    title: "Duplicate Statement Template",
                                    message: `Are you sure you want to duplicate statement template <b>${params.data.TemplateName}</b>?`,
                                    buttonTextYes: "Duplicate",
                                    buttonClassYes: "btn-primary",
                                    buttonTextNo: "Cancel",
                                })
                                .then((confirmed) => {
                                    if (confirmed) {
                                        this.statementTemplateByGeographyService
                                            .duplicateStatementTemplateStatementTemplateByGeography(params.data.GeographyID, params.data.StatementTemplateID)
                                            .subscribe({
                                                next: () => {
                                                    this.refreshStatementTemplates$.next(params.data.GeographyID);
                                                    this.alertService.pushAlert(new Alert("Statement Template successfully duplicated", AlertContext.Success));
                                                },
                                            });
                                    }
                                });
                        },
                    },
                ];
                return actions;
            }),
            this.utilityFunctionsService.createBasicColumnDef("Template Title", "TemplateTitle"),
            this.utilityFunctionsService.createBasicColumnDef("Internal Description", "InternalDescription"),
            this.utilityFunctionsService.createBasicColumnDef("Template Type", "StatementTemplateType.StatementTemplateTypeDisplayName", {
                CustomDropdownFilterField: "StatementTemplateType.StatementTemplateTypeDisplayName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Updated By", "UpdateUserFullName"),
            this.utilityFunctionsService.createDateColumnDef("Last Updated", "LastUpdated", "short"),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }
}
