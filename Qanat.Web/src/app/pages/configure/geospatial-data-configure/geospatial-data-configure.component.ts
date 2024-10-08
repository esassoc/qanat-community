import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { forkJoin } from "rxjs";
import { Subscription } from "rxjs/internal/Subscription";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ExternalMapLayerDto } from "src/app/shared/generated/model/external-map-layer-dto";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { AlertService } from "src/app/shared/services/alert.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { ParcelMapComponent } from "../../../shared/components/parcel-map/parcel-map.component";
import { NgIf } from "@angular/common";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "geospatial-data-configure",
    templateUrl: "./geospatial-data-configure.component.html",
    styleUrls: ["./geospatial-data-configure.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, RouterLink, QanatGridComponent, NgIf, ParcelMapComponent],
})
export class GeospatialDataConfigureComponent implements OnInit, OnDestroy {
    public selectedGeography$ = Subscription.EMPTY;
    public geographyID: number;

    public geography: GeographyDto;
    public columnDefs: ColDef[];
    public richTextTypeID: number = CustomRichTextTypeEnum.ExternalMapLayers;
    public csvDownloadColIDsToExclude = ["0"];
    public mapCqlFilter: string;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public geospatialData: ExternalMapLayerDto[];
    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private route: ActivatedRoute,
        private externalMapLayerService: ExternalMapLayerService,
        private router: Router,
        private geographyService: GeographyService,
        private selectedGeographyService: SelectedGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
        this.createColumnDefs();
    }

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }

    getDataForGeographyID(geographyID: number) {
        forkJoin({
            geography: this.geographyService.geographiesGeographyIDGet(geographyID),
            geospatialData: this.externalMapLayerService.geographiesGeographyIDExternalMapLayersGet(geographyID),
        }).subscribe(({ geography, geospatialData }) => {
            this.geography = geography;
            this.geospatialData = geospatialData;
            this.mapCqlFilter = `GeographyID = ${geographyID}`;
        });
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.router.navigate([`edit/${params.data.ExternalMapLayerID}`], { relativeTo: this.route }),
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("Layer Name", "ExternalMapLayerDisplayName", {
                FieldDefinitionType: "ExternalMapLayersName",
                FieldDefinitionLabelOverride: "Layer Name",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Layer Type", "ExternalMapLayerType.ExternalMapLayerTypeDisplayName", {
                FieldDefinitionType: "ExternalMapLayersType",
                FieldDefinitionLabelOverride: "Layer Type",
            }),
            {
                headerName: "URL",
                field: "ExternalMapLayerURL",
            },
            {
                headerName: "Visible By Default?",
                field: "LayerIsOnByDefault",
            },
            {
                headerName: "Active?",
                field: "IsActive",
            },
            {
                headerName: "Description",
                field: "ExternalMapLayerDescription",
            },
            this.utilityFunctionsService.createBasicColumnDef("Pop-Up Field", "PopUpField", { FieldDefinitionType: "PopUpField" }),
            this.utilityFunctionsService.createBasicColumnDef("Min Zoom", "MinZoom", {
                FieldDefinitionType: "ExternalMapLayersMinimumZoom",
                FieldDefinitionLabelOverride: "Minimum Zoom",
            }),
        ];
    }
}
