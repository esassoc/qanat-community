import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf } from "@angular/common";

@Component({
    selector: "dashboard-update-parcels",
    templateUrl: "./dashboard-update-parcels.component.html",
    styleUrls: ["./dashboard-update-parcels.component.scss"],
    standalone: true,
    imports: [NgIf, RouterLink, RouterLinkActive, RouterOutlet, PageHeaderComponent],
})
export class DashboardUpdateParcelsComponent implements OnInit, OnDestroy {
    public selectedGeography$ = Subscription.EMPTY;

    private geographyID: number;
    private geographyIDSub: Subscription = Subscription.EMPTY;
    public geography: GeographyDto;

    constructor(
        private geographyService: GeographyService,
        private selectedGeographyService: SelectedGeographyService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.geography = geography;
        });
    }

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }
}
