import { DatePipe } from "@angular/common";
import { Component, ComponentRef, OnInit, ViewChild, ViewContainerRef } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { forkJoin } from "rxjs";
import { Alert } from "src/app/shared/models/alert";
import { routeParams } from "src/app/app.routes";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { AddUserByEmailDto } from "src/app/shared/generated/model/add-user-by-email-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { UserDto, WaterAccountDto, WaterAccountRoleSimpleDto, WaterAccountUserMinimalDto } from "src/app/shared/generated/model/models";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";

@Component({
    selector: "user-permissions",
    templateUrl: "./user-permissions.component.html",
    styleUrls: ["./user-permissions.component.scss"],
})
export class UserPermissionsComponent implements OnInit {
    @ViewChild("removeUserModal") removeUserModal;
    private removeUserModalComponent: ComponentRef<ModalComponent>;

    @ViewChild("updateUserRoleModal") updateUserRoleModal;
    private updateUserRoleModalComponent: ComponentRef<ModalComponent>;

    public waterAccount: WaterAccountDto;
    public users: WaterAccountUserMinimalDto[];
    public waterAccountRoles: WaterAccountRoleSimpleDto[];
    public selectedUser: WaterAccountUserMinimalDto;
    public addUserByEmailDto: AddUserByEmailDto;
    private currentUser: UserDto;

    public isLoadingSubmit: boolean;

    constructor(
        private waterAccountService: WaterAccountService,
        private waterAccountUserService: WaterAccountUserService,
        private route: ActivatedRoute,
        private datePipe: DatePipe,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private alertService: AlertService,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        const waterAccountID = parseInt(this.route.snapshot.paramMap.get(routeParams.waterAccountID));

        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });

        forkJoin({
            waterAccount: this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID),
            waterAccountRoles: this.waterAccountUserService.waterAccountRolesGet(),
            users: this.waterAccountUserService.waterAccountsWaterAccountIDUsersGet(waterAccountID),
        }).subscribe(({ waterAccount, waterAccountRoles, users }) => {
            this.waterAccount = waterAccount;
            this.waterAccountRoles = waterAccountRoles;
            this.users = users;
            this.addUserByEmailDto = new AddUserByEmailDto();
            this.addUserByEmailDto.WaterAccountRoleID = 0;

            this.isLoadingSubmit = false;
        });
    }

    removeUser() {
        this.isLoadingSubmit = true;
        this.waterAccountUserService.waterAccountsWaterAccountIDUserWaterAccountUserIDDelete(this.waterAccount.WaterAccountID, this.selectedUser.WaterAccountUserID).subscribe(
            (response) => {
                this.isLoadingSubmit = false;
                this.users.splice(this.users.indexOf(this.selectedUser), 1);
                this.closeRemoveModal();
                this.alertService.pushAlert(new Alert("User successfully removed!", AlertContext.Success, true));
            },
            (error) => {
                this.isLoadingSubmit = false;
                this.closeRemoveModal();
            }
        );
    }

    openRemoveModal(user: WaterAccountUserMinimalDto): void {
        this.selectedUser = user;
        this.removeUserModalComponent = this.modalService.open(this.removeUserModal);
    }

    closeRemoveModal(): void {
        if (!this.removeUserModalComponent) return;
        this.modalService.close(this.removeUserModalComponent);
    }

    openUpdateUserModal(user: WaterAccountUserMinimalDto): void {
        this.selectedUser = user;
        this.updateUserRoleModalComponent = this.modalService.open(this.updateUserRoleModal);
    }

    closeUpdateUserModal(): void {
        if (!this.updateUserRoleModalComponent) return;
        this.modalService.close(this.updateUserRoleModalComponent);
    }

    changeUserRole() {
        this.waterAccountUserService.waterAccountsWaterAccountIDUserUserIDPut(this.waterAccount.WaterAccountID, this.selectedUser.User.UserID, this.selectedUser).subscribe(
            (response) => {
                this.closeUpdateUserModal();
                this.alertService.pushAlert(new Alert("Role successfully updated!", AlertContext.Success, true));
            },
            (error) => {
                this.isLoadingSubmit = false;
                this.closeUpdateUserModal();
            }
        );
    }

    resendEmailToPendingUser(user: WaterAccountUserMinimalDto) {
        this.waterAccountUserService
            .waterAccountsWaterAccountIDInvitingUserInvitingUserIDResendPost(this.waterAccount.WaterAccountID, this.currentUser.UserID, user)
            .subscribe((response) => {
                this.alertService.pushAlert(new Alert(`Successfully resent email to ${user.UserEmail}.`));
            });
    }

    getDateFromString(dateString: string) {
        const _datePipe = this.datePipe;
        if (dateString != null) return _datePipe.transform(dateString, "M/d/yyyy");
    }

    addUserFromEmail() {
        this.waterAccountUserService
            .waterAccountsWaterAccountIDInvitingUserInvitingUserIDPost(this.waterAccount.WaterAccountID, this.currentUser.UserID, this.addUserByEmailDto)
            .subscribe((response) => {
                this.users.push(response);
                this.addUserByEmailDto = new AddUserByEmailDto();
                this.addUserByEmailDto.WaterAccountRoleID = 0;
                this.alertService.pushAlert(new Alert("User successfully added! An email has been sent to notify them.", AlertContext.Success, true));
            });
    }

    checkUserIsPending(user: WaterAccountUserMinimalDto) {
        return user.User.RoleID == RoleEnum.PendingLogin;
    }

    isCurrentUserWaterAccountOwner() {
        const hasPermission = this.authenticationService.hasOverallPermission(
            this.currentUser,
            PermissionEnum.WaterAccountUserRights,
            RightsEnum.Create,
            this.waterAccount.Geography.GeographyID,
            this.waterAccount.WaterAccountID
        );

        return hasPermission;
    }
}
