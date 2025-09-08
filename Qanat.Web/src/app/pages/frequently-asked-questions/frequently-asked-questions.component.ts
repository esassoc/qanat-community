import { Component, OnInit } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, debounceTime, map, startWith, tap } from "rxjs";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FrequentlyAskedQuestionSimpleDto } from "src/app/shared/generated/model/frequently-asked-question-simple-dto";
import { LoadingDirective } from "../../shared/directives/loading.directive";
import { AsyncPipe } from "@angular/common";
import { FormFieldComponent } from "../../shared/components/forms/form-field/form-field.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FrequentlyAskedQuestionDisplayComponent } from "src/app/shared/components/frequently-asked-question-display/frequently-asked-question-display.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "frequently-asked-questions",
    templateUrl: "./frequently-asked-questions.component.html",
    styleUrl: "./frequently-asked-questions.component.scss",
    imports: [
        PageHeaderComponent,
        AlertDisplayComponent,
        FormFieldComponent,
        FormsModule,
        ReactiveFormsModule,
        FrequentlyAskedQuestionDisplayComponent,
        LoadingDirective,
        AsyncPipe,
    ]
})
export class FrequentlyAskedQuestionsComponent implements OnInit {
    frequentlyAskedQuestions$: Observable<FrequentlyAskedQuestionSimpleDto[]>;
    private allFaqs: FrequentlyAskedQuestionSimpleDto[] = [];
    public faqsToDisplay$: Observable<FrequentlyAskedQuestionSimpleDto[]>;
    public FormFieldType = FormFieldType;
    customRichTextID = CustomRichTextTypeEnum.GeneralFAQ;

    searchFormControl = new FormControl("");
    public searchTermDebounced: string = "";

    constructor(private publicService: PublicService) {}

    ngOnInit(): void {
        this.frequentlyAskedQuestions$ = this.publicService.listFrequentlyAskedQuestionsPublic().pipe(
            tap((faqs) => {
                this.allFaqs = faqs;
                this.faqsToDisplay$ = this.searchFormControl.valueChanges.pipe(
                    debounceTime(100),
                    map((searchTerm) => {
                        this.searchTermDebounced = searchTerm;
                        return this.allFaqs.filter(
                            (faq) => faq.QuestionText.toLowerCase().includes(searchTerm.toLowerCase()) || faq.AnswerText.toLowerCase().includes(searchTerm.toLowerCase())
                        );
                    }),
                    startWith(faqs)
                );
            })
        );
    }

    clearSearch(): void {
        this.searchFormControl.setValue("");
    }
}
