//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WaterAccountRole]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WaterAccountRoleEnum {
  WaterAccountHolder = 1,
  WaterAccountViewer = 2
}

export const WaterAccountRoles: LookupTableEntry[]  = [
  { Name: "WaterAccountHolder", DisplayName: "Account Holder", Value: 1 },
  { Name: "WaterAccountViewer", DisplayName: "Viewer", Value: 2 }
];
export const WaterAccountRolesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WaterAccountRoles.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
