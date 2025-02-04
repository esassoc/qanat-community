import { Component, ComponentRef, OnDestroy, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { Observable, Subscription, combineLatest, forkJoin, of } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { switchMap, tap } from "rxjs/operators";
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MergeWaterAccountsDto } from "src/app/shared/generated/model/merge-water-accounts-dto";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { SelectDropdownOption } from "../../../inputs/select-dropdown/select-dropdown.component";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { BtnGroupRadioInputComponent } from "../../../inputs/btn-group-radio-input/btn-group-radio-input.component";
import { FieldDefinitionComponent } from "../../../field-definition/field-definition.component";
import { WaterAccountCardComponent } from "../../water-account-card/water-account-card.component";
import { NoteComponent } from "../../../note/note.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { SearchWaterAccountsComponent } from "../../../search-water-accounts/search-water-accounts.component";
import { NgIf, NgFor, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";

@Component({
    selector: "merge-water-accounts",
    templateUrl: "./merge-water-accounts.component.html",
    styleUrls: ["./merge-water-accounts.component.scss"],
    standalone: true,
    imports: [
        CustomRichTextComponent,
        FormsModule,
        ReactiveFormsModule,
        NgIf,
        SearchWaterAccountsComponent,
        NgFor,
        ParcelIconWithNumberComponent,
        NoteComponent,
        WaterAccountCardComponent,
        FieldDefinitionComponent,
        BtnGroupRadioInputComponent,
        FormFieldComponent,
        AsyncPipe,
        DatePipe,
    ],
})
export class MergeWaterAccountsComponent implements OnInit, IModal, OnDestroy {
    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;
    public FormFieldType = FormFieldType;

    public geographyID: number;
    public waterAccount$: Observable<WaterAccountDto>;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalMergeWaterAccounts;

    public formGroup: FormGroup<{
        primaryWaterAccount: FormControl<WaterAccountDto>;
        secondaryWaterAccount: FormControl<WaterAccountDto>;
        deleteMerge: FormControl<boolean>;
        effectiveYear: FormControl<number>;
    }> = new FormGroup({
        primaryWaterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
        secondaryWaterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
        deleteMerge: new FormControl<boolean>({ value: null, disabled: false }, [Validators.required]),
        effectiveYear: new FormControl<number>({ value: null, disabled: false }, []),
    });

    public previewWaterAccount: WaterAccountDto = null;
    public formChangesSubscription$: Subscription = Subscription.EMPTY;
    public isLoadingSubmit: boolean = false;
    public excludedWaterAccountIDs: number[] = [];

    public reportingPeriods$: Observable<ReportingPeriodDto[]>;
    public defaultReportingPeriod$: Observable<ReportingPeriodDto>;
    public reportingPeriodYears: number[];

    public effectiveYear: number;
    public effectiveYear$: Observable<Date>;
    public effectiveYearDropdownOptions: SelectDropdownOption[];

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private reportingPeriodService: ReportingPeriodService,
        private waterAccountService: WaterAccountService,
        private parcelByGeographyService: ParcelByGeographyService
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).pipe(
            tap((x) => {
                this.formGroup.controls.primaryWaterAccount.setValue(x);
                this.geographyID = x.Geography.GeographyID;
            })
        );

        this.reportingPeriods$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.reportingPeriodService.geographiesGeographyIDReportingPeriodsGet(waterAccount.Geography.GeographyID);
            }),
            tap((reportingPeriods) => {
                this.reportingPeriodYears = reportingPeriods.map((x) => new Date(x.StartDate).getFullYear()).reverse();
            })
        );

        this.defaultReportingPeriod$ = this.reportingPeriods$.pipe(
            switchMap((reportingPeriods) => {
                return of(reportingPeriods.find((x) => x.IsDefaultReportingPeriod));
            })
        );

        this.effectiveYear$ = combineLatest([this.defaultReportingPeriod$, this.formGroup.controls.effectiveYear.valueChanges]).pipe(
            switchMap(([reportingPeriod, effectiveYear]) => {
                if (!reportingPeriod || !effectiveYear) {
                    return of(null);
                }

                const date = new Date(reportingPeriod.StartDate);
                return of(date);
            })
        );

        this.formGroup.controls.secondaryWaterAccount.valueChanges.subscribe(() => this.updateEffectiveYearDropdownOptions());

        this.formChangesSubscription$ = this.formGroup.valueChanges.subscribe((x) => {
            // exclude any values selected from the other search results
            this.excludedWaterAccountIDs = [];
            if (x.primaryWaterAccount?.WaterAccountID != null) this.excludedWaterAccountIDs = [...this.excludedWaterAccountIDs, x.primaryWaterAccount?.WaterAccountID];
            if (x.secondaryWaterAccount?.WaterAccountID != null) this.excludedWaterAccountIDs = [...this.excludedWaterAccountIDs, x.secondaryWaterAccount?.WaterAccountID];

            // set up the preview model
            if (x.primaryWaterAccount != null && x.secondaryWaterAccount != null) {
                // init the preview
                this.previewWaterAccount = new WaterAccountDto(x.primaryWaterAccount);
                this.previewWaterAccount.Parcels = [...x.primaryWaterAccount.Parcels, ...x.secondaryWaterAccount.Parcels];
            } else {
                // clear the preview model
                this.previewWaterAccount = null;
            }
        });
    }

    updateEffectiveYearDropdownOptions() {
        const parcelIDs = this.formGroup.controls.secondaryWaterAccount.value.Parcels.map((x) => x.ParcelID);
        this.parcelByGeographyService.geographiesGeographyIDParcelsLatestEffectiveYearPost(this.geographyID, parcelIDs).subscribe((latestEffectiveYear) => {
            const years = this.reportingPeriodYears.filter((x) => x >= latestEffectiveYear);

            let options = years.map((x) => ({ Value: x, Label: x.toString() }) as SelectDropdownOption);
            // insert an empty option at the front
            options = [{ Value: null, Label: "Select an Option", Disabled: true }, ...options];

            this.effectiveYearDropdownOptions = options;

            if (this.formGroup.controls.effectiveYear.value && !years.includes(this.formGroup.controls.effectiveYear.value)) {
                this.formGroup.controls.effectiveYear.setValue(null);
            }
        });
    }

    ngOnDestroy(): void {
        this.formChangesSubscription$.unsubscribe();
    }

    swapAccounts() {
        const oldPrimary = { ...this.formGroup.controls.primaryWaterAccount.value };
        const oldSecondary = { ...this.formGroup.controls.secondaryWaterAccount.value };

        this.formGroup.controls.primaryWaterAccount.setValue(oldSecondary);
        this.formGroup.controls.secondaryWaterAccount.setValue(oldPrimary);
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.waterAccountService
            .waterAccountsWaterAccountIDMergeSecondaryWaterAccountIDPut(
                this.formGroup.controls.primaryWaterAccount.value.WaterAccountID,
                this.formGroup.controls.secondaryWaterAccount.value.WaterAccountID,
                new MergeWaterAccountsDto({ PrimaryReportingPeriodYear: this.formGroup.controls.effectiveYear.value, IsDeleteMerge: this.formGroup.controls.deleteMerge.value })
            )
            .subscribe(
                (updatedPrimaryAccount) => {
                    this.isLoadingSubmit = false;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully merged water accounts.", AlertContext.Success));
                    this.modalService.close(this.modalComponentRef, updatedPrimaryAccount);
                },
                (error: any) => {
                    this.isLoadingSubmit = false;
                    this.modalService.close(this.modalComponentRef);
                },
                () => {
                    this.isLoadingSubmit = false;
                    this.modalService.close(this.modalComponentRef);
                }
            );
    }
}
