import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { NgForm, FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { UserDetailedDto } from "src/app/shared/generated/model/user-detailed-dto";
import { routeParams } from "src/app/app.routes";
import { combineLatest, Observable, switchMap, tap } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AsyncPipe, NgFor, NgIf } from "@angular/common";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyForAdminEditorsDto } from "src/app/shared/generated/model/geography-for-admin-editors-dto";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
@Component({
    selector: "water-managers-configure",
    templateUrl: "./water-managers-configure.component.html",
    styleUrls: ["./water-managers-configure.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, LoadingDirective, AsyncPipe, AlertDisplayComponent, FormsModule, SelectDropDownModule, NgFor, NgIf, RouterLink, AsyncPipe],
})
export class WaterManagersConfigureComponent implements OnInit {
    public initialData$: Observable<WaterManagersConfigureInitialData>;
    public isLoading: boolean;

    public geography: GeographyForAdminEditorsDto;
    public allUsers: UserDetailedDto[];
    public selectedUser: UserDetailedDto;
    public filteredUsers: UserDetailedDto[];
    public usersToSave: UserDetailedDto[] = [];
    public modelOnLoad: string;

    public isLoadingSubmit: boolean = false;
    public customRichTextTypeID = CustomRichTextTypeEnum.ConfigureWaterManagers;
    public userDropdownConfig = {
        search: true,
        height: "320px",
        placeholder: "Select a user from the list of users",
        displayKey: "FullName",
        searchOnKey: "FullName",
    };

    constructor(
        public authenticationService: AuthenticationService,
        private userService: UserService,
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private geographyService: GeographyService
    ) {}

    public ngOnInit(): void {
        this.initialData$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return combineLatest({
                    users: this.userService.usersNormalUsersGet(),
                    geography: this.geographyService.geographiesGeographyNameGeographyNameForAdminEditorGet(geographyName),
                });
            }),
            tap(({ users, geography }) => {
                this.allUsers = users;
                this.filteredUsers = users;
                this.usersToSave = geography.WaterManagers.length > 0 ? [...geography.WaterManagers] : [];
                this.modelOnLoad = JSON.stringify(this.usersToSave);
                this.geography = geography;
                this.filterUsers();
                this.isLoading = false;
            })
        );
    }

    private filterUsers(): void {
        this.filteredUsers = [...this.allUsers.filter((x) => !this.usersToSave.map((y) => y.UserID).includes(x.UserID))];
    }

    public onSubmit(form: NgForm): void {
        this.geographyService.geographiesGeographyIDWaterManagersPut(this.geography.GeographyID, this.usersToSave).subscribe(() => {
            this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                this.alertService.pushAlert(new Alert(`Successfully saved Water Managers for the ${this.geography.GeographyDisplayName} geography.`, AlertContext.Success));
            });
        });
    }

    public removeUser(user: UserDetailedDto): void {
        const index = this.usersToSave.indexOf(user);
        if (index > -1) {
            this.usersToSave.splice(index, 1);
        }
        this.filterUsers();
    }

    public addUser(): void {
        // this select dropdown is really annoying...
        if (Object.keys(this.selectedUser).length != 0 && !this.usersToSave.find((x) => x.UserID == this.selectedUser.UserID)) {
            this.usersToSave.push(this.selectedUser);
        }

        this.selectedUser = null;
        this.filterUsers();
    }

    public canExit() {
        if (!this.modelOnLoad) {
            return true;
        }

        return JSON.stringify(this.usersToSave) == this.modelOnLoad;
    }
}

class WaterManagersConfigureInitialData {
    public geography;
    public users: UserDetailedDto[];
}
