//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WaterMeasurementCalculationType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WaterMeasurementCalculationTypeEnum {
  CalculateEffectivePrecip = 1,
  CalculateSurfaceWaterConsumption = 2,
  ETMinusPrecipMinusTotalSurfaceWater = 3,
  CalculatePrecipitationCreditOffset = 4,
  CalculatePositiveConsumedGroundwater = 5,
  CalculateUnadjustedExtractedGroundwater = 6,
  CalculateExtractedGroundwater = 7,
  CalculateExtractedAgainstSupply = 8,
  CalculateOpenETConsumptiveUse = 9
}

export const WaterMeasurementCalculationTypes: LookupTableEntry[]  = [
  { Name: "CalculateEffectivePrecip", DisplayName: "Calculate Effective Precip", Value: 1 },
  { Name: "CalculateSurfaceWaterConsumption", DisplayName: "Calculate SurfaceWater Consumption", Value: 2 },
  { Name: "ETMinusPrecipMinusTotalSurfaceWater", DisplayName: "ET - Precip - TotalSurfaceWater", Value: 3 },
  { Name: "CalculatePrecipitationCreditOffset", DisplayName: "Calculate Precipitation Credit Offset", Value: 4 },
  { Name: "CalculatePositiveConsumedGroundwater", DisplayName: "Calculate Positive Consumed Groundwater", Value: 5 },
  { Name: "CalculateUnadjustedExtractedGroundwater", DisplayName: "Calculate Unadjusted Extracted Groundwater", Value: 6 },
  { Name: "CalculateExtractedGroundwater", DisplayName: "Calculate Extracted Groundwater", Value: 7 },
  { Name: "CalculateExtractedAgainstSupply", DisplayName: "Calculate Extracted Against Supply", Value: 8 },
  { Name: "CalculateOpenETConsumptiveUse", DisplayName: "Calculate Open ET Consumptive Use", Value: 9 }
];
export const WaterMeasurementCalculationTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WaterMeasurementCalculationTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
