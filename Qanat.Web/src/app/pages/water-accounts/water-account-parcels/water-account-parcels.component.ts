import { ChangeDetectorRef, Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { Observable, forkJoin } from "rxjs";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { AllocationPlanMinimalDto, ExternalMapLayerSimpleDto, ParcelDetailDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { map, switchMap, tap } from "rxjs/operators";
import { ColDef, GridApi, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ParcelLayerComponent } from "src/app/shared/components/leaflet/layers/parcel-layer/parcel-layer.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { Map, layerControl } from "leaflet";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { WaterAccountParcelByWaterAccountService } from "src/app/shared/generated/api/water-account-parcel-by-water-account.service";

@Component({
    selector: "water-account-parcels",
    templateUrl: "./water-account-parcels.component.html",
    styleUrls: ["./water-account-parcels.component.scss"],
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        QanatGridComponent,
        AsyncPipe,
        QanatMapComponent,
        ParcelLayerComponent,
        LoadingDirective,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
    ],
})
export class WaterAccountParcelsComponent implements OnInit, OnDestroy {
    public geographyID: number;

    public parcels: ParcelDetailDto[];
    public parcels$: Observable<ParcelDetailDto[]>;

    public selectedParcelIDs: number[];
    public highlightedParcelDto: ParcelDetailDto;
    public waterAccount$: Observable<WaterAccountDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    public zoneGroups: ZoneGroupMinimalDto[];
    public externalMapLayers: ExternalMapLayerSimpleDto[];

    public columnDefs: ColDef<any>[];
    public isLoading = true;
    private _highlightedParcelID: number;
    set highlightedParcelID(value: number) {
        this._highlightedParcelID = value;
        this.highlightedParcelDto = this.parcels.filter((x) => x.ParcelID == value)[0];
        this.selectHighlightedParcelIDRowNode();
    }

    get highlightedParcelID(): number {
        return this._highlightedParcelID;
    }

    public gridApi: GridApi;
    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private waterAccountService: WaterAccountService,
        private waterAccountParcelByWaterAccountService: WaterAccountParcelByWaterAccountService,
        private route: ActivatedRoute,
        private utilityFunctionsService: UtilityFunctionsService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.route.paramMap.pipe(
            map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
            switchMap((waterAccountID) => this.waterAccountService.getByIDWaterAccount(waterAccountID)),
            tap((waterAccount) => {
                this.geographyID = waterAccount.Geography.GeographyID;

                this.parcels$ = this.route.paramMap.pipe(
                    map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
                    switchMap((waterAccountID) =>
                        forkJoin({
                            parcels: this.waterAccountParcelByWaterAccountService.getWaterAccountParcelsWaterAccountParcelByWaterAccount(waterAccountID),
                            zoneGroups: this.zoneGroupService.listZoneGroup(this.geographyID),
                            externalMapLayers: this.externalMapLayerService.getExternalMapLayer(this.geographyID),
                        })
                    ),
                    tap(({ parcels, zoneGroups, externalMapLayers }) => {
                        this.selectedParcelIDs = parcels.map((x) => x.ParcelID);
                        this.parcels = parcels;
                        this.zoneGroups = zoneGroups;
                        this.externalMapLayers = externalMapLayers;

                        this.buildColumnDefs();
                        this.isLoading = false;
                    }),
                    map((x) => x.parcels)
                );

                this.allocationPlans$ = this.waterAccountService.getAccountAllocationPlansByAccountIDWaterAccount(waterAccount.WaterAccountID);
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public buildColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", { InRouterLink: "/parcels/" }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (Acres)", "ParcelArea"),
            this.utilityFunctionsService.createBasicColumnDef("Parcel Status", "ParcelStatusDisplayName", {
                FieldDefinitionType: "ParcelStatus",
                CustomDropdownFilterField: "ParcelStatusDisplayName",
            }),
            { headerName: "Owner Name", field: "OwnerName" },
            { headerName: "Owner Address", field: "OwnerAddress" },
        ];

        this.zoneGroups.forEach((zoneGroup) => {
            this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "ZoneIDs"));
        });
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onGridSelectionChanged(event: SelectionChangedEvent) {
        const selection = event.api.getSelectedRows()[0];
        if (selection && selection.ParcelID) {
            this.highlightedParcelID = selection.ParcelID;
        }
    }

    public onMapSelectionChanged(selectedParcelID: number) {
        if (selectedParcelID == this.highlightedParcelID) return;
        this.highlightedParcelID = selectedParcelID;
        this.highlightedParcelDto = this.parcels.find((x) => x.ParcelID == selectedParcelID);

        this.selectHighlightedParcelIDRowNode();
    }

    public selectHighlightedParcelIDRowNode() {
        this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
            if (rowNode.data.ParcelID == this.highlightedParcelID) {
                rowNode.setSelected(true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }
}
