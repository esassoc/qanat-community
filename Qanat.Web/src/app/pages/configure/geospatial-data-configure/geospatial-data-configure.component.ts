import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ExternalMapLayerDto } from "src/app/shared/generated/model/external-map-layer-dto";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { ParcelMapComponent } from "../../../shared/components/parcel/parcel-map/parcel-map.component";
import { AsyncPipe, NgIf } from "@angular/common";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { routeParams } from "src/app/app.routes";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "geospatial-data-configure",
    templateUrl: "./geospatial-data-configure.component.html",
    styleUrls: ["./geospatial-data-configure.component.scss"],
    standalone: true,
    imports: [AsyncPipe, PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, RouterLink, QanatGridComponent, NgIf, ParcelMapComponent, LoadingDirective],
})
export class GeospatialDataConfigureComponent implements OnInit {
    public geospatialData$: Observable<ExternalMapLayerDto[]>;
    public geography$: Observable<GeographyDto>;
    public isLoading: boolean;

    public columnDefs: ColDef[];
    public richTextTypeID: number = CustomRichTextTypeEnum.ExternalMapLayers;
    public csvDownloadColIDsToExclude = ["0"];
    public mapCqlFilter: string;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;

    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private route: ActivatedRoute,
        private externalMapLayerService: ExternalMapLayerService,
        private router: Router,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.geospatialData$ = this.route.params.pipe(
            tap(() => (this.isLoading = true)),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.mapCqlFilter = `GeographyID = ${geography.GeographyID}`;
                this.currentGeographyService.setCurrentGeography(geography);
            }),
            switchMap((geography) => {
                return this.externalMapLayerService.geographiesGeographyIDExternalMapLayersGet(geography.GeographyID);
            }),
            tap(() => (this.isLoading = false))
        );

        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            switchMap((geography) => {
                return this.geographyService.geographiesGeographyIDGet(geography.GeographyID);
            })
        );

        this.createColumnDefs();
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
