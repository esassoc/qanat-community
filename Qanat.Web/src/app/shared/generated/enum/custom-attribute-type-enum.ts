//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[CustomAttributeType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum CustomAttributeTypeEnum {
  WaterAccount = 1,
  Parcel = 2
}

export const CustomAttributeTypes: LookupTableEntry[]  = [
  { Name: "WaterAccount", DisplayName: "Water Account", Value: 1 },
  { Name: "Parcel", DisplayName: "Parcel", Value: 2 }
];
export const CustomAttributeTypesAsSelectDropdownOptions = CustomAttributeTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
