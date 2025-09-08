import { Component, OnInit } from "@angular/core";
import { Observable, forkJoin } from "rxjs";
import { map, switchMap, tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UserDto, UserGeographySummaryDto } from "src/app/shared/generated/model/models";
import { AsyncPipe, CommonModule } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FormsModule } from "@angular/forms";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { UserService } from "src/app/shared/generated/api/user.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { GeographyActivitiesComponent } from "./geography-activities/geography-activities.component";

@Component({
    selector: "water-dashboard-activity-center",
    templateUrl: "./water-dashboard-activity-center.component.html",
    styleUrls: ["./water-dashboard-activity-center.component.scss"],
    imports: [
        PageHeaderComponent,
        LoadingDirective,
        AsyncPipe,
        FormsModule,
        CommonModule,
        LoadingDirective,
        WaterDashboardNavComponent,
        AlertDisplayComponent,
        GeographyActivitiesComponent,
    ],
})
export class WaterDashboardActivityCenterComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;
    public isSystemAdmin: boolean;

    public currentUserGeographySummaries$: Observable<UserGeographySummaryDto[]>;

    public richTextID: number = CustomRichTextTypeEnum.WaterDashboardActivityCenter;
    public isLoading: boolean = true;

    constructor(
        private authenticationService: AuthenticationService,
        private userService: UserService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
                this.isSystemAdmin = AuthorizationHelper.isSystemAdministrator(user);
            })
        );

        this.currentUserGeographySummaries$ = this.currentUser$.pipe(
            switchMap(() => {
                return forkJoin([this.userService.getGeographySummaryUser(), this.geographyService.listForCurrentUserGeography()]);
            }),
            map(([geographySummaries, geographies]) => {
                geographies.forEach((geography) => {
                    if (
                        geographySummaries.findIndex((x) => x.GeographyID == geography.GeographyID) < 0 &&
                        (this.isSystemAdmin || AuthorizationHelper.hasGeographyFlag(geography.GeographyID, FlagEnum.HasManagerDashboard, this.currentUser))
                    ) {
                        geographySummaries.push({
                            GeographyID: geography.GeographyID,
                            GeographyName: geography.GeographyName,
                            GeographyDisplayName: geography.GeographyDisplayName,
                            AllowWaterMeasurementSelfReporting: geography.AllowWaterMeasurementSelfReporting,
                            AllowCoverCropSelfReporting: geography.AllowCoverCropSelfReporting,
                            AllowFallowSelfReporting: geography.AllowFallowSelfReporting,
                            WellRegistryEnabled: geography.GeographyConfiguration.WellRegistryEnabled,
                            AllowLandownersToRequestAccountChanges: geography.AllowLandownersToRequestAccountChanges,
                            IsGeographyWaterManager: true,
                        });
                    }
                });

                return geographySummaries;
            }),
            tap(() => (this.isLoading = false))
        );
    }
}
