import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { routeParams } from "src/app/app.routes";
import { ParcelSupplyUpsertDto } from "src/app/shared/generated/model/parcel-supply-upsert-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Subscription } from "rxjs";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { ParcelMinimalDto, UserDto, WaterTypeSimpleDto } from "src/app/shared/generated/model/models";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelSupplyService } from "src/app/shared/generated/api/parcel-supply.service";
import { WaterTypeService } from "src/app/shared/generated/api/water-type.service";
import { NgIf } from "@angular/common";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { ParcelTypeaheadComponent } from "src/app/shared/components/parcel-typeahead/parcel-typeahead.component";

@Component({
    selector: "water-transactions-create",
    templateUrl: "./water-transactions-create.component.html",
    styleUrls: ["./water-transactions-create.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ParcelTypeaheadComponent, NgSelectModule, FieldDefinitionComponent, ButtonComponent, NgIf],
})
export class WaterTransactionsCreateComponent implements OnInit {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    public geographyID: number;

    public currentUser: UserDto;

    public selectedParcel: ParcelMinimalDto;
    public waterTypes: WaterTypeSimpleDto[];
    public model: ParcelSupplyUpsertDto;

    public isLoadingSubmit: boolean = false;
    public richTextTypeID: number = CustomRichTextTypeEnum.WaterTransactionCreate;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private parcelService: ParcelService,
        private ParcelSupplyService: ParcelSupplyService,
        private waterTypeService: WaterTypeService,
        private alertService: AlertService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    private getDataForGeographyID(geographyID: number): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;

            this.model = new ParcelSupplyUpsertDto();
            this.model.ParcelIDs = new Array<number>();

            const id = parseInt(this.route.snapshot.paramMap.get(routeParams.parcelID));
            if (id) {
                this.parcelService.parcelsParcelIDGet(id).subscribe((parcel) => {
                    this.selectedParcel = parcel;
                });
            } else {
                this.selectedParcel = new ParcelMinimalDto();
            }

            this.waterTypeService.geographiesGeographyIDWaterTypesActiveGet(geographyID).subscribe((waterTypes) => {
                this.waterTypes = waterTypes;
            });
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    public onSelectedParcelChanged(selectedParcel: ParcelMinimalDto) {
        this.selectedParcel = selectedParcel;
    }

    public onSubmit(): void {
        if (!this.selectedParcel?.ParcelID) {
            this.alertService.pushAlert(new Alert("The APN field is required.", AlertContext.Danger));
            return;
        }

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();
        this.model.ParcelIDs.push(this.selectedParcel.ParcelID);

        this.ParcelSupplyService.geographiesGeographyIDParcelSuppliesPost(this.geographyID, this.model).subscribe({
            next: () => {
                this.router.navigate(["../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
                });
            },
            error: () => {
                this.cdr.detectChanges();
            },
            complete: () => (this.isLoadingSubmit = false),
        });
    }
}
