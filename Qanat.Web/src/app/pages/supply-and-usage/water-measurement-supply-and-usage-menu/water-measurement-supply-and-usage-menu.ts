import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { combineLatest, Observable, of, shareReplay, switchMap } from "rxjs";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { RouterLink } from "@angular/router";
import { AsyncPipe, NgClass } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { RefreshWaterMeasurementCalculationsModalComponent } from "src/app/pages/supply-and-usage/water-measurement-supply-and-usage-menu/refresh-water-measurement-calculations-modal/refresh-water-measurement-calculations-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { DownloadWaterMeasurementsModalComponent } from "./download-water-measurements-modal/download-water-measurements-modal.component";
import { WaterMeasurementTypeSimpleDto } from "src/app/shared/generated/model/models";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ColDef } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "water-measurement-supply-and-usage-menu",
    templateUrl: "./water-measurement-supply-and-usage-menu.html",
    styleUrls: ["./water-measurement-supply-and-usage-menu.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, RouterLink, AsyncPipe, NgClass, QanatGridComponent],
})
export class WaterMeasurementSupplyAndUsageMenu implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public canCreateTransactions$: Observable<boolean>;
    public waterMeasurementTypes$: Observable<WaterMeasurementTypeSimpleDto[]>;

    public currentUser$: Observable<UserDto>;

    public richTextTypeID = CustomRichTextTypeEnum.UsageEstimates;
    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;
    public waterMeasurementTypeGridCols: ColDef[];

    constructor(
        private authenticationService: AuthenticationService,
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementService: WaterMeasurementService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {
        this.waterMeasurementTypeGridCols = [
            this.utilityFunctionsService.createBasicColumnDef("Water Measurement Type", "WaterMeasurementTypeName", {}),
            this.utilityFunctionsService.createBasicColumnDef("Short Name", "ShortName", {}),
            this.utilityFunctionsService.createDecimalColumnDef("Sort Order", "SortOrder", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createBasicColumnDef("Water Measurement Category", "WaterMeasurementCategoryName", {
                CustomDropdownFilterField: "WaterMeasurementCategoryName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Is Active", "IsActive", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.IsActive),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Can Be Uploaded", "IsUserEditable", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.IsUserEditable),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Is Self Reportable", "IsSelfReportable", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.IsSelfReportable),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Show to Landowner", "ShowToLandowner", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.ShowToLandowner),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Is Source of Record", "IsSourceOfRecord", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.IsSourceOfRecord),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Water Measurement Calculation", "WaterMeasurementCalculationName", {}),
            this.utilityFunctionsService.createBasicColumnDef("Calculation JSON", "CalculationJSON", {}),
        ];
    }

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.currentUser$ = this.authenticationService.getCurrentUser();

        this.canCreateTransactions$ = combineLatest([this.geography$, this.currentUser$]).pipe(
            switchMap(([geography, user]) => {
                const hasPermission = this.authenticationService.hasOverallPermission(user, PermissionEnum.WaterTransactionRights, RightsEnum.Create, geography.GeographyID);
                return of(hasPermission);
            }),
            shareReplay(1)
        );

        this.waterMeasurementTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterMeasurementTypeService.getWaterMeasurementTypesWaterMeasurementType(geography.GeographyID);
            })
        );
    }

    public openDownloadWaterMeasurementsModal(geography: GeographyMinimalDto) {
        if (!this.isDownloading) {
            const dialogRef = this.dialogService.open(DownloadWaterMeasurementsModalComponent, {
                data: {
                    GeographyID: geography.GeographyID,
                    GeographyName: geography.GeographyName,
                    GeographyStartYear: null,
                },
                size: "lg",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.downloadWaterMeasurements(geography, result);
                }
            });
        }
    }

    private downloadWaterMeasurements(geography: GeographyMinimalDto, year: number) {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;

        this.waterMeasurementService.downloadExcelWorkbookForGeographyAndYearWaterMeasurement(geography.GeographyID, year).subscribe(
            (result) =>
                this.handleDownloadSuccess(result, `${geography.GeographyName}_${year}_waterMeasurements`, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            (error) => this.handleDownloadError(error)
        );
    }

    private handleDownloadSuccess(result, fileName, contentType) {
        const blob = new Blob([result], {
            type: contentType,
        });

        //Create a fake object to trigger downloading the zip file that was returned
        const a: any = document.createElement("a");
        document.body.appendChild(a);

        a.style = "display: none";
        const url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isDownloading = false;
    }

    private handleDownloadError(error) {
        this.downloadError = true;
        //Because our return type is ArrayBuffer, the message will be ugly. Convert it and display
        const decodedString = String.fromCharCode.apply(null, new Uint8Array(error.error) as any);
        this.downloadErrorMessage = decodedString;
        this.isDownloading = false;
    }

    public refreshWaterMeasurementCalculations(geography: GeographyMinimalDto) {
        if (!this.isDownloading) {
            const dialogRef = this.dialogService.open(RefreshWaterMeasurementCalculationsModalComponent, {
                data: {
                    GeographyID: geography.GeographyID,
                    GeographyName: geography.GeographyName,
                    GeographyStartYear: null,
                    UsageLocationIDs: null,
                    ReportingPeriodID: null,
                },
                size: "sm",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                }
            });
        }
    }
}
