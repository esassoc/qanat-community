//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[OpenETDataType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum OpenETDataTypeEnum {
  Evapotranspiration = 1,
  Precipitation = 2
}

export const OpenETDataTypes: LookupTableEntry[]  = [
  { Name: "Evapotranspiration", DisplayName: "Evapotranspiration", Value: 1 },
  { Name: "Precipitation", DisplayName: "Precipitation", Value: 2 }
];
export const OpenETDataTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...OpenETDataTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
