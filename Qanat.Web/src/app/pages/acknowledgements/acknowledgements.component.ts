import { Component } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CustomRichTextComponent } from "../../shared/components/custom-rich-text/custom-rich-text.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "acknowledgements",
    templateUrl: "./acknowledgements.component.html",
    styleUrl: "./acknowledgements.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, CustomRichTextComponent],
})
export class AcknowledgementsComponent {
    public customRichTextID: number = CustomRichTextTypeEnum.Acknowledgements;
}
