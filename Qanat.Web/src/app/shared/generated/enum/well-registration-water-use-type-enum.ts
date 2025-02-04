//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[WellRegistrationWaterUseType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum WellRegistrationWaterUseTypeEnum {
  Agricultural = 1,
  StockWatering = 2,
  Domestic = 3,
  PublicMunicipal = 4,
  PrivateMunicipal = 5,
  Other = 6
}

export const WellRegistrationWaterUseTypes: LookupTableEntry[]  = [
  { Name: "Agricultural", DisplayName: "Agricultural", Value: 1 },
  { Name: "StockWatering", DisplayName: "Stock Watering", Value: 2 },
  { Name: "Domestic", DisplayName: "Domestic", Value: 3 },
  { Name: "PublicMunicipal", DisplayName: "Public Municipal", Value: 4 },
  { Name: "PrivateMunicipal", DisplayName: "Private Municipal", Value: 5 },
  { Name: "Other", DisplayName: "Other", Value: 6 }
];
export const WellRegistrationWaterUseTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...WellRegistrationWaterUseTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
