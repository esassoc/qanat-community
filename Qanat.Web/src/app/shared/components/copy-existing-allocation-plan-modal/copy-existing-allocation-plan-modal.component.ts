import { Component, inject, OnInit } from "@angular/core";

import { AlertService } from "src/app/shared/services/alert.service";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AllocationPeriodContext } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { Observable, of } from "rxjs";
import { switchMap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { AllocationPlanMinimalDto } from "src/app/shared/generated/model/allocation-plan-minimal-dto";
import { PublicService } from "../../generated/api/public.service";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "copy-existing-allocation-plan-modal",
    imports: [ReactiveFormsModule, CustomRichTextComponent, FormFieldComponent, NoteComponent, AsyncPipe],
    templateUrl: "./copy-existing-allocation-plan-modal.component.html",
    styleUrls: ["./copy-existing-allocation-plan-modal.component.scss"],
    animations: [inOutAnimation],
})
export class CopyExistingAllocationPlanModalComponent implements OnInit {
    public ref: DialogRef<AllocationPeriodContext, any> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<CopyAllocationPlanForm> = new FormGroup<CopyAllocationPlanForm>({
        SelectedAllocationPlan: new FormControl(null, {
            nonNullable: false,
            validators: [Validators.required],
        }),
    });

    public allocationPlanDropdownOptions$: Observable<SelectDropdownOption[]>;
    public richTextTypeID = CustomRichTextTypeEnum.CloneAllocationPlan;

    constructor(
        private allocationPlanService: AllocationPlanService,
        private publicService: PublicService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.allocationPlanDropdownOptions$ = this.publicService
            .listAllocationPlansByGeographyIDPublic(this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID)
            .pipe(
                switchMap((plans) => {
                    return of([
                        { Value: null, disabled: true, Label: "Select an Allocation Plan" },
                        ...plans
                            .filter((x) => x.AllocationPlanID != this.ref.data.AllocationPlanManageDto.AllocationPlanID && x.AllocationPeriodsCount > 0)
                            .map((x) => {
                                return { Value: x, Label: `${x.WaterTypeName} / ${x.ZoneName}` } as SelectDropdownOption;
                            }),
                    ]);
                })
            );
    }

    save(): void {
        this.allocationPlanService
            .copyAllocationPlanAllocationPlan(
                this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                this.ref.data.AllocationPlanManageDto.AllocationPlanID,
                this.formGroup.controls.SelectedAllocationPlan.value.AllocationPlanID
            )
            .subscribe((response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Successfully copied Allocation Plan.`, AlertContext.Success));
                this.ref.close(response);
            });
    }

    close(): void {
        this.ref.close(null);
    }
}

export interface CopyAllocationPlanForm {
    SelectedAllocationPlan: FormControl<AllocationPlanMinimalDto>;
}
