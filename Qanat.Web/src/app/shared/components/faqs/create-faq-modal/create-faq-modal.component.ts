import { ComponentRef, Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { WaterAccountContext } from "../../water-account/modals/update-water-account-info/update-water-account-info.component";
import { Observable, tap } from "rxjs";
import { FaqDisplayLocationTypeSimpleDto } from "src/app/shared/generated/model/faq-display-location-type-simple-dto";
import { FrequentlyAskedQuestionAdminFormDtoForm, FrequentlyAskedQuestionAdminFormDtoFormControls } from "src/app/shared/generated/model/models";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { FormFieldComponent, FormFieldType, FormInputOption } from "../../forms/form-field/form-field.component";
import { CommonModule } from "@angular/common";

@Component({
    selector: "create-faq-modal",
    standalone: true,
    templateUrl: "./create-faq-modal.component.html",
    styleUrl: "./create-faq-modal.component.scss",
    imports: [ReactiveFormsModule, SelectDropDownModule, FormFieldComponent, CommonModule],
})
export class CreateFaqModalComponent implements IModal, OnInit {
    changedDisplayLocations(event: any) {
        const newValue = event.value.map((x) => x.Value);
        this.formGroup.controls.FaqDisplayLocationTypeIDs.patchValue(newValue);
    }

    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public selectedParcel: ParcelDisplayDto;
    public waterAccountParcels: ParcelDisplayDto[] = [];
    public faqDisplayLocationTypes$: Observable<FaqDisplayLocationTypeSimpleDto[]>;
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
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService
    ) {}

    ngOnInit(): void {
        this.faqDisplayLocationTypes$ = this.frequentlyAskedQuestionsService.faqDisplayLocationsGet().pipe(
            tap((faqDisplayLocationTypes) => {
                this.faqDisplayLocationTypesFormInputOptions = faqDisplayLocationTypes.map((faqDisplayLocationType) => {
                    return {
                        Value: faqDisplayLocationType.FaqDisplayLocationTypeID,
                        Label: faqDisplayLocationType.FaqDisplayLocationTypeDisplayName,
                    } as FormInputOption;
                });
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.faqPost(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Created new question successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, result);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new frequently asked question.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }
}
