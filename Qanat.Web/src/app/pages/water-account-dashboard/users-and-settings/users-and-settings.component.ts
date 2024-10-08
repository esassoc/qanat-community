import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable, forkJoin, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { GeographyRoleEnum } from "src/app/shared/generated/enum/geography-role-enum";
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
    ],
})
export class UsersAndSettingsComponent implements OnInit {
    public RoleEnum = RoleEnum;
    public WaterAccountRoleEnum = WaterAccountRoleEnum;

    public waterAccount: WaterAccountDto;
    public users: WaterAccountUserMinimalDto[];
    public currentUser: UserDto;
    public waterAccountUsers$: Observable<any>;
    public isCurrentUserAnAccountHolder: boolean = false;
    public isCurrentUserGeographyManager: boolean = false;
    public allocationPlans: AllocationPlanMinimalDto[];

    public isLoadingSubmit: boolean;

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
        const waterAccountID = parseInt(this.route.snapshot.paramMap.get(routeParams.waterAccountID));

        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });

        this.waterAccountUsers$ = forkJoin({
            waterAccount: this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID),
            users: this.waterAccountUserService.waterAccountsWaterAccountIDUsersGet(waterAccountID),
            allocationPlans: this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(waterAccountID),
        }).pipe(
            tap(({ waterAccount, users, allocationPlans }) => {
                this.waterAccount = waterAccount;
                this.users = users;
                if (this.users.find((x) => x.UserID == this.currentUser.UserID)) {
                    this.isCurrentUserAnAccountHolder = this.users.find((x) => x.UserID == this.currentUser.UserID).WaterAccountRoleID == WaterAccountRoleEnum.WaterAccountHolder;
                }
                if (this.currentUser.GeographyUser.length > 0) {
                    this.isCurrentUserGeographyManager =
                        this.currentUser.GeographyUser.find((x) => x.GeographyID == this.waterAccount.Geography.GeographyID)?.GeographyRoleID == GeographyRoleEnum.WaterManager;
                }
                this.allocationPlans = allocationPlans;
                this.isLoadingSubmit = false;
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
                    this.refreshWaterAccountUsers(waterAccount.WaterAccountID);
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
                    this.refreshWaterAccountUsers(this.waterAccount.WaterAccountID);
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
                            this.refreshWaterAccountUsers(this.waterAccount.WaterAccountID);
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

    private refreshWaterAccountUsers(waterAccountID: number) {
        this.waterAccountUsers$ = this.waterAccountUserService.waterAccountsWaterAccountIDUsersGet(waterAccountID).pipe(tap((users) => (this.users = users)));
    }
}

export class InviteToWaterAccountContext {
    CurrentUserID: number;
    WaterAccountID: number;
}
