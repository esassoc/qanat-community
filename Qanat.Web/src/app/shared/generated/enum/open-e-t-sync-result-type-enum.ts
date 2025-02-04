//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[OpenETSyncResultType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum OpenETSyncResultTypeEnum {
  InProgress = 1,
  Succeeded = 2,
  Failed = 3,
  NoNewData = 4,
  DataNotAvailable = 5,
  Created = 6
}

export const OpenETSyncResultTypes: LookupTableEntry[]  = [
  { Name: "InProgress", DisplayName: "In Progress", Value: 1 },
  { Name: "Succeeded", DisplayName: "Succeeded", Value: 2 },
  { Name: "Failed", DisplayName: "Failed", Value: 3 },
  { Name: "NoNewData", DisplayName: "No New Data", Value: 4 },
  { Name: "DataNotAvailable", DisplayName: "Data Not Available", Value: 5 },
  { Name: "Created", DisplayName: "Created", Value: 6 }
];
export const OpenETSyncResultTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...OpenETSyncResultTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
