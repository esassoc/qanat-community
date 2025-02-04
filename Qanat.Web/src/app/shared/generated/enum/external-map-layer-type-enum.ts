//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ExternalMapLayerType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum ExternalMapLayerTypeEnum {
  ESRIFeatureServer = 1,
  ESRIMapServer = 2
}

export const ExternalMapLayerTypes: LookupTableEntry[]  = [
  { Name: "ESRIFeatureServer", DisplayName: "ESRI Feature Server (WFS/vector)", Value: 1 },
  { Name: "ESRIMapServer", DisplayName: "ESRI Map Server (WMS/raster)", Value: 2 }
];
export const ExternalMapLayerTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...ExternalMapLayerTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
