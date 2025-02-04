//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[UnitType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum UnitTypeEnum {
  Inches = 1,
  Millimeters = 2
}

export const UnitTypes: LookupTableEntry[]  = [
  { Name: "Inches", DisplayName: "inches", Value: 1 },
  { Name: "Millimeters", DisplayName: "millimeters", Value: 2 }
];
export const UnitTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...UnitTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
