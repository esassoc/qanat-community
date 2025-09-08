import { Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable, map, tap } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { MonitoringWellService } from "src/app/shared/generated/api/monitoring-well.service";
import { GeographyBoundarySimpleDto, GeographyDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "admin-geographies",
    templateUrl: "./admin-geographies.component.html",
    styleUrls: ["./admin-geographies.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, AsyncPipe, DatePipe]
})
export class AdminGeographiesComponent implements OnInit {
    public geographies$: Observable<GeographyDto[]>;
    public geographyBoundaries$: Observable<GeographyBoundarySimpleDto[]>;
    public lastUpdatedGSABoundariesDate: Date;
    public isLoadingSubmit = false;

    public columnDefs: ColDef[];
    public colIDsToExclude = ["0"];

    constructor(
        private geographyService: GeographyService,
        private publicService: PublicService,
        private alertService: AlertService,
        private monitoringWellService: MonitoringWellService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.geographies$ = this.geographyService
            .listGeography()
            .pipe(map((geographies) => geographies.sort((a, b) => (a.GeographyDisplayName > b.GeographyDisplayName ? 1 : -1))));

        this.geographyBoundaries$ = this.publicService
            .listBoundariesPublic()
            .pipe(
                tap(
                    (geographyBoundaries) =>
                        (this.lastUpdatedGSABoundariesDate = geographyBoundaries.map((x) => (x.GSABoundaryLastUpdated ? new Date(x.GSABoundaryLastUpdated) : null)).sort()[0])
                )
            );

        this.createColumnDefs();
    }

    createColumnDefs(): void {
        this.columnDefs = [
            { headerName: "Long Name", field: "GeographyDisplayName" },
            { headerName: "Short Name", field: "GeographyName" },
            this.utilityFunctionsService.createYearColumnDef("Start Year", "StartYear"),
            { headerName: "APN Regex", field: "APNRegexPattern" },
            { headerName: "APN Regex Display", field: "APNRegexPatternDisplay" },
            { headerName: "Landowner Dashboard Supply Label", field: "LandownerDashboardSupplyLabel" },
            { headerName: "Landowner Dashboard Usage Label", field: "LandownerDashboardUsageLabel" },
            { headerName: "Coordinate System", field: "CoordinateSystem" },
            { headerName: "Contact Email", field: "ContactEmail" },
            this.utilityFunctionsService.createPhoneNumberColumnDef("Contact Phone", "ContactPhoneNumber"),
            { headerName: "Contact Address Line 1", field: "ContactAddressLine1" },
            { headerName: "Contact Address Line 2", field: "ContactAddressLine2" },
            { headerName: "Is Demo Geography?", valueGetter: (params) => (params.data.IsDemoGeography ? "Yes" : "No") },
        ];
    }

    refreshGSABoundaries() {
        this.isLoadingSubmit = true;

        this.geographyService.refreshGSABoundariesGeography().subscribe({
            next: (geographyBoundaries) => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Geography GSA boundaries successfully refreshed", AlertContext.Success));
                this.lastUpdatedGSABoundariesDate = geographyBoundaries.map((x) => (x.GSABoundaryLastUpdated ? new Date(x.GSABoundaryLastUpdated) : null)).sort()[0];
            },
            complete: () => {
                this.isLoadingSubmit = false;
            },
        });
    }

    triggerMonitorWellsCNRA() {
        this.monitoringWellService.updateMonitoringWellDataMonitoringWell().subscribe((response) => {
            this.alertService.pushAlert(new Alert("Monitoring Wells Job triggered successfully!", AlertContext.Success, true));
        });
    }
}
