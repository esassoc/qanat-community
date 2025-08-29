import { Component, inject, OnDestroy } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { Subscription } from "rxjs";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { WaterMeasurementSelfReportService } from "src/app/shared/generated/api/water-measurement-self-report.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WaterMeasurementSelfReportDto, WaterMeasurementSelfReportSimpleDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "submit-self-report-modal",
    imports: [CustomRichTextComponent, NoteComponent, FormsModule, FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./submit-self-report-modal.component.html",
    styleUrl: "./submit-self-report-modal.component.scss",
})
export class SubmitSelfReportModalComponent implements OnDestroy {
    public ref: DialogRef<SelfReportContext, WaterMeasurementSelfReportDto> = inject(DialogRef);

    public customRichTextID: number = CustomRichTextTypeEnum.SubmitSelfReportDisclaimer;

    public FormFieldType = FormFieldType;
    public formControl = new FormControl<boolean>(false);

    public isLoadingSubmit: boolean = false;
    public subscriptions: Subscription[] = [];

    constructor(
        private waterMeasurementSelfReportService: WaterMeasurementSelfReportService,
        private alertService: AlertService
    ) {}

    ngOnDestroy(): void {
        this.subscriptions.forEach((sub) => {
            if (sub && sub.unsubscribe) {
                sub.unsubscribe();
            }
        });
    }

    submit(): void {
        this.isLoadingSubmit = true;
        let submitRequest = this.waterMeasurementSelfReportService
            .submitWaterMeasurementSelfReport(this.ref.data.GeographyID, this.ref.data.WaterAccountID, this.ref.data.SelfReport.WaterMeasurementSelfReportID)
            .subscribe((result) => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Successfully submitted self report.", AlertContext.Success));
                this.ref.close(result);
            });

        this.subscriptions.push(submitRequest);
    }

    close(): void {
        this.ref.close(null);
    }
}

export class SelfReportContext {
    SelfReport: WaterMeasurementSelfReportSimpleDto;
    GeographyID: number;
    WaterAccountID: number;
}
