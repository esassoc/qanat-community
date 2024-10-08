import { Component, Input, OnInit, ViewContainerRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AllocationPlanPeriodSimpleDto } from "src/app/shared/generated/model/allocation-plan-period-simple-dto";
import { AllocationPlanTableRowComponent } from "../allocation-plan-table-row/allocation-plan-table-row.component";
import { PeriodBodyComponent } from "./period-body/period-body.component";
import { ModalOptions, ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { AllocationPlanManageDto } from "src/app/shared/generated/model/allocation-plan-manage-dto";
import { DeleteAllocationPeriodModalComponent } from "../delete-allocation-period-modal/delete-allocation-period-modal.component";
import { AllocationPeriodContext, UpsertAllocationPeriodModalComponent } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";

@Component({
    selector: "allocation-plan-period",
    standalone: true,
    imports: [CommonModule, AllocationPlanTableRowComponent, PeriodBodyComponent],
    templateUrl: "./allocation-plan-period.component.html",
    styleUrls: ["./allocation-plan-period.component.scss"],
})
export class AllocationPlanPeriodComponent implements OnInit {
    @Input() years: number[];
    @Input() allocationPlanPeriod: AllocationPlanPeriodSimpleDto;
    @Input() allocationPlanManageDto: AllocationPlanManageDto;
    @Input() readonly: boolean = true;

    public startYear: number;
    public totalSpan: number;

    constructor(
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef
    ) {}

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
        this.modalService
            .open(
                DeleteAllocationPeriodModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    AllocationPlanPeriodSimpleDto: this.allocationPlanPeriod,
                    AllocationPlanManageDto: this.allocationPlanManageDto,
                } as AllocationPeriodContext
            )
            .instance.result.then((result) => {
                if (result) {
                }
            });
    }

    openEditModal(): void {
        this.modalService
            .open(
                UpsertAllocationPeriodModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.ExtraLarge,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    AllocationPlanPeriodSimpleDto: this.allocationPlanPeriod,
                    AllocationPlanManageDto: this.allocationPlanManageDto,
                    Update: true,
                } as AllocationPeriodContext
            )
            .instance.result.then((result) => {
                if (result) {
                    // this.setupObservable();
                }
            });
    }
}
