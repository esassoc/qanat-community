import { Component, Input, OnInit } from "@angular/core";
import { tap } from "rxjs/operators";
import { CommonModule } from "@angular/common";
import { WellService } from "src/app/shared/generated/api/well.service";
import { Observable } from "rxjs";
import { RouterModule } from "@angular/router";
import { WellPopupDto } from "src/app/shared/generated/model/well-popup-dto";
import { ParcelTitleComponent } from "../../parcel/parcel-title/parcel-title.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "well-popup",
    standalone: true,
    imports: [CommonModule, RouterModule, ParcelTitleComponent, IconComponent, LoadingDirective],
    templateUrl: "./well-popup.component.html",
    styleUrls: ["./well-popup.component.scss"],
})
export class WellPopupComponent implements OnInit {
    @Input() wellId: number;
    public isLoading: boolean = true;
    public well$: Observable<WellPopupDto>;

    constructor(private wellService: WellService) {}

    ngOnInit(): void {
        this.well$ = this.wellService.wellsWellIDPopupGet(this.wellId).pipe(
            tap((x) => {
                this.isLoading = false;
            })
        );
    }

    getGeographyLowerCase(well: WellPopupDto) {
        return well.Geography.GeographyName.toLowerCase();
    }
}
