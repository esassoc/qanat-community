import { Component, inject, OnDestroy } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { AlertService } from "../../services/alert.service";
import { OpenETSyncService } from "../../generated/api/open-et-sync.service";
import { UsageLocationService } from "../../generated/api/usage-location.service";
import { IconComponent } from "../icon/icon.component";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { ReportingPeriodSelectComponent } from "../reporting-period-select/reporting-period-select.component";
import { ReportingPeriodDto } from "../../generated/model/reporting-period-dto";
import { OpenETSyncDto } from "../../generated/model/open-et-sync-dto";
import { BehaviorSubject, combineLatest, filter, map, Observable, of, shareReplay, Subscription, tap } from "rxjs";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "../forms/form-field/form-field.component";
import { OpenETDataTypesAsSelectDropdownOptions } from "../../generated/enum/open-e-t-data-type-enum";
import { HangfireBackgroundJobResultDto, UsageLocationDto } from "../../generated/model/models";
import { NoteComponent } from "../note/note.component";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "recalculate-open-et-sync-raster-data-modal",
    imports: [ReactiveFormsModule, ReportingPeriodSelectComponent, FormFieldComponent, AsyncPipe, NoteComponent],
    templateUrl: "./recalculate-open-et-sync-raster-data-modal.component.html",
    styleUrl: "./recalculate-open-et-sync-raster-data-modal.component.scss",
})
export class RecalculateOpenETSyncRasterDataModalComponent implements OnDestroy {
    public ref: DialogRef<RecalculateOpenETSyncRasterDataModalModalContext, HangfireBackgroundJobResultDto> = inject(DialogRef);

    public openETSyncs$: Observable<OpenETSyncDto[]>;
    private allOpenETSyncs: OpenETSyncDto[] = [];
    public usageLocations$: Observable<UsageLocationDto[]>;
    private allUsageLocationsForParcel: UsageLocationDto[];
    public selectedUsageLocations: UsageLocationDto[];

    public formGroup: FormGroup<{
        OpenETDataTypeID: FormControl<number>;
        ReportingPeriodID: FormControl<number>;
        Month: FormControl<number>;
    }> = new FormGroup({
        OpenETDataTypeID: new FormControl(1, { nonNullable: true }),
        ReportingPeriodID: new FormControl(null, { nonNullable: true }),
        Month: new FormControl(null, { nonNullable: true }),
    });

    public OpenETDataTypeSelectOptions = OpenETDataTypesAsSelectDropdownOptions;
    public reportingPeriodSelected$: BehaviorSubject<ReportingPeriodDto> = new BehaviorSubject<ReportingPeriodDto>(null);
    private reportingPeriodSelected: ReportingPeriodDto;
    public monthOptions$: Observable<SelectDropdownOption[]>;
    public monthOptions: SelectDropdownOption[] = [];

    public subscriptions: Subscription[] = [];

    public FormFieldType = FormFieldType;

    constructor(
        private openETSyncRasterDataService: OpenETSyncService,
        private usageLocationService: UsageLocationService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.openETSyncs$ = this.openETSyncRasterDataService.listOpenETSyncsOpenETSync(this.ref.data.GeographyID).pipe(
            tap((syncs) => {
                this.allOpenETSyncs = syncs;
            })
        );

        this.usageLocations$ = this.ref.data.ParcelID
            ? this.usageLocationService.listUsageLocation(this.ref.data.GeographyID, this.ref.data.ParcelID).pipe(
                  tap((locations) => {
                      this.allUsageLocationsForParcel = locations;
                  })
              )
            : of([]);

        let usageLocationsSubscription = this.usageLocations$.subscribe((locations) => {
            this.selectedUsageLocations = locations.filter((location) => location.ReportingPeriod?.ReportingPeriodID === this.formGroup.value.ReportingPeriodID);
        });

        this.subscriptions.push(usageLocationsSubscription);

        this.monthOptions$ = combineLatest({ reportingPeriod: this.reportingPeriodSelected$, openETSyncs: this.openETSyncs$ }).pipe(
            filter(({ reportingPeriod }) => !!reportingPeriod),
            map(({ reportingPeriod, openETSyncs }) => {
                let reportingPeriodDate = new Date(reportingPeriod.EndDate);
                let year = reportingPeriodDate.getUTCFullYear();
                let currentDate = new Date(reportingPeriod.StartDate);
                let currentMonth = currentDate.getUTCMonth();

                let openETSyncsForYearAndDataType = openETSyncs.filter(
                    (sync) => sync.Year === year && sync.OpenETDataType.OpenETDataTypeID === this.formGroup.value.OpenETDataTypeID && sync.FileResourceGUID
                );

                let result = [];
                for (let i = 0; i < 12; i++) {
                    let label = currentDate.toLocaleString("default", { month: "short", timeZone: "UTC" });
                    let openETSync = openETSyncsForYearAndDataType.find((sync) => {
                        return sync.Month === currentMonth + 1;
                    });

                    if (!openETSync) {
                        label += " (No Raster Data)";
                    }

                    let month = { Label: label, Value: currentMonth + 1, disabled: !openETSync } as SelectDropdownOption;
                    result.push(month);
                    currentDate.setMonth(currentDate.getMonth() + 1);
                    currentMonth = currentDate.getUTCMonth();
                }
                this.monthOptions = result;
                return result;
            }),
            shareReplay(1)
        );

        if (this.ref.data.ReportingPeriodID) {
            this.formGroup.patchValue({
                ReportingPeriodID: this.ref.data.ReportingPeriodID,
            });
        }
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((sub) => sub.unsubscribe());
    }

    onReportingPeriodSelected(reportingPeriod: ReportingPeriodDto) {
        this.formGroup.patchValue({
            ReportingPeriodID: reportingPeriod.ReportingPeriodID,
            Month: null, // Reset month when reporting period changes
        });

        this.reportingPeriodSelected = reportingPeriod;
        this.reportingPeriodSelected$.next(reportingPeriod);
        if (this.allUsageLocationsForParcel) {
            this.selectedUsageLocations = this.allUsageLocationsForParcel.filter(
                (location) => location.ReportingPeriod?.ReportingPeriodID === this.formGroup.value.ReportingPeriodID
            );
        }
    }

    close() {
        this.ref.close(null);
    }

    save() {
        let usageLocationIDs = this.ref.data.UsageLocationIDs ?? this.selectedUsageLocations.map((location) => location.UsageLocationID);
        let reportingPeriodDate = new Date(this.reportingPeriodSelected.EndDate);
        let year = reportingPeriodDate.getUTCFullYear();
        let openETSync = this.allOpenETSyncs.find((sync) => {
            return sync.Year === year && sync.Month === this.formGroup.value.Month && sync.OpenETDataType.OpenETDataTypeID === this.formGroup.value.OpenETDataTypeID;
        });

        let monthOption = this.monthOptions.find((option) => option.Value === this.formGroup.value.Month);
        let calculateSubscription = this.openETSyncRasterDataService
            .calculateRasterForOpenETSyncOpenETSync(this.ref.data.GeographyID, openETSync.OpenETSyncID, {
                UsageLocationIDs: usageLocationIDs,
            })
            .subscribe({
                next: (result) => {
                    this.ref.close(result);
                    this.alertService.pushAlert(
                        new Alert(
                            `${monthOption.Label} ${this.reportingPeriodSelected.Name} raster data recalculation for ${this.ref.data.ParcelID ? "parcel has" : "usage locations have"} been queued successfully.`,
                            AlertContext.Success
                        )
                    );
                },
                error: (error) => {
                    this.alertService.pushAlert(
                        new Alert(`Failed to queue ${monthOption.Label} ${this.reportingPeriodSelected.Name} raster data recalculation: ${error.message}`, AlertContext.Danger)
                    );
                },
            });

        this.subscriptions.push(calculateSubscription);
    }
}

export class RecalculateOpenETSyncRasterDataModalModalContext {
    public GeographyID: number;
    public ParcelID: number | null; // If null, use UsageLocationIDs
    public UsageLocationIDs: number[] | null; // If null, use ParcelID to get all usage locations for the parcel
    public ReportingPeriodID: number | null; // If null, use the default reporting period
}
