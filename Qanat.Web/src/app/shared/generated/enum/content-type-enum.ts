//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ContentType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component"

export enum ContentTypeEnum {
  PageInstructions = 1,
  FormInstructions = 2,
  FieldDefinition = 3
}

export const ContentTypes: LookupTableEntry[]  = [
  { Name: "PageInstructions", DisplayName: "Page Instructions", Value: 1 },
  { Name: "FormInstructions", DisplayName: "Form Instructions", Value: 2 },
  { Name: "FieldDefinition", DisplayName: "Field Definition", Value: 3 }
];
export const ContentTypesAsSelectDropdownOptions = [{ Value: null, Label: "- Select -", Disabled: true }, ...ContentTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName } as SelectDropdownOption))];
