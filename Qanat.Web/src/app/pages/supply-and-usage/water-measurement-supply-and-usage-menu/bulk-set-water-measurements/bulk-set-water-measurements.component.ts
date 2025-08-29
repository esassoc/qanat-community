import { AsyncPipe } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { combineLatest, map, Observable, of, startWith, switchMap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto, WaterMeasurementBulkSetDto, WaterMeasurementBulkSetDtoForm, WaterMeasurementBulkSetDtoFormControls } from "src/app/shared/generated/model/models";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { Router, RouterLink } from "@angular/router";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "bulk-set-water-measurements",
    imports: [AsyncPipe, PageHeaderComponent, FormFieldComponent, AlertDisplayComponent, ReactiveFormsModule, NoteComponent, LoadingDirective, RouterLink],
    templateUrl: "./bulk-set-water-measurements.component.html",
    styleUrl: "./bulk-set-water-measurements.component.scss"
})
export class BulkSetWaterMeasurementsComponent implements OnInit {
    public FormFieldType = FormFieldType;
    public isLoading$: Observable<boolean> = of(true);
    public geography$: Observable<GeographyMinimalDto>;
    public viewModel$: Observable<BulkSetWaterMeasurementViewModel>;
    public waterMeasurementTypesSelectOptions$: Observable<SelectDropdownOption[]>;
    public yearSelectOptions$: Observable<SelectDropdownOption[]>;
    public monthOptions$: Observable<SelectDropdownOption[]>;

    public richTextTypeID = CustomRichTextTypeEnum.BulkSetWaterMeasurementForm;

    public isLoadingSubmit = false;
    public formGroup = new FormGroup<WaterMeasurementBulkSetDtoForm>({
        WaterMeasurementTypeID: WaterMeasurementBulkSetDtoFormControls.WaterMeasurementTypeID(),
        Year: WaterMeasurementBulkSetDtoFormControls.Year(),
        Month: WaterMeasurementBulkSetDtoFormControls.Month(),
        ValueInAcreFeetPerAcre: WaterMeasurementBulkSetDtoFormControls.ValueInAcreFeetPerAcre(),
        Comment: WaterMeasurementBulkSetDtoFormControls.Comment(),
    });

    public constructor(
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private reportingPeriodService: ReportingPeriodService,
        private waterMeasurementService: WaterMeasurementService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.waterMeasurementTypesSelectOptions$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterMeasurementTypeService.getWaterMeasurementTypesWaterMeasurementType(geography.GeographyID).pipe(
                    map((waterMeasurementTypes) => {
                        return waterMeasurementTypes.filter((x) => x.IsUserEditable).sort((a, b) => a.SortOrder - b.SortOrder);
                    }),
                    map((filteredTypes) => {
                        return filteredTypes.map(
                            (x) =>
                                (({
                                    Label: x.WaterMeasurementTypeName,
                                    Value: x.WaterMeasurementTypeID
                                }) as SelectDropdownOption)
                        );
                    })
                );
            })
        );

        this.yearSelectOptions$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID);
            }),
            switchMap((reportingPeriods) => {
                let years = reportingPeriods.map((x) => new Date(x.EndDate).getFullYear());
                let yearSelectOptions = years.map((year) => {
                    return {
                        Label: year.toString(),
                        Value: year,
                    } as SelectDropdownOption;
                });

                return of(yearSelectOptions);
            })
        );

        this.monthOptions$ = this.yearSelectOptions$.pipe(
            switchMap((yearOptions) => {
                return of(UtilityFunctionsService.months).pipe(
                    map((months) => {
                        return months.map((month, index) => {
                            return {
                                Label: month,
                                Value: index + 1,
                            } as SelectDropdownOption;
                        });
                    })
                );
            })
        );

        this.viewModel$ = this.geography$.pipe(
            switchMap((geography) => {
                this.formGroup.reset();
                return combineLatest([this.geography$, this.waterMeasurementTypesSelectOptions$, this.yearSelectOptions$, this.monthOptions$]).pipe(
                    map(([geography, waterMeasurementTypesSelectOptions, yearSelectOptions, monthOptions]) => {
                        return {
                            IsLoading: false,
                            Geography: geography,
                            WaterMeasurementTypeSelectOptions: waterMeasurementTypesSelectOptions,
                            YearSelectOptions: yearSelectOptions,
                            MonthSelectOptions: monthOptions,
                        } as BulkSetWaterMeasurementViewModel;
                    }),
                    startWith({
                        IsLoading: true,
                        Geography: geography,
                        WaterMeasurementTypeSelectOptions: [],
                        YearSelectOptions: [],
                        MonthSelectOptions: [],
                    } as BulkSetWaterMeasurementViewModel) // Initialize with an empty object to avoid undefined error
                );
            })
        );
    }

    public onSubmit(geography: GeographyMinimalDto): void {
        this.isLoadingSubmit = true;
        let waterMeasurementBulkSetDto = new WaterMeasurementBulkSetDto(this.formGroup.value);
        this.waterMeasurementService.bulkSetWaterMeasurementsWaterMeasurement(geography.GeographyID, waterMeasurementBulkSetDto).subscribe(
            (result) => {
                this.isLoadingSubmit = false;
                this.router.navigate(["/supply-and-usage", geography.GeographyName.toLowerCase(), "water-measurements"]).then(() => {
                    this.alertService.pushAlert(new Alert(`Water measurements have been set successfully.`, AlertContext.Success));
                });
            },
            (error) => {
                this.isLoadingSubmit = false;
            }
        );
    }
}

interface BulkSetWaterMeasurementViewModel {
    IsLoading: boolean;
    Geography: GeographyMinimalDto;
    WaterMeasurementTypeSelectOptions: SelectDropdownOption[];
    YearSelectOptions: SelectDropdownOption[];
    MonthSelectOptions: SelectDropdownOption[];
}
