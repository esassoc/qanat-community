//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[StatementTemplateType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum StatementTemplateTypeEnum {
  UsageStatement = 1
}

export const StatementTemplateTypes: LookupTableEntry[]  = [
  { Name: "UsageStatement", DisplayName: "Usage Statement", Value: 1 }
];
export const StatementTemplateTypesAsSelectDropdownOptions = StatementTemplateTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption));
