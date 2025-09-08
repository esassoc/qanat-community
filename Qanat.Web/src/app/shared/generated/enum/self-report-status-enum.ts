//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SelfReportStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum SelfReportStatusEnum {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Returned = 4
}

export const SelfReportStatuses: LookupTableEntry[]  = [
  { Name: "Draft", DisplayName: "Draft", Value: 1 },
  { Name: "Submitted", DisplayName: "Submitted", Value: 2 },
  { Name: "Approved", DisplayName: "Approved", Value: 3 },
  { Name: "Returned", DisplayName: "Returned", Value: 4 }
];
export const SelfReportStatusesAsSelectDropdownOptions = SelfReportStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
