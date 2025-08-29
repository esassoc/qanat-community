//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Model]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ModelEnum {
  MercedWaterResourcesModel = 1,
  KernC2VSimFGKern = 2,
  YSGAWaterResourcesModel = 3
}

export const Models: LookupTableEntry[]  = [
  { Name: "Merced Water Resources Model", DisplayName: "Merced Water Resources Model", Value: 1 },
  { Name: "Kern - C2VSimFG-Kern", DisplayName: "Kern - C2VSimFG-Kern", Value: 2 },
  { Name: "YSGA Water Resources Model", DisplayName: "YSGA Water Resources Model", Value: 3 }
];
export const ModelsAsSelectDropdownOptions = Models.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
