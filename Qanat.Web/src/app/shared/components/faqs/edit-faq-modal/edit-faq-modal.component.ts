import { ComponentRef, Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { Observable, map, tap } from "rxjs";
import { FrequentlyAskedQuestionAdminFormDtoForm, FrequentlyAskedQuestionAdminFormDtoFormControls, FrequentlyAskedQuestionGridDto } from "src/app/shared/generated/model/models";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { FormFieldComponent, FormFieldType, FormInputOption } from "../../forms/form-field/form-field.component";
import { CommonModule } from "@angular/common";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "edit-faq-modal",
    standalone: true,
    imports: [ReactiveFormsModule, SelectDropDownModule, FormFieldComponent, CommonModule],
    templateUrl: "./edit-faq-modal.component.html",
    styleUrl: "./edit-faq-modal.component.scss",
})
export class EditFaqModalComponent implements IModal, OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: FAQContext;

    public FormFieldType = FormFieldType;
    public frequentlyAskedQuestion$: Observable<FrequentlyAskedQuestionGridDto>;
    public faqDisplayLocationTypes$: Observable<FormInputOption[]>;
    public faqDisplayLocationTypesFormInputOptions: FormInputOption[];

    public formGroup = new FormGroup<FrequentlyAskedQuestionAdminFormDtoForm>({
        FrequentlyAskedQuestionID: FrequentlyAskedQuestionAdminFormDtoFormControls.FrequentlyAskedQuestionID(),
        QuestionText: FrequentlyAskedQuestionAdminFormDtoFormControls.QuestionText(),
        AnswerText: FrequentlyAskedQuestionAdminFormDtoFormControls.AnswerText(),
        FaqDisplayLocationTypeIDs: FrequentlyAskedQuestionAdminFormDtoFormControls.FaqDisplayLocationTypeIDs(),
    });

    public dropdownFormControl: FormControl = new FormControl();

    dropdownConfig = {
        height: "320px",
        displayKey: "Label",
        placeholder: "Select Pages to display this question on",
        searchOnKey: "Label",
    };

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService,
        private publicService: PublicService
    ) {}

    changedDisplayLocations(event: any) {
        const newValue = event.value.map((x) => x.Value);
        this.formGroup.controls.FaqDisplayLocationTypeIDs.patchValue(newValue);
    }

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    ngOnInit(): void {
        this.frequentlyAskedQuestion$ = this.publicService.publicFaqFrequentlyAskedQuestionIDGet(this.modalContext.FrequentlyAskedQuestionID).pipe(
            tap((faq) => {
                this.formGroup.patchValue(faq);
                this.formGroup.controls.FaqDisplayLocationTypeIDs.patchValue(faq.FaqDisplayLocations.map((x) => x.FaqDisplayLocationTypeID));

                this.faqDisplayLocationTypes$ = this.frequentlyAskedQuestionsService.faqDisplayLocationsGet().pipe(
                    map((faqDisplayLocationTypes) => {
                        return faqDisplayLocationTypes.map((faqDisplayLocationType) => {
                            return {
                                Value: faqDisplayLocationType.FaqDisplayLocationTypeID,
                                Label: faqDisplayLocationType.FaqDisplayLocationTypeDisplayName,
                            } as FormInputOption;
                        });
                    }),
                    tap((x) => {
                        const displayLocationIDs = faq.FaqDisplayLocations.map((x) => x.FaqDisplayLocationTypeID);
                        const inputOptionsSelected = x.filter((y) => displayLocationIDs.includes(y.Value));
                        this.dropdownFormControl.patchValue(inputOptionsSelected);
                    })
                );
            })
        );
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.faqPut(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Updated question successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, result);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new frequently asked question.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }
}

export class FAQContext {
    FrequentlyAskedQuestionID: number;
    FaqDisplayLocationTypeID: number;
}
