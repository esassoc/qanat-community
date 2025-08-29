import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from "@angular/core";
import { Router, ActivatedRoute, RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { Observable, combineLatest, of } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { routeParams } from "src/app/app.routes";
import { switchMap, tap } from "rxjs/operators";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographyMinimalDto, GeographyUserDto, UserDto, WaterAccountUserMinimalDto, WellRegistrationUserDetailDto } from "src/app/shared/generated/model/models";
import { UserService } from "src/app/shared/generated/api/user.service";
import { ImpersonationService } from "src/app/shared/generated/api/impersonation.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { DeleteWellModalComponent } from "src/app/shared/components/well/modals/delete-well-modal/delete-well-modal.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { UpdateUserInformationModalComponent } from "./modals/update-user-information-modal/update-user-information-modal.component";
import { AddWaterAccountUserModalComponent } from "./modals/add-water-account-user-modal/add-water-account-user-modal.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { AsyncPipe, NgClass } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { UpdateWaterAccountUserRoleModalComponent } from "src/app/shared/components/update-water-account-user-role-modal/update-water-account-user-role-modal.component";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { environment } from "src/environments/environment";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { AgGridAngular } from "ag-grid-angular";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "template-user-detail",
    templateUrl: "./user-detail.component.html",
    styleUrls: ["./user-detail.component.scss"],
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        NgClass,
        PageHeaderComponent,
        AlertDisplayComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        IconComponent,
        QanatGridHeaderComponent,
        QanatGridComponent,
        AsyncPipe,
        RouterLink,
    ],
})
export class UserDetailComponent implements OnInit, OnDestroy {
    public userAndCurrentUser$: Observable<[UserDto, UserDto, GeographyMinimalDto]>;
    public user: UserDto;
    private currentUser: UserDto;
    public isCurrentUser: boolean;
    public currentUserIsAdmin: boolean = false;
    public currentUserIsWaterManager: boolean = false;
    public canImpersonateUser: boolean = false;

    public userWaterAccounts$: Observable<WaterAccountUserMinimalDto[]>;
    public userGeographyPermissions$: Observable<GeographyUserDto[]>;
    public wellRegistrations$: Observable<WellRegistrationUserDetailDto[]>;
    public userIsAdmin: boolean = false;

    public geographyWaterAccountRoleDictionary: { [key: number]: string } = {};
    public isGeographyWaterManagerDictionary: { [key: number]: string } = {};

    public waterAccountGridColumnDefs: ColDef[];
    public waterAccountCSVDownloadColIDsToExclude = ["0"];
    public waterAccountGridApi: GridApi;
    public waterAccountGridRef: AgGridAngular;

    public wellRegistrationGridColumnDefs: ColDef[];
    public wellRegistrationCSVDownloadColIDsToExclude = ["0"];
    public wellRegistrationGridApi: GridApi;

    public displayProfileEdit: boolean = false;
    public geographySpecific: boolean = false;
    public geography$: Observable<GeographyMinimalDto>;
    public apiKey$: Observable<string>;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterAccountUserService: WaterAccountUserService,
        private impersonationService: ImpersonationService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService
    ) {}

    ngOnInit() {
        const userIDFromRoute = parseInt(this.route.snapshot.paramMap.get(routeParams.userID));
        this.displayProfileEdit = this.route.snapshot.data.displayProfileEdit;
        this.geographySpecific = this.route.snapshot.data.geographySpecific;

        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                let geographyName = params[routeParams.geographyName];
                if (geographyName) {
                    return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
                }
                return of(null);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );

        const userAction = isNaN(userIDFromRoute) ? this.authenticationService.getCurrentUser() : this.userService.getUser(userIDFromRoute);

        this.userAndCurrentUser$ = combineLatest([userAction, this.authenticationService.getCurrentUser(), this.geography$]).pipe(
            tap(([userInQuestion, currentUser, geography]) => {
                this.user = userInQuestion;
                this.userIsAdmin = AuthorizationHelper.isSystemAdministrator(this.user);
                this.currentUser = currentUser;
                this.isCurrentUser = this.user.UserID == this.currentUser.UserID;
                this.currentUserIsAdmin = AuthorizationHelper.isSystemAdministrator(this.currentUser);
                this.currentUserIsWaterManager = geography ? AuthorizationHelper.hasGeographyFlag(geography.GeographyID, FlagEnum.HasManagerDashboard, this.currentUser) : false;
                this.canImpersonateUser = !environment.production && this.authenticationService.hasFlag(this.currentUser, FlagEnum.CanImpersonateUsers);
                this.cdr.detectChanges();
                this.getWaterAccounts();

                this.wellRegistrations$ = this.userService.listWellRegistrationsUser(this.user.UserID);

                this.userGeographyPermissions$ = this.userService.getGeographyPermissionsUser(this.user.UserID).pipe(
                    tap((userGeographyPermissions) => {
                        const tempDictionary = {};

                        userGeographyPermissions.forEach((userGeographyPermission) => {
                            const geographyID = userGeographyPermission.Geography.GeographyID;
                            const geographyRoleName = userGeographyPermission.GeographyRole.GeographyRoleDisplayName;

                            if (!tempDictionary[geographyID]) {
                                tempDictionary[geographyID] = new Set(); //Use a set to avoid duplicates.
                            }

                            if (AuthorizationHelper.hasGeographyFlag(geographyID, FlagEnum.HasManagerDashboard, userGeographyPermission.User)) {
                                tempDictionary[geographyID].add(geographyRoleName);
                            }
                        });

                        Object.keys(tempDictionary).forEach((key) => {
                            const sortedRoles = Array.from(tempDictionary[key]).sort();
                            this.isGeographyWaterManagerDictionary[key] = sortedRoles.join(", ");
                        });
                    })
                );
                this.apiKey$ = this.userService.getApiKeyUser(this.user.UserID);
            })
        );

        this.createWaterAccountGridColumnDefs();
        this.createWellRegistrationGridColumnDefs();
    }

    getWaterAccounts() {
        this.userWaterAccounts$ = this.waterAccountUserService.getUserWaterAccountsWaterAccountUser(this.user.UserID).pipe(
            tap((userWaterAccounts) => {
                const tempDictionary = {};

                userWaterAccounts.forEach((userWaterAccount) => {
                    const geographyID = userWaterAccount.WaterAccount.GeographyID;
                    const waterAccountRoleName = userWaterAccount.WaterAccountRole.WaterAccountRoleDisplayName;

                    if (!tempDictionary[geographyID]) {
                        tempDictionary[geographyID] = new Set(); //Use a set to avoid duplicates.
                    }

                    tempDictionary[geographyID].add(waterAccountRoleName);
                });

                Object.keys(tempDictionary).forEach((key) => {
                    const sortedRoles = Array.from(tempDictionary[key]).sort();
                    if (sortedRoles.length > 0) {
                        this.geographyWaterAccountRoleDictionary[key] = sortedRoles.join(", ");
                    }
                });
            })
        );
    }

    impersonateUser(userID: number) {
        this.impersonationService.impersonateUserImpersonation(userID).subscribe((response) => {
            this.currentUser = response;
            this.authenticationService.refreshUserInfo(response);
            this.cdr.detectChanges();
            this.router.navigateByUrl("/").then((x) => {
                this.alertService.pushAlert(new Alert(`Successfully impersonating user: ${this.currentUser.FullName}`, AlertContext.Success));
            });
        });
    }

    editProfile() {
        this.confirmService
            .confirm({
                title: "Edit Profile",
                message: "Editing your profile will require you to log back in to the application.",
                buttonTextYes: "Okay",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((result) => {
                if (result) {
                    this.authenticationService.editProfile();
                }
            });
    }

    generateNewApiKeyModal() {
        this.confirmService
            .confirm({
                title: "Generate New API Key",
                message: "Are you sure you want to generate a new API key? This will invalidate your current API key.",
                buttonTextYes: "Confirm",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((result) => {
                if (result) {
                    this.userService.generateNewApiKeyUser(this.user.UserID).subscribe((response) => {
                        this.apiKey$ = of(response);
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("New API Key generated successfully.", AlertContext.Success));
                        this.cdr.markForCheck();
                    });
                }
            });
    }

    updateEmailAddress() {
        this.confirmService
            .confirm({
                title: "Edit Login Email",
                message: "Editing your login email will require you to log back in to the application.",
                buttonTextYes: "Okay",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((result) => {
                if (result) {
                    this.authenticationService.updateEmail();
                }
            });
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    updateUserInformationModal(systemRoleUpdate: boolean) {
        const dialogRef = this.dialogService.open(UpdateUserInformationModalComponent, {
            data: {
                User: this.user,
                SystemRoleEdit: systemRoleUpdate,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.user = result;
                this.cdr.markForCheck();
            }
        });
    }

    createWaterAccountGridColumnDefs() {
        this.waterAccountGridColumnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                const actions = [
                    {
                        ActionName: "Update Role",
                        ActionIcon: "fas fa-long-arrow-alt-right",
                        ActionHandler: () => {
                            this.updateWaterAccountRoleForUserModal(params.data);
                        },
                    },
                    {
                        ActionName: "Remove User From Account",
                        ActionIcon: "fa fa-times-circle text-danger",
                        ActionHandler: () => {
                            this.removeUserFromWaterAccount(params.data);
                        },
                    },
                ];
                return actions;
            }),
            this.utilityFunctionsService.createLinkColumnDef("Geography", "WaterAccount.Geography.GeographyName", "", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccount.Geography.GeographyName}/overview`, LinkDisplay: params.data.WaterAccount.Geography.GeographyName };
                },
                InRouterLink: "/geographies/",
                CustomDropdownFilterField: "WaterAccount.Geography.GeographyName",
            }),
            this.utilityFunctionsService.createLinkColumnDef("Account #", "WaterAccount.WaterAccountNumber", "WaterAccount.WaterAccountID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.WaterAccount.WaterAccountID}/water-budget`, LinkDisplay: params.data.WaterAccount.WaterAccountNumber };
                },
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
                FieldDefinitionLabelOverride: "Water Account #",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Role", "WaterAccountRole.WaterAccountRoleDisplayName", {
                CustomDropdownFilterField: "WaterAccountRole.WaterAccountRoleDisplayName",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Account Name", "WaterAccount.WaterAccountName"),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "WaterAccount.ContactName"),
            this.utilityFunctionsService.createBasicColumnDef("Contact Address", "WaterAccount.FullAddress"),
        ];
    }

    public onWaterAccountGridReady(params: GridReadyEvent) {
        this.waterAccountGridApi = params.api;
    }

    public onWaterAccountGridRefReady($event: AgGridAngular) {
        this.waterAccountGridRef = $event;
        this.cdr.detectChanges();
    }

    public addWaterAccountUserModal() {
        const dialogRef = this.dialogService.open(AddWaterAccountUserModalComponent, {
            data: {
                User: this.user,
                SystemRoleEdit: false,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.getWaterAccounts();
                this.alertService.pushAlert(new Alert(`Water account succesfully added.`, AlertContext.Success, true));
                this.cdr.markForCheck();
            }
        });
    }

    public updateWaterAccountRoleForUserModal(waterAccountUser: WaterAccountUserMinimalDto) {
        const dialogRef = this.dialogService.open(UpdateWaterAccountUserRoleModalComponent, {
            data: {
                WaterAccountUser: waterAccountUser,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.getWaterAccounts();
                this.alertService.pushAlert(new Alert(`Water account succesfully added.`, AlertContext.Success, true));
                this.cdr.markForCheck();
            }
        });
    }

    public removeUserFromWaterAccount(waterAccountUser: WaterAccountUserMinimalDto) {
        this.confirmService
            .confirm({
                title: `Remove ${waterAccountUser.User.FullName} from ${waterAccountUser.WaterAccount.WaterAccountNumber}`,
                message: `Are you sure you want to remove ${waterAccountUser.UserFullName} from Water Account #${waterAccountUser.WaterAccount.WaterAccountNumber}?`,
                buttonTextYes: "Remove User",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.waterAccountUserService
                        .removeUserFromWaterAccountWaterAccountUser(waterAccountUser.WaterAccount.WaterAccountID, waterAccountUser.WaterAccountUserID)
                        .subscribe((response) => {
                            this.getWaterAccounts();
                            this.alertService.pushAlert(
                                new Alert(`User successfully removed from Water Account #${waterAccountUser.WaterAccount.WaterAccountNumber}.`, AlertContext.Success, true)
                            );
                            this.cdr.markForCheck();
                        });
                }
            });
    }

    createWellRegistrationGridColumnDefs() {
        this.wellRegistrationGridColumnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                if (
                    params.data.WellRegistrationStatus.WellRegistrationStatusID != WellRegistrationStatusEnum.Draft &&
                    params.data.WellRegistrationStatus.WellRegistrationStatusID != WellRegistrationStatusEnum.Returned
                ) {
                    return null;
                }
                const actions = [
                    {
                        ActionName: "Continue",
                        ActionIcon: "fas fa-long-arrow-alt-right",
                        ActionHandler: () =>
                            this.router.navigateByUrl(`well-registry/${params.data.Geography.GeographyName.toLowerCase()}/well/${params.data.WellRegistrationID}/edit`),
                    },
                    {
                        ActionName: "Delete",
                        ActionIcon: "fa fa-times-circle text-danger",
                        ActionHandler: () => this.deleteWell(params.data.WellID),
                    },
                ];
                return actions;
            }),
            this.utilityFunctionsService.createLinkColumnDef("Well Name", "WellName", "WellID", {
                ValueGetter: (params) => {
                    return {
                        LinkValue: `${params.data.Geography.GeographyName.toLowerCase()}/well-registrations/${params.data.WellRegistrationID}`,
                        LinkDisplay: params.data.WellName ?? "Unnamed Well",
                    };
                },
                InRouterLink: "/wells/",
                FieldDefinitionType: "WellName",
                FieldDefinitionLabelOverride: "Well Name",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Geography", "Geography.GeographyName", { CustomDropdownFilterField: "Geography.GeographyName" }),
            this.utilityFunctionsService.createBasicColumnDef("Parcel", "Parcel.ParcelNumber"),
            this.utilityFunctionsService.createBasicColumnDef("Status", "WellRegistrationStatus.WellRegistrationStatusDisplayName", {
                CustomDropdownFilterField: "WellRegistrationStatus.WellRegistrationStatusDisplayName",
            }),
            this.utilityFunctionsService.createDateColumnDef("Date Submitted", "SubmitDate", "short"),
            this.utilityFunctionsService.createDateColumnDef("Date Approved", "ApprovalDate", "short"),
        ];
    }

    public onWellRegistrationGridReady(params: GridReadyEvent) {
        this.wellRegistrationGridApi = params.api;
    }

    private deleteWell(wellID: number) {
        const dialogRef = this.dialogService.open(DeleteWellModalComponent, {
            data: {
                WellID: wellID,
                UpdatingTechnicalInfo: false,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                const selectedData = this.wellRegistrationGridApi.getSelectedRows();
                this.wellRegistrationGridApi.applyTransaction({ remove: selectedData });
            }
        });
    }
}
