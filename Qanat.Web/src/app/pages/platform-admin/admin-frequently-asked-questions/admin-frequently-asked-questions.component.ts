import { Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CreateFaqModalComponent } from "src/app/shared/components/faqs/create-faq-modal/create-faq-modal.component";
import { EditFaqModalComponent } from "src/app/shared/components/faqs/edit-faq-modal/edit-faq-modal.component";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FrequentlyAskedQuestionGridDto } from "src/app/shared/generated/model/frequently-asked-question-grid-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { FaqDisplayLocationTypes } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "admin-frequently-asked-questions",
    templateUrl: "./admin-frequently-asked-questions.component.html",
    styleUrl: "./admin-frequently-asked-questions.component.scss",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, AsyncPipe],
})
export class AdminFrequentlyAskedQuestionsComponent implements OnInit {
    frequentlyAskedQuestions$: Observable<FrequentlyAskedQuestionGridDto[]>;
    colDefs: ColDef[];
    private gridApi;
    customRichTextID = CustomRichTextTypeEnum.AdminFAQ;

    constructor(
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService,
        private publicService: PublicService,
        private utilityFunctionsService: UtilityFunctionsService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.loadFAQs();
        this.loadColDefs();
    }

    loadFAQs() {
        this.frequentlyAskedQuestions$ = this.publicService.listFrequentlyAskedQuestionsPublic();
    }

    loadColDefs() {
        this.colDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.openEditModal(params.data.FrequentlyAskedQuestionID),
                    },
                    {
                        ActionName: "Delete",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.openDeleteModal(params.data.FrequentlyAskedQuestionID),
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("ID", "FrequentlyAskedQuestionID"),
            this.utilityFunctionsService.createBasicColumnDef("Question", "QuestionText", { Width: 500 }),
            this.utilityFunctionsService.createBasicColumnDef("Displayed On", "FaqDisplayLocations", {
                ValueGetter: (params) => {
                    const ids = params.data.FaqDisplayLocations.map((x) => x.FaqDisplayLocationTypeID);
                    if (ids.length > 0) {
                        return FaqDisplayLocationTypes.filter((x) => ids.includes(x.Value))
                            .map((x) => x.DisplayName)
                            .join(", ");
                    }
                },
            }),
        ];
    }

    onGridReady = (params) => {
        this.gridApi = params.api;
    };

    public openCreateNewModal() {
        const dialogRef = this.dialogService.open(CreateFaqModalComponent, {
            data: {
                FrequentlyAskedQuestionID: null,
                FaqDisplayLocationTypeID: null,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadFAQs();
            }
        });
    }

    public openEditModal(faqID: number) {
        const dialogRef = this.dialogService.open(EditFaqModalComponent, {
            data: {
                FrequentlyAskedQuestionID: faqID,
                FaqDisplayLocationTypeID: null,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadFAQs();
            }
        });
    }

    public openDeleteModal(faqID: number) {
        this.confirmService
            .confirm({
                title: "Delete Frequently Asked Question",
                message: "Are you sure you want to delete this frequently asked question? This action cannot be undone.",
                buttonClassYes: "btn-danger",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.frequentlyAskedQuestionsService.deleteFrequentlyAskedQuestion(faqID).subscribe((response) => {
                        this.loadFAQs();
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("Frequently Asked Question Successfully Deleted.", AlertContext.Success));
                    });
                }
            });
    }
}
