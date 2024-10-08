import { Component, OnInit } from "@angular/core";
import { NgForm, FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { UserDetailedDto } from "src/app/shared/generated/model/user-detailed-dto";
import { routeParams } from "src/app/app.routes";
import { forkJoin } from "rxjs";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AsyncPipe, NgFor, NgIf } from "@angular/common";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
@Component({
    selector: "geography-water-managers--edit",
    templateUrl: "./geography-water-managers-edit.component.html",
    styleUrls: ["./geography-water-managers-edit.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, FormsModule, SelectDropDownModule, NgFor, NgIf, RouterLink, AsyncPipe],
})
export class GeographyWaterManagersEditComponent implements OnInit {
    public isLoadingSubmit: boolean = false;
    public customRichTextTypeID = CustomRichTextTypeEnum.ConfigureWaterManagers;
    public userDropdownConfig = {
        search: true,
        height: "320px",
        placeholder: "Select a user from the list of users",
        displayKey: "FullName",
        searchOnKey: "FullName",
    };

    public geography: GeographyDto;
    public allUsers: UserDetailedDto[];
    public selectedUser: UserDetailedDto;
    public filteredUsers: UserDetailedDto[];

    public usersToSave: UserDetailedDto[] = [];

    constructor(
        public authenticationService: AuthenticationService,
        private userService: UserService,
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        const geographySlug = this.route.snapshot.paramMap.get(routeParams.geographyName);
        forkJoin({
            allUsers: this.userService.usersNormalUsersGet(),
            geography: this.geographyService.publicGeographyNameGeographyNameGet(geographySlug),
        }).subscribe(({ allUsers, geography }) => {
            this.allUsers = allUsers;
            this.filteredUsers = allUsers;

            this.usersToSave = geography.WaterManagers.length > 0 ? [...geography.WaterManagers] : [];

            this.geography = geography;
            this.filterUsers();
        });
    }

    filterUsers(): void {
        this.filteredUsers = [...this.allUsers.filter((x) => !this.usersToSave.map((y) => y.UserID).includes(x.UserID))];
    }

    onSubmit(form: NgForm): void {
        this.geographyService.geographiesGeographyIDEditWaterManagersPut(this.geography.GeographyID, this.usersToSave).subscribe(() => {
            this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                this.alertService.pushAlert(new Alert(`Successfully saved Water Managers for the ${this.geography.GeographyName} geography.`, AlertContext.Success));
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
}
