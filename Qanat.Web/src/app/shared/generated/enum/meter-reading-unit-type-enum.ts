//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[MeterReadingUnitType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum MeterReadingUnitTypeEnum {
  AcreFeet = 1,
  Gallons = 2
}

export const MeterReadingUnitTypes: LookupTableEntry[]  = [
  { Name: "AcreFeet", DisplayName: "acre-feet", Value: 1 },
  { Name: "Gallons", DisplayName: "gallons", Value: 2 }
];
export const MeterReadingUnitTypesAsSelectDropdownOptions = MeterReadingUnitTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
