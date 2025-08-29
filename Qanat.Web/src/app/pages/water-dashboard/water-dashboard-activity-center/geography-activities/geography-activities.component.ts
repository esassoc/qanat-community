import { AsyncPipe, DatePipe, DecimalPipe } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { Params, RouterLink } from "@angular/router";
import { combineLatest, map, Observable, of, switchMap, tap } from "rxjs";
import { GeographyLogoComponent } from "src/app/shared/components/geography-logo/geography-logo.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { WaterAccountCoverCropSummaryService } from "src/app/shared/generated/api/water-account-cover-crop-summary.service";
import { WaterAccountFallowSummaryService } from "src/app/shared/generated/api/water-account-fallow-summary.service";
import { ReportingPeriodDto, SelfReportSummaryDto, UserGeographySummaryDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "geography-activities",
    imports: [GeographyLogoComponent, IconComponent, RouterLink, ExpandCollapseDirective, DecimalPipe, AsyncPipe],
    templateUrl: "./geography-activities.component.html",
    styleUrl: "./geography-activities.component.scss",
})
export class GeographyActivitiesComponent implements OnInit {
    @Input() geographySummary: UserGeographySummaryDto;
    @Input() isSystemAdmin: boolean;

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public coverCropReportingPeriodsDue: ReportingPeriodDto[] = [];
    public fallowingReportingPeriodsDue: ReportingPeriodDto[] = [];

    public notifications$: Observable<ActivityCenterNotificiation[]>;

    public coverCropSelfReportSummaries$: Observable<SelfReportSummaryDto[]>;
    public fallowSelfReportSummaries$: Observable<SelfReportSummaryDto[]>;

    public constructor(
        private reportingPeriodService: ReportingPeriodService,
        private waterAccountCoverCropSummaryService: WaterAccountCoverCropSummaryService,
        private waterAccountFallowSummaryService: WaterAccountFallowSummaryService,
        private datePipe: DatePipe
    ) {}

    ngOnInit(): void {
        this.reportingPeriods$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.geographySummary.GeographyID).pipe(
            tap((reportingPeriods) => {
                let currentDate = new Date();
                this.coverCropReportingPeriodsDue = reportingPeriods.filter((reportingPeriod) => {
                    return (
                        reportingPeriod.CoverCropSelfReportReadyForAccountHolders &&
                        reportingPeriod.CoverCropSelfReportStartDate &&
                        new Date(reportingPeriod.CoverCropSelfReportStartDate) < currentDate &&
                        reportingPeriod.CoverCropSelfReportEndDate &&
                        new Date(reportingPeriod.CoverCropSelfReportEndDate) > currentDate
                    );
                });

                this.fallowingReportingPeriodsDue = reportingPeriods.filter((reportingPeriod) => {
                    return (
                        reportingPeriod.FallowSelfReportReadyForAccountHolders &&
                        reportingPeriod.FallowSelfReportStartDate &&
                        new Date(reportingPeriod.FallowSelfReportStartDate) < currentDate &&
                        reportingPeriod.FallowSelfReportEndDate &&
                        new Date(reportingPeriod.FallowSelfReportEndDate) > currentDate
                    );
                });
            })
        );

        this.notifications$ = this.reportingPeriods$.pipe(
            switchMap(() => {
                let notifications: ActivityCenterNotificiation[] = [];
                this.coverCropReportingPeriodsDue.forEach((reportingPeriod) => {
                    const notification = new ActivityCenterNotificiation();
                    notification.Message = `Cover Crop Self-Reporting is due <b>${this.datePipe.transform(reportingPeriod.CoverCropSelfReportEndDate, "mediumDate")}</b> for Reporting Period ${reportingPeriod.Name}. `;
                    notification.LinkText = "Report now.";
                    notification.Link = `/cover-crop-self-reports`;
                    notification.LinkQueryParams = { geographyID: this.geographySummary.GeographyID, reportingPeriodID: reportingPeriod.ReportingPeriodID };
                    notification.SortDate = new Date(reportingPeriod.CoverCropSelfReportEndDate);
                    notifications.push(notification);
                });

                this.fallowingReportingPeriodsDue.forEach((reportingPeriod) => {
                    const notification = new ActivityCenterNotificiation();
                    notification.Message = `Fallow Self-Reporting is due <b>${this.datePipe.transform(reportingPeriod.FallowSelfReportEndDate, "mediumDate")}</b> for Reporting Period ${reportingPeriod.Name}. `;
                    notification.LinkText = "Report now.";
                    notification.Link = `/fallow-self-reports`;
                    notification.LinkQueryParams = { geographyID: this.geographySummary.GeographyID, reportingPeriodID: reportingPeriod.ReportingPeriodID };
                    notification.SortDate = new Date(reportingPeriod.FallowSelfReportEndDate);
                    notifications.push(notification);
                });

                notifications.sort((a, b) => a.SortDate.getTime() - b.SortDate.getTime());

                return of(notifications);
            })
        );

        this.coverCropSelfReportSummaries$ = this.waterAccountCoverCropSummaryService.listSummariesWaterAccountCoverCropSummary(this.geographySummary.GeographyID);
        this.fallowSelfReportSummaries$ = this.waterAccountFallowSummaryService.listSummariesWaterAccountFallowSummary(this.geographySummary.GeographyID);
    }
}

export class ActivityCenterNotificiation {
    public Message: string;
    public LinkText: string;
    public Link: string;
    public LinkQueryParams: Params;
    public SortDate: Date;
}
