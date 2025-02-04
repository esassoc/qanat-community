import { Component, ChangeDetectorRef, ViewContainerRef, OnInit } from "@angular/core";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { forkJoin, Observable, share, tap } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { ModalOptions, ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { CustomAttributeSimpleDto, ParcelWaterSupplyDto, GeographyMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { CustomAttributeTypeEnum } from "src/app/shared/generated/enum/custom-attribute-type-enum";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { BulkUpdateParcelStatusModalComponent, ParcelUpdateContext } from "src/app/shared/components/bulk-update-parcel-status-modal/bulk-update-parcel-status-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { RouterLink } from "@angular/router";
import { AsyncPipe, NgIf } from "@angular/common";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "parcel-bulk-actions",
    templateUrl: "./parcel-bulk-actions.component.html",
    styleUrl: "./parcel-bulk-actions.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, IconComponent, RouterLink, NgIf, AsyncPipe],
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
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private customAttributeService: CustomAttributeService
    ) {}

    ngOnInit() {
        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.getDataForGeographyID(this.geographyID);
            }),
            share()
        );
    }

    getDataForGeographyID(geographyID: number) {
        forkJoin([this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID)]).subscribe(([zoneGroups]) => {
            this.gridApi?.showLoadingOverlay();
            this.zoneGroups = zoneGroups;

            forkJoin([
                this.parcelByGeographyService.geographiesGeographyIDParcelsGet(geographyID),
                this.geographyService.geographiesGeographyIDGet(geographyID),
                this.customAttributeService.geographiesGeographyIDCustomAttributesCustomAttributeTypeIDGet(geographyID, CustomAttributeTypeEnum.Parcel),
            ]).subscribe(([parcels, geography, customAttributes]) => {
                this.isLoading = false;
                this.parcels = parcels;
                this.geography = geography;
                this.customAttributes = customAttributes;
                this.gridApi.hideOverlay();
                this.cdr.detectChanges();

                this.createColumnDefs();
            });
        });
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createCheckboxSelectionColumnDef(),
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
        this.parcelByGeographyService.geographiesGeographyIDParcelsWaterAccountStartYearPost(this.geographyID, this.selectedParcelIDs).subscribe((years) => {
            this.years = years;
            this.modalService
                .open(
                    BulkUpdateParcelStatusModalComponent,
                    this.viewContainerRef,
                    {
                        ModalSize: ModalSizeEnum.Medium,
                        ModalTheme: ModalThemeEnum.Light,
                    } as ModalOptions,
                    {
                        ParcelIDs: this.selectedParcelIDs,
                        Years: this.years,
                        GeographyID: this.geographyID,
                    } as ParcelUpdateContext
                )
                .instance.result.then((submitted) => {
                    if (submitted === true) {
                        this.getDataForGeographyID(this.geographyID);
                        this.isLoading = false;
                    }
                });
        });
    }
}
