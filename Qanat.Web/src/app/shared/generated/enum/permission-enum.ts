//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Permission]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum PermissionEnum {
  CustomRichTextRights = 2,
  FieldDefinitionRights = 3,
  FileResourceRights = 4,
  UserRights = 5,
  WaterAccountRights = 6,
  ParcelRights = 7,
  TagRights = 8,
  WellRights = 9,
  WaterTransactionRights = 10,
  ReportingPeriodRights = 12,
  WaterTypeRights = 13,
  GeographyRights = 14,
  ExternalMapLayerRights = 15,
  WaterAccountUserRights = 16,
  ZoneGroupRights = 17,
  MonitoringWellRights = 18,
  AllocationPlanRights = 19,
  FrequentlyAskedQuestionRights = 20,
  CustomAttributeRights = 21,
  UsageLocationRights = 22,
  ModelRights = 23,
  ScenarioRights = 24,
  ScenarioRunRights = 25,
  MeterRights = 26,
  WellMeterReadingRights = 27,
  StatementTemplateRights = 28
}

export const Permissions: LookupTableEntry[]  = [
  { Name: "CustomRichTextRights", DisplayName: "CustomRichTextRights", Value: 2 },
  { Name: "FieldDefinitionRights", DisplayName: "FieldDefinitionRights", Value: 3 },
  { Name: "FileResourceRights", DisplayName: "FileResourceRights", Value: 4 },
  { Name: "UserRights", DisplayName: "UserRights", Value: 5 },
  { Name: "WaterAccountRights", DisplayName: "WaterAccountRights", Value: 6 },
  { Name: "ParcelRights", DisplayName: "ParcelRights", Value: 7 },
  { Name: "TagRights", DisplayName: "TagRights", Value: 8 },
  { Name: "WellRights", DisplayName: "WellRights", Value: 9 },
  { Name: "WaterTransactionRights", DisplayName: "WaterTransactionRights", Value: 10 },
  { Name: "ReportingPeriodRights", DisplayName: "ReportingPeriodRights", Value: 12 },
  { Name: "WaterTypeRights", DisplayName: "WaterTypeRights", Value: 13 },
  { Name: "GeographyRights", DisplayName: "GeographyRights", Value: 14 },
  { Name: "ExternalMapLayerRights", DisplayName: "ExternalMapLayerRights", Value: 15 },
  { Name: "WaterAccountUserRights", DisplayName: "WaterAccountUserRights", Value: 16 },
  { Name: "ZoneGroupRights", DisplayName: "ZoneGroupRights", Value: 17 },
  { Name: "MonitoringWellRights", DisplayName: "MonitoringWellRights", Value: 18 },
  { Name: "AllocationPlanRights", DisplayName: "AllocationPlanRights", Value: 19 },
  { Name: "FrequentlyAskedQuestionRights", DisplayName: "FrequentlyAskedQuestionRights", Value: 20 },
  { Name: "CustomAttributeRights", DisplayName: "CustomAttributeRights", Value: 21 },
  { Name: "UsageLocationRights", DisplayName: "UsageLocationRights", Value: 22 },
  { Name: "ModelRights", DisplayName: "ModelRights", Value: 23 },
  { Name: "ScenarioRights", DisplayName: "ScenarioRights", Value: 24 },
  { Name: "ScenarioRunRights", DisplayName: "ScenarioRunRights", Value: 25 },
  { Name: "MeterRights", DisplayName: "MeterRights", Value: 26 },
  { Name: "WellMeterReadingRights", DisplayName: "WellMeterReadingRights", Value: 27 },
  { Name: "StatementTemplateRights", DisplayName: "StatementTemplateRights", Value: 28 }
];
export const PermissionsAsSelectDropdownOptions = Permissions.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
