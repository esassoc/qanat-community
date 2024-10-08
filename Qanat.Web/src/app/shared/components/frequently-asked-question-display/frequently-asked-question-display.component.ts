import { Component, Input } from "@angular/core";
import { FrequentlyAskedQuestionSimpleDto } from "src/app/shared/generated/model/frequently-asked-question-simple-dto";
import { IconComponent } from "../icon/icon.component";
import { HighlightDirective } from "../../../shared/directives/highlight.directive";
import { ExpandCollapseDirective } from "../../../shared/directives/expand-collapse.directive";

@Component({
    selector: "frequently-asked-question-display",
    templateUrl: "./frequently-asked-question-display.component.html",
    styleUrl: "./frequently-asked-question-display.component.scss",
    standalone: true,
    imports: [ExpandCollapseDirective, HighlightDirective, IconComponent],
})
export class FrequentlyAskedQuestionDisplayComponent {
    @Input() faq: FrequentlyAskedQuestionSimpleDto;
    @Input() highlightText: string = "";
    public expanded: boolean = false;
}
