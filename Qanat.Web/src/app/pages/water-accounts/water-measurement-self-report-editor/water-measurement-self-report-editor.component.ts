import { AsyncPipe, DecimalPipe, NgClass, DatePipe } from "@angular/common";
import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, Observable, Subscription, combineLatest, of, share, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { IrrigationMethodService } from "src/app/shared/generated/api/irrigation-method.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterMeasurementSelfReportService } from "src/app/shared/generated/api/water-measurement-self-report.service";
import {
    FileResourceSimpleDto,
    ParcelSimpleDto,
    UserDto,
    WaterAccountDto,
    WaterMeasurementSelfReportDto,
    WaterMeasurementSelfReportFileResourceDto,
    WaterMeasurementSelfReportFileResourceUpdateDto,
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
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { SubmitSelfReportModalComponent } from "./submit-self-report-modal/submit-self-report-modal.component";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { WaterMeasurementSelfReportFileResourceService } from "src/app/shared/generated/api/water-measurement-self-report-file-resource.service";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import saveAs from "file-saver";
import { FileResourceListComponent } from "src/app/shared/components/file-resource-list/file-resource-list.component";
import { IFileResourceUpload } from "src/app/shared/components/file-resource-list/file-upload-modal/file-upload-modal.component";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "water-measurement-self-report-editor",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        LoadingDirective,
        DecimalPipe,
        SumPipe,
        FormFieldComponent,
        NgClass,
        AlertDisplayComponent,
        ButtonLoadingDirective,
        DatePipe,
        FileResourceListComponent,
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

    private waterAccountID: number;
    private selfReportID: number;
    public geographyID: number;
    public geographyName: string;

    public waterAccount: WaterAccountDto;
    public waterAccount$: Observable<WaterAccountDto>;

    public isPageLoading: boolean = true;
    public isLoadingSubmit: boolean = false;

    public waterMeasurementType$: Observable<WaterMeasurementTypeSimpleDto>;

    public selfReport: WaterMeasurementSelfReportDto;
    public selfReport$: Observable<WaterMeasurementSelfReportDto>;
    public refreshSelfReportData$ = new BehaviorSubject<null>(null);

    public parcels: ParcelSimpleDto[];
    public lineItemViewModels: SelfReportLineItemViewModel[] = [];
    public lineItemViewModels$: Observable<SelfReportLineItemViewModel[]>;
    public irrigationMethodOptions$: Observable<FormInputOption[]>;

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

    public canEditFiles: boolean = true;
    public fileResources$: Observable<WaterMeasurementSelfReportFileResourceDto[]>;
    public refreshFileResources$ = new BehaviorSubject<null>(null);
    public filesLoaded: boolean = false;
    public fileCount: number = 0;
    public filesToUpload: File[] = [];

    public subscriptions: Subscription[] = [];

    public constructor(
        private waterAccountService: WaterAccountService,
        private waterMeasurementSelfReportService: WaterMeasurementSelfReportService,
        private fileResourceService: FileResourceService,
        private waterMeasurementSelfReportFileResourceService: WaterMeasurementSelfReportFileResourceService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private irrigationMethodService: IrrigationMethodService,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
        private router: Router,
        private route: ActivatedRoute,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.route.params.pipe(
            switchMap((params) => {
                let waterAccountID = parseInt(params[routeParams.waterAccountID]);
                return this.waterAccountService.getByIDWaterAccount(waterAccountID);
            }),
            tap((waterAccount) => {
                this.waterAccount = waterAccount;
                this.waterAccountID = waterAccount.WaterAccountID;
                this.geographyID = waterAccount.Geography.GeographyID;
                this.geographyName = waterAccount.Geography.GeographyName;
            })
        );

        this.selfReport$ = combineLatest({ params: this.route.params, waterAccount: this.waterAccount$, _: this.refreshSelfReportData$ }).pipe(
            switchMap(({ params, waterAccount }) => {
                let selfReportID = parseInt(params[routeParams.selfReportID]);
                return this.waterMeasurementSelfReportService.readSingleWaterMeasurementSelfReport(waterAccount.Geography.GeographyID, waterAccount.WaterAccountID, selfReportID);
            }),
            tap((selfReport) => {
                this.selfReport = selfReport;
                this.selfReportID = selfReport.WaterMeasurementSelfReportID;
            })
        );

        this.waterMeasurementType$ = this.selfReport$.pipe(
            switchMap((selfReport) => {
                return of(selfReport.WaterMeasurementType);
            })
        );

        this.lineItemViewModels$ = combineLatest({ waterAccount: this.waterAccount$, selfReport: this.selfReport$ }).pipe(
            tap(() => {
                this.isPageLoading = true;
            }),
            switchMap(({ waterAccount, selfReport }) => {
                return this.waterAccountParcelByWaterAccountService.getWaterAccountParcelsWaterAccountParcelByWaterAccount(waterAccount.WaterAccountID).pipe(
                    switchMap((parcels) => {
                        return of({ parcels: parcels, selfReport: selfReport });
                    })
                );
            }),
            switchMap(({ parcels, selfReport }) => {
                let months = UtilityFunctionsService.months;
                let viewModels: SelfReportLineItemViewModel[] = [];

                parcels.forEach((parcel) => {
                    let viewModel = this.processParcel(selfReport, parcel, months);
                    viewModels.push(viewModel);
                });

                this.parcels = parcels;
                return of(viewModels);
            }),
            tap((viewModels) => {
                this.lineItemViewModels = viewModels;

                let months = UtilityFunctionsService.months;

                months.forEach((month) => {
                    this.setMonthTotal(month);
                });

                this.updateLineItemTotals();
                this.updateCanSave();
                this.isPageLoading = false;
            })
        );

        this.irrigationMethodOptions$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.irrigationMethodService.readListIrrigationMethod(waterAccount.Geography.GeographyID);
            }),
            switchMap((irrigationMethods) => {
                let options = irrigationMethods.map((irrigationMethod) => {
                    return {
                        Value: irrigationMethod.IrrigationMethodID,
                        Label: irrigationMethod.Name,
                        Group: irrigationMethod.SystemType,
                    } as FormInputOption;
                });

                //Add a blank option so that the user can clear the irrigation method for a parcel.
                options.unshift({ Value: null, Label: "No Irrigation Method", Group: null } as FormInputOption);

                return of(options);
            }),
            share()
        );

        this.fileResources$ = combineLatest({ selfReport: this.selfReport$, _: this.refreshFileResources$ }).pipe(
            switchMap(({ selfReport }) => {
                return this.waterMeasurementSelfReportFileResourceService.listWaterMeasurementSelfReportFileResource(
                    selfReport.Geography.GeographyID,
                    selfReport.WaterAccount.WaterAccountID,
                    selfReport.WaterMeasurementSelfReportID
                );
            }),
            tap((fileResources) => {
                this.fileCount = fileResources.length;
                this.filesLoaded = true;
            })
        );

        this.currentUser$ = combineLatest({ currentUser: this.authenticationService.getCurrentUser(), selfReport: this.selfReport$ }).pipe(
            tap(({ currentUser, selfReport }) => {
                this.currentUser = currentUser;
                this.showSubmitButton =
                    selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Draft ||
                    selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Returned;

                const isSystemAdminOrGeographyManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(this.currentUser, this.geographyID);
                this.showApproveButton = selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Submitted && isSystemAdminOrGeographyManager;

                this.showReturnButton = selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Submitted && isSystemAdminOrGeographyManager;

                if (isSystemAdminOrGeographyManager) {
                    this.canEditFiles = selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID != SelfReportStatusEnum.Approved;
                } else {
                    this.canEditFiles =
                        selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID != SelfReportStatusEnum.Approved &&
                        selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID != SelfReportStatusEnum.Submitted;
                }

                this.updateCanSave();
            }),
            switchMap(({ currentUser }) => {
                return of(currentUser);
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

    processParcel(selfReport: WaterMeasurementSelfReportDto, parcel: ParcelSimpleDto, months: string[]): SelfReportLineItemViewModel {
        let viewModel = new SelfReportLineItemViewModel();

        //Find existing line item if exists, otherwise create a new one
        let lineItem = selfReport.LineItems.find((li) => li.ParcelID === parcel.ParcelID) ?? new WaterMeasurementSelfReportLineItemSimpleDto();
        months.forEach((month) => {
            let monthControl = new FormControl(lineItem[month + "OverrideValueInAcreFeet"], { validators: [Validators.min(0)] });

            monthControl.valueChanges.subscribe((value) => {
                lineItem[month + "OverrideValueInAcreFeet"] = value;
                this.setLineItemTotal(viewModel);
                this.setMonthTotal(month);
                this.updateLineItemTotals();
                this.updateCanSave();
            });

            viewModel.MonthFormControls[month] = monthControl;

            if (!lineItem.IrrigationMethodID) {
                monthControl.disable({ onlySelf: true, emitEvent: false });
            }
        });

        viewModel.Parcel = parcel;
        viewModel.LineItem = lineItem;
        this.setLineItemTotal(viewModel);

        this.irrigationMethodControlByAPN[parcel.ParcelNumber] = new FormControl(lineItem?.IrrigationMethodID);
        this.irrigationMethodControlByAPN[parcel.ParcelNumber].valueChanges.subscribe((value) => {
            let irrigationMethodID = lineItem.IrrigationMethodID;
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

        return viewModel;
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
            this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID !== (SelfReportStatusEnum.Draft as number) &&
            this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID !== (SelfReportStatusEnum.Returned as number)
        ) {
            let isSystemAdminOrGeographyManager = AuthorizationHelper.isSystemAdministratorOrGeographyManager(this.currentUser, this.waterAccount.Geography.GeographyID);
            if (!isSystemAdminOrGeographyManager) {
                this.canSave = false;
                return;
            }
        }

        if (this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === (SelfReportStatusEnum.Approved as number)) {
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
            .updateWaterMeasurementSelfReport(this.geographyID, this.waterAccountID, this.selfReportID, selfReportUpdateDto)
            .subscribe(
                (result) => {
                    this.isLoadingSubmit = false;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully updated self report.", AlertContext.Success));
                    this.refreshSelfReportData$.next(null);

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
            (this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Draft ||
                this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Returned); // Draft
    }

    openSubmitModal() {
        const dialogRef = this.dialogService.open(SubmitSelfReportModalComponent, {
            data: {
                SelfReport: this.selfReport,
                GeographyID: this.geographyID,
                WaterAccountID: this.waterAccountID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshSelfReportData$.next(null);
            }
        });
    }

    updateCanApprove() {
        this.canApprove = this.canSave && this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Submitted; // Submitted
    }

    approve() {
        const approveRequest = this.waterMeasurementSelfReportService
            .approveWaterMeasurementSelfReport(this.geographyID, this.waterAccountID, this.selfReportID)
            .subscribe((result) => {
                this.alertService.pushAlert(new Alert("Successfully approved self report.", AlertContext.Success));
                this.router.navigate(["/review-self-reports", this.geographyName.toLowerCase()]);
            });

        this.subscriptions.push(approveRequest);
    }

    updateCanReturn() {
        this.canReturn = this.canSave && this.selfReport.WaterMeasurementSelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Submitted; // Submitted
    }

    return() {
        const returnRequest = this.waterMeasurementSelfReportService
            .returnWaterMeasurementSelfReport(this.geographyID, this.waterAccountID, this.selfReportID)
            .subscribe((result) => {
                this.alertService.pushAlert(new Alert("Successfully returned self report.", AlertContext.Success));
                this.router.navigate(["/review-self-reports", this.geographyName.toLowerCase()]);
            });

        this.subscriptions.push(returnRequest);
    }

    public onFileResourceUploaded(fileResource: IFileResourceUpload) {
        this.waterMeasurementSelfReportFileResourceService
            .createWaterMeasurementSelfReportFileResource(this.geographyID, this.waterAccountID, this.selfReportID, fileResource.File, fileResource.FileDescription)
            .subscribe((result) => {
                this.alertService.pushAlert(new Alert("Successfully uploaded file.", AlertContext.Success));
                this.refreshFileResources$.next(null);
            });
    }

    public downloadFileResource(fileResource: FileResourceSimpleDto) {
        let downloadFileSubscription = this.fileResourceService.downloadFileResourceFileResource(fileResource.FileResourceGUID).subscribe((response) => {
            saveAs(response, `${fileResource.OriginalBaseFilename}.${fileResource.OriginalFileExtension}`);
        });

        this.subscriptions.push(downloadFileSubscription);
    }

    public onFileResourceUpdated(fileResource: any) {
        this.alertService.clearAlerts();

        let fileResourceUpdateDto = new WaterMeasurementSelfReportFileResourceUpdateDto({
            FileDescription: fileResource.FileDescription,
        });

        let updateFileResource = this.waterMeasurementSelfReportFileResourceService
            .updateWaterMeasurementSelfReportFileResource(
                this.geographyID,
                this.waterAccountID,
                this.selfReportID,
                fileResource.WaterMeasurementSelfReportFileResourceID,
                fileResourceUpdateDto
            )
            .subscribe(() => {
                this.alertService.pushAlert(new Alert("Successfully updated file.", AlertContext.Success));
                this.refreshFileResources$.next(null);
            });

        this.subscriptions.push(updateFileResource);
    }

    public deleteFileResource(file: WaterMeasurementSelfReportFileResourceDto) {
        this.alertService.clearAlerts();

        const message = `You are about to delete <b>${file.FileResource.OriginalBaseFilename}</b>. Are you sure you wish to proceed?`;
        this.confirmService.confirm({ title: "Delete File", message: message, buttonTextYes: "Delete", buttonClassYes: "btn-danger", buttonTextNo: "Cancel" }).then((confirmed) => {
            if (!confirmed) {
                return;
            }

            let deleteFileResource = this.waterMeasurementSelfReportFileResourceService
                .deleteWaterMeasurementSelfReportFileResource(this.geographyID, this.waterAccountID, this.selfReportID, file.WaterMeasurementSelfReportFileResourceID)
                .subscribe(() => {
                    this.alertService.pushAlert(new Alert("Successfully deleted file.", AlertContext.Success));
                    this.refreshFileResources$.next(null);
                });

            this.subscriptions.push(deleteFileResource);
        });
    }
}

class SelfReportLineItemViewModel {
    Parcel: ParcelSimpleDto;
    LineItem: WaterMeasurementSelfReportLineItemSimpleDto;
    LineItemTotal: number;
    MonthFormControls: { [key: string]: FormControl } = {};
    IsValid: boolean = true;
}
