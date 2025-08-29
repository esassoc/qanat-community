//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FallowType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FallowTypeEnum {
  Active = 1,
  Permanently = 2,
  Temporarily = 3
}

export const FallowTypes: LookupTableEntry[]  = [
  { Name: "Active", DisplayName: "Active", Value: 1 },
  { Name: "Permanently", DisplayName: "Permanently", Value: 2 },
  { Name: "Temporarily", DisplayName: "Temporarily", Value: 3 }
];
export const FallowTypesAsSelectDropdownOptions = FallowTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
