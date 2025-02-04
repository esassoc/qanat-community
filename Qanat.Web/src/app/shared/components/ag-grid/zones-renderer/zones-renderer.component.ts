import { Component } from "@angular/core";
import { ICellRendererParams } from "ag-grid-community";
import { NgFor } from "@angular/common";
import { ZoneMinimalDto } from "src/app/shared/generated/model/zone-minimal-dto";

@Component({
    selector: "zones-renderer",
    templateUrl: "./zones-renderer.component.html",
    styleUrl: "./zones-renderer.component.scss",
    standalone: true,
    imports: [NgFor],
})
export class ZonesRendererComponent {
    public params: ICellRendererParams;
    public zones: ZoneMinimalDto[];

    agInit(params: ICellRendererParams): void {
        if (params) {
            this.params = params;

            this.zones = params.value.zones;
        } else {
            this.params = { value: [] } as ICellRendererParams;
        }
    }

    refresh(params: ICellRendererParams): boolean {
        return false;
    }
}
