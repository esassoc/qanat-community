import { Component, inject, OnInit } from "@angular/core";
import { AlertService } from "src/app/shared/services/alert.service";
import { CreateWaterAccountFromSuggestionDto, ParcelDisplayDto, ParcelWithGeoJSONDto, ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { CustomGeoJSONLayer } from "../../../parcel/parcel-map/parcel-map.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Validators, FormsModule, ReactiveFormsModule, FormControl, FormGroup } from "@angular/forms";
import { DecimalPipe, AsyncPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { Observable, of, switchMap } from "rxjs";
import { NgSelectModule } from "@ng-select/ng-select";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "bulk-approve-water-account-suggestion",
    templateUrl: "./bulk-approve-water-account-suggestion.component.html",
    styleUrls: ["./bulk-approve-water-account-suggestion.component.scss"],
    imports: [AsyncPipe, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, DecimalPipe, NgSelectModule],
})
export class BulkApproveWaterAccountSuggestionComponent implements OnInit {
    public ref: DialogRef<BulkApproveWaterAccountSuggestionContext, boolean> = inject(DialogRef);

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;

    public FormFieldType = FormFieldType;
    public formGroup: FormGroup<{
        reportingPeriodID: FormControl<number>;
    }> = new FormGroup({
        reportingPeriodID: new FormControl<number>(null, [Validators.required]),
    });

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.reportingPeriods$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.ref.data.GeographyID);

        this.reportingPeriodSelectOptions$ = this.reportingPeriods$.pipe(
            switchMap((reportingPeriods) => {
                let options = reportingPeriods.map((x) => (({
                    Value: x.ReportingPeriodID,
                    Label: x.Name
                }) as SelectDropdownOption));
                return of(options);
            })
        );
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;
        const reportingPeriodID = this.formGroup.get("reportingPeriodID").value;
        this.ref.data.WaterAccountSuggestions.forEach((x) => (x.ReportingPeriodID = reportingPeriodID));
        this.waterAccountByGeographyService
            .bulkCreateWaterAccountFromSuggestionWaterAccountByGeography(this.ref.data.GeographyID, this.ref.data.WaterAccountSuggestions)
            .subscribe((response) => {
                this.alertService.pushAlert(
                    new Alert(
                        `Successfully approved ${this.ref.data.WaterAccountSuggestions.length} water account${this.ref.data.WaterAccountSuggestions.length == 1 ? "" : "s"}.`,
                        AlertContext.Success
                    )
                );
                this.ref.close(true);
                this.isLoadingSubmit = false;
            });
    }
}

export interface BulkApproveWaterAccountSuggestionContext {
    GeographyID: number;
    WaterAccountSuggestions: CreateWaterAccountFromSuggestionDto[];
}
