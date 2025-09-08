import { Component, OnDestroy } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CoverCropTabComponent } from "./cover-crop-tab/cover-crop-tab.component";
import { FallowTabComponent } from "./fallow-tab/fallow-tab.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { BehaviorSubject, combineLatest, filter, map, Observable, of, shareReplay, Subscription, switchMap, tap } from "rxjs";
import { ReportingPeriodDto } from "src/app/shared/generated/model/reporting-period-dto";
import { AsyncPipe } from "@angular/common";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterMeasurementsTabComponent } from "./water-measurements-tab/water-measurements-tab.component";
import { UsageLocationTypeDto } from "src/app/shared/generated/model/usage-location-type-dto";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";

@Component({
    selector: "self-reporting-configure",
    imports: [PageHeaderComponent, AlertDisplayComponent, LoadingDirective, AsyncPipe, CoverCropTabComponent, FallowTabComponent, WaterMeasurementsTabComponent],
    templateUrl: "./self-reporting-configure.component.html",
    styleUrl: "./self-reporting-configure.component.scss",
})
export class SelfReportingConfigureComponent implements OnDestroy {
    public isLoading: boolean = true;

    public richTextID: number = CustomRichTextTypeEnum.SelfReportingConfiguration;

    public activeTab: "Cover Crop" | "Fallowing" | "Surface Water" = "Cover Crop";

    public currentGeography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public refreshReportingPeriodsTrigger$: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public usageLocationTypes$: Observable<UsageLocationTypeDto[]>;
    public refreshUsageLocationTypesTrigger$: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public subscriptions: Subscription[] = [];

    public constructor(
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private usageLocationTypeService: UsageLocationTypeService
    ) {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.currentGeography = geography;
            })
        );

        //MK 7/24/2025 -- Configure layout menu is watching the route and setting the current geography.
        this.reportingPeriods$ = combineLatest({ geography: this.currentGeography$, refresh: this.refreshReportingPeriodsTrigger$ }).pipe(
            switchMap(({ geography }) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                this.isLoading = false;
            }),
            shareReplay(1)
        );

        this.usageLocationTypes$ = combineLatest({ geography: this.currentGeography$, refresh: this.refreshUsageLocationTypesTrigger$ }).pipe(
            switchMap(({ geography }) => {
                return this.usageLocationTypeService.listUsageLocationType(geography.GeographyID);
            }),
            map((usageLocationTypes) => usageLocationTypes.filter((ult) => ult.CanBeRemoteSensed)),
            shareReplay(1)
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
    }

    public setActiveTab(tab: "Cover Crop" | "Fallowing" | "Surface Water"): void {
        this.activeTab = tab;
    }

    public onUpdatedGeography(): void {
        this.geographyService.getByNameAsMinimalDtoGeography(this.currentGeography.GeographyName.toLowerCase()).subscribe({
            next: (geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            },
        });
    }

    public onUpdatedReportingPeriods(): void {
        this.refreshReportingPeriodsTrigger$.next();
    }

    public onUpdatedUsageLocationTypes(): void {
        this.refreshUsageLocationTypesTrigger$.next();
    }
}
