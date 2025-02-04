//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomRichTextType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class CustomRichTextType : IHavePrimaryKey
    {
        public static readonly CustomRichTextTypePlatformOverview PlatformOverview = CustomRichTextTypePlatformOverview.Instance;
        public static readonly CustomRichTextTypeDisclaimer Disclaimer = CustomRichTextTypeDisclaimer.Instance;
        public static readonly CustomRichTextTypeHomepage Homepage = CustomRichTextTypeHomepage.Instance;
        public static readonly CustomRichTextTypeHelp Help = CustomRichTextTypeHelp.Instance;
        public static readonly CustomRichTextTypeLabelsAndDefinitionsList LabelsAndDefinitionsList = CustomRichTextTypeLabelsAndDefinitionsList.Instance;
        public static readonly CustomRichTextTypeManageUserParcels ManageUserParcels = CustomRichTextTypeManageUserParcels.Instance;
        public static readonly CustomRichTextTypeTraining Training = CustomRichTextTypeTraining.Instance;
        public static readonly CustomRichTextTypeCustomPages CustomPages = CustomRichTextTypeCustomPages.Instance;
        public static readonly CustomRichTextTypeMailMergeReport MailMergeReport = CustomRichTextTypeMailMergeReport.Instance;
        public static readonly CustomRichTextTypeParcelList ParcelList = CustomRichTextTypeParcelList.Instance;
        public static readonly CustomRichTextTypeOnboardOverview OnboardOverview = CustomRichTextTypeOnboardOverview.Instance;
        public static readonly CustomRichTextTypeOnboardWaterAccountPINs OnboardWaterAccountPINs = CustomRichTextTypeOnboardWaterAccountPINs.Instance;
        public static readonly CustomRichTextTypeOnboardClaimParcels OnboardClaimParcels = CustomRichTextTypeOnboardClaimParcels.Instance;
        public static readonly CustomRichTextTypeWellRegistryIntro WellRegistryIntro = CustomRichTextTypeWellRegistryIntro.Instance;
        public static readonly CustomRichTextTypeTagList TagList = CustomRichTextTypeTagList.Instance;
        public static readonly CustomRichTextTypeBulkTagParcels BulkTagParcels = CustomRichTextTypeBulkTagParcels.Instance;
        public static readonly CustomRichTextTypeWaterAccounts WaterAccounts = CustomRichTextTypeWaterAccounts.Instance;
        public static readonly CustomRichTextTypeWaterTransactions WaterTransactions = CustomRichTextTypeWaterTransactions.Instance;
        public static readonly CustomRichTextTypeWaterTransactionCreate WaterTransactionCreate = CustomRichTextTypeWaterTransactionCreate.Instance;
        public static readonly CustomRichTextTypeWaterTransactionBulkCreate WaterTransactionBulkCreate = CustomRichTextTypeWaterTransactionBulkCreate.Instance;
        public static readonly CustomRichTextTypeWaterTransactionCSVUploadSupply WaterTransactionCSVUploadSupply = CustomRichTextTypeWaterTransactionCSVUploadSupply.Instance;
        public static readonly CustomRichTextTypeWaterTransactionHistory WaterTransactionHistory = CustomRichTextTypeWaterTransactionHistory.Instance;
        public static readonly CustomRichTextTypeWaterAccount WaterAccount = CustomRichTextTypeWaterAccount.Instance;
        public static readonly CustomRichTextTypeTag Tag = CustomRichTextTypeTag.Instance;
        public static readonly CustomRichTextTypeWaterAccountPIN WaterAccountPIN = CustomRichTextTypeWaterAccountPIN.Instance;
        public static readonly CustomRichTextTypeSupplyType SupplyType = CustomRichTextTypeSupplyType.Instance;
        public static readonly CustomRichTextTypeTotalSupply TotalSupply = CustomRichTextTypeTotalSupply.Instance;
        public static readonly CustomRichTextTypeTotalUsage TotalUsage = CustomRichTextTypeTotalUsage.Instance;
        public static readonly CustomRichTextTypeTransactionType TransactionType = CustomRichTextTypeTransactionType.Instance;
        public static readonly CustomRichTextTypeEffectiveDate EffectiveDate = CustomRichTextTypeEffectiveDate.Instance;
        public static readonly CustomRichTextTypeReportingPeriodConfiguration ReportingPeriodConfiguration = CustomRichTextTypeReportingPeriodConfiguration.Instance;
        public static readonly CustomRichTextTypeWaterSupplyConfiguration WaterSupplyConfiguration = CustomRichTextTypeWaterSupplyConfiguration.Instance;
        public static readonly CustomRichTextTypeWaterLevelsConfiguration WaterLevelsConfiguration = CustomRichTextTypeWaterLevelsConfiguration.Instance;
        public static readonly CustomRichTextTypeTradingConfiguration TradingConfiguration = CustomRichTextTypeTradingConfiguration.Instance;
        public static readonly CustomRichTextTypeScenariosConfiguration ScenariosConfiguration = CustomRichTextTypeScenariosConfiguration.Instance;
        public static readonly CustomRichTextTypeWellRegistryConfiguration WellRegistryConfiguration = CustomRichTextTypeWellRegistryConfiguration.Instance;
        public static readonly CustomRichTextTypePermissionsConfiguration PermissionsConfiguration = CustomRichTextTypePermissionsConfiguration.Instance;
        public static readonly CustomRichTextTypeGeospatialDataConfiguration GeospatialDataConfiguration = CustomRichTextTypeGeospatialDataConfiguration.Instance;
        public static readonly CustomRichTextTypeCustomPagesConfiguration CustomPagesConfiguration = CustomRichTextTypeCustomPagesConfiguration.Instance;
        public static readonly CustomRichTextTypeUpdateParcelsUpload UpdateParcelsUpload = CustomRichTextTypeUpdateParcelsUpload.Instance;
        public static readonly CustomRichTextTypeUpdateParcelsReviewParcels UpdateParcelsReviewParcels = CustomRichTextTypeUpdateParcelsReviewParcels.Instance;
        public static readonly CustomRichTextTypeUpdateParcelsConfirm UpdateParcelsConfirm = CustomRichTextTypeUpdateParcelsConfirm.Instance;
        public static readonly CustomRichTextTypeAccountReconciliation AccountReconciliation = CustomRichTextTypeAccountReconciliation.Instance;
        public static readonly CustomRichTextTypeFooter Footer = CustomRichTextTypeFooter.Instance;
        public static readonly CustomRichTextTypeEditAccounts EditAccounts = CustomRichTextTypeEditAccounts.Instance;
        public static readonly CustomRichTextTypeEditUsers EditUsers = CustomRichTextTypeEditUsers.Instance;
        public static readonly CustomRichTextTypeAccountActivity AccountActivity = CustomRichTextTypeAccountActivity.Instance;
        public static readonly CustomRichTextTypeUsageByParcel UsageByParcel = CustomRichTextTypeUsageByParcel.Instance;
        public static readonly CustomRichTextTypeAccountMap AccountMap = CustomRichTextTypeAccountMap.Instance;
        public static readonly CustomRichTextTypeChangeParcelOwnership ChangeParcelOwnership = CustomRichTextTypeChangeParcelOwnership.Instance;
        public static readonly CustomRichTextTypeWellBulkUpload WellBulkUpload = CustomRichTextTypeWellBulkUpload.Instance;
        public static readonly CustomRichTextTypeWellName WellName = CustomRichTextTypeWellName.Instance;
        public static readonly CustomRichTextTypeCountyWellPermitNo CountyWellPermitNo = CustomRichTextTypeCountyWellPermitNo.Instance;
        public static readonly CustomRichTextTypeStateWCRNo StateWCRNo = CustomRichTextTypeStateWCRNo.Instance;
        public static readonly CustomRichTextTypeWellDepth WellDepth = CustomRichTextTypeWellDepth.Instance;
        public static readonly CustomRichTextTypeDateDrilled DateDrilled = CustomRichTextTypeDateDrilled.Instance;
        public static readonly CustomRichTextTypeWaterTransactionCsvUploadUsage WaterTransactionCsvUploadUsage = CustomRichTextTypeWaterTransactionCsvUploadUsage.Instance;
        public static readonly CustomRichTextTypeAPNColumn APNColumn = CustomRichTextTypeAPNColumn.Instance;
        public static readonly CustomRichTextTypeValueColumn ValueColumn = CustomRichTextTypeValueColumn.Instance;
        public static readonly CustomRichTextTypeUsageType UsageType = CustomRichTextTypeUsageType.Instance;
        public static readonly CustomRichTextTypeWells Wells = CustomRichTextTypeWells.Instance;
        public static readonly CustomRichTextTypeExternalMapLayers ExternalMapLayers = CustomRichTextTypeExternalMapLayers.Instance;
        public static readonly CustomRichTextTypeExternalMapLayersType ExternalMapLayersType = CustomRichTextTypeExternalMapLayersType.Instance;
        public static readonly CustomRichTextTypeExternalMapLayersEdit ExternalMapLayersEdit = CustomRichTextTypeExternalMapLayersEdit.Instance;
        public static readonly CustomRichTextTypeExternalMapLayersName ExternalMapLayersName = CustomRichTextTypeExternalMapLayersName.Instance;
        public static readonly CustomRichTextTypeExternalMapLayersMinimumZoom ExternalMapLayersMinimumZoom = CustomRichTextTypeExternalMapLayersMinimumZoom.Instance;
        public static readonly CustomRichTextTypeOurGeographies OurGeographies = CustomRichTextTypeOurGeographies.Instance;
        public static readonly CustomRichTextTypePopUpField PopUpField = CustomRichTextTypePopUpField.Instance;
        public static readonly CustomRichTextTypeOpenETSyncIntegration OpenETSyncIntegration = CustomRichTextTypeOpenETSyncIntegration.Instance;
        public static readonly CustomRichTextTypeOpenETInstructions OpenETInstructions = CustomRichTextTypeOpenETInstructions.Instance;
        public static readonly CustomRichTextTypeUsageEstimates UsageEstimates = CustomRichTextTypeUsageEstimates.Instance;
        public static readonly CustomRichTextTypeLastSuccessfulSyncDate LastSuccessfulSyncDate = CustomRichTextTypeLastSuccessfulSyncDate.Instance;
        public static readonly CustomRichTextTypeLastSyncDate LastSyncDate = CustomRichTextTypeLastSyncDate.Instance;
        public static readonly CustomRichTextTypeDateFinalized DateFinalized = CustomRichTextTypeDateFinalized.Instance;
        public static readonly CustomRichTextTypeEstimateDate EstimateDate = CustomRichTextTypeEstimateDate.Instance;
        public static readonly CustomRichTextTypeZoneGroupsEdit ZoneGroupsEdit = CustomRichTextTypeZoneGroupsEdit.Instance;
        public static readonly CustomRichTextTypeZoneGroupConfiguration ZoneGroupConfiguration = CustomRichTextTypeZoneGroupConfiguration.Instance;
        public static readonly CustomRichTextTypeZoneGroupCSVUploader ZoneGroupCSVUploader = CustomRichTextTypeZoneGroupCSVUploader.Instance;
        public static readonly CustomRichTextTypeZoneColumn ZoneColumn = CustomRichTextTypeZoneColumn.Instance;
        public static readonly CustomRichTextTypeZoneGroupList ZoneGroupList = CustomRichTextTypeZoneGroupList.Instance;
        public static readonly CustomRichTextTypeHomeAboutCopy HomeAboutCopy = CustomRichTextTypeHomeAboutCopy.Instance;
        public static readonly CustomRichTextTypeWaterAccountBudgetReport WaterAccountBudgetReport = CustomRichTextTypeWaterAccountBudgetReport.Instance;
        public static readonly CustomRichTextTypeCustomPageEditProperties CustomPageEditProperties = CustomRichTextTypeCustomPageEditProperties.Instance;
        public static readonly CustomRichTextTypeZoneGroupUsageChart ZoneGroupUsageChart = CustomRichTextTypeZoneGroupUsageChart.Instance;
        public static readonly CustomRichTextTypeModalUpdateWaterAccountInformation ModalUpdateWaterAccountInformation = CustomRichTextTypeModalUpdateWaterAccountInformation.Instance;
        public static readonly CustomRichTextTypeModalCreateNewWaterAccount ModalCreateNewWaterAccount = CustomRichTextTypeModalCreateNewWaterAccount.Instance;
        public static readonly CustomRichTextTypeModalAddParcelToWaterAccount ModalAddParcelToWaterAccount = CustomRichTextTypeModalAddParcelToWaterAccount.Instance;
        public static readonly CustomRichTextTypeModalMergeWaterAccounts ModalMergeWaterAccounts = CustomRichTextTypeModalMergeWaterAccounts.Instance;
        public static readonly CustomRichTextTypeModalUpdateWaterAccountParcels ModalUpdateWaterAccountParcels = CustomRichTextTypeModalUpdateWaterAccountParcels.Instance;
        public static readonly CustomRichTextTypeWaterAccountMergeType WaterAccountMergeType = CustomRichTextTypeWaterAccountMergeType.Instance;
        public static readonly CustomRichTextTypeGeographyAbout GeographyAbout = CustomRichTextTypeGeographyAbout.Instance;
        public static readonly CustomRichTextTypeGeographyAllocations GeographyAllocations = CustomRichTextTypeGeographyAllocations.Instance;
        public static readonly CustomRichTextTypeGeographyWaterLevels GeographyWaterLevels = CustomRichTextTypeGeographyWaterLevels.Instance;
        public static readonly CustomRichTextTypeGeographySupport GeographySupport = CustomRichTextTypeGeographySupport.Instance;
        public static readonly CustomRichTextTypeMonitoringWellsGrid MonitoringWellsGrid = CustomRichTextTypeMonitoringWellsGrid.Instance;
        public static readonly CustomRichTextTypeWaterDashboardWaterAccounts WaterDashboardWaterAccounts = CustomRichTextTypeWaterDashboardWaterAccounts.Instance;
        public static readonly CustomRichTextTypeAddAWellScenario AddAWellScenario = CustomRichTextTypeAddAWellScenario.Instance;
        public static readonly CustomRichTextTypeAllocationPlanEdit AllocationPlanEdit = CustomRichTextTypeAllocationPlanEdit.Instance;
        public static readonly CustomRichTextTypeAllocationPlansConfigure AllocationPlansConfigure = CustomRichTextTypeAllocationPlansConfigure.Instance;
        public static readonly CustomRichTextTypeCloneAllocationPlan CloneAllocationPlan = CustomRichTextTypeCloneAllocationPlan.Instance;
        public static readonly CustomRichTextTypeAccountAllocationPlans AccountAllocationPlans = CustomRichTextTypeAccountAllocationPlans.Instance;
        public static readonly CustomRichTextTypeRechargeScenario RechargeScenario = CustomRichTextTypeRechargeScenario.Instance;
        public static readonly CustomRichTextTypePostToSupply PostToSupply = CustomRichTextTypePostToSupply.Instance;
        public static readonly CustomRichTextTypeOpenETSyncVariable OpenETSyncVariable = CustomRichTextTypeOpenETSyncVariable.Instance;
        public static readonly CustomRichTextTypeLandownerParcelIndex LandownerParcelIndex = CustomRichTextTypeLandownerParcelIndex.Instance;
        public static readonly CustomRichTextTypeScenarioPlanner ScenarioPlanner = CustomRichTextTypeScenarioPlanner.Instance;
        public static readonly CustomRichTextTypeScenarioPlannerGET ScenarioPlannerGET = CustomRichTextTypeScenarioPlannerGET.Instance;
        public static readonly CustomRichTextTypeScenarioPlannerScenarioRuns ScenarioPlannerScenarioRuns = CustomRichTextTypeScenarioPlannerScenarioRuns.Instance;
        public static readonly CustomRichTextTypeActivityCenter ActivityCenter = CustomRichTextTypeActivityCenter.Instance;
        public static readonly CustomRichTextTypeWaterAccountSuggestions WaterAccountSuggestions = CustomRichTextTypeWaterAccountSuggestions.Instance;
        public static readonly CustomRichTextTypeModalReviewWaterAccountSuggestion ModalReviewWaterAccountSuggestion = CustomRichTextTypeModalReviewWaterAccountSuggestion.Instance;
        public static readonly CustomRichTextTypeWellRegistryConfigurationPage WellRegistryConfigurationPage = CustomRichTextTypeWellRegistryConfigurationPage.Instance;
        public static readonly CustomRichTextTypeWellRegistryMapYourWell WellRegistryMapYourWell = CustomRichTextTypeWellRegistryMapYourWell.Instance;
        public static readonly CustomRichTextTypeWellRegistryConfirmWellLocation WellRegistryConfirmWellLocation = CustomRichTextTypeWellRegistryConfirmWellLocation.Instance;
        public static readonly CustomRichTextTypeWellRegistryIrrigatedParcels WellRegistryIrrigatedParcels = CustomRichTextTypeWellRegistryIrrigatedParcels.Instance;
        public static readonly CustomRichTextTypeWellRegistryContacts WellRegistryContacts = CustomRichTextTypeWellRegistryContacts.Instance;
        public static readonly CustomRichTextTypeWellRegistryBasicInformation WellRegistryBasicInformation = CustomRichTextTypeWellRegistryBasicInformation.Instance;
        public static readonly CustomRichTextTypeWellRegistrySupportingInformation WellRegistrySupportingInformation = CustomRichTextTypeWellRegistrySupportingInformation.Instance;
        public static readonly CustomRichTextTypeWellRegistryAttachments WellRegistryAttachments = CustomRichTextTypeWellRegistryAttachments.Instance;
        public static readonly CustomRichTextTypeWellRegistrySubmit WellRegistrySubmit = CustomRichTextTypeWellRegistrySubmit.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWellName WellRegistryFieldWellName = CustomRichTextTypeWellRegistryFieldWellName.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldSWN WellRegistryFieldSWN = CustomRichTextTypeWellRegistryFieldSWN.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWCR WellRegistryFieldWCR = CustomRichTextTypeWellRegistryFieldWCR.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldCountyWellPermit WellRegistryFieldCountyWellPermit = CustomRichTextTypeWellRegistryFieldCountyWellPermit.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldDateDrilled WellRegistryFieldDateDrilled = CustomRichTextTypeWellRegistryFieldDateDrilled.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural WellRegistryFieldWaterUseDescriptionAgricultural = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering WellRegistryFieldWaterUseDescriptionStockWatering = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic WellRegistryFieldWaterUseDescriptionDomestic = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal WellRegistryFieldWaterUseDescriptionPublicMunicipal = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal WellRegistryFieldWaterUseDescriptionPrivateMunicipal = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther WellRegistryFieldWaterUseDescriptionOther = CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldTopOfPerforations WellRegistryFieldTopOfPerforations = CustomRichTextTypeWellRegistryFieldTopOfPerforations.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldBottomOfPerforations WellRegistryFieldBottomOfPerforations = CustomRichTextTypeWellRegistryFieldBottomOfPerforations.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldMaxiumumFlow WellRegistryFieldMaxiumumFlow = CustomRichTextTypeWellRegistryFieldMaxiumumFlow.Instance;
        public static readonly CustomRichTextTypeWellRegistryFieldTypicalFlow WellRegistryFieldTypicalFlow = CustomRichTextTypeWellRegistryFieldTypicalFlow.Instance;
        public static readonly CustomRichTextTypeManageReviewSubmittedWells ManageReviewSubmittedWells = CustomRichTextTypeManageReviewSubmittedWells.Instance;
        public static readonly CustomRichTextTypeLandownerWellList LandownerWellList = CustomRichTextTypeLandownerWellList.Instance;
        public static readonly CustomRichTextTypeManageAllWellRegistrations ManageAllWellRegistrations = CustomRichTextTypeManageAllWellRegistrations.Instance;
        public static readonly CustomRichTextTypeFormAsteriskExplanation FormAsteriskExplanation = CustomRichTextTypeFormAsteriskExplanation.Instance;
        public static readonly CustomRichTextTypeWellRegistryIncompleteText WellRegistryIncompleteText = CustomRichTextTypeWellRegistryIncompleteText.Instance;
        public static readonly CustomRichTextTypeReferenceWellsList ReferenceWellsList = CustomRichTextTypeReferenceWellsList.Instance;
        public static readonly CustomRichTextTypeReferenceWellsUploader ReferenceWellsUploader = CustomRichTextTypeReferenceWellsUploader.Instance;
        public static readonly CustomRichTextTypeLandingPageBody LandingPageBody = CustomRichTextTypeLandingPageBody.Instance;
        public static readonly CustomRichTextTypeLandingPageUserCard LandingPageUserCard = CustomRichTextTypeLandingPageUserCard.Instance;
        public static readonly CustomRichTextTypeLandingPageParcelCard LandingPageParcelCard = CustomRichTextTypeLandingPageParcelCard.Instance;
        public static readonly CustomRichTextTypeLandingPageWellCard LandingPageWellCard = CustomRichTextTypeLandingPageWellCard.Instance;
        public static readonly CustomRichTextTypeLandingPageWaterAccountCard LandingPageWaterAccountCard = CustomRichTextTypeLandingPageWaterAccountCard.Instance;
        public static readonly CustomRichTextTypeLandingPageOverview LandingPageOverview = CustomRichTextTypeLandingPageOverview.Instance;
        public static readonly CustomRichTextTypeLandingPageAllocationPlans LandingPageAllocationPlans = CustomRichTextTypeLandingPageAllocationPlans.Instance;
        public static readonly CustomRichTextTypeLandingPageWaterLevels LandingPageWaterLevels = CustomRichTextTypeLandingPageWaterLevels.Instance;
        public static readonly CustomRichTextTypeLandingPageContact LandingPageContact = CustomRichTextTypeLandingPageContact.Instance;
        public static readonly CustomRichTextTypeConfigureLandingPage ConfigureLandingPage = CustomRichTextTypeConfigureLandingPage.Instance;
        public static readonly CustomRichTextTypeHomepageUpdateProfileLink HomepageUpdateProfileLink = CustomRichTextTypeHomepageUpdateProfileLink.Instance;
        public static readonly CustomRichTextTypeHomepageGrowerGuideLink HomepageGrowerGuideLink = CustomRichTextTypeHomepageGrowerGuideLink.Instance;
        public static readonly CustomRichTextTypeHomepageGeographiesLink HomepageGeographiesLink = CustomRichTextTypeHomepageGeographiesLink.Instance;
        public static readonly CustomRichTextTypeHomepageClaimWaterAccountsPanel HomepageClaimWaterAccountsPanel = CustomRichTextTypeHomepageClaimWaterAccountsPanel.Instance;
        public static readonly CustomRichTextTypeParcelStatus ParcelStatus = CustomRichTextTypeParcelStatus.Instance;
        public static readonly CustomRichTextTypeOnboardOverviewContent OnboardOverviewContent = CustomRichTextTypeOnboardOverviewContent.Instance;
        public static readonly CustomRichTextTypeParcelBulkActions ParcelBulkActions = CustomRichTextTypeParcelBulkActions.Instance;
        public static readonly CustomRichTextTypeMeterList MeterList = CustomRichTextTypeMeterList.Instance;
        public static readonly CustomRichTextTypeSerialNumber SerialNumber = CustomRichTextTypeSerialNumber.Instance;
        public static readonly CustomRichTextTypeMeterConfiguration MeterConfiguration = CustomRichTextTypeMeterConfiguration.Instance;
        public static readonly CustomRichTextTypeAcknowledgements Acknowledgements = CustomRichTextTypeAcknowledgements.Instance;
        public static readonly CustomRichTextTypeAdminGeographyEditForm AdminGeographyEditForm = CustomRichTextTypeAdminGeographyEditForm.Instance;
        public static readonly CustomRichTextTypeLandingPageConfigure LandingPageConfigure = CustomRichTextTypeLandingPageConfigure.Instance;
        public static readonly CustomRichTextTypeMeterDataConfigure MeterDataConfigure = CustomRichTextTypeMeterDataConfigure.Instance;
        public static readonly CustomRichTextTypeAllocationPlanConfigureCard AllocationPlanConfigureCard = CustomRichTextTypeAllocationPlanConfigureCard.Instance;
        public static readonly CustomRichTextTypeWellStatus WellStatus = CustomRichTextTypeWellStatus.Instance;
        public static readonly CustomRichTextTypeUpdateWellInfo UpdateWellInfo = CustomRichTextTypeUpdateWellInfo.Instance;
        public static readonly CustomRichTextTypeUpdateWellLocation UpdateWellLocation = CustomRichTextTypeUpdateWellLocation.Instance;
        public static readonly CustomRichTextTypeUpdateWellIrrigatedParcels UpdateWellIrrigatedParcels = CustomRichTextTypeUpdateWellIrrigatedParcels.Instance;
        public static readonly CustomRichTextTypeAdminFAQ AdminFAQ = CustomRichTextTypeAdminFAQ.Instance;
        public static readonly CustomRichTextTypeGeneralFAQ GeneralFAQ = CustomRichTextTypeGeneralFAQ.Instance;
        public static readonly CustomRichTextTypeWaterAccountCustomAttributesEdit WaterAccountCustomAttributesEdit = CustomRichTextTypeWaterAccountCustomAttributesEdit.Instance;
        public static readonly CustomRichTextTypeParcelCustomAttributesEdit ParcelCustomAttributesEdit = CustomRichTextTypeParcelCustomAttributesEdit.Instance;
        public static readonly CustomRichTextTypeWaterDashboardLink WaterDashboardLink = CustomRichTextTypeWaterDashboardLink.Instance;
        public static readonly CustomRichTextTypeWaterManagerGuideLink WaterManagerGuideLink = CustomRichTextTypeWaterManagerGuideLink.Instance;
        public static readonly CustomRichTextTypeReviewParcelChanges ReviewParcelChanges = CustomRichTextTypeReviewParcelChanges.Instance;
        public static readonly CustomRichTextTypeWaterAccountRequestChanges WaterAccountRequestChanges = CustomRichTextTypeWaterAccountRequestChanges.Instance;
        public static readonly CustomRichTextTypeWaterAccountRequestChangesCertification WaterAccountRequestChangesCertification = CustomRichTextTypeWaterAccountRequestChangesCertification.Instance;
        public static readonly CustomRichTextTypeConsolidateWaterAccountsDisclaimer ConsolidateWaterAccountsDisclaimer = CustomRichTextTypeConsolidateWaterAccountsDisclaimer.Instance;
        public static readonly CustomRichTextTypeKernScenarioModel KernScenarioModel = CustomRichTextTypeKernScenarioModel.Instance;
        public static readonly CustomRichTextTypeMercedWaterResourcesModel MercedWaterResourcesModel = CustomRichTextTypeMercedWaterResourcesModel.Instance;
        public static readonly CustomRichTextTypeYoloScenarioModel YoloScenarioModel = CustomRichTextTypeYoloScenarioModel.Instance;
        public static readonly CustomRichTextTypeConfigureGeographySetup ConfigureGeographySetup = CustomRichTextTypeConfigureGeographySetup.Instance;
        public static readonly CustomRichTextTypeConfigureCustomAttributes ConfigureCustomAttributes = CustomRichTextTypeConfigureCustomAttributes.Instance;
        public static readonly CustomRichTextTypeConfigureWaterManagers ConfigureWaterManagers = CustomRichTextTypeConfigureWaterManagers.Instance;
        public static readonly CustomRichTextTypeRasterUploadGuidance RasterUploadGuidance = CustomRichTextTypeRasterUploadGuidance.Instance;
        public static readonly CustomRichTextTypeWaterDashboardParcels WaterDashboardParcels = CustomRichTextTypeWaterDashboardParcels.Instance;
        public static readonly CustomRichTextTypeWaterDashboardWells WaterDashboardWells = CustomRichTextTypeWaterDashboardWells.Instance;
        public static readonly CustomRichTextTypeRequestSupport RequestSupport = CustomRichTextTypeRequestSupport.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorStepOne FeeCalculatorStepOne = CustomRichTextTypeFeeCalculatorStepOne.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorSurfaceWater FeeCalculatorSurfaceWater = CustomRichTextTypeFeeCalculatorSurfaceWater.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorStepTwo FeeCalculatorStepTwo = CustomRichTextTypeFeeCalculatorStepTwo.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorStepThree FeeCalculatorStepThree = CustomRichTextTypeFeeCalculatorStepThree.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorAboutFeeStructures FeeCalculatorAboutFeeStructures = CustomRichTextTypeFeeCalculatorAboutFeeStructures.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorIncentivePayment FeeCalculatorIncentivePayment = CustomRichTextTypeFeeCalculatorIncentivePayment.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater FeeCalculatorWhatIsConsumedGroundwater = CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorYourData FeeCalculatorYourData = CustomRichTextTypeFeeCalculatorYourData.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorAcres FeeCalculatorAcres = CustomRichTextTypeFeeCalculatorAcres.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorFallowingSelfDirected FeeCalculatorFallowingSelfDirected = CustomRichTextTypeFeeCalculatorFallowingSelfDirected.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected FeeCalculatorCoverCroppingSelfDirected = CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram FeeCalculatorTemporaryFallowingLandFallowingProgram = CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP FeeCalculatorRotationalExtendedFallowingMLRP = CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP = CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP FeeCalculatorOrchardSwaleRewildingMLRP = CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP = CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP = CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP.Instance;
        public static readonly CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP FeeCalculatorStorageOrRechargeBasinsMLRP = CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP.Instance;
        public static readonly CustomRichTextTypeLandingPageFeeCalculator LandingPageFeeCalculator = CustomRichTextTypeLandingPageFeeCalculator.Instance;
        public static readonly CustomRichTextTypeNewsAndAnnouncements NewsAndAnnouncements = CustomRichTextTypeNewsAndAnnouncements.Instance;
        public static readonly CustomRichTextTypeReviewSelfReportList ReviewSelfReportList = CustomRichTextTypeReviewSelfReportList.Instance;
        public static readonly CustomRichTextTypeSubmitSelfReportDisclaimer SubmitSelfReportDisclaimer = CustomRichTextTypeSubmitSelfReportDisclaimer.Instance;
        public static readonly CustomRichTextTypeSelfReportEditorInstructions SelfReportEditorInstructions = CustomRichTextTypeSelfReportEditorInstructions.Instance;

        public static readonly List<CustomRichTextType> All;
        public static readonly List<CustomRichTextTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, CustomRichTextType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, CustomRichTextTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static CustomRichTextType()
        {
            All = new List<CustomRichTextType> { PlatformOverview, Disclaimer, Homepage, Help, LabelsAndDefinitionsList, ManageUserParcels, Training, CustomPages, MailMergeReport, ParcelList, OnboardOverview, OnboardWaterAccountPINs, OnboardClaimParcels, WellRegistryIntro, TagList, BulkTagParcels, WaterAccounts, WaterTransactions, WaterTransactionCreate, WaterTransactionBulkCreate, WaterTransactionCSVUploadSupply, WaterTransactionHistory, WaterAccount, Tag, WaterAccountPIN, SupplyType, TotalSupply, TotalUsage, TransactionType, EffectiveDate, ReportingPeriodConfiguration, WaterSupplyConfiguration, WaterLevelsConfiguration, TradingConfiguration, ScenariosConfiguration, WellRegistryConfiguration, PermissionsConfiguration, GeospatialDataConfiguration, CustomPagesConfiguration, UpdateParcelsUpload, UpdateParcelsReviewParcels, UpdateParcelsConfirm, AccountReconciliation, Footer, EditAccounts, EditUsers, AccountActivity, UsageByParcel, AccountMap, ChangeParcelOwnership, WellBulkUpload, WellName, CountyWellPermitNo, StateWCRNo, WellDepth, DateDrilled, WaterTransactionCsvUploadUsage, APNColumn, ValueColumn, UsageType, Wells, ExternalMapLayers, ExternalMapLayersType, ExternalMapLayersEdit, ExternalMapLayersName, ExternalMapLayersMinimumZoom, OurGeographies, PopUpField, OpenETSyncIntegration, OpenETInstructions, UsageEstimates, LastSuccessfulSyncDate, LastSyncDate, DateFinalized, EstimateDate, ZoneGroupsEdit, ZoneGroupConfiguration, ZoneGroupCSVUploader, ZoneColumn, ZoneGroupList, HomeAboutCopy, WaterAccountBudgetReport, CustomPageEditProperties, ZoneGroupUsageChart, ModalUpdateWaterAccountInformation, ModalCreateNewWaterAccount, ModalAddParcelToWaterAccount, ModalMergeWaterAccounts, ModalUpdateWaterAccountParcels, WaterAccountMergeType, GeographyAbout, GeographyAllocations, GeographyWaterLevels, GeographySupport, MonitoringWellsGrid, WaterDashboardWaterAccounts, AddAWellScenario, AllocationPlanEdit, AllocationPlansConfigure, CloneAllocationPlan, AccountAllocationPlans, RechargeScenario, PostToSupply, OpenETSyncVariable, LandownerParcelIndex, ScenarioPlanner, ScenarioPlannerGET, ScenarioPlannerScenarioRuns, ActivityCenter, WaterAccountSuggestions, ModalReviewWaterAccountSuggestion, WellRegistryConfigurationPage, WellRegistryMapYourWell, WellRegistryConfirmWellLocation, WellRegistryIrrigatedParcels, WellRegistryContacts, WellRegistryBasicInformation, WellRegistrySupportingInformation, WellRegistryAttachments, WellRegistrySubmit, WellRegistryFieldWellName, WellRegistryFieldSWN, WellRegistryFieldWCR, WellRegistryFieldCountyWellPermit, WellRegistryFieldDateDrilled, WellRegistryFieldWaterUseDescriptionAgricultural, WellRegistryFieldWaterUseDescriptionStockWatering, WellRegistryFieldWaterUseDescriptionDomestic, WellRegistryFieldWaterUseDescriptionPublicMunicipal, WellRegistryFieldWaterUseDescriptionPrivateMunicipal, WellRegistryFieldWaterUseDescriptionOther, WellRegistryFieldTopOfPerforations, WellRegistryFieldBottomOfPerforations, WellRegistryFieldMaxiumumFlow, WellRegistryFieldTypicalFlow, ManageReviewSubmittedWells, LandownerWellList, ManageAllWellRegistrations, FormAsteriskExplanation, WellRegistryIncompleteText, ReferenceWellsList, ReferenceWellsUploader, LandingPageBody, LandingPageUserCard, LandingPageParcelCard, LandingPageWellCard, LandingPageWaterAccountCard, LandingPageOverview, LandingPageAllocationPlans, LandingPageWaterLevels, LandingPageContact, ConfigureLandingPage, HomepageUpdateProfileLink, HomepageGrowerGuideLink, HomepageGeographiesLink, HomepageClaimWaterAccountsPanel, ParcelStatus, OnboardOverviewContent, ParcelBulkActions, MeterList, SerialNumber, MeterConfiguration, Acknowledgements, AdminGeographyEditForm, LandingPageConfigure, MeterDataConfigure, AllocationPlanConfigureCard, WellStatus, UpdateWellInfo, UpdateWellLocation, UpdateWellIrrigatedParcels, AdminFAQ, GeneralFAQ, WaterAccountCustomAttributesEdit, ParcelCustomAttributesEdit, WaterDashboardLink, WaterManagerGuideLink, ReviewParcelChanges, WaterAccountRequestChanges, WaterAccountRequestChangesCertification, ConsolidateWaterAccountsDisclaimer, KernScenarioModel, MercedWaterResourcesModel, YoloScenarioModel, ConfigureGeographySetup, ConfigureCustomAttributes, ConfigureWaterManagers, RasterUploadGuidance, WaterDashboardParcels, WaterDashboardWells, RequestSupport, FeeCalculatorStepOne, FeeCalculatorSurfaceWater, FeeCalculatorStepTwo, FeeCalculatorStepThree, FeeCalculatorAboutFeeStructures, FeeCalculatorIncentivePayment, FeeCalculatorWhatIsConsumedGroundwater, FeeCalculatorYourData, FeeCalculatorAcres, FeeCalculatorFallowingSelfDirected, FeeCalculatorCoverCroppingSelfDirected, FeeCalculatorTemporaryFallowingLandFallowingProgram, FeeCalculatorRotationalExtendedFallowingMLRP, FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP, FeeCalculatorOrchardSwaleRewildingMLRP, FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP, FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP, FeeCalculatorStorageOrRechargeBasinsMLRP, LandingPageFeeCalculator, NewsAndAnnouncements, ReviewSelfReportList, SubmitSelfReportDisclaimer, SelfReportEditorInstructions };
            AllAsSimpleDto = new List<CustomRichTextTypeSimpleDto> { PlatformOverview.AsSimpleDto(), Disclaimer.AsSimpleDto(), Homepage.AsSimpleDto(), Help.AsSimpleDto(), LabelsAndDefinitionsList.AsSimpleDto(), ManageUserParcels.AsSimpleDto(), Training.AsSimpleDto(), CustomPages.AsSimpleDto(), MailMergeReport.AsSimpleDto(), ParcelList.AsSimpleDto(), OnboardOverview.AsSimpleDto(), OnboardWaterAccountPINs.AsSimpleDto(), OnboardClaimParcels.AsSimpleDto(), WellRegistryIntro.AsSimpleDto(), TagList.AsSimpleDto(), BulkTagParcels.AsSimpleDto(), WaterAccounts.AsSimpleDto(), WaterTransactions.AsSimpleDto(), WaterTransactionCreate.AsSimpleDto(), WaterTransactionBulkCreate.AsSimpleDto(), WaterTransactionCSVUploadSupply.AsSimpleDto(), WaterTransactionHistory.AsSimpleDto(), WaterAccount.AsSimpleDto(), Tag.AsSimpleDto(), WaterAccountPIN.AsSimpleDto(), SupplyType.AsSimpleDto(), TotalSupply.AsSimpleDto(), TotalUsage.AsSimpleDto(), TransactionType.AsSimpleDto(), EffectiveDate.AsSimpleDto(), ReportingPeriodConfiguration.AsSimpleDto(), WaterSupplyConfiguration.AsSimpleDto(), WaterLevelsConfiguration.AsSimpleDto(), TradingConfiguration.AsSimpleDto(), ScenariosConfiguration.AsSimpleDto(), WellRegistryConfiguration.AsSimpleDto(), PermissionsConfiguration.AsSimpleDto(), GeospatialDataConfiguration.AsSimpleDto(), CustomPagesConfiguration.AsSimpleDto(), UpdateParcelsUpload.AsSimpleDto(), UpdateParcelsReviewParcels.AsSimpleDto(), UpdateParcelsConfirm.AsSimpleDto(), AccountReconciliation.AsSimpleDto(), Footer.AsSimpleDto(), EditAccounts.AsSimpleDto(), EditUsers.AsSimpleDto(), AccountActivity.AsSimpleDto(), UsageByParcel.AsSimpleDto(), AccountMap.AsSimpleDto(), ChangeParcelOwnership.AsSimpleDto(), WellBulkUpload.AsSimpleDto(), WellName.AsSimpleDto(), CountyWellPermitNo.AsSimpleDto(), StateWCRNo.AsSimpleDto(), WellDepth.AsSimpleDto(), DateDrilled.AsSimpleDto(), WaterTransactionCsvUploadUsage.AsSimpleDto(), APNColumn.AsSimpleDto(), ValueColumn.AsSimpleDto(), UsageType.AsSimpleDto(), Wells.AsSimpleDto(), ExternalMapLayers.AsSimpleDto(), ExternalMapLayersType.AsSimpleDto(), ExternalMapLayersEdit.AsSimpleDto(), ExternalMapLayersName.AsSimpleDto(), ExternalMapLayersMinimumZoom.AsSimpleDto(), OurGeographies.AsSimpleDto(), PopUpField.AsSimpleDto(), OpenETSyncIntegration.AsSimpleDto(), OpenETInstructions.AsSimpleDto(), UsageEstimates.AsSimpleDto(), LastSuccessfulSyncDate.AsSimpleDto(), LastSyncDate.AsSimpleDto(), DateFinalized.AsSimpleDto(), EstimateDate.AsSimpleDto(), ZoneGroupsEdit.AsSimpleDto(), ZoneGroupConfiguration.AsSimpleDto(), ZoneGroupCSVUploader.AsSimpleDto(), ZoneColumn.AsSimpleDto(), ZoneGroupList.AsSimpleDto(), HomeAboutCopy.AsSimpleDto(), WaterAccountBudgetReport.AsSimpleDto(), CustomPageEditProperties.AsSimpleDto(), ZoneGroupUsageChart.AsSimpleDto(), ModalUpdateWaterAccountInformation.AsSimpleDto(), ModalCreateNewWaterAccount.AsSimpleDto(), ModalAddParcelToWaterAccount.AsSimpleDto(), ModalMergeWaterAccounts.AsSimpleDto(), ModalUpdateWaterAccountParcels.AsSimpleDto(), WaterAccountMergeType.AsSimpleDto(), GeographyAbout.AsSimpleDto(), GeographyAllocations.AsSimpleDto(), GeographyWaterLevels.AsSimpleDto(), GeographySupport.AsSimpleDto(), MonitoringWellsGrid.AsSimpleDto(), WaterDashboardWaterAccounts.AsSimpleDto(), AddAWellScenario.AsSimpleDto(), AllocationPlanEdit.AsSimpleDto(), AllocationPlansConfigure.AsSimpleDto(), CloneAllocationPlan.AsSimpleDto(), AccountAllocationPlans.AsSimpleDto(), RechargeScenario.AsSimpleDto(), PostToSupply.AsSimpleDto(), OpenETSyncVariable.AsSimpleDto(), LandownerParcelIndex.AsSimpleDto(), ScenarioPlanner.AsSimpleDto(), ScenarioPlannerGET.AsSimpleDto(), ScenarioPlannerScenarioRuns.AsSimpleDto(), ActivityCenter.AsSimpleDto(), WaterAccountSuggestions.AsSimpleDto(), ModalReviewWaterAccountSuggestion.AsSimpleDto(), WellRegistryConfigurationPage.AsSimpleDto(), WellRegistryMapYourWell.AsSimpleDto(), WellRegistryConfirmWellLocation.AsSimpleDto(), WellRegistryIrrigatedParcels.AsSimpleDto(), WellRegistryContacts.AsSimpleDto(), WellRegistryBasicInformation.AsSimpleDto(), WellRegistrySupportingInformation.AsSimpleDto(), WellRegistryAttachments.AsSimpleDto(), WellRegistrySubmit.AsSimpleDto(), WellRegistryFieldWellName.AsSimpleDto(), WellRegistryFieldSWN.AsSimpleDto(), WellRegistryFieldWCR.AsSimpleDto(), WellRegistryFieldCountyWellPermit.AsSimpleDto(), WellRegistryFieldDateDrilled.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionAgricultural.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionStockWatering.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionDomestic.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionPublicMunicipal.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionPrivateMunicipal.AsSimpleDto(), WellRegistryFieldWaterUseDescriptionOther.AsSimpleDto(), WellRegistryFieldTopOfPerforations.AsSimpleDto(), WellRegistryFieldBottomOfPerforations.AsSimpleDto(), WellRegistryFieldMaxiumumFlow.AsSimpleDto(), WellRegistryFieldTypicalFlow.AsSimpleDto(), ManageReviewSubmittedWells.AsSimpleDto(), LandownerWellList.AsSimpleDto(), ManageAllWellRegistrations.AsSimpleDto(), FormAsteriskExplanation.AsSimpleDto(), WellRegistryIncompleteText.AsSimpleDto(), ReferenceWellsList.AsSimpleDto(), ReferenceWellsUploader.AsSimpleDto(), LandingPageBody.AsSimpleDto(), LandingPageUserCard.AsSimpleDto(), LandingPageParcelCard.AsSimpleDto(), LandingPageWellCard.AsSimpleDto(), LandingPageWaterAccountCard.AsSimpleDto(), LandingPageOverview.AsSimpleDto(), LandingPageAllocationPlans.AsSimpleDto(), LandingPageWaterLevels.AsSimpleDto(), LandingPageContact.AsSimpleDto(), ConfigureLandingPage.AsSimpleDto(), HomepageUpdateProfileLink.AsSimpleDto(), HomepageGrowerGuideLink.AsSimpleDto(), HomepageGeographiesLink.AsSimpleDto(), HomepageClaimWaterAccountsPanel.AsSimpleDto(), ParcelStatus.AsSimpleDto(), OnboardOverviewContent.AsSimpleDto(), ParcelBulkActions.AsSimpleDto(), MeterList.AsSimpleDto(), SerialNumber.AsSimpleDto(), MeterConfiguration.AsSimpleDto(), Acknowledgements.AsSimpleDto(), AdminGeographyEditForm.AsSimpleDto(), LandingPageConfigure.AsSimpleDto(), MeterDataConfigure.AsSimpleDto(), AllocationPlanConfigureCard.AsSimpleDto(), WellStatus.AsSimpleDto(), UpdateWellInfo.AsSimpleDto(), UpdateWellLocation.AsSimpleDto(), UpdateWellIrrigatedParcels.AsSimpleDto(), AdminFAQ.AsSimpleDto(), GeneralFAQ.AsSimpleDto(), WaterAccountCustomAttributesEdit.AsSimpleDto(), ParcelCustomAttributesEdit.AsSimpleDto(), WaterDashboardLink.AsSimpleDto(), WaterManagerGuideLink.AsSimpleDto(), ReviewParcelChanges.AsSimpleDto(), WaterAccountRequestChanges.AsSimpleDto(), WaterAccountRequestChangesCertification.AsSimpleDto(), ConsolidateWaterAccountsDisclaimer.AsSimpleDto(), KernScenarioModel.AsSimpleDto(), MercedWaterResourcesModel.AsSimpleDto(), YoloScenarioModel.AsSimpleDto(), ConfigureGeographySetup.AsSimpleDto(), ConfigureCustomAttributes.AsSimpleDto(), ConfigureWaterManagers.AsSimpleDto(), RasterUploadGuidance.AsSimpleDto(), WaterDashboardParcels.AsSimpleDto(), WaterDashboardWells.AsSimpleDto(), RequestSupport.AsSimpleDto(), FeeCalculatorStepOne.AsSimpleDto(), FeeCalculatorSurfaceWater.AsSimpleDto(), FeeCalculatorStepTwo.AsSimpleDto(), FeeCalculatorStepThree.AsSimpleDto(), FeeCalculatorAboutFeeStructures.AsSimpleDto(), FeeCalculatorIncentivePayment.AsSimpleDto(), FeeCalculatorWhatIsConsumedGroundwater.AsSimpleDto(), FeeCalculatorYourData.AsSimpleDto(), FeeCalculatorAcres.AsSimpleDto(), FeeCalculatorFallowingSelfDirected.AsSimpleDto(), FeeCalculatorCoverCroppingSelfDirected.AsSimpleDto(), FeeCalculatorTemporaryFallowingLandFallowingProgram.AsSimpleDto(), FeeCalculatorRotationalExtendedFallowingMLRP.AsSimpleDto(), FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP.AsSimpleDto(), FeeCalculatorOrchardSwaleRewildingMLRP.AsSimpleDto(), FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP.AsSimpleDto(), FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP.AsSimpleDto(), FeeCalculatorStorageOrRechargeBasinsMLRP.AsSimpleDto(), LandingPageFeeCalculator.AsSimpleDto(), NewsAndAnnouncements.AsSimpleDto(), ReviewSelfReportList.AsSimpleDto(), SubmitSelfReportDisclaimer.AsSimpleDto(), SelfReportEditorInstructions.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, CustomRichTextType>(All.ToDictionary(x => x.CustomRichTextTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, CustomRichTextTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.CustomRichTextTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected CustomRichTextType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID)
        {
            CustomRichTextTypeID = customRichTextTypeID;
            CustomRichTextTypeName = customRichTextTypeName;
            CustomRichTextTypeDisplayName = customRichTextTypeDisplayName;
            ContentTypeID = contentTypeID;
        }
        public ContentType? ContentType => ContentTypeID.HasValue ? ContentType.AllLookupDictionary[ContentTypeID.Value] : null;
        [Key]
        public int CustomRichTextTypeID { get; private set; }
        public string CustomRichTextTypeName { get; private set; }
        public string CustomRichTextTypeDisplayName { get; private set; }
        public int? ContentTypeID { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return CustomRichTextTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(CustomRichTextType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.CustomRichTextTypeID == CustomRichTextTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CustomRichTextType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return CustomRichTextTypeID;
        }

        public static bool operator ==(CustomRichTextType left, CustomRichTextType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomRichTextType left, CustomRichTextType right)
        {
            return !Equals(left, right);
        }

        public CustomRichTextTypeEnum ToEnum => (CustomRichTextTypeEnum)GetHashCode();

        public static CustomRichTextType ToType(int enumValue)
        {
            return ToType((CustomRichTextTypeEnum)enumValue);
        }

        public static CustomRichTextType ToType(CustomRichTextTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case CustomRichTextTypeEnum.AccountActivity:
                    return AccountActivity;
                case CustomRichTextTypeEnum.AccountAllocationPlans:
                    return AccountAllocationPlans;
                case CustomRichTextTypeEnum.AccountMap:
                    return AccountMap;
                case CustomRichTextTypeEnum.AccountReconciliation:
                    return AccountReconciliation;
                case CustomRichTextTypeEnum.Acknowledgements:
                    return Acknowledgements;
                case CustomRichTextTypeEnum.ActivityCenter:
                    return ActivityCenter;
                case CustomRichTextTypeEnum.AddAWellScenario:
                    return AddAWellScenario;
                case CustomRichTextTypeEnum.AdminFAQ:
                    return AdminFAQ;
                case CustomRichTextTypeEnum.AdminGeographyEditForm:
                    return AdminGeographyEditForm;
                case CustomRichTextTypeEnum.AllocationPlanConfigureCard:
                    return AllocationPlanConfigureCard;
                case CustomRichTextTypeEnum.AllocationPlanEdit:
                    return AllocationPlanEdit;
                case CustomRichTextTypeEnum.AllocationPlansConfigure:
                    return AllocationPlansConfigure;
                case CustomRichTextTypeEnum.APNColumn:
                    return APNColumn;
                case CustomRichTextTypeEnum.BulkTagParcels:
                    return BulkTagParcels;
                case CustomRichTextTypeEnum.ChangeParcelOwnership:
                    return ChangeParcelOwnership;
                case CustomRichTextTypeEnum.CloneAllocationPlan:
                    return CloneAllocationPlan;
                case CustomRichTextTypeEnum.ConfigureCustomAttributes:
                    return ConfigureCustomAttributes;
                case CustomRichTextTypeEnum.ConfigureGeographySetup:
                    return ConfigureGeographySetup;
                case CustomRichTextTypeEnum.ConfigureLandingPage:
                    return ConfigureLandingPage;
                case CustomRichTextTypeEnum.ConfigureWaterManagers:
                    return ConfigureWaterManagers;
                case CustomRichTextTypeEnum.ConsolidateWaterAccountsDisclaimer:
                    return ConsolidateWaterAccountsDisclaimer;
                case CustomRichTextTypeEnum.CountyWellPermitNo:
                    return CountyWellPermitNo;
                case CustomRichTextTypeEnum.CustomPageEditProperties:
                    return CustomPageEditProperties;
                case CustomRichTextTypeEnum.CustomPages:
                    return CustomPages;
                case CustomRichTextTypeEnum.CustomPagesConfiguration:
                    return CustomPagesConfiguration;
                case CustomRichTextTypeEnum.DateDrilled:
                    return DateDrilled;
                case CustomRichTextTypeEnum.DateFinalized:
                    return DateFinalized;
                case CustomRichTextTypeEnum.Disclaimer:
                    return Disclaimer;
                case CustomRichTextTypeEnum.EditAccounts:
                    return EditAccounts;
                case CustomRichTextTypeEnum.EditUsers:
                    return EditUsers;
                case CustomRichTextTypeEnum.EffectiveDate:
                    return EffectiveDate;
                case CustomRichTextTypeEnum.EstimateDate:
                    return EstimateDate;
                case CustomRichTextTypeEnum.ExternalMapLayers:
                    return ExternalMapLayers;
                case CustomRichTextTypeEnum.ExternalMapLayersEdit:
                    return ExternalMapLayersEdit;
                case CustomRichTextTypeEnum.ExternalMapLayersMinimumZoom:
                    return ExternalMapLayersMinimumZoom;
                case CustomRichTextTypeEnum.ExternalMapLayersName:
                    return ExternalMapLayersName;
                case CustomRichTextTypeEnum.ExternalMapLayersType:
                    return ExternalMapLayersType;
                case CustomRichTextTypeEnum.FeeCalculatorAboutFeeStructures:
                    return FeeCalculatorAboutFeeStructures;
                case CustomRichTextTypeEnum.FeeCalculatorAcres:
                    return FeeCalculatorAcres;
                case CustomRichTextTypeEnum.FeeCalculatorCoverCroppingSelfDirected:
                    return FeeCalculatorCoverCroppingSelfDirected;
                case CustomRichTextTypeEnum.FeeCalculatorFallowingSelfDirected:
                    return FeeCalculatorFallowingSelfDirected;
                case CustomRichTextTypeEnum.FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP:
                    return FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP:
                    return FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorIncentivePayment:
                    return FeeCalculatorIncentivePayment;
                case CustomRichTextTypeEnum.FeeCalculatorOrchardSwaleRewildingMLRP:
                    return FeeCalculatorOrchardSwaleRewildingMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP:
                    return FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorRotationalExtendedFallowingMLRP:
                    return FeeCalculatorRotationalExtendedFallowingMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorStepOne:
                    return FeeCalculatorStepOne;
                case CustomRichTextTypeEnum.FeeCalculatorStepThree:
                    return FeeCalculatorStepThree;
                case CustomRichTextTypeEnum.FeeCalculatorStepTwo:
                    return FeeCalculatorStepTwo;
                case CustomRichTextTypeEnum.FeeCalculatorStorageOrRechargeBasinsMLRP:
                    return FeeCalculatorStorageOrRechargeBasinsMLRP;
                case CustomRichTextTypeEnum.FeeCalculatorSurfaceWater:
                    return FeeCalculatorSurfaceWater;
                case CustomRichTextTypeEnum.FeeCalculatorTemporaryFallowingLandFallowingProgram:
                    return FeeCalculatorTemporaryFallowingLandFallowingProgram;
                case CustomRichTextTypeEnum.FeeCalculatorWhatIsConsumedGroundwater:
                    return FeeCalculatorWhatIsConsumedGroundwater;
                case CustomRichTextTypeEnum.FeeCalculatorYourData:
                    return FeeCalculatorYourData;
                case CustomRichTextTypeEnum.Footer:
                    return Footer;
                case CustomRichTextTypeEnum.FormAsteriskExplanation:
                    return FormAsteriskExplanation;
                case CustomRichTextTypeEnum.GeneralFAQ:
                    return GeneralFAQ;
                case CustomRichTextTypeEnum.GeographyAbout:
                    return GeographyAbout;
                case CustomRichTextTypeEnum.GeographyAllocations:
                    return GeographyAllocations;
                case CustomRichTextTypeEnum.GeographySupport:
                    return GeographySupport;
                case CustomRichTextTypeEnum.GeographyWaterLevels:
                    return GeographyWaterLevels;
                case CustomRichTextTypeEnum.GeospatialDataConfiguration:
                    return GeospatialDataConfiguration;
                case CustomRichTextTypeEnum.Help:
                    return Help;
                case CustomRichTextTypeEnum.Homepage:
                    return Homepage;
                case CustomRichTextTypeEnum.HomeAboutCopy:
                    return HomeAboutCopy;
                case CustomRichTextTypeEnum.HomepageClaimWaterAccountsPanel:
                    return HomepageClaimWaterAccountsPanel;
                case CustomRichTextTypeEnum.HomepageGeographiesLink:
                    return HomepageGeographiesLink;
                case CustomRichTextTypeEnum.HomepageGrowerGuideLink:
                    return HomepageGrowerGuideLink;
                case CustomRichTextTypeEnum.HomepageUpdateProfileLink:
                    return HomepageUpdateProfileLink;
                case CustomRichTextTypeEnum.KernScenarioModel:
                    return KernScenarioModel;
                case CustomRichTextTypeEnum.LabelsAndDefinitionsList:
                    return LabelsAndDefinitionsList;
                case CustomRichTextTypeEnum.LandingPageAllocationPlans:
                    return LandingPageAllocationPlans;
                case CustomRichTextTypeEnum.LandingPageBody:
                    return LandingPageBody;
                case CustomRichTextTypeEnum.LandingPageConfigure:
                    return LandingPageConfigure;
                case CustomRichTextTypeEnum.LandingPageContact:
                    return LandingPageContact;
                case CustomRichTextTypeEnum.LandingPageFeeCalculator:
                    return LandingPageFeeCalculator;
                case CustomRichTextTypeEnum.LandingPageOverview:
                    return LandingPageOverview;
                case CustomRichTextTypeEnum.LandingPageParcelCard:
                    return LandingPageParcelCard;
                case CustomRichTextTypeEnum.LandingPageUserCard:
                    return LandingPageUserCard;
                case CustomRichTextTypeEnum.LandingPageWaterAccountCard:
                    return LandingPageWaterAccountCard;
                case CustomRichTextTypeEnum.LandingPageWaterLevels:
                    return LandingPageWaterLevels;
                case CustomRichTextTypeEnum.LandingPageWellCard:
                    return LandingPageWellCard;
                case CustomRichTextTypeEnum.LandownerParcelIndex:
                    return LandownerParcelIndex;
                case CustomRichTextTypeEnum.LandownerWellList:
                    return LandownerWellList;
                case CustomRichTextTypeEnum.LastSuccessfulSyncDate:
                    return LastSuccessfulSyncDate;
                case CustomRichTextTypeEnum.LastSyncDate:
                    return LastSyncDate;
                case CustomRichTextTypeEnum.MailMergeReport:
                    return MailMergeReport;
                case CustomRichTextTypeEnum.ManageAllWellRegistrations:
                    return ManageAllWellRegistrations;
                case CustomRichTextTypeEnum.ManageReviewSubmittedWells:
                    return ManageReviewSubmittedWells;
                case CustomRichTextTypeEnum.ManageUserParcels:
                    return ManageUserParcels;
                case CustomRichTextTypeEnum.MercedWaterResourcesModel:
                    return MercedWaterResourcesModel;
                case CustomRichTextTypeEnum.MeterConfiguration:
                    return MeterConfiguration;
                case CustomRichTextTypeEnum.MeterDataConfigure:
                    return MeterDataConfigure;
                case CustomRichTextTypeEnum.MeterList:
                    return MeterList;
                case CustomRichTextTypeEnum.ModalAddParcelToWaterAccount:
                    return ModalAddParcelToWaterAccount;
                case CustomRichTextTypeEnum.ModalCreateNewWaterAccount:
                    return ModalCreateNewWaterAccount;
                case CustomRichTextTypeEnum.ModalMergeWaterAccounts:
                    return ModalMergeWaterAccounts;
                case CustomRichTextTypeEnum.ModalReviewWaterAccountSuggestion:
                    return ModalReviewWaterAccountSuggestion;
                case CustomRichTextTypeEnum.ModalUpdateWaterAccountInformation:
                    return ModalUpdateWaterAccountInformation;
                case CustomRichTextTypeEnum.ModalUpdateWaterAccountParcels:
                    return ModalUpdateWaterAccountParcels;
                case CustomRichTextTypeEnum.MonitoringWellsGrid:
                    return MonitoringWellsGrid;
                case CustomRichTextTypeEnum.NewsAndAnnouncements:
                    return NewsAndAnnouncements;
                case CustomRichTextTypeEnum.OnboardClaimParcels:
                    return OnboardClaimParcels;
                case CustomRichTextTypeEnum.OnboardOverview:
                    return OnboardOverview;
                case CustomRichTextTypeEnum.OnboardOverviewContent:
                    return OnboardOverviewContent;
                case CustomRichTextTypeEnum.OnboardWaterAccountPINs:
                    return OnboardWaterAccountPINs;
                case CustomRichTextTypeEnum.OpenETInstructions:
                    return OpenETInstructions;
                case CustomRichTextTypeEnum.OpenETSyncIntegration:
                    return OpenETSyncIntegration;
                case CustomRichTextTypeEnum.OpenETSyncVariable:
                    return OpenETSyncVariable;
                case CustomRichTextTypeEnum.OurGeographies:
                    return OurGeographies;
                case CustomRichTextTypeEnum.ParcelBulkActions:
                    return ParcelBulkActions;
                case CustomRichTextTypeEnum.ParcelCustomAttributesEdit:
                    return ParcelCustomAttributesEdit;
                case CustomRichTextTypeEnum.ParcelList:
                    return ParcelList;
                case CustomRichTextTypeEnum.ParcelStatus:
                    return ParcelStatus;
                case CustomRichTextTypeEnum.PermissionsConfiguration:
                    return PermissionsConfiguration;
                case CustomRichTextTypeEnum.PlatformOverview:
                    return PlatformOverview;
                case CustomRichTextTypeEnum.PopUpField:
                    return PopUpField;
                case CustomRichTextTypeEnum.PostToSupply:
                    return PostToSupply;
                case CustomRichTextTypeEnum.RasterUploadGuidance:
                    return RasterUploadGuidance;
                case CustomRichTextTypeEnum.RechargeScenario:
                    return RechargeScenario;
                case CustomRichTextTypeEnum.ReferenceWellsList:
                    return ReferenceWellsList;
                case CustomRichTextTypeEnum.ReferenceWellsUploader:
                    return ReferenceWellsUploader;
                case CustomRichTextTypeEnum.ReportingPeriodConfiguration:
                    return ReportingPeriodConfiguration;
                case CustomRichTextTypeEnum.RequestSupport:
                    return RequestSupport;
                case CustomRichTextTypeEnum.ReviewParcelChanges:
                    return ReviewParcelChanges;
                case CustomRichTextTypeEnum.ReviewSelfReportList:
                    return ReviewSelfReportList;
                case CustomRichTextTypeEnum.ScenarioPlanner:
                    return ScenarioPlanner;
                case CustomRichTextTypeEnum.ScenarioPlannerGET:
                    return ScenarioPlannerGET;
                case CustomRichTextTypeEnum.ScenarioPlannerScenarioRuns:
                    return ScenarioPlannerScenarioRuns;
                case CustomRichTextTypeEnum.ScenariosConfiguration:
                    return ScenariosConfiguration;
                case CustomRichTextTypeEnum.SelfReportEditorInstructions:
                    return SelfReportEditorInstructions;
                case CustomRichTextTypeEnum.SerialNumber:
                    return SerialNumber;
                case CustomRichTextTypeEnum.StateWCRNo:
                    return StateWCRNo;
                case CustomRichTextTypeEnum.SubmitSelfReportDisclaimer:
                    return SubmitSelfReportDisclaimer;
                case CustomRichTextTypeEnum.SupplyType:
                    return SupplyType;
                case CustomRichTextTypeEnum.Tag:
                    return Tag;
                case CustomRichTextTypeEnum.TagList:
                    return TagList;
                case CustomRichTextTypeEnum.TotalSupply:
                    return TotalSupply;
                case CustomRichTextTypeEnum.TotalUsage:
                    return TotalUsage;
                case CustomRichTextTypeEnum.TradingConfiguration:
                    return TradingConfiguration;
                case CustomRichTextTypeEnum.Training:
                    return Training;
                case CustomRichTextTypeEnum.TransactionType:
                    return TransactionType;
                case CustomRichTextTypeEnum.UpdateParcelsConfirm:
                    return UpdateParcelsConfirm;
                case CustomRichTextTypeEnum.UpdateParcelsReviewParcels:
                    return UpdateParcelsReviewParcels;
                case CustomRichTextTypeEnum.UpdateParcelsUpload:
                    return UpdateParcelsUpload;
                case CustomRichTextTypeEnum.UpdateWellInfo:
                    return UpdateWellInfo;
                case CustomRichTextTypeEnum.UpdateWellIrrigatedParcels:
                    return UpdateWellIrrigatedParcels;
                case CustomRichTextTypeEnum.UpdateWellLocation:
                    return UpdateWellLocation;
                case CustomRichTextTypeEnum.UsageByParcel:
                    return UsageByParcel;
                case CustomRichTextTypeEnum.UsageEstimates:
                    return UsageEstimates;
                case CustomRichTextTypeEnum.UsageType:
                    return UsageType;
                case CustomRichTextTypeEnum.ValueColumn:
                    return ValueColumn;
                case CustomRichTextTypeEnum.WaterAccount:
                    return WaterAccount;
                case CustomRichTextTypeEnum.WaterAccountBudgetReport:
                    return WaterAccountBudgetReport;
                case CustomRichTextTypeEnum.WaterAccountCustomAttributesEdit:
                    return WaterAccountCustomAttributesEdit;
                case CustomRichTextTypeEnum.WaterAccountMergeType:
                    return WaterAccountMergeType;
                case CustomRichTextTypeEnum.WaterAccountPIN:
                    return WaterAccountPIN;
                case CustomRichTextTypeEnum.WaterAccountRequestChanges:
                    return WaterAccountRequestChanges;
                case CustomRichTextTypeEnum.WaterAccountRequestChangesCertification:
                    return WaterAccountRequestChangesCertification;
                case CustomRichTextTypeEnum.WaterAccounts:
                    return WaterAccounts;
                case CustomRichTextTypeEnum.WaterAccountSuggestions:
                    return WaterAccountSuggestions;
                case CustomRichTextTypeEnum.WaterDashboardLink:
                    return WaterDashboardLink;
                case CustomRichTextTypeEnum.WaterDashboardParcels:
                    return WaterDashboardParcels;
                case CustomRichTextTypeEnum.WaterDashboardWaterAccounts:
                    return WaterDashboardWaterAccounts;
                case CustomRichTextTypeEnum.WaterDashboardWells:
                    return WaterDashboardWells;
                case CustomRichTextTypeEnum.WaterLevelsConfiguration:
                    return WaterLevelsConfiguration;
                case CustomRichTextTypeEnum.WaterManagerGuideLink:
                    return WaterManagerGuideLink;
                case CustomRichTextTypeEnum.WaterSupplyConfiguration:
                    return WaterSupplyConfiguration;
                case CustomRichTextTypeEnum.WaterTransactionBulkCreate:
                    return WaterTransactionBulkCreate;
                case CustomRichTextTypeEnum.WaterTransactionCreate:
                    return WaterTransactionCreate;
                case CustomRichTextTypeEnum.WaterTransactionCSVUploadSupply:
                    return WaterTransactionCSVUploadSupply;
                case CustomRichTextTypeEnum.WaterTransactionCsvUploadUsage:
                    return WaterTransactionCsvUploadUsage;
                case CustomRichTextTypeEnum.WaterTransactionHistory:
                    return WaterTransactionHistory;
                case CustomRichTextTypeEnum.WaterTransactions:
                    return WaterTransactions;
                case CustomRichTextTypeEnum.WellBulkUpload:
                    return WellBulkUpload;
                case CustomRichTextTypeEnum.WellDepth:
                    return WellDepth;
                case CustomRichTextTypeEnum.WellName:
                    return WellName;
                case CustomRichTextTypeEnum.WellRegistryAttachments:
                    return WellRegistryAttachments;
                case CustomRichTextTypeEnum.WellRegistryBasicInformation:
                    return WellRegistryBasicInformation;
                case CustomRichTextTypeEnum.WellRegistryConfiguration:
                    return WellRegistryConfiguration;
                case CustomRichTextTypeEnum.WellRegistryConfigurationPage:
                    return WellRegistryConfigurationPage;
                case CustomRichTextTypeEnum.WellRegistryConfirmWellLocation:
                    return WellRegistryConfirmWellLocation;
                case CustomRichTextTypeEnum.WellRegistryContacts:
                    return WellRegistryContacts;
                case CustomRichTextTypeEnum.WellRegistryFieldBottomOfPerforations:
                    return WellRegistryFieldBottomOfPerforations;
                case CustomRichTextTypeEnum.WellRegistryFieldCountyWellPermit:
                    return WellRegistryFieldCountyWellPermit;
                case CustomRichTextTypeEnum.WellRegistryFieldDateDrilled:
                    return WellRegistryFieldDateDrilled;
                case CustomRichTextTypeEnum.WellRegistryFieldMaxiumumFlow:
                    return WellRegistryFieldMaxiumumFlow;
                case CustomRichTextTypeEnum.WellRegistryFieldSWN:
                    return WellRegistryFieldSWN;
                case CustomRichTextTypeEnum.WellRegistryFieldTopOfPerforations:
                    return WellRegistryFieldTopOfPerforations;
                case CustomRichTextTypeEnum.WellRegistryFieldTypicalFlow:
                    return WellRegistryFieldTypicalFlow;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionAgricultural:
                    return WellRegistryFieldWaterUseDescriptionAgricultural;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionDomestic:
                    return WellRegistryFieldWaterUseDescriptionDomestic;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionOther:
                    return WellRegistryFieldWaterUseDescriptionOther;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionPrivateMunicipal:
                    return WellRegistryFieldWaterUseDescriptionPrivateMunicipal;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionPublicMunicipal:
                    return WellRegistryFieldWaterUseDescriptionPublicMunicipal;
                case CustomRichTextTypeEnum.WellRegistryFieldWaterUseDescriptionStockWatering:
                    return WellRegistryFieldWaterUseDescriptionStockWatering;
                case CustomRichTextTypeEnum.WellRegistryFieldWCR:
                    return WellRegistryFieldWCR;
                case CustomRichTextTypeEnum.WellRegistryFieldWellName:
                    return WellRegistryFieldWellName;
                case CustomRichTextTypeEnum.WellRegistryIncompleteText:
                    return WellRegistryIncompleteText;
                case CustomRichTextTypeEnum.WellRegistryIntro:
                    return WellRegistryIntro;
                case CustomRichTextTypeEnum.WellRegistryIrrigatedParcels:
                    return WellRegistryIrrigatedParcels;
                case CustomRichTextTypeEnum.WellRegistryMapYourWell:
                    return WellRegistryMapYourWell;
                case CustomRichTextTypeEnum.WellRegistrySubmit:
                    return WellRegistrySubmit;
                case CustomRichTextTypeEnum.WellRegistrySupportingInformation:
                    return WellRegistrySupportingInformation;
                case CustomRichTextTypeEnum.Wells:
                    return Wells;
                case CustomRichTextTypeEnum.WellStatus:
                    return WellStatus;
                case CustomRichTextTypeEnum.YoloScenarioModel:
                    return YoloScenarioModel;
                case CustomRichTextTypeEnum.ZoneColumn:
                    return ZoneColumn;
                case CustomRichTextTypeEnum.ZoneGroupConfiguration:
                    return ZoneGroupConfiguration;
                case CustomRichTextTypeEnum.ZoneGroupCSVUploader:
                    return ZoneGroupCSVUploader;
                case CustomRichTextTypeEnum.ZoneGroupList:
                    return ZoneGroupList;
                case CustomRichTextTypeEnum.ZoneGroupsEdit:
                    return ZoneGroupsEdit;
                case CustomRichTextTypeEnum.ZoneGroupUsageChart:
                    return ZoneGroupUsageChart;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum CustomRichTextTypeEnum
    {
        PlatformOverview = 1,
        Disclaimer = 2,
        Homepage = 3,
        Help = 4,
        LabelsAndDefinitionsList = 5,
        ManageUserParcels = 6,
        Training = 7,
        CustomPages = 8,
        MailMergeReport = 9,
        ParcelList = 10,
        OnboardOverview = 11,
        OnboardWaterAccountPINs = 12,
        OnboardClaimParcels = 13,
        WellRegistryIntro = 14,
        TagList = 15,
        BulkTagParcels = 16,
        WaterAccounts = 17,
        WaterTransactions = 18,
        WaterTransactionCreate = 19,
        WaterTransactionBulkCreate = 20,
        WaterTransactionCSVUploadSupply = 21,
        WaterTransactionHistory = 22,
        WaterAccount = 23,
        Tag = 24,
        WaterAccountPIN = 25,
        SupplyType = 26,
        TotalSupply = 27,
        TotalUsage = 28,
        TransactionType = 29,
        EffectiveDate = 30,
        ReportingPeriodConfiguration = 31,
        WaterSupplyConfiguration = 32,
        WaterLevelsConfiguration = 33,
        TradingConfiguration = 34,
        ScenariosConfiguration = 35,
        WellRegistryConfiguration = 36,
        PermissionsConfiguration = 37,
        GeospatialDataConfiguration = 38,
        CustomPagesConfiguration = 39,
        UpdateParcelsUpload = 40,
        UpdateParcelsReviewParcels = 41,
        UpdateParcelsConfirm = 42,
        AccountReconciliation = 43,
        Footer = 44,
        EditAccounts = 45,
        EditUsers = 46,
        AccountActivity = 47,
        UsageByParcel = 48,
        AccountMap = 49,
        ChangeParcelOwnership = 50,
        WellBulkUpload = 51,
        WellName = 52,
        CountyWellPermitNo = 53,
        StateWCRNo = 54,
        WellDepth = 55,
        DateDrilled = 56,
        WaterTransactionCsvUploadUsage = 57,
        APNColumn = 58,
        ValueColumn = 59,
        UsageType = 60,
        Wells = 61,
        ExternalMapLayers = 62,
        ExternalMapLayersType = 63,
        ExternalMapLayersEdit = 64,
        ExternalMapLayersName = 65,
        ExternalMapLayersMinimumZoom = 66,
        OurGeographies = 67,
        PopUpField = 68,
        OpenETSyncIntegration = 69,
        OpenETInstructions = 70,
        UsageEstimates = 71,
        LastSuccessfulSyncDate = 72,
        LastSyncDate = 73,
        DateFinalized = 74,
        EstimateDate = 75,
        ZoneGroupsEdit = 76,
        ZoneGroupConfiguration = 77,
        ZoneGroupCSVUploader = 78,
        ZoneColumn = 79,
        ZoneGroupList = 80,
        HomeAboutCopy = 81,
        WaterAccountBudgetReport = 82,
        CustomPageEditProperties = 83,
        ZoneGroupUsageChart = 84,
        ModalUpdateWaterAccountInformation = 85,
        ModalCreateNewWaterAccount = 86,
        ModalAddParcelToWaterAccount = 87,
        ModalMergeWaterAccounts = 88,
        ModalUpdateWaterAccountParcels = 89,
        WaterAccountMergeType = 90,
        GeographyAbout = 91,
        GeographyAllocations = 92,
        GeographyWaterLevels = 93,
        GeographySupport = 94,
        MonitoringWellsGrid = 95,
        WaterDashboardWaterAccounts = 96,
        AddAWellScenario = 97,
        AllocationPlanEdit = 98,
        AllocationPlansConfigure = 99,
        CloneAllocationPlan = 100,
        AccountAllocationPlans = 101,
        RechargeScenario = 102,
        PostToSupply = 103,
        OpenETSyncVariable = 104,
        LandownerParcelIndex = 105,
        ScenarioPlanner = 106,
        ScenarioPlannerGET = 107,
        ScenarioPlannerScenarioRuns = 108,
        ActivityCenter = 109,
        WaterAccountSuggestions = 110,
        ModalReviewWaterAccountSuggestion = 111,
        WellRegistryConfigurationPage = 112,
        WellRegistryMapYourWell = 113,
        WellRegistryConfirmWellLocation = 114,
        WellRegistryIrrigatedParcels = 115,
        WellRegistryContacts = 116,
        WellRegistryBasicInformation = 117,
        WellRegistrySupportingInformation = 118,
        WellRegistryAttachments = 119,
        WellRegistrySubmit = 120,
        WellRegistryFieldWellName = 121,
        WellRegistryFieldSWN = 122,
        WellRegistryFieldWCR = 123,
        WellRegistryFieldCountyWellPermit = 124,
        WellRegistryFieldDateDrilled = 125,
        WellRegistryFieldWaterUseDescriptionAgricultural = 126,
        WellRegistryFieldWaterUseDescriptionStockWatering = 127,
        WellRegistryFieldWaterUseDescriptionDomestic = 128,
        WellRegistryFieldWaterUseDescriptionPublicMunicipal = 129,
        WellRegistryFieldWaterUseDescriptionPrivateMunicipal = 130,
        WellRegistryFieldWaterUseDescriptionOther = 131,
        WellRegistryFieldTopOfPerforations = 132,
        WellRegistryFieldBottomOfPerforations = 133,
        WellRegistryFieldMaxiumumFlow = 134,
        WellRegistryFieldTypicalFlow = 135,
        ManageReviewSubmittedWells = 136,
        LandownerWellList = 137,
        ManageAllWellRegistrations = 138,
        FormAsteriskExplanation = 139,
        WellRegistryIncompleteText = 140,
        ReferenceWellsList = 141,
        ReferenceWellsUploader = 142,
        LandingPageBody = 143,
        LandingPageUserCard = 144,
        LandingPageParcelCard = 145,
        LandingPageWellCard = 146,
        LandingPageWaterAccountCard = 147,
        LandingPageOverview = 148,
        LandingPageAllocationPlans = 149,
        LandingPageWaterLevels = 150,
        LandingPageContact = 151,
        ConfigureLandingPage = 152,
        HomepageUpdateProfileLink = 153,
        HomepageGrowerGuideLink = 154,
        HomepageGeographiesLink = 155,
        HomepageClaimWaterAccountsPanel = 156,
        ParcelStatus = 157,
        OnboardOverviewContent = 158,
        ParcelBulkActions = 159,
        MeterList = 160,
        SerialNumber = 161,
        MeterConfiguration = 162,
        Acknowledgements = 163,
        AdminGeographyEditForm = 164,
        LandingPageConfigure = 165,
        MeterDataConfigure = 166,
        AllocationPlanConfigureCard = 167,
        WellStatus = 168,
        UpdateWellInfo = 169,
        UpdateWellLocation = 170,
        UpdateWellIrrigatedParcels = 171,
        AdminFAQ = 172,
        GeneralFAQ = 173,
        WaterAccountCustomAttributesEdit = 174,
        ParcelCustomAttributesEdit = 175,
        WaterDashboardLink = 176,
        WaterManagerGuideLink = 177,
        ReviewParcelChanges = 178,
        WaterAccountRequestChanges = 179,
        WaterAccountRequestChangesCertification = 180,
        ConsolidateWaterAccountsDisclaimer = 181,
        KernScenarioModel = 182,
        MercedWaterResourcesModel = 183,
        YoloScenarioModel = 184,
        ConfigureGeographySetup = 185,
        ConfigureCustomAttributes = 186,
        ConfigureWaterManagers = 187,
        RasterUploadGuidance = 188,
        WaterDashboardParcels = 189,
        WaterDashboardWells = 190,
        RequestSupport = 191,
        FeeCalculatorStepOne = 192,
        FeeCalculatorSurfaceWater = 193,
        FeeCalculatorStepTwo = 194,
        FeeCalculatorStepThree = 195,
        FeeCalculatorAboutFeeStructures = 196,
        FeeCalculatorIncentivePayment = 197,
        FeeCalculatorWhatIsConsumedGroundwater = 198,
        FeeCalculatorYourData = 199,
        FeeCalculatorAcres = 200,
        FeeCalculatorFallowingSelfDirected = 201,
        FeeCalculatorCoverCroppingSelfDirected = 202,
        FeeCalculatorTemporaryFallowingLandFallowingProgram = 203,
        FeeCalculatorRotationalExtendedFallowingMLRP = 204,
        FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP = 205,
        FeeCalculatorOrchardSwaleRewildingMLRP = 206,
        FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP = 207,
        FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP = 208,
        FeeCalculatorStorageOrRechargeBasinsMLRP = 209,
        LandingPageFeeCalculator = 210,
        NewsAndAnnouncements = 211,
        ReviewSelfReportList = 212,
        SubmitSelfReportDisclaimer = 213,
        SelfReportEditorInstructions = 214
    }

    public partial class CustomRichTextTypePlatformOverview : CustomRichTextType
    {
        private CustomRichTextTypePlatformOverview(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypePlatformOverview Instance = new CustomRichTextTypePlatformOverview(1, @"Platform Overview", @"Platform Overview", 1);
    }

    public partial class CustomRichTextTypeDisclaimer : CustomRichTextType
    {
        private CustomRichTextTypeDisclaimer(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeDisclaimer Instance = new CustomRichTextTypeDisclaimer(2, @"Disclaimer", @"Disclaimer", 1);
    }

    public partial class CustomRichTextTypeHomepage : CustomRichTextType
    {
        private CustomRichTextTypeHomepage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomepage Instance = new CustomRichTextTypeHomepage(3, @"Home page", @"Home page", 1);
    }

    public partial class CustomRichTextTypeHelp : CustomRichTextType
    {
        private CustomRichTextTypeHelp(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHelp Instance = new CustomRichTextTypeHelp(4, @"Help", @"Help", 1);
    }

    public partial class CustomRichTextTypeLabelsAndDefinitionsList : CustomRichTextType
    {
        private CustomRichTextTypeLabelsAndDefinitionsList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLabelsAndDefinitionsList Instance = new CustomRichTextTypeLabelsAndDefinitionsList(5, @"LabelsAndDefinitionsList", @"Labels and Definitions List", 1);
    }

    public partial class CustomRichTextTypeManageUserParcels : CustomRichTextType
    {
        private CustomRichTextTypeManageUserParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeManageUserParcels Instance = new CustomRichTextTypeManageUserParcels(6, @"ManageUserParcels", @"Manage User Parcels", 1);
    }

    public partial class CustomRichTextTypeTraining : CustomRichTextType
    {
        private CustomRichTextTypeTraining(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTraining Instance = new CustomRichTextTypeTraining(7, @"Training", @"Training", 1);
    }

    public partial class CustomRichTextTypeCustomPages : CustomRichTextType
    {
        private CustomRichTextTypeCustomPages(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeCustomPages Instance = new CustomRichTextTypeCustomPages(8, @"CustomPages", @"Custom Pages", 1);
    }

    public partial class CustomRichTextTypeMailMergeReport : CustomRichTextType
    {
        private CustomRichTextTypeMailMergeReport(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMailMergeReport Instance = new CustomRichTextTypeMailMergeReport(9, @"MailMergeReport", @"Mail Merge Report", 1);
    }

    public partial class CustomRichTextTypeParcelList : CustomRichTextType
    {
        private CustomRichTextTypeParcelList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeParcelList Instance = new CustomRichTextTypeParcelList(10, @"ParcelList", @"Parcel List", 1);
    }

    public partial class CustomRichTextTypeOnboardOverview : CustomRichTextType
    {
        private CustomRichTextTypeOnboardOverview(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOnboardOverview Instance = new CustomRichTextTypeOnboardOverview(11, @"OnboardOverview", @"Onboard Overview", 2);
    }

    public partial class CustomRichTextTypeOnboardWaterAccountPINs : CustomRichTextType
    {
        private CustomRichTextTypeOnboardWaterAccountPINs(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOnboardWaterAccountPINs Instance = new CustomRichTextTypeOnboardWaterAccountPINs(12, @"OnboardWaterAccountPINs", @"Water Account PINs", 2);
    }

    public partial class CustomRichTextTypeOnboardClaimParcels : CustomRichTextType
    {
        private CustomRichTextTypeOnboardClaimParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOnboardClaimParcels Instance = new CustomRichTextTypeOnboardClaimParcels(13, @"OnboardClaimParcels", @"Claim Parcels", 2);
    }

    public partial class CustomRichTextTypeWellRegistryIntro : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryIntro(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryIntro Instance = new CustomRichTextTypeWellRegistryIntro(14, @"WellRegistryIntro", @"Well Registry Intro", 2);
    }

    public partial class CustomRichTextTypeTagList : CustomRichTextType
    {
        private CustomRichTextTypeTagList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTagList Instance = new CustomRichTextTypeTagList(15, @"TagList", @"Tag List", 1);
    }

    public partial class CustomRichTextTypeBulkTagParcels : CustomRichTextType
    {
        private CustomRichTextTypeBulkTagParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeBulkTagParcels Instance = new CustomRichTextTypeBulkTagParcels(16, @"BulkTagParcels", @"Bulk Tag Parcels", 1);
    }

    public partial class CustomRichTextTypeWaterAccounts : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccounts(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccounts Instance = new CustomRichTextTypeWaterAccounts(17, @"WaterAccounts", @"Water Accounts", 1);
    }

    public partial class CustomRichTextTypeWaterTransactions : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactions Instance = new CustomRichTextTypeWaterTransactions(18, @"WaterTransactions", @"Water Transactions", 1);
    }

    public partial class CustomRichTextTypeWaterTransactionCreate : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactionCreate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactionCreate Instance = new CustomRichTextTypeWaterTransactionCreate(19, @"WaterTransactionCreate", @"Water Transaction Create", 1);
    }

    public partial class CustomRichTextTypeWaterTransactionBulkCreate : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactionBulkCreate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactionBulkCreate Instance = new CustomRichTextTypeWaterTransactionBulkCreate(20, @"WaterTransactionBulkCreate", @"Water Transaction Bulk Create", 1);
    }

    public partial class CustomRichTextTypeWaterTransactionCSVUploadSupply : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactionCSVUploadSupply(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactionCSVUploadSupply Instance = new CustomRichTextTypeWaterTransactionCSVUploadSupply(21, @"WaterTransactionCSVUploadSupply", @"Water Transaction CSV Upload Supply", 1);
    }

    public partial class CustomRichTextTypeWaterTransactionHistory : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactionHistory(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactionHistory Instance = new CustomRichTextTypeWaterTransactionHistory(22, @"WaterTransactionHistory", @"WaterTransaction History", 1);
    }

    public partial class CustomRichTextTypeWaterAccount : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccount(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccount Instance = new CustomRichTextTypeWaterAccount(23, @"WaterAccount", @"Water Account", 3);
    }

    public partial class CustomRichTextTypeTag : CustomRichTextType
    {
        private CustomRichTextTypeTag(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTag Instance = new CustomRichTextTypeTag(24, @"Tag", @"Tag", 3);
    }

    public partial class CustomRichTextTypeWaterAccountPIN : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountPIN(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountPIN Instance = new CustomRichTextTypeWaterAccountPIN(25, @"WaterAccountPIN", @"Water Account PIN", 3);
    }

    public partial class CustomRichTextTypeSupplyType : CustomRichTextType
    {
        private CustomRichTextTypeSupplyType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeSupplyType Instance = new CustomRichTextTypeSupplyType(26, @"SupplyType", @"Supply Type", 3);
    }

    public partial class CustomRichTextTypeTotalSupply : CustomRichTextType
    {
        private CustomRichTextTypeTotalSupply(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTotalSupply Instance = new CustomRichTextTypeTotalSupply(27, @"TotalSupply", @"Total Supply", 3);
    }

    public partial class CustomRichTextTypeTotalUsage : CustomRichTextType
    {
        private CustomRichTextTypeTotalUsage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTotalUsage Instance = new CustomRichTextTypeTotalUsage(28, @"TotalUsage", @"Total Usage", 3);
    }

    public partial class CustomRichTextTypeTransactionType : CustomRichTextType
    {
        private CustomRichTextTypeTransactionType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTransactionType Instance = new CustomRichTextTypeTransactionType(29, @"TransactionType", @"Transaction Type", 3);
    }

    public partial class CustomRichTextTypeEffectiveDate : CustomRichTextType
    {
        private CustomRichTextTypeEffectiveDate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeEffectiveDate Instance = new CustomRichTextTypeEffectiveDate(30, @"EffectiveDate", @"Effective Date", 3);
    }

    public partial class CustomRichTextTypeReportingPeriodConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeReportingPeriodConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeReportingPeriodConfiguration Instance = new CustomRichTextTypeReportingPeriodConfiguration(31, @"ReportingPeriodConfiguration", @"Reporting Period Configuration", 1);
    }

    public partial class CustomRichTextTypeWaterSupplyConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeWaterSupplyConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterSupplyConfiguration Instance = new CustomRichTextTypeWaterSupplyConfiguration(32, @"WaterSupplyConfiguration", @"Water Supply Configuration", 1);
    }

    public partial class CustomRichTextTypeWaterLevelsConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeWaterLevelsConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterLevelsConfiguration Instance = new CustomRichTextTypeWaterLevelsConfiguration(33, @"WaterLevelsConfiguration", @"Water Levels Configuration", 1);
    }

    public partial class CustomRichTextTypeTradingConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeTradingConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeTradingConfiguration Instance = new CustomRichTextTypeTradingConfiguration(34, @"TradingConfiguration", @"Trading Configuration", 1);
    }

    public partial class CustomRichTextTypeScenariosConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeScenariosConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeScenariosConfiguration Instance = new CustomRichTextTypeScenariosConfiguration(35, @"ScenariosConfiguration", @"Scenarios Configuration", 1);
    }

    public partial class CustomRichTextTypeWellRegistryConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryConfiguration Instance = new CustomRichTextTypeWellRegistryConfiguration(36, @"WellRegistryConfiguration", @"Well Registry Configuration", 1);
    }

    public partial class CustomRichTextTypePermissionsConfiguration : CustomRichTextType
    {
        private CustomRichTextTypePermissionsConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypePermissionsConfiguration Instance = new CustomRichTextTypePermissionsConfiguration(37, @"PermissionsConfiguration", @"Permissions Configuration", 1);
    }

    public partial class CustomRichTextTypeGeospatialDataConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeGeospatialDataConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeospatialDataConfiguration Instance = new CustomRichTextTypeGeospatialDataConfiguration(38, @"GeospatialDataConfiguration", @"Geospatial Data Configuration", 1);
    }

    public partial class CustomRichTextTypeCustomPagesConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeCustomPagesConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeCustomPagesConfiguration Instance = new CustomRichTextTypeCustomPagesConfiguration(39, @"CustomPagesConfiguration", @"Custom Pages Configuration", 1);
    }

    public partial class CustomRichTextTypeUpdateParcelsUpload : CustomRichTextType
    {
        private CustomRichTextTypeUpdateParcelsUpload(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateParcelsUpload Instance = new CustomRichTextTypeUpdateParcelsUpload(40, @"UpdateParcelsUpload", @"Update Parcels Upload", 2);
    }

    public partial class CustomRichTextTypeUpdateParcelsReviewParcels : CustomRichTextType
    {
        private CustomRichTextTypeUpdateParcelsReviewParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateParcelsReviewParcels Instance = new CustomRichTextTypeUpdateParcelsReviewParcels(41, @"UpdateParcelsReviewParcels", @"Update Parcels Review Parcels", 2);
    }

    public partial class CustomRichTextTypeUpdateParcelsConfirm : CustomRichTextType
    {
        private CustomRichTextTypeUpdateParcelsConfirm(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateParcelsConfirm Instance = new CustomRichTextTypeUpdateParcelsConfirm(42, @"UpdateParcelsConfirm", @"Update Parcels Confirm", 2);
    }

    public partial class CustomRichTextTypeAccountReconciliation : CustomRichTextType
    {
        private CustomRichTextTypeAccountReconciliation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAccountReconciliation Instance = new CustomRichTextTypeAccountReconciliation(43, @"AccountReconciliation", @"Account Reconciliation", 2);
    }

    public partial class CustomRichTextTypeFooter : CustomRichTextType
    {
        private CustomRichTextTypeFooter(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFooter Instance = new CustomRichTextTypeFooter(44, @"Footer", @"Footer", 1);
    }

    public partial class CustomRichTextTypeEditAccounts : CustomRichTextType
    {
        private CustomRichTextTypeEditAccounts(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeEditAccounts Instance = new CustomRichTextTypeEditAccounts(45, @"EditAccounts", @"Edit Accounts", 1);
    }

    public partial class CustomRichTextTypeEditUsers : CustomRichTextType
    {
        private CustomRichTextTypeEditUsers(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeEditUsers Instance = new CustomRichTextTypeEditUsers(46, @"EditUsers", @"Edit Users", 1);
    }

    public partial class CustomRichTextTypeAccountActivity : CustomRichTextType
    {
        private CustomRichTextTypeAccountActivity(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAccountActivity Instance = new CustomRichTextTypeAccountActivity(47, @"AccountActivity", @"Account Activity", 1);
    }

    public partial class CustomRichTextTypeUsageByParcel : CustomRichTextType
    {
        private CustomRichTextTypeUsageByParcel(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUsageByParcel Instance = new CustomRichTextTypeUsageByParcel(48, @"UsageByParcel", @"Usage By Parcel", 1);
    }

    public partial class CustomRichTextTypeAccountMap : CustomRichTextType
    {
        private CustomRichTextTypeAccountMap(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAccountMap Instance = new CustomRichTextTypeAccountMap(49, @"AccountMap", @"Account Map", 1);
    }

    public partial class CustomRichTextTypeChangeParcelOwnership : CustomRichTextType
    {
        private CustomRichTextTypeChangeParcelOwnership(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeChangeParcelOwnership Instance = new CustomRichTextTypeChangeParcelOwnership(50, @"ChangeParcelOwnership", @"Change Parcel Ownership", 1);
    }

    public partial class CustomRichTextTypeWellBulkUpload : CustomRichTextType
    {
        private CustomRichTextTypeWellBulkUpload(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellBulkUpload Instance = new CustomRichTextTypeWellBulkUpload(51, @"WellBulkUpload", @"Well Bulk Upload", 2);
    }

    public partial class CustomRichTextTypeWellName : CustomRichTextType
    {
        private CustomRichTextTypeWellName(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellName Instance = new CustomRichTextTypeWellName(52, @"WellName", @"Well Name", 1);
    }

    public partial class CustomRichTextTypeCountyWellPermitNo : CustomRichTextType
    {
        private CustomRichTextTypeCountyWellPermitNo(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeCountyWellPermitNo Instance = new CustomRichTextTypeCountyWellPermitNo(53, @"CountyWellPermitNo", @"County Well Permit No", 1);
    }

    public partial class CustomRichTextTypeStateWCRNo : CustomRichTextType
    {
        private CustomRichTextTypeStateWCRNo(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeStateWCRNo Instance = new CustomRichTextTypeStateWCRNo(54, @"StateWCRNo", @"State WCR No", 1);
    }

    public partial class CustomRichTextTypeWellDepth : CustomRichTextType
    {
        private CustomRichTextTypeWellDepth(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellDepth Instance = new CustomRichTextTypeWellDepth(55, @"WellDepth", @"Well Depth", 1);
    }

    public partial class CustomRichTextTypeDateDrilled : CustomRichTextType
    {
        private CustomRichTextTypeDateDrilled(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeDateDrilled Instance = new CustomRichTextTypeDateDrilled(56, @"DateDrilled", @"Date Drilled", 1);
    }

    public partial class CustomRichTextTypeWaterTransactionCsvUploadUsage : CustomRichTextType
    {
        private CustomRichTextTypeWaterTransactionCsvUploadUsage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterTransactionCsvUploadUsage Instance = new CustomRichTextTypeWaterTransactionCsvUploadUsage(57, @"WaterTransactionCsvUploadUsage", @"Water Transaction CSV Upload Usage", 1);
    }

    public partial class CustomRichTextTypeAPNColumn : CustomRichTextType
    {
        private CustomRichTextTypeAPNColumn(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAPNColumn Instance = new CustomRichTextTypeAPNColumn(58, @"APNColumn", @"APN Column", 3);
    }

    public partial class CustomRichTextTypeValueColumn : CustomRichTextType
    {
        private CustomRichTextTypeValueColumn(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeValueColumn Instance = new CustomRichTextTypeValueColumn(59, @"ValueColumn", @"Value Column", 3);
    }

    public partial class CustomRichTextTypeUsageType : CustomRichTextType
    {
        private CustomRichTextTypeUsageType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUsageType Instance = new CustomRichTextTypeUsageType(60, @"UsageType", @"Water Measurement Type", 3);
    }

    public partial class CustomRichTextTypeWells : CustomRichTextType
    {
        private CustomRichTextTypeWells(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWells Instance = new CustomRichTextTypeWells(61, @"Wells", @"Wells", 1);
    }

    public partial class CustomRichTextTypeExternalMapLayers : CustomRichTextType
    {
        private CustomRichTextTypeExternalMapLayers(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeExternalMapLayers Instance = new CustomRichTextTypeExternalMapLayers(62, @"ExternalMapLayers", @"External Map Layers", 1);
    }

    public partial class CustomRichTextTypeExternalMapLayersType : CustomRichTextType
    {
        private CustomRichTextTypeExternalMapLayersType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeExternalMapLayersType Instance = new CustomRichTextTypeExternalMapLayersType(63, @"ExternalMapLayersType", @"External Map Layers Type", 3);
    }

    public partial class CustomRichTextTypeExternalMapLayersEdit : CustomRichTextType
    {
        private CustomRichTextTypeExternalMapLayersEdit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeExternalMapLayersEdit Instance = new CustomRichTextTypeExternalMapLayersEdit(64, @"ExternalMapLayersEdit", @"External Map Layers Edit", 1);
    }

    public partial class CustomRichTextTypeExternalMapLayersName : CustomRichTextType
    {
        private CustomRichTextTypeExternalMapLayersName(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeExternalMapLayersName Instance = new CustomRichTextTypeExternalMapLayersName(65, @"ExternalMapLayersName", @"External Map Layers Name", 3);
    }

    public partial class CustomRichTextTypeExternalMapLayersMinimumZoom : CustomRichTextType
    {
        private CustomRichTextTypeExternalMapLayersMinimumZoom(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeExternalMapLayersMinimumZoom Instance = new CustomRichTextTypeExternalMapLayersMinimumZoom(66, @"ExternalMapLayersMinimumZoom", @"External Map Layers Minimum Zoom", 3);
    }

    public partial class CustomRichTextTypeOurGeographies : CustomRichTextType
    {
        private CustomRichTextTypeOurGeographies(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOurGeographies Instance = new CustomRichTextTypeOurGeographies(67, @"OurGeographies", @"Our Geographies", 1);
    }

    public partial class CustomRichTextTypePopUpField : CustomRichTextType
    {
        private CustomRichTextTypePopUpField(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypePopUpField Instance = new CustomRichTextTypePopUpField(68, @"PopUpField", @"Pop-up Field", 3);
    }

    public partial class CustomRichTextTypeOpenETSyncIntegration : CustomRichTextType
    {
        private CustomRichTextTypeOpenETSyncIntegration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOpenETSyncIntegration Instance = new CustomRichTextTypeOpenETSyncIntegration(69, @"OpenETSyncIntegration", @"OpenET Sync Integration", 1);
    }

    public partial class CustomRichTextTypeOpenETInstructions : CustomRichTextType
    {
        private CustomRichTextTypeOpenETInstructions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOpenETInstructions Instance = new CustomRichTextTypeOpenETInstructions(70, @"OpenETInstructions", @"Openet Instructions", 1);
    }

    public partial class CustomRichTextTypeUsageEstimates : CustomRichTextType
    {
        private CustomRichTextTypeUsageEstimates(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUsageEstimates Instance = new CustomRichTextTypeUsageEstimates(71, @"UsageEstimates", @"Usage Estimates", 1);
    }

    public partial class CustomRichTextTypeLastSuccessfulSyncDate : CustomRichTextType
    {
        private CustomRichTextTypeLastSuccessfulSyncDate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLastSuccessfulSyncDate Instance = new CustomRichTextTypeLastSuccessfulSyncDate(72, @"LastSuccessfulSyncDate", @"Last Successful Sync Date", 2);
    }

    public partial class CustomRichTextTypeLastSyncDate : CustomRichTextType
    {
        private CustomRichTextTypeLastSyncDate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLastSyncDate Instance = new CustomRichTextTypeLastSyncDate(73, @"LastSyncDate", @"Last Sync Date", 2);
    }

    public partial class CustomRichTextTypeDateFinalized : CustomRichTextType
    {
        private CustomRichTextTypeDateFinalized(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeDateFinalized Instance = new CustomRichTextTypeDateFinalized(74, @"DateFinalized", @"Date Finalized", 2);
    }

    public partial class CustomRichTextTypeEstimateDate : CustomRichTextType
    {
        private CustomRichTextTypeEstimateDate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeEstimateDate Instance = new CustomRichTextTypeEstimateDate(75, @"EstimateDate", @"Estimate Date", 3);
    }

    public partial class CustomRichTextTypeZoneGroupsEdit : CustomRichTextType
    {
        private CustomRichTextTypeZoneGroupsEdit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneGroupsEdit Instance = new CustomRichTextTypeZoneGroupsEdit(76, @"ZoneGroupsEdit", @"Zone Groups Edit", 1);
    }

    public partial class CustomRichTextTypeZoneGroupConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeZoneGroupConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneGroupConfiguration Instance = new CustomRichTextTypeZoneGroupConfiguration(77, @"ZoneGroupConfiguration", @"Zone Group Configuration", 1);
    }

    public partial class CustomRichTextTypeZoneGroupCSVUploader : CustomRichTextType
    {
        private CustomRichTextTypeZoneGroupCSVUploader(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneGroupCSVUploader Instance = new CustomRichTextTypeZoneGroupCSVUploader(78, @"ZoneGroupCSVUploader", @"Zone Group CSV Uploader", 1);
    }

    public partial class CustomRichTextTypeZoneColumn : CustomRichTextType
    {
        private CustomRichTextTypeZoneColumn(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneColumn Instance = new CustomRichTextTypeZoneColumn(79, @"ZoneColumn", @"Zone Column", 3);
    }

    public partial class CustomRichTextTypeZoneGroupList : CustomRichTextType
    {
        private CustomRichTextTypeZoneGroupList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneGroupList Instance = new CustomRichTextTypeZoneGroupList(80, @"ZoneGroupList", @"Zone Group List", 1);
    }

    public partial class CustomRichTextTypeHomeAboutCopy : CustomRichTextType
    {
        private CustomRichTextTypeHomeAboutCopy(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomeAboutCopy Instance = new CustomRichTextTypeHomeAboutCopy(81, @"HomeAboutCopy", @"Home About Copy", 1);
    }

    public partial class CustomRichTextTypeWaterAccountBudgetReport : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountBudgetReport(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountBudgetReport Instance = new CustomRichTextTypeWaterAccountBudgetReport(82, @"WaterAccountBudgetReport", @"Water Account Budgets Report", 1);
    }

    public partial class CustomRichTextTypeCustomPageEditProperties : CustomRichTextType
    {
        private CustomRichTextTypeCustomPageEditProperties(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeCustomPageEditProperties Instance = new CustomRichTextTypeCustomPageEditProperties(83, @"CustomPageEditProperties", @"Custom Page Edit Properties", 1);
    }

    public partial class CustomRichTextTypeZoneGroupUsageChart : CustomRichTextType
    {
        private CustomRichTextTypeZoneGroupUsageChart(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeZoneGroupUsageChart Instance = new CustomRichTextTypeZoneGroupUsageChart(84, @"ZoneGroupUsageChart", @"Historical Water Usage by Zone", 1);
    }

    public partial class CustomRichTextTypeModalUpdateWaterAccountInformation : CustomRichTextType
    {
        private CustomRichTextTypeModalUpdateWaterAccountInformation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalUpdateWaterAccountInformation Instance = new CustomRichTextTypeModalUpdateWaterAccountInformation(85, @"ModalUpdateWaterAccountInformation", @"Update Water Account Information", 2);
    }

    public partial class CustomRichTextTypeModalCreateNewWaterAccount : CustomRichTextType
    {
        private CustomRichTextTypeModalCreateNewWaterAccount(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalCreateNewWaterAccount Instance = new CustomRichTextTypeModalCreateNewWaterAccount(86, @"ModalCreateNewWaterAccount", @"Create a new Water Account", 2);
    }

    public partial class CustomRichTextTypeModalAddParcelToWaterAccount : CustomRichTextType
    {
        private CustomRichTextTypeModalAddParcelToWaterAccount(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalAddParcelToWaterAccount Instance = new CustomRichTextTypeModalAddParcelToWaterAccount(87, @"ModalAddParcelToWaterAccount", @"Add Parcel to Water Account", 2);
    }

    public partial class CustomRichTextTypeModalMergeWaterAccounts : CustomRichTextType
    {
        private CustomRichTextTypeModalMergeWaterAccounts(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalMergeWaterAccounts Instance = new CustomRichTextTypeModalMergeWaterAccounts(88, @"ModalMergeWaterAccounts", @"Merge Water Accounts", 2);
    }

    public partial class CustomRichTextTypeModalUpdateWaterAccountParcels : CustomRichTextType
    {
        private CustomRichTextTypeModalUpdateWaterAccountParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalUpdateWaterAccountParcels Instance = new CustomRichTextTypeModalUpdateWaterAccountParcels(89, @"ModalUpdateWaterAccountParcels", @"Update Water Account Parcels", 2);
    }

    public partial class CustomRichTextTypeWaterAccountMergeType : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountMergeType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountMergeType Instance = new CustomRichTextTypeWaterAccountMergeType(90, @"WaterAccountMergeType", @"Merge Type", 3);
    }

    public partial class CustomRichTextTypeGeographyAbout : CustomRichTextType
    {
        private CustomRichTextTypeGeographyAbout(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeographyAbout Instance = new CustomRichTextTypeGeographyAbout(91, @"GeographyAbout", @"Geography About", 1);
    }

    public partial class CustomRichTextTypeGeographyAllocations : CustomRichTextType
    {
        private CustomRichTextTypeGeographyAllocations(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeographyAllocations Instance = new CustomRichTextTypeGeographyAllocations(92, @"GeographyAllocations", @"Geography Allocations", 1);
    }

    public partial class CustomRichTextTypeGeographyWaterLevels : CustomRichTextType
    {
        private CustomRichTextTypeGeographyWaterLevels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeographyWaterLevels Instance = new CustomRichTextTypeGeographyWaterLevels(93, @"GeographyWaterLevels", @"Geography Water Levels", 1);
    }

    public partial class CustomRichTextTypeGeographySupport : CustomRichTextType
    {
        private CustomRichTextTypeGeographySupport(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeographySupport Instance = new CustomRichTextTypeGeographySupport(94, @"GeographySupport", @"Geography Support", 1);
    }

    public partial class CustomRichTextTypeMonitoringWellsGrid : CustomRichTextType
    {
        private CustomRichTextTypeMonitoringWellsGrid(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMonitoringWellsGrid Instance = new CustomRichTextTypeMonitoringWellsGrid(95, @"MonitoringWellsGrid", @"MonitoringWellsGrid", 1);
    }

    public partial class CustomRichTextTypeWaterDashboardWaterAccounts : CustomRichTextType
    {
        private CustomRichTextTypeWaterDashboardWaterAccounts(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterDashboardWaterAccounts Instance = new CustomRichTextTypeWaterDashboardWaterAccounts(96, @"WaterDashboardWaterAccounts", @"Water Dashboard Water Accounts", 1);
    }

    public partial class CustomRichTextTypeAddAWellScenario : CustomRichTextType
    {
        private CustomRichTextTypeAddAWellScenario(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAddAWellScenario Instance = new CustomRichTextTypeAddAWellScenario(97, @"AddAWellScenario", @"Add a Well Scenario", 1);
    }

    public partial class CustomRichTextTypeAllocationPlanEdit : CustomRichTextType
    {
        private CustomRichTextTypeAllocationPlanEdit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAllocationPlanEdit Instance = new CustomRichTextTypeAllocationPlanEdit(98, @"AllocationPlanEdit", @"Allocation Plan Edit", 1);
    }

    public partial class CustomRichTextTypeAllocationPlansConfigure : CustomRichTextType
    {
        private CustomRichTextTypeAllocationPlansConfigure(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAllocationPlansConfigure Instance = new CustomRichTextTypeAllocationPlansConfigure(99, @"AllocationPlansConfigure", @"Configure Allocation Plans", 1);
    }

    public partial class CustomRichTextTypeCloneAllocationPlan : CustomRichTextType
    {
        private CustomRichTextTypeCloneAllocationPlan(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeCloneAllocationPlan Instance = new CustomRichTextTypeCloneAllocationPlan(100, @"CloneAllocationPlan", @"Clone Allocation Plan", 1);
    }

    public partial class CustomRichTextTypeAccountAllocationPlans : CustomRichTextType
    {
        private CustomRichTextTypeAccountAllocationPlans(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAccountAllocationPlans Instance = new CustomRichTextTypeAccountAllocationPlans(101, @"AccountAllocationPlans", @"Account Allocation Plans", 1);
    }

    public partial class CustomRichTextTypeRechargeScenario : CustomRichTextType
    {
        private CustomRichTextTypeRechargeScenario(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeRechargeScenario Instance = new CustomRichTextTypeRechargeScenario(102, @"RechargeScenario", @"Recharge Scenario", 1);
    }

    public partial class CustomRichTextTypePostToSupply : CustomRichTextType
    {
        private CustomRichTextTypePostToSupply(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypePostToSupply Instance = new CustomRichTextTypePostToSupply(103, @"PostToSupply", @"Post to Supply", 1);
    }

    public partial class CustomRichTextTypeOpenETSyncVariable : CustomRichTextType
    {
        private CustomRichTextTypeOpenETSyncVariable(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOpenETSyncVariable Instance = new CustomRichTextTypeOpenETSyncVariable(104, @"OpenETSyncVariable", @"OpenET Sync Variable", 3);
    }

    public partial class CustomRichTextTypeLandownerParcelIndex : CustomRichTextType
    {
        private CustomRichTextTypeLandownerParcelIndex(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandownerParcelIndex Instance = new CustomRichTextTypeLandownerParcelIndex(105, @"LandownerParcelIndex", @"Landowner Parcel Index", 1);
    }

    public partial class CustomRichTextTypeScenarioPlanner : CustomRichTextType
    {
        private CustomRichTextTypeScenarioPlanner(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeScenarioPlanner Instance = new CustomRichTextTypeScenarioPlanner(106, @"ScenarioPlanner", @"Scenario Planner", 1);
    }

    public partial class CustomRichTextTypeScenarioPlannerGET : CustomRichTextType
    {
        private CustomRichTextTypeScenarioPlannerGET(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeScenarioPlannerGET Instance = new CustomRichTextTypeScenarioPlannerGET(107, @"ScenarioPlannerGET", @"Scenario Planner GET", 1);
    }

    public partial class CustomRichTextTypeScenarioPlannerScenarioRuns : CustomRichTextType
    {
        private CustomRichTextTypeScenarioPlannerScenarioRuns(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeScenarioPlannerScenarioRuns Instance = new CustomRichTextTypeScenarioPlannerScenarioRuns(108, @"ScenarioPlannerScenarioRuns", @"Scenario Runs", 1);
    }

    public partial class CustomRichTextTypeActivityCenter : CustomRichTextType
    {
        private CustomRichTextTypeActivityCenter(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeActivityCenter Instance = new CustomRichTextTypeActivityCenter(109, @"ActivityCenter", @"Activity Center", 1);
    }

    public partial class CustomRichTextTypeWaterAccountSuggestions : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountSuggestions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountSuggestions Instance = new CustomRichTextTypeWaterAccountSuggestions(110, @"WaterAccountSuggestions", @"Water Account Suggestions", 1);
    }

    public partial class CustomRichTextTypeModalReviewWaterAccountSuggestion : CustomRichTextType
    {
        private CustomRichTextTypeModalReviewWaterAccountSuggestion(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeModalReviewWaterAccountSuggestion Instance = new CustomRichTextTypeModalReviewWaterAccountSuggestion(111, @"ModalReviewWaterAccountSuggestion", @"Review Water Account Suggestion", 2);
    }

    public partial class CustomRichTextTypeWellRegistryConfigurationPage : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryConfigurationPage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryConfigurationPage Instance = new CustomRichTextTypeWellRegistryConfigurationPage(112, @"WellRegistryConfigurationPage", @"Well Registry Configuration Page", 1);
    }

    public partial class CustomRichTextTypeWellRegistryMapYourWell : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryMapYourWell(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryMapYourWell Instance = new CustomRichTextTypeWellRegistryMapYourWell(113, @"WellRegistryMapYourWell", @"Well Registry Map Your Well", 2);
    }

    public partial class CustomRichTextTypeWellRegistryConfirmWellLocation : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryConfirmWellLocation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryConfirmWellLocation Instance = new CustomRichTextTypeWellRegistryConfirmWellLocation(114, @"WellRegistryConfirmWellLocation", @"Well Registry Confirm Well Location", 2);
    }

    public partial class CustomRichTextTypeWellRegistryIrrigatedParcels : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryIrrigatedParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryIrrigatedParcels Instance = new CustomRichTextTypeWellRegistryIrrigatedParcels(115, @"WellRegistryIrrigatedParcels", @"Well Registry Irrigated Parcels", 2);
    }

    public partial class CustomRichTextTypeWellRegistryContacts : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryContacts(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryContacts Instance = new CustomRichTextTypeWellRegistryContacts(116, @"WellRegistryContacts", @"Well Registry Contacts", 2);
    }

    public partial class CustomRichTextTypeWellRegistryBasicInformation : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryBasicInformation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryBasicInformation Instance = new CustomRichTextTypeWellRegistryBasicInformation(117, @"WellRegistryBasicInformation", @"Well Registry Basic Information", 2);
    }

    public partial class CustomRichTextTypeWellRegistrySupportingInformation : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistrySupportingInformation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistrySupportingInformation Instance = new CustomRichTextTypeWellRegistrySupportingInformation(118, @"WellRegistrySupportingInformation", @"Well Registry Supporting Information", 2);
    }

    public partial class CustomRichTextTypeWellRegistryAttachments : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryAttachments(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryAttachments Instance = new CustomRichTextTypeWellRegistryAttachments(119, @"WellRegistryAttachments", @"Well Registry Attachments", 2);
    }

    public partial class CustomRichTextTypeWellRegistrySubmit : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistrySubmit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistrySubmit Instance = new CustomRichTextTypeWellRegistrySubmit(120, @"WellRegistrySubmit", @"Well Registry Submit", 2);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWellName : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWellName(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWellName Instance = new CustomRichTextTypeWellRegistryFieldWellName(121, @"WellRegistryFieldWellName", @"WellRegistryFieldWellName", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldSWN : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldSWN(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldSWN Instance = new CustomRichTextTypeWellRegistryFieldSWN(122, @"WellRegistryFieldSWN", @"WellRegistryFieldSWN", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWCR : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWCR(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWCR Instance = new CustomRichTextTypeWellRegistryFieldWCR(123, @"WellRegistryFieldWCR", @"WellRegistryFieldWCR", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldCountyWellPermit : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldCountyWellPermit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldCountyWellPermit Instance = new CustomRichTextTypeWellRegistryFieldCountyWellPermit(124, @"WellRegistryFieldCountyWellPermit", @"WellRegistryFieldCountyWellPermit", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldDateDrilled : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldDateDrilled(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldDateDrilled Instance = new CustomRichTextTypeWellRegistryFieldDateDrilled(125, @"WellRegistryFieldDateDrilled", @"WellRegistryFieldDateDrilled", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionAgricultural(126, @"WellRegistryFieldWaterUseDescriptionAgricultural", @"WellRegistryFieldWaterUseDescriptionAgricultural", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionStockWatering(127, @"WellRegistryFieldWaterUseDescriptionStockWatering", @"WellRegistryFieldWaterUseDescriptionStockWatering", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionDomestic(128, @"WellRegistryFieldWaterUseDescriptionDomestic", @"WellRegistryFieldWaterUseDescriptionDomestic", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPublicMunicipal(129, @"WellRegistryFieldWaterUseDescriptionPublicMunicipal", @"WellRegistryFieldWaterUseDescriptionPublicMunicipal", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionPrivateMunicipal(130, @"WellRegistryFieldWaterUseDescriptionPrivateMunicipal", @"WellRegistryFieldWaterUseDescriptionPrivateMunicipal", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther Instance = new CustomRichTextTypeWellRegistryFieldWaterUseDescriptionOther(131, @"WellRegistryFieldWaterUseDescriptionOther", @"WellRegistryFieldWaterUseDescriptionOther", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldTopOfPerforations : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldTopOfPerforations(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldTopOfPerforations Instance = new CustomRichTextTypeWellRegistryFieldTopOfPerforations(132, @"WellRegistryFieldTopOfPerforations", @"WellRegistryFieldTopOfPerforations", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldBottomOfPerforations : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldBottomOfPerforations(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldBottomOfPerforations Instance = new CustomRichTextTypeWellRegistryFieldBottomOfPerforations(133, @"WellRegistryFieldBottomOfPerforations", @"WellRegistryFieldBottomOfPerforations", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldMaxiumumFlow : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldMaxiumumFlow(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldMaxiumumFlow Instance = new CustomRichTextTypeWellRegistryFieldMaxiumumFlow(134, @"WellRegistryFieldMaxiumumFlow", @"WellRegistryFieldMaxiumumFlow", 3);
    }

    public partial class CustomRichTextTypeWellRegistryFieldTypicalFlow : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryFieldTypicalFlow(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryFieldTypicalFlow Instance = new CustomRichTextTypeWellRegistryFieldTypicalFlow(135, @"WellRegistryFieldTypicalFlow", @"WellRegistryFieldTypicalFlow", 3);
    }

    public partial class CustomRichTextTypeManageReviewSubmittedWells : CustomRichTextType
    {
        private CustomRichTextTypeManageReviewSubmittedWells(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeManageReviewSubmittedWells Instance = new CustomRichTextTypeManageReviewSubmittedWells(136, @"ManageReviewSubmittedWells", @"ManageReviewSubmittedWells", 1);
    }

    public partial class CustomRichTextTypeLandownerWellList : CustomRichTextType
    {
        private CustomRichTextTypeLandownerWellList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandownerWellList Instance = new CustomRichTextTypeLandownerWellList(137, @"LandownerWellList", @"Landowner Well List", 1);
    }

    public partial class CustomRichTextTypeManageAllWellRegistrations : CustomRichTextType
    {
        private CustomRichTextTypeManageAllWellRegistrations(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeManageAllWellRegistrations Instance = new CustomRichTextTypeManageAllWellRegistrations(138, @"ManageAllWellRegistrations", @"ManageAllWellRegistrations", 1);
    }

    public partial class CustomRichTextTypeFormAsteriskExplanation : CustomRichTextType
    {
        private CustomRichTextTypeFormAsteriskExplanation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFormAsteriskExplanation Instance = new CustomRichTextTypeFormAsteriskExplanation(139, @"FormAsteriskExplanation", @"FormAsteriskExplanation", 1);
    }

    public partial class CustomRichTextTypeWellRegistryIncompleteText : CustomRichTextType
    {
        private CustomRichTextTypeWellRegistryIncompleteText(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellRegistryIncompleteText Instance = new CustomRichTextTypeWellRegistryIncompleteText(140, @"WellRegistryIncompleteText", @"WellRegistryIncompleteText", 1);
    }

    public partial class CustomRichTextTypeReferenceWellsList : CustomRichTextType
    {
        private CustomRichTextTypeReferenceWellsList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeReferenceWellsList Instance = new CustomRichTextTypeReferenceWellsList(141, @"ReferenceWellsList", @"Reference Wells List", 1);
    }

    public partial class CustomRichTextTypeReferenceWellsUploader : CustomRichTextType
    {
        private CustomRichTextTypeReferenceWellsUploader(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeReferenceWellsUploader Instance = new CustomRichTextTypeReferenceWellsUploader(142, @"ReferenceWellsUploader", @"Reference Wells Uploader", 2);
    }

    public partial class CustomRichTextTypeLandingPageBody : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageBody(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageBody Instance = new CustomRichTextTypeLandingPageBody(143, @"LandingPageBody", @"Landing Page Body", 1);
    }

    public partial class CustomRichTextTypeLandingPageUserCard : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageUserCard(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageUserCard Instance = new CustomRichTextTypeLandingPageUserCard(144, @"LandingPageUserCard", @"Landing Page User Card", 1);
    }

    public partial class CustomRichTextTypeLandingPageParcelCard : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageParcelCard(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageParcelCard Instance = new CustomRichTextTypeLandingPageParcelCard(145, @"LandingPageParcelCard", @"Landing Page Parcel Card", 1);
    }

    public partial class CustomRichTextTypeLandingPageWellCard : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageWellCard(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageWellCard Instance = new CustomRichTextTypeLandingPageWellCard(146, @"LandingPageWellCard", @"Landing Page Well Card", 1);
    }

    public partial class CustomRichTextTypeLandingPageWaterAccountCard : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageWaterAccountCard(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageWaterAccountCard Instance = new CustomRichTextTypeLandingPageWaterAccountCard(147, @"LandingPageWaterAccountCard", @"Landing Page Water Account Card", 1);
    }

    public partial class CustomRichTextTypeLandingPageOverview : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageOverview(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageOverview Instance = new CustomRichTextTypeLandingPageOverview(148, @"LandingPageOverview", @"Landing Page Overview", 1);
    }

    public partial class CustomRichTextTypeLandingPageAllocationPlans : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageAllocationPlans(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageAllocationPlans Instance = new CustomRichTextTypeLandingPageAllocationPlans(149, @"LandingPageAllocationPlans", @"Landing Page Allocation Plans", 1);
    }

    public partial class CustomRichTextTypeLandingPageWaterLevels : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageWaterLevels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageWaterLevels Instance = new CustomRichTextTypeLandingPageWaterLevels(150, @"LandingPageWaterLevels", @"Landing Page Water Levels", 1);
    }

    public partial class CustomRichTextTypeLandingPageContact : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageContact(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageContact Instance = new CustomRichTextTypeLandingPageContact(151, @"LandingPageContact", @"Landing Page Contact", 1);
    }

    public partial class CustomRichTextTypeConfigureLandingPage : CustomRichTextType
    {
        private CustomRichTextTypeConfigureLandingPage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeConfigureLandingPage Instance = new CustomRichTextTypeConfigureLandingPage(152, @"ConfigureLandingPage", @"ConfigureLandingPage", 1);
    }

    public partial class CustomRichTextTypeHomepageUpdateProfileLink : CustomRichTextType
    {
        private CustomRichTextTypeHomepageUpdateProfileLink(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomepageUpdateProfileLink Instance = new CustomRichTextTypeHomepageUpdateProfileLink(153, @"HomepageUpdateProfileLink", @"Homepage Update Profile Link", 1);
    }

    public partial class CustomRichTextTypeHomepageGrowerGuideLink : CustomRichTextType
    {
        private CustomRichTextTypeHomepageGrowerGuideLink(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomepageGrowerGuideLink Instance = new CustomRichTextTypeHomepageGrowerGuideLink(154, @"HomepageGrowerGuideLink", @"Homepage Grower Guide Link", 1);
    }

    public partial class CustomRichTextTypeHomepageGeographiesLink : CustomRichTextType
    {
        private CustomRichTextTypeHomepageGeographiesLink(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomepageGeographiesLink Instance = new CustomRichTextTypeHomepageGeographiesLink(155, @"HomepageGeographiesLink", @"Homepage Geographies Link", 1);
    }

    public partial class CustomRichTextTypeHomepageClaimWaterAccountsPanel : CustomRichTextType
    {
        private CustomRichTextTypeHomepageClaimWaterAccountsPanel(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeHomepageClaimWaterAccountsPanel Instance = new CustomRichTextTypeHomepageClaimWaterAccountsPanel(156, @"HomepageClaimWaterAccountsPanel", @"Homepage Claim Water Accounts Panel", 1);
    }

    public partial class CustomRichTextTypeParcelStatus : CustomRichTextType
    {
        private CustomRichTextTypeParcelStatus(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeParcelStatus Instance = new CustomRichTextTypeParcelStatus(157, @"ParcelStatus", @"Parcel Status", 3);
    }

    public partial class CustomRichTextTypeOnboardOverviewContent : CustomRichTextType
    {
        private CustomRichTextTypeOnboardOverviewContent(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeOnboardOverviewContent Instance = new CustomRichTextTypeOnboardOverviewContent(158, @"OnboardOverviewContent", @"Onboard Overview Content", 1);
    }

    public partial class CustomRichTextTypeParcelBulkActions : CustomRichTextType
    {
        private CustomRichTextTypeParcelBulkActions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeParcelBulkActions Instance = new CustomRichTextTypeParcelBulkActions(159, @"ParcelBulkActions", @"Parcel Bulk Actions", 1);
    }

    public partial class CustomRichTextTypeMeterList : CustomRichTextType
    {
        private CustomRichTextTypeMeterList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMeterList Instance = new CustomRichTextTypeMeterList(160, @"MeterList", @"Meter List", 1);
    }

    public partial class CustomRichTextTypeSerialNumber : CustomRichTextType
    {
        private CustomRichTextTypeSerialNumber(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeSerialNumber Instance = new CustomRichTextTypeSerialNumber(161, @"SerialNumber", @"Serial Number", 3);
    }

    public partial class CustomRichTextTypeMeterConfiguration : CustomRichTextType
    {
        private CustomRichTextTypeMeterConfiguration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMeterConfiguration Instance = new CustomRichTextTypeMeterConfiguration(162, @"MeterConfiguration", @"Meter Configuration", 1);
    }

    public partial class CustomRichTextTypeAcknowledgements : CustomRichTextType
    {
        private CustomRichTextTypeAcknowledgements(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAcknowledgements Instance = new CustomRichTextTypeAcknowledgements(163, @"Acknowledgements", @"Acknowledgements", 1);
    }

    public partial class CustomRichTextTypeAdminGeographyEditForm : CustomRichTextType
    {
        private CustomRichTextTypeAdminGeographyEditForm(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAdminGeographyEditForm Instance = new CustomRichTextTypeAdminGeographyEditForm(164, @"AdminGeographyEditForm", @"Admin Geography Edit Form", 1);
    }

    public partial class CustomRichTextTypeLandingPageConfigure : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageConfigure(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageConfigure Instance = new CustomRichTextTypeLandingPageConfigure(165, @"LandingPageConfigure", @"Landing Page Configure", 1);
    }

    public partial class CustomRichTextTypeMeterDataConfigure : CustomRichTextType
    {
        private CustomRichTextTypeMeterDataConfigure(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMeterDataConfigure Instance = new CustomRichTextTypeMeterDataConfigure(166, @"MeterDataConfigure", @"Meter Data Configure", 1);
    }

    public partial class CustomRichTextTypeAllocationPlanConfigureCard : CustomRichTextType
    {
        private CustomRichTextTypeAllocationPlanConfigureCard(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAllocationPlanConfigureCard Instance = new CustomRichTextTypeAllocationPlanConfigureCard(167, @"AllocationPlanConfigureCard", @"Allocation Plan Configure Card", 1);
    }

    public partial class CustomRichTextTypeWellStatus : CustomRichTextType
    {
        private CustomRichTextTypeWellStatus(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWellStatus Instance = new CustomRichTextTypeWellStatus(168, @"WellStatus", @"Well Status", 2);
    }

    public partial class CustomRichTextTypeUpdateWellInfo : CustomRichTextType
    {
        private CustomRichTextTypeUpdateWellInfo(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateWellInfo Instance = new CustomRichTextTypeUpdateWellInfo(169, @"UpdateWellInfo", @"Update Well Info", 1);
    }

    public partial class CustomRichTextTypeUpdateWellLocation : CustomRichTextType
    {
        private CustomRichTextTypeUpdateWellLocation(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateWellLocation Instance = new CustomRichTextTypeUpdateWellLocation(170, @"UpdateWellLocation", @"UpdateWellLocation", 1);
    }

    public partial class CustomRichTextTypeUpdateWellIrrigatedParcels : CustomRichTextType
    {
        private CustomRichTextTypeUpdateWellIrrigatedParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeUpdateWellIrrigatedParcels Instance = new CustomRichTextTypeUpdateWellIrrigatedParcels(171, @"UpdateWellIrrigatedParcels", @"Update Well Irrigated Parcels", 1);
    }

    public partial class CustomRichTextTypeAdminFAQ : CustomRichTextType
    {
        private CustomRichTextTypeAdminFAQ(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeAdminFAQ Instance = new CustomRichTextTypeAdminFAQ(172, @"AdminFAQ", @"Admin Frequently Asked Questions", 1);
    }

    public partial class CustomRichTextTypeGeneralFAQ : CustomRichTextType
    {
        private CustomRichTextTypeGeneralFAQ(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeGeneralFAQ Instance = new CustomRichTextTypeGeneralFAQ(173, @"GeneralFAQ", @"General Frequently Asked Questions", 1);
    }

    public partial class CustomRichTextTypeWaterAccountCustomAttributesEdit : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountCustomAttributesEdit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountCustomAttributesEdit Instance = new CustomRichTextTypeWaterAccountCustomAttributesEdit(174, @"WaterAccountCustomAttributesEdit", @"Edit Water Account Attributes", 1);
    }

    public partial class CustomRichTextTypeParcelCustomAttributesEdit : CustomRichTextType
    {
        private CustomRichTextTypeParcelCustomAttributesEdit(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeParcelCustomAttributesEdit Instance = new CustomRichTextTypeParcelCustomAttributesEdit(175, @"ParcelCustomAttributesEdit", @"Edit Parcel Attributes", 1);
    }

    public partial class CustomRichTextTypeWaterDashboardLink : CustomRichTextType
    {
        private CustomRichTextTypeWaterDashboardLink(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterDashboardLink Instance = new CustomRichTextTypeWaterDashboardLink(176, @"WaterDashboardLink", @"Water Dashboard Link", 1);
    }

    public partial class CustomRichTextTypeWaterManagerGuideLink : CustomRichTextType
    {
        private CustomRichTextTypeWaterManagerGuideLink(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterManagerGuideLink Instance = new CustomRichTextTypeWaterManagerGuideLink(177, @"WaterManagerGuideLink", @"Water Manager Guide Link", 1);
    }

    public partial class CustomRichTextTypeReviewParcelChanges : CustomRichTextType
    {
        private CustomRichTextTypeReviewParcelChanges(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeReviewParcelChanges Instance = new CustomRichTextTypeReviewParcelChanges(178, @"ReviewParcelChanges", @"Review Parcel Changes", 1);
    }

    public partial class CustomRichTextTypeWaterAccountRequestChanges : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountRequestChanges(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountRequestChanges Instance = new CustomRichTextTypeWaterAccountRequestChanges(179, @"WaterAccountRequestChanges", @"Request Water Account Changes", 1);
    }

    public partial class CustomRichTextTypeWaterAccountRequestChangesCertification : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountRequestChangesCertification(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterAccountRequestChangesCertification Instance = new CustomRichTextTypeWaterAccountRequestChangesCertification(180, @"WaterAccountRequestChangesCertification", @"Request Water Account Changes Certification", 1);
    }

    public partial class CustomRichTextTypeConsolidateWaterAccountsDisclaimer : CustomRichTextType
    {
        private CustomRichTextTypeConsolidateWaterAccountsDisclaimer(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeConsolidateWaterAccountsDisclaimer Instance = new CustomRichTextTypeConsolidateWaterAccountsDisclaimer(181, @"ConsolidateWaterAccountsDisclaimer", @"Consolidate Water Accounts Disclaimer", 1);
    }

    public partial class CustomRichTextTypeKernScenarioModel : CustomRichTextType
    {
        private CustomRichTextTypeKernScenarioModel(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeKernScenarioModel Instance = new CustomRichTextTypeKernScenarioModel(182, @"KernScenarioModel", @"Kern Scenario Model", 1);
    }

    public partial class CustomRichTextTypeMercedWaterResourcesModel : CustomRichTextType
    {
        private CustomRichTextTypeMercedWaterResourcesModel(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeMercedWaterResourcesModel Instance = new CustomRichTextTypeMercedWaterResourcesModel(183, @"MercedWaterResourcesModel", @"Merced Water Resources Model", 1);
    }

    public partial class CustomRichTextTypeYoloScenarioModel : CustomRichTextType
    {
        private CustomRichTextTypeYoloScenarioModel(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeYoloScenarioModel Instance = new CustomRichTextTypeYoloScenarioModel(184, @"YoloScenarioModel", @"Yolo Scenario Model", 1);
    }

    public partial class CustomRichTextTypeConfigureGeographySetup : CustomRichTextType
    {
        private CustomRichTextTypeConfigureGeographySetup(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeConfigureGeographySetup Instance = new CustomRichTextTypeConfigureGeographySetup(185, @"ConfigureGeographySetup", @"Configure Geography Setup", 1);
    }

    public partial class CustomRichTextTypeConfigureCustomAttributes : CustomRichTextType
    {
        private CustomRichTextTypeConfigureCustomAttributes(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeConfigureCustomAttributes Instance = new CustomRichTextTypeConfigureCustomAttributes(186, @"ConfigureCustomAttributes", @"Configure Custom Attributes", 1);
    }

    public partial class CustomRichTextTypeConfigureWaterManagers : CustomRichTextType
    {
        private CustomRichTextTypeConfigureWaterManagers(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeConfigureWaterManagers Instance = new CustomRichTextTypeConfigureWaterManagers(187, @"ConfigureWaterManagers", @"Configure Water Managers", 1);
    }

    public partial class CustomRichTextTypeRasterUploadGuidance : CustomRichTextType
    {
        private CustomRichTextTypeRasterUploadGuidance(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeRasterUploadGuidance Instance = new CustomRichTextTypeRasterUploadGuidance(188, @"RasterUploadGuidance", @"Raster Upload Guidance", 1);
    }

    public partial class CustomRichTextTypeWaterDashboardParcels : CustomRichTextType
    {
        private CustomRichTextTypeWaterDashboardParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterDashboardParcels Instance = new CustomRichTextTypeWaterDashboardParcels(189, @"WaterDashboardParcels", @"Water Dashboard Parcels", 1);
    }

    public partial class CustomRichTextTypeWaterDashboardWells : CustomRichTextType
    {
        private CustomRichTextTypeWaterDashboardWells(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeWaterDashboardWells Instance = new CustomRichTextTypeWaterDashboardWells(190, @"WaterDashboardWells", @"Water Dashboard Wells", 1);
    }

    public partial class CustomRichTextTypeRequestSupport : CustomRichTextType
    {
        private CustomRichTextTypeRequestSupport(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeRequestSupport Instance = new CustomRichTextTypeRequestSupport(191, @"RequestSupport", @"Request Support", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorStepOne : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorStepOne(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorStepOne Instance = new CustomRichTextTypeFeeCalculatorStepOne(192, @"FeeCalculatorStepOne", @"Fee Calculator Step One", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorSurfaceWater : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorSurfaceWater(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorSurfaceWater Instance = new CustomRichTextTypeFeeCalculatorSurfaceWater(193, @"FeeCalculatorSurfaceWater", @"DO YOU RECEIVE SURFACE WATER?", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorStepTwo : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorStepTwo(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorStepTwo Instance = new CustomRichTextTypeFeeCalculatorStepTwo(194, @"FeeCalculatorStepTwo", @"Fee Calculator Step Two", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorStepThree : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorStepThree(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorStepThree Instance = new CustomRichTextTypeFeeCalculatorStepThree(195, @"FeeCalculatorStepThree", @"Fee Calculator Step Three", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorAboutFeeStructures : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorAboutFeeStructures(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorAboutFeeStructures Instance = new CustomRichTextTypeFeeCalculatorAboutFeeStructures(196, @"FeeCalculatorAboutFeeStructures", @"Fee Calculator About Fee Structures", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorIncentivePayment : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorIncentivePayment(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorIncentivePayment Instance = new CustomRichTextTypeFeeCalculatorIncentivePayment(197, @"FeeCalculatorIncentivePayment", @"Incentive Payment", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater Instance = new CustomRichTextTypeFeeCalculatorWhatIsConsumedGroundwater(198, @"FeeCalculatorWhatIsConsumedGroundwater", @"What is Consumed Groundwater?", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorYourData : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorYourData(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorYourData Instance = new CustomRichTextTypeFeeCalculatorYourData(199, @"FeeCalculatorYourData", @"Your Data", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorAcres : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorAcres(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorAcres Instance = new CustomRichTextTypeFeeCalculatorAcres(200, @"FeeCalculatorAcres", @"Acres", 1);
    }

    public partial class CustomRichTextTypeFeeCalculatorFallowingSelfDirected : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorFallowingSelfDirected(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorFallowingSelfDirected Instance = new CustomRichTextTypeFeeCalculatorFallowingSelfDirected(201, @"FeeCalculatorFallowingSelfDirected", @"Fallowing (Self-Directed)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected Instance = new CustomRichTextTypeFeeCalculatorCoverCroppingSelfDirected(202, @"FeeCalculatorCoverCroppingSelfDirected", @"Cover Cropping (Self-Directed)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram Instance = new CustomRichTextTypeFeeCalculatorTemporaryFallowingLandFallowingProgram(203, @"FeeCalculatorTemporaryFallowingLandFallowingProgram", @"Temporary Fallowing (Land Fallowing Program)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP Instance = new CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingMLRP(204, @"FeeCalculatorRotationalExtendedFallowingMLRP", @"Rotational Extended Fallowing (MLRP)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP Instance = new CustomRichTextTypeFeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP(205, @"FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP", @"Rotational Extended Fallowing in Designated Buffer Zones (MLRP)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP Instance = new CustomRichTextTypeFeeCalculatorOrchardSwaleRewildingMLRP(206, @"FeeCalculatorOrchardSwaleRewildingMLRP", @"Orchard Swale Rewilding (MLRP)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP Instance = new CustomRichTextTypeFeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP(207, @"FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP", @"Floodplain Reconnection and Related Spreading and Recharge (MLRP)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP Instance = new CustomRichTextTypeFeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP(208, @"FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP", @"Floodflow Spreading on Non-Floodplain Lands (MLRP)", 3);
    }

    public partial class CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP : CustomRichTextType
    {
        private CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP Instance = new CustomRichTextTypeFeeCalculatorStorageOrRechargeBasinsMLRP(209, @"FeeCalculatorStorageOrRechargeBasinsMLRP", @"Storage or Recharge Basins (MLRP)", 3);
    }

    public partial class CustomRichTextTypeLandingPageFeeCalculator : CustomRichTextType
    {
        private CustomRichTextTypeLandingPageFeeCalculator(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeLandingPageFeeCalculator Instance = new CustomRichTextTypeLandingPageFeeCalculator(210, @"LandingPageFeeCalculator", @"Fee Calculator", 1);
    }

    public partial class CustomRichTextTypeNewsAndAnnouncements : CustomRichTextType
    {
        private CustomRichTextTypeNewsAndAnnouncements(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeNewsAndAnnouncements Instance = new CustomRichTextTypeNewsAndAnnouncements(211, @"NewsAndAnnouncements", @"News and Announcements", 1);
    }

    public partial class CustomRichTextTypeReviewSelfReportList : CustomRichTextType
    {
        private CustomRichTextTypeReviewSelfReportList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeReviewSelfReportList Instance = new CustomRichTextTypeReviewSelfReportList(212, @"ReviewSelfReportList", @"Review Self Report List", 1);
    }

    public partial class CustomRichTextTypeSubmitSelfReportDisclaimer : CustomRichTextType
    {
        private CustomRichTextTypeSubmitSelfReportDisclaimer(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeSubmitSelfReportDisclaimer Instance = new CustomRichTextTypeSubmitSelfReportDisclaimer(213, @"SubmitSelfReportDisclaimer", @"Submit Self Report Disclaimer", 1);
    }

    public partial class CustomRichTextTypeSelfReportEditorInstructions : CustomRichTextType
    {
        private CustomRichTextTypeSelfReportEditorInstructions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName, int? contentTypeID) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName, contentTypeID) {}
        public static readonly CustomRichTextTypeSelfReportEditorInstructions Instance = new CustomRichTextTypeSelfReportEditorInstructions(214, @"SelfReportEditorInstructions", @"Self Report Editor Instructions", 1);
    }
}