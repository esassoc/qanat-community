import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
    selector: "icon",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./icon.component.html",
    styleUrls: ["./icon.component.scss"],
})
export class IconComponent {
    @Input() icon: typeof IconInterface;
    @Input() enableFontSize: boolean = false;
}

export var IconInterface:
    | "ActivityCenter"
    | "Allocations"
    | "AngleUp"
    | "AngleDown"
    | "ArrowLeft"
    | "Budget"
    | "BulletedList"
    | "Calendar"
    | "Calculator"
    | "CaretDown"
    | "CaretUp"
    | "ChatBubble"
    | "CircleCheckmark"
    | "CircleX"
    | "Code"
    | "Configure"
    | "Contact"
    | "DataLayers"
    | "Delete"
    | "Download"
    | "Dollar"
    | "Drag"
    | "ExternalLink"
    | "Geography"
    | "Geography-Alt"
    | "Guide"
    | "Inbox"
    | "Info"
    | "Layout"
    | "License"
    | "LineChart"
    | "LineChartTrendingUp"
    | "LineChartTrendingDown"
    | "Logo"
    | "Manage"
    | "Map"
    | "Measurements"
    | "Model"
    | "News"
    | "Parcels"
    | "Question"
    | "Resend"
    | "Review"
    | "ReviewApprove"
    | "ReviewReturn"
    | "Satellite"
    | "ScenarioPlanner"
    | "ScenarioRun"
    | "Star"
    | "Statistics"
    | "StepComplete"
    | "StepIncomplete"
    | "SupportLogo"
    | "Transactions"
    | "User"
    | "Users"
    | "VerticalMap"
    | "Warning"
    | "WaterAccounts"
    | "WaterDrop"
    | "WaterDropFilled"
    | "WaterSupply"
    | "Wells"
    | "Zones";
