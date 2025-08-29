import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { Observable, map } from "rxjs";

@Component({
    selector: "well-registry-workflow-progress",
    imports: [CommonModule],
    templateUrl: "./well-registry-workflow-progress.component.html",
    styleUrls: ["./well-registry-workflow-progress.component.scss"]
})
export class WellRegistryWorkflowProgressComponent {
    public progressPercent$: Observable<number>;
    constructor(private wellRegistryWorkflowProgressService: WellRegistryWorkflowProgressService) {}
    ngOnInit(): void {
        this.progressPercent$ = this.wellRegistryWorkflowProgressService.progressObservable$.pipe(
            map((x) => {
                const totalSteps = Object.keys(x.Steps).length;
                if (!totalSteps) return 0;
                const totalFinished = Object.values(x.Steps).filter((x) => x).length;
                return totalFinished / totalSteps;
            })
        );
    }
}
