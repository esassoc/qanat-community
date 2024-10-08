import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, RouterOutlet } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WellDetailDto } from "src/app/shared/generated/model/models";
import { ViewingDetailMenuService } from "src/app/shared/services/viewing-detail-menu.service";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "well-detail-outlet",
    templateUrl: "./well-detail-outlet.component.html",
    styleUrl: "./well-detail-outlet.component.scss",
    standalone: true,
    imports: [NgIf, RouterOutlet, AsyncPipe],
})
export class WellDetailOutletComponent implements OnInit, OnDestroy {
    public well$: Observable<WellDetailDto>;

    constructor(
        private wellService: WellService,
        private route: ActivatedRoute,
        private viewingDetailMenuService: ViewingDetailMenuService
    ) {}

    ngOnInit(): void {
        const wellID = parseInt(this.route.snapshot.paramMap.get(routeParams.wellID));
        this.well$ = this.wellService.wellsWellIDDetailsGet(wellID).pipe(
            tap((well) => {
                this.viewingDetailMenuService.loadedWell(well);
            })
        );
    }

    ngOnDestroy(): void {
        this.viewingDetailMenuService.unLoadedWell();
    }
}
