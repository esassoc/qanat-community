import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef, ValueGetterFunc } from "ag-grid-community";
import { Subscription } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { OpenETSyncService } from "src/app/shared/generated/api/open-et-sync.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { OpenETRunDto } from "src/app/shared/generated/model/open-et-run-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { OpenETSyncDto } from "src/app/shared/generated/model/open-et-sync-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import saveAs from "file-saver";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";

@Component({
    selector: "openet-sync-integration",
    templateUrl: "./openet-sync-integration.component.html",
    styleUrls: ["./openet-sync-integration.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent],
})
export class OpenetSyncIntegrationComponent implements OnInit, OnDestroy {
    public selectedGeography$ = Subscription.EMPTY;

    @ViewChild("openETGrid") openETGrid: AgGridAngular;
    @ViewChild("openETSyncModel") openETSyncModel;
    @ViewChild("openETFinalizeModel") openETFinalizeModel;

    private currentUser: UserDto;
    private geographyID: number;

    public richTextTypeID: number = CustomRichTextTypeEnum.OpenETSyncIntegration;

    public columnDefs: ColDef[];

    public openETSyncs: OpenETSyncDto[];
    public isLoadingSubmit: boolean;
    public openETSyncIDToFinalize: number;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    constructor(
        private authenticationService: AuthenticationService,
        private openETSyncService: OpenETSyncService,
        private utilityFunctionsService: UtilityFunctionsService,
        private cdr: ChangeDetectorRef,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private selectedGeographyService: SelectedGeographyService,
        private fileResourceService: FileResourceService
    ) {}

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID();
        });
    }

    getDataForGeographyID() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
            this.openETGrid?.api.showLoadingOverlay();
            this.initializeGrid();
            this.updateGridData();
        });
    }

    public updateGridData(): void {
        this.openETSyncService.geographiesGeographyIDOpenEtSyncsGet(this.geographyID).subscribe((openETSyncs) => {
            this.openETSyncs = openETSyncs;
            this.pushSyncAlert(openETSyncs);
            this.cdr.detectChanges();
        });
    }

    initializeGrid() {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef(this.createActionsColumnValueGetter()),
            {
                headerName: "Year",
                field: "Year",
                filter: "agNumberColumnFilter",
                width: 100,
            },
            {
                headerName: "Month",
                valueGetter: (params) => this.utilityFunctionsService.getMonthName(params.data.Month),
                width: 100,
            },
            this.utilityFunctionsService.createBasicColumnDef("Variable", "OpenETDataType.OpenETDataTypeDisplayName", {
                FieldDefinitionType: "OpenETSyncVariable",
                CustomDropdownFilterField: "OpenETDataType.OpenETDataTypeDisplayName",
            }),

            this.utilityFunctionsService.createBasicColumnDef("Last Sync Status", "LastSyncStatus.OpenETSyncResultTypeDisplayName", {
                CustomDropdownFilterField: "LastSyncStatus.OpenETSyncResultTypeDisplayName",
            }),
            this.utilityFunctionsService.createDateColumnDef("LastSyncDate", "LastSyncDate", "M/d/yyyy", { FieldDefinitionType: "LastSyncDate" }),
            this.utilityFunctionsService.createDateColumnDef("LastSuccessfulSyncDate", "LastSuccessfulSyncDate", "M/d/yyyy", { FieldDefinitionType: "LastSuccessfulSyncDate" }),
            {
                headerName: "Last Sync Message",
                field: "LastSyncMessage",
                flex: 1,
            },

            this.utilityFunctionsService.createBasicColumnDef("Last Calculation Status", "LastRasterCalculationStatus.OpenETRasterCalculationResultTypeDisplayName", {
                CustomDropdownFilterField: "LastRasterCalculationStatus.OpenETRasterCalculationResultTypeDisplayName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Last Calculation Date", "LastRasterCalculationDate", "M/d/yyyy"),
            this.utilityFunctionsService.createDateColumnDef("Last Successful Calculation Date", "LastSuccessfulCalculationDate", "M/d/yyyy"),
            {
                headerName: "Last Calculation Message",
                field: "LastRasterCalculationMessage",
                flex: 1,
            },

            this.utilityFunctionsService.createDateColumnDef("DateFinalized", "FinalizeDate", "M/d/yyyy", { FieldDefinitionType: "DateFinalized" }),
        ];
    }

    pushSyncAlert(openETSyncs: OpenETSyncDto[]) {
        const inProgressSyncs = openETSyncs.filter((x) => x.HasInProgressSync);
        if (inProgressSyncs.length > 0) {
            this.alertService.pushAlert(
                new Alert(
                    `Sync in progress for ${inProgressSyncs.map((x) => this.utilityFunctionsService.getMonthName(x.Month) + " " + x.Year).join(", ")}`,
                    AlertContext.Info,
                    false
                )
            );
        }

        const inProgressCalculations = openETSyncs.filter((x) => x.LastRasterCalculationStatus?.OpenETRasterCalculationResultTypeName == "InProgress");
        if (inProgressCalculations.length > 0) {
            this.alertService.pushAlert(
                new Alert(
                    `Calculation in progress for ${inProgressCalculations.map((x) => this.utilityFunctionsService.getMonthName(x.Month) + " " + x.Year).join(", ")}`,
                    AlertContext.Info,
                    false
                )
            );
        }
    }

    public createActionsColumnValueGetter(): ValueGetterFunc {
        return (params: any) => {
            if (params.data.FinalizeDate != null) {
                return null;
            }

            const isSyncSuccesfull =
                params?.data?.LastSyncStatus?.OpenETSyncResultTypeDisplayName == "Succeeded" || params?.data?.LastSyncStatus?.OpenETSyncResultTypeDisplayName == "No New Data";
            const actions = [
                {
                    ActionName: isSyncSuccesfull ? "Resync" : "Sync",
                    ActionHandler: () => {
                        this.confirmService
                            .confirm({
                                title: "Sync Now",
                                message: `Are you sure you want to query OpenET for data updates for ${this.utilityFunctionsService.getMonthName(params.data.Month)} ${params.data.Year}? Note: This may take some time to return data.`,
                                buttonTextYes: "Sync",
                                buttonClassYes: "btn-primary",
                                buttonTextNo: "Cancel",
                            })
                            .then((confirmed) => {
                                if (confirmed) {
                                    this.isLoadingSubmit = true;
                                    const openETRunDto = new OpenETRunDto();
                                    openETRunDto.Year = params.data.Year;
                                    openETRunDto.Month = params.data.Month;
                                    openETRunDto.OpenETDataTypeID = params.data.OpenETDataType.OpenETDataTypeID;

                                    this.openETSyncService.geographiesGeographyIDOpenEtSyncsPost(this.geographyID, openETRunDto).subscribe(
                                        () => {
                                            this.isLoadingSubmit = false;
                                            this.alertService.pushAlert(
                                                new Alert(
                                                    `Sync started for ${this.utilityFunctionsService.getMonthName(params.data.Month)} ${params.data.Year} ${params.data.OpenETDataType.OpenETDataTypeDisplayName}, please check again in a few minutes.`,
                                                    AlertContext.Success,
                                                    true
                                                )
                                            );

                                            (this.openETGrid as any)?.gridApi?.showLoadingOverlay();
                                            this.updateGridData();
                                            (this.openETGrid as any)?.hideOverlay();
                                            this.cdr.detectChanges();
                                        },
                                        (error) => {
                                            this.isLoadingSubmit = false;
                                        }
                                    );
                                }
                            });
                    },
                },
            ];

            if (isSyncSuccesfull) {
                if (params.data.FileResourceGUID) {
                    actions.push({
                        ActionName: "Download Raster",
                        ActionHandler: () => {
                            this.fileResourceService.fileResourcesFileResourceGuidAsStringGet(params.data.FileResourceGUID).subscribe((response) => {
                                saveAs(response, `${params.data.FileResourceOriginalName}.${params.data.FileResourceOriginalFileExtension}`);
                            });
                        },
                    });

                    actions.push({
                        ActionName: "Run Raster Calculation",
                        ActionHandler: () => {
                            this.confirmService
                                .confirm({
                                    title: "Run Raster Calculation",
                                    message: `Are you sure you would like to run the raster calculation for ${this.utilityFunctionsService.getMonthName(params.data.Month)} ${params.data.Year}?`,
                                    buttonTextYes: "Run Calculation",
                                    buttonClassYes: "btn-primary",
                                    buttonTextNo: "Cancel",
                                })
                                .then((confirmed) => {
                                    if (confirmed) {
                                        this.isLoadingSubmit = true;
                                        this.openETSyncService.geographiesGeographyIDOpenEtSyncsOpenETSyncIDCalculatePut(this.geographyID, params.data.OpenETSyncID).subscribe(
                                            () => {
                                                this.isLoadingSubmit = false;
                                                this.alertService.pushAlert(
                                                    new Alert(
                                                        `Calculation started for ${this.utilityFunctionsService.getMonthName(params.data.Month)} ${params.data.Year} ${params.data.OpenETDataType.OpenETDataTypeDisplayName}, please check again in a few minutes.`,
                                                        AlertContext.Success,
                                                        true
                                                    )
                                                );
                                            },
                                            (error) => {
                                                this.isLoadingSubmit = false;
                                            }
                                        );
                                    }
                                });
                        },
                    });
                    if (params.data.LastRasterCalculationStatus?.OpenETRasterCalculationResultTypeDisplayName == "Succeeded") {
                        actions.push({
                            ActionName: "Finalize",
                            ActionHandler: () => {
                                this.confirmService
                                    .confirm({
                                        title: "Finalize",
                                        message: `Are you sure you would like to finalize ${this.utilityFunctionsService.getMonthName(params.data.Month)} ${params.data.Year}?  The system will no longer query OpenET for updates once marked as final.`,
                                        buttonTextYes: "Finalize",
                                        buttonClassYes: "btn-primary",
                                        buttonTextNo: "Cancel",
                                    })
                                    .then((confirmed) => {
                                        if (confirmed) {
                                            this.isLoadingSubmit = true;
                                            this.openETSyncService.geographiesGeographyIDOpenEtSyncsOpenETSyncIDFinalizePut(this.geographyID, params.data.OpenETSyncID).subscribe(
                                                () => {
                                                    this.isLoadingSubmit = false;
                                                    this.alertService.pushAlert(new Alert(`Successfully finalized!`, AlertContext.Success, true));
                                                    this.updateGridData();
                                                },
                                                (error) => {
                                                    this.isLoadingSubmit = false;
                                                }
                                            );
                                        }
                                    });
                            },
                        });
                    }
                }
            }

            return actions;
        };
    }
}
