import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";

@Component({
    selector: "model-image",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./model-image.component.html",
    styleUrls: ["./model-image.component.scss"],
})
export class ModelImageComponent implements OnInit {
    @Input() modelID: number;

    public image$: Observable<string>;
    constructor(private getActionService: GETActionService) {}

    ngOnInit(): void {
        this.image$ = this.getActionService.modelsModelIDImageGet(this.modelID);
    }
}
