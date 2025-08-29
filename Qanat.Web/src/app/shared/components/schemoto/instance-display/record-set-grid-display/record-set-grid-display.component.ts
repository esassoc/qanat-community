import { Component, Input, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { RecordSetSchema } from "src/app/shared/generated/model/record-set-schema";
import { QanatGridComponent } from "../../../qanat-grid/qanat-grid.component";
import { DecimalType, RecordSet } from "src/app/shared/generated/model/models";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "record-set-grid-display",
    imports: [QanatGridComponent],
    templateUrl: "./record-set-grid-display.component.html",
    styleUrl: "./record-set-grid-display.component.scss",
})
export class RecordSetGridDisplayComponent implements OnInit {
    @Input() recordSetSchemaWithRecordSet: { RecordSetSchema: RecordSetSchema; RecordSet: RecordSet };
    @Input() instance: any;

    public gridCols: ColDef[];

    constructor(private utilityFunctionsService: UtilityFunctionsService) {}

    ngOnInit(): void {
        this.gridCols = this.createGridColumns();
    }

    private createGridColumns(): ColDef[] {
        let colDefs = [];

        this.recordSetSchemaWithRecordSet.RecordSetSchema.FieldSchemata.forEach((fieldSchema) => {
            switch (fieldSchema.DataType.Type) {
                case "String":
                    let stringColDef = this.utilityFunctionsService.createBasicColumnDef(fieldSchema.Name, `Fields.${fieldSchema.CanonicalName}`);
                    colDefs.push(stringColDef);
                    break;
                case "Integer":
                    let intColDef = this.utilityFunctionsService.createDecimalColumnDef(fieldSchema.Name, `Fields.${fieldSchema.CanonicalName}`, { MaxDecimalPlacesToDisplay: 0 });
                    colDefs.push(intColDef);
                    break;
                case "Decimal":
                    let decimalType = fieldSchema.DataType as DecimalType;
                    let columnHeader = decimalType.UnitOfMeasure ? `${fieldSchema.Name} (${decimalType.UnitOfMeasure.Abbreviation})` : fieldSchema.Name;

                    let decimalColDef = this.utilityFunctionsService.createDecimalColumnDef(columnHeader, `Fields.${fieldSchema.CanonicalName}`, {
                        MaxDecimalPlacesToDisplay: fieldSchema.DataType["Scale"],
                    });
                    colDefs.push(decimalColDef);
                    break;
                case "DateTime":
                    let dateColDef = this.utilityFunctionsService.createDateColumnDef(fieldSchema.Name, `Fields.${fieldSchema.CanonicalName}`, "M/d/yyyy", {
                        IgnoreLocalTimezone: true,
                    });

                    colDefs.push(dateColDef);
                    break;
                default:
                    break;
            }
        });

        return colDefs;
    }
}
