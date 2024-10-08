import { Component, OnInit, ViewContainerRef } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { GeographySimpleDto } from "src/app/shared/generated/model/geography-simple-dto";
import { CommonModule } from "@angular/common";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UpdateParcelsComponent } from "src/app/shared/components/water-account/modals/update-parcels/update-parcels.component";
import { UpdateWaterAccountInfoComponent, WaterAccountContext } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { DeleteWaterAccountComponent } from "src/app/shared/components/water-account/modals/delete-water-account/delete-water-account.component";
import { MergeWaterAccountsComponent } from "src/app/shared/components/water-account/modals/merge-water-accounts/merge-water-accounts.component";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { EntityCustomAttributesDto } from "src/app/shared/generated/model/entity-custom-attributes-dto";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";

@Component({
    selector: "water-admin-panel",
    standalone: true,
    imports: [
        PageHeaderComponent,
        CommonModule,
        LoadingDirective,
        IconComponent,
        KeyValuePairListComponent,
        KeyValuePairComponent,
        FieldDefinitionComponent,
        ModelNameTagComponent,
        RouterLink,
        AlertDisplayComponent,
    ],
    templateUrl: "./water-admin-panel.component.html",
    styleUrl: "./water-admin-panel.component.scss",
})
export class WaterAdminPanelComponent implements OnInit {
    public waterAccountID: number;
    private accountIDSubscription: Subscription = Subscription.EMPTY;
    public currentWaterAccount: WaterAccountDto;
    public currentGeography: GeographySimpleDto;
    public currentGeographySlug: string;
    public isLoading: boolean = false;
    public waterAccountCustomAttributes$: Observable<EntityCustomAttributesDto>;
    allocationPlans: any;

    constructor(
        private route: ActivatedRoute,
        private waterAccountService: WaterAccountService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private customAttributeService: CustomAttributeService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.accountIDSubscription = this.route.paramMap.subscribe((paramMap) => {
            this.waterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
            this.waterAccountCustomAttributes$ = this.customAttributeService.customAttributesWaterAccountsWaterAccountIDGet(this.waterAccountID);
            this.waterAccountService.waterAccountsWaterAccountIDGet(this.waterAccountID).subscribe((waterAccount) => {
                this.isLoading = true;
                this.currentWaterAccount = waterAccount;
                this.currentGeography = waterAccount.Geography;
                this.currentGeographySlug = waterAccount.Geography.GeographyName.replace(" ", "-").toLowerCase();
                this.isLoading = false;
            });
        });
    }

    openUpdateInfoModal(): void {
        this.modalService
            .open(UpdateWaterAccountInfoComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.currentWaterAccount.WaterAccountID,
                GeographyID: this.currentWaterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.currentWaterAccount = result;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully updated Water Account!", AlertContext.Success));
                }
            });
    }

    openMergeModal(): void {
        this.modalService
            .open(MergeWaterAccountsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.currentWaterAccount.WaterAccountID,
                GeographyID: this.currentWaterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.currentWaterAccount = { ...result };
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully merged Water Account!", AlertContext.Success));
                }
            });
    }

    openUpdateParcelsModal(): void {
        this.modalService
            .open(UpdateParcelsComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.currentWaterAccount.WaterAccountID,
                GeographyID: this.currentWaterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.currentWaterAccount = { ...result };
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully updated Parcels!", AlertContext.Success));
                }
            });
    }

    openDeleteModal(): void {
        this.modalService
            .open(DeleteWaterAccountComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountID: this.currentWaterAccount.WaterAccountID,
                GeographyID: this.currentWaterAccount.Geography.GeographyID,
            } as WaterAccountContext)
            .instance.result.then((result) => {
                if (result) {
                    this.router.navigate(["../.."], { relativeTo: this.route }).then(() => {
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("Successfully deleted Water Account!", AlertContext.Success));
                    });
                }
            });
    }
}
