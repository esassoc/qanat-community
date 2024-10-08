import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AllocationPlanPeriodSimpleDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "period-body",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./period-body.component.html",
    styleUrls: ["./period-body.component.scss"],
})
export class PeriodBodyComponent implements OnInit {
    @Input() allocationPlanPeriod: AllocationPlanPeriodSimpleDto;
    @Input() zoneColor: string;

    public borrowForwardStart: number;
    public borrowForwardEnd: number;

    public primaryAllocationStart: number;
    public primaryAllocationEnd: number;

    public carryOverStart: number;
    public carryOverEnd: number;

    constructor() {}

    ngOnInit(): void {
        // the order of these is important
        this.borrowForwardStart = 1;
        this.borrowForwardEnd = 1 + this.allocationPlanPeriod.BorrowForwardNumberOfYears;

        this.primaryAllocationStart = this.allocationPlanPeriod.EnableBorrowForward ? this.borrowForwardEnd : 1;
        this.primaryAllocationEnd = this.primaryAllocationStart + this.allocationPlanPeriod.NumberOfYears;

        this.carryOverStart = this.primaryAllocationEnd;
        this.carryOverEnd = this.allocationPlanPeriod.EnableCarryOver ? this.allocationPlanPeriod.CarryOverNumberOfYears + this.primaryAllocationEnd : 1;
    }
}
