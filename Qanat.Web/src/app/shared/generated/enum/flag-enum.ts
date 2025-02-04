//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Flag]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum FlagEnum {
  CanImpersonateUsers = 1,
  HasManagerDashboard = 2,
  IsSystemAdmin = 3,
  CanClaimWaterAccounts = 4,
  CanRegisterWells = 5,
  CanReviewWells = 6
}

export const Flags: LookupTableEntry[]  = [
  { Name: "CanImpersonateUsers", DisplayName: "CanImpersonateUsers", Value: 1 },
  { Name: "HasManagerDashboard", DisplayName: "HasManagerDashboard", Value: 2 },
  { Name: "IsSystemAdmin", DisplayName: "IsSystemAdmin", Value: 3 },
  { Name: "CanClaimWaterAccounts", DisplayName: "CanClaimWaterAccounts", Value: 4 },
  { Name: "CanRegisterWells", DisplayName: "CanRegisterWells", Value: 5 },
  { Name: "CanReviewWells", DisplayName: "CanReviewWells", Value: 6 }
];
export const FlagsAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...Flags.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
