import { AsyncPipe } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Observable, of, switchMap, tap } from "rxjs";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { UsageLocationByGeographyService } from "src/app/shared/generated/api/usage-location-by-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "create-usage-locations-from-parcels-modal",
    imports: [FormFieldComponent, ButtonLoadingDirective, AsyncPipe, NoteComponent, LoadingDirective],
    templateUrl: "./create-usage-locations-from-parcels-modal.component.html",
    styleUrl: "./create-usage-locations-from-parcels-modal.component.scss"
})
export class CreateUsageLocationsFromParcelsModalComponent implements OnInit {
    public ref: DialogRef<CreateUsageLocationsFromParcelsModalContext, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public formGroup: FormGroup<{
        reportingPeriodID: FormControl<number>;
    }> = new FormGroup({
        reportingPeriodID: new FormControl<number>(null, [Validators.required]),
    });

    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;
    public isLoading: boolean = true;
    public isLoadingSubmit: boolean = false;

    constructor(
        private reportingPeriodService: ReportingPeriodService,
        private usageLocationByGeographyService: UsageLocationByGeographyService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.reportingPeriodSelectOptions$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.ref.data.Geography.GeographyID).pipe(
            switchMap((reportingPeriods) => {
                let options = reportingPeriods.map((x) => (({
                    Value: x.ReportingPeriodID,
                    Label: x.Name
                }) as SelectDropdownOption));
                return of(options);
            }),
            tap(() => {
                this.isLoading = false;
            })
        );
    }

    public close() {
        this.ref.close(false);
    }

    save() {
        let reportingPeriodID = this.formGroup.get("reportingPeriodID").value;
        this.usageLocationByGeographyService.replaceFromParcelsUsageLocationByGeography(this.ref.data.Geography.GeographyID, reportingPeriodID).subscribe((results) => {
            this.alertService.pushAlert(new Alert("Usage Locations created from parcels.", AlertContext.Success));
            this.ref.close(true);
        });
    }
}

export class CreateUsageLocationsFromParcelsModalContext {
    Geography: GeographyMinimalDto;
}
