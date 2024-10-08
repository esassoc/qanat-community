import { Routes } from '@angular/router';
import { UnauthenticatedComponent, SubscriptionInsufficientComponent } from './shared/pages';
import { UserListComponent } from './pages/user-list/user-list.component';
import { HomeIndexComponent } from './pages/home/home-index/home-index.component';
import { UserDetailComponent } from './pages/user-detail/user-detail.component';
import { LoginCallbackComponent } from './pages/login-callback/login-callback.component';
import { CreateUserCallbackComponent } from './pages/create-user-callback/create-user-callback.component';
import { FieldDefinitionListComponent } from './pages/field-definition-list/field-definition-list.component';
import { FieldDefinitionEditComponent } from './pages/field-definition-edit/field-definition-edit.component';
import { MsalGuard } from '@azure/msal-angular';
import { UnsavedChangesGuard } from './guards/unsaved-changes-guard';
import { DashboardOnboardComponent } from './pages/dashboards/dashboard-onboard/dashboard-onboard.component';
import { OnboardOverviewComponent } from './pages/dashboards/dashboard-onboard/onboard-overview/onboard-overview.component';
import { OnboardWaterAccountPINsComponent } from './pages/dashboards/dashboard-onboard/onboard-water-account-pins/onboard-water-account-pins.component';
import { OnboardWaterAccountsComponent } from './pages/dashboards/dashboard-onboard/onboard-water-accounts/onboard-water-accounts.component';
import { WellRegistrySelectParcelComponent } from './pages/well-registry-workflow/select-parcel/select-parcel.component';
import { WellLocationComponent } from './pages/well-registry-workflow/well-location/well-location.component';
import { BasicWellInfoComponent } from './pages/well-registry-workflow/basic-well-info/basic-well-info.component';
import { SupportingWellInfoComponent } from './pages/well-registry-workflow/supporting-well-info/supporting-well-info.component';
import { WellAttachmentsComponent } from './pages/well-registry-workflow/well-attachments/well-attachments.component';
import { SubmitComponent } from './pages/well-registry-workflow/submit/submit.component';
import { WellContactsComponent } from './pages/well-registry-workflow/well-contacts/well-contacts.component';
import { WellRegistryWorkflowComponent } from './pages/well-registry-workflow/well-registry-workflow.component';
import { StatisticsComponent } from './pages/statistics/statistics.component';
import { ModelDetailComponent } from './pages/scenarios/model-detail/model-detail.component';
import { ActionDetailComponent } from './pages/scenarios/action-detail/action-detail.component';
import { ScenariosConfigureComponent } from './pages/configure/scenarios-configure/scenarios-configure.component';
import { TradingConfigureComponent } from './pages/configure/trading-configure/trading-configure.component';
import { WaterLevelsConfigureComponent } from './pages/configure/water-levels-configure/water-levels-configure.component';
import { WaterSupplyConfigureComponent } from './pages/configure/water-supply-configure/water-supply-configure.component';
import { ReportingPeriodConfigureComponent } from './pages/configure/water-measurement-types-configure/reporting-period-configure.component';
import { WellRegistryConfigureComponent } from './pages/configure/well-registry-configure/well-registry-configure.component';
import { DashboardConfigureComponent } from './pages/dashboards/dashboard-configure/dashboard-configure.component';
import { PermissionsConfigureComponent } from './pages/configure/permissions-configure/permissions-configure.component';
import { GeospatialDataConfigureComponent } from './pages/configure/geospatial-data-configure/geospatial-data-configure.component';
import { DashboardUpdateParcelsComponent } from './pages/dashboards/dashboard-update-parcels/dashboard-update-parcels.component';
import { StyleGuideComponent } from './pages/style-guide/style-guide.component';
import { DashboardAdminComponent } from './pages/platform-admin/dashboard-admin.component';
import { GeographyWaterManagersEditComponent } from './pages/geography-water-managers-edit/geography-water-managers-edit.component';
import { UserProfileComponent } from './pages/user-profile/user-profile.component';
import { GeographiesComponent } from './pages/geographies/geographies.component';
import { GeospatialDataCreateComponent } from './pages/geospatial-data-create/geospatial-data-create.component';
import { ZoneGroupListComponent } from './pages/zones/zone-group-list/zone-group-list.component';
import { ZoneGroupDetailComponent } from './pages/zones/zone-group-detail/zone-group-detail.component';
import { ZoneGroupEditComponent } from './pages/zones/zone-group-edit/zone-group-edit.component';
import { WaterAccountBudgetsReportComponent } from './pages/water-account-budgets-report/water-account-budgets-report.component';
import { GrowerGuideComponent } from './pages/grower-guide/grower-guide.component';
import { ManagerGuideComponent } from './pages/manager-guide/manager-guide.component';
import { GettingStartedComponent } from './pages/getting-started/getting-started.component';
import { AllocationPlansConfigureComponent } from './pages/configure/allocation-plans-configure/allocation-plans-configure.component';
import { ScenarioActionAddAWellComponent } from './pages/scenarios/scenario-action-add-a-well/scenario-action-add-a-well.component';
import { DashboardScenarioPlannerComponent } from './pages/dashboards/dashboard-scenario-planner/dashboard-scenario-planner.component';
import { ScenarioActionRechargeComponent } from './pages/scenarios/scenario-action-recharge/scenario-action-recharge.component';
import { ScenarioPlannerIndexComponent } from './pages/scenarios/scenario-planner-index/scenario-planner-index.component';
import { ScenarioPlannerAllScenarioRunsComponent } from './pages/scenarios/scenario-planner-all-scenario-runs/scenario-planner-all-scenario-runs.component';
import { NotFoundComponent } from './pages/not-found/not-found.component';
import { ConfirmWellLocationComponent } from './pages/well-registry-workflow/confirm-well-location/confirm-well-location.component';
import { IrrigatedParcelsEditComponent } from './pages/well-registry-workflow/irrigated-parcels-edit/irrigated-parcels-edit.component';
import { wellRegistryEnabledGuard } from './guards/geography-configuration/well-registry-enabled.guard';
import { DashboardWaterAccountWellRegistrationComponent } from './pages/well-list/well-list.component';
import { LandownerWellRegistrationDetailComponent } from './pages/landowner-well-registration-detail/landowner-well-registration-detail.component';
import { GeographyLandingPageComponent } from './pages/geography-landing-page/geography-landing-page.component';
import { landingPageEnabledGuard } from './guards/geography-configuration/landing-page-enabled.guard';
import { AcknowledgementsComponent } from './pages/acknowledgements/acknowledgements.component';
import { withFlagGuard } from './guards/authorization/with-flag.guard';
import { FlagEnum } from './shared/generated/enum/flag-enum';
import { withGeographyFlagGuard } from './guards/authorization/with-geography-flag.guard';
import { PermissionEnum } from './shared/generated/enum/permission-enum';
import { RightsEnum } from './shared/models/enums/rights.enum';
import { GeographySetupComponent } from './pages/geography-setup/geography-setup.component';
import { LandingPageConfigureComponent } from './pages/configure/landing-page-configure/landing-page-configure.component';
import { MeterDataConfigureComponent } from './pages/configure/meter-data-configure/meter-data-configure.component';
import { withGeographyRolePermissionGuard } from './guards/authorization/with-geography-role-permission.guard';
import { CustomAttributesConfigureComponent } from './pages/custom-attributes-configure/custom-attributes-configure.component';
import { FrequentlyAskedQuestionsComponent } from './pages/frequently-asked-questions/frequently-asked-questions.component';
import { WaterAccountCustomAttributesEditComponent } from './pages/water-account-custom-attributes-edit/water-account-custom-attributes-edit.component';
import { ParcelCustomAttributesEditComponent } from './pages/parcel-custom-attributes-edit/parcel-custom-attributes-edit.component';
import { ExampleMapComponent } from './shared/components/leaflet/example-map/example-map.component';
import { AdminFrequentlyAskedQuestionsComponent } from './pages/platform-admin/admin-frequently-asked-questions/admin-frequently-asked-questions.component';
import { AdminGeographiesComponent } from './pages/platform-admin/admin-geographies/admin-geographies.component';
import { AccountActivityComponent } from './pages/water-account-dashboard/account-activity/account-activity.component';
import { AccountAllocationPlansComponent } from './pages/water-account-dashboard/account-allocation-plans/account-allocation-plans.component';
import { WaterAdminPanelComponent } from './pages/water-account-dashboard/water-admin-panel/water-admin-panel.component';
import { WaterBudgetComponent } from './pages/water-account-dashboard/water-budget/water-budget.component';
import { WaterDashboardWaterAccountParcelsComponent } from './pages/water-account-dashboard/water-dashboard-water-account-parcels/water-dashboard-water-account-parcels.component';
import { UsersAndSettingsComponent } from './pages/water-account-dashboard/users-and-settings/users-and-settings.component';
import { WaterAccountDetailLayoutComponent } from './pages/water-account-dashboard/water-account-detail-layout/water-account-detail-layout.component';
import { ParcelDetailLayoutComponent } from './pages/water-account-dashboard/parcel-detail-layout/parcel-detail-layout.component';
import { WaterAccountRequestChangesComponent } from './pages/water-account-dashboard/water-account-request-changes/water-account-request-changes.component';
import { ParcelAdminPanelComponent } from './pages/water-account-dashboard/parcel-admin-panel/parcel-admin-panel.component';
import { ParcelDetailComponent } from './pages/water-account-dashboard/parcel-detail/parcel-detail.component';
import { GeographyMenuLayoutComponent } from './pages/geography-menu/geography-menu-layout/geography-menu-layout.component';
import { GeographyAboutComponent } from './pages/geography-menu/geography-about/geography-about.component';
import { GeographyAllocationsComponent } from './pages/geography-menu/geography-allocations/geography-allocations.component';
import { GeographyGroundwaterLevelsComponent } from './pages/geography-menu/geography-groundwater-levels/geography-groundwater-levels.component';
import { GeographySupportComponent } from './pages/geography-menu/geography-support/geography-support.component';
import { RasterUploadComponent } from './pages/usage-estimates/raster-upload/raster-upload.component';
import { ActivityCenterComponent } from './pages/supply-and-usage/activity-center/activity-center.component';
import { AllocationPlanDetailComponent } from './pages/supply-and-usage/allocation-plan-detail/allocation-plan-detail.component';
import { AllocationPlansComponent } from './pages/supply-and-usage/allocation-plans/allocation-plans.component';
import { MeterListComponent } from './pages/supply-and-usage/meter-list/meter-list.component';
import { OpenetSyncIntegrationComponent } from './pages/supply-and-usage/openet-sync-integration/openet-sync-integration.component';
import { ParcelBulkActionsComponent } from './pages/supply-and-usage/parcel-bulk-actions/parcel-bulk-actions.component';
import { ParcelsReviewChangesComponent } from './pages/supply-and-usage/parcels-review-changes/parcels-review-changes.component';
import { ReferenceWellsListComponent } from './pages/supply-and-usage/reference-wells-list/reference-wells-list.component';
import { ReferenceWellsUploadComponent } from './pages/supply-and-usage/reference-wells-upload/reference-wells-upload.component';
import { ManagerWellRegistrationDetailComponent } from './pages/supply-and-usage/supply-and-usage-menu-layout/manager-well-registration-detail/manager-well-registration-detail.component';
import { ReviewSubmittedWellsComponent } from './pages/supply-and-usage/supply-and-usage-menu-layout/review-submitted-wells/review-submitted-wells.component';
import { SupplyAndUsageMenuLayoutComponent } from './pages/supply-and-usage/supply-and-usage-menu-layout/supply-and-usage-menu-layout.component';
import { WellDetailOutletComponent } from './pages/supply-and-usage/supply-and-usage-menu-layout/well-detail-outlet/well-detail-outlet.component';
import { WellRegistrationListComponent } from './pages/supply-and-usage/supply-and-usage-menu-layout/well-registration-list/well-registration-list.component';
import { UpdateParcelsConfirmComponent } from './pages/supply-and-usage/update-parcels/update-parcels-confirm/update-parcels-confirm.component';
import { UpdateParcelsReviewComponent } from './pages/supply-and-usage/update-parcels/update-parcels-review/update-parcels-review.component';
import { UpdateParcelsUploadComponent } from './pages/supply-and-usage/update-parcels/update-parcels-upload/update-parcels-upload.component';
import { UploadUsageEntityGdbComponent } from './pages/supply-and-usage/upload-usage-entity-gdb/upload-usage-entity-gdb.component';
import { UsageEstimatesComponent } from './pages/supply-and-usage/usage-estimates/usage-estimates.component';
import { WaterAccountSuggestionsComponent } from './pages/supply-and-usage/water-account-suggestions/water-account-suggestions.component';
import { WaterTransactionsBulkCreateComponent } from './pages/supply-and-usage/water-transactions/bulk-create/water-transactions-bulk-create.component';
import { WaterTransactionsCreateComponent } from './pages/supply-and-usage/water-transactions/create/water-transactions-create.component';
import { WaterTransactionsCsvUploadSupplyComponent } from './pages/supply-and-usage/water-transactions/csv-upload-supply/water-transactions-csv-upload-supply.component';
import { WaterTransactionsCsvUploadUsageComponent } from './pages/supply-and-usage/water-transactions/csv-upload-usage/water-transactions-csv-upload-usage.component';
import { WaterTransactionsComponent } from './pages/supply-and-usage/water-transactions/water-transactions.component';
import { WellBulkUploadComponent } from './pages/supply-and-usage/well-bulk-upload/well-bulk-upload.component';
import { WellDetailComponent } from './pages/supply-and-usage/well-detail/well-detail.component';
import { WellIrrigatedParcelsEditComponent } from './pages/supply-and-usage/well-irrigated-parcels-edit/well-irrigated-parcels-edit.component';
import { WellLocationEditComponent } from './pages/supply-and-usage/well-location-edit/well-location-edit.component';
import { ZoneGroupDataUploaderComponent } from './pages/supply-and-usage/zone-group-data-uploader/zone-group-data-uploader.component';
import { WaterAccountListComponent } from './pages/water-account-dashboard/water-account-list/water-account-list.component';
import { ParcelListComponent } from './pages/water-account-dashboard/parcel-list/parcel-list.component';
import { WaterDashboardWaterAccountWellsComponent } from './pages/water-account-dashboard/water-dashboard-water-account-wells/water-dashboard-water-account-wells.component';

export const routeParams = {
  userID: 'userID',
  fieldDefinitionID: 'fieldDefinitionID',
  parcelID: 'parcelID',
  tagID: 'tagID',
  wellRegistrationID: 'wellRegistrationID',
  wellID: 'wellID',
  modelShortName: 'modelID',
  scenarioShortName: 'scenarioID',
  actionID: 'actionID',
  geographyName: 'geographyName',
  waterAccountID: 'waterAccountID',
  externalMapLayerID: 'externalMapLayerID',
  zoneGroupSlug: 'zoneGroupSlug',
  zoneSlug: 'zoneSlug',
  waterTypeSlug: 'waterTypeSlug',
};

export const routes: Routes = [
  {
    path: 'configure',
    title: 'Configure Geography',
    component: DashboardConfigureComponent,
    canActivate: [MsalGuard],
  },
  {
    path: `configure/:${routeParams.geographyName}`,
    component: DashboardConfigureComponent,
    canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
    children: [
      { path: 'allocation-plans', title: 'Allocation Plans', component: AllocationPlansConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
      { path: 'custom-attributes', component: CustomAttributesConfigureComponent, title: 'Custom Attributes', canDeactivate: [UnsavedChangesGuard] },
      { path: `edit-water-managers`, component: GeographyWaterManagersEditComponent, title: 'Edit Water Managers' },
      { path: 'geospatial-data', title: 'Geospatial Data Layers', component: GeospatialDataConfigureComponent },
      {
        path: 'geospatial-data/create',
        title: 'Create Geospatial Data Layer',
        component: GeospatialDataCreateComponent,
        data: { create: true },
        canDeactivate: [UnsavedChangesGuard],
      },
      {
        path: `geospatial-data/edit/:${routeParams.externalMapLayerID}`,
        title: 'Edit Geospatial Data Layer',
        data: { create: false },
        component: GeospatialDataCreateComponent,
        canDeactivate: [UnsavedChangesGuard],
      },
      { path: 'landing-page', title: 'Landing Page', component: LandingPageConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
      { path: 'meter-data', title: 'Meter Data', component: MeterDataConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
      { path: 'permissions', title: 'Permissions', component: PermissionsConfigureComponent },
      { path: 'reporting-period', title: 'Reporting Period', component: ReportingPeriodConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
      { path: 'scenarios', title: 'Scenarios', component: ScenariosConfigureComponent },
      { path: `setup`, component: GeographySetupComponent, title: 'Setup', canDeactivate: [UnsavedChangesGuard] },
      { path: 'trading', title: 'Trading', component: TradingConfigureComponent },
      { path: 'water-supply', title: 'Water Supply', component: WaterSupplyConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
      { path: 'water-levels', title: 'Water Levels', component: WaterLevelsConfigureComponent },
      { path: 'well-registry', title: 'Well Registry', component: WellRegistryConfigureComponent },
      { path: 'zones', title: 'Zones', component: ZoneGroupEditComponent, canDeactivate: [UnsavedChangesGuard] },
    ],
  },
  { path: 'geographies', title: 'Geographies', component: GeographiesComponent },
  {
    path: `geographies/:${routeParams.geographyName}`,
    title: 'Geography',
    component: GeographyMenuLayoutComponent,

    children: [
      { path: '', redirectTo: 'overview', pathMatch: 'full' },
      {
        path: 'allocation-plans',
        title: 'Allocation Plans',
        component: GeographyAllocationsComponent,
        children: [
          {
            path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`,
            title: 'Allocation Plan',
            component: AllocationPlanDetailComponent,
            data: { editable: false },
          },
        ],
      },
      {
        path: 'groundwater-levels',
        title: 'Groundwater Levels',
        component: GeographyGroundwaterLevelsComponent,
        canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
      },
      { path: 'overview', title: 'Geography Overview', component: GeographyAboutComponent },
      { path: 'support', title: 'Support', component: GeographySupportComponent },
    ],
  },
  {
    path: `:${routeParams.geographyName}/claim-water-accounts`,
    title: 'Onboard',
    component: DashboardOnboardComponent,
    canActivate: [MsalGuard],
    children: [
      { path: '', redirectTo: 'overview', pathMatch: 'full' },
      { path: 'enter-pin', title: 'Water Account PINs', component: OnboardWaterAccountPINsComponent },
      { path: 'overview', title: 'Overview', component: OnboardOverviewComponent },
      { path: 'review', title: 'Claim Accounts', component: OnboardWaterAccountsComponent },
    ],
  },
  {
    path: 'platform-admin',
    component: DashboardAdminComponent,
    title: 'Platform Admin',
    canActivate: [MsalGuard, withFlagGuard(FlagEnum.HasAdminDashboard)],
    children: [
      { path: '', redirectTo: 'geographies', pathMatch: 'full' },
      { path: 'frequently-asked-questions', component: AdminFrequentlyAskedQuestionsComponent, title: 'Frequently Asked Questions' },
      { path: 'geographies', component: AdminGeographiesComponent, title: 'Geographies' },
      { path: 'labels-and-definitions', component: FieldDefinitionListComponent, title: 'Labels & Definitions' },
      {
        path: `labels-and-definitions/:${routeParams.fieldDefinitionID}`,
        component: FieldDefinitionEditComponent,
        canDeactivate: [UnsavedChangesGuard],
        title: 'Edit Label Definition',
      },
      { path: 'users', component: UserListComponent, title: 'Users' },
      { path: `users/:${routeParams.userID}`, component: UserDetailComponent, title: 'User Detail', data: { displayProfileEdit: false } },
    ],
  },
  {
    path: 'profile',
    title: 'User Profile',
    component: UserProfileComponent,
    canActivate: [MsalGuard],
    children: [{ path: '', title: 'Details', component: UserDetailComponent, data: { displayProfileEdit: true } }],
  },
  {
    path: `profile/well-registrations/:${routeParams.wellRegistrationID}`,
    title: 'Well Registration',
    component: LandownerWellRegistrationDetailComponent,
    canActivate: [MsalGuard],
  },
  {
    path: 'scenario-planner',
    title: 'Scenario Planner',
    component: DashboardScenarioPlannerComponent,
    canActivate: [MsalGuard],
    children: [
      { path: '', redirectTo: 'models', pathMatch: 'full' },
      { path: 'runs', title: 'Scenario Runs', component: ScenarioPlannerAllScenarioRunsComponent },
      {
        path: 'models',
        title: 'Models',
        component: ScenarioPlannerIndexComponent,
        children: [
          { path: `:${routeParams.modelShortName}`, title: 'Model Details', component: ModelDetailComponent },
          {
            path: `:${routeParams.modelShortName}/add-a-well/new`,
            title: 'New Add a Well Scenario',
            component: ScenarioActionAddAWellComponent,
            canDeactivate: [UnsavedChangesGuard],
          },
          { path: `:${routeParams.modelShortName}/add-a-well/:${routeParams.actionID}`, title: 'Add a Well Scenario Run Reults', component: ActionDetailComponent },
          { path: `:${routeParams.modelShortName}/recharge/:${routeParams.actionID}`, title: 'Recharge Scenario Run Results', component: ActionDetailComponent },
          {
            path: `:${routeParams.modelShortName}/recharge/new`,
            title: 'New Recharge Scenario',
            component: ScenarioActionRechargeComponent,
            canDeactivate: [UnsavedChangesGuard],
          },
        ],
      },
    ],
  },
  {
    path: 'supply-and-usage',
    component: SupplyAndUsageMenuLayoutComponent,
    canActivate: [MsalGuard],
  },

  {
    path: `supply-and-usage/:${routeParams.geographyName}`,
    title: 'Supply and Usage Menu',
    component: SupplyAndUsageMenuLayoutComponent,
    canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
    children: [
      { path: '', redirectTo: 'activity-center', pathMatch: 'full' },
      { path: 'activity-center', title: 'Activity Center', component: ActivityCenterComponent },
      {
        path: 'parcels/allocation-plans',
        title: 'Allocation Plans',
        component: AllocationPlansComponent,
        children: [
          {
            path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`,
            title: 'Allocation Plan',
            component: AllocationPlanDetailComponent,
            data: { editable: true },
          },
        ],
      },
      { path: 'parcels/bulk-actions', title: 'Bulk Parcel Actions', component: ParcelBulkActionsComponent },
      { path: 'parcels/update', title: 'Update Parcels', component: ParcelsReviewChangesComponent },
      { path: `parcels/upload-usage-entity-gdb`, component: UploadUsageEntityGdbComponent, title: 'Upload Usage Entity GDB' },
      { path: 'statistics', title: 'Statistics', component: StatisticsComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
      {
        path: 'water-account-budgets-report',
        title: 'Water Account Budget Report',
        canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
        component: WaterAccountBudgetsReportComponent,
      },
      { path: 'water-accounts/water-account-suggestions', title: 'Water Account Suggestions', component: WaterAccountSuggestionsComponent },
      { path: 'water-measurements/raster-upload', title: 'Upload Raster', component: RasterUploadComponent },
      { path: 'water-measurements', title: 'Water Measurements', component: UsageEstimatesComponent },
      { path: 'water-measurements/upload-water-measurements', title: 'Upload Water Measurements', component: WaterTransactionsCsvUploadUsageComponent },
      { path: 'water-supply', title: 'Supply', component: WaterTransactionsComponent },
      { path: 'water-supply/bulk-new', title: 'Bulk Create Transactions', component: WaterTransactionsBulkCreateComponent },
      { path: 'water-supply/csv-upload/supply', title: 'Upload Supply', component: WaterTransactionsCsvUploadSupplyComponent },
      { path: 'water-supply/new', title: 'New Transactions', component: WaterTransactionsCreateComponent },
      { path: `water-supply/new/:${routeParams.parcelID}`, title: 'New Parcel Transactions', component: WaterTransactionsCreateComponent },
      { path: 'water-supply/openet-integration', title: 'OpenET Integration', component: OpenetSyncIntegrationComponent },
      {
        path: `wells/:${routeParams.wellID}`,
        title: 'Well',
        component: WellDetailOutletComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
        children: [
          { path: ``, title: 'Well', component: WellDetailComponent },
          { path: `update-irrigated-parcels`, title: 'Update Irrigated Parcels', component: WellIrrigatedParcelsEditComponent },
          { path: `update-location`, title: 'Update Well Location', component: WellLocationEditComponent },
        ],
      },
      { path: 'wells/bulk-upload', title: 'Bulk Upload Wells', component: WellBulkUploadComponent },
      { path: 'wells/review-submitted-wells', title: 'Review Submitted Wells', component: ReviewSubmittedWellsComponent },
      { path: 'wells/reference-wells', title: 'Reference Wells', component: ReferenceWellsListComponent },
      { path: 'wells/reference-wells/upload', title: 'Bulk Upload Reference Wells', component: ReferenceWellsUploadComponent },
      { path: 'wells/meters', title: 'Meters', component: MeterListComponent },
      { path: 'wells/well-registrations', title: 'View All Well Registrations', component: WellRegistrationListComponent },
      { path: `wells/well-registrations/:${routeParams.wellRegistrationID}`, title: 'Well Registration', component: ManagerWellRegistrationDetailComponent },
      { path: 'zones', title: 'Zones', component: ZoneGroupListComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
      {
        path: `zones/:${routeParams.zoneGroupSlug}`,
        title: 'Zone Group',
        component: ZoneGroupDetailComponent,
        canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
      },
      { path: `zones/upload`, title: 'Upload Zones', component: ZoneGroupDataUploaderComponent },
    ],
  },
  {
    path: `supply-and-usage/:${routeParams.geographyName}/upload-parcel-data`,
    title: 'Update Parcels',
    component: DashboardUpdateParcelsComponent,
    canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
    children: [
      { path: '', redirectTo: 'upload', pathMatch: 'full' },
      { path: 'confirm', title: 'Confirm Upload', component: UpdateParcelsConfirmComponent },
      { path: 'review-parcels', title: 'Review Parcels', component: UpdateParcelsReviewComponent },
      { path: 'upload', title: 'Upload Parcels', component: UpdateParcelsUploadComponent },
    ],
  },
  {
    path: 'water-dashboard',
    title: 'Water Dashboard',
    canActivate: [MsalGuard],
    children: [
      { path: '', redirectTo: 'water-accounts', pathMatch: 'full' },
      {
        path: 'water-accounts',
        title: 'Water Accounts',
        component: WaterAccountListComponent,
        canActivate: [MsalGuard],
      },
      { path: `water-accounts/request-changes`, title: 'Request Changes', component: WaterAccountRequestChangesComponent, canDeactivate: [UnsavedChangesGuard] },
      {
        path: `water-accounts/:${routeParams.geographyName}/suggestions`,
        title: 'Water Account Suggestions',
        component: WaterAccountSuggestionsComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WaterAccountRights, RightsEnum.Read)],
        canDeactivate: [UnsavedChangesGuard],
      },
      {
        path: `water-accounts/:${routeParams.waterAccountID}`,
        title: 'Water Account',
        component: WaterAccountDetailLayoutComponent,
        canActivate: [MsalGuard],
        children: [
          { path: '', redirectTo: 'water-budget', pathMatch: 'full' },
          { path: 'activity', title: 'Activity', component: AccountActivityComponent },
          { path: 'admin-panel', title: 'Water Account Admin Panel', component: WaterAdminPanelComponent },
          {
            path: 'allocation-plans',
            title: 'Allocation plans',
            component: AccountAllocationPlansComponent,
            children: [{ path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`, component: AllocationPlanDetailComponent, data: { editable: false } }],
          },
          { path: `admin-panel/edit-attributes`, title: 'Edit Attributes', component: WaterAccountCustomAttributesEditComponent, canDeactivate: [UnsavedChangesGuard] },
          { path: 'parcels', title: 'Parcels', component: WaterDashboardWaterAccountParcelsComponent },
          { path: `users-and-settings`, title: 'Users & Settings', component: UsersAndSettingsComponent },
          { path: 'water-budget', title: 'Water Budget', component: WaterBudgetComponent },
          { path: 'wells', title: 'Wells', component: WaterDashboardWaterAccountWellsComponent },
        ],
      },
      {
        path: 'parcels',
        title: 'Water Dashboard - Parcels',
        component: ParcelListComponent,
        canActivate: [MsalGuard],
      },
      {
        path: `parcels/:${routeParams.geographyName}/bulk-actions`,
        title: 'Bulk Parcel Actions',
        component: ParcelBulkActionsComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
      },
      {
        path: `parcels/:${routeParams.geographyName}/update`,
        title: 'Update Parcels',
        component: ParcelsReviewChangesComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
      },
      {
        path: `parcels/:${routeParams.geographyName}/zones/upload`,
        title: 'Assign Parcels to Zones',
        component: ZoneGroupDataUploaderComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
      },
      {
        path: `parcels/:${routeParams.parcelID}`,
        title: 'Water Dashboard - Parcel',
        component: ParcelDetailLayoutComponent,
        canActivate: [MsalGuard],
        children: [
          { path: '', redirectTo: 'detail', pathMatch: 'full' },
          { path: `admin-panel`, title: 'Parcel Admin Panel', component: ParcelAdminPanelComponent, canActivate: [MsalGuard] },
          { path: `detail`, title: 'Water Dashboard - Parcel', component: ParcelDetailComponent, canActivate: [MsalGuard] },
          { path: `admin-panel/edit-attributes`, title: 'Edit Attributes', component: ParcelCustomAttributesEditComponent, canDeactivate: [UnsavedChangesGuard] },
        ],
      },

      {
        path: 'wells',
        title: 'Water Dashboard - Wells',
        component: DashboardWaterAccountWellRegistrationComponent,
        canActivate: [MsalGuard],
      },
      {
        path: `wells/:${routeParams.geographyName}/reference-wells`,
        title: 'Reference Wells',
        component: ReferenceWellsListComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
      },
      {
        path: `wells/:${routeParams.geographyName}/review-submitted-wells`,
        title: 'Review Submitted Wells',
        component: ReviewSubmittedWellsComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
      },
      {
        path: `wells/:${routeParams.geographyName}/well-registrations`,
        title: 'Well Registrations',
        component: WellRegistrationListComponent,
        canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
      },
    ],
  },
  {
    path: `well-registry/:${routeParams.geographyName}`,
    title: 'Well Registry',
    canActivate: [MsalGuard, wellRegistryEnabledGuard],
    children: [
      { path: '', redirectTo: 'new', pathMatch: 'full' },
      {
        path: 'new',
        title: 'New Well',
        component: WellRegistryWorkflowComponent,
        data: { create: true },
        children: [
          { path: '', redirectTo: 'select-parcel', pathMatch: 'full' },
          { path: 'select-parcel', title: 'Select a Parcel', component: WellRegistrySelectParcelComponent },
        ],
      },
      {
        path: `well/:${routeParams.wellRegistrationID}/edit`,
        title: 'Edit Well',
        component: WellRegistryWorkflowComponent,
        children: [
          { path: '', redirectTo: 'select-parcel', pathMatch: 'full' },
          { path: 'attachments', title: 'Attachments', component: WellAttachmentsComponent },
          { path: 'basic-information', title: 'Basic Information', component: BasicWellInfoComponent, canDeactivate: [UnsavedChangesGuard] },
          { path: 'confirm-location', title: 'Location', component: ConfirmWellLocationComponent },
          { path: 'contacts', title: 'Contacts', component: WellContactsComponent, canDeactivate: [UnsavedChangesGuard] },
          { path: 'irrigated-parcels', title: 'Location', component: IrrigatedParcelsEditComponent, canDeactivate: [UnsavedChangesGuard] },
          { path: 'location', title: 'Location', component: WellLocationComponent },
          { path: 'select-parcel', title: 'Select a Parcel', component: WellRegistrySelectParcelComponent },
          { path: 'submit', title: 'Submit', component: SubmitComponent },
          { path: 'supporting-information', title: 'Supporting Information', component: SupportingWellInfoComponent, canDeactivate: [UnsavedChangesGuard] },
        ],
      },
    ],
  },

  { path: 'acknowledgements', title: 'Acknowledgements', component: AcknowledgementsComponent },
  { path: 'create-user-callback', component: CreateUserCallbackComponent },
  { path: 'example-map', title: 'Example Map', component: ExampleMapComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
  { path: 'frequently-asked-questions', title: 'Frequently Asked Questions', component: FrequentlyAskedQuestionsComponent },
  { path: 'getting-started', title: 'Getting Started', component: GettingStartedComponent },
  { path: 'grower-guide', title: 'Grower Guide', component: GrowerGuideComponent },
  { path: 'manager-guide', title: 'Manager Guide', component: ManagerGuideComponent },
  { path: 'not-found', title: 'Page Not Found', component: NotFoundComponent },
  { path: 'signin-oidc', component: LoginCallbackComponent },
  { path: 'style-guide', title: 'Style Guide', component: StyleGuideComponent },
  { path: 'subscription-insufficient', title: 'Insufficient Priveleges', component: SubscriptionInsufficientComponent },
  { path: 'unauthenticated', title: 'Unauthenticated', component: UnauthenticatedComponent },

  // DO NOT REORDER THESE THAT WILL CAUSE THEM TO NOT WORK
  { path: '', component: HomeIndexComponent },
  { path: `:${routeParams.geographyName}`, component: GeographyLandingPageComponent, canActivate: [landingPageEnabledGuard] },
  { path: '**', title: 'Page Not Found', component: NotFoundComponent },
];
