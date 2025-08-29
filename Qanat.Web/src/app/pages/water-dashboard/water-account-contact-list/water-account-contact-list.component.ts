import { AsyncPipe, CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { DialogService } from "@ngneat/dialog";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef, GridApi, GridReadyEvent, RowNode } from "ag-grid-community";
import { BehaviorSubject, combineLatest, Observable, of, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { WaterAccountContactUpdateComponent } from "src/app/shared/components/water-account-contact/modals/water-account-contact-update/water-account-contact-update.component";
import { WaterDashboardNavComponent } from "src/app/shared/components/water-dashboard-nav/water-dashboard-nav.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { WaterAccountContactByGeographyService } from "src/app/shared/generated/api/water-account-contact-by-geography.service";
import { WaterAccountContactService } from "src/app/shared/generated/api/water-account-contact.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { WaterAccountContactDto } from "src/app/shared/generated/model/water-account-contact-dto";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "water-account-contact-list",
    standalone: true,
    imports: [
        LoadingDirective,
        WaterDashboardNavComponent,
        PageHeaderComponent,
        AsyncPipe,
        AlertDisplayComponent,
        QanatGridComponent,
        QanatGridHeaderComponent,
        CommonModule,
        FormsModule,
        ModelNameTagComponent,
        NgSelectModule,
    ],
    templateUrl: "./water-account-contact-list.component.html",
    styleUrl: "./water-account-contact-list.component.scss",
})
export class WaterAccountContactListComponent implements OnInit {
    public currentGeography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;
    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public refreshWaterAccountContacts$: BehaviorSubject<null> = new BehaviorSubject(null);
    public waterAccountContacts$: Observable<WaterAccountContactDto[]>;

    public columnDefs: ColDef<WaterAccountContactDto>[];
    public gridApi: GridApi;
    public gridRef: AgGridAngular;

    public isLoading: boolean = true;
    public firstLoad: boolean = true;

    public customRichTextTypeID: number = CustomRichTextTypeEnum.WaterAccountContactList;

    constructor(
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterAccountContactService: WaterAccountContactService,
        private waterAccountContactByGeographyService: WaterAccountContactByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentUserGeographies$ = this.geographyService.listForCurrentUserGeography();
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((currentGeography) => {
                this.currentGeography = currentGeography;
            })
        );

        this.waterAccountContacts$ = combineLatest({ geography: this.currentGeography$, _: this.refreshWaterAccountContacts$ }).pipe(
            tap(() => {
                this.isLoading = true;
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap((data) => {
                return this.waterAccountContactByGeographyService.listByGeographyIDWaterAccountContactByGeography(data.geography.GeographyID);
            }),
            tap((x) => {
                this.isLoading = false;
                this.firstLoad = false;
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", false);
                    setTimeout(() => {
                        this.gridApi.sizeColumnsToFit();
                    }, 1);
                }
            })
        );

        this.columnDefs = this.buildColumnDefs();
    }

    gridReady($event: GridReadyEvent<any, any>) {
        this.gridApi = $event.api;
        setTimeout(() => {
            this.gridApi.sizeColumnsToFit();
        }, 1);
    }

    public openCreateModal() {
        const dialogRef = this.dialogService.open(WaterAccountContactUpdateComponent, {
            data: {
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.refreshWaterAccountContacts$.next(null);
            }
        });
    }

    public openUpdateModal(waterAccountContactID: number, rowNode: RowNode) {
        const dialogRef = this.dialogService.open(WaterAccountContactUpdateComponent, {
            data: {
                WaterAccountContactID: waterAccountContactID,
                GeographyID: this.currentGeography.GeographyID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                rowNode.setData(result);
            }
        });
    }

    public openDeleteModal(waterAccountContactID: number) {
        this.confirmService
            .confirm({
                title: "Delete Water Account Contact",
                message: "Are you sure you want to delete this contact?",
                buttonTextYes: "Delete",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.waterAccountContactService.deleteWaterAccountContact(waterAccountContactID).subscribe({
                        next: () => {
                            this.alertService.pushAlert(new Alert("Water account contact successfully deleted.", AlertContext.Success));
                            this.refreshWaterAccountContacts$.next(null);
                        },
                    });
                }
            });
    }

    buildColumnDefs(): ColDef<WaterAccountContactDto>[] {
        return [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [
                    {
                        ActionName: "Update Info",
                        ActionIcon: "fas fa-info-circle",
                        ActionHandler: () => this.openUpdateModal(params.data.WaterAccountContactID, params.node),
                    },
                ];
                if (params.data.WaterAccounts?.length == 0) {
                    actions.push({
                        ActionName: "Delete",
                        ActionIcon: "fa fa-times-circle text-danger",
                        ActionHandler: () => this.openDeleteModal(params.data.WaterAccountContactID),
                    });
                }
                return actions;
            }),
            this.utilityFunctionsService.createLinkColumnDef("Contact ID", "WaterAccountContactID", "WaterAccountContactID", {
                InRouterLink: "/contacts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "ContactName"),
            this.utilityFunctionsService.createBasicColumnDef("Email", "ContactEmail"),
            this.utilityFunctionsService.createPhoneNumberColumnDef("Phone Number", "ContactPhoneNumber"),
            this.utilityFunctionsService.createBasicColumnDef("Address", "Address"),
            this.utilityFunctionsService.createBasicColumnDef("Secondary Address", "SecondaryAddress"),
            this.utilityFunctionsService.createBasicColumnDef("City", "City"),
            this.utilityFunctionsService.createBasicColumnDef("State", "State"),
            this.utilityFunctionsService.createBasicColumnDef("Zip Code", "ZipCode"),
            this.utilityFunctionsService.createBasicColumnDef("Address Validated?", "", {
                ValueGetter: (params) => (params.data.AddressValidated ? "Yes" : "No"),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Communication Preference", "", {
                ValueGetter: (params) => (params.data.PrefersPhysicalCommunication ? "Physical Mail" : "Email"),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Water Accounts", "WaterAccounts", "WaterAccountID", "WaterAccountNumber", {
                InRouterLink: "/water-accounts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Account Names", "", {
                ValueGetter: (params) => params.data.WaterAccounts.map((account) => account.WaterAccountName).join(", "),
            }),
            this.utilityFunctionsService.createBasicColumnDef("Account PINs", "", {
                ValueGetter: (params) => params.data.WaterAccounts.map((account) => account.WaterAccountPIN).join(", "),
            }),
        ];
    }

    public onGeographySelected(selectedGeography: GeographyMinimalDto) {
        this.currentGeographyService.setCurrentGeography(selectedGeography);
    }
}
