import { Component, ChangeDetectorRef, OnInit } from "@angular/core";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Observable, tap, share, BehaviorSubject, combineLatest, switchMap } from "rxjs";
import { WaterAccountContactDto } from "src/app/shared/generated/model/water-account-contact-dto";
import { WaterAccountContactByGeographyService } from "src/app/shared/generated/api/water-account-contact-by-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { BatchValidateWaterAccountContactAddressRequestDto } from "src/app/shared/generated/model/batch-validate-water-account-contact-address-request-dto";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AsyncPipe } from "@angular/common";

@Component({
    selector: "water-account-contact-bulk-actions",
    templateUrl: "./water-account-contact-bulk-actions.component.html",
    styleUrls: ["./water-account-contact-bulk-actions.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, AsyncPipe],
})
export class WaterAccountContactBulkActionsComponent implements OnInit {
    public currentGeography$: Observable<GeographyMinimalDto>;
    public currentGeographyID: number;

    public refreshWaterAccountContacts$: BehaviorSubject<null> = new BehaviorSubject(null);
    public waterAccountContacts$: Observable<WaterAccountContactDto[]>;
    public selectedWaterAccountContactIDs: number[] = [];

    public columnDefs: ColDef<WaterAccountContactDto>[];
    public gridApi: GridApi;

    public isLoadingSubmit: boolean = false;
    public customRichTextTypeID: number = CustomRichTextTypeEnum.WaterAccountContactList;

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private waterAccountContactByGeographyService: WaterAccountContactByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.currentGeographyID = geography.GeographyID;
            })
        );

        this.waterAccountContacts$ = combineLatest({ geography: this.currentGeography$, _: this.refreshWaterAccountContacts$ }).pipe(
            tap(() => {
                if (this.gridApi) {
                    this.gridApi.setGridOption("loading", true);
                }
            }),
            switchMap((data) => {
                return this.waterAccountContactByGeographyService.listByGeographyIDWaterAccountContactByGeography(data.geography.GeographyID);
            }),
            tap((x) => {
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

    public onGridReady($event: GridReadyEvent<any, any>) {
        this.gridApi = $event.api;
        setTimeout(() => {
            this.gridApi.sizeColumnsToFit();
        }, 1);
    }

    public onSelectionChanged() {
        this.selectedWaterAccountContactIDs = this.gridApi.getSelectedNodes().map((x) => x.data.WaterAccountContactID);
    }

    public validateAddresses() {
        this.confirmService
            .confirm({
                title: "Batch Validate Addresses",
                message: `Are you sure you want to validate the addresses for the ${this.selectedWaterAccountContactIDs.length} contacts you've selected?`,
                buttonTextYes: "Validate Addresses",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.isLoadingSubmit = true;

                    var requestDto = new BatchValidateWaterAccountContactAddressRequestDto();
                    requestDto.WaterAccountContactIDs = this.selectedWaterAccountContactIDs;

                    this.waterAccountContactByGeographyService.batchValidateAddressesWaterAccountContactByGeography(this.currentGeographyID, requestDto).subscribe({
                        next: () => {
                            this.isLoadingSubmit = false;
                            this.alertService.pushAlert(
                                new Alert(
                                    "Address validation completed successfully. All addresses with an exact or high-confidence match have been updated.",
                                    AlertContext.Success
                                )
                            );
                            this.refreshWaterAccountContacts$.next(null);
                        },
                        error: () => {
                            this.isLoadingSubmit = false;
                            this.alertService.pushAlert(new Alert("An error occurred while validating addresses.", AlertContext.Danger));
                        },
                    });
                }
            });
    }

    buildColumnDefs(): ColDef<WaterAccountContactDto>[] {
        return [
            this.utilityFunctionsService.createLinkColumnDef("Contact ID", "WaterAccountContactID", "WaterAccountContactID", {
                InRouterLink: "/contacts/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Contact Name", "ContactName"),
            this.utilityFunctionsService.createBasicColumnDef("Email", "ContactEmail"),
            this.utilityFunctionsService.createBasicColumnDef("Phone Number", "ContactPhoneNumber"),
            this.utilityFunctionsService.createBasicColumnDef("Address", "FullAddress"),
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
}
