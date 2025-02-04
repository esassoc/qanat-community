//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[OpenETRasterCalculationResultType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum OpenETRasterCalculationResultTypeEnum {
  NotStarted = 1,
  InProgress = 2,
  Succeeded = 3,
  Failed = 4
}

export const OpenETRasterCalculationResultTypes: LookupTableEntry[]  = [
  { Name: "NotStarted", DisplayName: "Not Started", Value: 1 },
  { Name: "InProgress", DisplayName: "In Progress", Value: 2 },
  { Name: "Succeeded", DisplayName: "Succeeded", Value: 3 },
  { Name: "Failed", DisplayName: "Failed", Value: 4 }
];
export const OpenETRasterCalculationResultTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...OpenETRasterCalculationResultTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
