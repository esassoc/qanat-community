import { Component, Input, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { WorkflowHelpComponent } from "../workflow-help/workflow-help.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "workflow-body",
    standalone: true,
    imports: [CommonModule, WorkflowHelpComponent, LoadingDirective],
    templateUrl: "./workflow-body.component.html",
    styleUrls: ["./workflow-body.component.scss"],
})
export class WorkflowBodyComponent {
    @Input() showLoadingSpinner: boolean = false;
    @Input() helpCustomRichTextTypeID: number = null;
    @Input() sidebarTemplateAppend: TemplateRef<any> = null;
    public displaySidebar: boolean = false;

    constructor() {}

    ngOnInit(): void {
        this.displaySidebar = this.sidebarTemplateAppend != null || this.helpCustomRichTextTypeID != null;
    }
}
