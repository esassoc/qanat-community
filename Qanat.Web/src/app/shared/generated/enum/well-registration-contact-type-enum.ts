//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WellRegistrationContactType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WellRegistrationContactTypeEnum {
  Landowner = 1,
  OwnerOperator = 2
}

export const WellRegistrationContactTypes: LookupTableEntry[]  = [
  { Name: "Landowner", DisplayName: "Landowner", Value: 1 },
  { Name: "OwnerOperator", DisplayName: "OwnerOperator", Value: 2 }
];
export const WellRegistrationContactTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WellRegistrationContactTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
