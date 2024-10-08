import { Component, ComponentRef, OnDestroy, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { Observable, Subscription, forkJoin, of } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { switchMap, tap } from "rxjs/operators";
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MergeWaterAccountsDto } from "src/app/shared/generated/model/merge-water-accounts-dto";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ReportingPeriodSimpleDto } from "src/app/shared/generated/model/models";
import { SelectDropdownOption } from "../../../inputs/select-dropdown/select-dropdown.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { BtnGroupRadioInputComponent } from "../../../inputs/btn-group-radio-input/btn-group-radio-input.component";
import { FieldDefinitionComponent } from "../../../field-definition/field-definition.component";
import { WaterAccountCardComponent } from "../../water-account-card/water-account-card.component";
import { NoteComponent } from "../../../note/note.component";
import { ParcelIconWithNumberComponent } from "../../../parcel/parcel-icon-with-number/parcel-icon-with-number.component";
import { SearchWaterAccountsComponent } from "../../../search-water-accounts/search-water-accounts.component";
import { NgIf, NgFor, AsyncPipe, DatePipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../custom-rich-text/custom-rich-text.component";

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
    public waterAccountDto$: Observable<WaterAccountDto>;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalMergeWaterAccounts;

    public formGroup: FormGroup = new FormGroup({
        primaryWaterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
        secondaryWaterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
        deleteMerge: new FormControl<boolean>({ value: null, disabled: false }, [Validators.required]),
        effectiveYear: new FormControl<number>({ value: null, disabled: false }, []),
    });

    public previewWaterAccount: WaterAccountDto = null;
    public formChangesSubscription$: Subscription = Subscription.EMPTY;
    public isLoadingSubmit: boolean = false;
    public excludedWaterAccountIDs: number[] = [];

    public reportingPeriod: ReportingPeriodSimpleDto;
    public reportingPeriodYears: number[];

    public effectiveYear: number;
    public effectiveYear$: Observable<Date>;
    public effectiveYearDropdownOptions: SelectDropdownOption[];

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private reportingPeriodService: ReportingPeriodService,
        private waterAccountService: WaterAccountService,
        private parcelService: ParcelService
    ) {}

    ngOnInit(): void {
        this.waterAccountDto$ = this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).pipe(
            tap((x) => {
                this.formGroup.controls.primaryWaterAccount.setValue(x);
                this.geographyID = x.Geography.GeographyID;
            })
        );

        type ReportingPeriodForkJoinResults = [number[], ReportingPeriodSimpleDto];
        forkJoin<ReportingPeriodForkJoinResults>([
            this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(this.modalContext.GeographyID),
            this.reportingPeriodService.geographiesGeographyIDReportingPeriodGet(this.modalContext.GeographyID),
        ]).subscribe(([reportingPeriodYears, reportingPeriod]) => {
            this.reportingPeriod = reportingPeriod;
            this.reportingPeriodYears = reportingPeriodYears.reverse();
        });

        this.effectiveYear$ = this.formGroup.controls.effectiveYear.valueChanges.pipe(
            switchMap((x) => {
                if (!this.reportingPeriod || !this.formGroup.controls.effectiveYear.value) return of(null);

                const date = new Date();
                date.setDate(1);
                date.setMonth(this.reportingPeriod.StartMonth - 1);
                date.setFullYear(this.reportingPeriod.StartMonth == 1 ? this.formGroup.controls.effectiveYear.value : this.formGroup.controls.effectiveYear.value - 1);
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
        this.parcelService.geographiesGeographyIDParcelsLatestEffectiveYearPost(this.geographyID, parcelIDs).subscribe((latestEffectiveYear) => {
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

interface ReportingPeriodsResult {
    years: number[];
    reportingPeriod: ReportingPeriodSimpleDto;
}
