//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FaqDisplayLocationType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FaqDisplayLocationTypeEnum {
  GrowersGuide = 1,
  WaterManagerGuide = 2,
  RequestSupport = 3
}

export const FaqDisplayLocationTypes: LookupTableEntry[]  = [
  { Name: "GrowersGuide", DisplayName: "Growers Guide", Value: 1 },
  { Name: "WaterManagerGuide", DisplayName: "Water Manager Guide", Value: 2 },
  { Name: "RequestSupport", DisplayName: "Request Support", Value: 3 }
];
export const FaqDisplayLocationTypesAsSelectDropdownOptions = FaqDisplayLocationTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
