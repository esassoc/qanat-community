import { Component } from "@angular/core";
import { FaqDisplayLocationTypeEnum } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FaqDisplayComponent } from "../../shared/components/faqs/faq-display/faq-display.component";
import { RouterLink } from "@angular/router";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { RichLinkComponent } from "src/app/shared/components/rich-link/rich-link.component";

@Component({
    selector: "grower-guide",
    templateUrl: "./grower-guide.component.html",
    styleUrls: ["./grower-guide.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, IconComponent, RichLinkComponent, RouterLink, FaqDisplayComponent]
})
export class GrowerGuideComponent {
    public faqDisplayLocationTypeID: FaqDisplayLocationTypeEnum = FaqDisplayLocationTypeEnum.GrowersGuide;
    public waterDashboardRichTextTypeID = CustomRichTextTypeEnum.WaterDashboardLink;
    public managerGuideRichTextTypeID = CustomRichTextTypeEnum.WaterManagerGuideLink;
    public geographiesRichTextTypeID = CustomRichTextTypeEnum.HomepageGeographiesLink;
}
