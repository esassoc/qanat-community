import { Component, ChangeDetectorRef, ViewContainerRef, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { forkJoin, Subscription } from 'rxjs';
import { GeographyDto } from 'src/app/shared/generated/model/geography-dto';
import { SelectedGeographyService } from 'src/app/shared/services/selected-geography.service';
import { AgGridHelper } from 'src/app/shared/helpers/ag-grid-helper';
import { ModalOptions, ModalService, ModalSizeEnum, ModalThemeEnum } from 'src/app/shared/services/modal/modal.service';
import { CustomAttributeSimpleDto, ParcelWaterSupplyDto, UserDto, ZoneGroupMinimalDto } from 'src/app/shared/generated/model/models';
import { ParcelService } from 'src/app/shared/generated/api/parcel.service';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';
import { ZoneGroupService } from 'src/app/shared/generated/api/zone-group.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { CustomAttributeService } from 'src/app/shared/generated/api/custom-attribute.service';
import { CustomAttributeTypeEnum } from 'src/app/shared/generated/enum/custom-attribute-type-enum';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { BulkUpdateParcelStatusModalComponent, ParcelUpdateContext } from 'src/app/shared/components/bulk-update-parcel-status-modal/bulk-update-parcel-status-modal.component';
import { AlertDisplayComponent } from 'src/app/shared/components/alert-display/alert-display.component';
import { LoadingDirective } from 'src/app/shared/directives/loading.directive';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'parcel-bulk-actions',
  templateUrl: './parcel-bulk-actions.component.html',
  styleUrl: './parcel-bulk-actions.component.scss',
  standalone: true,
  imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, IconComponent, RouterLink],
})
export class ParcelBulkActionsComponent implements OnInit, OnDestroy {
  private geographyID: number;
  private selectedGeography$: Subscription = Subscription.EMPTY;
  public years: number[];

  private currentUser: UserDto;
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
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private parcelService: ParcelService,
    private geographyService: GeographyService,
    private selectedGeographyService: SelectedGeographyService,
    private zoneGroupService: ZoneGroupService,
    private modalService: ModalService,
    private viewContainerRef: ViewContainerRef,
    private customAttributeService: CustomAttributeService
  ) {}

  ngOnInit() {
    this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
      this.geographyID = geography.GeographyID;
      this.getDataForGeographyID(this.geographyID);
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
    this.selectedGeography$.unsubscribe();
  }

  getDataForGeographyID(geographyID: number) {
    forkJoin([this.authenticationService.getCurrentUser(), this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID)]).subscribe(
      ([currentUser, zoneGroups]) => {
        this.currentUser = currentUser;
        this.gridApi.showLoadingOverlay();
        this.zoneGroups = zoneGroups;

        forkJoin([
          this.parcelService.geographiesGeographyIDParcelsGet(geographyID),
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
      }
    );
  }

  private createColumnDefs() {
    this.columnDefs = [
      this.utilityFunctionsService.createCheckboxSelectionColumnDef(),
      this.utilityFunctionsService.createLinkColumnDef('APN', 'ParcelNumber', 'ParcelID', {
        ValueGetter: (params) => {
          return { LinkValue: `${params.data.ParcelID}/detail`, LinkDisplay: params.data.ParcelNumber };
        },
        InRouterLink: '../../../../water-dashboard/parcels/',
      }),
      this.utilityFunctionsService.createDecimalColumnDef('Area (Acres)', 'ParcelArea'),
      this.utilityFunctionsService.createLinkColumnDef('Account', 'WaterAccountNumber', 'WaterAccountID', {
        InRouterLink: '../../water-accounts/',
        FieldDefinitionType: 'WaterAccount',
        FieldDefinitionLabelOverride: 'Water Account #',
      }),
      this.utilityFunctionsService.createBasicColumnDef('Parcel Status', 'ParcelStatusDisplayName', {
        FieldDefinitionType: 'ParcelStatus',
        CustomDropdownFilterField: 'ParcelStatusDisplayName',
      }),
      { headerName: 'Owner Name', field: 'OwnerName' },
      { headerName: 'Owner Address', field: 'OwnerAddress' },
    ];
    this.addZoneColumnsToColDefs();
    this.addCustomAttributeColumnsToColDefs();
  }

  private addZoneColumnsToColDefs() {
    this.zoneGroups.forEach((zoneGroup) => {
      this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, 'Zones'));
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
    this.parcelService.geographiesGeographyIDParcelsWaterAccountStartYearPost(this.geographyID, this.selectedParcelIDs).subscribe((years) => {
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
