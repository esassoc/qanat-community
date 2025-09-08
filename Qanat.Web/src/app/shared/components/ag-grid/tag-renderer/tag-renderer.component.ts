import { Component } from "@angular/core";
import { ICellRendererParams } from "ag-grid-community";

@Component({
    selector: "tag-renderer",
    templateUrl: "./tag-renderer.component.html",
    styleUrl: "./tag-renderer.component.scss",
    imports: []
})
export class TagRendererComponent {
    public params: ICellRendererParams;

    agInit(params: ICellRendererParams): void {
        if (params) {
            this.params = params;
        } else {
            this.params = { value: [] } as ICellRendererParams;
        }
    }

    refresh(params: ICellRendererParams): boolean {
        return false;
    }
}
