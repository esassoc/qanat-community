import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';


import { AccountReconciliationService } from './api/account-reconciliation.service';
import { AllocationPlanService } from './api/allocation-plan.service';
import { CustomAttributeService } from './api/custom-attribute.service';
import { CustomRichTextService } from './api/custom-rich-text.service';
import { ExternalMapLayerService } from './api/external-map-layer.service';
import { FeeCalculatorService } from './api/fee-calculator.service';
import { FileResourceService } from './api/file-resource.service';
import { FrequentlyAskedQuestionService } from './api/frequently-asked-question.service';
import { GeographyService } from './api/geography.service';
import { GeographyConfigurationService } from './api/geography-configuration.service';
import { ImpersonationService } from './api/impersonation.service';
import { IrrigationMethodService } from './api/irrigation-method.service';
import { MeterService } from './api/meter.service';
import { ModelService } from './api/model.service';
import { MonitoringWellService } from './api/monitoring-well.service';
import { OpenETConfigurationService } from './api/open-et-configuration.service';
import { OpenETSyncService } from './api/open-et-sync.service';
import { ParcelService } from './api/parcel.service';
import { ParcelByGeographyService } from './api/parcel-by-geography.service';
import { ParcelSupplyByGeographyService } from './api/parcel-supply-by-geography.service';
import { PublicService } from './api/public.service';
import { ReportingPeriodService } from './api/reporting-period.service';
import { ScenarioService } from './api/scenario.service';
import { ScenarioRunService } from './api/scenario-run.service';
import { SearchService } from './api/search.service';
import { SupportTicketService } from './api/support-ticket.service';
import { SystemInfoService } from './api/system-info.service';
import { UnitTypeService } from './api/unit-type.service';
import { UsageEntityService } from './api/usage-entity.service';
import { UsageEntityByGeographyService } from './api/usage-entity-by-geography.service';
import { UserService } from './api/user.service';
import { UserClaimsService } from './api/user-claims.service';
import { WaterAccountService } from './api/water-account.service';
import { WaterAccountByGeographyService } from './api/water-account-by-geography.service';
import { WaterAccountParcelService } from './api/water-account-parcel.service';
import { WaterAccountUserService } from './api/water-account-user.service';
import { WaterMeasurementService } from './api/water-measurement.service';
import { WaterMeasurementReviewSelfReportService } from './api/water-measurement-review-self-report.service';
import { WaterMeasurementSelfReportService } from './api/water-measurement-self-report.service';
import { WaterMeasurementTypeService } from './api/water-measurement-type.service';
import { WaterTypeByGeographyService } from './api/water-type-by-geography.service';
import { WellService } from './api/well.service';
import { WellRegistrationService } from './api/well-registration.service';
import { WellRegistrationFileResourceService } from './api/well-registration-file-resource.service';
import { ZoneGroupService } from './api/zone-group.service';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: [
    AccountReconciliationService,
    AllocationPlanService,
    CustomAttributeService,
    CustomRichTextService,
    ExternalMapLayerService,
    FeeCalculatorService,
    FileResourceService,
    FrequentlyAskedQuestionService,
    GeographyService,
    GeographyConfigurationService,
    ImpersonationService,
    IrrigationMethodService,
    MeterService,
    ModelService,
    MonitoringWellService,
    OpenETConfigurationService,
    OpenETSyncService,
    ParcelService,
    ParcelByGeographyService,
    ParcelSupplyByGeographyService,
    PublicService,
    ReportingPeriodService,
    ScenarioService,
    ScenarioRunService,
    SearchService,
    SupportTicketService,
    SystemInfoService,
    UnitTypeService,
    UsageEntityService,
    UsageEntityByGeographyService,
    UserService,
    UserClaimsService,
    WaterAccountService,
    WaterAccountByGeographyService,
    WaterAccountParcelService,
    WaterAccountUserService,
    WaterMeasurementService,
    WaterMeasurementReviewSelfReportService,
    WaterMeasurementSelfReportService,
    WaterMeasurementTypeService,
    WaterTypeByGeographyService,
    WellService,
    WellRegistrationService,
    WellRegistrationFileResourceService,
    ZoneGroupService,
     ]
})
export class ApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: ApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
