//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ParcelStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ParcelStatusEnum {
  Assigned = 1,
  Inactive = 2,
  Unassigned = 3,
  Excluded = 4
}

export const ParcelStatuses: LookupTableEntry[]  = [
  { Name: "Assigned", DisplayName: "Active", Value: 1 },
  { Name: "Inactive", DisplayName: "Inactive", Value: 2 },
  { Name: "Unassigned", DisplayName: "Unassigned", Value: 3 },
  { Name: "Excluded", DisplayName: "Excluded", Value: 4 }
];
export const ParcelStatusesAsSelectDropdownOptions = ParcelStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
