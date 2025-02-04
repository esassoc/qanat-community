//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GETActionStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum GETActionStatusEnum {
  Created = 1,
  GETIntegrationFailure = 2,
  CreatedInGET = 3,
  Queued = 4,
  Processing = 5,
  Complete = 6,
  SystemError = 7,
  InvalidOutput = 8,
  InvalidInput = 9,
  HasDryCells = 10,
  AnalysisFailed = 11,
  AnalysisSuccess = 12,
  ProcesingInputs = 13,
  RunningAnalysis = 14
}

export const GETActionStatuses: LookupTableEntry[]  = [
  { Name: "Created", DisplayName: "Created", Value: 1 },
  { Name: "GETIntegrationFailure", DisplayName: "GET Integration Failure", Value: 2 },
  { Name: "CreatedInGET", DisplayName: "Created in GET", Value: 3 },
  { Name: "Queued", DisplayName: "Queued", Value: 4 },
  { Name: "Processing", DisplayName: "Processing", Value: 5 },
  { Name: "Complete", DisplayName: "Complete", Value: 6 },
  { Name: "SystemError", DisplayName: "System Error", Value: 7 },
  { Name: "InvalidOutput", DisplayName: "Invalid Output", Value: 8 },
  { Name: "InvalidInput", DisplayName: "Invalid Input", Value: 9 },
  { Name: "HasDryCells", DisplayName: "Completed with Dry Cells", Value: 10 },
  { Name: "AnalysisFailed", DisplayName: "Analysis Failed", Value: 11 },
  { Name: "AnalysisSuccess", DisplayName: "Analysis Succeeded", Value: 12 },
  { Name: "ProcesingInputs", DisplayName: "Processing Inputs", Value: 13 },
  { Name: "RunningAnalysis", DisplayName: "Running Analysis", Value: 14 }
];
export const GETActionStatusesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...GETActionStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
