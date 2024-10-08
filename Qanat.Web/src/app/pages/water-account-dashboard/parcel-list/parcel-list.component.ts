import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, forkJoin } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { AsyncPipe, CommonModule } from '@angular/common';
import { LoadingDirective } from '../../../shared/directives/loading.directive';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { WaterDashboardNavComponent } from '../../../shared/components/water-dashboard-nav/water-dashboard-nav.component';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { GeographySimpleDto } from 'src/app/shared/generated/model/geography-simple-dto';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { ParcelIndexGridDto } from 'src/app/shared/generated/model/parcel-index-grid-dto';
import { WaterAccountIndexGridDto } from 'src/app/shared/generated/model/water-account-index-grid-dto';
import { AgGridAngular } from 'ag-grid-angular';
import { Map, layerControl } from 'leaflet';
import { PermissionEnum } from 'src/app/shared/generated/enum/permission-enum';
import { RightsEnum } from 'src/app/shared/models/enums/rights.enum';
import { ParcelService } from 'src/app/shared/generated/api/parcel.service';
import { WaterAccountService } from 'src/app/shared/generated/api/water-account.service';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { QanatMapComponent, QanatMapInitEvent } from 'src/app/shared/components/leaflet/qanat-map/qanat-map.component';
import { ZoneGroupService } from 'src/app/shared/generated/api/zone-group.service';
import { CustomAttributeService } from 'src/app/shared/generated/api/custom-attribute.service';
import { CustomAttributeTypeEnum } from 'src/app/shared/generated/enum/custom-attribute-type-enum';
import { ZoneGroupMinimalDto } from 'src/app/shared/generated/model/zone-group-minimal-dto';
import { CustomAttributeSimpleDto } from 'src/app/shared/generated/model/custom-attribute-simple-dto';
import { QanatGridHeaderComponent } from 'src/app/shared/components/qanat-grid-header/qanat-grid-header.component';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { FormsModule } from '@angular/forms';
import { WaterDashboardManagementActionsPanelComponent } from 'src/app/shared/components/water-dashboard-management-actions-panel/water-dashboard-management-actions-panel.component';
import { GsaBoundariesComponent } from 'src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component';
import { ParcelLayerComponent } from 'src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component';
import { RouterLink } from '@angular/router';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';

@Component({
  selector: 'parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss'],
  standalone: true,
  imports: [PageHeaderComponent, LoadingDirective, CommonModule, FormsModule, AsyncPipe, WaterDashboardNavComponent, QanatGridHeaderComponent, QanatGridComponent, QanatMapComponent, WaterDashboardManagementActionsPanelComponent, GsaBoundariesComponent, ParcelLayerComponent, RouterLink, IconComponent]
})
export class ParcelListComponent implements OnInit, OnDestroy {
  public currentUser$: Observable<UserDto>;
  private currentUser: UserDto;

  public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
  public currentUserHasOverallPermission: boolean = false;
  public currentUserHasNoGeographies: boolean = false;

  public waterAccountGeographies$: Observable<GeographySimpleDto[]>;
  public selectedGeography: GeographySimpleDto;
  public refreshParcels$ = new BehaviorSubject(null);
  public parcels$: Observable<ParcelIndexGridDto[]>;

  public columnDefs: ColDef<WaterAccountIndexGridDto>[];
  public gridApi: GridApi;
  public gridRef: AgGridAngular;

  public selectedPanel: 'Grid' | 'Hybrid' | 'Map' = 'Hybrid';
  public selectedParcelID: number;

  public map: Map;
  public layerControl: layerControl;
  public bounds: any;
  public mapIsReady: boolean = false;
  public parcelIDs: number[];

  public zoneGroups: ZoneGroupMinimalDto[];
  public customAttributes: CustomAttributeSimpleDto[];

  public richTextID: number = CustomRichTextTypeEnum.WaterAccountDashboard;
  public isLoading: boolean = true;
  public firstLoad: boolean = true;

  constructor(
    private parcelService: ParcelService,
    public waterAccountService: WaterAccountService,
    public zoneGroupService: ZoneGroupService,
    public customAttributeService: CustomAttributeService,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private geographyService: GeographyService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
      tap((currentUser) => {
        this.currentUser = currentUser;
        this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(currentUser, PermissionEnum.ParcelRights, RightsEnum.Read);

        this.waterAccountGeographies$ = this.geographyService.geographiesCurrentUserGet().pipe(
          tap(geographies => {
            if (geographies.length == 0) {
              this.currentUserHasNoGeographies = true;
              return;
            }
            this.selectedGeography = geographies[0];
            this.onGeographySelected();
          })
        );

        this.parcels$ = this.refreshParcels$.pipe(
          switchMap(geographyID => {
            if (!geographyID) return [];
            this.isLoading = true;

            return forkJoin([
              this.parcelService.geographiesGeographyIDParcelsCurrentUserGet(geographyID),
              this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geographyID),
              this.currentUserHasManagerPermissionsForSelectedGeography ? this.customAttributeService.geographiesGeographyIDCustomAttributesCustomAttributeTypeIDGet(geographyID, CustomAttributeTypeEnum.Parcel) : []
            ]).pipe(
              map(([parcels, zoneGroups, customAttributes]) => {
                this.parcelIDs = parcels.map(x => x.ParcelID);

                this.zoneGroups = zoneGroups;
                this.customAttributes = customAttributes;
                this.createColumnDefs();

                this.isLoading = false;
                this.firstLoad = false;

                this.cdr.detectChanges();

                return parcels;
              })
            );
          })
        );

      })
    );
  }

  ngOnDestroy(): void {
    this.cdr.detach();
  }

  public toggleSelectedPanel(selectedPanel: 'Grid' | 'Hybrid' | 'Map') {
    this.selectedPanel = selectedPanel;
    
    setTimeout(() => {
      this.map.invalidateSize(true);
  
      if (this.layerControl && this.bounds) {
        this.map.fitBounds(this.bounds);
      }
    }, 300);

    // if no map is visible, turn of grid selection
    if (selectedPanel == 'Grid') {
      this.gridApi.setGridOption('rowSelection', null);
      this.selectedParcelID = undefined;
    } else {
      this.gridApi.setGridOption('rowSelection', 'single');
    }
  }

  public onGeographySelected() {
    this.currentUserHasManagerPermissionsForSelectedGeography = this.currentUserHasOverallPermission
      || this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read, this.selectedGeography.GeographyID);

    this.refreshParcels$.next(this.selectedGeography.GeographyID);
  }

  public handleMapReady(event: QanatMapInitEvent) {
    this.map = event.map;
    this.layerControl = event.layerControl;
    this.mapIsReady = true;
    this.cdr.detectChanges();
  }

  public handleLayerBoundsCalculated(bounds: any) {
    this.bounds = bounds;
  }

  public onGridReady(event: GridReadyEvent) {
    this.gridApi = event.api;
  }

  public onGridRefReady(gridRef: AgGridAngular) {
    this.gridRef = gridRef;
  }

  public onGridSelectionChanged() {
    const selectedNodes = this.gridApi.getSelectedNodes();
    this.selectedParcelID = selectedNodes.length > 0 ? selectedNodes[0].data.ParcelID : null;
  }

  public onMapSelectionChanged(selectedParcelID: number) {
    if (this.selectedParcelID == selectedParcelID) return;

    this.selectedParcelID = selectedParcelID;
    this.gridApi.forEachNode((node, index) => {
      if (node.data.ParcelID == selectedParcelID) {
        node.setSelected(true, true);
        this.gridApi.ensureIndexVisible(index, 'top');
      }
    });
  }

  private createColumnDefs() {
    if (this.columnDefs) {
      this.columnDefs = null;
    }

    this.columnDefs = [
      this.utilityFunctionsService.createLinkColumnDef('APN', 'ParcelNumber', 'ParcelID', { 
        ValueGetter: params => {
          return { LinkValue: `${params.data.ParcelID}/detail`, LinkDisplay: params.data.ParcelNumber };
        }
      }),
      this.utilityFunctionsService.createDecimalColumnDef('Area (Acres)', 'ParcelArea'),
      this.utilityFunctionsService.createLinkColumnDef('Account #', 'WaterAccountNumber', 'WaterAccountID', {
        InRouterLink: '../water-accounts/',
        FieldDefinitionType: 'WaterAccount',
        FieldDefinitionLabelOverride: 'Water Account #'
      }),
      this.utilityFunctionsService.createBasicColumnDef('Water Account Name', 'WaterAccountName'),
      this.utilityFunctionsService.createMultiLinkColumnDef('Wells on Parcel', 'WellsOnParcel', 'WellID', 'WellID', {
        InRouterLink: '../wells', MaxWidth: 300
      }),
      this.utilityFunctionsService.createMultiLinkColumnDef('Irrigated By', 'IrrigatedByWells', 'WellID', 'WellID', {
        InRouterLink: '../wells', MaxWidth: 300
      }),
      { headerName: 'Owner Name', field: 'OwnerName' },
      { headerName: 'Owner Address', field: 'OwnerAddress' },
      this.utilityFunctionsService.createBasicColumnDef('Parcel Status', 'ParcelStatusDisplayName', {
        FieldDefinitionType: 'ParcelStatus',
        CustomDropdownFilterField: 'ParcelStatusDisplayName',
        Hide: !this.currentUserHasManagerPermissionsForSelectedGeography
      })
    ];

    this.zoneGroups.forEach(zoneGroup => {
      this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, 'Zones', !this.currentUserHasManagerPermissionsForSelectedGeography));
    });

    this.columnDefs.push(...this.utilityFunctionsService.createCustomAttributeColumnDefs(this.customAttributes, !this.currentUserHasManagerPermissionsForSelectedGeography));
  }

}