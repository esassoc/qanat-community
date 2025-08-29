import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Observable, tap } from "rxjs";
import { RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CreateUsageLocationsFromParcelsModalComponent } from "./create-usage-locations-from-parcels-modal/create-usage-locations-from-parcels-modal.component";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "usage-location-supply-and-usage-menu",
    imports: [PageHeaderComponent, AlertDisplayComponent, RouterLink, AsyncPipe],
    templateUrl: "./usage-location-supply-and-usage-menu.component.html",
    styleUrl: "./usage-location-supply-and-usage-menu.component.scss",
})
export class UsageLocationSupplyAndUsageMenuComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;

    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public richTextTypeID = CustomRichTextTypeEnum.UsageLocationSupplyAndUsageMenu;

    constructor(
        private authenticationService: AuthenticationService,
        private currentGeographyService: CurrentGeographyService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
            })
        );
    }

    openCreateUsageLocationsFromParcelModal(geography: GeographyMinimalDto): void {
        const dialogRef = this.dialogService.open(CreateUsageLocationsFromParcelsModalComponent, {
            data: {
                Geography: geography,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
            }
        });
    }
}
