//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[MeterStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum MeterStatusEnum {
  Active = 1,
  Broken = 2,
  Retired = 3
}

export const MeterStatuses: LookupTableEntry[]  = [
  { Name: "Active", DisplayName: "Active", Value: 1 },
  { Name: "Broken", DisplayName: "Broken", Value: 2 },
  { Name: "Retired", DisplayName: "Retired", Value: 3 }
];
export const MeterStatusesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...MeterStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
