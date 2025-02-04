//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FuelType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum FuelTypeEnum {
  Electric = 1,
  Diesel = 2,
  LPGas = 3,
  Other = 4
}

export const FuelTypes: LookupTableEntry[]  = [
  { Name: "Electric", DisplayName: "Electric", Value: 1 },
  { Name: "Diesel", DisplayName: "Diesel", Value: 2 },
  { Name: "LP Gas", DisplayName: "LP Gas", Value: 3 },
  { Name: "Other", DisplayName: "Other", Value: 4 }
];
export const FuelTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...FuelTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
