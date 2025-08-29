import { Component, OnInit, inject } from "@angular/core";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Observable, tap } from "rxjs";
import { FrequentlyAskedQuestionAdminFormDtoForm, FrequentlyAskedQuestionAdminFormDtoFormControls, FrequentlyAskedQuestionGridDto } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType, FormInputOption } from "../../forms/form-field/form-field.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { FaqDisplayLocationTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { FAQResult } from "../create-faq-modal/create-faq-modal.component";

@Component({
    selector: "edit-faq-modal",
    imports: [ReactiveFormsModule, FormFieldComponent, AsyncPipe],
    templateUrl: "./edit-faq-modal.component.html",
    styleUrl: "./edit-faq-modal.component.scss",
})
export class EditFaqModalComponent implements OnInit {
    public ref: DialogRef<FAQContext, FAQResult> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public frequentlyAskedQuestion$: Observable<FrequentlyAskedQuestionGridDto>;
    public faqDisplayLocationTypesFormInputOptions: FormInputOption[];
    public FaqDisplayLocationTypesAsSelectDropdownOptions = FaqDisplayLocationTypesAsSelectDropdownOptions;

    public formGroup = new FormGroup<FrequentlyAskedQuestionAdminFormDtoForm>({
        FrequentlyAskedQuestionID: FrequentlyAskedQuestionAdminFormDtoFormControls.FrequentlyAskedQuestionID(),
        QuestionText: FrequentlyAskedQuestionAdminFormDtoFormControls.QuestionText(),
        AnswerText: FrequentlyAskedQuestionAdminFormDtoFormControls.AnswerText(),
        FaqDisplayLocationTypeIDs: FrequentlyAskedQuestionAdminFormDtoFormControls.FaqDisplayLocationTypeIDs(),
    });

    constructor(
        private alertService: AlertService,
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService,
        private publicService: PublicService
    ) {}

    changedDisplayLocations(event: any) {
        const newValue = event.value.map((x) => x.Value);
        this.formGroup.controls.FaqDisplayLocationTypeIDs.patchValue(newValue);
    }

    close() {
        this.ref.close(null);
    }

    ngOnInit(): void {
        this.frequentlyAskedQuestion$ = this.publicService.getFrequentlyAskedQuestionByIDPublic(this.ref.data.FrequentlyAskedQuestionID).pipe(
            tap((faq) => {
                this.formGroup.patchValue(faq);
                this.formGroup.controls.FaqDisplayLocationTypeIDs.patchValue(faq.FaqDisplayLocations.map((x) => x.FaqDisplayLocationTypeID));
            })
        );
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.updateFrequentlyAskedQuestion(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Updated question successfully.", AlertContext.Success));
                this.ref.close({ Success: true, FrequentlyAskedQuestionDto: result } as FAQResult);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new frequently asked question.", AlertContext.Success));
                this.ref.close(null);
            }
        );
    }
}

export class FAQContext {
    FrequentlyAskedQuestionID: number;
    FaqDisplayLocationTypeID: number;
}
