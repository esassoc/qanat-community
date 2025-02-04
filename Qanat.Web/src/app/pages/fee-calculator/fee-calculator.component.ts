import { AsyncPipe, CurrencyPipe, DecimalPipe, JsonPipe, KeyValuePipe, NgClass, NgFor, NgIf } from "@angular/common";
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, catchError, debounce, delay, map, of, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterAccountCardComponent } from "src/app/shared/components/water-account/water-account-card/water-account-card.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FeeCalculatorService } from "src/app/shared/generated/api/fee-calculator.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    FeeCalculatorInputDto,
    FeeCalculatorInputDtoForm,
    FeeCalculatorInputDtoFormControls,
    FeeCalculatorInputOptionsDto,
    FeeCalculatorOutputDto,
    FeeStructureDto,
    WaterAccountParcelWaterMeasurementDto,
} from "src/app/shared/generated/model/models";
import { FeeCalculatorScenarioDisplayComponent } from "./fee-calculator-scenario-display/fee-calculator-scenario-display.component";
import { WaterAccountParcelWaterMeasurementsGridComponent } from "../water-accounts/water-budget/components/water-account-parcel-water-measurements-grid/water-account-parcel-water-measurements-grid.component";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";

@Component({
    selector: "fee-calculator",
    standalone: true,
    imports: [
        NgIf,
        NgFor,
        PageHeaderComponent,
        FormsModule,
        FormFieldComponent,
        ReactiveFormsModule,
        LoadingDirective,
        ExpandCollapseDirective,
        WaterAccountCardComponent,
        AlertDisplayComponent,
        CustomRichTextComponent,
        AsyncPipe,
        IconComponent,
        FieldDefinitionComponent,
        DecimalPipe,
        FeeCalculatorScenarioDisplayComponent,
        NgClass,
        WaterAccountParcelWaterMeasurementsGridComponent,
    ],
    templateUrl: "./fee-calculator.component.html",
    styleUrl: "./fee-calculator.component.scss",
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeeCalculatorComponent implements OnInit {
    public isLoadingInputs: boolean = true;
    public isLoadingOutput: boolean = true;
    public inputs$: Observable<FeeCalculatorInputOptionsDto>;
    public inputsTS: FeeCalculatorInputOptionsDto;

    public output: FeeCalculatorOutputDto;

    public inputFormGroup = new FormGroup<FeeCalculatorInputDtoForm>({
        WaterAccountID: FeeCalculatorInputDtoFormControls.WaterAccountID(),
        ReportingYear: FeeCalculatorInputDtoFormControls.ReportingYear(),
        FeeStructureID: FeeCalculatorInputDtoFormControls.FeeStructureID(),
        SurfaceWaterDelivered: FeeCalculatorInputDtoFormControls.SurfaceWaterDelivered(),
        SurfaceWaterIrrigationEfficiency: FeeCalculatorInputDtoFormControls.SurfaceWaterIrrigationEfficiency(),
    });

    public surfaceWaterToggleControl: FormControl = new FormControl(false);
    public showSurfaceWaterFields: boolean = false;

    //string to control dictionary
    public mlrpIncentiveAcreControls: { [key: string]: FormControl } = {};

    public waterAccountOptions: FormInputOption[];
    public yearOptions: FormInputOption[];
    public feeStructureOptions: FormInputOption[];
    public selectedFeeStructure: FeeStructureDto;

    public FormFieldType = FormFieldType;
    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;

    public debounceTime = 1000 * 1; // 1 second

    public selectedPanel: string = "Summary";

    public mergedFeeStructureReportData = [];

    public waterMeasurements: WaterAccountParcelWaterMeasurementDto[];

    constructor(
        private feeCalculatorService: FeeCalculatorService,
        private waterAccountService: WaterAccountService,
        private decimalPipe: DecimalPipe,
        private route: ActivatedRoute,
        private router: Router,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit() {
        this.route.params.subscribe((params) => {
            if (params && params.geographyName) {
                this.inputs$ = this.feeCalculatorService.feeCalculatorGeographyNameInputsGet(params.geographyName).pipe(
                    tap((inputs) => {
                        setTimeout(() => {
                            this.setUpControls(inputs);

                            this.route.queryParams.subscribe((params) => {
                                if (params && params["selectedWaterAccountID"]) {
                                    let selectedWaterAccountID = parseInt(params["selectedWaterAccountID"]);
                                    this.inputFormGroup.get("WaterAccountID").setValue(selectedWaterAccountID);
                                    this.updateSelectedWaterAccountID(selectedWaterAccountID);
                                }
                            });
                        });
                    }),
                    catchError((error: any) => {
                        if (error.status === 404) {
                            //redirect to 404 page, this wasn't happening when I tested a bogus geography name
                            this.router.navigate(["/not-found"]);
                        }

                        return of(null);
                    })
                );
            }
        });
    }

    private setUpControls(inputs: FeeCalculatorInputOptionsDto) {
        this.inputsTS = inputs;
        this.output = inputs.InitialOutput;
        this.isLoadingInputs = false;
        this.isLoadingOutput = false;

        this.waterAccountOptions = inputs.WaterAccounts.map((wa) => {
            return {
                Value: wa.WaterAccountID,
                Label: `${wa.WaterAccountNameAndNumber}`,
            } as FormInputOption;
        });

        this.inputFormGroup.get("WaterAccountID").setValue(inputs.InitialInputs.WaterAccountID);
        this.inputFormGroup.get("WaterAccountID").valueChanges.subscribe((waterAccountID: number) => {
            this.updateSelectedWaterAccountID(waterAccountID);
        });

        this.yearOptions = inputs.Years.map((year) => {
            return {
                Value: year.Year,
                Label: year.Name,
            } as FormInputOption;
        });

        this.inputFormGroup.get("ReportingYear").setValue(inputs.InitialInputs.ReportingYear);
        this.inputFormGroup.get("ReportingYear").valueChanges.subscribe((value) => {
            this.updateSelectedReportingYear(value);
        });

        this.feeStructureOptions = inputs.FeeStructures.map((fs) => {
            return {
                Value: fs.FeeStructureID,
                Label: fs.Name,
            } as FormInputOption;
        });

        this.cdr.markForCheck();
        this.inputFormGroup.get("FeeStructureID").setValue(inputs.InitialInputs.FeeStructureID);
        this.inputFormGroup.get("FeeStructureID").valueChanges.subscribe((value) => {
            this.inputsTS.InitialInputs.FeeStructureID = value;

            let feeStructure = this.inputsTS.FeeStructures.find((x) => x.FeeStructureID === value);
            if (feeStructure) {
                this.selectedFeeStructure = feeStructure;
            }

            this.calculate();
            this.cdr.markForCheck();
        });

        if (this.inputsTS.InitialInputs.SurfaceWaterDelivered) {
            this.surfaceWaterToggleControl.setValue(true);
            this.inputFormGroup.get("SurfaceWaterDelivered").setValue(inputs.InitialInputs.SurfaceWaterDelivered);
        }

        this.inputFormGroup
            .get("SurfaceWaterDelivered")
            .valueChanges.pipe(
                debounce((value) => {
                    return of(value).pipe(
                        delay(this.debounceTime),
                        tap(() => {
                            this.inputsTS.InitialInputs.SurfaceWaterDelivered = value;
                            this.calculate();
                        })
                    );
                })
            )
            .subscribe();

        if (this.inputsTS.InitialInputs.SurfaceWaterIrrigationEfficiency) {
            this.inputFormGroup.get("SurfaceWaterIrrigationEfficiency").setValue(inputs.InitialInputs.SurfaceWaterIrrigationEfficiency);
        }

        this.inputFormGroup.get("SurfaceWaterIrrigationEfficiency").setValidators([Validators.min(0), Validators.max(100)]);

        this.inputFormGroup
            .get("SurfaceWaterIrrigationEfficiency")
            .valueChanges.pipe(
                debounce((value) => {
                    return of(value).pipe(
                        delay(this.debounceTime),
                        tap(() => {
                            this.inputsTS.InitialInputs.SurfaceWaterIrrigationEfficiency = value;
                            this.calculate();
                        })
                    );
                })
            )
            .subscribe();

        this.surfaceWaterToggleControl.valueChanges.subscribe((value) => {
            this.showSurfaceWaterFields = value;
            this.inputFormGroup.get("SurfaceWaterDelivered").setValue(0);
            this.inputFormGroup.get("SurfaceWaterIrrigationEfficiency").setValue(0);
            this.cdr.markForCheck();
        });

        this.updateMLRPControls();
        this.mergeFeeStructureReportData();
    }

    private updateSelectedWaterAccountID(waterAccountID: number) {
        this.inputsTS.InitialInputs.WaterAccountID = waterAccountID;

        this.router.navigate([], {
            relativeTo: this.route,
            queryParams: { selectedWaterAccountID: waterAccountID },
            queryParamsHandling: "merge",
        });

        this.calculate(true);

        //this.updateMLRPControls();
        //this.updateWaterMeasurements();
    }

    private updateMLRPControls() {
        this.inputsTS.MLRPIncentives.forEach((mlrp) => {
            // Safely get or create the FormControl<number>
            let control = this.inputFormGroup.get(`${mlrp.Name}`);
            let mlrpIncentiveControl: FormControl<number>;

            let acresLimit = parseFloat(this.decimalPipe.transform(this.output.BaselineScenario.Acres, "1.2-2"));

            if (control instanceof FormControl) {
                mlrpIncentiveControl = control as FormControl<number>;
                mlrpIncentiveControl.setValue(0);
                mlrpIncentiveControl.setValidators([Validators.min(0), Validators.max(acresLimit)]);
                mlrpIncentiveControl.updateValueAndValidity();
                mlrpIncentiveControl.reset();
            } else {
                mlrpIncentiveControl = new FormControl<number>(0, [Validators.min(0), Validators.max(acresLimit)]);

                // Add the control to the form group so the value changes are tracked.
                (this.inputFormGroup as any).addControl(`${mlrp.Name}`, mlrpIncentiveControl);
                mlrpIncentiveControl.valueChanges
                    .pipe(
                        tap((value) => {
                            let mlrpIncentive = this.inputsTS.MLRPIncentives.find((x) => x.Name === mlrp.Name);
                            mlrpIncentive.Acres = value as any;
                        }),
                        debounce((value) => {
                            return of(value).pipe(
                                delay(this.debounceTime),
                                tap((val) => {
                                    if (!val) {
                                        mlrpIncentiveControl.setValue(0, { emitEvent: false, onlySelf: true, emitModelToViewChange: true, emitViewToModelChange: false });
                                    }

                                    this.isLoadingOutput = true;
                                    this.cdr.markForCheck();
                                    this.calculate();
                                })
                            );
                        })
                    )
                    .subscribe();
            }

            this.mlrpIncentiveAcreControls[mlrp.Name] = mlrpIncentiveControl;
        });

        this.cdr.markForCheck();
    }

    private updateSelectedReportingYear(reportingYear: number) {
        this.inputsTS.InitialInputs.ReportingYear = reportingYear;
        this.calculate();
        this.updateWaterMeasurements();
    }

    private updateWaterMeasurements() {
        this.waterAccountService
            .waterAccountsWaterAccountIDParcelSuppliesYearsYearMonthlyUsageSummaryGet(this.inputsTS.InitialInputs.WaterAccountID, this.inputsTS.InitialInputs.ReportingYear)
            .subscribe((result) => {
                this.waterMeasurements = result;
                this.cdr.markForCheck();
            });
    }

    private calculate(updateForm: boolean = false) {
        let inputs = new FeeCalculatorInputDto({
            WaterAccountID: this.inputsTS.InitialInputs.WaterAccountID,
            ReportingYear: this.inputsTS.InitialInputs.ReportingYear,
            FeeStructureID: this.inputsTS.InitialInputs.FeeStructureID,
            SurfaceWaterDelivered: this.inputFormGroup.get("SurfaceWaterDelivered").value,
            SurfaceWaterIrrigationEfficiency: this.inputFormGroup.get("SurfaceWaterIrrigationEfficiency").value,
            MLRPIncentives: [...this.inputsTS.MLRPIncentives],
        });

        this.feeCalculatorService.feeCalculatorGeographyNameCalculatePost(this.route.snapshot.params.geographyName, inputs).subscribe((result) => {
            this.output = result;
            this.mergeFeeStructureReportData();
            this.isLoadingOutput = false;

            if (updateForm) {
                this.updateMLRPControls();
            }

            this.cdr.markForCheck();
        });
    }

    private mergeFeeStructureReportData() {
        this.mergedFeeStructureReportData = this.output.BaselineScenario.CategoryBreakdown.map((category) => {
            let landuseScenarioCategory = this.output.LandUseChangeScenario.CategoryBreakdown.find((x) => x.Name === category.Name);
            const isOverused =
                (!category.TotalAllocationInAcreFeet && category.AllocationUsedInAcreFeet) ||
                (landuseScenarioCategory && !landuseScenarioCategory.TotalAllocationInAcreFeet && landuseScenarioCategory.AllocationUsedInAcreFeet);

            return {
                Name: category.Name,
                Baseline: category,
                LandUseChange: landuseScenarioCategory,
                IsOverused: isOverused,
            };
        });
    }

    public toggleSelectedPanel(panel: "FeeStructureReport" | "YourData" | "WhatIsConsumedGroundwater" | "Summary") {
        this.selectedPanel = panel;
    }
}
