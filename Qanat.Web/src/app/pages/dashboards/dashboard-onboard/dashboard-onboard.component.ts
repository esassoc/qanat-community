import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { GeographyLogoComponent } from "../../../shared/components/geography-logo/geography-logo.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "dashboard-onboard",
    templateUrl: "./dashboard-onboard.component.html",
    styleUrls: ["./dashboard-onboard.component.scss"],
    standalone: true,
    imports: [NgIf, RouterLink, GeographyLogoComponent, RouterLinkActive, IconComponent, RouterOutlet, AsyncPipe],
})
export class DashboardOnboardComponent implements OnInit, OnDestroy {
    private currentUserSubscription: Subscription;
    public currentUser: UserDto;

    public geography$: Observable<GeographyDto>;

    constructor(private authenticationService: AuthenticationService, private route: ActivatedRoute, private publicService: PublicService) {}

    ngOnInit(): void {
        this.currentUserSubscription = this.authenticationService.currentUserSetObservable.subscribe((user) => {
            this.currentUser = user;
        });

        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.publicService.publicGeographiesNameGeographyNameGet(geographyName);
    }

    ngOnDestroy(): void {
        this.currentUserSubscription.unsubscribe();
    }

    isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }
}
