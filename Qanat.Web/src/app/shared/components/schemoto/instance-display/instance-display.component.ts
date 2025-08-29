import { Component, Input, OnInit } from "@angular/core";
import { Schema } from "src/app/shared/generated/model/schema";
import { RecordSetGridDisplayComponent } from "./record-set-grid-display/record-set-grid-display.component";
import { RecordSetSchema } from "src/app/shared/generated/model/record-set-schema";
import { RecordSet } from "src/app/shared/generated/model/record-set";
import { Instance } from "src/app/shared/generated/model/instance";

@Component({
    selector: "instance-display",
    imports: [RecordSetGridDisplayComponent],
    templateUrl: "./instance-display.component.html",
    styleUrl: "./instance-display.component.scss",
})
export class InstanceDisplayComponent implements OnInit {
    @Input() schema: Schema;
    @Input() instance: Instance;
    @Input() gridCol: string = "g-col-6";

    public recordSetSchemataWithRecordSets: { RecordSetSchema: RecordSetSchema; RecordSet: RecordSet }[] = [];

    ngOnInit() {
        this.initializeRecordSetSchemaWithRecordSets();
    }

    private initializeRecordSetSchemaWithRecordSets() {
        if (this.schema && this.schema.RecordSetSchemata) {
            this.recordSetSchemataWithRecordSets = this.schema.RecordSetSchemata.map((recordSetSchema) => {
                const recordSet = this.instance.RecordSets?.find((rs) => rs.CanonicalName === recordSetSchema.CanonicalName);
                return { RecordSetSchema: recordSetSchema, RecordSet: recordSet };
            });
        }
    }
}
