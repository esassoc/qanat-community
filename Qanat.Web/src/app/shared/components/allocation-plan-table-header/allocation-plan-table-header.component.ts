import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AllocationPlanTableRowComponent } from "../allocation-plan-table-row/allocation-plan-table-row.component";

@Component({
    selector: "allocation-plan-table-header",
    standalone: true,
    imports: [CommonModule, AllocationPlanTableRowComponent],
    templateUrl: "./allocation-plan-table-header.component.html",
    styleUrls: ["./allocation-plan-table-header.component.scss"],
})
export class AllocationPlanTableHeaderComponent implements OnInit {
    @Input() years: number[] = [];
    constructor() {}

    ngOnInit(): void {}
}
