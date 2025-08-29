import { ChangeDetectorRef, Component } from "@angular/core";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "../../../../shared/components/page-header/page-header.component";
import { Validators, FormsModule, ReactiveFormsModule, FormGroup, FormControl } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, of, switchMap, tap } from "rxjs";
import { GeographyService } from "../../../../shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "../../../../shared/generated/enum/custom-rich-text-type-enum";
import { GeographyMinimalDto } from "../../../../shared/generated/model/geography-minimal-dto";
import { GeographyPublicDto } from "../../../../shared/generated/model/geography-public-dto";
import { Alert } from "../../../../shared/models/alert";
import { AlertContext } from "../../../../shared/models/enums/alert-context.enum";
import { AlertService } from "../../../../shared/services/alert.service";
import { ConfirmService } from "../../../../shared/services/confirm/confirm.service";
import { CurrentGeographyService } from "../../../../shared/services/current-geography.service";
import { AsyncPipe } from "@angular/common";
import { ButtonComponent } from "../../../../shared/components/button/button.component";
import { UsageLocationByGeographyService } from "../../../../shared/generated/api/usage-location-by-geography.service";
import { GDBColumnsDto } from "../../../../shared/generated/model/gdb-columns-dto";
import { SelectDropdownOption, FormFieldComponent, FormFieldType } from "../../../../shared/components/forms/form-field/form-field.component";
import { ColumnMappingDto } from "../../../../shared/generated/model/column-mapping-dto";
import { ReportingPeriodService } from "../../../../shared/generated/api/reporting-period.service";
import { BtnGroupRadioInputComponent } from "../../../../shared/components/inputs/btn-group-radio-input/btn-group-radio-input.component";
import { NoteComponent } from "../../../../shared/components/note/note.component";
import { IconComponent } from "../../../../shared/components/icon/icon.component";
import { FieldDefinitionComponent } from "../../../../shared/components/field-definition/field-definition.component";

@Component({
    selector: "upload-usage-location-column-review",
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        FormsModule,
        ReactiveFormsModule,
        ButtonComponent,
        AlertDisplayComponent,
        FormFieldComponent,
        BtnGroupRadioInputComponent,
        NoteComponent,
        IconComponent,
        FieldDefinitionComponent,
    ],
    templateUrl: "./upload-usage-location-column-review.component.html",
    styleUrl: "./upload-usage-location-column-review.component.scss",
})
export class UploadUsageLocationColumnReviewComponent {
    public geography$: Observable<GeographyPublicDto>;
    public resultsPreview$: Observable<GDBColumnsDto>;
    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;

    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup({
        ReportingPeriodID: new FormControl<number>(null, [Validators.required]),
        UsageLocationName: new FormControl("", [Validators.required]),
        APN: new FormControl("", [Validators.required]),
        UsageType: new FormControl(""),
        Crop1: new FormControl(""),
        Crop2: new FormControl(""),
        Crop3: new FormControl(""),
        Crop4: new FormControl(""),
        usageLocationOption: new FormControl<boolean>(true),
    });

    public customRichTextType: CustomRichTextTypeEnum = CustomRichTextTypeEnum.UsageLocationUploadReview;
    public expectedResultsRetrievedSuccessfully: boolean = false;
    public isLoadingSubmit: boolean;
    public columnOptions: SelectDropdownOption[];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private cdr: ChangeDetectorRef,
        private usageLocationByGeographyService: UsageLocationByGeographyService,
        private confirmService: ConfirmService,
        private reportingPeriodService: ReportingPeriodService
    ) {}

    ngOnInit() {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );

        this.resultsPreview$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.usageLocationByGeographyService.uploadGDBAndParseFeatureClassesForColumnsUsageLocationByGeography(geography.GeographyID);
            }),
            tap((results) => {
                this.expectedResultsRetrievedSuccessfully = true;
                this.columnOptions = results.FeatureClasses[0].Columns.map(
                    (y) =>
                        ({
                            Value: y,
                            Label: y,
                        }) as SelectDropdownOption
                );
            })
        );

        this.reportingPeriodSelectOptions$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID).pipe(
                    switchMap((reportingPeriods) => {
                        let options = reportingPeriods.map(
                            (x) =>
                                ({
                                    Value: x.ReportingPeriodID,
                                    Label: x.Name,
                                }) as SelectDropdownOption
                        );
                        return of(options);
                    }),
                    tap(() => {
                        this.isLoadingSubmit = false;
                    })
                );
            })
        );
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    public onSubmitChanges(geography: GeographyMinimalDto) {
        this.isLoadingSubmit = true;
        var temp: ColumnMappingDto[] = [];
        temp.push(
            new ColumnMappingDto({
                DestinationColumnName: "UsageLocationName",
                SourceColumnName: this.formGroup.get("UsageLocationName").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "APN",
                SourceColumnName: this.formGroup.get("APN").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "UsageLocationType",
                SourceColumnName: this.formGroup.get("UsageType").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "Crop1",
                SourceColumnName: this.formGroup.get("Crop1").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "Crop2",
                SourceColumnName: this.formGroup.get("Crop2").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "Crop3",
                SourceColumnName: this.formGroup.get("Crop3").value,
            }),
            new ColumnMappingDto({
                DestinationColumnName: "Crop4",
                SourceColumnName: this.formGroup.get("Crop4").value,
            })
        );
        this.usageLocationByGeographyService
            .uploadGDBAndParseFeatureClassesUsageLocationByGeography(
                geography.GeographyID,
                this.formGroup.controls.ReportingPeriodID.value,
                this.formGroup.controls.usageLocationOption.value,
                temp
            )
            .subscribe({
                next: (response) => {
                    this.isLoadingSubmit = false;
                    if (response.Messages.length == 1 && response.Messages[0].Message.includes("Duplicate")) {
                        this.alertService.pushAlert(new Alert(response.Messages[0].Message, AlertContext.Danger));
                    } else {
                        this.router.navigate(["../.."], { relativeTo: this.route }).then(() => {
                            if (response.Messages.length > 0) {
                                for (const message of response.Messages) {
                                    this.alertService.pushAlert(new Alert(message.Message, AlertContext.Warning));
                                }
                            }
                            this.alertService.pushAlert(new Alert(`Successfully updated usage locations.`, AlertContext.Success));
                        });
                    }
                },
                error: (error) => {
                    this.isLoadingSubmit = false;
                    this.alertService.pushAlert(new Alert(error.message, AlertContext.Danger));
                },
            });
    }

    public launchModal(geography: GeographyMinimalDto): void {
        const confirmOptions = {
            title: "Finalize Usage Location Changes",
            message: `Are you sure you want to finalize these changes? This action cannot be undone.`,
            buttonClassYes: "btn btn-secondary",
            buttonTextYes: "Save",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.onSubmitChanges(geography);
            }
        });
    }
}
