import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import {
    CopyWaterAccountParcelsFromReportingPeriodDto,
    CopyWaterAccountParcelsFromReportingPeriodDtoForm,
    CopyWaterAccountParcelsFromReportingPeriodDtoFormControls,
    ReportingPeriodDto,
} from "src/app/shared/generated/model/models";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { FormFieldType, FormFieldComponent } from "src/app/shared/components/forms/form-field/form-field.component";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WaterAccountParcelByGeographyService } from "src/app/shared/generated/api/water-account-parcel-by-geography.service";
import { NoteComponent } from "../../../../shared/components/note/note.component";
import { Subscription } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "copy-water-account-parcels-modal",
    imports: [ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent, NoteComponent],
    templateUrl: "./copy-water-account-parcels-modal.component.html",
    styleUrl: "./copy-water-account-parcels-modal.component.scss"
})
export class CopyWaterAccountParcelsModalComponent implements OnInit, OnDestroy {
    public ref: DialogRef<CopyWaterAccountParcelsModalContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public reportingPeriodSelectOptions: SelectDropdownOption[] = [];

    public formGroup = new FormGroup<CopyWaterAccountParcelsFromReportingPeriodDtoForm>({
        FromReportingPeriodID: CopyWaterAccountParcelsFromReportingPeriodDtoFormControls.FromReportingPeriodID(),
    });

    public saveSubscription: Subscription;
    constructor(
        private waterAccountParcelByGeographyService: WaterAccountParcelByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        if (this.ref.data.ReportingPeriods) {
            this.reportingPeriodSelectOptions = this.ref.data.ReportingPeriods.filter((rp) => rp.ReportingPeriodID != this.ref.data.ToReportingPeriod.ReportingPeriodID).map(
                (reportingPeriod) => {
                    return {
                        Label: reportingPeriod.Name,
                        Value: reportingPeriod.ReportingPeriodID,
                    } as SelectDropdownOption;
                }
            );
        }
    }

    ngOnDestroy(): void {
        if (this.saveSubscription) {
            this.saveSubscription.unsubscribe();
        }
    }

    save(): void {
        let copyWaterAccountParcelsFromReportingPeriodDto = new CopyWaterAccountParcelsFromReportingPeriodDto({
            FromReportingPeriodID: this.formGroup.get("FromReportingPeriodID").value,
        });

        this.saveSubscription = this.waterAccountParcelByGeographyService
            .copyFromReportingPeriodWaterAccountParcelByGeography(
                this.ref.data.GeographyID,
                this.ref.data.ToReportingPeriod.ReportingPeriodID,
                copyWaterAccountParcelsFromReportingPeriodDto
            )
            .subscribe({
                next: () => {
                    this.ref.close(true);
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Copied Parcel assignments successfully.", AlertContext.Success));
                },
                error: () => {
                    this.alertService.pushAlert(new Alert("There was an error copying Parcel assignments.", AlertContext.Success));
                    this.ref.close(false);
                },
            });
    }

    cancel(): void {
        this.ref.close(false);
    }
}

export class CopyWaterAccountParcelsModalContext {
    GeographyID: number;
    ToReportingPeriod: ReportingPeriodDto;
    ReportingPeriods: ReportingPeriodDto[];
}
