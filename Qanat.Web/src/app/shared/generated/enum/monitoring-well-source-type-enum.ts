//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[MonitoringWellSourceType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum MonitoringWellSourceTypeEnum {
  CNRA = 1,
  YoloWRID = 2
}

export const MonitoringWellSourceTypes: LookupTableEntry[]  = [
  { Name: "CNRA", DisplayName: "California Natural Resources Agency", Value: 1 },
  { Name: "YoloWRID", DisplayName: "Yolo WRID", Value: 2 }
];
export const MonitoringWellSourceTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...MonitoringWellSourceTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
