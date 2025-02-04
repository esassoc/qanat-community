//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WellStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WellStatusEnum {
  Operational = 1,
  NonOperational = 2
}

export const WellStatuses: LookupTableEntry[]  = [
  { Name: "Operational", DisplayName: "Operational", Value: 1 },
  { Name: "NonOperational", DisplayName: "Non-Operational", Value: 2 }
];
export const WellStatusesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WellStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
