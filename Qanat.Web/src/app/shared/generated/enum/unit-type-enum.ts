//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[UnitType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum UnitTypeEnum {
  Inches = 1,
  Millimeters = 2,
  AcreFeet = 3,
  AcreFeetPerAcre = 4
}

export const UnitTypes: LookupTableEntry[]  = [
  { Name: "Inches", DisplayName: "inches", Value: 1 },
  { Name: "Millimeters", DisplayName: "millimeters", Value: 2 },
  { Name: "AcreFeet", DisplayName: "acre-feet", Value: 3 },
  { Name: "AcreFeetPerAcre", DisplayName: "acre-feet/acre", Value: 4 }
];
export const UnitTypesAsSelectDropdownOptions = UnitTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
