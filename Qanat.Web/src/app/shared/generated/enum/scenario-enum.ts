//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Scenario]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum ScenarioEnum {
  AddaWell = 2,
  Recharge = 3
}

export const Scenarios: LookupTableEntry[]  = [
  { Name: "Add a Well", DisplayName: "Add a Well", Value: 2 },
  { Name: "Recharge", DisplayName: "Recharge", Value: 3 }
];
export const ScenariosAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...Scenarios.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
