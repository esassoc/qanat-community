import { Component } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FaqDisplayLocationTypeEnum } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { FaqDisplayComponent } from "../../shared/components/faqs/faq-display/faq-display.component";
import { RouterLink } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { RichLinkComponent } from "src/app/shared/components/rich-link/rich-link.component";

@Component({
    selector: "manager-guide",
    templateUrl: "./manager-guide.component.html",
    styleUrls: ["./manager-guide.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, IconComponent, RichLinkComponent, RouterLink, FaqDisplayComponent]
})
export class ManagerGuideComponent {
    public faqDisplayLocationTypeID: FaqDisplayLocationTypeEnum = FaqDisplayLocationTypeEnum.WaterManagerGuide;
    public updateProfileRichTextTypeID = CustomRichTextTypeEnum.HomepageUpdateProfileLink;
    public growerGuideRichTextTypeID = CustomRichTextTypeEnum.HomepageGrowerGuideLink;
    public geographiesRichTextTypeID = CustomRichTextTypeEnum.HomepageGeographiesLink;
}
