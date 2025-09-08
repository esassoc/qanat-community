import { Component, OnInit } from "@angular/core";
import { ParcelEditZoneAssignmentsModalComponent } from "src/app/shared/components/parcel/modals/parcel-edit-zone-assignments-modal/parcel-edit-zone-assignments-modal.component";
import { ParcelModifyParcelStatusModalComponent } from "src/app/shared/components/parcel/modals/parcel-modify-parcel-status-modal/parcel-modify-parcel-status-modal.component";
import { ParcelUpdateOwnershipInfoModalComponent } from "src/app/shared/components/parcel/modals/parcel-update-ownership-info-modal/parcel-update-ownership-info-modal.component";
import { ParcelUpdateWaterAccountModalComponent } from "src/app/shared/components/parcel/modals/parcel-update-water-account-modal/parcel-update-water-account-modal.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { Observable, switchMap, tap } from "rxjs";
import { AsyncPipe, DatePipe, KeyValuePipe } from "@angular/common";
import { ParcelDetailDto } from "src/app/shared/generated/model/parcel-detail-dto";
import { ParcelHistoryDto } from "src/app/shared/generated/model/parcel-history-dto";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterAccountTitleComponent } from "src/app/shared/components/water-account/water-account-title/water-account-title.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { KeyValuePairListComponent } from "../../../shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "../../../shared/components/key-value-pair/key-value-pair.component";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { EntityCustomAttributesDto } from "src/app/shared/generated/model/entity-custom-attributes-dto";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { QanatMapComponent, QanatMapInitEvent } from "../../../shared/components/leaflet/qanat-map/qanat-map.component";
import { HighlightedParcelsLayerComponent } from "../../../shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import * as L from "leaflet";
import { WellsLayerComponent } from "../../../shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { WellLocationDto } from "src/app/shared/generated/model/well-location-dto";
import { ParcelAcreOverrideModalComponent } from "src/app/shared/components/parcel/modals/parcel-acre-override-modal/parcel-acre-override-modal.component";
import { WaterAccountParcelByParcelService } from "src/app/shared/generated/api/water-account-parcel-by-parcel.service";
import { ParcelWaterAccountHistorySimpleDto } from "src/app/shared/generated/model/parcel-water-account-history-simple-dto";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { WaterAccountMinimalAndReportingPeriodSimpleDto } from "src/app/shared/generated/model/water-account-minimal-and-reporting-period-simple-dto";
import { DialogService } from "@ngneat/dialog";
import { RecalculateOpenETSyncRasterDataModalComponent } from "src/app/shared/components/recalculate-open-et-sync-raster-data-modal/recalculate-open-et-sync-raster-data-modal.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { UsageLocationParcelHistoryDto } from "src/app/shared/generated/model/usage-location-parcel-history-dto";
import { UsageLocationParcelHistoryService } from "src/app/shared/generated/api/usage-location-parcel-history.service";
import { UsageLocationHistoryDto } from "src/app/shared/generated/model/models";
import { UsageLocationHistoryService } from "src/app/shared/generated/api/usage-location-history.service";

@Component({
    selector: "parcel-admin-panel",
    imports: [
        IconComponent,
        PageHeaderComponent,
        WaterAccountTitleComponent,
        AsyncPipe,
        RouterLink,
        DatePipe,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        KeyValuePipe,
        LoadingDirective,
        ModelNameTagComponent,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellsLayerComponent,
        QanatGridComponent,
        AlertDisplayComponent,
    ],
    templateUrl: "./parcel-admin-panel.component.html",
    styleUrl: "./parcel-admin-panel.component.scss",
})
export class ParcelAdminPanelComponent implements OnInit {
    public parcel$: Observable<ParcelDetailDto>;
    public geography$: Observable<GeographyMinimalDto>;
    public parcelHistories$: Observable<ParcelHistoryDto[]>;
    public waterAccountParcels$: Observable<WaterAccountMinimalAndReportingPeriodSimpleDto[]>;
    public parcelWaterAccountHistories$: Observable<ParcelWaterAccountHistorySimpleDto[]>;
    public parcelCustomAttributes$: Observable<EntityCustomAttributesDto>;
    public wellLocations$: Observable<WellLocationDto[]>;
    public usageLocationParcelHistories$: Observable<UsageLocationParcelHistoryDto[]>;
    public usageLocationHistories$: Observable<UsageLocationHistoryDto[]>;

    public isLoading: boolean = false;
    public parcelWaterAccountHistoriesColumnDefs: ColDef[];
    public parcelWaterAccountHistoriesGridApi: GridApi;

    public usageLocationParcelHistoriesColumnDefs: ColDef[];
    public usageLocationParcelHistoriesGridApi: GridApi;

    public usageLocationHistoriesColumnDefs: ColDef[];
    public usageLocationHistoriesGridApi: GridApi;

    public parcel: ParcelDetailDto;

    constructor(
        private parcelService: ParcelService,
        private geographyService: GeographyService,
        private waterAccountParcelsByParcelService: WaterAccountParcelByParcelService,
        private usageLocationParcelHistoryService: UsageLocationParcelHistoryService,
        private usageLocationHistoryService: UsageLocationHistoryService,
        private route: ActivatedRoute,
        private customAttributeService: CustomAttributeService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                this.isLoading = true;
                let parcelID = parseInt(paramMap.get(routeParams.parcelID));
                return this.parcelService.getParcelWithZonesDtoByIDParcel(parcelID).pipe(
                    tap((parcel) => {
                        this.parcel = parcel;
                        this.isLoading = false;
                    })
                );
            })
        );

        this.geography$ = this.parcel$.pipe(switchMap((parcel) => this.geographyService.getByNameAsMinimalDtoGeography(parcel.GeographyName)));

        this.wellLocations$ = this.parcel$.pipe(switchMap((parcel) => this.parcelService.getWellLocationsForParcelParcel(parcel.ParcelID)));

        this.parcelHistories$ = this.parcel$.pipe(switchMap((parcel) => this.parcelService.getHistoryParcel(parcel.ParcelID)));

        this.waterAccountParcels$ = this.parcel$.pipe(
            switchMap((parcel) => this.waterAccountParcelsByParcelService.getWaterAccountParcelsForParcelWaterAccountParcelByParcel(parcel.ParcelID))
        );

        this.parcelWaterAccountHistories$ = this.parcel$.pipe(
            switchMap((parcel) => this.waterAccountParcelsByParcelService.getWaterAccountParcelHistoryWaterAccountParcelByParcel(parcel.ParcelID))
        );

        this.usageLocationParcelHistories$ = this.parcel$.pipe(
            switchMap((parcel) => this.usageLocationParcelHistoryService.listUsageLocationParcelHistory(parcel.GeographyID, parcel.ParcelID))
        );

        this.usageLocationHistories$ = this.parcel$.pipe(
            switchMap((parcel) => this.usageLocationHistoryService.listByParcelUsageLocationHistory(parcel.GeographyID, parcel.ParcelID))
        );

        this.parcelCustomAttributes$ = this.parcel$.pipe(switchMap((parcel) => this.customAttributeService.listAllParcelCustomAttributesCustomAttribute(parcel.ParcelID)));

        this.parcelWaterAccountHistoriesColumnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriodName"),
            this.utilityFunctionsService.createLinkColumnDef("From Water Account", "FromWaterAccountNumberAndName", "FromWaterAccountID", {
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createLinkColumnDef("To Water Account", "ToWaterAccountNumberAndName", "ToWaterAccountID", {
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Reason", "Reason"),
            this.utilityFunctionsService.createDateColumnDef("Date", "CreateDate", "short"),
            this.utilityFunctionsService.createBasicColumnDef("User", "CreateUserFullName"),
        ];

        this.usageLocationParcelHistoriesColumnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Usage Location", "UsageLocationName"),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriodName"),
            this.utilityFunctionsService.createLinkColumnDef("From Parcel", "FromParcelNumber", "FromParcelID", {
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createLinkColumnDef("To Parcel", "ToParcelNumber", "ToParcelID", {
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createDateColumnDef("Date", "CreateDate", "short"),
            this.utilityFunctionsService.createBasicColumnDef("User", "CreateUserFullName"),
        ];

        this.usageLocationHistoriesColumnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Usage Location", "UsageLocationName"),
            this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationTypeName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriodName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createDateColumnDef("Date", "CreateDate", "short", { Sort: "desc" }),
            this.utilityFunctionsService.createBasicColumnDef("User", "CreateUserFullName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createBasicColumnDef("Note", "Note"),
        ];
    }

    updateOwnershipInfo(): void {
        const dialogRef = this.dialogService.open(ParcelUpdateOwnershipInfoModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcel.ParcelID);
            }
        });
    }

    updateWaterAccounts(): void {
        const dialogRef = this.dialogService.open(ParcelUpdateWaterAccountModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.waterAccountParcels$ = this.waterAccountParcelsByParcelService.getWaterAccountParcelsForParcelWaterAccountParcelByParcel(this.parcel.ParcelID);
                this.parcelWaterAccountHistories$ = this.waterAccountParcelsByParcelService.getWaterAccountParcelHistoryWaterAccountParcelByParcel(this.parcel.ParcelID);
            }
        });
    }

    editZoneAssignments(): void {
        const dialogRef = this.dialogService.open(ParcelEditZoneAssignmentsModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }

    modifyParcelStatus(): void {
        const dialogRef = this.dialogService.open(ParcelModifyParcelStatusModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcel.ParcelID);
            }
        });
    }

    editAcres(): void {
        const dialogRef = this.dialogService.open(ParcelAcreOverrideModalComponent, {
            data: { ParcelID: this.parcel.ParcelID, GeographyID: this.parcel.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.parcel$ = this.parcelService.getParcelWithZonesDtoByIDParcel(this.parcel.ParcelID);
            }
        });
    }

    recalculateOpenETRasterData(parcel: ParcelDetailDto): void {
        const dialogRef = this.dialogService.open(RecalculateOpenETSyncRasterDataModalComponent, {
            data: { GeographyID: parcel.GeographyID, ParcelID: parcel.ParcelID, UsageLocationIDs: null, ReportingPeriodID: null },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }

    onParcelWaterAccountHistoriesGridReady(event: GridReadyEvent): void {
        this.parcelWaterAccountHistoriesGridApi = event.api;
    }

    onUsageLocationParcelHistoriesGridReady(event: GridReadyEvent): void {
        this.usageLocationParcelHistoriesGridApi = event.api;
    }

    onUsageLocationHistoriesGridReady(event: GridReadyEvent): void {
        this.usageLocationHistoriesGridApi = event.api;
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
