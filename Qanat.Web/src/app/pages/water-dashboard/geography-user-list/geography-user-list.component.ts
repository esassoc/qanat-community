import { AsyncPipe, CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule, NgModel } from "@angular/forms";
import { Router } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { combineLatest, Observable, of, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyUserService } from "src/app/shared/generated/api/geography-user.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { GeographyUserDto } from "src/app/shared/generated/model/geography-user-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "geography-user-list",
    imports: [
        LoadingDirective,
        WaterDashboardNavComponent,
        PageHeaderComponent,
        AsyncPipe,
        AlertDisplayComponent,
        QanatGridComponent,
        QanatGridHeaderComponent,
        CommonModule,
        FormsModule,
        ModelNameTagComponent,
        NgSelectModule,
    ],
    templateUrl: "./geography-user-list.component.html",
    styleUrl: "./geography-user-list.component.scss",
})
export class GeographyUserListComponent implements OnInit {
    public compareGeography = GeographyHelper.compareGeography;
    public currentUser$: Observable<UserDto>;

    public currentGeography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;
    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;

    public geographyUsers$: Observable<GeographyUserDto[]>;
    public columnDefs$: Observable<ColDef<GeographyUserDto>[]>;
    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public currentUserHasManagerPermissionsForSelectedGeography: boolean = false;
    public isLoading: boolean = true;
    public firstLoad: boolean = true;
    public richTextID: number = CustomRichTextTypeEnum.GeographyUserList;

    constructor(
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private geographyUserService: GeographyUserService,
        private utilityFunctionsService: UtilityFunctionsService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser();

        this.currentUserGeographies$ = this.currentUser$.pipe(
            switchMap(() => {
                return this.geographyService.listForCurrentUserGeography();
            })
        );

        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((currentGeography) => {
                this.currentGeography = currentGeography;
            })
        );

        this.geographyUsers$ = this.currentGeography$.pipe(
            tap(() => {
                this.isLoading = true;

                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap((currentGeography) => {
                return this.geographyUserService.listGeographyUser(currentGeography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
                this.firstLoad = false;

                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", false);
                    setTimeout(() => {
                        this.gridApi.sizeColumnsToFit();
                    }, 1);
                }
            })
        );

        this.columnDefs$ = combineLatest({ currentGeography: this.currentGeography$, currentUser: this.currentUser$ }).pipe(
            switchMap((result) => {
                let columnDefs = this.buildColumnDefs(result.currentGeography, result.currentUser);
                return of(columnDefs);
            })
        );
    }

    public onGeographySelected(currentUser: UserDto, selectedGeography: GeographyMinimalDto) {
        if (!currentUser || !selectedGeography) {
            this.currentUserHasManagerPermissionsForSelectedGeography = false;
            return;
        }

        this.currentUserHasManagerPermissionsForSelectedGeography =
            AuthorizationHelper.hasFlag(FlagEnum.IsSystemAdmin, currentUser) ||
            AuthorizationHelper.hasGeographyFlag(FlagEnum.HasManagerDashboard, selectedGeography.GeographyID, currentUser);

        this.currentGeographyService.setCurrentGeography(selectedGeography);

        if (!this.currentUserHasManagerPermissionsForSelectedGeography) {
            this.router.navigate(["/water-dashboard/water-account-list"]);
        }
    }

    gridReady($event: GridReadyEvent<any, any>) {
        this.gridApi = $event.api;

        setTimeout(() => {
            this.gridApi.sizeColumnsToFit();
        }, 1);
    }

    buildColumnDefs(currentGeography: GeographyMinimalDto, currentUser: UserDto): any {
        let nameCol = AuthorizationHelper.hasFlag(FlagEnum.IsSystemAdmin, currentUser)
            ? this.utilityFunctionsService.createLinkColumnDef("User", "User.FullName", "User.UserID", {
                  InRouterLink: "/platform-admin/",
                  ValueGetter: (params) => {
                      return { LinkValue: `/users/${params.data.User.UserID}`, LinkDisplay: params.data.User.FullName };
                  },
              })
            : this.utilityFunctionsService.createLinkColumnDef("User", "User.FullName", "User.UserID", {
                  InRouterLink: "/geographies/",
                  ValueGetter: (params) => {
                      return { LinkValue: `${currentGeography.GeographyName.toLowerCase()}/users/${params.data.User.UserID}`, LinkDisplay: params.data.User.FullName };
                  },
              });

        let columnDefs: ColDef<GeographyUserDto>[] = [
            nameCol,
            this.utilityFunctionsService.createBasicColumnDef("Email", "User.Email"),
            this.utilityFunctionsService.createDecimalColumnDef("# of Water Accounts", "WaterAccountCount", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Water Accounts", "WaterAccounts", "WaterAccountID", "WaterAccountNumber", {
                InRouterLink: "/water-accounts",
                MaxWidth: 300,
            }),

            this.utilityFunctionsService.createMultiLinkColumnDef("Water Account Pin #", "WaterAccounts", "WaterAccountID", "WaterAccountPIN", {
                InRouterLink: "/water-accounts",
                MaxWidth: 300,
            }),
            this.utilityFunctionsService.createDecimalColumnDef("# of Well Registrations", "WellRegistrationCount", { MaxDecimalPlacesToDisplay: 0 }),
        ];

        return columnDefs;
    }
}
