//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SupportTicketQuestionType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum SupportTicketQuestionTypeEnum {
  AccessingData = 1,
  PolicyQuestion = 2,
  InterpretingDataQuestion = 3,
  LogInQuestion = 4,
  Bug = 5,
  Other = 6
}

export const SupportTicketQuestionTypes: LookupTableEntry[]  = [
  { Name: "AccessingData", DisplayName: "I need help accessing my Water Account / Parcel / Usage Data", Value: 1 },
  { Name: "PolicyQuestion", DisplayName: "I have a question about policies, rules, fees, etc", Value: 2 },
  { Name: "InterpretingDataQuestion", DisplayName: "I need help interpreting my water usage or allocations", Value: 3 },
  { Name: "LogInQuestion", DisplayName: "I can't log in or my account isn't configured", Value: 4 },
  { Name: "Bug", DisplayName: "I ran into a bug or problem with the system", Value: 5 },
  { Name: "Other", DisplayName: "Other", Value: 6 }
];
export const SupportTicketQuestionTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...SupportTicketQuestionTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
