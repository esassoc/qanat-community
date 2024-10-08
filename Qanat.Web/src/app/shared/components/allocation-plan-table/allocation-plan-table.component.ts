import { Component, Input, OnInit, ViewEncapsulation } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AllocationPlanManageDto } from "src/app/shared/generated/model/models";
import { AllocationPlanTableHeaderComponent } from "../allocation-plan-table-header/allocation-plan-table-header.component";
import { AllocationPlanPeriodComponent } from "../allocation-plan-period/allocation-plan-period.component";
import { AllocationPlanTableRowComponent } from "../allocation-plan-table-row/allocation-plan-table-row.component";

@Component({
    selector: "allocation-plan-table",
    standalone: true,
    imports: [CommonModule, AllocationPlanTableHeaderComponent, AllocationPlanPeriodComponent, AllocationPlanTableRowComponent],
    templateUrl: "./allocation-plan-table.component.html",
    styleUrls: ["./allocation-plan-table.component.scss"],
    encapsulation: ViewEncapsulation.None,
})
export class AllocationPlanTableComponent implements OnInit {
    @Input() allocationPlan: AllocationPlanManageDto;
    @Input() readonly: boolean = true;

    public years: number[] = [];
    constructor() {}

    ngOnInit(): void {
        // the start year will either be the confiured start year or the start year of a period configured with borrow-forward
        let startYears = [this.allocationPlan.GeographyAllocationPlanConfiguration.StartYear];
        const periodStartYears = this.allocationPlan.AllocationPlanPeriods.map((period) => {
            const startYear = period.EnableBorrowForward ? period.StartYear - period.BorrowForwardNumberOfYears : period.StartYear;
            return startYear;
        });
        startYears = [...startYears, ...periodStartYears];
        const startYear = startYears.sort()[0];

        // the end year for the table will be the geography configured end year or the biggest "end year" for an individual period (due to length of carry over)
        let endYears = [this.allocationPlan.GeographyAllocationPlanConfiguration.EndYear];
        const periodEndYears = this.allocationPlan.AllocationPlanPeriods.map((period) => {
            const lastYear = period.EnableCarryOver ? period.StartYear + (period.NumberOfYears - 1) + period.CarryOverNumberOfYears : period.StartYear + (period.NumberOfYears - 1);
            return lastYear;
        });
        endYears = [...endYears, ...periodEndYears];
        const endYear = endYears.sort()[endYears.length - 1];

        // loop through the start to end years to build out table
        for (let i = startYear; i <= endYear; i++) {
            this.years = [...this.years, i];
        }
    }
}
