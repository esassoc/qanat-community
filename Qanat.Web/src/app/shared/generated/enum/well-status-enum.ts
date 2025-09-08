//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WellStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum WellStatusEnum {
  Operational = 1,
  NonOperational = 2,
  Duplicate = 3
}

export const WellStatuses: LookupTableEntry[]  = [
  { Name: "Operational", DisplayName: "Operational", Value: 1 },
  { Name: "NonOperational", DisplayName: "Non-Operational", Value: 2 },
  { Name: "Duplicate", DisplayName: "Duplicate", Value: 3 }
];
export const WellStatusesAsSelectDropdownOptions = WellStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
