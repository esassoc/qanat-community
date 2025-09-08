//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ScenarioRunOutputFileType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ScenarioRunOutputFileTypeEnum {
  GroundWaterBudget = 1,
  TimeSeriesData = 2,
  WaterBudget = 3,
  PointsofInterest = 4
}

export const ScenarioRunOutputFileTypes: LookupTableEntry[]  = [
  { Name: "GroundWaterBudget", DisplayName: "GroundWaterBudget", Value: 1 },
  { Name: "TimeSeriesData", DisplayName: "TimeSeriesData", Value: 2 },
  { Name: "Water Budget", DisplayName: "Water Budget", Value: 3 },
  { Name: "Points of Interest", DisplayName: "Points of Interest", Value: 4 }
];
export const ScenarioRunOutputFileTypesAsSelectDropdownOptions = ScenarioRunOutputFileTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
