import { Component, EventEmitter, Input, Output } from "@angular/core";
import { IconComponent } from "../icon/icon.component";
import { GeographyLogoComponent } from "../geography-logo/geography-logo.component";
import { CommonModule } from "@angular/common";

@Component({
    selector: "usage-statement-template-preview",
    imports: [IconComponent, GeographyLogoComponent, CommonModule],
    templateUrl: "./usage-statement-template-preview.component.html",
    styleUrl: "./usage-statement-template-preview.component.scss"
})
export class UsageStatementTemplatePreviewComponent {
    @Input() geographyID: number;
    @Output() editButtonClicked: EventEmitter<string> = new EventEmitter<string>();

    public customFieldName1 = "Page 1: Additional Information";
    public customFieldName2 = "Page 1: About This Usage Statement";
    public customFieldName3 = "Page 2: Additional Information";
    public customFieldName4 = "Page 2: Have Questions?";

    public onEditButtonClicked(customFieldName: string) {
        this.editButtonClicked.emit(customFieldName);
    }
}
