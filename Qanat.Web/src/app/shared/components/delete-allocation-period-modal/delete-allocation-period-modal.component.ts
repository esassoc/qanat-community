import { Component, inject } from "@angular/core";

import { AlertService } from "src/app/shared/services/alert.service";
import { ReactiveFormsModule } from "@angular/forms";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AllocationPeriodContext } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "delete-allocation-period-modal",
    imports: [ReactiveFormsModule, NoteComponent],
    templateUrl: "./delete-allocation-period-modal.component.html",
    styleUrls: ["./delete-allocation-period-modal.component.scss"],
    animations: [inOutAnimation],
})
export class DeleteAllocationPeriodModalComponent {
    public ref: DialogRef<AllocationPeriodContext, number> = inject(DialogRef);

    constructor(
        private allocationPlanService: AllocationPlanService,
        private alertService: AlertService
    ) {}

    save(): void {
        this.allocationPlanService
            .deleteAllocationPlanPeriodAllocationPlan(
                this.ref.data.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                this.ref.data.AllocationPlanPeriodSimpleDto.AllocationPlanID,
                this.ref.data.AllocationPlanPeriodSimpleDto.AllocationPlanPeriodID
            )
            .subscribe((response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(
                    new Alert(`Successfully deleted Allocation Plan Period "${this.ref.data.AllocationPlanPeriodSimpleDto.AllocationPeriodName}".`, AlertContext.Success)
                );
                this.ref.close(this.ref.data.AllocationPlanPeriodSimpleDto.AllocationPlanPeriodID);
            });
    }

    close(): void {
        this.ref.close(null);
    }
}
