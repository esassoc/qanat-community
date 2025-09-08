import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { Observable, BehaviorSubject, Subscription, combineLatest, filter, finalize, map, startWith, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterAccountTitleComponent } from "src/app/shared/components/water-account/water-account-title/water-account-title.component";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { WaterAccountCoverCropStatusService } from "src/app/shared/generated/api/water-account-cover-crop-status.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { GeographyMinimalDto, ReportingPeriodDto, UserDto, WaterAccountCoverCropStatusDto, WaterAccountFallowStatusDto } from "src/app/shared/generated/model/models";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "cover-crop-self-report-list",
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
        SumPipe,
        ExpandCollapseDirective,
        NoteComponent,
    ],
    templateUrl: "./cover-crop-self-report-list.component.html",
    styleUrl: "./cover-crop-self-report-list.component.scss",
})
export class CoverCropSelfReportListComponent {
    public currentUser$: Observable<UserDto>;
    public currentUser: UserDto | null = null;

    public geographies$: Observable<GeographyMinimalDto[]>;
    public selectedGeography: GeographyMinimalDto;
    public compareGeography = GeographyHelper.compareGeography;
    public geographySelected$: BehaviorSubject<GeographyMinimalDto> = new BehaviorSubject<GeographyMinimalDto>(null);

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public selectedReportingPeriod: ReportingPeriodDto;
    public reportingPeriodSelected$: BehaviorSubject<ReportingPeriodDto> = new BehaviorSubject<ReportingPeriodDto>(null);

    public coverCropSelfReports$: Observable<WaterAccountCoverCropStatusDto[]>;
    public isLoading: boolean = true;
    public geographiesLoading: boolean = true;
    public currentUserHasGeographiesThatAllowCoverCropSelfReporting: boolean = false;

    private subscriptions: Subscription[] = [];

    public richTextTypeID = CustomRichTextTypeEnum.CoverCropSelfReportingList;
    public SelfReportStatusEnum = SelfReportStatusEnum;

    public constructor(
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private coverCropSelfReportingService: WaterAccountCoverCropStatusService,
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
                        return geographies.filter((g) => g.AllowCoverCropSelfReporting);
                    }),
                    tap((geographies) => {
                        this.currentUserHasGeographiesThatAllowCoverCropSelfReporting = geographies.length > 0;
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
                        return reportingPeriod.CoverCropSelfReportReadyForAccountHolders;
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

        this.coverCropSelfReports$ = combineLatest({ geography: this.geographySelected$, reportingPeriod: this.reportingPeriodSelected$ }).pipe(
            filter(({ geography, reportingPeriod }) => geography !== null && reportingPeriod !== null),
            switchMap(({ geography, reportingPeriod }) => {
                return this.fetchCoverCropReports(geography, reportingPeriod);
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
        this.subscriptions = [];
    }

    private fetchCoverCropReports(geography: GeographyMinimalDto, reportingPeriod: ReportingPeriodDto): Observable<WaterAccountFallowStatusDto[]> {
        this.isLoading = true;

        return this.coverCropSelfReportingService.listWaterAccountCoverCropStatus(geography.GeographyID, reportingPeriod.ReportingPeriodID).pipe(
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

    public createCoverCropSelfReport(selfReport: WaterAccountFallowStatusDto) {
        let createSelfReportSubscription = this.coverCropSelfReportingService
            .createWaterAccountCoverCropStatus(this.selectedGeography.GeographyID, this.selectedReportingPeriod.ReportingPeriodID, selfReport.WaterAccount.WaterAccountID)
            .subscribe((result) => {
                this.router.navigate([
                    "/geographies",
                    this.selectedGeography.GeographyID,
                    "reporting-periods",
                    this.selectedReportingPeriod.ReportingPeriodID,
                    "cover-crop-self-reports",
                    result.WaterAccountCoverCropStatusID,
                ]);
            });

        this.subscriptions.push(createSelfReportSubscription);
    }
}
