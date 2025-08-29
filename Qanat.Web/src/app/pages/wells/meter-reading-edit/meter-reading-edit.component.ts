import { AsyncPipe, DecimalPipe, DatePipe, Time, NgClass } from "@angular/common";
import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { catchError, combineLatest, debounceTime, distinctUntilChanged, filter, interval, Observable, of, shareReplay, Subscription, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { MeterReadingDto, MeterReadingUpsertDto, MeterReadingUpsertDtoForm, MeterReadingUpsertDtoFormControls, WellMeterDto } from "src/app/shared/generated/model/models";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { routeParams } from "src/app/app.routes";
import { MeterByWellService } from "src/app/shared/generated/api/meter-by-well.service";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { MeterReadingUnitTypeEnum, MeterReadingUnitTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/meter-reading-unit-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { MeterReadingByMeterService } from "src/app/shared/generated/api/meter-reading-by-meter.service";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { debounce } from "vega";

@Component({
    selector: "meter-reading-edit",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        LoadingDirective,
        FormFieldComponent,
        AlertDisplayComponent,
        FormsModule,
        ReactiveFormsModule,
        CustomRichTextComponent,
        NoteComponent,
        DecimalPipe,
        ButtonLoadingDirective,
        NgClass,
    ],
    templateUrl: "./meter-reading-edit.component.html",
    styleUrl: "./meter-reading-edit.component.scss",
    encapsulation: ViewEncapsulation.None
})
export class MeterReadingEditComponent implements OnInit, OnDestroy, IDeactivateComponent {
    public wellMeter$: Observable<WellMeterDto>;
    public geographyID: number;
    public wellID: number;
    public meterID: number;

    public meterReading$: Observable<MeterReadingDto>;
    public meterReadingID: number;

    public previousMeterReading$: Observable<MeterReadingDto>;
    public previousMeterDateTime: string;

    public meterReadingSubscription: Subscription = Subscription.EMPTY;
    public unitValueChangesSubscription = Subscription.EMPTY;
    public unitSelectOptions = MeterReadingUnitTypesAsSelectDropdownOptions;

    public showAcreFeetConversion: boolean = false;
    public volume: number;
    public volumeInAcreFeet: number;
    public readonly GallonsToAcreFeetConversionFactor = 325851;

    public customRichTextTypeID: number = CustomRichTextTypeEnum.EditMeterReading;
    public formCustomRichTextTypeID = CustomRichTextTypeEnum.EditMeterReadingForm;

    public pageHeaderVerb: "Add" | "Edit" = "Add";
    public isLoading: boolean = true;
    public isLoadingPreviousReading: boolean = false;
    public isLoadingSubmit: boolean = false;
    public FormFieldType = FormFieldType;

    private subscriptions: Subscription[] = [];

    public formGroup: FormGroup<MeterReadingUpsertDtoForm> = new FormGroup<MeterReadingUpsertDtoForm>({
        MeterReadingUnitTypeID: MeterReadingUpsertDtoFormControls.MeterReadingUnitTypeID(),
        ReadingDate: MeterReadingUpsertDtoFormControls.ReadingDate(),
        ReadingTime: MeterReadingUpsertDtoFormControls.ReadingTime(),
        ReaderInitials: MeterReadingUpsertDtoFormControls.ReaderInitials(),
        PreviousReading: MeterReadingUpsertDtoFormControls.PreviousReading(),
        CurrentReading: MeterReadingUpsertDtoFormControls.CurrentReading(),
        Comment: MeterReadingUpsertDtoFormControls.Comment(),
    });

    public initialFormData: MeterReadingUpsertDto;

    public constructor(
        private route: ActivatedRoute,
        private router: Router,
        private meterByWellService: MeterByWellService,
        private meterReadingByMeterService: MeterReadingByMeterService,
        private utilityFunctionService: UtilityFunctionsService,
        private alertService: AlertService,
        private datePipe: DatePipe
    ) {}

    ngOnInit(): void {
        this.wellMeter$ = this.route.params.pipe(
            switchMap((params) => {
                const wellID = parseInt(params[routeParams.wellID]);
                if (!wellID) return [];

                return this.meterByWellService.getWellMeterByWellIDMeterByWell(wellID);
            }),
            tap((wellMeter) => {
                this.geographyID = wellMeter.GeographyID;
                this.wellID = wellMeter.WellID;
                this.meterID = wellMeter.MeterID;
            })
        );

        this.meterReading$ = combineLatest([this.wellMeter$, this.route.params]).pipe(
            filter(([wellMeter, params]) => {
                return !!wellMeter;
            }),
            switchMap(([wellMeter, params]) => {
                const meterReadingID = parseInt(params[routeParams.meterReadingID]);
                if (!meterReadingID) {
                    this.formGroup.controls.MeterReadingUnitTypeID.setValue(MeterReadingUnitTypeEnum.AcreFeet);
                    return of(new MeterReadingDto());
                }

                return this.meterReadingByMeterService.getMeterReadingByIDMeterReadingByMeter(wellMeter.GeographyID, wellMeter.WellID, wellMeter.MeterID, meterReadingID);
            }),
            tap((meterReading) => {
                if (meterReading && meterReading.MeterReadingID) {
                    this.meterReadingID = meterReading.MeterReadingID;
                    this.pageHeaderVerb = "Edit";
                    this.formGroup.controls.MeterReadingUnitTypeID.setValue(meterReading?.MeterReadingUnitType.MeterReadingUnitTypeID);
                    this.formGroup.controls.ReadingDate.setValue(this.datePipe.transform(meterReading.ReadingDate, "yyyy-MM-dd", "UTC"));
                    this.formGroup.controls.ReadingTime.setValue(this.datePipe.transform(meterReading.ReadingDate, "HH:mm", "UTC"));
                    this.formGroup.controls.ReaderInitials.setValue(meterReading.ReaderInitials);
                    this.formGroup.controls.PreviousReading.setValue(meterReading.PreviousReading);
                    this.formGroup.controls.CurrentReading.setValue(meterReading.CurrentReading);
                    this.formGroup.controls.Comment.setValue(meterReading.Comment);

                    if (meterReading.MeterReadingUnitType.MeterReadingUnitTypeID == MeterReadingUnitTypeEnum.Gallons) {
                        this.showAcreFeetConversion = true;
                        this.volumeInAcreFeet = meterReading.VolumeInAcreFeet;
                    }
                }
                this.initialFormData = this.formGroup.getRawValue();
                this.isLoading = false;
            })
        );

        this.formGroup.controls.ReadingDate.valueChanges.subscribe((value) => {});

        this.previousMeterReading$ = this.formGroup.controls.ReadingDate.valueChanges.pipe(
            debounceTime(400),
            distinctUntilChanged(),
            filter((readingDate) => {
                let date = new Date(readingDate);
                return !!readingDate && date >= this.utilityFunctionService.minDate() && date <= this.utilityFunctionService.maxDate();
            }),
            tap(() => {
                this.isLoadingPreviousReading = true;
            }),
            switchMap((readingDate) => {
                let date = new Date(readingDate);
                let dateFormatted = this.datePipe.transform(date, "yyyy-MM-dd");
                return this.meterReadingByMeterService.getLastReadingFromDateMeterReadingByMeter(this.geographyID, this.wellID, this.meterID, dateFormatted).pipe(
                    catchError(() => {
                        this.isLoadingPreviousReading = false;
                        return of(null);
                    })
                );
            }),
            catchError(() => {
                this.isLoadingPreviousReading = false;
                return of(null);
            }),
            filter((previousReading) => {
                this.isLoadingPreviousReading = false;
                return !!previousReading && previousReading.MeterReadingID != this.meterReadingID;
            }),
            tap((previousReading) => {
                let date = new Date(previousReading.ReadingDate);
                this.previousMeterDateTime = this.datePipe.transform(date, "MM/dd/yyyy hh:mm a");
            }),
            shareReplay(1)
        );

        let meterReadingUnitTypeSubscription = this.formGroup.controls.MeterReadingUnitTypeID.valueChanges.subscribe(() => this.getAcreFeetConversion());
        this.subscriptions.push(meterReadingUnitTypeSubscription);

        let previousReadingSubscription = this.formGroup.controls.PreviousReading.valueChanges.subscribe(() => this.getAcreFeetConversion());
        this.subscriptions.push(previousReadingSubscription);

        let currentReadingSubscription = this.formGroup.controls.CurrentReading.valueChanges.subscribe(() => this.getAcreFeetConversion());
        this.subscriptions.push(currentReadingSubscription);
    }

    ngOnDestroy(): void {
        this.meterReadingSubscription.unsubscribe();
        this.unitValueChangesSubscription.unsubscribe();

        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
    }

    public canExit(): boolean {
        let currentFormData = this.formGroup.getRawValue();
        let canExit = this.utilityFunctionService.deepEqual(this.initialFormData, currentFormData);
        return canExit;
    }
    private getAcreFeetConversion() {
        const meterReadingTypeID = this.formGroup.controls.MeterReadingUnitTypeID.value;
        this.volume = this.formGroup.controls.CurrentReading.value - this.formGroup.controls.PreviousReading.value;

        if (meterReadingTypeID == MeterReadingUnitTypeEnum.Gallons) {
            this.volumeInAcreFeet = this.calculateAcreFeetFromGallons();
            this.showAcreFeetConversion = true;
        } else {
            this.showAcreFeetConversion = false;
            this.volumeInAcreFeet = null;
        }
    }

    private calculateAcreFeetFromGallons(): number {
        return this.volume / this.GallonsToAcreFeetConversionFactor;
    }

    save() {
        this.isLoadingSubmit = true;

        const request = this.meterReadingID
            ? this.meterReadingByMeterService.updateWellMeterReadingMeterReadingByMeter(
                  this.geographyID,
                  this.wellID,
                  this.meterID,
                  this.meterReadingID,
                  this.formGroup.getRawValue()
              )
            : this.meterReadingByMeterService.createMeterReadingMeterReadingByMeter(this.geographyID, this.wellID, this.meterID, this.formGroup.getRawValue());

        request.subscribe({
            next: () => {
                this.initialFormData = this.formGroup.getRawValue();
                this.router.navigate(["/wells", this.wellID]).then(() => {
                    this.alertService.pushAlert(new Alert("Meter reading successfully saved.", AlertContext.Success));
                });
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
