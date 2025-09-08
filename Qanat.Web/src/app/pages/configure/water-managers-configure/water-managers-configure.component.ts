import { ChangeDetectorRef, Component, OnInit, ViewEncapsulation } from "@angular/core";
import { NgForm, FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { routeParams } from "src/app/app.routes";
import { combineLatest, Observable, switchMap, tap } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyForAdminEditorsDto } from "src/app/shared/generated/model/geography-for-admin-editors-dto";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { NgSelectModule } from "@ng-select/ng-select";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { GeographyWaterManagerDto } from "src/app/shared/generated/model/geography-water-manager-dto";
@Component({
    selector: "water-managers-configure",
    templateUrl: "./water-managers-configure.component.html",
    styleUrls: ["./water-managers-configure.component.scss"],
    imports: [PageHeaderComponent, LoadingDirective, AsyncPipe, AlertDisplayComponent, FormsModule, RouterLink, AsyncPipe, NgSelectModule],
})
export class WaterManagersConfigureComponent implements OnInit {
    public initialData$: Observable<WaterManagersConfigureInitialData>;
    public isLoading: boolean;

    public geography: GeographyForAdminEditorsDto;
    public allUsers: GeographyWaterManagerDto[];
    public selectedUser: GeographyWaterManagerDto;
    public filteredUsers: GeographyWaterManagerDto[];
    public usersToSave: GeographyWaterManagerDto[];
    public modelOnLoad: string;

    public isLoadingSubmit: boolean = false;
    public customRichTextTypeID = CustomRichTextTypeEnum.ConfigureWaterManagers;

    constructor(
        public authenticationService: AuthenticationService,
        private userService: UserService,
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private geographyService: GeographyService,
        private cdr: ChangeDetectorRef
    ) {}

    public ngOnInit(): void {
        this.initialData$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return combineLatest({
                    users: this.userService.listNormalUser(),
                    geography: this.geographyService.getByGeographyNameForAdminEditorGeography(geographyName),
                });
            }),
            tap(({ users, geography }) => {
                this.geography = geography;
                this.allUsers = users.map(
                    (user) =>
                        new GeographyWaterManagerDto({
                            GeographyID: geography.GeographyID,
                            UserID: user.UserID,
                            UserFullName: user.FullName,
                            ReceivesNotifications: user.GeographyUser.find((x) => x.GeographyID == geography.GeographyID)?.ReceivesNotifications ?? false,
                        })
                );

                const waterManagerUserIDs = geography.WaterManagers.map((user) => user.UserID);
                this.usersToSave = this.allUsers.filter((x) => waterManagerUserIDs.includes(x.UserID));

                this.filteredUsers = this.allUsers;
                this.filterUsers();

                this.modelOnLoad = JSON.stringify(this.usersToSave);
                this.isLoading = false;
                this.cdr.detectChanges();
            })
        );
    }

    private filterUsers(): void {
        this.filteredUsers = [...this.allUsers.filter((x) => !this.usersToSave.map((y) => y.UserID).includes(x.UserID))];
    }

    public onSubmit(form: NgForm): void {
        this.geographyService.editGeographyWaterManagersGeography(this.geography.GeographyID, this.usersToSave).subscribe(() => {
            this.modelOnLoad = null;
            this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                this.alertService.pushAlert(new Alert(`Successfully saved Water Managers for the ${this.geography.GeographyDisplayName} geography.`, AlertContext.Success));
            });
        });
    }

    public removeUser(user: UserDto): void {
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
    public users: UserDto[];
}
