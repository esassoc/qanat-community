import { Component, ComponentRef, inject } from "@angular/core";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { FormGroup, FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, shareReplay, switchMap, tap } from "rxjs";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ReportingPeriodDto } from "src/app/shared/generated/model/reporting-period-dto";
import { WaterAccountParcelByParcelService } from "src/app/shared/generated/api/water-account-parcel-by-parcel.service";
import { UpdateWaterAccountParcelsByParcelDto, WaterAccountMinimalAndReportingPeriodSimpleDto, WaterAccountReportingPeriodDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "parcel-update-water-account-modal",
    imports: [FormsModule, ReactiveFormsModule, FormFieldComponent, AsyncPipe],
    templateUrl: "./parcel-update-water-account-modal.component.html",
    styleUrls: ["./parcel-update-water-account-modal.component.scss"],
})
export class ParcelUpdateWaterAccountModalComponent {
    public ref: DialogRef<ParcelContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public parcel$: Observable<ParcelMinimalDto>;
    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public waterAccountParcels$: Observable<WaterAccountMinimalAndReportingPeriodSimpleDto[]>;

    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup = new FormGroup({});

    public waterAccountFormControls: { [reportingPeriodID: string]: FormControl<number> };

    constructor(
        private alertService: AlertService,
        private parcelService: ParcelService,
        private waterAccountParcelByParcelService: WaterAccountParcelByParcelService,
        private reportingPeriodService: ReportingPeriodService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.getByIDParcel(this.ref.data.ParcelID);

        this.reportingPeriods$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.ref.data.GeographyID).pipe(
            tap((reportingPeriods) => {
                // Initialize Form Controls for each Reporting Period
                this.waterAccountFormControls = reportingPeriods.reduce(
                    (acc, reportingPeriod) => {
                        acc[reportingPeriod.ReportingPeriodID] = new FormControl<number>(null, []);
                        this.formGroup.addControl(reportingPeriod.ReportingPeriodID.toString(), acc[reportingPeriod.ReportingPeriodID]);
                        return acc;
                    },
                    {} as { [reportingPeriodID: number]: FormControl<number> }
                );
            }),
            shareReplay(1) // Prevents multiple API calls and form resets
        );

        this.waterAccountParcels$ = this.reportingPeriods$.pipe(
            switchMap(() => this.waterAccountParcelByParcelService.getWaterAccountParcelsForParcelWaterAccountParcelByParcel(this.ref.data.ParcelID)),
            tap((waterAccountParcels) => {
                waterAccountParcels.forEach((waterAccountParcel) => {
                    const reportingPeriodID = waterAccountParcel.ReportingPeriod.ReportingPeriodID;
                    if (this.formGroup.controls[reportingPeriodID.toString()]) {
                        this.formGroup.controls[reportingPeriodID.toString()].setValue(waterAccountParcel.WaterAccount.WaterAccountID);
                    }
                });
            })
        );
    }

    close() {
        this.ref.close(false);
    }

    save() {
        this.isLoadingSubmit = true;
        let values = this.formGroup.value;
        let reportingPeriodWaterAccounts: WaterAccountReportingPeriodDto[] = Object.keys(values).map((reportingPeriodID) => ({
            ReportingPeriodID: parseInt(reportingPeriodID),
            WaterAccountID: values[reportingPeriodID],
        }));

        let updateWaterAccountParcelsByParcelDto: UpdateWaterAccountParcelsByParcelDto = {
            ReportingPeriodWaterAccounts: reportingPeriodWaterAccounts,
        };

        this.waterAccountParcelByParcelService.updateWaterAccountParcelsForParcelWaterAccountParcelByParcel(this.ref.data.ParcelID, updateWaterAccountParcelsByParcelDto).subscribe(
            () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully updated water accounts for parcel", AlertContext.Success));
                this.ref.close(true);
            },
            (error: any) => {
                this.isLoadingSubmit = false;
            },
            () => {
                this.isLoadingSubmit = false;
            }
        );
    }
}
