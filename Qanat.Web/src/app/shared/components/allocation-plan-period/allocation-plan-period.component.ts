import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

import { AllocationPlanPeriodSimpleDto } from "src/app/shared/generated/model/allocation-plan-period-simple-dto";
import { AllocationPlanTableRowComponent } from "../allocation-plan-table-row/allocation-plan-table-row.component";
import { PeriodBodyComponent } from "./period-body/period-body.component";
import { AllocationPlanManageDto } from "src/app/shared/generated/model/allocation-plan-manage-dto";
import { DeleteAllocationPeriodModalComponent } from "../delete-allocation-period-modal/delete-allocation-period-modal.component";
import { UpsertAllocationPeriodModalComponent } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { DialogService } from "@ngneat/dialog";
import { AllocationPlanPeriodUpsertDto } from "../../generated/model/allocation-plan-period-upsert-dto";

@Component({
    selector: "allocation-plan-period",
    imports: [AllocationPlanTableRowComponent, PeriodBodyComponent],
    templateUrl: "./allocation-plan-period.component.html",
    styleUrls: ["./allocation-plan-period.component.scss"],
})
export class AllocationPlanPeriodComponent implements OnInit {
    @Input() years: number[];
    @Input() allocationPlanPeriod: AllocationPlanPeriodSimpleDto;
    @Input() allocationPlanManageDto: AllocationPlanManageDto;
    @Input() readonly: boolean = true;

    @Output() change = new EventEmitter<AllocationPlanPeriodUpsertDto>();
    @Output() delete = new EventEmitter<number>();

    public startYear: number;
    public totalSpan: number;

    constructor(private dialogService: DialogService) {}

    ngOnInit(): void {
        // the total span on the allocation plan period is the number of year + carry over years + borrow forward years
        let span = this.allocationPlanPeriod.NumberOfYears;
        if (this.allocationPlanPeriod.EnableCarryOver) {
            span += this.allocationPlanPeriod.CarryOverNumberOfYears;
        }
        if (this.allocationPlanPeriod.EnableBorrowForward) {
            span += this.allocationPlanPeriod.BorrowForwardNumberOfYears;
        }
        this.totalSpan = span;

        // the start year is which year the allocation plan period starts on (takes into account the borrow forward if it's enabled)
        this.startYear = this.allocationPlanPeriod.StartYear - (this.allocationPlanPeriod.EnableBorrowForward ? this.allocationPlanPeriod.BorrowForwardNumberOfYears : 0);
    }

    openDeleteModal(): void {
        const dialogRef = this.dialogService.open(DeleteAllocationPeriodModalComponent, {
            data: {
                AllocationPlanPeriodSimpleDto: this.allocationPlanPeriod,
                AllocationPlanManageDto: this.allocationPlanManageDto,
                Update: true,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.delete.emit(result);
            }
        });
    }

    openEditModal(): void {
        const dialogRef = this.dialogService.open(UpsertAllocationPeriodModalComponent, {
            data: {
                AllocationPlanPeriodSimpleDto: this.allocationPlanPeriod,
                AllocationPlanManageDto: this.allocationPlanManageDto,
                Update: true,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.change.emit(result);
            }
        });
    }
}
