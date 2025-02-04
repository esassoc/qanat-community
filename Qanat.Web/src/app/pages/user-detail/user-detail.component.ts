import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy, ViewContainerRef } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { Observable, forkJoin } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { routeParams } from "src/app/app.routes";
import { tap } from "rxjs/operators";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographySimpleDto, GeographyUserDto, UserDto, WaterAccountUserMinimalDto, WellRegistrationUserDetailDto } from "src/app/shared/generated/model/models";
import { UserService } from "src/app/shared/generated/api/user.service";
import { ImpersonationService } from "src/app/shared/generated/api/impersonation.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ModalOptions, ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { DeleteWellModalComponent, WellContext } from "src/app/shared/components/well/delete-well-modal/delete-well-modal.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { GeographyRoleEnum } from "src/app/shared/generated/enum/geography-role-enum";
import { UpdateUserInformationModalComponent, UserContext } from "./modals/update-user-information-modal/update-user-information-modal.component";
import { AddWaterAccountUserModalComponent } from "./modals/add-water-account-user-modal/add-water-account-user-modal.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { NgIf, NgFor, AsyncPipe, NgClass } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import {
    UpdateWaterAccountUserRoleContext,
    UpdateWaterAccountUserRoleModalComponent,
} from "src/app/shared/components/update-water-account-user-role-modal/update-water-account-user-role-modal.component";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { environment } from "src/environments/environment";
import { env } from "process";

@Component({
    selector: "template-user-detail",
    templateUrl: "./user-detail.component.html",
    styleUrls: ["./user-detail.component.scss"],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [NgIf, PageHeaderComponent, AlertDisplayComponent, KeyValuePairListComponent, KeyValuePairComponent, NgFor, IconComponent, QanatGridComponent, AsyncPipe],
})
export class UserDetailComponent implements OnInit, OnDestroy {
    public userAndCurrentUser$: Observable<UserDto[]>;
    public user: UserDto;
    private currentUser: UserDto;
    public isCurrentUser: boolean;
    public currentUserIsAdmin: boolean = false;
    public canImpersonateUser: boolean = false;

    public userWaterAccounts$: Observable<WaterAccountUserMinimalDto[]>;
    public userGeographyPermissions$: Observable<GeographyUserDto[]>;
    public wellRegistrations$: Observable<WellRegistrationUserDetailDto[]>;
    public userIsAdmin: boolean = false;
    public geographiesWhereUserIsWaterManager: GeographySimpleDto[];

    public geographyWaterAccountRoleDictionary: { [key: number]: string } = {};
    public isGeographyWaterManagerDictionary: { [key: number]: string } = {};

    public waterAccountGridColumnDefs: ColDef[];
    public waterAccountCSVDownloadColIDsToExclude = ["0"];
    public waterAccountGridApi: GridApi;

    public wellRegistrationGridColumnDefs: ColDef[];
    public wellRegistrationCSVDownloadColIDsToExclude = ["0"];
    public wellRegistrationGridApi: GridApi;

    public displayProfileEdit: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private waterAccountUserService: WaterAccountUserService,
        private impersonationService: ImpersonationService,
        private authenticationService: AuthenticationService,
        private cdr: ChangeDetectorRef,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private utilityFunctionsService: UtilityFunctionsService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit() {
        const userIDFromRoute = parseInt(this.route.snapshot.paramMap.get(routeParams.userID));
        this.displayProfileEdit = this.route.snapshot.data.displayProfileEdit;
        const userAction = isNaN(userIDFromRoute) ? this.authenticationService.getCurrentUser() : this.userService.usersUserIDGet(userIDFromRoute);

        this.userAndCurrentUser$ = forkJoin([userAction, this.authenticationService.getCurrentUser()]).pipe(
            tap((x) => {
                this.user = x[0];
                this.userIsAdmin = AuthorizationHelper.isSystemAdministrator(this.user);
                this.currentUser = x[1];
                this.isCurrentUser = this.user.UserID == this.currentUser.UserID;
                this.currentUserIsAdmin = AuthorizationHelper.isSystemAdministrator(this.currentUser);
                this.canImpersonateUser = !environment.production && this.authenticationService.hasFlag(this.currentUser, FlagEnum.CanImpersonateUsers);

                this.getWaterAccounts();

                this.wellRegistrations$ = this.userService.usersUserIDWellRegistrationsGet(this.user.UserID);

                this.userGeographyPermissions$ = this.userService.userUserIDPermissionsGet(this.user.UserID).pipe(
                    tap((userGeographyPermissions) => {
                        const tempDictionary = {};

                        userGeographyPermissions.forEach((userGeographyPermission) => {
                            const geographyID = userGeographyPermission.Geography.GeographyID;
                            const geographyRoleName = userGeographyPermission.GeographyRole.GeographyRoleDisplayName;

                            if (!tempDictionary[geographyID]) {
                                tempDictionary[geographyID] = new Set(); //Use a set to avoid duplicates.
                            }

                            if (userGeographyPermission.GeographyRole.GeographyRoleID == GeographyRoleEnum.WaterManager) {
                                tempDictionary[geographyID].add(geographyRoleName);
                            }
                        });

                        Object.keys(tempDictionary).forEach((key) => {
                            const sortedRoles = Array.from(tempDictionary[key]).sort();
                            this.isGeographyWaterManagerDictionary[key] = sortedRoles.join(", ");
                        });

                        this.geographiesWhereUserIsWaterManager = userGeographyPermissions
                            .filter((x) => x.GeographyRole.GeographyRoleID == GeographyRoleEnum.WaterManager)
                            .map((x) => x.Geography);
                    })
                );
            })
        );

        this.createWaterAccountGridColumnDefs();
        this.createWellRegistrationGridColumnDefs();
    }

    getWaterAccounts() {
        this.userWaterAccounts$ = this.waterAccountUserService.userUserIDWaterAccountsGet(this.user.UserID).pipe(
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
        this.impersonationService.impersonateUserIDPost(userID).subscribe((response) => {
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
        this.modalService
            .open(
                UpdateUserInformationModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Large,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    User: this.user,
                    SystemRoleEdit: systemRoleUpdate,
                } as UserContext
            )
            .instance.result.then((result) => {
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
            this.utilityFunctionsService.createBasicColumnDef("Contact Address", "WaterAccount.ContactAddress"),
        ];
    }

    public onWaterAccountGridReady(params: GridReadyEvent) {
        this.waterAccountGridApi = params.api;
    }

    public addWaterAccountUserModal() {
        this.modalService
            .open(
                AddWaterAccountUserModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    User: this.user,
                } as UserContext
            )
            .instance.result.then((result) => {
                if (result) {
                    this.getWaterAccounts();
                    this.alertService.pushAlert(new Alert(`Water account succesfully added.`, AlertContext.Success, true));
                    this.cdr.markForCheck();
                }
            });
    }

    public updateWaterAccountRoleForUserModal(waterAccountUser: WaterAccountUserMinimalDto) {
        this.modalService
            .open(
                UpdateWaterAccountUserRoleModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    WaterAccountUser: waterAccountUser,
                } as UpdateWaterAccountUserRoleContext
            )
            .instance.result.then((result) => {
                if (result) {
                    this.getWaterAccounts();
                    this.alertService.pushAlert(new Alert(`Role successfully updated.`, AlertContext.Success, true));
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
                        .waterAccountsWaterAccountIDUserWaterAccountUserIDDelete(waterAccountUser.WaterAccount.WaterAccountID, waterAccountUser.WaterAccountUserID)
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
        this.modalService
            .open(DeleteWellModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, { WellID: wellID } as WellContext)
            .instance.result.then((result) => {
                if (result) {
                    const selectedData = this.wellRegistrationGridApi.getSelectedRows();
                    this.wellRegistrationGridApi.applyTransaction({ remove: selectedData });
                }
            });
    }
}
