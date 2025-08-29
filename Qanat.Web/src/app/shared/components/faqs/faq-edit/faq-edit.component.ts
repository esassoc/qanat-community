import { Component, Input } from "@angular/core";

import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CdkDrag, CdkDragDrop, CdkDragHandle, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { FrequentlyAskedQuestionLocationDisplayDto } from "src/app/shared/generated/model/frequently-asked-question-location-display-dto";

@Component({
    selector: "faq-edit",
    templateUrl: "./faq-edit.component.html",
    styleUrl: "./faq-edit.component.scss",
    imports: [IconComponent, CdkDrag, CdkDragHandle, CdkDropList]
})
export class FaqEditComponent {
    @Input() frequentlyAskedQuestions: FrequentlyAskedQuestionLocationDisplayDto[];

    public isLoadingSubmit: boolean = false;

    public removefrequentlyAskedQuestion(index: number) {
        this.frequentlyAskedQuestions.splice(index, 1);
    }

    drop(event: CdkDragDrop<string[]>) {
        moveItemInArray(this.frequentlyAskedQuestions, event.previousIndex, event.currentIndex);
    }
}
