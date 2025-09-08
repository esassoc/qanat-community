import { DecimalPipe, NgClass } from "@angular/common";
import { Component, Input } from "@angular/core";
import { FeeCalculatorOutputScenarioDto } from "src/app/shared/generated/model/fee-calculator-output-scenario-dto";

@Component({
    selector: "fee-calculator-scenario-display",
    imports: [DecimalPipe, NgClass],
    templateUrl: "./fee-calculator-scenario-display.component.html",
    styleUrls: ["./fee-calculator-scenario-display.component.scss", "../fee-calculator.component.scss"],
})
export class FeeCalculatorScenarioDisplayComponent {
    @Input() scenario: FeeCalculatorOutputScenarioDto;
}
