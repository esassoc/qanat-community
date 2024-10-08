import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ITooltipAngularComp } from "ag-grid-angular";
import { ITooltipParams } from "ag-grid-community";
import { CopyToClipboardDirective } from "src/app/shared/directives/copy-to-clipboard.directive";

@Component({
    selector: "tooltip",
    standalone: true,
    imports: [CommonModule, CopyToClipboardDirective],
    templateUrl: "./tooltip.component.html",
    styleUrl: "./tooltip.component.scss",
})
export class TooltipComponent implements ITooltipAngularComp {
    public displayValue: string;

    agInit(params: ITooltipParams): void {
        if (!params.value) return;

        this.displayValue = params.value instanceof Object ? params.value.LinkDisplay ?? params.value.downloadDisplay : params.valueFormatted ?? params.value;
    }
}
