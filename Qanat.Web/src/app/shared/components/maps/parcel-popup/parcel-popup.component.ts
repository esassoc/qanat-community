import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { RouterModule } from "@angular/router";
import { WaterAccountTitleComponent } from "../../water-account/water-account-title/water-account-title.component";
import { ParcelTitleComponent } from "../../parcel/parcel-title/parcel-title.component";
import { ParcelPopupDto } from "src/app/shared/generated/model/models";
import { ModelNameTagComponent } from "../../name-tag/name-tag.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "parcel-popup",
    standalone: true,
    templateUrl: "./parcel-popup.component.html",
    styleUrls: ["./parcel-popup.component.scss"],
    imports: [CommonModule, WaterAccountTitleComponent, RouterModule, ParcelTitleComponent, ModelNameTagComponent, LoadingDirective],
})
export class ParcelPopupComponent implements OnInit {
    @Input() parcelId: number;
    public isLoading: boolean = true;
    public parcel$: Observable<ParcelPopupDto>;

    constructor(private parcelService: ParcelService) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDMapPopupGet(this.parcelId).pipe(
            tap((x) => {
                this.isLoading = false;
            })
        );
    }
}
