import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable, tap } from "rxjs";
import { WaterAccountTitleComponent } from "../../water-account/water-account-title/water-account-title.component";
import { RouterModule } from "@angular/router";
import { UsageEntityService } from "src/app/shared/generated/api/usage-entity.service";
import { UsageEntityPopupDto } from "src/app/shared/generated/model/usage-entity-popup-dto";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "usage-entity-popup",
    standalone: true,
    imports: [CommonModule, WaterAccountTitleComponent, RouterModule, LoadingDirective],
    templateUrl: "./usage-entity-popup.component.html",
    styleUrls: ["./usage-entity-popup.component.scss"],
})
export class UsageEntityPopupComponent implements OnInit {
    @Input() usageEntityId: number;
    public isLoading: boolean = true;
    public data$: Observable<UsageEntityPopupDto>;

    constructor(private usageEntityService: UsageEntityService) {}

    ngOnInit(): void {
        this.data$ = this.usageEntityService.usageEntitiesUsageEntityIDGet(this.usageEntityId).pipe(
            tap((x) => {
                this.isLoading = false;
            })
        );
    }
}
