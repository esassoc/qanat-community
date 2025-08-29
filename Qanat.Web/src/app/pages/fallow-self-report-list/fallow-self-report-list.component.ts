import { Component, OnDestroy, OnInit } from "@angular/core";
import { BehaviorSubject, combineLatest, filter, finalize, map, Observable, startWith, Subscription, switchMap, tap } from "rxjs";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyMinimalDto, ReportingPeriodDto, UserDto, WaterAccountFallowStatusDto } from "src/app/shared/generated/model/models";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { IconComponent } from "../../shared/components/icon/icon.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { WaterAccountFallowStatusService } from "src/app/shared/generated/api/water-account-fallow-status.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { WaterAccountTitleComponent } from "../../shared/components/water-account/water-account-title/water-account-title.component";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";

@Component({
    selector: "fallow-self-report-list",
    imports: [
        PageHeaderComponent,
        CustomRichTextComponent,
        IconComponent,
        NgSelectModule,
        CommonModule,
        FormsModule,
        LoadingDirective,
        AlertDisplayComponent,
        WaterAccountTitleComponent,
        RouterLink,
        ExpandCollapseDirective,
        NoteComponent,
    ],
    templateUrl: "./fallow-self-report-list.component.html",
    styleUrl: "./fallow-self-report-list.component.scss",
})
export class FallowSelfReportListComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;
    public currentUser: UserDto | null = null;
    public currentUserHasGeographiesThatAllowFallowSelfReporting: boolean = false;

    public geographies$: Observable<GeographyMinimalDto[]>;
    public geographiesLoading: boolean = true;
    public selectedGeography: GeographyMinimalDto;
    public compareGeography = GeographyHelper.compareGeography;
    public geographySelected$: BehaviorSubject<GeographyMinimalDto> = new BehaviorSubject<GeographyMinimalDto>(null);

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public selectedReportingPeriod: ReportingPeriodDto;
    public reportingPeriodSelected$: BehaviorSubject<ReportingPeriodDto> = new BehaviorSubject<ReportingPeriodDto>(null);

    public fallowSelfReports$: Observable<WaterAccountFallowStatusDto[]>;
    public isLoading: boolean = true;

    private subscriptions: Subscription[] = [];

    public richTextTypeID = CustomRichTextTypeEnum.FallowSelfReportingList;
    public SelfReportStatusEnum = SelfReportStatusEnum;

    public constructor(
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private fallowSelfReportingService: WaterAccountFallowStatusService,
        private router: Router,
        private activatedRoute: ActivatedRoute
    ) {}

    public ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
            })
        );

        this.geographies$ = this.currentUser$.pipe(
            switchMap((user) => {
                return this.geographyService.listForCurrentUserGeography().pipe(
                    map((geographies) => {
                        return geographies.filter((g) => g.AllowFallowSelfReporting);
                    }),
                    tap((geographies) => {
                        this.currentUserHasGeographiesThatAllowFallowSelfReporting = geographies.length > 0;
                        this.geographiesLoading = false;
                    })
                );
            }),
            filter((geographies) => geographies.length > 0),
            tap((geographies) => {
                let geographyID = this.activatedRoute.snapshot.queryParamMap.get("geographyID");
                if (geographyID) {
                    const foundGeography = geographies.find((g) => g.GeographyID === +geographyID);
                    if (foundGeography) {
                        this.selectedGeography = foundGeography;
                    }
                } else {
                    if (this.currentGeographyService.currentGeography) {
                        let currentGeography = this.currentGeographyService.currentGeography;
                        if (geographies.some((g) => g.GeographyID === currentGeography.GeographyID)) {
                            this.selectedGeography = currentGeography;
                        } else {
                            this.selectedGeography = geographies[0];
                        }
                    } else {
                        this.selectedGeography = geographies[0];
                    }
                }

                this.currentGeographyService.setCurrentGeography(this.selectedGeography);
                this.geographySelected$.next(this.selectedGeography);
            })
        );

        this.reportingPeriods$ = this.geographySelected$.pipe(
            filter((geography) => geography !== null),
            switchMap((geography) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            filter((reportingPeriods) => reportingPeriods.length > 0),
            map((reportingPeriods) => {
                let currentUserIsAdminOrWaterManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(this.currentUser, this.selectedGeography.GeographyID);
                if (currentUserIsAdminOrWaterManager) {
                    return reportingPeriods;
                } else {
                    return reportingPeriods.filter((reportingPeriod) => {
                        return reportingPeriod.FallowSelfReportReadyForAccountHolders;
                    });
                }
            }),
            tap((reportingPeriods) => {
                if (reportingPeriods.length === 0) {
                    this.selectedReportingPeriod = null;
                    this.reportingPeriodSelected$.next(null);
                    this.isLoading = false;
                    return;
                }

                this.selectedReportingPeriod = reportingPeriods[0];

                let reportingPeriodID = this.activatedRoute.snapshot.queryParamMap.get("reportingPeriodID");
                if (reportingPeriodID) {
                    const foundReportingPeriod = reportingPeriods.find((rp) => rp.ReportingPeriodID === +reportingPeriodID);
                    if (foundReportingPeriod) {
                        this.selectedReportingPeriod = foundReportingPeriod;
                    }
                }

                this.reportingPeriodSelected$.next(this.selectedReportingPeriod);
            })
        );

        this.fallowSelfReports$ = combineLatest({ geography: this.geographySelected$, reportingPeriod: this.reportingPeriodSelected$ }).pipe(
            filter(({ geography, reportingPeriod }) => geography !== null && reportingPeriod !== null),
            switchMap(({ geography, reportingPeriod }) => {
                return this.fetchFallowReports(geography, reportingPeriod);
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
        this.subscriptions = [];
    }

    private fetchFallowReports(geography: GeographyMinimalDto, reportingPeriod: ReportingPeriodDto): Observable<WaterAccountFallowStatusDto[]> {
        this.isLoading = true;

        return this.fallowSelfReportingService.listWaterAccountFallowStatus(geography.GeographyID, reportingPeriod.ReportingPeriodID).pipe(
            startWith([]),
            finalize(() => {
                this.isLoading = false;
            })
        );
    }

    public onGeographySelected(selectedGeography: GeographyMinimalDto) {
        if (!this.currentUser) {
            return;
        }

        this.selectedGeography = selectedGeography;
        this.currentGeographyService.setCurrentGeography(selectedGeography);
        this.geographySelected$.next(selectedGeography);
    }

    public onReportingPeriodSelected(selectedReportingPeriod: ReportingPeriodDto) {
        this.selectedReportingPeriod = selectedReportingPeriod;
        this.reportingPeriodSelected$.next(selectedReportingPeriod);
    }

    public createFallowSelfReport(selfReport: WaterAccountFallowStatusDto) {
        let createSelfReportSubscription = this.fallowSelfReportingService
            .createWaterAccountFallowStatus(this.selectedGeography.GeographyID, this.selectedReportingPeriod.ReportingPeriodID, selfReport.WaterAccount.WaterAccountID)
            .subscribe((result) => {
                this.router.navigate([
                    "/geographies",
                    this.selectedGeography.GeographyID,
                    "reporting-periods",
                    this.selectedReportingPeriod.ReportingPeriodID,
                    "fallow-self-reports",
                    result.WaterAccountFallowStatusID,
                ]);
            });

        this.subscriptions.push(createSelfReportSubscription);
    }
}
