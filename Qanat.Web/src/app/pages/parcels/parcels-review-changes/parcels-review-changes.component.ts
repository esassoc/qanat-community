import { Component, OnInit } from "@angular/core";
import { ColDef, FilterChangedEvent, GetRowIdParams, GridApi, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { BehaviorSubject, Observable, switchMap, tap } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { TagRendererComponent } from "src/app/shared/components/ag-grid/tag-renderer/tag-renderer.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ParcelChangesGridItemDto } from "src/app/shared/generated/model/parcel-changes-grid-item-dto";
import { UploadedGdbSimpleDto } from "src/app/shared/generated/model/uploaded-gdb-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormsModule } from "@angular/forms";
import { AsyncPipe, DecimalPipe, DatePipe } from "@angular/common";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { ParcelReviewChangesCardComponent } from "src/app/shared/components/parcel/parcel-review-changes-card/parcel-review-changes-card.component";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "parcels-review-changes",
    templateUrl: "./parcels-review-changes.component.html",
    styleUrl: "./parcels-review-changes.component.scss",
    imports: [
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        NoteComponent,
        IconComponent,
        ParcelReviewChangesCardComponent,
        QanatGridComponent,
        FormsModule,
        AsyncPipe,
        DecimalPipe,
        DatePipe,
    ],
})
export class ParcelsReviewChangesComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public geographyID: number;
    public customRichTextTypeID = CustomRichTextTypeEnum.ReviewParcelChanges;

    public latestGDBUpload$: Observable<UploadedGdbSimpleDto>;

    public parcels$: Observable<ParcelChangesGridItemDto[]>;
    public parcelRefresh$ = new BehaviorSubject(null);
    public parcels: ParcelChangesGridItemDto[];
    public parcelsToDisplay: ParcelChangesGridItemDto[];

    public selectedParcel: ParcelChangesGridItemDto;
    public unreviewedParcelsCount: number;
    public reviewedParcelsCount: number;
    public showReviewedParcels: boolean = false;

    public columnDefs: ColDef<ParcelChangesGridItemDto>[];
    public gridApi: GridApi;
    public anyGridFilterPresent: boolean = false;
    public filteredRowsCount: number;

    public isLoadingSubmit: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private geographyService: GeographyService,
        private parcelService: ParcelService,
        private parcelByGeographyService: ParcelByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.geographyID = geography.GeographyID;

                this.latestGDBUpload$ = this.parcelByGeographyService.getLatestUploadedFinalizedGDBUploadForGeographyParcelByGeography(geography.GeographyID);

                this.parcels$ = this.parcelRefresh$.pipe(
                    tap(() => this.gridApi?.showLoadingOverlay()),
                    switchMap(() => this.parcelByGeographyService.getParcelReviewChangesGridItemsParcelByGeography(geography.GeographyID)),
                    tap((parcels) => {
                        this.parcels = parcels;
                        this.setParcelsToDisplay();

                        this.reviewedParcelsCount = parcels.filter((x) => x.IsReviewed).length;
                        this.unreviewedParcelsCount = parcels.length - this.reviewedParcelsCount;
                    })
                );
            })
        );

        this.createColumnDefs();
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Reviewed?", "IsReviewed", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.IsReviewed),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.ParcelID}/detail`, LinkDisplay: params.data.ParcelNumber };
                },
                InRouterLink: "/parcels/",
                CellClass: (params) => (params.data.IsReviewed ? "muted-text" : null),
            }),
            {
                headerName: "Parcel Changes",
                cellRenderer: TagRendererComponent,
                valueGetter: (params) => {
                    return params.data.ParcelFieldDiffs.filter((x) => x.CurrentFieldValue != x.PreviousFieldValue).map((x) => x.FieldShortName);
                },
                cellRendererParams: (params) => {
                    return { disabled: params.data.IsReviewed };
                },
                width: 400,
            },
            this.utilityFunctionsService.createLinkColumnDef("Water Account", "WaterAccount.WaterAccountNumber", "WaterAccount.WaterAccountID", {
                InRouterLink: "/water-accounts/",
                FieldDefinitionType: "WaterAccount",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Status", "ParcelStatus.ParcelStatusDisplayName", {
                CustomDropdownFilterField: "ParcelStatus.ParcelStatusDisplayName",
                FieldDefinitionType: "ParcelStatus",
            }),
            this.utilityFunctionsService.createDateColumnDef("Date Reviewed", "ReviewDate", "shortDate"),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.updateGridSelection();
    }

    public onFilterChanged(event: FilterChangedEvent) {
        this.anyGridFilterPresent = event.api.isAnyFilterPresent();
        let filteredRowsCount = 0;
        this.gridApi.forEachNodeAfterFilter(() => {
            filteredRowsCount++;
        });
        this.filteredRowsCount = filteredRowsCount;
    }

    public getRowId = (event: GetRowIdParams) => event.data.ParcelID;

    public setParcelsToDisplay() {
        this.parcelsToDisplay = this.showReviewedParcels ? this.parcels : this.parcels.filter((x) => !x.IsReviewed);

        this.setSelectedParcel();
    }

    public setSelectedParcel() {
        if (this.selectedParcel) {
            const index = this.parcels.findIndex((x) => x.ParcelID == this.selectedParcel.ParcelID);
            if (index >= 0) {
                this.selectedParcel = this.parcels[index];
            } else {
                this.selectedParcel = null;
            }
        }

        if (!this.selectedParcel && this.parcelsToDisplay.length > 0) {
            this.selectedParcel = this.parcelsToDisplay[0];
            this.updateGridSelection();
        }
    }

    public selectNextParcel() {
        if (this.parcelsToDisplay.length == 0) {
            this.selectedParcel = null;
            return;
        }

        const currentSelectedParcelIndex = this.parcelsToDisplay.findIndex((x) => x.ParcelID == this.selectedParcel.ParcelID);

        let newIndex = currentSelectedParcelIndex + 1;
        if (newIndex == this.parcelsToDisplay.length) {
            newIndex = 0;
        }

        this.selectedParcel = this.parcelsToDisplay[newIndex];
        this.updateGridSelection();
    }

    public onParcelSelected(params: SelectionChangedEvent) {
        const selectedRow = params.api.getSelectedRows()[0];
        if (!selectedRow) return;

        this.selectedParcel = params.api.getSelectedRows()[0];
    }

    public updateGridSelection() {
        if (!this.gridApi || !this.selectedParcel) return;

        const rowNodeToSelect = this.gridApi.getRowNode(this.selectedParcel.ParcelID.toString());
        if (!rowNodeToSelect) return;

        rowNodeToSelect.setSelected(true, true);
        this.gridApi.ensureNodeVisible(rowNodeToSelect);
    }

    public onParcelReviewed() {
        this.selectedParcel = null;
        this.parcelRefresh$.next(null);
    }
    public onParcelUpdated() {
        this.parcelRefresh$.next(null);
    }

    public markAllAsReviewed() {
        const parcelIDs = [];
        this.gridApi.forEachNodeAfterFilter((x) => parcelIDs.push(x.id));

        const message = `Are you sure you want to mark <b>${parcelIDs.length} parcels</b> as reviewed?`;
        this.confirmService
            .confirm({ buttonClassYes: "btn-primary", title: "Mark Parcels as Reviewed", message: message, buttonTextYes: "Mark Parcels as Reviewed", buttonTextNo: "Cancel" })
            .then((confirmed) => {
                if (!confirmed) return;

                this.parcelService.markParcelAsReviewedParcel(this.geographyID, parcelIDs).subscribe({
                    next: () => {
                        this.isLoadingSubmit = false;
                        this.selectedParcel = null;
                        this.alertService.pushAlert(new Alert(`${parcelIDs.length} parcels successfully marked as reviewed.`, AlertContext.Success));
                        this.parcelRefresh$.next(null);
                    },
                    error: () => (this.isLoadingSubmit = false),
                });
            });
    }
}
