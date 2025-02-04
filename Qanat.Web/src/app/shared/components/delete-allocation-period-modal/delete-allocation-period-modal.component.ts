import { Component, ComponentRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IModal, ModalEvent, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { ReactiveFormsModule } from "@angular/forms";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AllocationPeriodContext } from "../upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "delete-allocation-period-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, NoteComponent],
    templateUrl: "./delete-allocation-period-modal.component.html",
    styleUrls: ["./delete-allocation-period-modal.component.scss"],
    animations: [inOutAnimation],
})
export class DeleteAllocationPeriodModalComponent implements IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: AllocationPeriodContext;

    constructor(private modalService: ModalService, private allocationPlanService: AllocationPlanService, private alertService: AlertService) {}

    save(): void {
        this.allocationPlanService
            .geographiesGeographyIDAllocationPlansAllocationPlanIDAllocationPlanPeriodIDDelete(
                this.modalContext.AllocationPlanManageDto.GeographyAllocationPlanConfiguration.GeographyID,
                this.modalContext.AllocationPlanPeriodSimpleDto.AllocationPlanID,
                this.modalContext.AllocationPlanPeriodSimpleDto.AllocationPlanPeriodID
            )
            .subscribe((response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(
                    new Alert(`Successfully deleted Allocation Plan Period "${this.modalContext.AllocationPlanPeriodSimpleDto.AllocationPeriodName}".`, AlertContext.Success)
                );
                this.modalService.close(this.modalComponentRef, response, new DeletedAllocationPeriodEvent(response));
            });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class DeletedAllocationPeriodEvent extends ModalEvent {
    constructor(value: any) {
        super(value);
    }
}
