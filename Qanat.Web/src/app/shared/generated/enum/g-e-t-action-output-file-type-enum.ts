//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GETActionOutputFileType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum GETActionOutputFileTypeEnum {
  GroundWaterBudget = 1,
  TimeSeriesData = 2,
  WaterBudget = 3,
  PointsofInterest = 4
}

export const GETActionOutputFileTypes: LookupTableEntry[]  = [
  { Name: "GroundWaterBudget", DisplayName: "GroundWaterBudget", Value: 1 },
  { Name: "TimeSeriesData", DisplayName: "TimeSeriesData", Value: 2 },
  { Name: "Water Budget", DisplayName: "Water Budget", Value: 3 },
  { Name: "Points of Interest", DisplayName: "Points of Interest", Value: 4 }
];
export const GETActionOutputFileTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...GETActionOutputFileTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
