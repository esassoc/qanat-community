import { Component, inject, OnInit } from "@angular/core";

import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { AllocationPlanManageDto } from "src/app/shared/generated/model/allocation-plan-manage-dto";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AllocationPlanPeriodSimpleDto } from "src/app/shared/generated/model/allocation-plan-period-simple-dto";
import {
    AllocationPlanPeriodUpsertDto,
    AllocationPlanPeriodUpsertDtoForm,
    AllocationPlanPeriodUpsertDtoFormControls,
} from "src/app/shared/generated/model/allocation-plan-period-upsert-dto";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "upsert-allocation-period-modal",
    imports: [ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent, AsyncPipe],
    templateUrl: "./upsert-allocation-period-modal.component.html",
    styleUrls: ["./upsert-allocation-period-modal.component.scss"],
    animations: [inOutAnimation],
})
export class UpsertAllocationPeriodModalComponent implements OnInit {
    public ref: DialogRef<AllocationPeriodContext, any> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<AllocationPlanPeriodUpsertDtoForm> = new FormGroup<AllocationPlanPeriodUpsertDtoForm>({
        AllocationPlanID: AllocationPlanPeriodUpsertDtoFormControls.AllocationPlanID(),
        AllocationPlanPeriodID: AllocationPlanPeriodUpsertDtoFormControls.AllocationPlanPeriodID(0),
        AllocationPeriodName: AllocationPlanPeriodUpsertDtoFormControls.AllocationPeriodName(),
        AllocationAcreFeetPerAcre: AllocationPlanPeriodUpsertDtoFormControls.AllocationAcreFeetPerAcre(),
        NumberOfYears: AllocationPlanPeriodUpsertDtoFormControls.NumberOfYears(1, {
            validators: [Validators.required, Validators.min(1), Validators.max(100)],
        }),
        StartYear: AllocationPlanPeriodUpsertDtoFormControls.StartYear(),
        EnableCarryOver: AllocationPlanPeriodUpsertDtoFormControls.EnableCarryOver(false),
        CarryOverNumberOfYears: AllocationPlanPeriodUpsertDtoFormControls.CarryOverNumberOfYears(null, {
            validators: [Validators.required, Validators.min(1), Validators.max(100)],
        }),
        CarryOverDepreciationRate: AllocationPlanPeriodUpsertDtoFormControls.CarryOverDepreciationRate(1, {
            validators: [Validators.required, Validators.min(0), Validators.max(1)],
        }),
        EnableBorrowForward: AllocationPlanPeriodUpsertDtoFormControls.EnableBorrowForward(false),
        BorrowForwardNumberOfYears: AllocationPlanPeriodUpsertDtoFormControls.BorrowForwardNumberOfYears(null, {
            validators: [Validators.required, Validators.min(1), Validators.max(100)],
        }),
    });

    public yearOptions: SelectDropdownOption[] = [
        { Value: null, Label: "Select a Year", disabled: true },
        { Value: 1, Label: "1", disabled: false },
        { Value: 2, Label: "2", disabled: false },
        { Value: 3, Label: "3", disabled: false },
        { Value: 4, Label: "4", disabled: false },
        { Value: 5, Label: "5", disabled: false },
    ];

    public carryOverValue$: Observable<any>;
    public borrowForwardValue$: Observable<any>;

    constructor(
        private allocationPlanService: AllocationPlanService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.carryOverValue$ = this.formGroup.controls.EnableCarryOver.valueChanges.pipe(
            tap((enabled) => {
                if (enabled) {
                    this.formGroup.controls.CarryOverNumberOfYears.enable();
                    this.formGroup.controls.CarryOverDepreciationRate.enable();
                } else {
                    this.formGroup.controls.CarryOverNumberOfYears.disable();
                    this.formGroup.controls.CarryOverDepreciationRate.disable();
                }
            })
        );

        this.borrowForwardValue$ = this.formGroup.controls.EnableBorrowForward.valueChanges.pipe(
            tap((enabled) => {
                if (enabled) {
                    this.formGroup.controls.BorrowForwardNumberOfYears.enable();
                } else {
                    this.formGroup.controls.BorrowForwardNumberOfYears.disable();
                }
            })
        );

        const years: number[] = [];
        for (
            let i = 0;
            i <=
            this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.EndYear -
                this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.StartYear;
            i++
        ) {
            years.push(this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.StartYear + i);
        }

        this.yearOptions = [
            { Value: null, Label: "Select a Year", disabled: true },
            ...years.map((year) => {
                return { Value: year, Label: year.toString(), disabled: false } as SelectDropdownOption;
            }),
        ];
    }

    ngAfterViewInit(): void {
        if (this.ref.data.Update === true) {
            this.formGroup.patchValue(this.ref.data.AllocationPlanPeriodSimpleDto, { emitEvent: true });
        } else {
            this.formGroup.controls.AllocationPlanID.patchValue(this.ref.data.AllocationPlanManageDto.AllocationPlanID);
        }
        this.formGroup.controls.EnableCarryOver.updateValueAndValidity({ emitEvent: true });
        this.formGroup.controls.EnableBorrowForward.updateValueAndValidity({ emitEvent: true });
    }

    save(): void {
        const submitDto = new AllocationPlanPeriodUpsertDto(this.formGroup.value);

        const endPoint =
            this.ref.data.Update === true
                ? this.allocationPlanService.updateAllocationPlanPeriodAllocationPlan(
                      this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                      this.ref.data.AllocationPlanManageDto.AllocationPlanID,
                      this.ref.data.AllocationPlanPeriodSimpleDto.AllocationPlanPeriodID,
                      submitDto
                  )
                : this.allocationPlanService.createAllocationPlanPeriodAllocationPlan(
                      this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                      this.ref.data.AllocationPlanManageDto.AllocationPlanID,
                      submitDto
                  );

        endPoint.subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert(`Successfully ${this.ref.data.Update === true ? "updated" : "created"} Allocation Plan Period.`, AlertContext.Success));
            this.ref.close(submitDto);
        });
    }

    close(): void {
        this.ref.close(null);
    }
}

export interface AllocationPeriodContext {
    AllocationPlanManageDto: AllocationPlanManageDto | null;
    AllocationPlanPeriodSimpleDto: AllocationPlanPeriodSimpleDto | null;
    Update: boolean | null;
}
