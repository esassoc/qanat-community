import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef } from "ag-grid-community";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ParcelSupplyService } from "src/app/shared/generated/api/parcel-supply.service";
import { Subscription } from "rxjs";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";

@Component({
    selector: "water-transactions",
    templateUrl: "./water-transactions.component.html",
    styleUrls: ["./water-transactions.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, ButtonComponent, RouterLink, CustomRichTextComponent, QanatGridComponent],
})
export class WaterTransactionsComponent implements OnInit, OnDestroy {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    private geographyID: number;

    @ViewChild("transactionHistoryGrid") transactionHistoryGrid: AgGridAngular;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    private currentUser: UserDto;

    public transactionHistory;
    public columnDefs: Array<ColDef>;

    public richTextTypeID = CustomRichTextTypeEnum.WaterTransactions;
    public historyRichTextTypeID = CustomRichTextTypeEnum.WaterTransactionHistory;

    constructor(
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private utilityFunctionsService: UtilityFunctionsService,
        private ParcelSupplyService: ParcelSupplyService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    private getDataForGeographyID(geographyID: number): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
            this.createTransactionHistoryGridColumnDefs();

            this.ParcelSupplyService.geographiesGeographyIDParcelSuppliesTransactionHistoryGet(geographyID).subscribe((transactionHistory) => {
                this.transactionHistory = transactionHistory;
            });

            this.cdr.detectChanges();
        });
    }

    public canCreateTransactions(): boolean {
        const hasGeographyPermission = this.authenticationService.hasGeographyPermission(
            this.currentUser,
            PermissionEnum.WaterTransactionRights,
            RightsEnum.Create,
            this.geographyID
        );
        const hasSystemPermission = this.authenticationService.hasPermission(this.currentUser, PermissionEnum.WaterTransactionRights, RightsEnum.Create);

        return hasGeographyPermission || hasSystemPermission;
    }

    public createTransactionHistoryGridColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createDateColumnDef("EffectiveDate", "EffectiveDate", "M/d/yyyy", {
                IgnoreLocalTimezone: true,
                FieldDefinitionType: "EffectiveDate",
            }),
            this.utilityFunctionsService.createDateColumnDef("Transaction Date", "TransactionDate", "short", {
                Sort: "desc",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Created By", "CreateUserFullName"),
            this.utilityFunctionsService.createBasicColumnDef("Supply Type", "WaterTypeName", {
                FieldDefinitionType: "SupplyType",
                CustomDropdownFilterField: "WaterTypeName",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Total Parcels Affected", "AffectedParcelsCount", { DecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("Total Acres Affected", "AffectedAcresCount", { DecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Depth (ac-ft/ac)", "TransactionDepth"),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Volume (ac-ft)", "TransactionVolume"),
            this.utilityFunctionsService.createBasicColumnDef("Spreadsheet Data Source", "UploadedFileName"),
        ];
    }
}
