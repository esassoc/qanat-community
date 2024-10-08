import { Component, ComponentRef, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IModal, ModalEvent, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AllocationPeriodContext } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { Observable, of } from "rxjs";
import { switchMap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { AllocationPlanMinimalDto } from "src/app/shared/generated/model/allocation-plan-minimal-dto";

@Component({
    selector: "copy-existing-allocation-plan-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, CustomRichTextComponent, FormFieldComponent, NoteComponent],
    templateUrl: "./copy-existing-allocation-plan-modal.component.html",
    styleUrls: ["./copy-existing-allocation-plan-modal.component.scss"],
    animations: [inOutAnimation],
})
export class CopyExistingAllocationPlanModalComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: AllocationPeriodContext;
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
        private modalService: ModalService,
        private allocationPlanService: AllocationPlanService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.allocationPlanDropdownOptions$ = this.allocationPlanService
            .publicGeographyGeographyIDAllocationPlansGet(this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID)
            .pipe(
                switchMap((plans) => {
                    return of([
                        { Value: null, Disabled: true, Label: "Select an Allocation Plan" },
                        ...plans
                            .filter((x) => x.AllocationPlanID != this.modalContext.AllocationPlanManageDto.AllocationPlanID && x.AllocationPeriodsCount > 0)
                            .map((x) => {
                                return { Value: x, Label: `${x.WaterTypeName} / ${x.ZoneName}` } as SelectDropdownOption;
                            }),
                    ]);
                })
            );
    }

    save(): void {
        this.allocationPlanService
            .geographiesGeographyIDAllocationPlansCopyToAllocationPlanIDCopyFromCopyFromAllocationPlanIDPost(
                this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                this.modalContext.AllocationPlanManageDto.AllocationPlanID,
                this.formGroup.controls.SelectedAllocationPlan.value.AllocationPlanID
            )
            .subscribe((response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Successfully copied Allocation Plan.`, AlertContext.Success));
                this.modalService.close(this.modalComponentRef, response, new CopiedAllocationPlanEvent(response));
            });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class CopiedAllocationPlanEvent extends ModalEvent {
    constructor(value: any) {
        super(value);
    }
}
export interface CopyAllocationPlanForm {
    SelectedAllocationPlan: FormControl<AllocationPlanMinimalDto>;
}
