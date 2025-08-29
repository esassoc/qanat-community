import { Component, Input } from "@angular/core";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";

@Component({
    selector: "workflow-help",
    imports: [CustomRichTextComponent],
    templateUrl: "./workflow-help.component.html",
    styleUrls: ["./workflow-help.component.scss"]
})
export class WorkflowHelpComponent {
    @Input() customRichTextTypeID: number;
}
