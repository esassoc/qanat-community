import { Component, ComponentRef, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IModal, ModalEvent, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
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

@Component({
    selector: "upsert-allocation-period-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent],
    templateUrl: "./upsert-allocation-period-modal.component.html",
    styleUrls: ["./upsert-allocation-period-modal.component.scss"],
    animations: [inOutAnimation],
})
export class UpsertAllocationPeriodModalComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: AllocationPeriodContext;
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
        { Value: null, Label: "Select a Year", Disabled: true },
        { Value: 1, Label: "1", Disabled: false },
        { Value: 2, Label: "2", Disabled: false },
        { Value: 3, Label: "3", Disabled: false },
        { Value: 4, Label: "4", Disabled: false },
        { Value: 5, Label: "5", Disabled: false },
    ];

    public carryOverValue$: Observable<any>;
    public borrowForwardValue$: Observable<any>;

    constructor(
        private modalService: ModalService,
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
            this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.EndYear -
                this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.StartYear;
            i++
        ) {
            years.push(this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.StartYear + i);
        }

        this.yearOptions = [
            { Value: null, Label: "Select a Year", Disabled: true },
            ...years.map((year) => {
                return { Value: year, Label: year.toString(), Disabled: false } as SelectDropdownOption;
            }),
        ];
    }

    ngAfterViewInit(): void {
        if (this.modalContext.Update === true) {
            this.formGroup.patchValue(this.modalContext.AllocationPlanPeriodSimpleDto, { emitEvent: true });
        } else {
            this.formGroup.controls.AllocationPlanID.patchValue(this.modalContext.AllocationPlanManageDto.AllocationPlanID);
        }
        this.formGroup.controls.EnableCarryOver.updateValueAndValidity({ emitEvent: true });
        this.formGroup.controls.EnableBorrowForward.updateValueAndValidity({ emitEvent: true });
    }

    save(): void {
        const submitDto = new AllocationPlanPeriodUpsertDto(this.formGroup.value);

        const endPoint =
            this.modalContext.Update === true
                ? this.allocationPlanService.geographiesGeographyIDAllocationPlansAllocationPlanIDAllocationPlanPeriodIDPut(
                      this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                      this.modalContext.AllocationPlanManageDto.AllocationPlanID,
                      this.modalContext.AllocationPlanPeriodSimpleDto.AllocationPlanPeriodID,
                      submitDto
                  )
                : this.allocationPlanService.geographiesGeographyIDAllocationPlansAllocationPlanIDPost(
                      this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                      this.modalContext.AllocationPlanManageDto.AllocationPlanID,
                      submitDto
                  );

        endPoint.subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert(`Successfully ${this.modalContext.Update === true ? "updated" : "created"} Allocation Plan Period.`, AlertContext.Success));
            this.modalService.close(this.modalComponentRef, response, new UpsertAllocationPeriodEvent(response));
        });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export interface AllocationPeriodContext {
    AllocationPlanManageDto: AllocationPlanManageDto | null;
    AllocationPlanPeriodSimpleDto: AllocationPlanPeriodSimpleDto | null;
    Update: boolean | null;
}

export class UpsertAllocationPeriodEvent extends ModalEvent {
    constructor(value: any) {
        super(value);
    }
}
