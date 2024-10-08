import { Injectable } from "@angular/core";
import { Observable, ReplaySubject, Subject, Subscription } from "rxjs";
import { WellRegistryWorkflowProgressDto, WellRegistryWorkflowProgressDtoSteps } from "../generated/model/models";
import { ActivatedRoute, Router } from "@angular/router";
import { WellRegistrationService } from "../generated/api/well-registration.service";

@Injectable({
    providedIn: "root",
})
export class WellRegistryWorkflowProgressService {
    private progressSubject: Subject<WellRegistryWorkflowProgressDto> = new ReplaySubject();
    public progressObservable$: Observable<WellRegistryWorkflowProgressDto> = this.progressSubject.asObservable();

    private progressSubscription = Subscription.EMPTY;

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    updateProgress(wellID: number): void {
        this.progressSubscription.unsubscribe();
        this.getProgress(wellID);
    }

    getProgress(wellID: number) {
        if (wellID) {
            this.progressSubscription = this.wellRegistrationService.wellRegistrationsWellRegistrationIDProgressGet(wellID).subscribe((response) => {
                this.progressSubject.next(response);
            });
        } else {
            this.progressSubject.next(new WellRegistryWorkflowProgressDto({ Steps: new WellRegistryWorkflowProgressDtoSteps() }));
        }
    }
}
