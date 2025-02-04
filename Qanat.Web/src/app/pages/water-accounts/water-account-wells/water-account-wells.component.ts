import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { Observable } from "rxjs";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { ExternalMapLayerDto, WellMinimalDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { map, switchMap, tap } from "rxjs/operators";
import { ColDef, GridApi, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { NgIf, AsyncPipe, NgForOf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { Map, layerControl } from "leaflet";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { GeographyExternalMapLayerComponent } from "src/app/shared/components/leaflet/layers/geography-external-map-layer/geography-external-map-layer.component";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";

@Component({
    selector: "water-account-wells",
    templateUrl: "./water-account-wells.component.html",
    styleUrls: ["./water-account-wells.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        RouterLink,
        QanatGridComponent,
        AsyncPipe,
        QanatMapComponent,
        WellsLayerComponent,
        LoadingDirective,
        ZoneGroupLayerComponent,
        GeographyExternalMapLayerComponent,
        NgForOf,
    ],
})
export class WaterAccountWellsComponent implements OnInit, OnDestroy {
    public geographyID: number;

    public waterAccount$: Observable<WaterAccountDto>;
    public wells$: Observable<WellMinimalDto[]>;
    public geographyWellDict: { [GeographyName: string]: WellMinimalDto[] } = {};
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public externalMapLayers$: Observable<ExternalMapLayerDto[]>;

    public selectedParcelIDs: number[];

    public columnDefs: ColDef<any>[];

    private _highlightedWellID: number;
    set highlightedWellID(value: number) {
        this._highlightedWellID = value;
        this.selectHighlightedWellIDRowNode();
    }

    get highlightedWellID(): number {
        return this._highlightedWellID;
    }

    public gridApi: GridApi;

    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;

    constructor(
        private wellService: WellService,
        private waterAccountService: WaterAccountService,
        private route: ActivatedRoute,
        private utilityFunctionsService: UtilityFunctionsService,
        private zoneGroupService: ZoneGroupService,
        private externalMapLayerService: ExternalMapLayerService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.waterAccount$ = this.route.paramMap.pipe(
            map((paramMap) => parseInt(paramMap.get(routeParams.waterAccountID))),
            switchMap((waterAccountID) => this.waterAccountService.waterAccountsWaterAccountIDGet(waterAccountID)),
            tap((waterAccount) => {
                this.geographyID = waterAccount.Geography.GeographyID;

                this.wells$ = this.wellService.waterAccountsWaterAccountIDWellsGet(waterAccount.WaterAccountID);
                this.zoneGroups$ = this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID);
                this.externalMapLayers$ = this.externalMapLayerService.geographiesGeographyIDExternalMapLayersGet(this.geographyID);
            })
        );

        this.buildColumnDefs();
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public buildColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Well ID", "WellID", "WellID", { InRouterLink: "/wells/" }),
            this.utilityFunctionsService.createBasicColumnDef("Well Name", "WellName"),
            this.utilityFunctionsService.createLinkColumnDef("Default APN", "ParcelNumber", "ParcelID", { InRouterLink: "/parcels/" }),
            this.utilityFunctionsService.createMultiLinkColumnDef("Irrigates", "IrrigatesParcels", "ParcelID", "ParcelNumber", {
                InRouterLink: "/parcels",
                MaxWidth: 300,
            }),
            this.utilityFunctionsService.createBasicColumnDef("County Well Permit #", "CountyWellPermitNumber", { FieldDefinitionType: "CountyWellPermitNo" }),
            this.utilityFunctionsService.createBasicColumnDef("State WCR #", "StateWCRNumber", { FieldDefinitionType: "StateWCRNo" }),
            this.utilityFunctionsService.createDateColumnDef("DateDrilled", "DateDrilled", "M/d/yyyy", { FieldDefinitionType: "DateDrilled" }),
            this.utilityFunctionsService.createDecimalColumnDef("Well Depth", "WellDepth", { FieldDefinitionType: "WellDepth" }),
            this.utilityFunctionsService.createBasicColumnDef("Well Status", "WellStatusDisplayName", {
                FieldDefinitionType: "WellStatus",
                CustomDropdownFilterField: "WellStatusDisplayName",
            }),
            this.utilityFunctionsService.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctionsService.createLatLonColumnDef("Longitude", "Longitude"),
            this.utilityFunctionsService.createBasicColumnDef("Notes", "Notes"),
        ];
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onGridSelectionChanged(event: SelectionChangedEvent) {
        const selection = event.api.getSelectedRows()[0];
        if (selection && selection.WellID) {
            this.highlightedWellID = selection.WellID;
        }
    }

    public handleMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;

        this.cdr.detectChanges();
    }

    public onMapSelectionChanged(event) {
        if (this.highlightedWellID == event.wellID) return;

        this.highlightedWellID = event.wellID;
        this.gridApi.forEachNode((node, index) => {
            if (node.data.WellID == this.highlightedWellID) {
                node.setSelected(true, true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }

    public selectHighlightedWellIDRowNode() {
        this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
            if (rowNode.data.WellID == this.highlightedWellID) {
                rowNode.setSelected(true);
                this.gridApi.ensureIndexVisible(index, "top");
            }
        });
    }
}
