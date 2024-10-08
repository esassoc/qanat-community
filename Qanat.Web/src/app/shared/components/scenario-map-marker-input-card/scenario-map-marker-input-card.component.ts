import { Component, Input, OnInit, Output, EventEmitter } from "@angular/core";
import { DecimalPipe } from "@angular/common";

@Component({
    selector: "scenario-map-marker-input-card",
    templateUrl: "./scenario-map-marker-input-card.component.html",
    styleUrls: ["./scenario-map-marker-input-card.component.scss"],
    standalone: true,
    imports: [DecimalPipe],
})
export class ScenarioMapMarkerInputCardComponent implements OnInit {
    @Input() latitude: number;
    @Input() longitude: number;
    @Input() selected: boolean = false;
    @Input() colorScheme: "yellow" | "blue" = "blue";

    @Output() cardSelected = new EventEmitter<any>();
    @Output() cardDeleted = new EventEmitter<any>();

    constructor() {}

    ngOnInit(): void {}

    public onCardSelected() {
        this.cardSelected.emit();
    }

    public onDelete() {
        this.cardDeleted.emit();
    }
}
