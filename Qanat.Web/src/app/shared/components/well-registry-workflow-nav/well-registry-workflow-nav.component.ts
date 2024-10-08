import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { routeParams } from "src/app/app.routes";
import { ActivatedRoute } from "@angular/router";
import { Observable, tap } from "rxjs";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { WorkflowNavComponent } from "../workflow-nav/workflow-nav.component";
import { WorkflowNavItemComponent } from "../workflow-nav/workflow-nav-item/workflow-nav-item.component";
import { WellRegistryWorkflowProgressDto } from "src/app/shared/generated/model/well-registry-workflow-progress-dto";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";

@Component({
    selector: "well-registry-workflow-nav",
    standalone: true,
    imports: [CommonModule, WorkflowNavComponent, WorkflowNavItemComponent],
    templateUrl: "./well-registry-workflow-nav.component.html",
    styleUrls: ["./well-registry-workflow-nav.component.scss"],
})
export class WellRegistryWorkflowNavComponent implements OnInit {
    public wellRegistrationID: number;
    public isCreating: boolean;
    public progress$: Observable<WellRegistryWorkflowProgressDto>;
    public submitted: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService
    ) {}

    ngOnInit(): void {
        this.wellRegistrationID = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID)
            ? parseInt(this.route.snapshot.paramMap.get(routeParams.wellRegistrationID))
            : null;
        this.route.data.subscribe((x) => {
            this.isCreating = x.create;
        });
        this.progress$ = this.wellRegistryProgressService.progressObservable$.pipe(
            tap((x) => {
                this.submitted =
                    x.WellRegistrationStatus?.WellRegistrationStatusID == WellRegistrationStatusEnum.Approved ||
                    x.WellRegistrationStatus?.WellRegistrationStatusID == WellRegistrationStatusEnum.Submitted;
            })
        );
        this.wellRegistryProgressService.getProgress(this.wellRegistrationID);
    }
}
