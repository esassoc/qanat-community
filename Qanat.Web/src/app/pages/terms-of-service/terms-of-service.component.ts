import { Component } from "@angular/core";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";

@Component({
    selector: "terms-of-service",
    imports: [PageHeaderComponent, CustomRichTextComponent],
    templateUrl: "./terms-of-service.component.html",
    styleUrl: "./terms-of-service.component.scss",
})
export class TermsOfServiceComponent {
    public customRichTextID: number = CustomRichTextTypeEnum.TermsOfService;
}
