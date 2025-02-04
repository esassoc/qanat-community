import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { ModelService } from "src/app/shared/generated/api/model.service";

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
    constructor(private modelService: ModelService) {}

    ngOnInit(): void {
        this.image$ = this.modelService.modelsModelIDImageGet(this.modelID);
    }
}
