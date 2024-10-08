import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountDto } from "src/app/shared/generated/model/models";
import { Observable, forkJoin } from "rxjs";
import { tap } from "rxjs/operators";
import { WaterAccountTitleComponent } from "../../water-account/water-account-title/water-account-title.component";
import { RouterModule } from "@angular/router";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ModelNameTagComponent } from "../../name-tag/name-tag.component";

@Component({
    selector: "water-account-popup",
    standalone: true,
    imports: [CommonModule, WaterAccountTitleComponent, RouterModule, LoadingDirective, ModelNameTagComponent],
    templateUrl: "./water-account-popup.component.html",
    styleUrls: ["./water-account-popup.component.scss"],
})
export class WaterAccountPopupComponent implements OnInit {
    @Input() waterAccountId: number;
    public isLoading: boolean = true;

    public data$: Observable<any>;
    public waterAccount: WaterAccountDto;
    public waterAccountLink = [];

    constructor(private waterAccountService: WaterAccountService) {}

    ngOnInit(): void {
        this.data$ = forkJoin([this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountId)]).pipe(
            tap((x) => {
                this.waterAccount = x[0];
                this.isLoading = false;

                this.waterAccountLink = ["water-accounts", this.waterAccount.WaterAccountID];
            })
        );
    }
}
