import { Component, input, Input, OnInit } from "@angular/core";
import { CommonModule, JsonPipe } from "@angular/common";
import { Observable, tap } from "rxjs";
import { RouterModule } from "@angular/router";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UsageLocationService } from "src/app/shared/generated/api/usage-location.service";
import { UsageLocationDto } from "src/app/shared/generated/model/usage-location-dto";

@Component({
    selector: "usage-location-popup",
    imports: [CommonModule, RouterModule, LoadingDirective],
    templateUrl: "./usage-location-popup.component.html",
    styleUrls: ["./usage-location-popup.component.scss"]
})
export class UsageLocationPopupComponent implements OnInit {
    //MK 2/20/2025: These IDs need to be Id, because of the way leaflet passes them in
    @Input() geographyId: number;
    @Input() parcelId: number;
    @Input() usageLocationId: number;

    public isLoading: boolean = true;
    public usageLocation$: Observable<UsageLocationDto>;

    constructor(private usageLocationService: UsageLocationService) {}

    ngOnInit(): void {
        this.usageLocation$ = this.usageLocationService.getUsageLocation(this.geographyId, this.parcelId, this.usageLocationId).pipe(
            tap((x) => {
                this.isLoading = false;
            })
        );
    }
}
