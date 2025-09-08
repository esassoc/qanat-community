import { Component, inject, OnInit } from "@angular/core";
import { Observable, tap } from "rxjs";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FAQContext } from "../edit-faq-modal/edit-faq-modal.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FaqEditComponent } from "../faq-edit/faq-edit.component";
import { FrequentlyAskedQuestionGridDto, FrequentlyAskedQuestionLocationDisplayDto } from "src/app/shared/generated/model/models";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "faq-display-edit-modal",
    templateUrl: "./faq-display-edit-modal.component.html",
    styleUrl: "./faq-display-edit-modal.component.scss",
    imports: [FaqEditComponent, ReactiveFormsModule, LoadingDirective, FormsModule, NgSelectModule, AsyncPipe],
})
export class FaqDisplayEditModalComponent implements OnInit {
    public ref: DialogRef<FAQContext, FrequentlyAskedQuestionLocationDisplayDto[]> = inject(DialogRef);

    public frequentlyAskedQuestionForPage$: Observable<FrequentlyAskedQuestionGridDto[]>;
    public frequentlyAskedQuestions$: Observable<FrequentlyAskedQuestionGridDto[]>;
    public selectedFaqsModel: FrequentlyAskedQuestionGridDto[] = [];

    isLoadingSubmit: boolean = false;

    constructor(
        private alertService: AlertService,
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService,
        private publicService: PublicService
    ) {}

    close() {
        this.ref.close(null);
    }

    ngOnInit(): void {
        this.frequentlyAskedQuestionForPage$ = this.publicService.listFrequentlyAskedQuestionsByLocationTypeIDPublic(this.ref.data.FaqDisplayLocationTypeID).pipe(
            tap((faqs) => {
                this.selectedFaqsModel = faqs;
            })
        );
        this.frequentlyAskedQuestions$ = this.publicService.listFrequentlyAskedQuestionsPublic();
    }

    save() {
        this.alertService.clearAlerts();
        this.frequentlyAskedQuestionsService.updateFaqDisplayLocationTypeFrequentlyAskedQuestion(this.ref.data.FaqDisplayLocationTypeID, this.selectedFaqsModel).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Updated question successfully.", AlertContext.Success));
                this.ref.close(result);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new frequently asked question.", AlertContext.Success));
                this.ref.close(null);
            }
        );
    }
}
