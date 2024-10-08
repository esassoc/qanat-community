import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from "@angular/core";
import { UserService } from "src/app/shared/generated/api/user.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ColDef } from "ag-grid-community";
import { AgGridAngular } from "ag-grid-angular";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { forkJoin } from "rxjs";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";

declare let $: any;

@Component({
    selector: "qanat-user-list",
    templateUrl: "./user-list.component.html",
    styleUrls: ["./user-list.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, QanatGridComponent],
})
export class UserListComponent implements OnInit, OnDestroy {
    @ViewChild("usersGrid") usersGrid: AgGridAngular;
    @ViewChild("unassignedUsersGrid") unassignedUsersGrid: AgGridAngular;

    private currentUser: UserDto;

    public users: UserDto[];
    public pendingUsers: UserDto[];
    public unassignedUsers: UserDto[];
    public columnDefs: ColDef[];
    public columnDefsUnassigned: ColDef[];
    public columnDefsPending: ColDef[];
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;
    frameworkComponents: any;

    constructor(
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private utilityFunctionsService: UtilityFunctionsService,
        private userService: UserService
    ) {}

    ngOnInit() {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;

            forkJoin({
                users: this.userService.usersGet(),
                pendingUsers: this.userService.pendingUsersGet(),
            }).subscribe(({ users, pendingUsers }) => {
                this.users = users;
                this.pendingUsers = pendingUsers;
                this.unassignedUsers = users.filter((u) => {
                    return u.Role.RoleID === RoleEnum.NoAccess && u.IsActive;
                });
            });

            this.createColumnDefs();
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Name", "FullName", "UserID"),
            { headerName: "Email", field: "Email" },
            this.utilityFunctionsService.createBasicColumnDef("System Role", "Role.RoleDisplayName", { CustomDropdownFilterField: "Role.RoleDisplayName" }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Associated Water Accounts", "NumberOfWaterAccounts", { DecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Associated Geography Roles", "", {
                ValueGetter: (params) => this.utilityFunctionsService.customDecimalValueGetter(this.getNumberOfGeographiesManaged(params.data.GeographyFlags), 0),
            }),
            this.utilityFunctionsService.createBasicColumnDef("Receives System Communications?", "", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.ReceiveSupportEmails, false),
                CustomDropdownFilterField: "ReceiveSupportEmails",
            }),
            this.utilityFunctionsService.createDateColumnDef("Create Date", "CreateDate", "M/d/yyyy"),
        ];

        this.columnDefsUnassigned = this.columnDefs.slice(1);

        this.columnDefsPending = [
            { headerName: "Email", field: "Email" },
            this.utilityFunctionsService.createDecimalColumnDef("# of Associated Water Accounts", "NumberOfWaterAccounts", { DecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDateColumnDef("Last Email Sent", "CreateDate", "M/d/yyyy"),
        ];
    }
    // todo: rights
    isUserAdmin(): boolean {
        return this.currentUser.Role.RoleID == RoleEnum.SystemAdmin;
    }

    private getNumberOfGeographiesManaged(dictionary) {
        if (!dictionary) return;

        let count = 0;
        const keys = Object.keys(dictionary);
        keys.forEach((key) => {
            if (dictionary[key].HasManagerDashboard) {
                count++;
            }
        });
        return count;
    }
}
