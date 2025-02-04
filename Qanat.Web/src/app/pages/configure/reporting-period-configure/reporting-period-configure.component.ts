import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgIf, AsyncPipe } from "@angular/common";
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
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { UpsertReportingPeriodModalComponent, UpsertReportingPeriodModalContext } from "./upsert-reporting-period-modal/upsert-reporting-period-modal.component";
@Component({
    selector: "reporting-period-configure",
    templateUrl: "./reporting-period-configure.component.html",
    styleUrls: ["./reporting-period-configure.component.scss"],
    standalone: true,
    imports: [AsyncPipe, LoadingDirective, PageHeaderComponent, NgIf, FormsModule, AgGridModule, QanatGridComponent],
})
export class ReportingPeriodConfigureComponent implements OnInit {
    public isLoading: boolean = true;

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public refreshReportingPeriodsTrigger: BehaviorSubject<void> = new BehaviorSubject(null);
    public refreshReportingPeriodsTrigger$: Observable<void> = this.refreshReportingPeriodsTrigger.asObservable();

    private geographyID: number;

    public gridApi: GridApi;
    public columnDefs: ColDef[];

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private utilityFunctionsService: UtilityFunctionsService,
        private modalService: ModalService
    ) {}

    ngOnInit(): void {
        //MK 2/3/2025 -- Configure layout menu is watching the route and setting the current geography.
        this.reportingPeriods$ = combineLatest({ geography: this.currentGeographyService.getCurrentGeography(), _: this.refreshReportingPeriodsTrigger$ }).pipe(
            tap(({ geography }) => {
                this.isLoading = true;
                this.geographyID = geography.GeographyID;
            }),
            switchMap(({ geography }) => {
                return this.reportingPeriodService.geographiesGeographyIDReportingPeriodsGet(geography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
                if (this.gridApi) {
                    setTimeout(() => {
                        this.gridApi.sizeColumnsToFit();
                    }, 1); //MK 2/3/2025 -- This is a hack to get the grid to resize properly. Really don't like it, but need to move on.
                }
            })
        );

        this.initializeGrid();
    }

    initializeGrid(): void {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.updateReportingPeriod(params.data as ReportingPeriodDto),
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("Name", "Name"),
            this.utilityFunctionsService.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDateColumnDef("End Date", "EndDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createBasicColumnDef("Ready for Account Holders", "ReadyForAccountHolders", {
                ValueFormatter: (params) => (params.value ? "Yes" : "No"),
            }),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridApi.sizeColumnsToFit();
    }

    addReportingPeriod() {
        this.modalService
            .open(UpsertReportingPeriodModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: this.geographyID,
                ReportingPeriod: null,
            } as UpsertReportingPeriodModalContext)
            .instance.result.then((result) => {
                if (result) {
                    this.refreshReportingPeriodsTrigger.next();
                }
            });
    }

    updateReportingPeriod(reportingPeriod: ReportingPeriodDto) {
        this.modalService
            .open(UpsertReportingPeriodModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: this.geographyID,
                ReportingPeriod: reportingPeriod,
            } as UpsertReportingPeriodModalContext)
            .instance.result.then((result) => {
                if (result) {
                    this.refreshReportingPeriodsTrigger.next();
                }
            });
    }
}
