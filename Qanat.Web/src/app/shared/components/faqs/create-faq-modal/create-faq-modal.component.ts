import { Component, OnInit, inject } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WaterAccountContext } from "../../water-account/modals/update-water-account-info/update-water-account-info.component";
import { FrequentlyAskedQuestionAdminFormDtoForm, FrequentlyAskedQuestionAdminFormDtoFormControls, FrequentlyAskedQuestionSimpleDto } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType } from "../../forms/form-field/form-field.component";

import { FaqDisplayLocationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { DialogRef } from "@ngneat/dialog";
import { FAQContext } from "../edit-faq-modal/edit-faq-modal.component";

@Component({
    selector: "create-faq-modal",
    templateUrl: "./create-faq-modal.component.html",
    styleUrl: "./create-faq-modal.component.scss",
    imports: [ReactiveFormsModule, FormFieldComponent]
})
export class CreateFaqModalComponent implements OnInit {
    public ref: DialogRef<FAQContext, FAQResult> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public selectedParcel: ParcelDisplayDto;
    public waterAccountParcels: ParcelDisplayDto[] = [];
    public faqDisplayLocationTypesFormInputOptions = FaqDisplayLocationTypesAsSelectDropdownOptions;

    public formGroup = new FormGroup<FrequentlyAskedQuestionAdminFormDtoForm>({
        FrequentlyAskedQuestionID: FrequentlyAskedQuestionAdminFormDtoFormControls.FrequentlyAskedQuestionID(),
        QuestionText: FrequentlyAskedQuestionAdminFormDtoFormControls.QuestionText(),
        AnswerText: FrequentlyAskedQuestionAdminFormDtoFormControls.AnswerText(),
        FaqDisplayLocationTypeIDs: FrequentlyAskedQuestionAdminFormDtoFormControls.FaqDisplayLocationTypeIDs(),
    });

    constructor(
        private alertService: AlertService,
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService
    ) {}

    ngOnInit(): void {}

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.createFrequentlyAskedQuestion(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Created new question successfully.", AlertContext.Success));
                this.ref.close({ Success: true, FrequentlyAskedQuestionDto: result } as FAQResult);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new frequently asked question.", AlertContext.Success));
                this.ref.close(null);
            }
        );
    }
}

export class FAQResult {
    Success: boolean;
    FrequentlyAskedQuestionDto: FrequentlyAskedQuestionSimpleDto;
}
