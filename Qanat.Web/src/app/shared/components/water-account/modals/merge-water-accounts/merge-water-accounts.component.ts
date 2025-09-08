import { Component, inject, OnDestroy, OnInit } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WaterAccountContext } from "../update-water-account-info/update-water-account-info.component";
import { BehaviorSubject, Observable, Subscription, of } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { share, switchMap, tap } from "rxjs/operators";
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MergeWaterAccountsDto } from "src/app/shared/generated/model/merge-water-accounts-dto";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { BtnGroupRadioInputComponent } from "../../../inputs/btn-group-radio-input/btn-group-radio-input.component";
import { NoteComponent } from "../../../note/note.component";
import { AsyncPipe } from "@angular/common";
import { IconComponent } from "../../../icon/icon.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "merge-water-accounts",
    templateUrl: "./merge-water-accounts.component.html",
    styleUrls: ["./merge-water-accounts.component.scss"],
    imports: [FormsModule, ReactiveFormsModule, NoteComponent, BtnGroupRadioInputComponent, FormFieldComponent, AsyncPipe, IconComponent],
})
export class MergeWaterAccountsComponent implements OnInit, OnDestroy {
    public ref: DialogRef<WaterAccountContext, WaterAccountDto> = inject(DialogRef);
    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;
    public FormFieldType = FormFieldType;

    public geographyID: number;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.ModalMergeWaterAccounts;

    public formGroup: FormGroup<{
        primaryWaterAccountID: FormControl<number>;
        secondaryWaterAccountID: FormControl<number>;
        deleteMerge: FormControl<boolean>;
        reportingPeriodID: FormControl<number>;
    }> = new FormGroup({
        primaryWaterAccountID: new FormControl<number>(null, [Validators.required]),
        secondaryWaterAccountID: new FormControl<number>(null, [Validators.required]),
        deleteMerge: new FormControl<boolean>({ value: true, disabled: false }, [Validators.required]),
        reportingPeriodID: new FormControl<number>({ value: null, disabled: false }, []),
    });

    public refreshPrimaryWaterAccount$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public primaryWaterAccount$: Observable<WaterAccountDto>;
    public secondaryWaterAccount$: Observable<WaterAccountDto>;
    public primaryWaterAccount: WaterAccountDto;
    public secondaryWaterAccount: WaterAccountDto;

    public formChangesSubscription$: Subscription = Subscription.EMPTY;
    public isLoadingSubmit: boolean = false;
    public excludedWaterAccountIDs: number[] = [];

    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;

    constructor(
        private alertService: AlertService,
        private reportingPeriodService: ReportingPeriodService,
        private waterAccountService: WaterAccountService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.primaryWaterAccountID.setValue(this.ref.data.WaterAccountID);
        this.geographyID = this.ref.data.GeographyID;

        this.primaryWaterAccount$ = this.refreshPrimaryWaterAccount$.pipe(
            switchMap((primaryWaterAccountID) => {
                if (!primaryWaterAccountID) return of(new WaterAccountDto());
                return this.waterAccountService.getByIDWaterAccount(primaryWaterAccountID);
            }),
            tap((primaryWaterAccount) => {
                this.primaryWaterAccount = primaryWaterAccount;
            })
        );
        this.formGroup.controls.primaryWaterAccountID.valueChanges.subscribe((value) => this.refreshPrimaryWaterAccount$.next(value));
        this.refreshPrimaryWaterAccount$.next(this.ref.data.WaterAccountID);

        this.secondaryWaterAccount$ = this.formGroup.controls.secondaryWaterAccountID.valueChanges.pipe(
            switchMap((secondaryWaterAccountID) => {
                if (!secondaryWaterAccountID) return of(new WaterAccountDto());
                return this.waterAccountService.getByIDWaterAccount(secondaryWaterAccountID);
            }),
            tap((secondaryWaterAccount) => {
                this.secondaryWaterAccount = secondaryWaterAccount;
            }),
            share()
        );

        this.reportingPeriodSelectOptions$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(this.geographyID).pipe(
            switchMap((reportingPeriods) => {
                let options = reportingPeriods.map((x) => (({
                    Value: x.ReportingPeriodID,
                    Label: x.Name
                }) as SelectDropdownOption));
                return of(options);
            })
        );

        this.formChangesSubscription$ = this.formGroup.valueChanges.subscribe((x) => {
            // exclude any values selected from the other search results
            this.excludedWaterAccountIDs = [];
            if (x.primaryWaterAccountID != null) this.excludedWaterAccountIDs = [...this.excludedWaterAccountIDs, x.primaryWaterAccountID];
            if (x.secondaryWaterAccountID != null) this.excludedWaterAccountIDs = [...this.excludedWaterAccountIDs, x.secondaryWaterAccountID];
        });
    }

    ngOnDestroy(): void {
        this.formChangesSubscription$.unsubscribe();
    }

    swapAccounts() {
        const oldPrimary = this.formGroup.controls.primaryWaterAccountID.value;
        const oldSecondary = this.formGroup.controls.secondaryWaterAccountID.value;

        this.formGroup.controls.primaryWaterAccountID.setValue(oldSecondary);
        this.formGroup.controls.secondaryWaterAccountID.setValue(oldPrimary);
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.isLoadingSubmit = true;
        this.waterAccountService
            .mergeWaterAccount(
                this.formGroup.controls.primaryWaterAccountID.value,
                this.formGroup.controls.secondaryWaterAccountID.value,
                new MergeWaterAccountsDto({ ReportingPeriodID: this.formGroup.controls.reportingPeriodID.value, IsDeleteMerge: this.formGroup.controls.deleteMerge.value })
            )
            .subscribe({
                next: (updatedPrimaryAccount) => {
                    this.isLoadingSubmit = false;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully merged water accounts.", AlertContext.Success));
                    this.ref.close(updatedPrimaryAccount);
                },
                error: (error) => {
                    this.isLoadingSubmit = false;
                    this.ref.close(null);
                },
            });
    }
}
