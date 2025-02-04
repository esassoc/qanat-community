import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, Observable, Subject, forkJoin, map, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { WaterAccountRoleEnum } from "src/app/shared/generated/enum/water-account-role-enum";
import { AllocationPlanMinimalDto } from "src/app/shared/generated/model/allocation-plan-minimal-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { WaterAccountMinimalDto } from "src/app/shared/generated/model/water-account-minimal-dto";
import { WaterAccountUserMinimalDto } from "src/app/shared/generated/model/water-account-user-minimal-dto";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { InviteUserToWaterAccountModalComponent } from "./modals/invite-user-to-water-account-modal/invite-user-to-water-account-modal.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, NgClass, AsyncPipe, DatePipe } from "@angular/common";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { UpdateWaterAccountUserRoleModalComponent } from "src/app/shared/components/update-water-account-user-role-modal/update-water-account-user-role-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";

@Component({
    selector: "users-and-settings",
    templateUrl: "./users-and-settings.component.html",
    styleUrl: "./users-and-settings.component.scss",
    standalone: true,
    imports: [
        AlertDisplayComponent,
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        IconComponent,
        NgFor,
        NgClass,
        AsyncPipe,
        DatePipe,
        LoadingDirective,
    ],
})
export class UsersAndSettingsComponent implements OnInit {
    public waterAccount$: Observable<WaterAccountDto>;
    public waterAccount: WaterAccountDto;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    public waterAccountUsers$: Observable<WaterAccountUserMinimalDto[]>;
    public refreshWaterAccountUsers$: BehaviorSubject<number> = new BehaviorSubject(null);

    public currentUser: UserDto;
    public isCurrentUserAnAccountHolder: boolean = false;
    public isCurrentUserGeographyManager: boolean = false;

    public RoleEnum = RoleEnum;
    public WaterAccountRoleEnum = WaterAccountRoleEnum;

    public isLoadingSubmit: boolean = false;

    constructor(
        private waterAccountService: WaterAccountService,
        private waterAccountUserService: WaterAccountUserService,
        private route: ActivatedRoute,
        private authenticationService: AuthenticationService,
        private modalService: ModalService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });

        this.waterAccount$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const waterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
                return this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID);
            }),
            tap((waterAccount: WaterAccountDto) => {
                this.waterAccount = waterAccount;

                this.isCurrentUserGeographyManager = AuthorizationHelper.hasGeographyRolePermission(
                    this.waterAccount.Geography.GeographyID,
                    PermissionEnum.WaterAccountRights,
                    RightsEnum.Delete,
                    this.currentUser
                );
                this.allocationPlans$ = this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(waterAccount.WaterAccountID);
                this.refreshWaterAccountUsers$.next(waterAccount.WaterAccountID);
            })
        );

        this.waterAccountUsers$ = this.refreshWaterAccountUsers$.pipe(
            switchMap((waterAccountID) => {
                if (!waterAccountID) return [];
                return this.waterAccountUserService.waterAccountsWaterAccountIDUsersGet(waterAccountID);
            }),
            tap((waterAccountUsers) => {
                const currentWaterAccountUser = waterAccountUsers.find((x) => x.UserID == this.currentUser.UserID);
                if (currentWaterAccountUser) {
                    this.isCurrentUserAnAccountHolder = currentWaterAccountUser.WaterAccountRoleID == WaterAccountRoleEnum.WaterAccountHolder;
                }
            })
        );
    }

    public updateInfoModal(waterAccountID: number) {
        this.modalService
            .open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: waterAccountID,
                GeographyID: this.waterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.waterAccount = result;
                }
            });
    }

    public inviteUserToWaterAccountModal(waterAccount: WaterAccountMinimalDto) {
        this.alertService.clearAlerts();
        this.modalService
            .open(
                InviteUserToWaterAccountModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                },
                {
                    CurrentUserID: this.currentUser.UserID,
                    WaterAccountID: waterAccount.WaterAccountID,
                } as InviteToWaterAccountContext
            )
            .instance.result.then((result) => {
                if (result) {
                    this.refreshWaterAccountUsers$.next(waterAccount.WaterAccountID);
                }
            });
    }

    public updateUserWaterAccountRoleModal(userWaterAccount: WaterAccountUserMinimalDto) {
        const userDisplayName = userWaterAccount.User.RoleID == RoleEnum.PendingLogin ? userWaterAccount.User.Email : userWaterAccount.User.FullName;

        this.modalService
            .open(
                UpdateWaterAccountUserRoleModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                },
                {
                    WaterAccountUser: userWaterAccount,
                }
            )
            .instance.result.then((result) => {
                if (result) {
                    this.refreshWaterAccountUsers$.next(this.waterAccount.WaterAccountID);
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert(`Role updated for ${userDisplayName}.`, AlertContext.Success));
                }
            });
    }

    public resendUserInviteConfirmation(userWaterAccount: WaterAccountUserMinimalDto) {
        this.isLoadingSubmit = false;
        this.confirmService
            .confirm({
                title: "Confirm Resend",
                icon: "Resend",
                message: `Are you sure you want to resend an email to ${userWaterAccount.UserEmail} to invite them to Water Account #${userWaterAccount.WaterAccount.WaterAccountNumber}?`,
                buttonTextYes: "Save",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.waterAccountUserService
                        .waterAccountsWaterAccountIDInvitingUserInvitingUserIDResendPost(this.waterAccount.WaterAccountID, this.currentUser.UserID, userWaterAccount)
                        .subscribe(() => {
                            this.alertService.clearAlerts();
                            this.alertService.pushAlert(new Alert(`Invitation resent to ${userWaterAccount.UserEmail}.`, AlertContext.Success));
                            this.isLoadingSubmit = false;
                        });
                } else {
                    this.isLoadingSubmit = false;
                }
            });
    }

    public removeUserFromWaterAcountComfirmation(userWaterAccount: WaterAccountUserMinimalDto) {
        const userDisplayName = userWaterAccount.User.RoleID == RoleEnum.PendingLogin ? userWaterAccount.User.Email : userWaterAccount.User.FullName;

        this.confirmService
            .confirm({
                title: `Remove ${userDisplayName}`,
                icon: "User",
                message: `Are you sure you want to remove ${userWaterAccount.UserEmail} from Water Account #${userWaterAccount.WaterAccount.WaterAccountNumber}?`,
                buttonTextYes: "Remove User",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.waterAccountUserService
                        .waterAccountsWaterAccountIDUserWaterAccountUserIDDelete(this.waterAccount.WaterAccountID, userWaterAccount.WaterAccountUserID)
                        .subscribe(() => {
                            this.refreshWaterAccountUsers$.next(this.waterAccount.WaterAccountID);
                            this.alertService.clearAlerts();
                            this.alertService.pushAlert(
                                new Alert(`${userDisplayName} removed from Water Account #${userWaterAccount.WaterAccount.WaterAccountNumber}.`, AlertContext.Success)
                            );
                        });
                    this.isLoadingSubmit = false;
                } else {
                    this.isLoadingSubmit = false;
                }
            });
    }
}

export class InviteToWaterAccountContext {
    CurrentUserID: number;
    WaterAccountID: number;
}
