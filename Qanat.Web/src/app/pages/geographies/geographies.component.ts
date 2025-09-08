import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { filter, map, Observable, Subscription } from "rxjs";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GeographyLogoComponent } from "../../shared/components/geography-logo/geography-logo.component";

@Component({
    selector: "geographies",
    templateUrl: "./geographies.component.html",
    styleUrls: ["./geographies.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, AsyncPipe, GeographyLogoComponent]
})
export class GeographiesComponent implements OnInit, OnDestroy {
    public geographies$: Observable<GeographyDto[]>;
    public richTextTypeID: number = CustomRichTextTypeEnum.OurGeographies;
    public isAuthenticated: boolean = false;
    public currentUserSubscription: Subscription;

    constructor(
        private publicService: PublicService,
        private router: Router,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.geographies$ = this.publicService.geographiesListPublic();

        this.isAuthenticated = this.authenticationService.isAuthenticated();

        this.currentUserSubscription = this.authenticationService.currentUserSetObservable
            .pipe(
                map((user) => {
                    this.isAuthenticated = user !== null;
                })
            )
            .subscribe();
    }

    ngOnDestroy(): void {
        if (this.currentUserSubscription) {
            this.currentUserSubscription.unsubscribe();
        }
    }

    navigateToGeographyDashboard(geography: GeographyDto) {
        if (this.isAuthenticated) {
            const geographyName = geography.GeographyName.toLowerCase();
            this.router.navigateByUrl(`geographies/${geographyName}`);
        }
    }
}
