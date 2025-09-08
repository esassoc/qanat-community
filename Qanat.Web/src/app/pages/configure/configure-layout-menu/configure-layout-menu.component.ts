import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyMinimalDto, GeographyAllocationPlanConfigurationDto } from "src/app/shared/generated/model/models";
import { ConfigureCardComponent } from "../../../shared/components/configure-card/configure-card.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GeographySwitcherComponent } from "../../../shared/components/geography-switcher/geography-switcher.component";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { AsyncPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { routeParams } from "src/app/app.routes";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";

@Component({
    selector: "configure-layout-menu",
    templateUrl: "./configure-layout-menu.component.html",
    styleUrls: ["./configure-layout-menu.component.scss"],
    imports: [
        RouterLink,
        GeographyLogoComponent,
        IconComponent,
        GeographySwitcherComponent,
        RouterLinkActive,
        RouterOutlet,
        PageHeaderComponent,
        AlertDisplayComponent,
        ConfigureCardComponent,
        AsyncPipe,
    ],
})
export class DashboardConfigureComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public allocationPlan$: Observable<GeographyAllocationPlanConfigurationDto>;

    public reportingPeriodRichTextTypeID = CustomRichTextTypeEnum.ReportingPeriodConfiguration;
    public statementTemplateRichTextTypeID = CustomRichTextTypeEnum.StatementTemplateConfiguration;
    public waterSupplyRichTextTypeID = CustomRichTextTypeEnum.WaterSupplyConfiguration;
    public waterLevelsRichTextTypeID = CustomRichTextTypeEnum.WaterLevelsConfiguration;
    public tradingRichTextTypeID = CustomRichTextTypeEnum.TradingConfiguration;
    public scenariosRichTextTypeID = CustomRichTextTypeEnum.ScenariosConfiguration;
    public wellRegistryRichTextTypeID = CustomRichTextTypeEnum.WellRegistryConfiguration;
    public permissionsRichTextTypeID = CustomRichTextTypeEnum.PermissionsConfiguration;
    public geospatialDataRichTextTypeID = CustomRichTextTypeEnum.GeospatialDataConfiguration;
    public zoneGroupRichTextTypeID = CustomRichTextTypeEnum.ZoneGroupConfiguration;
    public usageLocationTypeRichTextTypeID = CustomRichTextTypeEnum.UsageLocationTypeConfiguration;
    public landingPageRichTextTypeID = CustomRichTextTypeEnum.ConfigureLandingPage;
    public meterRichTextTypeID = CustomRichTextTypeEnum.MeterConfiguration;
    public selfReportingRichTextTypeID = CustomRichTextTypeEnum.SelfReportingConfiguration;
    public allocationPlanRichTextTypeID = CustomRichTextTypeEnum.AllocationPlanConfigureCard;
    public setupRichTextTypeID = CustomRichTextTypeEnum.ConfigureGeographySetup;
    public customAttributesRichTextTypeID = CustomRichTextTypeEnum.ConfigureCustomAttributes;
    public waterManagersRichTextTypeID = CustomRichTextTypeEnum.ConfigureWaterManagers;

    public waterSupplyActive: boolean = true;
    public waterUsageActive: boolean = true;

    public routerLinkActiveOptions = {
        exact: true,
    };

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private allocationPlanService: AllocationPlanService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        if (this.router.routerState.snapshot.url == "/configure") {
            this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
                tap((geography) => {
                    if (geography) {
                        this.redirectToGeography(geography.GeographyName);
                    }
                })
            );
        } else {
            this.geography$ = this.route.params.pipe(
                switchMap((params) => {
                    const geographyName = params[routeParams.geographyName];
                    return this.geographyService.getByNameAsMinimalDtoGeography(geographyName).pipe(
                        tap((geography) => {
                            this.currentGeographyService.setCurrentGeography(geography);
                        })
                    );
                })
            );
        }

        this.allocationPlan$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.allocationPlanService.getAllocationPlanConfigurationByGeographyIDAllocationPlan(geography.GeographyID);
            })
        );
    }

    redirectToGeography(geographyName: string) {
        this.router.navigateByUrl(`/configure/${geographyName.toLowerCase()}`);
    }

    changeWaterSupplyActive() {
        this.waterSupplyActive = !this.waterSupplyActive;
    }

    isWaterSupplyActive(): boolean {
        return this.waterSupplyActive;
    }
}
