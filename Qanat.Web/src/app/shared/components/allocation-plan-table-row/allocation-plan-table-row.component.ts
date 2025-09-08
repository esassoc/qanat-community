import { Component, ContentChild, Input, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
    selector: "allocation-plan-table-row",
    imports: [CommonModule],
    templateUrl: "./allocation-plan-table-row.component.html",
    styleUrls: ["./allocation-plan-table-row.component.scss"]
})
export class AllocationPlanTableRowComponent implements OnInit {
    @Input() years: number[] = [];
    @Input() placeSlotAtYear: number;
    @Input() spanYears: number;

    @ContentChild("items") items!: TemplateRef<any>;
    @ContentChild("period") period!: TemplateRef<any>;
    @ContentChild("slot") slot!: TemplateRef<any>;

    public gridColumnStart: number;
    public gridColumnEnd: number;

    constructor() {}

    ngOnInit(): void {}

    ngAfterContentInit(): void {
        if (this.placeSlotAtYear) {
            this.gridColumnStart = this.years.indexOf(this.placeSlotAtYear) + 2;
            if (this.spanYears) {
                this.gridColumnEnd = this.gridColumnStart + this.spanYears;
            }
        }
    }
}
