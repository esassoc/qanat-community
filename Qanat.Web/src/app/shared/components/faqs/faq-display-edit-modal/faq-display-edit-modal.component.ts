import { Component, ComponentRef, OnInit } from "@angular/core";
import { Observable, tap } from "rxjs";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { FAQContext } from "../edit-faq-modal/edit-faq-modal.component";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FaqEditComponent } from "../faq-edit/faq-edit.component";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { FrequentlyAskedQuestionGridDto } from "src/app/shared/generated/model/models";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "faq-display-edit-modal",
    standalone: true,
    templateUrl: "./faq-display-edit-modal.component.html",
    styleUrl: "./faq-display-edit-modal.component.scss",
    imports: [CommonModule, FaqEditComponent, ReactiveFormsModule, SelectDropDownModule, IconComponent, LoadingDirective, FormsModule],
})
export class FaqDisplayEditModalComponent implements IModal, OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: FAQContext;
    public dropdownConfig = {
        search: true,
        height: "320px",
        placeholder: '(e.g. "What are Water Accounts...")',
        displayKey: "QuestionText",
        searchOnKey: "QuestionText",
        multiple: true,
    };

    public frequentlyAskedQuestionForPage$: Observable<FrequentlyAskedQuestionGridDto[]>;
    public frequentlyAskedQuestions$: Observable<FrequentlyAskedQuestionGridDto[]>;

    isLoadingSubmit: boolean = false;

    public selectedFaqsModel: FrequentlyAskedQuestionGridDto[] = [];

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService
    ) {}

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    ngOnInit(): void {
        this.frequentlyAskedQuestionForPage$ = this.frequentlyAskedQuestionsService
            .publicFaqLocationFaqDisplayQuestionLocationTypeIDGet(this.modalContext.FaqDisplayLocationTypeID)
            .pipe(
                tap((faqs) => {
                    this.selectedFaqsModel = faqs;
                })
            );
        this.frequentlyAskedQuestions$ = this.frequentlyAskedQuestionsService.publicFaqGet();
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.faqsFaqDisplayLocationTypeIDPost(this.modalContext.FaqDisplayLocationTypeID, this.selectedFaqsModel).subscribe(
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
