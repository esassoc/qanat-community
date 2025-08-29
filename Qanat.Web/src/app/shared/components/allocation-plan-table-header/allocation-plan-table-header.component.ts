import { Component, Input, OnInit } from "@angular/core";

import { AllocationPlanTableRowComponent } from "../allocation-plan-table-row/allocation-plan-table-row.component";

@Component({
    selector: "allocation-plan-table-header",
    imports: [AllocationPlanTableRowComponent],
    templateUrl: "./allocation-plan-table-header.component.html",
    styleUrls: ["./allocation-plan-table-header.component.scss"]
})
export class AllocationPlanTableHeaderComponent implements OnInit {
    @Input() years: number[] = [];
    constructor() {}

    ngOnInit(): void {}
}
