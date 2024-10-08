import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';


import { AccountReconciliationService } from './api/account-reconciliation.service';
import { AllocationPlanService } from './api/allocation-plan.service';
import { CustomAttributeService } from './api/custom-attribute.service';
import { CustomRichTextService } from './api/custom-rich-text.service';
import { ExternalMapLayerService } from './api/external-map-layer.service';
import { ExternalMapLayerTypeService } from './api/external-map-layer-type.service';
import { FileResourceService } from './api/file-resource.service';
import { FrequentlyAskedQuestionService } from './api/frequently-asked-question.service';
import { GETActionService } from './api/get-action.service';
import { GeographyService } from './api/geography.service';
import { GeographyConfigurationService } from './api/geography-configuration.service';
import { ImpersonationService } from './api/impersonation.service';
import { MenuItemService } from './api/menu-item.service';
import { MeterService } from './api/meter.service';
import { MonitoringWellService } from './api/monitoring-well.service';
import { OpenETConfigurationService } from './api/open-et-configuration.service';
import { OpenETSyncService } from './api/open-et-sync.service';
import { ParcelService } from './api/parcel.service';
import { ParcelSupplyService } from './api/parcel-supply.service';
import { ReportingPeriodService } from './api/reporting-period.service';
import { RoleService } from './api/role.service';
import { SearchService } from './api/search.service';
import { StateService } from './api/state.service';
import { SystemInfoService } from './api/system-info.service';
import { UnitTypeService } from './api/unit-type.service';
import { UsageEntityService } from './api/usage-entity.service';
import { UserService } from './api/user.service';
import { UserClaimsService } from './api/user-claims.service';
import { WaterAccountService } from './api/water-account.service';
import { WaterAccountByGeographyService } from './api/water-account-by-geography.service';
import { WaterAccountParcelService } from './api/water-account-parcel.service';
import { WaterAccountUserService } from './api/water-account-user.service';
import { WaterMeasurementService } from './api/water-measurement.service';
import { WaterMeasurementTypeService } from './api/water-measurement-type.service';
import { WaterTypeService } from './api/water-type.service';
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
    ExternalMapLayerTypeService,
    FileResourceService,
    FrequentlyAskedQuestionService,
    GETActionService,
    GeographyService,
    GeographyConfigurationService,
    ImpersonationService,
    MenuItemService,
    MeterService,
    MonitoringWellService,
    OpenETConfigurationService,
    OpenETSyncService,
    ParcelService,
    ParcelSupplyService,
    ReportingPeriodService,
    RoleService,
    SearchService,
    StateService,
    SystemInfoService,
    UnitTypeService,
    UsageEntityService,
    UserService,
    UserClaimsService,
    WaterAccountService,
    WaterAccountByGeographyService,
    WaterAccountParcelService,
    WaterAccountUserService,
    WaterMeasurementService,
    WaterMeasurementTypeService,
    WaterTypeService,
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
