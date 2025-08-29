import { Component, ChangeDetectorRef, ViewContainerRef, OnInit } from "@angular/core";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { forkJoin, Observable, share, switchMap, tap } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { CustomAttributeSimpleDto, ParcelWaterSupplyDto, GeographyMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { CustomAttributeTypeEnum } from "src/app/shared/generated/enum/custom-attribute-type-enum";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { BulkUpdateParcelStatusModalComponent } from "src/app/shared/components/bulk-update-parcel-status-modal/bulk-update-parcel-status-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { routeParams } from "src/app/app.routes";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "parcel-bulk-actions",
    templateUrl: "./parcel-bulk-actions.component.html",
    styleUrl: "./parcel-bulk-actions.component.scss",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, RouterLink, AsyncPipe],
})
export class ParcelBulkActionsComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    private geographyID: number;
    public years: number[];

    public isLoadingSubmit: boolean = false;

    public parcels: ParcelWaterSupplyDto[];
    public zoneGroups: ZoneGroupMinimalDto[];
    public gridApi: GridApi;

    public columnDefs: ColDef<ParcelWaterSupplyDto>[];
    public selectedParcelIDs: number[] = [];
    public richTextTypeID: number = CustomRichTextTypeEnum.ParcelBulkActions;

    public geography: GeographyDto;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;
    public isLoading: boolean = true;
    private customAttributes: CustomAttributeSimpleDto[];

    constructor(
        private cdr: ChangeDetectorRef,
        private utilityFunctionsService: UtilityFunctionsService,
        private parcelByGeographyService: ParcelByGeographyService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private zoneGroupService: ZoneGroupService,
        private customAttributeService: CustomAttributeService,
        private route: ActivatedRoute,
        private dialogService: DialogService
    ) {}

    ngOnInit() {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.currentGeographyService.setCurrentGeography(geography);
                this.getDataForGeographyID(this.geographyID);
            }),
            share()
        );
    }

    getDataForGeographyID(geographyID: number) {
        forkJoin([this.zoneGroupService.listZoneGroup(this.geographyID)]).subscribe(([zoneGroups]) => {
            this.gridApi?.setGridOption("loading", true);

            this.zoneGroups = zoneGroups;

            forkJoin([
                this.parcelByGeographyService.listByGeographyIDParcelByGeography(geographyID),
                this.geographyService.getGeographyByIDGeography(geographyID),
                this.customAttributeService.listCustomAttributesForGeographyCustomAttribute(geographyID, CustomAttributeTypeEnum.Parcel),
            ]).subscribe(([parcels, geography, customAttributes]) => {
                this.isLoading = false;
                this.parcels = parcels;
                this.geography = geography;
                this.customAttributes = customAttributes;
                this.gridApi?.setGridOption("loading", false);
                this.cdr.detectChanges();

                this.createColumnDefs();
            });
        });
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.ParcelID}/detail`, LinkDisplay: params.data.ParcelNumber };
                },
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (Acres)", "ParcelArea"),
            this.utilityFunctionsService.createLinkColumnDef("Account", "WaterAccountNumber", "WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Parcel Status", "ParcelStatusDisplayName", {
                FieldDefinitionType: "ParcelStatus",
                CustomDropdownFilterField: "ParcelStatusDisplayName",
            }),
            { headerName: "Owner Name", field: "OwnerName" },
            { headerName: "Owner Address", field: "OwnerAddress" },
        ];
        this.addZoneColumnsToColDefs();
        this.addCustomAttributeColumnsToColDefs();
    }

    private addZoneColumnsToColDefs() {
        this.zoneGroups.forEach((zoneGroup) => {
            this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "ZoneIDs"));
        });
    }

    private addCustomAttributeColumnsToColDefs() {
        this.columnDefs.push(...this.utilityFunctionsService.createCustomAttributeColumnDefs(this.customAttributes));
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onSelectionChanged() {
        this.selectedParcelIDs = this.gridApi.getSelectedNodes().map((x) => x.data.ParcelID);
    }

    public changeStatus() {
        this.isLoading = true;
        this.parcelByGeographyService.getYearsForParcelsParcelByGeography(this.geographyID, this.selectedParcelIDs).subscribe((years) => {
            this.years = years;
            const dialogRef = this.dialogService.open(BulkUpdateParcelStatusModalComponent, {
                data: {
                    ParcelIDs: this.selectedParcelIDs,
                    Years: this.years,
                    GeographyID: this.geographyID,
                },
                size: "sm",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.getDataForGeographyID(this.geographyID);
                }
                this.isLoading = false;
            });
        });
    }
}
