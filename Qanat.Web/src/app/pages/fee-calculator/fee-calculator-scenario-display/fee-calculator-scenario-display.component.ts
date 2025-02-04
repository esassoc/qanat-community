import { DecimalPipe, JsonPipe, NgClass, NgIf } from "@angular/common";
import { Component, Input } from "@angular/core";
import { FeeCalculatorOutputSavingsAndIncentivesDto } from "src/app/shared/generated/model/fee-calculator-output-savings-and-incentives-dto";
import { FeeCalculatorOutputScenarioDto } from "src/app/shared/generated/model/fee-calculator-output-scenario-dto";
import { SumPipe } from "src/app/shared/pipes/sum.pipe";

@Component({
    selector: "fee-calculator-scenario-display",
    standalone: true,
    imports: [NgIf, JsonPipe, DecimalPipe, SumPipe, NgClass],
    templateUrl: "./fee-calculator-scenario-display.component.html",
    styleUrls: ["./fee-calculator-scenario-display.component.scss", "../fee-calculator.component.scss"],
})
export class FeeCalculatorScenarioDisplayComponent {
    @Input() scenario: FeeCalculatorOutputScenarioDto;
}
