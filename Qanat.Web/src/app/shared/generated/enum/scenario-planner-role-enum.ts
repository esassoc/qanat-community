//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ScenarioPlannerRole]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum ScenarioPlannerRoleEnum {
  NoAccess = 1,
  ScenarioUser = 2
}

export const ScenarioPlannerRoles: LookupTableEntry[]  = [
  { Name: "NoAccess", DisplayName: "No Access", Value: 1 },
  { Name: "ScenarioUser", DisplayName: "Scenario User", Value: 2 }
];
export const ScenarioPlannerRolesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...ScenarioPlannerRoles.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
