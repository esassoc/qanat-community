import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewContainerRef } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { WaterAccountService } from 'src/app/shared/generated/api/water-account.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { GeographySimpleDto, UserDto, WaterAccountIndexGridDto } from 'src/app/shared/generated/model/models';
import { NgIf, AsyncPipe, CommonModule } from '@angular/common';
import { LoadingDirective } from '../../../shared/directives/loading.directive';
import { RouterLink } from '@angular/router';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { FormsModule } from '@angular/forms';
import { WaterAccountByGeographyService } from 'src/app/shared/generated/api/water-account-by-geography.service';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { PermissionEnum } from 'src/app/shared/generated/enum/permission-enum';
import { RightsEnum } from 'src/app/shared/models/enums/rights.enum';
import { ColDef, GridApi, GridReadyEvent, RowNode } from 'ag-grid-community';
import { ModalService, ModalSizeEnum, ModalThemeEnum } from 'src/app/shared/services/modal/modal.service';
import { DeleteWaterAccountComponent } from 'src/app/shared/components/water-account/modals/delete-water-account/delete-water-account.component';
import { MergeWaterAccountsComponent } from 'src/app/shared/components/water-account/modals/merge-water-accounts/merge-water-accounts.component';
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from 'src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component';
import { UpdateParcelsComponent } from 'src/app/shared/components/water-account/modals/update-parcels/update-parcels.component';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { WaterDashboardNavComponent } from '../../../shared/components/water-dashboard-nav/water-dashboard-nav.component';
import { AgGridAngular } from 'ag-grid-angular';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { WaterDashboardManagementActionsPanelComponent } from 'src/app/shared/components/water-dashboard-management-actions-panel/water-dashboard-management-actions-panel.component';
import { QanatMapComponent, QanatMapInitEvent } from 'src/app/shared/components/leaflet/qanat-map/qanat-map.component';
import { GsaBoundariesComponent } from 'src/app/shared/components/leaflet/layers/gsa-boundaries/gsa-boundaries.component';
import { Map, layerControl } from 'leaflet';
import { WaterAccountsLayerComponent } from 'src/app/shared/components/leaflet/layers/water-accounts-layer/water-accounts-layer.component';
import { QanatGridHeaderComponent } from 'src/app/shared/components/qanat-grid-header/qanat-grid-header.component';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';

@Component({
  selector: 'water-account-list',
  templateUrl: './water-account-list.component.html',
  styleUrls: ['./water-account-list.component.scss'],
  standalone: true,
  imports: [PageHeaderComponent, RouterLink, LoadingDirective, NgIf, AsyncPipe, FormsModule, CommonModule, QanatGridComponent, QanatGridHeaderComponent, LoadingDirective, WaterDashboardNavComponent, IconComponent, WaterDashboardManagementActionsPanelComponent, QanatMapComponent, GsaBoundariesComponent, WaterAccountsLayerComponent]
})
export class WaterAccountListComponent implements OnInit, OnDestroy {
  public currentUser$: Observable<UserDto>;
  private currentUser: UserDto;

  public waterAccountGeographies$: Observable<GeographySimpleDto[]>;
  public refreshWaterAccounts$ = new BehaviorSubject(null);
  public selectedGeography: GeographySimpleDto;
  public waterAccounts$: Observable<WaterAccountIndexGridDto[]>;

  public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
  public currentUserHasOverallPermission: boolean = false;
  public currentUserHasNoGeographies: boolean = false;

  public columnDefs: ColDef<WaterAccountIndexGridDto>[];
  public gridApi: GridApi;
  private managerOnlyColIDs = ['0', '10', '11', '12', '13'];
  public gridRef: AgGridAngular;

  public selectedPanel: 'Grid' | 'Hybrid' | 'Map' = 'Hybrid';
  public selectedWaterAccountID: number;

  public map: Map;
  public layerControl: layerControl;
  public bounds: any;
  public mapIsReady: boolean = false;
  public waterAccountIDs: number[];

  public richTextID: number = CustomRichTextTypeEnum.WaterAccountDashboard;
  public isLoading: boolean = true;
  public firstLoad: boolean = true;

  constructor(
    private authenticationService: AuthenticationService,
    private geographyService: GeographyService,
    private waterAccountService: WaterAccountService,
    private waterAccountByGeographyService: WaterAccountByGeographyService,
    private utilityFunctionsService: UtilityFunctionsService,
    private modalService: ModalService,
    private viewContainerRef: ViewContainerRef,
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

        this.waterAccounts$ = this.refreshWaterAccounts$.pipe(
          switchMap(geographyID => {
            if (!geographyID) return [];
            this.isLoading = true;
            return this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsCurrentUserGet(geographyID).pipe(
              tap(waterAccounts => {
                this.waterAccountIDs = waterAccounts.map(x => x.WaterAccountID);
                this.setManagerOnlyColumnVisibility();
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
      this.selectedWaterAccountID = undefined;
    } else {
      this.gridApi.setGridOption('rowSelection', 'single');
    }
  }

  public onGeographySelected() {
    this.currentUserHasManagerPermissionsForSelectedGeography = this.currentUserHasOverallPermission
      || this.authenticationService.hasGeographyPermission(this.currentUser, PermissionEnum.WaterAccountRights, RightsEnum.Read, this.selectedGeography.GeographyID);

    this.refreshWaterAccounts$.next(this.selectedGeography.GeographyID);
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
    this.setManagerOnlyColumnVisibility();
  }

  public onGridRefReady(gridRef: AgGridAngular) {
    this.gridRef = gridRef;
  }

  public onGridSelectionChanged() {
    const selectedNodes = this.gridApi.getSelectedNodes();
    this.selectedWaterAccountID = selectedNodes.length > 0 ? selectedNodes[0].data.WaterAccountID : null;
  }

  public onMapSelectionChanged(selectedWaterAccountID: number) {
    if (this.selectedWaterAccountID == selectedWaterAccountID) return;

    this.selectedWaterAccountID = selectedWaterAccountID;
    this.gridApi.forEachNode((node, index) => {
      if (node.data.WaterAccountID == selectedWaterAccountID) {
        node.setSelected(true, true);
        this.gridApi.ensureIndexVisible(index, 'top');
      }
    });
  }

  private setManagerOnlyColumnVisibility() {
    this.gridApi?.setColumnsVisible(this.managerOnlyColIDs, this.currentUserHasManagerPermissionsForSelectedGeography);
  }

  private createColumnDefs() {
    this.columnDefs = [
      this.utilityFunctionsService.createActionsColumnDef((params: any) => {
        return [
          { ActionName: 'Update Parcels', ActionIcon: 'fas fa-map', ActionHandler: () => this.updateParcelsModal(params.data.WaterAccountID, params.node) },
          { ActionName: 'Update Info', ActionIcon: 'fas fa-info-circle', ActionHandler: () => this.updateInfoModal(params.data.WaterAccountID, params.node) },
          { ActionName: 'Merge', ActionIcon: 'fas fa-long-arrow-alt-right', ActionHandler: () => this.mergeModal(params.data.WaterAccountID) },
          { ActionName: 'Delete', ActionIcon: 'fa fa-times-circle text-danger', ActionHandler: () => this.deleteModal(params.data.WaterAccountID) },
        ];
      }, true),
      this.utilityFunctionsService.createLinkColumnDef('Account Number', 'WaterAccountNumber', 'WaterAccountID', {
        FieldDefinitionType: 'WaterAccount',
        FieldDefinitionLabelOverride: 'Water Account #',
        ValueGetter: params => {
          return { LinkValue: `${params.data.WaterAccountID}`, LinkDisplay: params.data.WaterAccountNumber };
        }
      }),
      { headerName: 'Account Name', field: 'WaterAccountName' },
      this.utilityFunctionsService.createMultiLinkColumnDef('APN List', 'Parcels', 'ParcelID', 'ParcelNumber', {
        InRouterLink: '../parcels', MaxWidth: 300
      }),
      { headerName: 'Contact Name', field: 'ContactName' },
      { headerName: 'Contact Address', field: 'ContactAddress' },
      { headerName: 'Users', valueGetter: params => params.data.Users?.map(x => x.UserFullName) },
      this.utilityFunctionsService.createDecimalColumnDef('# of Users', 'Users.length', { DecimalPlacesToDisplay: 0 }),
      this.utilityFunctionsService.createDecimalColumnDef('# of Parcels', 'Parcels.length', { DecimalPlacesToDisplay: 0 }),
      { headerName: 'Notes', field: 'Notes' },
      this.utilityFunctionsService.createDateColumnDef('Create Date', 'CreateDate', 'short', { Hide: true }),
      this.utilityFunctionsService.createDateColumnDef('Last Update Date', 'UpdateDate', 'short', { Hide: true }),
      { headerName: 'Water Account PIN', field: 'WaterAccountPIN', hide: true },
      this.utilityFunctionsService.createDateColumnDef('Water Account PIN Last Used', 'WaterAccountPINLastUsed', 'short', { Hide: true })
    ];
  }

  public deleteModal(waterAccountID: number) {
    this.modalService.open(DeleteWaterAccountComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, { WaterAccountID: waterAccountID, GeographyID: this.selectedGeography.GeographyID } as WaterAccountContext)
      .instance.result.then(result => {
        if (result) {
          this.refreshWaterAccounts$.next(this.selectedGeography.GeographyID);
        }
      });
  }

  public mergeModal(waterAccountID: number) {
    this.modalService.open(MergeWaterAccountsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, { WaterAccountID: waterAccountID, GeographyID: this.selectedGeography.GeographyID } as WaterAccountContext)
      .instance.result.then(result => {
        if (result) {
          this.refreshWaterAccounts$.next(this.selectedGeography.GeographyID);
        }
      });
  }

  public updateInfoModal(waterAccountID: number, rowNode: RowNode) {
    this.modalService.open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, { WaterAccountID: waterAccountID, GeographyID: this.selectedGeography.GeographyID } as WaterAccountContext)
      .instance.result.then(result => {
        if (result) {
          rowNode.setData(result);
        }
      });
  }

  public updateParcelsModal(waterAccountID: number, rowNode: RowNode) {
    this.modalService.open(UpdateParcelsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light }, { WaterAccountID: waterAccountID, GeographyID: this.selectedGeography.GeographyID } as WaterAccountContext)
      .instance.result.then(result => {
        if (result) {
          rowNode.setData(result);
        }
      });
  }
}
