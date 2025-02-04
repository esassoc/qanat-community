//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SupportTicketStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum SupportTicketStatusEnum {
  Unassigned = 1,
  Assigned = 2,
  Closed = 3
}

export const SupportTicketStatuses: LookupTableEntry[]  = [
  { Name: "Unassigned", DisplayName: "Unassigned", Value: 1 },
  { Name: "Assigned", DisplayName: "Assigned", Value: 2 },
  { Name: "Closed", DisplayName: "Closed", Value: 3 }
];
export const SupportTicketStatusesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...SupportTicketStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
