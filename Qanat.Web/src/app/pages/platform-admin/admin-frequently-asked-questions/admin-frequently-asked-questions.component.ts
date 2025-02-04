import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable, forkJoin, map, tap } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CreateFaqModalComponent } from "src/app/shared/components/faqs/create-faq-modal/create-faq-modal.component";
import { EditFaqModalComponent, FAQContext } from "src/app/shared/components/faqs/edit-faq-modal/edit-faq-modal.component";
import { FrequentlyAskedQuestionService } from "src/app/shared/generated/api/frequently-asked-question.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FaqDisplayLocationTypeSimpleDto } from "src/app/shared/generated/model/faq-display-location-type-simple-dto";
import { FrequentlyAskedQuestionGridDto } from "src/app/shared/generated/model/frequently-asked-question-grid-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "admin-frequently-asked-questions",
    templateUrl: "./admin-frequently-asked-questions.component.html",
    styleUrl: "./admin-frequently-asked-questions.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, QanatGridComponent, AsyncPipe],
})
export class AdminFrequentlyAskedQuestionsComponent implements OnInit {
    frequentlyAskedQuestions$: Observable<FrequentlyAskedQuestionGridDto[]>;
    faqDisplayLocationTypes$: Observable<FaqDisplayLocationTypeSimpleDto[]>;
    faqDisplayLocationTypes: FaqDisplayLocationTypeSimpleDto[];
    colDefs: ColDef[];
    private gridApi;
    customRichTextID = CustomRichTextTypeEnum.AdminFAQ;

    constructor(
        private frequentlyAskedQuestionsService: FrequentlyAskedQuestionService,
        private publicService: PublicService,
        private utilityFunctionsService: UtilityFunctionsService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.loadFAQs();
        this.loadColDefs();
    }

    loadFAQs() {
        this.frequentlyAskedQuestions$ = forkJoin([this.publicService.publicFaqGet(), this.frequentlyAskedQuestionsService.faqDisplayLocationsGet()]).pipe(
            tap((x) => {
                this.faqDisplayLocationTypes = x[1];
            }),
            map((x) => {
                return x[0];
            })
        );
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
                        let temp = "";
                        ids.forEach((id) => {
                            temp += this.faqDisplayLocationTypes.find((x) => x.FaqDisplayLocationTypeID == id).FaqDisplayLocationTypeDisplayName + ", ";
                        });
                        return temp;
                    }
                },
            }),
        ];
    }

    onGridReady = (params) => {
        this.gridApi = params.api;
    };

    public openCreateNewModal() {
        this.modalService
            .open(CreateFaqModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light })
            .instance.result.then((result) => {
                if (result) {
                    this.loadFAQs();
                }
            });
    }

    public openEditModal(faqID: number) {
        this.modalService
            .open(EditFaqModalComponent, null, { CloseOnClickOut: false, TopLayer: false, ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                FrequentlyAskedQuestionID: faqID,
            } as FAQContext)
            .instance.result.then((result) => {
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
                    this.frequentlyAskedQuestionsService.faqFrequentlyAskedQuestionIDDelete(faqID).subscribe((response) => {
                        this.loadFAQs();
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("Frequently Asked Question Successfully Deleted.", AlertContext.Success));
                    });
                }
            });
    }
}
