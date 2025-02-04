import { NgIf } from "@angular/common";
import { Component, ComponentRef, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { Subscription } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ReportingPeriodDto, ReportingPeriodUpsertDto, ReportingPeriodUpsertDtoForm, ReportingPeriodUpsertDtoFormControls } from "src/app/shared/generated/model/models";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "upsert-reporting-period-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, NgIf, AlertDisplayComponent],
    templateUrl: "./upsert-reporting-period-modal.component.html",
    styleUrl: "./upsert-reporting-period-modal.component.scss",
})
export class UpsertReportingPeriodModalComponent implements OnInit, OnDestroy {
    public FormFieldType = FormFieldType;
    public modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: UpsertReportingPeriodModalContext;

    public isNewReportingPeriod: boolean = true;
    public endDate: string;
    public addOrEditSubscription: Subscription;

    public formGroup = new FormGroup<ReportingPeriodUpsertDtoForm>({
        Name: ReportingPeriodUpsertDtoFormControls.Name(),
        StartDate: ReportingPeriodUpsertDtoFormControls.StartDate(),
        ReadyForAccountHolders: ReportingPeriodUpsertDtoFormControls.ReadyForAccountHolders(),
    });

    public endDateControl = new FormControl({}, []);
    public startDateChangeSubscription: Subscription;

    constructor(
        private reportingPeriodService: ReportingPeriodService,
        private modalService: ModalService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        if (this.modalContext.ReportingPeriod) {
            this.modalContext.ReportingPeriod.StartDate = new Date(this.modalContext.ReportingPeriod.StartDate).toISOString().split("T")[0];
            this.formGroup.patchValue(this.modalContext.ReportingPeriod);

            this.endDate = new Date(this.modalContext.ReportingPeriod.EndDate).toISOString().split("T")[0];
            this.endDateControl.setValue(this.endDate);
            this.isNewReportingPeriod = false;
        }

        this.endDateControl.disable();

        this.startDateChangeSubscription = this.formGroup.get("StartDate").valueChanges.subscribe((startDate: string) => {
            this.calculateEndDate(startDate);
        });
    }

    ngOnDestroy(): void {
        if (this.addOrEditSubscription && this.addOrEditSubscription.unsubscribe) {
            this.addOrEditSubscription.unsubscribe();
        }

        if (this.startDateChangeSubscription && this.startDateChangeSubscription.unsubscribe) {
            this.startDateChangeSubscription.unsubscribe();
        }
    }

    calculateEndDate(startDate: string): void {
        let endDate = new Date(startDate);
        // set end date to the next year minus one day
        endDate.setFullYear(endDate.getFullYear() + 1);
        endDate.setDate(endDate.getDate() - 1);

        this.endDate = endDate.toISOString().split("T")[0];
        this.endDateControl.setValue(this.endDate);
    }

    save(): void {
        let upsertDto = this.formGroup.value as ReportingPeriodUpsertDto;
        if (this.isNewReportingPeriod) {
            this.addOrEditSubscription = this.reportingPeriodService.geographiesGeographyIDReportingPeriodsPost(this.modalContext.GeographyID, upsertDto).subscribe(
                (response) => {
                    this.modalService.close(this.modalComponentRef, response);
                }
                // (error) => {
                //     //MK 2/3/2025: App Alert Display is on the parent page, not sure it makes sense to have it here as well.
                //     this.modalService.close(this.modalComponentRef, null);
                // }
            );
        } else {
            this.addOrEditSubscription = this.reportingPeriodService
                .geographiesGeographyIDReportingPeriodsReportingPeriodIDPut(this.modalContext.GeographyID, this.modalContext.ReportingPeriod.ReportingPeriodID, upsertDto)
                .subscribe(
                    (response) => {
                        this.modalService.close(this.modalComponentRef, response);
                    }
                    // (error) => {
                    //     //MK 2/3/2025: App Alert Display is on the parent page, not sure it makes sense to have it here as well.
                    //     this.modalService.close(this.modalComponentRef, null);
                    // }
                );
        }
    }

    cancel(): void {
        this.modalService.close(this.modalComponentRef, null);
    }
}

export class UpsertReportingPeriodModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
}
