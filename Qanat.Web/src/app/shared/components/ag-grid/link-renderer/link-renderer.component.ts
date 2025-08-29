import { Component } from "@angular/core";
import { AgRendererComponent } from "ag-grid-angular";
import { RouterLink } from "@angular/router";

@Component({
    selector: "qanat-link-renderer",
    templateUrl: "./link-renderer.component.html",
    styleUrls: ["./link-renderer.component.scss"],
    imports: [RouterLink]
})
export class LinkRendererComponent implements AgRendererComponent {
    params: {
        value: {
            LinkDisplay: string;
            LinkValue: string;
            queryParams?: {};
        };
        inRouterLink: string;
        cssClasses?: string;
    };

    agInit(params: any): void {
        if (params.value === null) {
            params = {
                value: { LinkDisplay: "", LinkValue: "" },
                inRouterLink: "",
            };
        } else {
            this.params = params;

            if (!params.inRouterLink) {
                params.inRouterLink = "";
            }
        }
    }

    refresh(params: any): boolean {
        return false;
    }
}
