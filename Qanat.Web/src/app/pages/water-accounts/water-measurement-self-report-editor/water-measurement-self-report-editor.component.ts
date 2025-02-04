import { AsyncPipe, NgIf, NgFor, DecimalPipe, NgClass, DatePipe } from "@angular/common";
import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { BehaviorSubject, Observable, Subscription, forkJoin, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { IrrigationMethodService } from "src/app/shared/generated/api/irrigation-method.service";
import { WaterAccountParcelService } from "src/app/shared/generated/api/water-account-parcel.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterMeasurementSelfReportService } from "src/app/shared/generated/api/water-measurement-self-report.service";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { WaterMeasurementSelfReportStatusEnum } from "src/app/shared/generated/enum/water-measurement-self-report-status-enum";
import {
    AllocationPlanMinimalDto,
    ParcelSimpleDto,
    UserDto,
    WaterAccountDto,
    WaterMeasurementSelfReportDto,
    WaterMeasurementSelfReportLineItemSimpleDto,
    WaterMeasurementSelfReportLineItemUpdateDto,
    WaterMeasurementSelfReportUpdateDto,
    WaterMeasurementTypeSimpleDto,
} from "src/app/shared/generated/model/models";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";
import { AlertService } from "src/app/shared/services/alert.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { SelfReportContext, SubmitSelfReportModalComponent } from "./submit-self-report-modal/submit-self-report-modal.component";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

@Component({
    selector: "water-measurement-self-report-editor",
    standalone: true,
    imports: [
        AsyncPipe,
        NgIf,
        PageHeaderComponent,
        RouterLink,
        NgFor,
        LoadingDirective,
        DecimalPipe,
        SumPipe,
        FormFieldComponent,
        NgClass,
        AlertDisplayComponent,
        ButtonLoadingDirective,
        DatePipe,
    ],
    templateUrl: "./water-measurement-self-report-editor.component.html",
    styleUrl: "./water-measurement-self-report-editor.component.scss",
    encapsulation: ViewEncapsulation.None,
})
export class WaterMeasurementSelfReportEditorComponent implements OnInit, OnDestroy, IDeactivateComponent {
    public FormFieldType = FormFieldType;
    public customRichTextTypeID: number = CustomRichTextTypeEnum.SelfReportEditorInstructions;
    public currentUser$: Observable<UserDto>;
    public currentUser: UserDto;

    private routeParamsUpdateSubscription: Subscription = Subscription.EMPTY;
    private waterAccountID: number;
    private selfReportID: number;
    public geographyID: number;

    public waterAccount: WaterAccountDto;
    public reportingYear: number;
    public pageIsLoading: boolean = true;
    public isLoadingSubmit: boolean = false;

    public allocationPlans: AllocationPlanMinimalDto[];
    public waterMeasurementType: WaterMeasurementTypeSimpleDto;

    public selfReport: WaterMeasurementSelfReportDto;
    public selfReport$: Observable<WaterMeasurementSelfReportDto>;
    public refreshSelfReportData$ = new BehaviorSubject<WaterMeasurementSelfReportDto | null>(null);

    public parcels: ParcelSimpleDto[];
    public lineItemViewModels: SelfReportLineItemViewModel[] = [];
    public irrigationMethodOptions: FormInputOption[];

    //string to control dictionary
    public irrigationMethodControlByAPN: { [key: string]: FormControl } = {};

    public monthTotals: { [key: string]: number } = {};
    public lineItemTotalsSum: string | number = "-";

    public canSave: boolean = false;

    public showSubmitButton: boolean = false;
    public canSubmit: boolean = false;

    public showApproveButton: boolean = false;
    public canApprove: boolean = false;

    public showReturnButton: boolean = false;
    public canReturn: boolean = false;

    public subscriptions: Subscription[] = [];

    public constructor(
        private waterAccountService: WaterAccountService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private waterMeasurementSelfReportService: WaterMeasurementSelfReportService,
        private waterAccountParcelService: WaterAccountParcelService,
        private irrigationMethodService: IrrigationMethodService,
        private modalService: ModalService,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                this.currentUser = currentUser;

                this.selfReport$ = this.refreshSelfReportData$.pipe(
                    tap((selfReport) => {
                        this.selfReport = selfReport;
                        this.lineItemViewModels = [];

                        let months = UtilityFunctionsService.months;

                        this.parcels.forEach((parcel) => {
                            let viewModel = new SelfReportLineItemViewModel();

                            //Find existing line item if exists, otherwise create a new one
                            let lineItem = selfReport.LineItems.find((li) => li.ParcelID === parcel.ParcelID) ?? new WaterMeasurementSelfReportLineItemSimpleDto();
                            months.forEach((month) => {
                                let monthControl = new FormControl(lineItem[month + "OverrideValueInAcreFeet"], { validators: [Validators.min(0)] });

                                monthControl.valueChanges.subscribe((value) => {
                                    this.setLineItemTotal(viewModel);
                                    this.setMonthTotal(month);
                                    this.updateLineItemTotals();
                                    this.updateCanSave();
                                });

                                viewModel.MonthFormControls[month] = monthControl;
                            });

                            viewModel.Parcel = parcel;
                            viewModel.LineItem = lineItem;
                            this.setLineItemTotal(viewModel);

                            this.irrigationMethodControlByAPN[parcel.ParcelNumber] = new FormControl(lineItem?.IrrigationMethodID);
                            this.irrigationMethodControlByAPN[parcel.ParcelNumber].valueChanges.subscribe((value) => {
                                let irrigationMethodID = lineItem.IrrigationMethodID;
                                let controlWasDirty = this.irrigationMethodControlByAPN[parcel.ParcelNumber].dirty;
                                if (value === null && lineItem.IrrigationMethodID) {
                                    const confirmOptions = {
                                        title: "Clear Parcel",
                                        message: `Are you sure you want to clear the Irrigation Method for this parcel? This will discard any changes made to the parcel.`,
                                        buttonClassYes: "btn btn-primary",
                                        buttonTextYes: "Continue",
                                        buttonTextNo: "Cancel",
                                    };

                                    this.confirmService.confirm(confirmOptions).then((confirmed) => {
                                        if (confirmed) {
                                            lineItem.IrrigationMethodID = value;

                                            lineItem.JanuaryOverrideValueInAcreFeet = null;
                                            lineItem.FebruaryOverrideValueInAcreFeet = null;
                                            lineItem.MarchOverrideValueInAcreFeet = null;
                                            lineItem.AprilOverrideValueInAcreFeet = null;
                                            lineItem.MayOverrideValueInAcreFeet = null;
                                            lineItem.JuneOverrideValueInAcreFeet = null;
                                            lineItem.JulyOverrideValueInAcreFeet = null;
                                            lineItem.AugustOverrideValueInAcreFeet = null;
                                            lineItem.SeptemberOverrideValueInAcreFeet = null;
                                            lineItem.OctoberOverrideValueInAcreFeet = null;
                                            lineItem.NovemberOverrideValueInAcreFeet = null;
                                            lineItem.DecemberOverrideValueInAcreFeet = null;
                                            months.forEach((month) => {
                                                viewModel.MonthFormControls[month].setValue(null, {});
                                            });

                                            this.updateCanSave();
                                        } else {
                                            let control = this.irrigationMethodControlByAPN[parcel.ParcelNumber];
                                            control.setValue(irrigationMethodID, { emitEvent: false });
                                            control.markAsPristine();
                                        }
                                    });
                                }

                                if (value != lineItem.IrrigationMethodID) {
                                    lineItem.IrrigationMethodID = value;
                                    this.updateCanSave();
                                }
                            });

                            this.lineItemViewModels.push(viewModel);
                        });

                        months.forEach((month) => {
                            this.setMonthTotal(month);
                        });

                        this.updateLineItemTotals();
                        setTimeout(() => {
                            this.reportingYear = selfReport.ReportingYear;
                        });

                        this.showSubmitButton =
                            selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Draft ||
                            selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Returned;

                        const isSystemAdminOrGeographyManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(this.currentUser, this.geographyID);
                        this.showApproveButton =
                            selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Submitted &&
                            isSystemAdminOrGeographyManager;

                        this.showReturnButton =
                            selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Submitted &&
                            isSystemAdminOrGeographyManager;

                        this.updateCanSave();
                    })
                );

                this.routeParamsUpdateSubscription = this.route.paramMap.subscribe((paramMap) => {
                    this.waterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
                    this.selfReportID = parseInt(paramMap.get(routeParams.selfReportID));
                    this.pageIsLoading = true;
                    this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountID).subscribe((waterAccount) => {
                        this.waterAccount = waterAccount;
                        this.geographyID = waterAccount.Geography.GeographyID;

                        forkJoin({
                            waterMeasurementTypes: this.waterMeasurementTypeService.geographiesGeographyIDWaterMeasurementTypesActiveGet(this.geographyID),
                            allocationPlans: this.waterAccountService.waterAccountsWaterAccountIDAllocationPlansGet(this.waterAccountID),
                            selfReport:
                                this.waterMeasurementSelfReportService.geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDGet(
                                    this.geographyID,
                                    this.waterAccountID,
                                    this.selfReportID
                                ),
                            parcels: this.waterAccountParcelService.waterAccountsWaterAccountIDParcelsGet(this.waterAccountID),
                            irrigationMethods: this.irrigationMethodService.geographiesGeographyIDIrrigationMethodsGet(this.geographyID),
                        }).subscribe(({ waterMeasurementTypes, allocationPlans, selfReport, parcels, irrigationMethods }) => {
                            this.waterMeasurementType = waterMeasurementTypes.find((wmt) => wmt.WaterMeasurementTypeID === selfReport.WaterMeasurementType.WaterMeasurementTypeID);
                            this.allocationPlans = allocationPlans;
                            this.selfReport = selfReport;
                            this.parcels = parcels.sort((a, b) => a.ParcelNumber.localeCompare(b.ParcelNumber));
                            this.irrigationMethodOptions = irrigationMethods.map((im) => {
                                return {
                                    Label: im.Name,
                                    Value: im.IrrigationMethodID,
                                    Group: im.SystemType,
                                } as FormInputOption;
                            });

                            //add null option for irrigation method to allow blanking out a selection
                            this.irrigationMethodOptions.unshift({
                                Label: "Select an Irrigation Method",
                                Value: null,
                                Disabled: false,
                                Group: " ", // Empty string to prevent grouping
                            });

                            this.refreshSelfReportData$.next(selfReport);
                            this.pageIsLoading = false;
                        });
                    });
                });

                this.subscriptions.push(this.routeParamsUpdateSubscription);
            })
        );
    }

    ngOnDestroy() {
        this.subscriptions.forEach((sub) => {
            if (sub && sub.unsubscribe) {
                sub.unsubscribe();
            }
        });
    }

    public canExit(): boolean {
        let canExit = true;

        // Check if any line item irrigation controls are dirty
        for (let parcelNumber in this.irrigationMethodControlByAPN) {
            if (this.irrigationMethodControlByAPN[parcelNumber].dirty) {
                canExit = false;
                break;
            }
        }

        // Check if any month form controls are dirty
        this.lineItemViewModels.forEach((viewModel) => {
            for (let month in viewModel.MonthFormControls) {
                if (viewModel.MonthFormControls[month].dirty) {
                    canExit = false;
                    break;
                }
            }
        });

        return canExit;
    }

    setLineItemTotal(viewModel: SelfReportLineItemViewModel) {
        let months = UtilityFunctionsService.months;
        let lineItemMonthValues = months.map((m) => {
            if (viewModel?.MonthFormControls[m]?.value === null) {
                return null;
            }

            let value = Number(viewModel.MonthFormControls[m].value);
            return value;
        });

        let lineItemsWithValues = lineItemMonthValues.filter((value) => value !== null && value !== undefined);
        let total = lineItemsWithValues.reduce((a, b) => a + b, 0);
        viewModel.LineItemTotal = lineItemsWithValues.length > 0 ? total : null;
    }

    setMonthTotal(month: string) {
        let monthValues = this.lineItemViewModels.map((vm) => {
            let monthControl = vm.MonthFormControls[month];
            if (!monthControl?.value === null) {
                return null;
            }

            let value = Number(monthControl.value);
            return value;
        });

        let monthValuesWithValues = monthValues.filter((value) => value !== null && value !== undefined);
        let monthTotal = monthValuesWithValues.reduce((a, b) => a + b, 0);
        this.monthTotals[month] = monthValuesWithValues.length > 0 ? monthTotal : null;
    }

    updateLineItemTotals() {
        let lineItemTotals = this.lineItemViewModels.map((vm) => vm.LineItemTotal).filter((total) => total !== null && total !== undefined);
        this.lineItemTotalsSum = lineItemTotals.length > 0 ? lineItemTotals.reduce((a, b) => a + b) : "-";
    }

    updateCanSave() {
        if (
            this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID !== (WaterMeasurementSelfReportStatusEnum.Draft as number) &&
            this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID !== (WaterMeasurementSelfReportStatusEnum.Returned as number)
        ) {
            let isSystemAdminOrGeographyManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(this.currentUser, this.waterAccount.Geography.GeographyID);
            if (!isSystemAdminOrGeographyManager) {
                this.canSave = false;
                return;
            }
        }

        if (this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === (WaterMeasurementSelfReportStatusEnum.Approved as number)) {
            this.lineItemViewModels.forEach((viewModel) => {
                //Disable irrigation method dropdowns.
                this.irrigationMethodControlByAPN[viewModel.Parcel.ParcelNumber].disable({ onlySelf: true, emitEvent: false });

                //Disable all month form controls
                for (let month in viewModel.MonthFormControls) {
                    viewModel.MonthFormControls[month].disable({ onlySelf: true, emitEvent: false });
                }
            });

            this.canSave = false;
            return;
        }

        let isValid = true; // Assume form is valid initially

        this.lineItemViewModels.forEach((viewModel) => {
            const hasIrrigationMethod = !!viewModel.LineItem.IrrigationMethodID;
            const hasOverrideValue = Object.values(viewModel.MonthFormControls).some(
                (control) => control.value !== null && control.value !== undefined && control.value !== "" && control.value >= 0
            );

            // Skip validation if the line item is blank
            if (!hasIrrigationMethod && !hasOverrideValue) {
                viewModel.IsValid = true;
                for (let month in viewModel.MonthFormControls) {
                    viewModel.MonthFormControls[month].disable({ onlySelf: true, emitEvent: false });
                }

                return;
            }

            if (!hasIrrigationMethod) {
                // Disable all month form controls
                for (let month in viewModel.MonthFormControls) {
                    viewModel.MonthFormControls[month].disable({ onlySelf: true, emitEvent: false });
                }
            } else {
                // Enable all month form controls
                for (let month in viewModel.MonthFormControls) {
                    viewModel.MonthFormControls[month].enable({ onlySelf: true, emitEvent: false });
                }
            }
        });

        this.canSave = isValid; // Update the save button state

        this.updateCanSubmit();
        this.updateCanApprove();
        this.updateCanReturn();
    }

    save() {
        this.isLoadingSubmit = true;

        let lineItems = this.lineItemViewModels
            .filter((x) => x.LineItem.IrrigationMethodID)
            .map((vm) => {
                let lineItem = new WaterMeasurementSelfReportLineItemUpdateDto({
                    ParcelID: vm.Parcel.ParcelID,
                    IrrigationMethodID: vm.LineItem.IrrigationMethodID,
                    JanuaryOverrideValueInAcreFeet: vm.MonthFormControls["January"].value,
                    FebruaryOverrideValueInAcreFeet: vm.MonthFormControls["February"].value,
                    MarchOverrideValueInAcreFeet: vm.MonthFormControls["March"].value,
                    AprilOverrideValueInAcreFeet: vm.MonthFormControls["April"].value,
                    MayOverrideValueInAcreFeet: vm.MonthFormControls["May"].value,
                    JuneOverrideValueInAcreFeet: vm.MonthFormControls["June"].value,
                    JulyOverrideValueInAcreFeet: vm.MonthFormControls["July"].value,
                    AugustOverrideValueInAcreFeet: vm.MonthFormControls["August"].value,
                    SeptemberOverrideValueInAcreFeet: vm.MonthFormControls["September"].value,
                    OctoberOverrideValueInAcreFeet: vm.MonthFormControls["October"].value,
                    NovemberOverrideValueInAcreFeet: vm.MonthFormControls["November"].value,
                    DecemberOverrideValueInAcreFeet: vm.MonthFormControls["December"].value,
                });

                return lineItem;
            });

        let selfReportUpdateDto = new WaterMeasurementSelfReportUpdateDto({
            LineItems: lineItems,
        });

        let updateRequest = this.waterMeasurementSelfReportService
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDPut(
                this.geographyID,
                this.waterAccountID,
                this.selfReportID,
                selfReportUpdateDto
            )
            .subscribe(
                (result) => {
                    this.isLoadingSubmit = false;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully updated self report.", AlertContext.Success));
                    this.refreshSelfReportData$.next(result);

                    setTimeout(() => {
                        this.alertService.clearAlerts();
                    }, 5 * 1000); // Clear the success alert after 5 seconds
                },
                (error) => {
                    this.isLoadingSubmit = false;
                    this.alertService.pushAlert(new Alert("Failed to update self report.", AlertContext.Danger));
                }
            );

        this.subscriptions.push(updateRequest);
    }

    updateCanSubmit() {
        // Check each line item to see if it has an irrigation method and at least one month with a value
        let lineItemsAreValid = this.lineItemViewModels
            .filter((x) => x.LineItem.IrrigationMethodID)
            .every((x) => {
                return x.LineItemTotal > 0;
            });

        this.canSubmit =
            lineItemsAreValid &&
            this.canExit() &&
            this.lineItemTotalsSum != "-" &&
            (this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Draft ||
                this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Returned); // Draft
    }

    openSubmitModal() {
        this.modalService
            .open(SubmitSelfReportModalComponent, null, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true }, {
                SelfReport: this.selfReport,
                GeographyID: this.geographyID,
                WaterAccountID: this.waterAccountID,
            } as SelfReportContext)
            .instance.result.then((result) => {
                if (result) {
                    this.refreshSelfReportData$.next(result);
                }
            });
    }

    updateCanApprove() {
        this.canApprove = this.canSave && this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Submitted; // Submitted
    }

    approve() {
        const approveRequest = this.waterMeasurementSelfReportService
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDApprovePut(
                this.geographyID,
                this.waterAccountID,
                this.selfReportID
            )
            .subscribe((result) => {
                this.alertService.pushAlert(new Alert("Successfully approved self report.", AlertContext.Success));
                setTimeout(() => {
                    this.alertService.clearAlerts();
                }, 1000 * 5); // Clear the success alert after 5 seconds
                this.refreshSelfReportData$.next(result);
            });

        this.subscriptions.push(approveRequest);
    }

    updateCanReturn() {
        this.canReturn = this.canSave && this.selfReport.WaterMeasurementSelfReportStatus.WaterMeasurementSelfReportStatusID === WaterMeasurementSelfReportStatusEnum.Submitted; // Submitted
    }

    return() {
        const returnRequest = this.waterMeasurementSelfReportService
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDReturnPut(
                this.geographyID,
                this.waterAccountID,
                this.selfReportID
            )
            .subscribe((result) => {
                this.alertService.pushAlert(new Alert("Successfully returned self report.", AlertContext.Success));
                setTimeout(() => {
                    this.alertService.clearAlerts();
                }, 1000 * 5); // Clear the success alert after 5 seconds
                this.refreshSelfReportData$.next(result);
            });

        this.subscriptions.push(returnRequest);
    }
}

class SelfReportLineItemViewModel {
    Parcel: ParcelSimpleDto;
    LineItem: WaterMeasurementSelfReportLineItemSimpleDto;
    LineItemTotal: number;
    MonthFormControls: { [key: string]: FormControl } = {};
    IsValid: boolean = true;
}
