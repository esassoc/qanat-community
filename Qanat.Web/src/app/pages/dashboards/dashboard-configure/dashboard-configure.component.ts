import { Component, OnInit } from "@angular/core";
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/models";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { ConfigureCardComponent } from "../../../shared/components/configure-card/configure-card.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GeographySwitcherComponent } from "../../../shared/components/geography-switcher/geography-switcher.component";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "dashboard-configure",
    templateUrl: "./dashboard-configure.component.html",
    styleUrls: ["./dashboard-configure.component.scss"],
    standalone: true,
    imports: [
        NgIf,
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
    public currentGeography$: Observable<GeographyDto>;
    public currentGeographyID: number;

    public reportingPeriodRichTextTypeID = CustomRichTextTypeEnum.ReportingPeriodConfiguration;
    public waterSupplyRichTextTypeID = CustomRichTextTypeEnum.WaterSupplyConfiguration;
    public waterLevelsRichTextTypeID = CustomRichTextTypeEnum.WaterLevelsConfiguration;
    public tradingRichTextTypeID = CustomRichTextTypeEnum.TradingConfiguration;
    public scenariosRichTextTypeID = CustomRichTextTypeEnum.ScenariosConfiguration;
    public wellRegistryRichTextTypeID = CustomRichTextTypeEnum.WellRegistryConfiguration;
    public permissionsRichTextTypeID = CustomRichTextTypeEnum.PermissionsConfiguration;
    public geospatialDataRichTextTypeID = CustomRichTextTypeEnum.GeospatialDataConfiguration;
    public zoneGroupRichTextTypeID = CustomRichTextTypeEnum.ZoneGroupConfiguration;
    public landingPageRichTextTypeID = CustomRichTextTypeEnum.ConfigureLandingPage;
    public meterRichTextTypeID = CustomRichTextTypeEnum.MeterConfiguration;
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
        private selectedGeographyService: SelectedGeographyService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.pipe(
            tap((geography) => {
                this.currentGeographyID = geography.GeographyID;
                if (this.router.routerState.snapshot.url == "/configure") {
                    this.redirectToGeography(geography.GeographyName);
                }
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
