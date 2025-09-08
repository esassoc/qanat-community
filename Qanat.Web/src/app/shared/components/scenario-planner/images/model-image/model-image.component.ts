import { AsyncPipe } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { ModelService } from "src/app/shared/generated/api/model.service";

@Component({
    selector: "model-image",
    imports: [AsyncPipe],
    templateUrl: "./model-image.component.html",
    styleUrls: ["./model-image.component.scss"]
})
export class ModelImageComponent implements OnInit {
    @Input() modelID: number;

    public image$: Observable<string>;
    constructor(private modelService: ModelService) {}

    ngOnInit(): void {
        this.image$ = this.modelService.getModelImageByIDModel(this.modelID);
    }
}
