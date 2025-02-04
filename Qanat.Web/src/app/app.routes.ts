import { Routes } from "@angular/router";
import { UnauthenticatedComponent, SubscriptionInsufficientComponent } from "./shared/pages";
import { UserListComponent } from "./pages/platform-admin/user-list/user-list.component";
import { HomeIndexComponent } from "./pages/home/home-index/home-index.component";
import { UserDetailComponent } from "./pages/user-detail/user-detail.component";
import { LoginCallbackComponent } from "./pages/login-callback/login-callback.component";
import { CreateUserCallbackComponent } from "./pages/create-user-callback/create-user-callback.component";
import { FieldDefinitionListComponent } from "./pages/field-definition-list/field-definition-list.component";
import { FieldDefinitionEditComponent } from "./pages/field-definition-edit/field-definition-edit.component";
import { MsalGuard } from "@azure/msal-angular";
import { UnsavedChangesGuard } from "./guards/unsaved-changes-guard";
import { DashboardOnboardComponent } from "./pages/dashboards/dashboard-onboard/dashboard-onboard.component";
import { OnboardOverviewComponent } from "./pages/dashboards/dashboard-onboard/onboard-overview/onboard-overview.component";
import { OnboardWaterAccountPINsComponent } from "./pages/dashboards/dashboard-onboard/onboard-water-account-pins/onboard-water-account-pins.component";
import { OnboardWaterAccountsComponent } from "./pages/dashboards/dashboard-onboard/onboard-water-accounts/onboard-water-accounts.component";
import { WellRegistrySelectParcelComponent } from "./pages/well-registry-workflow/select-parcel/select-parcel.component";
import { WellLocationComponent } from "./pages/well-registry-workflow/well-location/well-location.component";
import { BasicWellInfoComponent } from "./pages/well-registry-workflow/basic-well-info/basic-well-info.component";
import { SupportingWellInfoComponent } from "./pages/well-registry-workflow/supporting-well-info/supporting-well-info.component";
import { WellAttachmentsComponent } from "./pages/well-registry-workflow/well-attachments/well-attachments.component";
import { SubmitComponent } from "./pages/well-registry-workflow/submit/submit.component";
import { WellContactsComponent } from "./pages/well-registry-workflow/well-contacts/well-contacts.component";
import { WellRegistryWorkflowComponent } from "./pages/well-registry-workflow/well-registry-workflow.component";
import { StatisticsComponent } from "./pages/supply-and-usage/statistics/statistics.component";
import { ModelDetailComponent } from "./pages/scenarios/model-detail/model-detail.component";
import { ScenarioRunDetailComponent as ScenarioRunDetailComponent } from "./pages/scenarios/scenario-run-detail/scenario-run-detail.component";
import { WaterSupplyConfigureComponent } from "./pages/configure/water-supply-configure/water-supply-configure.component";
import { ReportingPeriodConfigureComponent } from "./pages/configure/reporting-period-configure/reporting-period-configure.component";
import { WellRegistryConfigureComponent } from "./pages/configure/well-registry-configure/well-registry-configure.component";
import { DashboardConfigureComponent as ConfigureLayoutMenu } from "./pages/configure/configure-layout-menu/configure-layout-menu.component";
import { GeospatialDataConfigureComponent } from "./pages/configure/geospatial-data-configure/geospatial-data-configure.component";
import { DashboardUpdateParcelsComponent } from "./pages/parcels/dashboard-update-parcels/dashboard-update-parcels.component";
import { StyleGuideComponent } from "./pages/style-guide/style-guide.component";
import { DashboardAdminComponent } from "./pages/platform-admin/dashboard-admin.component";
import { UserProfileComponent } from "./pages/user-profile/user-profile.component";
import { GeographiesComponent } from "./pages/geographies/geographies.component";
import { GeospatialDataCreateComponent } from "./pages/configure/geospatial-data-configure/geospatial-data-create/geospatial-data-create.component";
import { ZoneGroupListComponent } from "./pages/supply-and-usage/zones/zone-group-list/zone-group-list.component";
import { ZoneGroupDetailComponent } from "./pages/supply-and-usage/zones/zone-group-detail/zone-group-detail.component";
import { WaterAccountBudgetsReportComponent } from "./pages/supply-and-usage/water-account-budgets-report/water-account-budgets-report.component";
import { GrowerGuideComponent } from "./pages/grower-guide/grower-guide.component";
import { ManagerGuideComponent } from "./pages/manager-guide/manager-guide.component";
import { GettingStartedComponent } from "./pages/getting-started/getting-started.component";
import { AllocationPlansConfigureComponent } from "./pages/configure/allocation-plans-configure/allocation-plans-configure.component";
import { ScenarioActionAddAWellComponent as ScenarioRunAddAWellComponent } from "./pages/scenarios/scenario-run-add-a-well/scenario-run-add-a-well.component";
import { DashboardScenarioPlannerComponent } from "./pages/scenarios/scenario-planner-layout-menu/scenario-planner-layout-menu.component";
import { ScenarioRunRechargeComponent } from "./pages/scenarios/scenario-run-recharge/scenario-run-recharge.component";
import { ScenarioPlannerIndexComponent } from "./pages/scenarios/scenario-planner-index/scenario-planner-index.component";
import { ScenarioPlannerAllScenarioRunsComponent } from "./pages/scenarios/scenario-planner-all-scenario-runs/scenario-planner-all-scenario-runs.component";
import { NotFoundComponent } from "./pages/not-found/not-found.component";
import { ConfirmWellLocationComponent } from "./pages/well-registry-workflow/confirm-well-location/confirm-well-location.component";
import { IrrigatedParcelsEditComponent } from "./pages/well-registry-workflow/irrigated-parcels-edit/irrigated-parcels-edit.component";
import { wellRegistryEnabledGuard } from "./guards/geography-configuration/well-registry-enabled.guard";
import { WellRegistrationDetailComponent } from "./pages/wells/well-registration-detail/well-registration-detail.component";
import { GeographyLandingPageComponent } from "./pages/geography-landing-page/geography-landing-page.component";
import { landingPageEnabledGuard } from "./guards/geography-configuration/landing-page-enabled.guard";
import { AcknowledgementsComponent } from "./pages/acknowledgements/acknowledgements.component";
import { withFlagGuard } from "./guards/authorization/with-flag.guard";
import { FlagEnum } from "./shared/generated/enum/flag-enum";
import { withGeographyFlagGuard } from "./guards/authorization/with-geography-flag.guard";
import { PermissionEnum } from "./shared/generated/enum/permission-enum";
import { RightsEnum } from "./shared/models/enums/rights.enum";
import { GeographySetupComponent } from "./pages/configure/geography-setup/geography-setup.component";
import { LandingPageConfigureComponent } from "./pages/configure/landing-page-configure/landing-page-configure.component";
import { MeterDataConfigureComponent } from "./pages/configure/meter-data-configure/meter-data-configure.component";
import { withGeographyRolePermissionGuard } from "./guards/authorization/with-geography-role-permission.guard";
import { CustomAttributesConfigureComponent } from "./pages/configure/custom-attributes-configure/custom-attributes-configure.component";
import { FrequentlyAskedQuestionsComponent } from "./pages/frequently-asked-questions/frequently-asked-questions.component";
import { WaterAccountCustomAttributesEditComponent } from "./pages/water-account-custom-attributes-edit/water-account-custom-attributes-edit.component";
import { ParcelCustomAttributesEditComponent } from "./pages/parcel-custom-attributes-edit/parcel-custom-attributes-edit.component";
import { ExampleMapComponent } from "./shared/components/leaflet/example-map/example-map.component";
import { AdminFrequentlyAskedQuestionsComponent } from "./pages/platform-admin/admin-frequently-asked-questions/admin-frequently-asked-questions.component";
import { AdminGeographiesComponent } from "./pages/platform-admin/admin-geographies/admin-geographies.component";
import { WaterAccountAdminPanelComponent } from "./pages/water-accounts/water-account-admin-panel/water-account-admin-panel.component";
import { ParcelDetailLayoutComponent } from "./pages/parcels/parcel-detail-layout/parcel-detail-layout.component";
import { WaterAccountRequestChangesComponent } from "./pages/water-accounts/water-account-request-changes/water-account-request-changes.component";
import { ParcelAdminPanelComponent } from "./pages/parcels/parcel-admin-panel/parcel-admin-panel.component";
import { ParcelDetailComponent } from "./pages/parcels/parcel-detail/parcel-detail.component";
import { GeographyMenuLayoutComponent } from "./pages/geography-menu/geography-menu-layout/geography-menu-layout.component";
import { GeographyAboutComponent } from "./pages/geography-menu/geography-about/geography-about.component";
import { GeographyAllocationsComponent } from "./pages/geography-menu/geography-allocations/geography-allocations.component";
import { GeographyGroundwaterLevelsComponent } from "./pages/geography-menu/geography-groundwater-levels/geography-groundwater-levels.component";
import { GeographySupportComponent } from "./pages/geography-menu/geography-support/geography-support.component";
import { RasterUploadComponent } from "./pages/supply-and-usage/water-measurement-supply-and-usage-menu/raster-upload/raster-upload.component";
import { ActivityCenterComponent } from "./pages/supply-and-usage/activity-center/activity-center.component";
import { AllocationPlanDetailComponent } from "./pages/supply-and-usage/allocation-plans/allocation-plan-detail/allocation-plan-detail.component";
import { AllocationPlansComponent } from "./pages/supply-and-usage/allocation-plans/allocation-plans.component";
import { MeterListComponent } from "./pages/supply-and-usage/meter-list/meter-list.component";
import { OpenetSyncIntegrationComponent } from "./pages/supply-and-usage/water-measurement-supply-and-usage-menu/openet-sync-integration/openet-sync-integration.component";
import { ParcelBulkActionsComponent } from "./pages/parcels/parcel-bulk-actions/parcel-bulk-actions.component";
import { ParcelsReviewChangesComponent } from "./pages/parcels/parcels-review-changes/parcels-review-changes.component";
import { ReferenceWellsListComponent } from "./pages/wells/reference-wells-list/reference-wells-list.component";
import { ReferenceWellsUploadComponent } from "./pages/wells/reference-wells-upload/reference-wells-upload.component";
import { ReviewSubmittedWellsComponent } from "./pages/wells/review-submitted-wells/review-submitted-wells.component";
import { SupplyAndUsageMenuLayoutComponent } from "./pages/supply-and-usage/supply-and-usage-menu-layout/supply-and-usage-menu-layout.component";
import { UpdateParcelsConfirmComponent } from "./pages/parcels/dashboard-update-parcels/update-parcels-confirm/update-parcels-confirm.component";
import { UpdateParcelsReviewComponent } from "./pages/parcels/dashboard-update-parcels/update-parcels-review/update-parcels-review.component";
import { UpdateParcelsUploadComponent } from "./pages/parcels/dashboard-update-parcels/update-parcels-upload/update-parcels-upload.component";
import { UploadUsageEntityGdbComponent } from "./pages/supply-and-usage/water-measurement-supply-and-usage-menu/upload-usage-entity-gdb/upload-usage-entity-gdb.component";
import { WaterMeasurementSupplyAndUsageMenu } from "./pages/supply-and-usage/water-measurement-supply-and-usage-menu/water-measurement-supply-and-usage-menu";
import { WaterAccountSuggestionsComponent } from "./pages/water-accounts/water-account-suggestions/water-account-suggestions.component";
import { WaterTransactionsBulkCreateComponent } from "./pages/supply-and-usage/water-transactions/bulk-create/water-transactions-bulk-create.component";
import { WaterTransactionsCreateComponent } from "./pages/supply-and-usage/water-transactions/create/water-transactions-create.component";
import { WaterTransactionsCsvUploadSupplyComponent } from "./pages/supply-and-usage/water-transactions/csv-upload-supply/water-transactions-csv-upload-supply.component";
import { WaterTransactionsCsvUploadUsageComponent } from "./pages/supply-and-usage/water-measurement-supply-and-usage-menu/csv-upload-usage/water-transactions-csv-upload-usage.component";
import { WaterTransactionsComponent } from "./pages/supply-and-usage/water-transactions/water-transactions.component";
import { WellBulkUploadComponent } from "./pages/wells/well-bulk-upload/well-bulk-upload.component";
import { SupportTicketListComponent } from "./pages/support-ticket-list/support-ticket-list.component";
import { WellDetailComponent } from "./pages/wells/well-detail/well-detail.component";
import { ZoneGroupDataUploaderComponent } from "./pages/parcels/zone-group-data-uploader/zone-group-data-uploader.component";
import { WaterAccountListComponent } from "./pages/water-dashboard/water-account-list/water-account-list.component";
import { ParcelListComponent } from "./pages/water-dashboard/parcel-list/parcel-list.component";
import { WellListComponent } from "./pages/water-dashboard/well-list/well-list.component";
import { WaterAccountDetailLayoutComponent } from "./pages/water-accounts/water-account-detail-layout/water-account-detail-layout.component";
import { AccountActivityComponent } from "./pages/water-accounts/account-activity/account-activity.component";
import { AccountAllocationPlansComponent } from "./pages/water-accounts/account-allocation-plans/account-allocation-plans.component";
import { WaterAccountParcelsComponent } from "./pages/water-accounts/water-account-parcels/water-account-parcels.component";
import { UsersAndSettingsComponent } from "./pages/water-accounts/users-and-settings/users-and-settings.component";
import { WaterBudgetComponent } from "./pages/water-accounts/water-budget/water-budget.component";
import { WaterAccountWellsComponent } from "./pages/water-accounts/water-account-wells/water-account-wells.component";
import { WellRegistrationListComponent } from "./pages/wells/well-registration-list/well-registration-list.component";
import { RequestSupportComponent } from "./pages/request-support/request-support.component";
import { FeeCalculatorComponent } from "./pages/fee-calculator/fee-calculator.component";
import { WellLocationEditComponent } from "./pages/wells/well-location-edit/well-location-edit.component";
import { WellIrrigatedParcelsEditComponent } from "./pages/wells/well-irrigated-parcels-edit/well-irrigated-parcels-edit.component";
import { SupportTicketInboxComponent } from "./pages/support-ticket-admin-pages/support-ticket-inbox/support-ticket-inbox.component";
import { NewsAndAnnouncementsComponent } from "./pages/news-and-announcements/news-and-announcements.component";
import { WaterMeasurementSelfReportListComponent } from "./pages/water-accounts/water-measurement-self-report-list/water-measurement-self-report-list.component";
import { WaterMeasurementSelfReportEditorComponent } from "./pages/water-accounts/water-measurement-self-report-editor/water-measurement-self-report-editor.component";
import { ReviewSelfReportComponent } from "./pages/review-self-report/review-self-report.component";
import { ZoneGroupConfigureComponent } from "./pages/configure/zone-group-configure/zone-group-configure.component";
import { WaterManagersConfigureComponent } from "./pages/configure/water-managers-configure/water-managers-configure.component";
import { AboutComponent } from "./pages/about/about.component";
import { ContactComponent } from "./pages/contact/contact.component";
import { HelpComponent } from "./pages/help/help.component";
import { LicenseComponent } from "./pages/license/license.component";

export const routeParams = {
    userID: "userID",
    fieldDefinitionID: "fieldDefinitionID",
    parcelID: "parcelID",
    tagID: "tagID",
    wellRegistrationID: "wellRegistrationID",
    wellID: "wellID",
    modelShortName: "modelID",
    scenarioShortName: "scenarioID",
    scenarioRunID: "scenarioRunID",
    geographyName: "geographyName",
    waterAccountID: "waterAccountID",
    externalMapLayerID: "externalMapLayerID",
    zoneGroupSlug: "zoneGroupSlug",
    zoneSlug: "zoneSlug",
    waterTypeSlug: "waterTypeSlug",
    supportTicketID: "supportTicketID",
    selfReportID: "selfReportID",
};

export const routes: Routes = [
    {
        path: "configure",
        title: "Configure Geography",
        component: ConfigureLayoutMenu,
        canActivate: [MsalGuard],
    },
    {
        path: `configure/:${routeParams.geographyName}`,
        component: ConfigureLayoutMenu,
        canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
        children: [
            { path: "allocation-plans", title: "Allocation Plans", component: AllocationPlansConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
            { path: "custom-attributes", component: CustomAttributesConfigureComponent, title: "Custom Attributes", canDeactivate: [UnsavedChangesGuard] },
            { path: "geospatial-data", title: "Geospatial Data Layers", component: GeospatialDataConfigureComponent },
            {
                path: "geospatial-data/create",
                title: "Create Geospatial Data Layer",
                component: GeospatialDataCreateComponent,
                data: { create: true },
                canDeactivate: [UnsavedChangesGuard],
            },
            {
                path: `geospatial-data/edit/:${routeParams.externalMapLayerID}`,
                title: "Edit Geospatial Data Layer",
                data: { create: false },
                component: GeospatialDataCreateComponent,
                canDeactivate: [UnsavedChangesGuard],
            },
            { path: "landing-page", title: "Landing Page", component: LandingPageConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
            { path: "meter-data", title: "Meter Data", component: MeterDataConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
            { path: "reporting-periods", title: "Reporting Periods", component: ReportingPeriodConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
            { path: "setup", component: GeographySetupComponent, title: "Setup", canDeactivate: [UnsavedChangesGuard] },
            { path: "water-managers", component: WaterManagersConfigureComponent, title: "Water Managers", canDeactivate: [UnsavedChangesGuard] },
            { path: "water-supply", title: "Water Supply", component: WaterSupplyConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
            { path: "well-registry", title: "Well Registry", component: WellRegistryConfigureComponent },
            { path: "zones", title: "Zones", component: ZoneGroupConfigureComponent, canDeactivate: [UnsavedChangesGuard] },
        ],
    },
    { path: "geographies", title: "Geographies", component: GeographiesComponent },
    {
        path: `geographies/:${routeParams.geographyName}`,
        title: "Geography",
        component: GeographyMenuLayoutComponent,

        children: [
            { path: "", redirectTo: "overview", pathMatch: "full" },
            {
                path: "allocation-plans",
                title: "Allocation Plans",
                component: GeographyAllocationsComponent,
                children: [
                    {
                        path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`,
                        title: "Allocation Plan",
                        component: AllocationPlanDetailComponent,
                        data: { editable: false },
                    },
                ],
            },
            {
                path: "groundwater-levels",
                title: "Groundwater Levels",
                component: GeographyGroundwaterLevelsComponent,
            },
            { path: "overview", title: "Geography Overview", component: GeographyAboutComponent },
            { path: "support", title: "Support", component: GeographySupportComponent },
        ],
    },
    {
        path: `:${routeParams.geographyName}/claim-water-accounts`,
        title: "Onboard",
        component: DashboardOnboardComponent,
        canActivate: [MsalGuard],
        children: [
            { path: "", redirectTo: "overview", pathMatch: "full" },
            { path: "enter-pin", title: "Water Account PINs", component: OnboardWaterAccountPINsComponent },
            { path: "overview", title: "Overview", component: OnboardOverviewComponent },
            { path: "review", title: "Claim Accounts", component: OnboardWaterAccountsComponent },
        ],
    },
    {
        path: "platform-admin",
        component: DashboardAdminComponent,
        title: "Platform Admin",
        canActivate: [MsalGuard, withFlagGuard(FlagEnum.IsSystemAdmin)],
        children: [
            { path: "", redirectTo: "geographies", pathMatch: "full" },
            { path: "frequently-asked-questions", component: AdminFrequentlyAskedQuestionsComponent, title: "Frequently Asked Questions" },
            { path: "geographies", component: AdminGeographiesComponent, title: "Geographies" },
            { path: "labels-and-definitions", component: FieldDefinitionListComponent, title: "Labels & Definitions" },
            {
                path: `labels-and-definitions/:${routeParams.fieldDefinitionID}`,
                component: FieldDefinitionEditComponent,
                canDeactivate: [UnsavedChangesGuard],
                title: "Edit Label Definition",
            },
            { path: "users", component: UserListComponent, title: "Users" },
            { path: `users/:${routeParams.userID}`, component: UserDetailComponent, title: "User Detail", data: { displayProfileEdit: false } },
        ],
    },
    {
        path: "profile",
        title: "User Profile",
        component: UserProfileComponent,
        canActivate: [MsalGuard],
        children: [{ path: "", title: "Details", component: UserDetailComponent, data: { displayProfileEdit: true } }],
    },
    {
        path: `review-self-reports/:${routeParams.geographyName}`,
        title: "Review Self Reports",
        component: ReviewSelfReportComponent,
        canActivate: [MsalGuard],
    },
    {
        path: "scenario-planner",
        title: "Scenario Planner",
        component: DashboardScenarioPlannerComponent,
        canActivate: [MsalGuard],
        children: [
            { path: "", redirectTo: "models", pathMatch: "full" },
            { path: "runs", title: "Scenario Runs", component: ScenarioPlannerAllScenarioRunsComponent },
            {
                path: "models",
                title: "Models",
                component: ScenarioPlannerIndexComponent,
                children: [
                    { path: `:${routeParams.modelShortName}`, title: "Model Details", component: ModelDetailComponent },

                    /* MK 10/10/2024 -- The route ordering matters for these next components. add-a-well/new and recharge/new need to be added before their corresponding :id routes.
                                        Otherwise the /new route will never be reached, and we'll try converting "new" to a number thus getting a 500 error from the server and seeing a blank screen*/
                    {
                        path: `:${routeParams.modelShortName}/add-a-well/new`,
                        title: "New Add a Well Scenario",
                        component: ScenarioRunAddAWellComponent,
                        canDeactivate: [UnsavedChangesGuard],
                    },
                    {
                        path: `:${routeParams.modelShortName}/add-a-well/:${routeParams.scenarioRunID}`,
                        title: "Add a Well Scenario Run Reults",
                        component: ScenarioRunDetailComponent,
                    },
                    {
                        path: `:${routeParams.modelShortName}/recharge/new`,
                        title: "New Recharge Scenario",
                        component: ScenarioRunRechargeComponent,
                        canDeactivate: [UnsavedChangesGuard],
                    },
                    {
                        path: `:${routeParams.modelShortName}/recharge/:${routeParams.scenarioRunID}`,
                        title: "Recharge Scenario Run Results",
                        component: ScenarioRunDetailComponent,
                    },
                ],
            },
        ],
    },
    {
        path: "supply-and-usage",
        component: SupplyAndUsageMenuLayoutComponent,
        canActivate: [MsalGuard],
    },
    {
        path: `supply-and-usage/:${routeParams.geographyName}`,
        title: "Supply and Usage Menu",
        component: SupplyAndUsageMenuLayoutComponent,
        canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
        children: [
            { path: "", redirectTo: "activity-center", pathMatch: "full" },
            { path: "activity-center", title: "Activity Center", component: ActivityCenterComponent },
            {
                path: "parcels/allocation-plans",
                title: "Allocation Plans",
                component: AllocationPlansComponent,
                children: [
                    {
                        path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`,
                        title: "Allocation Plan",
                        component: AllocationPlanDetailComponent,
                        data: { editable: true },
                    },
                ],
            },
            { path: "statistics", title: "Statistics", component: StatisticsComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
            {
                path: "water-account-budgets-report",
                title: "Water Account Budget Report",
                canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
                component: WaterAccountBudgetsReportComponent,
            },
            { path: "water-measurements", title: "Water Measurements", component: WaterMeasurementSupplyAndUsageMenu },
            { path: "water-measurements/parcels/upload-usage-entity-gdb", component: UploadUsageEntityGdbComponent, title: "Upload Usage Entity GDB" },
            { path: "water-measurements/openet-integration", title: "OpenET Integration", component: OpenetSyncIntegrationComponent },
            { path: "water-measurements/raster-upload", title: "Upload Raster", component: RasterUploadComponent },
            { path: "water-measurements/upload-water-measurements", title: "Upload Water Measurements", component: WaterTransactionsCsvUploadUsageComponent },
            { path: "water-supply", title: "Supply", component: WaterTransactionsComponent },
            { path: "water-supply/bulk-new", title: "Bulk Create Transactions", component: WaterTransactionsBulkCreateComponent },
            { path: "water-supply/csv-upload/supply", title: "Upload Supply", component: WaterTransactionsCsvUploadSupplyComponent },
            { path: "water-supply/new", title: "New Transactions", component: WaterTransactionsCreateComponent },
            { path: `water-supply/new/:${routeParams.parcelID}`, title: "New Parcel Transactions", component: WaterTransactionsCreateComponent },
            { path: "zones", title: "Zones", component: ZoneGroupListComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
            {
                path: `zones/:${routeParams.zoneGroupSlug}`,
                title: "Zone Group",
                component: ZoneGroupDetailComponent,
                canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
            },
        ],
    },
    {
        path: "parcels",
        children: [
            { path: "", redirectTo: "/water-dashboard/parcels", pathMatch: "full" },
            {
                path: `:${routeParams.geographyName}/bulk-actions`,
                title: "Bulk Parcel Actions",
                component: ParcelBulkActionsComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
            },
            {
                path: `:${routeParams.geographyName}/update`,
                title: "Update Parcels",
                component: ParcelsReviewChangesComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
            },
            {
                path: `:${routeParams.geographyName}/upload-parcel-data`,
                title: "Update Parcels",
                component: DashboardUpdateParcelsComponent,
                canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)],
                children: [
                    { path: "", redirectTo: "upload", pathMatch: "full" },
                    { path: "confirm", title: "Confirm Upload", component: UpdateParcelsConfirmComponent },
                    { path: "review-parcels", title: "Review Parcels", component: UpdateParcelsReviewComponent },
                    { path: "upload", title: "Upload Parcels", component: UpdateParcelsUploadComponent },
                ],
            },
            {
                path: `:${routeParams.geographyName}/zones/upload`,
                title: "Assign Parcels to Zones",
                component: ZoneGroupDataUploaderComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.ParcelRights, RightsEnum.Read)],
            },
            {
                path: `:${routeParams.parcelID}`,
                component: ParcelDetailLayoutComponent,
                canActivate: [MsalGuard],
                children: [
                    { path: "", redirectTo: "detail", pathMatch: "full" },
                    { path: "admin-panel", title: "Parcel Admin Panel", component: ParcelAdminPanelComponent, canActivate: [MsalGuard] },
                    { path: "admin-panel/edit-attributes", title: "Edit Attributes", component: ParcelCustomAttributesEditComponent, canDeactivate: [UnsavedChangesGuard] },
                    { path: "detail", title: "Parcel", component: ParcelDetailComponent, canActivate: [MsalGuard] },
                ],
            },
        ],
    },
    { path: "support-tickets", title: "Support Ticket", component: SupportTicketListComponent },
    { path: `support-tickets/:${routeParams.supportTicketID}`, title: "Inbox", component: SupportTicketInboxComponent },
    {
        path: "water-dashboard",
        title: "Water Dashboard",
        canActivate: [MsalGuard],
        children: [
            { path: "", redirectTo: "water-accounts", pathMatch: "full" },
            {
                path: "water-accounts",
                title: "Water Accounts",
                component: WaterAccountListComponent,
                canActivate: [MsalGuard],
            },
            {
                path: "parcels",
                title: "Parcels",
                component: ParcelListComponent,
                canActivate: [MsalGuard],
            },
            {
                path: "wells",
                title: "Wells",
                component: WellListComponent,
                canActivate: [MsalGuard],
            },
        ],
    },
    {
        path: `water-accounts`,
        title: "Water Accounts",
        children: [
            { path: "", redirectTo: "/water-dashboard/water-accounts", pathMatch: "full" },
            {
                path: `:${routeParams.geographyName}/suggestions`,
                title: "Water Account Suggestions",
                component: WaterAccountSuggestionsComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WaterAccountRights, RightsEnum.Read)],
                canDeactivate: [UnsavedChangesGuard],
            },
            { path: "request-changes", title: "Request Changes", component: WaterAccountRequestChangesComponent, canDeactivate: [UnsavedChangesGuard] },
            {
                path: `:${routeParams.waterAccountID}`,
                title: "Water Account",
                component: WaterAccountDetailLayoutComponent,
                canActivate: [MsalGuard],
                children: [
                    { path: "", redirectTo: "water-budget", pathMatch: "full" },
                    { path: "activity", title: "Activity", component: AccountActivityComponent },
                    { path: "admin-panel", title: "Water Account Admin Panel", component: WaterAccountAdminPanelComponent },
                    {
                        path: "allocation-plans",
                        title: "Allocation plans",
                        component: AccountAllocationPlansComponent,
                        children: [{ path: `:${routeParams.waterTypeSlug}/:${routeParams.zoneSlug}`, component: AllocationPlanDetailComponent, data: { editable: false } }],
                    },
                    { path: `admin-panel/edit-attributes`, title: "Edit Attributes", component: WaterAccountCustomAttributesEditComponent, canDeactivate: [UnsavedChangesGuard] },
                    { path: "parcels", title: "Parcels", component: WaterAccountParcelsComponent },
                    { path: `users-and-settings`, title: "Users & Settings", component: UsersAndSettingsComponent },
                    { path: "water-budget", title: "Water Budget", component: WaterBudgetComponent },
                    { path: "water-measurement-self-reports", title: "Self Reporting", component: WaterMeasurementSelfReportListComponent },
                    { path: "wells", title: "Wells", component: WaterAccountWellsComponent },
                ],
            },
        ],
    },
    {
        path: `water-accounts/:${routeParams.waterAccountID}/water-measurement-self-reports/:${routeParams.selfReportID}`,
        title: "Water Measurement Self Report",
        component: WaterMeasurementSelfReportEditorComponent,
        canActivate: [MsalGuard],
        canDeactivate: [UnsavedChangesGuard],
    },
    {
        path: "wells",
        children: [
            { path: "", redirectTo: "/water-dashboard/wells", pathMatch: "full" },
            { path: `:${routeParams.geographyName}/meters`, title: "Meters", component: MeterListComponent },
            {
                path: `:${routeParams.geographyName}/reference-wells`,
                title: "Reference Wells",
                component: ReferenceWellsListComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
            },
            { path: `:${routeParams.geographyName}/reference-wells/upload`, title: "Bulk Upload Reference Wells", component: ReferenceWellsUploadComponent },
            {
                path: `:${routeParams.geographyName}/review-submitted-wells`,
                title: "Review Submitted Wells",
                component: ReviewSubmittedWellsComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
            },
            { path: `:${routeParams.geographyName}/bulk-upload`, title: "Bulk Upload Wells", component: WellBulkUploadComponent },
            {
                path: `:${routeParams.geographyName}/well-registrations`,
                title: "Well Registrations",
                component: WellRegistrationListComponent,
                canActivate: [withGeographyRolePermissionGuard(PermissionEnum.WellRights, RightsEnum.Read)],
            },
            {
                path: `:${routeParams.geographyName}/well-registrations/:${routeParams.wellRegistrationID}`,
                title: "Well Registration",
                component: WellRegistrationDetailComponent,
            },
            {
                path: `:${routeParams.wellID}`,
                component: WellDetailComponent,
            },
            {
                path: `:${routeParams.wellID}/update-location`,
                component: WellLocationEditComponent,
            },
            {
                path: `:${routeParams.wellID}/update-irrigated-parcels`,
                component: WellIrrigatedParcelsEditComponent,
            },
        ],
    },
    {
        path: `well-registry/:${routeParams.geographyName}`,
        title: "Well Registry",
        canActivate: [MsalGuard, wellRegistryEnabledGuard],
        children: [
            { path: "", redirectTo: "new", pathMatch: "full" },
            {
                path: "new",
                title: "New Well",
                component: WellRegistryWorkflowComponent,
                data: { create: true },
                children: [
                    { path: "", redirectTo: "select-parcel", pathMatch: "full" },
                    { path: "select-parcel", title: "Select a Parcel", component: WellRegistrySelectParcelComponent },
                ],
            },
            {
                path: `well/:${routeParams.wellRegistrationID}/edit`,
                title: "Edit Well",
                component: WellRegistryWorkflowComponent,
                children: [
                    { path: "", redirectTo: "select-parcel", pathMatch: "full" },
                    { path: "attachments", title: "Attachments", component: WellAttachmentsComponent },
                    { path: "basic-information", title: "Basic Information", component: BasicWellInfoComponent, canDeactivate: [UnsavedChangesGuard] },
                    { path: "confirm-location", title: "Location", component: ConfirmWellLocationComponent },
                    { path: "contacts", title: "Contacts", component: WellContactsComponent, canDeactivate: [UnsavedChangesGuard] },
                    { path: "irrigated-parcels", title: "Location", component: IrrigatedParcelsEditComponent, canDeactivate: [UnsavedChangesGuard] },
                    { path: "location", title: "Location", component: WellLocationComponent },
                    { path: "select-parcel", title: "Select a Parcel", component: WellRegistrySelectParcelComponent },
                    { path: "submit", title: "Submit", component: SubmitComponent },
                    { path: "supporting-information", title: "Supporting Information", component: SupportingWellInfoComponent, canDeactivate: [UnsavedChangesGuard] },
                ],
            },
        ],
    },

    { path: `fee-calculator/:${routeParams.geographyName}`, title: "Fee Calculator", component: FeeCalculatorComponent },

    // Mostly static pages, alphabetized
    { path: "about", title: "About", component: AboutComponent },
    { path: "acknowledgements", title: "Acknowledgements", component: AcknowledgementsComponent },
    { path: "contact", title: "Contact", component: ContactComponent },
    { path: "create-user-callback", component: CreateUserCallbackComponent },
    { path: "example-map", title: "Example Map", component: ExampleMapComponent, canActivate: [MsalGuard, withGeographyFlagGuard(FlagEnum.HasManagerDashboard)] },
    { path: "frequently-asked-questions", title: "Frequently Asked Questions", component: FrequentlyAskedQuestionsComponent },
    { path: "getting-started", title: "Getting Started", component: GettingStartedComponent },
    { path: "grower-guide", title: "Grower Guide", component: GrowerGuideComponent },
    { path: "help", title: "Help", component: HelpComponent },
    { path: "license", title: "License", component: LicenseComponent },
    { path: "manager-guide", title: "Manager Guide", component: ManagerGuideComponent },
    { path: "news-and-announcements", title: "News & Announcements", component: NewsAndAnnouncementsComponent },
    { path: "not-found", title: "Page Not Found", component: NotFoundComponent },
    { path: "request-support", title: "Request Support", component: RequestSupportComponent },
    { path: "signin-oidc", component: LoginCallbackComponent },
    { path: "style-guide", title: "Style Guide", component: StyleGuideComponent },
    { path: "subscription-insufficient", title: "Insufficient Priveleges", component: SubscriptionInsufficientComponent },
    { path: "unauthenticated", title: "Unauthenticated", component: UnauthenticatedComponent },

    // DO NOT REORDER THESE THAT WILL CAUSE THEM TO NOT WORK
    { path: "", component: HomeIndexComponent },
    { path: `:${routeParams.geographyName}`, component: GeographyLandingPageComponent, canActivate: [landingPageEnabledGuard] },
    { path: "**", title: "Page Not Found", component: NotFoundComponent },
];
