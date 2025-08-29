import { Component, OnInit } from "@angular/core";
import { StatementTemplateByGeographyService } from "src/app/shared/generated/api/statement-template-by-geography.service";
import saveAs from "file-saver";
import { Observable, combineLatest, filter, of, switchMap, tap } from "rxjs";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { CommonModule } from "@angular/common";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { StatementTemplateUpsertDto, StatementTemplateUpsertDtoForm, StatementTemplateUpsertDtoFormControls } from "src/app/shared/generated/model/statement-template-upsert-dto";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";

import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { StatementTemplateDto } from "src/app/shared/generated/model/statement-template-dto";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { StatementTemplateTypeEnum, StatementTemplateTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/statement-template-type-enum";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { BasicRichTextEditorModalComponent } from "src/app/shared/components/basic-rich-text-editor-modal/basic-rich-text-editor-modal.component";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { UsageStatementTemplatePreviewComponent } from "../../shared/components/usage-statement-template-preview/usage-statement-template-preview.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { QanatMapInitEvent } from "../../shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import {
    StatementTemplatePdfPreviewRequestDtoForm,
    StatementTemplatePdfPreviewRequestDtoFormControls,
} from "src/app/shared/generated/model/statement-template-pdf-preview-request-dto";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "statement-template-edit",
    imports: [
        CommonModule,
        PageHeaderComponent,
        RouterLink,
        FormFieldComponent,
        FormsModule,
        ReactiveFormsModule,
        LoadingDirective,
        IconComponent,
        AlertDisplayComponent,
        NoteComponent,
        UsageStatementTemplatePreviewComponent,
        ButtonLoadingDirective,
    ],
    templateUrl: "./statement-template-edit.component.html",
    styleUrl: "./statement-template-edit.component.scss"
})
export class StatementTemplateEditComponent implements OnInit {
    public currentGeography$: Observable<GeographyDto>;
    public geographyID: number;
    public geographySlug: string;

    public statementTemplate$: Observable<StatementTemplateDto>;
    public statementTemplateID: number;
    public initialFormData: StatementTemplateUpsertDto;

    public customFieldValues: { [FieldName: string]: string } = {};

    public statementTemplateTypeSelectOptions = StatementTemplateTypesAsSelectDropdownOptions;
    public reportingPeriodSelectOptions$: Observable<SelectDropdownOption[]>;
    public waterAccountParcelIDs$: Observable<number[]>;

    public pageHeaderVerb: "Add" | "Edit" = "Add";
    public isLoading: boolean = true;
    public isLoadingPdfPreview: boolean = false;
    public isLoadingSubmit: boolean = false;
    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<StatementTemplateUpsertDtoForm> = new FormGroup<StatementTemplateUpsertDtoForm>({
        GeographyID: StatementTemplateUpsertDtoFormControls.GeographyID(),
        StatementTemplateTypeID: StatementTemplateUpsertDtoFormControls.StatementTemplateTypeID(),
        TemplateTitle: StatementTemplateUpsertDtoFormControls.TemplateTitle(),
        InternalDescription: StatementTemplateUpsertDtoFormControls.InternalDescription(),
        CustomFieldsContent: StatementTemplateUpsertDtoFormControls.CustomFieldsContent(),
        CustomLabels: StatementTemplateUpsertDtoFormControls.CustomLabels(),
    });

    // this is hard-coded for now, but can be made dynamic once we introduce more template types
    public balanceCustomLabelFormControl = new FormControl<string>("Balance");

    public pdfPreviewFormGroup: FormGroup<StatementTemplatePdfPreviewRequestDtoForm> = new FormGroup<StatementTemplatePdfPreviewRequestDtoForm>({
        StatementTemplateTypeID: StatementTemplatePdfPreviewRequestDtoFormControls.StatementTemplateTypeID(),
        StatementTemplateTitle: StatementTemplatePdfPreviewRequestDtoFormControls.StatementTemplateTitle(),
        WaterAccountID: StatementTemplatePdfPreviewRequestDtoFormControls.WaterAccountID(),
        ReportingPeriodYear: StatementTemplatePdfPreviewRequestDtoFormControls.ReportingPeriodYear(),
        CustomFields: StatementTemplatePdfPreviewRequestDtoFormControls.CustomFields(),
        CustomLabels: StatementTemplatePdfPreviewRequestDtoFormControls.CustomLabels(),
    });

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private statementTemplateByGeographyService: StatementTemplateByGeographyService,
        private currentGeographyService: CurrentGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.geographySlug = geography.GeographyName.toLowerCase();
                this.formGroup.controls.GeographyID.setValue(geography.GeographyID);

                this.reportingPeriodSelectOptions$ = this.reportingPeriodService.listByGeographyIDReportingPeriod(geography.GeographyID).pipe(
                    switchMap((reportingPeriods) => {
                        return of(
                            reportingPeriods.map((reportingPeriod) => {
                                return {
                                    Value: new Date(reportingPeriod.EndDate).getUTCFullYear(),
                                    Label: reportingPeriod.Name,
                                } as SelectDropdownOption;
                            })
                        );
                    })
                );
            })
        );

        this.statementTemplate$ = combineLatest([this.currentGeography$, this.route.params]).pipe(
            filter(([geography, params]) => {
                return !!geography;
            }),
            switchMap(([geography, params]) => {
                const statementTemplateID = parseInt(params[routeParams.statementTemplateID]);
                if (!statementTemplateID) {
                    this.formGroup.controls.StatementTemplateTypeID.setValue(StatementTemplateTypeEnum.UsageStatement);
                    return of(new StatementTemplateDto());
                }

                return this.statementTemplateByGeographyService.getStatementTemplateByIDStatementTemplateByGeography(geography.GeographyID, statementTemplateID);
            }),
            tap((statementTemplate) => {
                if (statementTemplate && statementTemplate.StatementTemplateID) {
                    this.statementTemplateID = statementTemplate.StatementTemplateID;
                    this.formGroup.controls.StatementTemplateTypeID.setValue(statementTemplate.StatementTemplateType.StatementTemplateTypeID);
                    this.formGroup.controls.TemplateTitle.setValue(statementTemplate.TemplateTitle);
                    this.formGroup.controls.InternalDescription.setValue(statementTemplate.InternalDescription);
                    this.formGroup.controls.CustomFieldsContent.setValue(statementTemplate.CustomFieldsContent);
                    this.formGroup.controls.CustomLabels.setValue(statementTemplate.CustomLabels);

                    this.customFieldValues = statementTemplate.CustomFieldsContent;
                    this.balanceCustomLabelFormControl.setValue(statementTemplate.CustomLabels["Balance"]);

                    this.pageHeaderVerb = "Edit";
                }
                this.initialFormData = this.formGroup.getRawValue();
                this.isLoading = false;
            })
        );

        // needs to update in real time so form group validation can determine whether Preview button should be enabled
        this.formGroup.controls.StatementTemplateTypeID.valueChanges.subscribe((value) => this.pdfPreviewFormGroup.controls.StatementTemplateTypeID.patchValue(value));
    }

    public onMapLayerLoaded() {
        this.isLoadingPdfPreview = false;
    }

    public canExit(): boolean {
        if (!this.initialFormData) return true;

        let currentFormData = this.formGroup.getRawValue();
        let canExit = this.utilityFunctionsService.deepEqual(this.initialFormData, currentFormData);
        return canExit;
    }

    public openEditModal(customField: string) {
        const dialogRef = this.dialogService.open(BasicRichTextEditorModalComponent, {
            data: {
                Header: customField,
                HtmlContent: this.customFieldValues[customField],
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.customFieldValues[customField] = result;
                this.formGroup.controls.CustomFieldsContent.patchValue(this.customFieldValues);
            }
        });
    }

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public async downloadStatementTemplatePDF() {
        this.isLoadingPdfPreview = true;

        this.pdfPreviewFormGroup.controls.StatementTemplateTitle.patchValue(this.formGroup.controls.TemplateTitle.getRawValue());
        this.pdfPreviewFormGroup.controls.CustomFields.patchValue(this.formGroup.controls.CustomFieldsContent.getRawValue());
        this.pdfPreviewFormGroup.controls.CustomLabels.patchValue({ Balance: this.balanceCustomLabelFormControl.getRawValue() });

        this.statementTemplateByGeographyService.generateStatementTemplatePdfStatementTemplateByGeography(this.geographyID, this.pdfPreviewFormGroup.getRawValue()).subscribe({
            next: (response) => {
                saveAs(response, `template-preview.pdf`);
                this.isLoadingPdfPreview = false;
            },
            error: () => (this.isLoadingPdfPreview = false),
        });
    }

    public saveStatementTemplate() {
        this.isLoadingSubmit = true;

        const labelDict = { Balance: this.balanceCustomLabelFormControl.getRawValue() };
        this.formGroup.controls.CustomLabels.setValue(labelDict);

        var requestDto = this.formGroup.getRawValue();
        var route = this.statementTemplateID
            ? this.statementTemplateByGeographyService.updateStatementTemplateStatementTemplateByGeography(this.geographyID, this.statementTemplateID, requestDto)
            : this.statementTemplateByGeographyService.createStatementTemplateStatementTemplateByGeography(this.geographyID, requestDto);

        route.subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.initialFormData = null;
                this.router.navigate(["/configure", this.geographySlug, "statement-templates"]).then(() => {
                    this.alertService.pushAlert(new Alert(`Statement Template successfully ${this.statementTemplateID ? "updated" : "created"}.`, AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
