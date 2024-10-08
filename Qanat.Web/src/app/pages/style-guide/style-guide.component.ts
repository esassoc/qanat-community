import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "style-guide",
    templateUrl: "./style-guide.component.html",
    styleUrls: ["./style-guide.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, FormsModule],
})
export class StyleGuideComponent {
    constructor() {}
}
