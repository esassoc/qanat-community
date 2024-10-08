import { CdkDragDrop, transferArrayItem, CdkDropListGroup, CdkDropList, CdkDrag, CdkDragHandle } from '@angular/cdk/drag-drop';
import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BehaviorSubject, Observable, switchMap, tap } from 'rxjs';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { ConfirmService } from 'src/app/shared/services/confirm/confirm.service';
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from 'src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component';
import { UserService } from 'src/app/shared/generated/api/user.service';
import { WaterAccountService } from 'src/app/shared/generated/api/water-account.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { GeographySimpleDto } from 'src/app/shared/generated/model/geography-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterAccountParcelsRequestChangesDto } from 'src/app/shared/generated/model/water-account-parcels-request-changes-dto';
import { WaterAccountRequestChangesDto } from 'src/app/shared/generated/model/water-account-request-changes-dto';
import { WaterAccountRequestChangesParcelDto } from 'src/app/shared/generated/model/water-account-request-changes-parcel-dto';
import { ZoneDisplayDto } from 'src/app/shared/generated/model/zone-display-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ModalService, ModalSizeEnum, ModalThemeEnum } from 'src/app/shared/services/modal/modal.service';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { ParcelListItemComponent } from 'src/app/shared/components/parcel-list-item/parcel-list-item.component';
import {
  ModifiedWaterAccountContext,
  WaterAccountModificationsContext,
  WaterAccountsConsolidateModalComponent,
} from 'src/app/shared/components/water-accounts-consolidate-modal/water-accounts-consolidate-modal.component';
import { WaterAccountRequestChangesConfirmModalComponent } from 'src/app/shared/components/water-account-request-changes-confirm-modal/water-account-request-changes-confirm-modal.component';
import { WaterAccountByGeographyService } from 'src/app/shared/generated/api/water-account-by-geography.service';
import { AlertDisplayComponent } from 'src/app/shared/components/alert-display/alert-display.component';
import { NoteComponent } from 'src/app/shared/components/note/note.component';
import { ButtonLoadingDirective } from 'src/app/shared/directives/button-loading.directive';
import { LoadingDirective } from 'src/app/shared/directives/loading.directive';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';

@Component({
  selector: 'water-account-request-changes',
  templateUrl: './water-account-request-changes.component.html',
  styleUrl: './water-account-request-changes.component.scss',
  standalone: true,
  imports: [
    NgIf,
    PageHeaderComponent,
    FormsModule,
    NgFor,
    SelectDropDownModule,
    LoadingDirective,
    AlertDisplayComponent,
    CdkDropListGroup,
    IconComponent,
    NoteComponent,
    CdkDropList,
    ParcelListItemComponent,
    CdkDrag,
    CdkDragHandle,
    ButtonLoadingDirective,
    RouterLink,
    AsyncPipe,
  ],
})
export class WaterAccountRequestChangesComponent implements OnInit {
  public currentUser$: Observable<UserDto>;
  private currentUser: UserDto;

  public geographies$: Observable<GeographySimpleDto[]>;
  public geographiesForTypeScript: GeographySimpleDto[];
  public geographyID: number;
  public selectedGeographyID: number;
  public selectedGeographyNameLowered: string;

  public waterAccountHolders$: Observable<UserDto[]>;
  public selectedUser: UserDto;
  public selectedUserID: number;

  public noWaterAccountHolders: boolean = false;

  public isWaterAccountViewer: boolean = false;
  public isWaterAccountHolder: boolean = false;
  public isWaterManager: boolean = false;
  public isAdmin: boolean = false;
  public selectedGeographyAllowRequests: boolean = false;
  public showForm: boolean = false;

  public usersSubject = new BehaviorSubject<number>(null);

  public waterAccountSubject = new BehaviorSubject<number>(null);
  public waterAccounts$: Observable<WaterAccountRequestChangesDto[]>;
  public waterAccounts: WaterAccountRequestChangesDto[];
  public waterAccountZones: { [waterAccountID: number]: ZoneDisplayDto } = {};

  public waterAccountParcelIDsOnLoad: { [WaterAccountID: number]: number[] };
  public waterAccountParcelIDsOnLoadJSON: string;

  public parcelsToRemove: WaterAccountRequestChangesParcelDto[] = [];

  public customRichTextTypeID = CustomRichTextTypeEnum.WaterAccountRequestChanges;

  public isLoading: boolean = true;
  public isConsolidatingAccounts: boolean = false;
  public isLoadingSubmit: boolean = false;

  public userDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: 'Select a user',
    searchFn: (user: UserDto) => `${user.FullName} (${user.Email})`,
    displayFn: (user: UserDto) => `${user.FullName} (${user.Email})`,
  };

  constructor(
    private authenticationService: AuthenticationService,
    private waterAccountService: WaterAccountService,
    private waterAccountByGeographyService: WaterAccountByGeographyService,
    private geographyService: GeographyService,
    private userService: UserService,
    private confirmService: ConfirmService,
    private modalService: ModalService,
    private viewContainerRef: ViewContainerRef,
    private alertService: AlertService,
    private router: Router
  ) {}

  public ngOnInit() {
    this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
      tap((currentUser) => {
        this.currentUser = currentUser;

        // get users for 'View As' dropdown for system and and water managers
        this.isAdmin = this.currentUser.Rights?.WaterAccountRights?.CanUpdate;
        this.isWaterManager = Object.keys(currentUser.GeographyRights).filter((x) => currentUser.GeographyRights[x].WaterAccountRights.CanUpdate).length > 0;

        if (this.isAdmin || this.isWaterManager) {
          this.waterAccountHolders$ = this.usersSubject.pipe(
            switchMap((geographyID) => {
              if (!geographyID) return [];
              return this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsWaterAccountHoldersGet(geographyID);
            }),
            tap((users) => {
              this.selectedUser = users[0];
              this.selectedUserID = users[0]?.UserID;
              this.noWaterAccountHolders = users?.length == 0;
              this.getSelectedUserData();
            })
          );
        } else {
          this.selectedUserID = currentUser.UserID;
          this.getSelectedUserData();
        }

        this.geographies$ = this.geographyService.geographiesCurrentUserGet().pipe(
          tap((geographies) => {
            this.geographiesForTypeScript = geographies;
            if (geographies.length == 0) {
              this.isWaterAccountViewer = true;
              this.isWaterAccountHolder = false;
              this.showForm = false;
              return;
            }

            const selectedGeography = geographies[0];
            this.selectedGeographyAllowRequests = selectedGeography.AllowLandownersToRequestAccountChanges || false;
            this.selectedGeographyID = selectedGeography.GeographyID;
            this.selectedGeographyNameLowered = selectedGeography.GeographyName.toLowerCase();
            this.geographyID = this.selectedGeographyID;
            this.refreshWaterAccounts();
            this.refreshUsers();
          })
        );
      })
    );
  }

  private getSelectedUserData() {
    this.waterAccounts$ = this.waterAccountSubject.pipe(
      tap(() => (this.isLoading = true)),
      switchMap((geographyID) => {
        if (!geographyID || !this.selectedUserID) return [];
        return this.userService.geographiesGeographyIDUsersUserIDWaterAccountsGet(geographyID, this.selectedUserID);
      }),
      tap((waterAccounts) => {
        this.waterAccounts = waterAccounts;

        this.isWaterAccountViewer = this.waterAccounts.length == 0;
        this.isWaterAccountHolder = this.waterAccounts.length > 0;

        this.setFormVisibility();

        this.parcelsToRemove = [];

        this.waterAccountParcelIDsOnLoad = this.getWaterAccountParcelIDs();
        this.waterAccountParcelIDsOnLoadJSON = JSON.stringify(this.waterAccountParcelIDsOnLoad);

        this.updateWaterAccountZones();
        this.isLoading = false;
      })
    );
  }

  public setFormVisibility() {
    this.showForm = this.isAdmin || this.isWaterManager || (this.isWaterAccountHolder && this.selectedGeographyAllowRequests);
  }

  public getWaterAccountParcelIDs() {
    const waterAccountParcelIDs = {};
    this.waterAccounts.forEach((x) => {
      waterAccountParcelIDs[x.WaterAccountID] = x.Parcels?.map((x) => x.ParcelID) ?? [];
    });

    return waterAccountParcelIDs;
  }

  public canExit(): boolean {
    if (this.waterAccountParcelIDsOnLoadJSON == null) return true;

    return this.waterAccountParcelIDsOnLoadJSON == JSON.stringify(this.getWaterAccountParcelIDs());
  }

  private refreshWaterAccounts() {
    this.waterAccountSubject.next(this.geographyID);
  }

  private refreshUsers() {
    this.usersSubject.next(this.geographyID);
  }
  public updateWaterAccountZones() {
    this.waterAccounts.forEach((x) => this.setWaterAccountZone(x));
  }

  public setWaterAccountZone(waterAccount: WaterAccountRequestChangesDto) {
    if (waterAccount.Parcels.length == 0) {
      this.waterAccountZones[waterAccount.WaterAccountID] = null;
    }

    const zone = waterAccount.Parcels[0]?.AllocationZone;
    const conflictingZoneIndex = waterAccount.Parcels.findIndex((x) => x.AllocationZone?.ZoneID != zone?.ZoneID);

    this.waterAccountZones[waterAccount.WaterAccountID] = conflictingZoneIndex == -1 ? zone : { ZoneID: -1 };
  }

  public onDrop(event: CdkDragDrop<WaterAccountRequestChangesParcelDto[]>) {
    if (event.previousContainer === event.container) return;

    transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.container.data.length);

    this.updateWaterAccountZones();
  }

  public onUserSelected(selectedUser: UserDto) {
    this.selectedUserID = selectedUser.UserID;
    this.refreshWaterAccounts();
  }

  public onGeographySelected() {
    if (this.canExit()) {
      this.geographyID = this.selectedGeographyID;

      const geography = this.geographiesForTypeScript.find((x) => {
        return x.GeographyID == this.selectedGeographyID;
      });

      if (geography != null) {
        this.selectedGeographyAllowRequests = geography.AllowLandownersToRequestAccountChanges || false;
        this.selectedGeographyNameLowered = geography.GeographyName.toLowerCase();
      }

      this.refreshWaterAccounts();
      this.refreshUsers();
      return;
    }

    this.confirmAction('Switch Geographies', 'switch geographies').then((confirmed) => {
      if (confirmed) {
        this.geographyID = this.selectedGeographyID;
        this.refreshWaterAccounts();
        this.refreshUsers();
      } else {
        this.selectedGeographyID = this.geographyID;
      }
    });
  }

  public reset() {
    if (this.canExit()) {
      this.refreshWaterAccounts();
      return;
    }

    this.confirmAction('Reset', 'reset').then((confirmed) => {
      if (confirmed) {
        this.refreshWaterAccounts();
      }
    });
  }

  private confirmAction(buttonTextYes: string, actionDescription: string): Promise<boolean> {
    const message = `You have unsaved changes on this page that will be lost if you ${actionDescription}. Are you sure you wish to proceed?`;
    const confirmed = this.confirmService
      .confirm({
        title: 'Unsaved Changes',
        message: message,
        buttonTextYes: buttonTextYes,
        buttonClassYes: 'btn-danger',
        buttonTextNo: 'Cancel',
      })
      .then((confirmed) => {
        return confirmed;
      });

    return confirmed;
  }

  public removeParcel(parcel: WaterAccountRequestChangesParcelDto, waterAccount: WaterAccountRequestChangesDto) {
    const index = waterAccount.Parcels.findIndex((x) => x.ParcelID == parcel.ParcelID);
    waterAccount.Parcels.splice(index, 1);

    this.parcelsToRemove.push(parcel);
    this.setWaterAccountZone(waterAccount);
  }

  public restoreParcel(parcel: WaterAccountRequestChangesParcelDto) {
    const index = this.parcelsToRemove.findIndex((x) => x.ParcelID == parcel.ParcelID);
    this.parcelsToRemove.splice(index, 1);

    const originalWaterAccount = this.waterAccounts.find((x) => x.WaterAccountID == parcel.WaterAccountID);
    originalWaterAccount.Parcels.push(parcel);

    this.setWaterAccountZone(originalWaterAccount);
  }

  public openUpdateInfoModal(waterAccount: WaterAccountRequestChangesDto): void {
    this.modalService
      .open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
        WaterAccountID: waterAccount.WaterAccountID,
        GeographyID: this.geographyID,
      } as WaterAccountContext)
      .instance.result.then((result) => {
        if (result) {
          waterAccount.WaterAccountName = result.WaterAccountName;
          waterAccount.ContactName = result.ContactName;
          waterAccount.ContactAddress = result.ContactAddress;
        }
      });
  }

  public consolidateWaterAccounts() {
    const parcels = this.waterAccounts.reduce((parcels, waterAccount) => {
      parcels.push(...waterAccount.Parcels);
      return parcels;
    }, []);

    const zoneIDs = parcels.reduce((zoneIDs, parcel) => {
      if (!zoneIDs.find((x) => x == parcel.AllocationZone?.ZoneID)) zoneIDs.push(parcel.AllocationZone?.ZoneID ?? -1);
      return zoneIDs;
    }, []);

    const modifiedWaterAccountParcels = {};
    this.waterAccounts.forEach((waterAccount, i) => {
      if (zoneIDs[i] == undefined) {
        modifiedWaterAccountParcels[waterAccount.WaterAccountID] = [];
      } else if (zoneIDs[i] == -1) {
        modifiedWaterAccountParcels[waterAccount.WaterAccountID] = parcels.filter((x) => !x.AllocationZone);
      } else {
        modifiedWaterAccountParcels[waterAccount.WaterAccountID] = parcels.filter((x) => x.AllocationZone?.ZoneID == zoneIDs[i]);
      }

      modifiedWaterAccountParcels[waterAccount.WaterAccountID].sort((a, b) => (a.ParcelNumber < b.ParcelNumber ? -1 : a.ParcelNumber > b.ParcelNumber ? 1 : 0));
    });

    const waterAccountModificationsModalContext = this.waterAccounts.map((x) => {
      return {
        WaterAccountID: x.WaterAccountID,
        WaterAccountName: x.WaterAccountName,
        WaterAccountNumber: x.WaterAccountNumber,
        ParcelsCount: modifiedWaterAccountParcels[x.WaterAccountID]?.length ?? 0,
        AddedParcelsCount: modifiedWaterAccountParcels[x.WaterAccountID].filter((y) => x.Parcels?.findIndex((z) => z.ParcelID == y.ParcelID) < 0).length ?? 0,
        RemovedParcelsCount: x.Parcels?.filter((y) => modifiedWaterAccountParcels[x.WaterAccountID].findIndex((z) => z.ParcelID == y.ParcelID) < 0).length ?? 0,
      } as ModifiedWaterAccountContext;
    });

    this.modalService
      .open(WaterAccountsConsolidateModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
        WaterAccounts: waterAccountModificationsModalContext,
        GeographyID: this.geographyID,
      } as WaterAccountModificationsContext)
      .instance.result.then((confirmed) => {
        if (confirmed) {
          this.isConsolidatingAccounts = true;

          this.waterAccounts.forEach((x) => {
            x.Parcels = modifiedWaterAccountParcels[x.WaterAccountID];
          });

          this.updateWaterAccountZones();
          this.isConsolidatingAccounts = false;
        }
      });
  }

  public submitWaterAccountChanges() {
    const updatedWaterAccountParcelsIDs = {};
    this.waterAccounts.forEach((x) => {
      updatedWaterAccountParcelsIDs[x.WaterAccountID] = x.Parcels?.map((x) => x.ParcelID) ?? [];
    });

    const waterAccountModificationsModalContext = this.waterAccounts.map((x) => {
      return {
        WaterAccountID: x.WaterAccountID,
        WaterAccountName: x.WaterAccountName,
        WaterAccountNumber: x.WaterAccountNumber,
        ParcelsCount: updatedWaterAccountParcelsIDs[x.WaterAccountID].length,
        AddedParcelsCount: updatedWaterAccountParcelsIDs[x.WaterAccountID].filter((parcelID) => !this.waterAccountParcelIDsOnLoad[x.WaterAccountID].includes(parcelID))
          .length,
        RemovedParcelsCount: this.waterAccountParcelIDsOnLoad[x.WaterAccountID].filter((parcelID) => !updatedWaterAccountParcelsIDs[x.WaterAccountID].includes(parcelID))
          .length,
      } as ModifiedWaterAccountContext;
    });

    let hasInvalidZones = false;
    this.waterAccounts.forEach((waterAccount) => {
      if (!waterAccount.Parcels || waterAccount.Parcels.length == 0) return;
      const zoneID = waterAccount.Parcels[0].AllocationZone.ZoneID;
      const conflictingZoneIndex = waterAccount.Parcels.findIndex((x) => x.AllocationZone.ZoneID != zoneID);

      if (conflictingZoneIndex > 0) hasInvalidZones = true;
    });

    this.modalService
      .open(WaterAccountRequestChangesConfirmModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
        WaterAccounts: waterAccountModificationsModalContext,
        GeographyID: this.geographyID,
        HasInvalidZones: hasInvalidZones,
      } as WaterAccountModificationsContext)
      .instance.result.then((confirmed) => {
        if (confirmed) {
          this.isLoadingSubmit = true;

          const requestDto = new WaterAccountParcelsRequestChangesDto({
            WaterAccounts: this.waterAccounts,
            ParcelsToRemove: this.parcelsToRemove,
          });

          this.userService.geographiesGeographyIDUsersUserIDWaterAccountsPut(this.geographyID, this.selectedUserID, requestDto).subscribe({
            next: () => {
              this.isLoadingSubmit = false;
              this.waterAccountParcelIDsOnLoadJSON = null;

              this.router.navigate(['water-dashboard', 'water-accounts']).then(() => {
                this.alertService.pushAlert(new Alert('Water account changes applied successfully.', AlertContext.Success));
              });
            },
            error: () => (this.isLoadingSubmit = false),
          });
        }
      });
  }
}
