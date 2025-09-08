import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { forkJoin, Subscription } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { DatePipe, DecimalPipe, NgClass } from "@angular/common";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ColDef } from "ag-grid-community";
import { ParcelActivityDto } from "src/app/shared/generated/model/parcel-activity-dto";
import { ParcelSupplyDetailDto } from "src/app/shared/generated/model/parcel-supply-detail-dto";
import { AllocationPlanMinimalDto, ParcelMinimalDto, ReportingPeriodDto, WaterAccountDto, WaterMeasurementTypeSimpleDto } from "src/app/shared/generated/model/models";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ButtonGroupComponent } from "src/app/shared/components/button-group/button-group.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";
import { ExpandCollapseDirective } from "src/app/shared/directives/expand-collapse.directive";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";

@Component({
    selector: "account-activity",
    templateUrl: "./account-activity.component.html",
    styleUrls: ["./account-activity.component.scss"],
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        ReportingPeriodSelectComponent,
        ButtonGroupComponent,
        NgClass,
        ExpandCollapseDirective,
        QanatGridComponent,
        DecimalPipe,
        DatePipe,
        LoadingDirective,
    ]
})
export class AccountActivityComponent implements OnInit, OnDestroy {
    private waterAccountID: number;
    private waterAccountIDSubscription: Subscription = Subscription.EMPTY;

    public waterAccount: WaterAccountDto;
    public geographyID: number;
    public selectedYear: number;
    public currentBalance: number;
    public totalAcreage: number;
    public mostRecentTransaction: ParcelActivityDto;
    public showAcresFeet: boolean = false;
    public acresFeetUnits: string = "ac-ft";
    public acresFeetAcreUnits: string = "ac-ft/ac";
    public columnDefs: Array<ColDef>;
    public showGrid: boolean = false;
    public parcelSuppliesBalance: Map<string, number>;
    public parcels: ParcelMinimalDto[];
    public sourceOfRecordWaterMeasurementType: WaterMeasurementTypeSimpleDto;
    public allocationPlans: AllocationPlanMinimalDto[];

    public transactions: ParcelActivityDto[];
    public parcelSupplyDtos: ParcelSupplyDetailDto[];
    public isLoading: boolean = true;

    constructor(
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private waterAccountService: WaterAccountService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private decimalPipe: DecimalPipe,
        private datePipe: DatePipe,
        private route: ActivatedRoute,
        private waterMeasurementTypeService: WaterMeasurementTypeService
    ) {}

    ngOnDestroy() {
        this.waterAccountIDSubscription.unsubscribe();
    }

    ngOnInit(): void {
        this.waterAccountIDSubscription = this.route.paramMap.subscribe((paramMap) => {
            this.isLoading = true;
            this.waterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
            this.waterAccountService.getByIDWaterAccount(this.waterAccountID).subscribe((waterAccount) => {
                this.waterAccount = waterAccount;
                this.geographyID = waterAccount.Geography.GeographyID;
                forkJoin({
                    sourceOfRecordWaterMeasurementType: this.waterMeasurementTypeService.getSourceOfRecordWaterMeasurementTypeWaterMeasurementType(this.geographyID),
                    allocationPlans: this.waterAccountService.getAccountAllocationPlansByAccountIDWaterAccount(this.waterAccountID),
                }).subscribe(({ sourceOfRecordWaterMeasurementType, allocationPlans }) => {
                    this.allocationPlans = allocationPlans;
                    this.sourceOfRecordWaterMeasurementType = sourceOfRecordWaterMeasurementType;
                    this.isLoading = false;
                    // forkJoin({
                    //     parcels: this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsWaterAccountIDParcelsYearsYearGet(
                    //         this.geographyID,
                    //         this.waterAccount.WaterAccountID,
                    //         this.selectedYear
                    //     ),
                    //     transactions: this.parcelSupplyByGeographyService.geographiesGeographyIDParcelSuppliesWaterAccountsWaterAccountIDTransactionsYearYearGet(
                    //         this.geographyID,
                    //         this.waterAccountID,
                    //         this.selectedYear
                    //     ),
                    // }).subscribe(({ parcels, transactions }) => {
                    //     this.transactions = transactions;
                    //     this.setCurrentBalance();
                    //     this.setTotalAcreage(parcels);
                    //     this.parcels = parcels;
                    //     this.setMostRecentTransaction();
                    //     this.createTransactionHistoryGridColumnDefs();
                    //     this.parcelSuppliesBalance = this.createParcelSuppliesBalance();
                    //     this.setParcelSupplies(transactions);
                    //     this.isLoading = false;
                    // });
                });
            });
        });
    }

    setParcelSupplies(transactions: ParcelActivityDto[]) {
        this.parcelSupplyDtos = [];
        transactions.forEach((transaction) => {
            this.parcelSupplyDtos = this.parcelSupplyDtos.concat(transaction.ParcelSupplies);
        });
    }

    setCurrentBalance() {
        this.currentBalance = 0;
        this.transactions.forEach((transaction) => {
            this.currentBalance += transaction.TransactionAmount;
        });
    }

    setTotalAcreage(parcels: ParcelMinimalDto[]) {
        this.totalAcreage = 0;
        parcels.forEach((parcel, i) => {
            if (parcels.findIndex((x) => x.ParcelID == parcel.ParcelID) == i) {
                this.totalAcreage += parcel.ParcelArea;
            }
        });
    }

    setMostRecentTransaction() {
        if (this.transactions.length > 0) {
            this.mostRecentTransaction = this.transactions[0];
        } else {
            this.mostRecentTransaction = null;
        }
    }

    getMostRecentTransactionAmount() {
        if (this.mostRecentTransaction == null || this.mostRecentTransaction == undefined) {
            return 0;
        }
        return this.mostRecentTransaction.TransactionAmount;
    }

    getMostRecentTransactionEffectiveDate() {
        if (this.mostRecentTransaction == null) {
            return 0;
        }
        const _datePipe = this.datePipe;
        if (this.mostRecentTransaction.EffectiveDate != null) {
            return _datePipe.transform(this.mostRecentTransaction.EffectiveDate, "mediumDate", "+0000");
        }
    }

    public createTransactionHistoryGridColumnDefs() {
        const _decimalPipe = this.decimalPipe;

        this.columnDefs = [
            { headerName: "APN", field: "Parcel.ParcelNumber", valueGetter: (params) => params.data.Parcel.ParcelNumber },
            this.utilityFunctionsService.createDateColumnDef("EffectiveDate", "EffectiveDate", "M/d/yyyy", { FieldDefinitionType: "EffectiveDate" }),
            this.utilityFunctionsService.createDateColumnDef("Transaction Date", "TransactionDate", "short"),
            this.utilityFunctionsService.createBasicColumnDef("Supply Type", "WaterType.WaterTypeName", {
                FieldDefinitionType: "SupplyType",
                CustomDropdownFilterField: "WaterType.WaterTypeName",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Volume (ac-ft)", "TransactionAmount"),
            this.utilityFunctionsService.createDecimalColumnDef("Transaction Depth (ac-ft/ac)", "TransactionAmount", {
                ValueGetter: (params) =>
                    params.data.TransactionAmount != null && params.data.Parcel?.ParcelArea > 0
                        ? _decimalPipe.transform(params.data.TransactionAmount / params.data.Parcel.ParcelArea, "1.2-2")
                        : null,
            }),
        ];
    }

    onSelectedReportingPeriodChange(selectedReportingPeriod: ReportingPeriodDto) {
        let endDate = new Date(selectedReportingPeriod.EndDate);
        this.selectedYear = endDate.getUTCFullYear();
        forkJoin({
            parcels: this.waterAccountByGeographyService.listParcelsByWaterAccountIDWaterAccountByGeography(this.geographyID, this.waterAccount.WaterAccountID, this.selectedYear),
            transactions: this.parcelSupplyByGeographyService.getTransactionsFromAccountIDParcelSupplyByGeography(this.geographyID, this.waterAccountID, this.selectedYear),
        }).subscribe(({ parcels, transactions }) => {
            this.transactions = transactions;
            this.setCurrentBalance();
            this.setTotalAcreage(parcels);
            this.parcels = parcels;
            this.setMostRecentTransaction();
            this.createTransactionHistoryGridColumnDefs();
            this.parcelSuppliesBalance = this.createParcelSuppliesBalance();
            this.setParcelSupplies(transactions);
        });
    }

    private createParcelSuppliesBalance(): Map<string, number> {
        const map = new Map();
        let currentBalance = this.transactions.reduce((a, b) => {
            return a + b.TransactionAmount;
        }, 0);

        for (const parcelSupply of this.transactions) {
            map.set(parcelSupply.ParcelActivityKey, currentBalance);
            currentBalance -= parcelSupply.TransactionAmount;
        }

        return map;
    }

    public isInitialEstimate(transaction: ParcelActivityDto, parcelID: number, index: number): boolean {
        return transaction.ParcelSupplies.findIndex((x) => x.ParcelID == parcelID) == index;
    }

    getMostRecentTransactionParcelArea() {
        if (this.mostRecentTransaction == null || this.mostRecentTransaction == undefined) {
            return 0;
        }
        return this.mostRecentTransaction.ParcelArea;
    }

    convertToAcresFeetAcreTotalAcreage(num) {
        return num / this.totalAcreage;
    }

    changeShowGrid(showGrid) {
        this.showGrid = showGrid;
    }

    changeUnits(temp) {
        this.showAcresFeet = temp;
    }

    getShowAcresFeet() {
        return this.showAcresFeet;
    }

    getTransactionAmountToDisplay(parcelSupply: ParcelSupplyDetailDto) {
        let transactionAmount = parcelSupply.TransactionAmount;

        if (!this.showAcresFeet) {
            transactionAmount /= parcelSupply.Parcel.ParcelArea;
        }

        return transactionAmount;
    }
}
