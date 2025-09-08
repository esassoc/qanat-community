import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { routeParams } from "src/app/app.routes";
import { ParcelSupplyUpsertDto } from "src/app/shared/generated/model/parcel-supply-upsert-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Observable, switchMap, tap } from "rxjs";
import { ParcelMinimalDto, UserDto, GeographyMinimalDto, WaterTypeSimpleDto } from "src/app/shared/generated/model/models";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { AsyncPipe } from "@angular/common";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { ParcelTypeaheadComponent } from "src/app/shared/components/parcel/parcel-typeahead/parcel-typeahead.component";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "water-transactions-create",
    templateUrl: "./water-transactions-create.component.html",
    styleUrls: ["./water-transactions-create.component.scss"],
    imports: [AsyncPipe, PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ParcelTypeaheadComponent, NgSelectModule, FieldDefinitionComponent, ButtonComponent]
})
export class WaterTransactionsCreateComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;

    public currentUser$: Observable<UserDto>;
    public waterTypes$: Observable<WaterTypeSimpleDto[]>;

    public selectedParcel: ParcelMinimalDto;
    public model: ParcelSupplyUpsertDto;

    public isLoadingSubmit: boolean = false;
    public richTextTypeID: number = CustomRichTextTypeEnum.WaterTransactionCreate;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private parcelService: ParcelService,
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private alertService: AlertService,
        private currentGeographyService: CurrentGeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                // Reset the model when the geography changes
                this.model = new ParcelSupplyUpsertDto();
                this.model.ParcelIDs = new Array<number>();

                const id = parseInt(this.route.snapshot.paramMap.get(routeParams.parcelID));
                if (id) {
                    this.parcelService.getByIDParcel(id).subscribe((parcel) => {
                        this.selectedParcel = parcel;
                    });
                } else {
                    this.selectedParcel = new ParcelMinimalDto();
                }
            })
        );

        this.waterTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterTypeByGeographyService.getActiveWaterTypesWaterTypeByGeography(geography.GeographyID);
            })
        );
    }

    public onSelectedParcelChanged(selectedParcel: ParcelMinimalDto) {
        this.selectedParcel = selectedParcel;
    }

    public onSubmit(geography: GeographyMinimalDto): void {
        if (!this.selectedParcel?.ParcelID) {
            this.alertService.pushAlert(new Alert("The APN field is required.", AlertContext.Danger));
            return;
        }

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();
        this.model.ParcelIDs.push(this.selectedParcel.ParcelID);

        this.parcelSupplyByGeographyService.newParcelSupplyByGeography(geography.GeographyID, this.model).subscribe({
            next: () => {
                this.router.navigate(["../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
                });
            },
            error: () => {
                this.alertService.pushAlert(new Alert("An error occurred while creating the transaction.", AlertContext.Danger));
            },
            complete: () => (this.isLoadingSubmit = false),
        });
    }
}
