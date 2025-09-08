import { CdkDragDrop, transferArrayItem, CdkDropListGroup, CdkDropList, CdkDrag, CdkDragHandle } from "@angular/cdk/drag-drop";
import { Component, OnInit } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { BehaviorSubject, Observable, map, switchMap, tap } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UpdateWaterAccountInfoComponent } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { UserService } from "src/app/shared/generated/api/user.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { WaterAccountParcelsRequestChangesDto } from "src/app/shared/generated/model/water-account-parcels-request-changes-dto";
import { WaterAccountRequestChangesDto } from "src/app/shared/generated/model/water-account-request-changes-dto";
import { WaterAccountRequestChangesParcelDto } from "src/app/shared/generated/model/water-account-request-changes-parcel-dto";
import { ZoneDisplayDto } from "src/app/shared/generated/model/zone-display-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormsModule } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ParcelListItemComponent } from "src/app/shared/components/parcel/parcel-list-item/parcel-list-item.component";
import {
    ModifiedWaterAccountContext,
    WaterAccountsConsolidateModalComponent,
} from "src/app/shared/components/water-account/modals/water-accounts-consolidate-modal/water-accounts-consolidate-modal.component";
import { WaterAccountRequestChangesConfirmModalComponent } from "src/app/shared/components/water-account/modals/water-account-request-changes-confirm-modal/water-account-request-changes-confirm-modal.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "water-account-request-changes",
    templateUrl: "./water-account-request-changes.component.html",
    styleUrl: "./water-account-request-changes.component.scss",
    imports: [
        PageHeaderComponent,
        FormsModule,
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
        NgSelectModule,
    ],
})
export class WaterAccountRequestChangesComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public geographies$: Observable<GeographyMinimalDto[]>;
    public geographies: GeographyMinimalDto[];
    public currentGeography$: Observable<GeographyMinimalDto>;

    // selectedGeographyID is updated when a new geography is selected; if there are unsaved changes, a confirmation modal is launched
    // before selection is pushed to geographyID and form is updated
    public geographyID: number;
    public geographyNameLowered: string;
    public selectedGeographyID: number;

    public waterAccountHolders$: Observable<FormInputOption[]>;
    public selectedUserID: number;

    public noWaterAccountHolders: boolean = false;

    public isWaterAccountViewer: boolean = false;
    public isWaterAccountHolder: boolean = false;
    public isWaterManager: boolean = false;
    public isAdmin: boolean = false;
    public selectedGeographyAllowRequests: boolean = false;
    public showForm: boolean = false;

    public geographyDataSubject = new BehaviorSubject<number>(null);
    public userSubject = new BehaviorSubject<number>(null);
    public waterAccounts$: Observable<WaterAccountRequestChangesDto[]>;
    public waterAccounts: WaterAccountRequestChangesDto[];
    public waterAccountZones: { [waterAccountID: number]: ZoneDisplayDto } = {};

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public defaultYear: number;

    public waterAccountParcelIDsOnLoad: { [WaterAccountID: number]: number[] };
    public waterAccountParcelIDsOnLoadJSON: string;

    public parcelsToRemove: WaterAccountRequestChangesParcelDto[] = [];

    public customRichTextTypeID = CustomRichTextTypeEnum.WaterAccountRequestChanges;

    public isLoading: boolean = true;
    public isConsolidatingAccounts: boolean = false;
    public isLoadingSubmit: boolean = false;

    constructor(
        private authenticationService: AuthenticationService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private geographyService: GeographyService,
        private userService: UserService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private router: Router,
        private reportingPeriodService: ReportingPeriodService,
        private currentGeographyService: CurrentGeographyService,
        private dialogService: DialogService
    ) {}

    public ngOnInit() {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                this.currentUser = currentUser;

                // get users for 'View As' dropdown for system and and water managers
                this.isAdmin = this.currentUser.Rights?.WaterAccountRights?.CanUpdate;
                this.isWaterManager = Object.keys(currentUser.GeographyRights).filter((x) => currentUser.GeographyRights[x].WaterAccountRights.CanUpdate).length > 0;

                if (this.isAdmin || this.isWaterManager) {
                    this.waterAccountHolders$ = this.userSubject.pipe(
                        switchMap((geographyID) => {
                            if (!geographyID) return [];
                            return this.waterAccountByGeographyService.listWaterAccountHoldersByGeographyIDWaterAccountByGeography(geographyID);
                        }),
                        map((users) => {
                            return users.map((x) => {
                                return {
                                    Value: x.UserID,
                                    Label: x.FullName,
                                } as FormInputOption;
                            });
                        }),
                        tap((users) => {
                            this.noWaterAccountHolders = users?.length == 0;

                            var selectedUserIndex = users.findIndex((x) => x.UserID == this.selectedUserID);
                            if (selectedUserIndex == -1) {
                                this.selectedUserID = null;
                                this.isWaterAccountHolder = false;
                                this.isWaterAccountViewer = false;
                                this.showForm = false;
                            } else {
                                this.getSelectedUserData();
                            }
                        })
                    );
                } else {
                    this.selectedUserID = currentUser.UserID;
                    this.getSelectedUserData();
                }

                this.geographies$ = this.geographyService.listForCurrentUserGeography().pipe(
                    tap((geographies) => {
                        this.geographies = geographies;
                        if (geographies.length == 0) {
                            this.isWaterAccountViewer = true;
                            this.isWaterAccountHolder = false;
                            this.showForm = false;
                            return;
                        }

                        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
                            tap((currentGeography) => {
                                const selectedGeography = currentGeography ?? this.geographies[0];
                                this.selectedGeographyAllowRequests = selectedGeography.AllowLandownersToRequestAccountChanges || false;
                                this.selectedGeographyID = selectedGeography.GeographyID;
                                this.geographyID = this.selectedGeographyID;
                                this.geographyNameLowered = selectedGeography.GeographyName.toLowerCase();
                                this.refreshGeographyData();
                                this.refreshUsers();
                            })
                        );
                    })
                );
            })
        );
    }

    private getSelectedUserData() {
        this.waterAccounts$ = this.geographyDataSubject.pipe(
            tap(() => (this.isLoading = true)),
            switchMap((geographyID) => {
                if (!geographyID || !this.selectedUserID) return [];
                return this.geographyService.listWaterAccountsOwnedByCurrentUserGeography(geographyID, this.selectedUserID);
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

        this.reportingPeriods$ = this.geographyDataSubject.pipe(
            switchMap((geographyID) => {
                if (!geographyID) return [];
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geographyID);
            }),
            tap((reportingPeriods) => {
                let defaultReportingPeriod = reportingPeriods.find((rp) => rp.IsDefault);
                if (!defaultReportingPeriod) {
                    defaultReportingPeriod = reportingPeriods[0];
                }

                this.defaultYear = new Date(defaultReportingPeriod.EndDate).getFullYear();
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

    private refreshGeographyData() {
        this.geographyDataSubject.next(this.geographyID);
    }

    private refreshUsers() {
        this.userSubject.next(this.geographyID);
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

    public onUserSelected() {
        this.refreshGeographyData();
    }

    public onGeographySelected() {
        if (this.canExit()) {
            this.updateGeographyFromSelectedGeographyID();

            this.refreshGeographyData();
            this.refreshUsers();
            return;
        }

        this.confirmAction("Switch Geographies", "switch geographies").then((confirmed) => {
            if (confirmed) {
                this.updateGeographyFromSelectedGeographyID();

                this.refreshGeographyData();
                this.refreshUsers();
            } else {
                this.selectedGeographyID = this.geographyID;
            }
        });
    }

    private updateGeographyFromSelectedGeographyID() {
        this.geographyID = this.selectedGeographyID;

        const geography = this.geographies.find((x) => x.GeographyID == this.selectedGeographyID);
        if (!geography) return;

        this.selectedGeographyAllowRequests = geography.AllowLandownersToRequestAccountChanges || false;
        this.geographyNameLowered = geography.GeographyName.toLowerCase();
    }

    public reset() {
        if (this.canExit()) {
            this.refreshGeographyData();
            return;
        }

        this.confirmAction("Reset", "reset").then((confirmed) => {
            if (confirmed) {
                this.refreshGeographyData();
            }
        });
    }

    private confirmAction(buttonTextYes: string, actionDescription: string): Promise<boolean> {
        const message = `You have unsaved changes on this page that will be lost if you ${actionDescription}. Are you sure you wish to proceed?`;
        const confirmed = this.confirmService
            .confirm({
                title: "Unsaved Changes",
                message: message,
                buttonTextYes: buttonTextYes,
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
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
        const dialogRef = this.dialogService.open(UpdateWaterAccountInfoComponent, {
            data: {
                WaterAccountID: waterAccount.WaterAccountID,
                GeographyID: this.geographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                waterAccount.WaterAccountName = result.WaterAccountName;
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
        const dialogRef = this.dialogService.open(WaterAccountsConsolidateModalComponent, {
            data: {
                WaterAccounts: waterAccountModificationsModalContext,
                GeographyID: this.geographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
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
            if (waterAccount.Parcels && waterAccount.Parcels.length != 0 && waterAccount.Parcels[0].AllocationZone != null) {
                const zoneID = waterAccount.Parcels[0].AllocationZone?.ZoneID;
                const conflictingZoneIndex = waterAccount.Parcels.findIndex((x) => x.AllocationZone.ZoneID != zoneID);

                if (conflictingZoneIndex > 0) hasInvalidZones = true;
            }
        });
        const dialogRef = this.dialogService.open(WaterAccountRequestChangesConfirmModalComponent, {
            data: {
                WaterAccounts: waterAccountModificationsModalContext,
                GeographyID: this.geographyID,
                HasInvalidZones: hasInvalidZones,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.isLoadingSubmit = true;

                const requestDto = new WaterAccountParcelsRequestChangesDto({
                    WaterAccounts: this.waterAccounts,
                    ParcelsToRemove: this.parcelsToRemove,
                });

                this.geographyService.updateWaterAccountsOwnedByCurrentUserGeography(this.geographyID, this.selectedUserID, requestDto).subscribe({
                    next: () => {
                        this.isLoadingSubmit = false;
                        this.waterAccountParcelIDsOnLoadJSON = null;

                        this.router.navigate(["water-dashboard", "water-accounts"]).then(() => {
                            this.alertService.pushAlert(new Alert("Water account changes applied successfully.", AlertContext.Success));
                        });
                    },
                    error: () => (this.isLoadingSubmit = false),
                });
            }
        });
    }
}
