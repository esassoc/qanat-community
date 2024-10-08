import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { routeParams } from 'src/app/app.routes';
import { Observable, forkJoin } from 'rxjs';
import { WaterAccountService } from 'src/app/shared/generated/api/water-account.service';
import { WaterAccountDto } from 'src/app/shared/generated/model/water-account-dto';
import { AllocationPlanMinimalDto, ParcelDetailDto, ZoneGroupMinimalDto } from 'src/app/shared/generated/model/models';
import { map, switchMap, tap } from 'rxjs/operators';
import { ColDef, GridApi, GridReadyEvent, SelectionChangedEvent } from 'ag-grid-community';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { ZoneGroupService } from 'src/app/shared/generated/api/zone-group.service';
import { NgIf, AsyncPipe } from '@angular/common';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { WaterAccountParcelService } from 'src/app/shared/generated/api/water-account-parcel.service';
import { ModelNameTagComponent } from 'src/app/shared/components/name-tag/name-tag.component';
import { QanatMapComponent } from 'src/app/shared/components/leaflet/qanat-map/qanat-map.component';
import { ParcelLayerComponent } from 'src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component';

@Component({
  selector: 'water-dashboard-water-account-parcels',
  templateUrl: './water-dashboard-water-account-parcels.component.html',
  styleUrls: ['./water-dashboard-water-account-parcels.component.scss'],
  standalone: true,
  imports: [NgIf, PageHeaderComponent, ModelNameTagComponent, RouterLink, QanatGridComponent, AsyncPipe, QanatMapComponent, ParcelLayerComponent],
})
export class WaterDashboardWaterAccountParcelsComponent implements OnInit {
  public geographyID: number;

  public parcels: ParcelDetailDto[];
  public parcels$: Observable<ParcelDetailDto[]>;

  public selectedParcelIDs: number[];
  public highlightedParcelDto: ParcelDetailDto;
  public waterAccount$: Observable<WaterAccountDto>;
  public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;

  public columnDefs: ColDef<any>[];

  private _highlightedParcelID: number;
  set highlightedParcelID(value: number) {
    this._highlightedParcelID = value;
    this.highlightedParcelDto = this.parcels.filter((x) => x.ParcelID == value)[0];
    this.selectHighlightedParcelIDRowNode();
  }

  get highlightedParcelID(): number {
    return this._highlightedParcelID;
  }

  public gridApi: GridApi;

  constructor(
    private waterAccountService: WaterAccountService,
    private waterAccountParcelService: WaterAccountParcelService,
    private route: ActivatedRoute,
    private utilityFunctionsService: UtilityFunctionsService,
    private zoneGroupService: ZoneGroupService
  ) {}

  ngOnInit(): void {
    this.waterAccount$ = this.route.paramMap.pipe(
      map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
      switchMap((waterAccountID) => this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID)),
      tap((waterAccount) => {
        this.geographyID = waterAccount.Geography.GeographyID;

        this.parcels$ = this.route.paramMap.pipe(
          map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
          switchMap((waterAccountID) =>
            forkJoin({
              parcels: this.waterAccountParcelService.waterAccountsWaterAccountIDParcelsGet(waterAccountID),
              zoneGroups: this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID),
            })
          ),
          tap(({ parcels, zoneGroups }) => {
            this.selectedParcelIDs = parcels.map((x) => x.ParcelID);
            this.parcels = parcels;
            this.buildColumnDefs(zoneGroups);
          }),
          map((x) => x.parcels)
        );

        this.allocationPlans$ = this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(waterAccount.WaterAccountID);
      })
    );
  }

  public buildColumnDefs(zoneGroups: ZoneGroupMinimalDto[]) {
    this.columnDefs = [
      this.utilityFunctionsService.createLinkColumnDef('APN', 'ParcelNumber', 'ParcelID', { InRouterLink: '../../../parcels/' }),
      this.utilityFunctionsService.createDecimalColumnDef('Area (Acres)', 'ParcelArea'),
      this.utilityFunctionsService.createLinkColumnDef('Account #', 'WaterAccount.WaterAccountNumber', 'WaterAccount.WaterAccountID', {
        InRouterLink: '../../',
        FieldDefinitionType: 'WaterAccount',
        FieldDefinitionLabelOverride: 'Water Account',
      }),
      this.utilityFunctionsService.createBasicColumnDef('Account Name', 'WaterAccount.WaterAccountName'),
      this.utilityFunctionsService.createBasicColumnDef('Parcel Status', 'ParcelStatus.ParcelStatusDisplayName', {
        FieldDefinitionType: 'ParcelStatus',
        CustomDropdownFilterField: 'ParcelStatus.ParcelStatusDisplayName',
      }),
      { headerName: 'Owner Name', field: 'OwnerName' },
      { headerName: 'Owner Address', field: 'OwnerAddress' },
    ];
    zoneGroups.forEach((zoneGroup) => {
      this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, 'Zones'));
    });
  }

  public onGridReady(event: GridReadyEvent) {
    this.gridApi = event.api;
  }

  public onSelectionChanged(event: SelectionChangedEvent) {
    const selection = event.api.getSelectedRows()[0];
    if (selection && selection.ParcelID) {
      this.highlightedParcelID = selection.ParcelID;
    }
  }

  public selectHighlightedParcelIDRowNode() {
    this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
      if (rowNode.data.ParcelID == this.highlightedParcelID) {
        rowNode.setSelected(true);
        this.gridApi.ensureIndexVisible(index, 'top');
      }
    });
  }
}
