import { ChangeDetectorRef, Component, OnInit, OnDestroy } from '@angular/core';
import { BehaviorSubject, switchMap, tap } from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { NgIf, AsyncPipe, CommonModule } from '@angular/common';
import { LoadingDirective } from '../../shared/directives/loading.directive';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { WaterDashboardNavComponent } from '../../shared/components/water-dashboard-nav/water-dashboard-nav.component';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { QanatMapComponent, QanatMapInitEvent } from 'src/app/shared/components/leaflet/qanat-map/qanat-map.component';
import { WaterDashboardManagementActionsPanelComponent } from 'src/app/shared/components/water-dashboard-management-actions-panel/water-dashboard-management-actions-panel.component';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { GeographySimpleDto } from 'src/app/shared/generated/model/geography-simple-dto';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AgGridAngular } from 'ag-grid-angular';
import { Map, layerControl } from 'leaflet';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { PermissionEnum } from 'src/app/shared/generated/enum/permission-enum';
import { RightsEnum } from 'src/app/shared/models/enums/rights.enum';
import { WellMinimalDto } from 'src/app/shared/generated/model/well-minimal-dto';
import { WellService } from 'src/app/shared/generated/api/well.service';
import { FormsModule } from '@angular/forms';
import { QanatGridHeaderComponent } from 'src/app/shared/components/qanat-grid-header/qanat-grid-header.component';
import { OpenedWellPopupEvent, WellsLayerComponent } from 'src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component';
import { RouterLink } from '@angular/router';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';

@Component({
  selector: 'well-list',
  templateUrl: './well-list.component.html',
  styleUrls: ['./well-list.component.scss'],
  standalone: true,
  imports: [PageHeaderComponent, LoadingDirective, NgIf, RouterLink, IconComponent, AsyncPipe, WaterDashboardNavComponent, QanatGridComponent, QanatGridHeaderComponent, QanatMapComponent,  WellsLayerComponent, WaterDashboardManagementActionsPanelComponent, CommonModule, FormsModule]
})
export class DashboardWaterAccountWellRegistrationComponent implements OnInit, OnDestroy {
  public currentUser$: Observable<UserDto>;
  private currentUser: UserDto;

  public waterAccountGeographies$: Observable<GeographySimpleDto[]>;
  public refreshWells$ = new BehaviorSubject(null);
  public selectedGeography: GeographySimpleDto;
  public wells$: Observable<WellMinimalDto[]>;

  public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
  public currentUserHasOverallPermission: boolean = false;
  public currentUserHasNoGeographies: boolean = false;

  public columnDefs: ColDef<WellMinimalDto>[];
  public gridApi: GridApi;
  public gridRef: AgGridAngular;

  public selectedPanel: 'Grid' | 'Hybrid' | 'Map' = 'Hybrid';

  public map: Map;
  public layerControl: layerControl;
  public bounds: any;
  public mapIsReady: boolean = false;
  public selectedWellID: number;

  public richTextID: number = CustomRichTextTypeEnum.WaterAccountDashboard;
  public isLoading: boolean = true;
  public firstLoad: boolean = true;

  constructor(
    private authenticationService: AuthenticationService,
    private wellService: WellService,
    private geographyService: GeographyService,
    private utilityFunctionsService: UtilityFunctionsService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
      tap((currentUser) => {
        this.currentUser = currentUser;
        this.currentUserHasOverallPermission = this.authenticationService.hasOverallPermission(currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read);

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

        this.wells$ = this.refreshWells$.pipe(
          switchMap(geographyID => {
            if (!geographyID) return [];
            this.isLoading = true;

            return this.wellService.geographiesGeographyIDWellsCurrentUserGet(geographyID).pipe(
              tap(() => {
                this.isLoading = false;
                this.firstLoad = false;

                this.cdr.detectChanges();
              })
            );
          })
        );

        this.createColumnDefs();
      })
    );
  }

  ngOnDestroy(): void {
    this.cdr.detach();
  }

  public toggleSelectedPanel(selectedPanel: 'Grid' | 'Hybrid' | 'Map') {
    this.selectedPanel = selectedPanel;
    
    // resizing map to fit new container width; timeout needed to ensure new width has registered before running invalidtaeSize()
    setTimeout(() => {
      this.map.invalidateSize(true);
  
      if (this.layerControl && this.bounds) {
        this.map.fitBounds(this.bounds);
      }
    }, 300);

    // if no map is visible, turn of grid selection
    if (selectedPanel == 'Grid') {
      this.gridApi.setGridOption('rowSelection', null);
      this.selectedWellID = undefined;
    } else {
      this.gridApi.setGridOption('rowSelection', 'single');
    }
  }

  public onGeographySelected() {
    this.currentUserHasManagerPermissionsForSelectedGeography = this.currentUserHasOverallPermission
      || this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read, this.selectedGeography.GeographyID);

    this.refreshWells$.next(this.selectedGeography.GeographyID);
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
    this.selectedWellID = selectedNodes.length > 0 ? selectedNodes[0].data.WellID : null;
  }

  public onMapSelectionChanged(event: OpenedWellPopupEvent) {
    if (this.selectedWellID == event.wellID) return;

    this.selectedWellID = event.wellID;
    this.gridApi.forEachNode((node, index) => {
      if (node.data.WellID == this.selectedWellID) {
        node.setSelected(true, true);
        this.gridApi.ensureIndexVisible(index, 'top');
      }
    });
  }

  private createColumnDefs() {
    this.columnDefs = [
      this.utilityFunctionsService.createLinkColumnDef('Well ID', 'WellID', 'WellID'),
      this.utilityFunctionsService.createLinkColumnDef('Well Name', 'WellName', 'WellID'),
      this.utilityFunctionsService.createLinkColumnDef('Default APN', 'ParcelNumber', 'ParcelID'),
      this.utilityFunctionsService.createMultiLinkColumnDef('Irrigates', 'IrrigatesParcels', 'ParcelID', 'ParcelNumber', {
        InRouterLink: '../parcels', MaxWidth: 300
      }),
      this.utilityFunctionsService.createBasicColumnDef('County Well Permit #', 'CountyWellPermitNumber', { FieldDefinitionType: 'CountyWellPermitNo' }),
      this.utilityFunctionsService.createBasicColumnDef('State WCR #', 'StateWCRNumber', { FieldDefinitionType: 'StateWCRNo' }),
      this.utilityFunctionsService.createDateColumnDef('DateDrilled', 'DateDrilled', 'M/d/yyyy', { FieldDefinitionType: 'DateDrilled' }),
      this.utilityFunctionsService.createDecimalColumnDef('Well Depth', 'WellDepth', { FieldDefinitionType: 'WellDepth' }),
      this.utilityFunctionsService.createBasicColumnDef('Well Status', 'WellStatusDisplayName', {
        FieldDefinitionType: 'WellStatus',
        CustomDropdownFilterField: 'WellStatusDisplayName'
      }),
      this.utilityFunctionsService.createLatLonColumnDef('Latitude', 'Latitude'),
      this.utilityFunctionsService.createLatLonColumnDef('Longitude', 'Longitude'),
      this.utilityFunctionsService.createBasicColumnDef('Notes', 'Notes')
    ];
  }
}
