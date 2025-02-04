import { AsyncPipe, DecimalPipe, NgFor, NgIf } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, Subscription, combineLatest, forkJoin, map, switchMap, tap } from "rxjs";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterMeasurementSelfReportService } from "src/app/shared/generated/api/water-measurement-self-report.service";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import {
    AllocationPlanMinimalDto,
    UserDto,
    WaterAccountDto,
    WaterMeasurementSelfReportCreateDto,
    WaterMeasurementSelfReportLineItemSimpleDto,
    WaterMeasurementSelfReportSimpleDto,
    WaterMeasurementTypeSimpleDto,
} from "src/app/shared/generated/model/models";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { AuthenticationService } from "src/app/shared/services/authentication.service";

@Component({
    selector: "water-measurement-self-report-list",
    standalone: true,
    imports: [
        AsyncPipe,
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        ReportingPeriodSelectComponent,
        NgFor,
        LoadingDirective,
        ButtonLoadingDirective,
        ExpandCollapseDirective,
        DecimalPipe,
        SumPipe,
    ],
    templateUrl: "./water-measurement-self-report-list.component.html",
    styleUrl: "./water-measurement-self-report-list.component.scss",
})
export class WaterMeasurementSelfReportListComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;
    public currentUser: UserDto;

    public currentUserCanCreateSelfReport: boolean = false;

    private waterAccountID: number;
    public waterAccount$: Observable<WaterAccountDto>;
    public geographyID: number;
    public pageIsLoading: boolean = true;
    public allocationPlans: AllocationPlanMinimalDto[];

    public waterMeasurementTypes: WaterMeasurementTypeSimpleDto[];

    public selfReports$: Observable<SelfReportWaterMeasurementTypeViewModel[]>;

    public selectedYear: number;
    public selectedYear$: Observable<number>;

    public selfReportsByWaterMeasurementType: SelfReportWaterMeasurementTypeViewModel[] = [];

    public subscriptions: Subscription[] = [];

    constructor(
        private waterAccountService: WaterAccountService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private waterMeasurementSelfReportService: WaterMeasurementSelfReportService,
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(tap((currentUser) => (this.currentUser = currentUser)));
        this.waterAccount$ = this.route.paramMap.pipe(
            map((paramMap) => parseInt(paramMap.get("waterAccountID"))),
            switchMap((waterAccountID) =>
                this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID).pipe(
                    tap((waterAccount) => {
                        this.waterAccountID = waterAccountID;
                        this.geographyID = waterAccount.Geography.GeographyID;

                        let query = this.route.snapshot.queryParamMap;
                        this.selectedYear = parseInt(query.get("reportingPeriod")) || null;

                        if (!this.selectedYear) {
                            this.selectedYear = waterAccount.Geography.DefaultDisplayYear;
                        }
                    })
                )
            )
        );

        this.selectedYear$ = this.route.queryParamMap.pipe(
            map((queryParamMap) => parseInt(queryParamMap.get("reportingPeriod")) || null),
            tap((selectedYear) => {
                if (selectedYear) {
                    this.selectedYear = selectedYear;
                }
            })
        );

        this.selfReports$ = combineLatest([this.waterAccount$, this.selectedYear$]).pipe(
            tap(() => (this.pageIsLoading = true)),
            switchMap(() =>
                forkJoin({
                    waterMeasurementTypes: this.waterMeasurementTypeService.geographiesGeographyIDWaterMeasurementTypesSelfReportableGet(this.geographyID),
                    allocationPlans: this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(this.waterAccountID),
                    selfReports: this.waterMeasurementSelfReportService.geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsReportingPeriodsYearGet(
                        this.geographyID,
                        this.waterAccountID,
                        this.selectedYear
                    ),
                }).pipe(
                    tap(({ waterMeasurementTypes, allocationPlans, selfReports }) => {
                        this.waterMeasurementTypes = waterMeasurementTypes.filter((wmt) => wmt.IsSelfReportable);
                        this.allocationPlans = allocationPlans;

                        this.currentUserCanCreateSelfReport = AuthorizationHelper.hasWaterAccountRolePermission(
                            this.geographyID,
                            this.waterAccountID,
                            PermissionEnum.WaterAccountRights,
                            RightsEnum.Create,
                            this.currentUser
                        );

                        this.pageIsLoading = false;
                    }),
                    map(({ selfReports }) => {
                        return this.waterMeasurementTypes.map((wmt) => ({
                            WaterMeasurementType: wmt,
                            SelfReport: selfReports.find((sr) => sr.WaterMeasurementTypeID === wmt.WaterMeasurementTypeID),
                            PostRequestInProgress: false,
                            LineItemRequestInProgress: false,
                            SelfReportLineItems: null,
                        }));
                    })
                )
            )
        );
    }

    ngOnDestroy() {
        this.subscriptions.forEach((sub) => {
            if (sub && sub.unsubscribe) {
                sub.unsubscribe();
            }
        });
    }

    updateReportingPeriod(selectedYear: number) {
        this.selectedYear = selectedYear;
        this.router.navigate([], {
            relativeTo: this.route,
            queryParams: { reportingPeriod: selectedYear },
            queryParamsHandling: "merge",
        });
    }

    startSelfReport(selfReportByWaterMeasurementType: SelfReportWaterMeasurementTypeViewModel) {
        let selfReportCreateDto = new WaterMeasurementSelfReportCreateDto({
            WaterMeasurementTypeID: selfReportByWaterMeasurementType.WaterMeasurementType.WaterMeasurementTypeID,
            ReportingYear: this.selectedYear,
        });

        selfReportByWaterMeasurementType.PostRequestInProgress = true;

        let createRequest = this.waterMeasurementSelfReportService
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsPost(this.geographyID, this.waterAccountID, selfReportCreateDto)
            .subscribe((selfReport) => {
                this.router.navigate([selfReport.WaterMeasurementSelfReportID], { relativeTo: this.route });
            });

        this.subscriptions.push(createRequest);
    }

    getReportLineItems(selfReportByWaterMeasurementType: SelfReportWaterMeasurementTypeViewModel) {
        selfReportByWaterMeasurementType.LineItemRequestInProgress = true;

        let getLineItemsRequest = this.waterMeasurementSelfReportService
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDGet(
                this.geographyID,
                this.waterAccountID,
                selfReportByWaterMeasurementType.SelfReport.WaterMeasurementSelfReportID
            )
            .subscribe((selfReportDto) => {
                selfReportByWaterMeasurementType.SelfReportLineItems = selfReportDto.LineItems;
                selfReportByWaterMeasurementType.LineItemRequestInProgress = false;
            });

        this.subscriptions.push(getLineItemsRequest);
    }
}

class SelfReportWaterMeasurementTypeViewModel {
    public WaterMeasurementType: WaterMeasurementTypeSimpleDto;
    public SelfReport: WaterMeasurementSelfReportSimpleDto;
    public SelfReportLineItems: WaterMeasurementSelfReportLineItemSimpleDto[];
    public PostRequestInProgress: boolean;
    public LineItemRequestInProgress: boolean;
}
