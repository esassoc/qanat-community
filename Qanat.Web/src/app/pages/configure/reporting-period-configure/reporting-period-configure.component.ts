import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { AgGridModule } from "ag-grid-angular";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { BehaviorSubject, combineLatest, Observable, switchMap, tap } from "rxjs";
import { ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { UpsertReportingPeriodModalComponent } from "./upsert-reporting-period-modal/upsert-reporting-period-modal.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { DialogService } from "@ngneat/dialog";
import { CopyWaterAccountParcelsModalComponent } from "./copy-water-account-parcels-modal/copy-water-account-parcels-modal.component";

@Component({
    selector: "reporting-period-configure",
    templateUrl: "./reporting-period-configure.component.html",
    styleUrls: ["./reporting-period-configure.component.scss"],
    imports: [AsyncPipe, LoadingDirective, PageHeaderComponent, FormsModule, AgGridModule, QanatGridComponent, AlertDisplayComponent],
})
export class ReportingPeriodConfigureComponent implements OnInit {
    public isLoading: boolean = true;

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public refreshReportingPeriodsTrigger: BehaviorSubject<void> = new BehaviorSubject(null);
    public refreshReportingPeriodsTrigger$: Observable<void> = this.refreshReportingPeriodsTrigger.asObservable();

    private geographyID: number;

    public gridApi: GridApi;
    public columnDefs: ColDef[];

    public richTextID: number = CustomRichTextTypeEnum.ReportingPeriodConfiguration;

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        //MK 2/3/2025 -- Configure layout menu is watching the route and setting the current geography.
        this.reportingPeriods$ = combineLatest({ geography: this.currentGeographyService.getCurrentGeography(), _: this.refreshReportingPeriodsTrigger$ }).pipe(
            tap(({ geography }) => {
                this.isLoading = true;
                this.geographyID = geography.GeographyID;
            }),
            switchMap(({ geography }) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                this.isLoading = false;

                this.initializeGrid(reportingPeriods);
                if (this.gridApi) {
                    setTimeout(() => {
                        this.gridApi.sizeColumnsToFit();
                    }, 1); //MK 2/3/2025 -- This is a hack to get the grid to resize properly. Really don't like it, but need to move on.
                }
            })
        );
    }

    initializeGrid(reportingPeriods: ReportingPeriodDto[]): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.updateReportingPeriod(params.data as ReportingPeriodDto),
                    },
                    {
                        ActionName: "Copy Parcel Water Account Assignments",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.copyWaterAccountParcels(params.data as ReportingPeriodDto, reportingPeriods),
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("Name", "Name"),
            this.utilityFunctionsService.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createBasicColumnDef("Ready for Account Holders", "ReadyForAccountHolders", {
                ValueGetter: (params) => (params.data.ReadyForAccountHolders ? "Yes" : "No"),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Is Default", "IsDefault", {
                ValueGetter: (params) => (params.data.IsDefault ? "Yes" : "No"),
                UseCustomDropdownFilter: true,
            }),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridApi.sizeColumnsToFit();
    }

    addReportingPeriod() {
        const dialogRef = this.dialogService.open(UpsertReportingPeriodModalComponent, {
            data: {
                GeographyID: this.geographyID,
                ReportingPeriod: null,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshReportingPeriodsTrigger.next();
            }
        });
    }

    updateReportingPeriod(reportingPeriod: ReportingPeriodDto) {
        const dialogRef = this.dialogService.open(UpsertReportingPeriodModalComponent, {
            data: {
                GeographyID: this.geographyID,
                ReportingPeriod: reportingPeriod,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshReportingPeriodsTrigger.next();
            }
        });
    }

    copyWaterAccountParcels(reportingPeriod: ReportingPeriodDto, reportingPeriods: ReportingPeriodDto[]) {
        const dialogRef = this.dialogService.open(CopyWaterAccountParcelsModalComponent, {
            data: {
                GeographyID: this.geographyID,
                ToReportingPeriod: reportingPeriod,
                ReportingPeriods: reportingPeriods,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshReportingPeriodsTrigger.next();
            }
        });
    }
}
