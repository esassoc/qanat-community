import { Component, OnDestroy, OnInit, ViewContainerRef } from "@angular/core";
import { ColDef, FilterChangedEvent, GridApi, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { Observable, Subscription, switchMap, tap } from "rxjs";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ConfirmModalComponent } from "src/app/shared/components/confirm-modal/confirm-modal.component";
import {
    BulkApproveWaterAccountSuggestionComponent,
    BulkApproveWaterAccountSuggestionContext,
} from "src/app/shared/components/water-account-suggestion/modals/bulk-approve-water-account-suggestion/bulk-approve-water-account-suggestion.component";
import {
    ReviewWaterAccountSuggestionComponent,
    WaterAccountSuggestionContext,
} from "src/app/shared/components/water-account-suggestion/modals/review-water-account-suggestion/review-water-account-suggestion.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { CreateWaterAccountFromSuggestionDto } from "src/app/shared/generated/model/create-water-account-from-suggestion-dto";
import { WaterAccountSuggestionDto } from "src/app/shared/generated/model/water-account-suggestion-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { NgIf, DecimalPipe, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "water-account-suggestions",
    templateUrl: "./water-account-suggestions.component.html",
    styleUrls: ["./water-account-suggestions.component.scss"],
    standalone: true,
    imports: [AsyncPipe, PageHeaderComponent, LoadingDirective, AlertDisplayComponent, NgIf, QanatGridComponent, ButtonLoadingDirective, DecimalPipe, RouterLink],
})
export class WaterAccountSuggestionsComponent implements OnInit, OnDestroy {
    public customRichTextTypeID: number = CustomRichTextTypeEnum.WaterAccountSuggestions;

    public geography$: Observable<GeographyMinimalDto>;
    public geographyID: number;
    public geographyName: string;

    public waterAccountSuggestions: Array<WaterAccountSuggestionDto>;
    public columnDefs: ColDef[];
    public colIDsToExclude = ["0", "1"];
    public gridApi: GridApi;
    public isLoadingSubmit: boolean = false;

    public selectedRows: any = [];
    public filteredRowCount: number;
    public isLoadingSuggestions = true;
    public reloadSubscriptions: Subscription[] = [];

    constructor(
        private route: ActivatedRoute,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
                this.geographyName = geography.GeographyName;
                this.reloadSuggestions();
            })
        );

        this.createColumnDefs();
    }

    ngOnDestroy(): void {
        this.reloadSubscriptions.forEach((x) => x.unsubscribe());
    }

    private reloadSuggestions() {
        const reloadSubscription = this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsSuggestedGet(this.geographyID).subscribe((response) => {
            this.waterAccountSuggestions = response;
            this.isLoadingSuggestions = false;
        });

        this.reloadSubscriptions.push(reloadSubscription);
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createCheckboxSelectionColumnDef(),
            this.utilityFunctionsService.createActionsColumnDef((params) => {
                return [
                    {
                        ActionName: "Review",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () =>
                            this.reviewSuggestionModal(
                                params.data.WaterAccountNumber,
                                params.data.WaterAccountName,
                                params.data.ParcelIDList,
                                params.data.WellIDList,
                                params.data.ContactName,
                                params.data.ContactAddress
                            ),
                    },
                ];
            }),
            { headerName: "Suggested Description", field: "WaterAccountName" },
            this.utilityFunctionsService.createMultiLinkColumnDef("APN List", "Parcels", "ParcelID", "LinkDisplay", {
                MaxWidth: 300,
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Well ID List", "WellIDs", "WellID", "WellID", {
                InRouterLink: "/wells",
                MaxWidth: 300,
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (Acres)", "ParcelArea"),
            { headerName: "Allocation Zones", field: "Zones" },
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public getSelectedRows(event: SelectionChangedEvent | FilterChangedEvent) {
        const selectedFilteredSortedRows = [];
        event.api.forEachNodeAfterFilterAndSort((node) => {
            if (node.isSelected()) {
                selectedFilteredSortedRows.push(node.data.ParcelIDList);
            }
        });

        this.selectedRows = selectedFilteredSortedRows;
    }

    public bulkReject() {
        const selectedRowsCount = this.selectedRows.length;
        this.modalService
            .open(ConfirmModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                title: "Reject Selected Suggestions",
                message: `Are you sure you want to reject ${selectedRowsCount} water account${
                    selectedRowsCount == 1 ? "" : "s"
                }? All parcels associated with these accounts will be excluded from future recommendations. You will need to remove these parcels from the exclusion list if you want to later add them to a Water Account`,
                buttonTextYes: "Reject Accounts",
                buttonClassYes: "btn-danger",
                buttonTextNo: "Cancel",
            } as ConfirmOptions)
            .instance.result.then((result) => {
                if (result) {
                    this.isLoadingSubmit = true;
                    const parcelIDList = this.selectedRows.flatMap((x) => x.split(",").map((y) => parseInt(y)));
                    this.waterAccountByGeographyService.geographiesGeographyIDWaterAccountsSuggestedRejectPost(this.geographyID, parcelIDList).subscribe(
                        () => {
                            this.isLoadingSubmit = false;
                            this.alertService.pushAlert(
                                new Alert(`Successfully rejected ${selectedRowsCount} suggested water account${selectedRowsCount == 1 ? "" : "s"}.`, AlertContext.Success)
                            );
                            this.reloadSuggestions();
                        },
                        (error) => {
                            this.isLoadingSubmit = false;
                        }
                    );
                }
            });
    }

    public reviewSuggestionModal(waterAccountNumber: number, waterAccountName: string, parcelIDList: string, wellIDList: string, contactName: string, contactAddress: string) {
        this.modalService
            .open(ReviewWaterAccountSuggestionComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light }, {
                WaterAccountNumber: waterAccountNumber,
                WaterAccountName: waterAccountName,
                WellIDList: wellIDList?.split(",").map((x) => parseInt(x)),
                ParcelIDList: parcelIDList.split(",").map((x) => parseInt(x)),
                GeographyID: this.geographyID,
                GeographyName: this.geographyName,
                ContactName: contactName,
                ContactAddress: contactAddress,
            } as WaterAccountSuggestionContext)
            .instance.result.then((result) => {
                if (result) {
                    this.reloadSuggestions();
                }
            });
    }

    public bulkApprove() {
        const selectedFilteredSortedRows: CreateWaterAccountFromSuggestionDto[] = [];
        this.gridApi?.forEachNodeAfterFilterAndSort((node) => {
            if (node.isSelected()) {
                selectedFilteredSortedRows.push(
                    new CreateWaterAccountFromSuggestionDto({
                        WaterAccountName: node.data.WaterAccountName,
                        ParcelIDList: node.data.ParcelIDList.split(",").map((x) => parseInt(x)),
                        ContactName: node.data.ContactName,
                        ContactAddress: node.data.ContactAddress,
                    })
                );
            }
        });

        this.modalService
            .open(BulkApproveWaterAccountSuggestionComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: this.geographyID,
                WaterAccountSuggestions: selectedFilteredSortedRows,
            } as BulkApproveWaterAccountSuggestionContext)
            .instance.result.then((result) => {
                if (result) {
                    this.reloadSuggestions();
                }
            });
    }
}
