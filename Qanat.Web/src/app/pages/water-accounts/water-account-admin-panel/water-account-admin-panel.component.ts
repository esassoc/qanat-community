import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, map, Observable, shareReplay, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { CommonModule } from "@angular/common";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UpdateParcelsComponent } from "src/app/shared/components/water-account/modals/update-parcels/update-parcels.component";
import { UpdateWaterAccountInfoComponent } from "src/app/shared/components/water-account/modals/update-water-account-info/update-water-account-info.component";
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
import { ParcelWaterAccountHistorySimpleDto, UsageLocationHistoryDto } from "src/app/shared/generated/model/models";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatGridComponent } from "../../../shared/components/qanat-grid/qanat-grid.component";
import { WaterAccountContactUpdateComponent } from "src/app/shared/components/water-account-contact/modals/water-account-contact-update/water-account-contact-update.component";
import { DialogService } from "@ngneat/dialog";
import { UsageLocationHistoryService } from "src/app/shared/generated/api/usage-location-history.service";

@Component({
    selector: "water-account-admin-panel",
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
        QanatGridComponent,
    ],
    templateUrl: "./water-account-admin-panel.component.html",
    styleUrl: "./water-account-admin-panel.component.scss",
})
export class WaterAccountAdminPanelComponent implements OnInit {
    public waterAccount$: Observable<WaterAccountDto>;
    public currentWaterAccount: WaterAccountDto;
    public isLoading: boolean = false;
    public waterAccountCustomAttributes$: Observable<EntityCustomAttributesDto>;

    public parcelWaterAccountHistories$: Observable<ParcelWaterAccountHistorySimpleDto[]>;
    public refreshParcelWaterAccountHistories$: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public usageLocationHistories$: Observable<UsageLocationHistoryDto[]>;

    public parcelWaterAccountHistoriesColumnDefs: ColDef[];
    public parcelWaterAccountHistoriesGridApi: GridApi;

    public usageLocationHistoriesColumnDefs: ColDef[];
    public usageLocationHistoriesGridApi: GridApi;

    allocationPlans: any;

    constructor(
        private route: ActivatedRoute,
        private waterAccountService: WaterAccountService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private usageLocationHistoryService: UsageLocationHistoryService,
        private customAttributeService: CustomAttributeService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private router: Router,
        private dialogService: DialogService
    ) {
        this.parcelWaterAccountHistoriesColumnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriodName"),
            this.utilityFunctionsService.createLinkColumnDef("From Water Account", "FromWaterAccountNumberAndName", "FromWaterAccountID", {
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createLinkColumnDef("To Water Account", "ToWaterAccountNumberAndName", "ToWaterAccountID", {
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Reason", "Reason"),
            this.utilityFunctionsService.createDateColumnDef("Date", "CreateDate", "short"),
            this.utilityFunctionsService.createBasicColumnDef("User", "CreateUserFullName"),
        ];

        this.usageLocationHistoriesColumnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Usage Location", "UsageLocationName"),
            this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationTypeName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "ReportingPeriodName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createDateColumnDef("Date", "CreateDate", "short", { Sort: "desc" }),
            this.utilityFunctionsService.createBasicColumnDef("User", "CreateUserFullName", { UseCustomDropdownFilter: true }),
            this.utilityFunctionsService.createBasicColumnDef("Note", "Note"),
        ];
    }

    ngOnInit(): void {
        this.waterAccount$ = this.route.paramMap.pipe(
            map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
            tap((waterAccountID) => {
                this.isLoading = true;
            }),
            switchMap((waterAccountID) => this.waterAccountService.getByIDWaterAccount(waterAccountID)),
            tap((waterAccount) => {
                this.currentWaterAccount = waterAccount;
                this.isLoading = false;
            }),
            shareReplay()
        );

        this.waterAccountCustomAttributes$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.customAttributeService.listAllWaterAccountCustomAttributesCustomAttribute(waterAccount.WaterAccountID);
            })
        );

        this.parcelWaterAccountHistories$ = combineLatest({ waterAccount: this.waterAccount$, _: this.refreshParcelWaterAccountHistories$ }).pipe(
            switchMap(({ waterAccount: waterAccount }) => {
                return this.waterAccountParcelByWaterAccountService.getWaterAccountParcelHistoryWaterAccountParcelByWaterAccount(waterAccount.WaterAccountID);
            })
        );

        this.usageLocationHistories$ = this.waterAccount$.pipe(
            switchMap((waterAccount) => {
                return this.usageLocationHistoryService.listByWaterAccountUsageLocationHistory(waterAccount.Geography.GeographyID, waterAccount.WaterAccountID);
            })
        );
    }

    openUpdateInfoModal(waterAccount: WaterAccountDto): void {
        const dialogRef = this.dialogService.open(UpdateWaterAccountInfoComponent, {
            data: {
                WaterAccountID: waterAccount.WaterAccountID,
                GeographyID: waterAccount.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.currentWaterAccount = result;
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully updated Water Account!", AlertContext.Success));
            }
        });
    }

    openMergeModal(waterAccount: WaterAccountDto): void {
        const dialogRef = this.dialogService.open(MergeWaterAccountsComponent, {
            data: {
                WaterAccountID: waterAccount.WaterAccountID,
                GeographyID: waterAccount.Geography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.currentWaterAccount = result;
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully merged water accounts.", AlertContext.Success));
            }
        });
    }

    openUpdateParcelsModal(waterAccount: WaterAccountDto): void {
        const dialogRef = this.dialogService.open(UpdateParcelsComponent, {
            data: {
                WaterAccountID: waterAccount.WaterAccountID,
                GeographyID: waterAccount.Geography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result.success) {
                this.refreshParcelWaterAccountHistories$.next();
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully updated Parcels!", AlertContext.Success));
            }
        });
    }

    openDeleteModal(waterAccount: WaterAccountDto): void {
        const dialogRef = this.dialogService.open(DeleteWaterAccountComponent, {
            data: {
                WaterAccountID: waterAccount.WaterAccountID,
                GeographyID: waterAccount.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.router.navigate(["../.."], { relativeTo: this.route }).then(() => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Successfully deleted Water Account!", AlertContext.Success));
                });
            }
        });
    }

    onParcelWaterAccountHistoriesGridReady(event: GridReadyEvent): void {
        this.parcelWaterAccountHistoriesGridApi = event.api;
    }

    onUsageLocationHistoriesGridReady(event: GridReadyEvent): void {
        this.usageLocationHistoriesGridApi = event.api;
    }
}
