//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Role]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum RoleEnum {
  SystemAdmin = 1,
  NoAccess = 2,
  Normal = 3,
  PendingLogin = 4
}

export const Roles: LookupTableEntry[]  = [
  { Name: "SystemAdmin", DisplayName: "System Administrator", Value: 1 },
  { Name: "NoAccess", DisplayName: "No Access", Value: 2 },
  { Name: "Normal", DisplayName: "Normal", Value: 3 },
  { Name: "PendingLogin", DisplayName: "Pending Login", Value: 4 }
];
export const RolesAsSelectDropdownOptions = Roles.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
