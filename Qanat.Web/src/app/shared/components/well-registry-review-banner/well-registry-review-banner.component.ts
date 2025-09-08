import { Component } from "@angular/core";

import { Observable, switchMap, tap } from "rxjs";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { Router } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { WellRegistryWorkflowProgressDto } from "src/app/shared/generated/model/well-registry-workflow-progress-dto";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { WithGeographyFlagDirective } from "src/app/shared/directives/with-geography-flag.directive";
import { IconComponent } from "../icon/icon.component";
import { AsyncPipe } from "@angular/common";

@Component({
    selector: "well-registry-review-banner",
    imports: [IconComponent, WithGeographyFlagDirective, AsyncPipe],
    templateUrl: "./well-registry-review-banner.component.html",
    styleUrls: ["./well-registry-review-banner.component.scss"]
})
export class WellRegistryReviewBannerComponent {
    public FlagEnum = FlagEnum;
    public WellRegistrationStatusEnum = WellRegistrationStatusEnum;
    public isLoadingSubmit: boolean = false;
    public wellProgress$: Observable<WellRegistryWorkflowProgressDto>;
    public currentUser: UserDto;
    public geographyID: number;
    public geography$: Observable<GeographyDto>;
    public geography: GeographyDto;

    constructor(
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private authenticationService: AuthenticationService,
        private geographyRouteservice: GeographyRouteService,
        private wellRegistrationService: WellRegistrationService,
        private router: Router,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.geographyRouteservice.geography$.pipe(tap((geography) => (this.geography = geography)));
        this.wellProgress$ = this.authenticationService.currentUserSetObservable.pipe(
            switchMap((currentUser) => {
                this.currentUser = currentUser;
                return this.wellRegistryProgressService.progressObservable$;
            }),
            tap((x) => {
                this.geographyID = x.GeographyID;
            })
        );
    }

    approve(wellID: number): void {
        this.isLoadingSubmit = true;
        this.wellRegistrationService.approveWellRegistrationWellRegistration(wellID).subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.wellRegistryProgressService.updateProgress(wellID);
                this.router.navigate(["/wells", this.geography.GeographyName.toLowerCase(), "review-submitted-wells"]).then(() => {
                    this.alertService.pushAlert(new Alert("Successfully approved Well Registration", AlertContext.Success));
                });
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    return(wellID: number): void {
        this.isLoadingSubmit = true;
        this.wellRegistrationService.returnWellRegistrationWellRegistration(wellID).subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.wellRegistryProgressService.updateProgress(wellID);
                this.router.navigate(["/wells", this.geography.GeographyName.toLowerCase(), "review-submitted-wells"]).then(() => {
                    this.alertService.pushAlert(new Alert("Successfully returned Well Registration", AlertContext.Success));
                });
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }
}
