//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WaterMeasurementCategoryType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WaterMeasurementCategoryTypeEnum {
  ET = 1,
  Precip = 2,
  Meter = 3,
  SurfaceWater = 4,
  Calculated = 5,
  PrecipitationCredit = 6,
  ManualAdjustment = 7,
  SelfReported = 8
}

export const WaterMeasurementCategoryTypes: LookupTableEntry[]  = [
  { Name: "ET", DisplayName: "ET", Value: 1 },
  { Name: "Precip", DisplayName: "Precip", Value: 2 },
  { Name: "Meter", DisplayName: "Meter", Value: 3 },
  { Name: "SurfaceWater", DisplayName: "Surface Water", Value: 4 },
  { Name: "Calculated", DisplayName: "Calculated", Value: 5 },
  { Name: "Precipitation Credit", DisplayName: "PrecipitationCredit", Value: 6 },
  { Name: "Manual Adjustment", DisplayName: "ManualAdjustment", Value: 7 },
  { Name: "Self Reported", DisplayName: "SelfReported", Value: 8 }
];
export const WaterMeasurementCategoryTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WaterMeasurementCategoryTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
