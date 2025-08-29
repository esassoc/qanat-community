//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WellRegistrationStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum WellRegistrationStatusEnum {
  Draft = 1,
  Submitted = 2,
  Returned = 3,
  Approved = 4
}

export const WellRegistrationStatuses: LookupTableEntry[]  = [
  { Name: "Draft", DisplayName: "Draft", Value: 1 },
  { Name: "Submitted", DisplayName: "Submitted", Value: 2 },
  { Name: "Returned", DisplayName: "Returned", Value: 3 },
  { Name: "Approved", DisplayName: "Approved", Value: 4 }
];
export const WellRegistrationStatusesAsSelectDropdownOptions = WellRegistrationStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
