//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GeographyRole]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum GeographyRoleEnum {
  WaterManager = 1,
  Normal = 2
}

export const GeographyRoles: LookupTableEntry[]  = [
  { Name: "WaterManager", DisplayName: "Water Manager", Value: 1 },
  { Name: "Normal", DisplayName: "Normal", Value: 2 }
];
export const GeographyRolesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...GeographyRoles.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
