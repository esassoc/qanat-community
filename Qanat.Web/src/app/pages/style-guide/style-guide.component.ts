import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { IconComponent, ICONS } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "style-guide",
    templateUrl: "./style-guide.component.html",
    styleUrls: ["./style-guide.component.scss"],
    imports: [PageHeaderComponent, FormsModule, IconComponent]
})
export class StyleGuideComponent {
    public icons = ICONS;
    constructor() {}
}
