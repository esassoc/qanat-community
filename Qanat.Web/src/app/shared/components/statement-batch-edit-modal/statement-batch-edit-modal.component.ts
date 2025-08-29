import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { AsyncPipe } from "@angular/common";
import { StatementBatchUpsertDtoForm, StatementBatchUpsertDtoFormControls } from "../../generated/model/statement-batch-upsert-dto";
import { StatementTemplateByGeographyService } from "../../generated/api/statement-template-by-geography.service";
import { NoteComponent } from "../note/note.component";
import { IconComponent } from "../icon/icon.component";
import { StatementBatchByGeographyService } from "../../generated/api/statement-batch-by-geography.service";
import { DialogRef } from "@ngneat/dialog";
import { StatementBatchDto } from "../../generated/model/models";

@Component({
    selector: "statement-batch-edit-modal",
    imports: [ReactiveFormsModule, AlertDisplayComponent, FormFieldComponent, AsyncPipe, NoteComponent, IconComponent],
    templateUrl: "./statement-batch-edit-modal.component.html",
    styleUrls: ["./statement-batch-edit-modal.component.scss"],
    animations: [inOutAnimation]
})
export class StatementBatchEditModalComponent implements OnInit {
    public ref: DialogRef<StatementBatchContext, StatementBatchResult> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public statementTemplateSelectOptions$: Observable<SelectDropdownOption[]>;

    public reportingPeriodNameAndDateRange: { [reportingPeriodID: number]: string } = {};
    public selectedReportingPeriodNameAndDateRange: string;

    public isLoadingSubmit: boolean = false;

    public formGroup: FormGroup<StatementBatchUpsertDtoForm> = new FormGroup<StatementBatchUpsertDtoForm>({
        StatementBatchName: StatementBatchUpsertDtoFormControls.StatementBatchName(),
        StatementTemplateID: StatementBatchUpsertDtoFormControls.StatementTemplateID(),
        ReportingPeriodID: StatementBatchUpsertDtoFormControls.ReportingPeriodID(),
        WaterAccountIDs: StatementBatchUpsertDtoFormControls.WaterAccountIDs(),
    });

    constructor(
        private statementTemplateByGeographyService: StatementTemplateByGeographyService,
        private statementBatchByGeographyService: StatementBatchByGeographyService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.ReportingPeriodID.patchValue(this.ref.data.ReportingPeriodID);
        this.formGroup.controls.WaterAccountIDs.patchValue(this.ref.data.WaterAccountIDs);

        this.statementTemplateSelectOptions$ = this.statementTemplateByGeographyService
            .listStatementTemplatesByGeographyIDStatementTemplateByGeography(this.ref.data.GeographyID)
            .pipe(
                map((statementTemplates) => {
                    return statementTemplates.map((statementTemplate) => {
                        return {
                            Value: statementTemplate.StatementTemplateID,
                            Label: statementTemplate.TemplateTitle,
                        } as SelectDropdownOption;
                    });
                })
            );

        this.formGroup.controls.ReportingPeriodID.valueChanges.subscribe((x) => (this.selectedReportingPeriodNameAndDateRange = this.reportingPeriodNameAndDateRange[x]));
    }

    save(): void {
        this.isLoadingSubmit = true;
        this.statementBatchByGeographyService.createStatementBatchStatementBatchByGeography(this.ref.data.GeographyID, this.formGroup.getRawValue()).subscribe({
            next: (statementBatch) => {
                this.ref.close({
                    Success: true,
                    StatementBatchDto: statementBatch,
                } as StatementBatchResult);
            },
            error: () => {
                this.close();
            },
        });
    }

    close(): void {
        this.ref.close(null);
    }
}

export interface StatementBatchContext {
    GeographyID: number;
    ReportingPeriodID: number;
    WaterAccountIDs: number[];
}

export interface StatementBatchResult {
    Success: boolean;
    StatementBatchDto: StatementBatchDto;
}
