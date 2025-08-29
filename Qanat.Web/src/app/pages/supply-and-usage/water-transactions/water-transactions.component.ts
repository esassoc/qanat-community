import { Component, OnInit, ViewChild } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef } from "ag-grid-community";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { combineLatest, Observable, of, switchMap } from "rxjs";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto, TransactionHistoryDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "water-transactions",
    templateUrl: "./water-transactions.component.html",
    styleUrls: ["./water-transactions.component.scss"],
    imports: [PageHeaderComponent, AsyncPipe, AlertDisplayComponent, RouterLink, CustomRichTextComponent, QanatGridComponent],
})
export class WaterTransactionsComponent implements OnInit {
    @ViewChild("transactionHistoryGrid") transactionHistoryGrid: AgGridAngular;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public geography$: Observable<GeographyMinimalDto>;
    public currentUser$: Observable<UserDto>;
    public transactionHistory$: Observable<TransactionHistoryDto[]>;

    public canCreateTransaction$: Observable<boolean>;

    public columnDefs: Array<ColDef>;

    public richTextTypeID = CustomRichTextTypeEnum.WaterTransactions;
    public historyRichTextTypeID = CustomRichTextTypeEnum.WaterTransactionHistory;

    constructor(
        private authenticationService: AuthenticationService,
        private utilityFunctionsService: UtilityFunctionsService,
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private currentGeographyService: CurrentGeographyService
    ) {
        this.createTransactionHistoryGridColumnDefs();
    }

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.currentUser$ = this.authenticationService.getCurrentUser();
        this.transactionHistory$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.parcelSupplyByGeographyService.listTransactionHistoryParcelSupplyByGeography(geography.GeographyID);
            })
        );

        this.canCreateTransaction$ = combineLatest({ geography: this.geography$, currentUser: this.currentUser$ }).pipe(
            switchMap(({ geography, currentUser }) => {
                const hasSystemPermission = this.authenticationService.hasPermission(currentUser, PermissionEnum.WaterTransactionRights, RightsEnum.Create);
                const hasGeographyPermission = this.authenticationService.hasGeographyPermission(
                    currentUser,
                    PermissionEnum.WaterTransactionRights,
                    RightsEnum.Create,
                    geography.GeographyID
                );

                const canCreateTransaction = hasSystemPermission || hasGeographyPermission;
                return of(canCreateTransaction);
            })
        );
    }

    private createTransactionHistoryGridColumnDefs() {
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
            this.utilityFunctionsService.createDecimalColumnDef("Total Parcels Affected", "AffectedParcelsCount", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("Total Acres Affected", "AffectedAcresCount", { MaxDecimalPlacesToDisplay: 0 }),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Depth (ac-ft/ac)", "TransactionDepth"),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Volume (ac-ft)", "TransactionVolume"),
            this.utilityFunctionsService.createBasicColumnDef("Spreadsheet Data Source", "UploadedFileName"),
        ];
    }
}
