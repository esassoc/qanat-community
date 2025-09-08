import { Component } from "@angular/core";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";

@Component({
    selector: "news-and-announcements",
    imports: [PageHeaderComponent, CustomRichTextComponent],
    templateUrl: "./news-and-announcements.component.html",
    styleUrl: "./news-and-announcements.component.scss"
})
export class NewsAndAnnouncementsComponent {
    public customRichTextID: number = CustomRichTextTypeEnum.NewsAndAnnouncements;
}
