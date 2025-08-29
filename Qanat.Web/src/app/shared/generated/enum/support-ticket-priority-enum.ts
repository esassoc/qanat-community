//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SupportTicketPriority]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum SupportTicketPriorityEnum {
  High = 1,
  Medium = 2,
  Low = 3,
  NotPrioritized = 4
}

export const SupportTicketPriorities: LookupTableEntry[]  = [
  { Name: "High", DisplayName: "High", Value: 1 },
  { Name: "Medium", DisplayName: "Medium", Value: 2 },
  { Name: "Low", DisplayName: "Low", Value: 3 },
  { Name: "NotPrioritized", DisplayName: "Not Prioritized", Value: 4 }
];
export const SupportTicketPrioritiesAsSelectDropdownOptions = SupportTicketPriorities.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
