import { ImplicitReceiver } from "@angular/compiler";
import { Component, ComponentRef, Input, OnDestroy } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Subscription } from "rxjs";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { WaterMeasurementSelfReportService } from "src/app/shared/generated/api/water-measurement-self-report.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographySimpleDto, WaterMeasurementSelfReportSimpleDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "submit-self-report-modal",
    standalone: true,
    imports: [CustomRichTextComponent, NoteComponent, FormsModule, FormFieldComponent, ReactiveFormsModule],
    templateUrl: "./submit-self-report-modal.component.html",
    styleUrl: "./submit-self-report-modal.component.scss",
})
export class SubmitSelfReportModalComponent implements IModal, OnDestroy {
    FormFieldType = FormFieldType;
    modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: SelfReportContext;
    public customRichTextID: number = CustomRichTextTypeEnum.SubmitSelfReportDisclaimer;

    public formControl = new FormControl<boolean>(false);

    public isLoadingSubmit: boolean = false;
    public subscriptions: Subscription[] = [];

    constructor(private modalService: ModalService, private waterMeasurementSelfReportService: WaterMeasurementSelfReportService, private alertService: AlertService) {}

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
            .geographiesGeographyIDWaterAccountsWaterAccountIDWaterMeasurementSelfReportsWaterMeasurementSelfReportIDSubmitPut(
                this.modalContext.GeographyID,
                this.modalContext.WaterAccountID,
                this.modalContext.SelfReport.WaterMeasurementSelfReportID
            )
            .subscribe((result) => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Successfully submitted self report.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, result);
            });

        this.subscriptions.push(submitRequest);
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class SelfReportContext {
    SelfReport: WaterMeasurementSelfReportSimpleDto;
    GeographyID: number;
    WaterAccountID: number;
}
