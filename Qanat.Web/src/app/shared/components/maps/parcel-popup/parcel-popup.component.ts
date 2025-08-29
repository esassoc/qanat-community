import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { RouterModule } from "@angular/router";
import { ParcelTitleComponent } from "../../parcel/parcel-title/parcel-title.component";
import { ParcelPopupDto } from "src/app/shared/generated/model/models";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "parcel-popup",
    templateUrl: "./parcel-popup.component.html",
    styleUrls: ["./parcel-popup.component.scss"],
    imports: [CommonModule, RouterModule, ParcelTitleComponent, LoadingDirective],
})
export class ParcelPopupComponent implements OnInit {
    @Input() parcelId: number;
    @Input() reportingPeriodId: number;

    public isLoading: boolean = true;
    public parcel$: Observable<ParcelPopupDto>;

    constructor(private parcelService: ParcelService) {}

    ngOnInit(): void {
        this.parcel$ =
            this.reportingPeriodId && (this.reportingPeriodId as any) != "null"
                ? this.parcelService.getParcelPopupDtoByIDAndReportingPeriodParcel(this.parcelId, this.reportingPeriodId).pipe(
                      tap((x) => {
                          this.isLoading = false;
                      })
                  )
                : this.parcelService.getParcelPopupDtoByIDParcel(this.parcelId).pipe(
                      tap((x) => {
                          this.isLoading = false;
                      })
                  );
    }
}
