//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SupportTicketStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum SupportTicketStatusEnum {
  Unassigned = 1,
  Active = 2,
  Closed = 3,
  OnHold = 4
}

export const SupportTicketStatuses: LookupTableEntry[]  = [
  { Name: "Unassigned", DisplayName: "Unassigned", Value: 1 },
  { Name: "Active", DisplayName: "Active", Value: 2 },
  { Name: "Closed", DisplayName: "Closed", Value: 3 },
  { Name: "OnHold", DisplayName: "On Hold", Value: 4 }
];
export const SupportTicketStatusesAsSelectDropdownOptions = SupportTicketStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
