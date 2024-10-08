import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy, ApplicationRef, ViewContainerRef, ChangeDetectorRef } from "@angular/core";
import { environment } from "src/environments/environment";
import { WfsService } from "../../services/wfs.service";
import { Control, FitBoundsOptions, LeafletEvent, LeafletMouseEvent, Map, MapOptions, tileLayer, WMSOptions } from "leaflet";
import * as esri from "esri-leaflet";
import "esri-leaflet-renderers";
import "leaflet.markercluster";
import "../../../../../node_modules/leaflet.fullscreen/Control.FullScreen.js";
import "../../../../../node_modules/leaflet-loading/src/Control.Loading.js";
import * as L from "leaflet";
import { CustomCompileService } from "../../services/custom-compile.service";
import { BoundingBoxDto } from "../../generated/model/bounding-box-dto";
import { forkJoin } from "rxjs";
import { WellLocationDto } from "../../generated/model/well-location-dto";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import GestureHandling from "leaflet-gesture-handling";
import { ExternalMapLayerTypeEnum } from "../../generated/enum/external-map-layer-type-enum";
import { FeatureCollection } from "geojson";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "../../services/modal/modal.service";
import {
    MonitoringWellMeasurementChartComponent,
    MonitoringWellContext,
} from "../monitoring-wells/modal/monitoring-well-measurement-chart/monitoring-well-measurement-chart.component";
import { GeographyEnum as GeographyEnum } from "../../models/enums/geography.enum";
import { ExternalMapLayerDto, WellMinimalDto, ZoneGroupMinimalDto } from "../../generated/model/models";
import { ParcelService } from "../../generated/api/parcel.service";
import { WellService } from "../../generated/api/well.service";
import { ExternalMapLayerService } from "../../generated/api/external-map-layer.service";
import { ZoneGroupService } from "../../generated/api/zone-group.service";
import { NgIf } from "@angular/common";
import { GsaBoundariesComponent } from "../leaflet/layers/gsa-boundaries/gsa-boundaries.component";

@Component({
    selector: "parcel-map",
    templateUrl: "./parcel-map.component.html",
    styleUrls: ["./parcel-map.component.scss"],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [NgIf, GsaBoundariesComponent],
})
export class ParcelMapComponent implements OnInit, AfterViewInit {
    public _mapID: string = "";

    @Input()
    set mapID(value: string) {
        this._mapID = value.replace(" ", "");
    }
    get mapID() {
        return this._mapID;
    }

    @Input()
    public visibleParcelStyle: string = "parcel";

    @Input()
    public visibleParcelIDs: Array<number> = [];

    @Input()
    public selectedParcelStyle: string = "parcel_blue";

    //If null then show all zone groups. If empty array show none of the zone groups.
    @Input()
    public visibleZoneGroupIDs: Array<number>;

    @Input()
    public cqlFilter: string;

    @Input()
    public boundingBoxInput: BoundingBoxDto;

    private _selectedParcelIDs: Array<number> = [];

    @Input() set selectedParcelIDs(value: Array<number>) {
        if (this.selectedParcelIDs.length != value.length || this.selectedParcelIDs.some((x) => !value.includes(x))) {
            this._selectedParcelIDs = value;
            if (this.map) {
                this.updateSelectedParcelsOverlayLayer(this.selectedParcelIDs);
                this.fitBoundsToSelectedParcels(this.selectedParcelIDs);
            }
        }
    }

    get selectedParcelIDs(): Array<number> {
        return this._selectedParcelIDs;
    }

    @Input() set selectedWellIDs(value: Array<number>) {
        if (this.selectedWellIDs.length != value.length || this.selectedWellIDs.some((x) => !value.includes(x))) {
            this._selectedParcelIDs = value;
        }
    }

    get selectedWellIDs(): Array<number> {
        return this._selectedParcelIDs;
    }

    @Input()
    public highlightedParcelStyle: string = "parcel_yellow";

    private _highlightedParcelID: number = null;

    @Input() set highlightedParcelID(value: number) {
        if (this.highlightedParcelID != value) {
            this._highlightedParcelID = value;
            this.highlightParcel();
        }
    }

    get highlightedParcelID(): number {
        return this._highlightedParcelID;
    }

    private _highlightedWellID: number = null;
    @Input() set highlightedWellID(value: number) {
        if (this.highlightedWellID != value) {
            this._highlightedWellID = value;
            this.updateSelectedWellLayer(this.highlightedWellID);
        }
    }

    get highlightedWellID(): number {
        return this._highlightedWellID;
    }

    @Output()
    public highlightedParcelIDChange: EventEmitter<number> = new EventEmitter<number>();

    @Output()
    public highlightedWellIDChange: EventEmitter<number> = new EventEmitter<number>();

    @Input()
    public onEachFeatureCallback?: (feature, layer) => void;

    @Input()
    public zoomMapToDefaultExtent: boolean = true;

    @Input()
    public highlightParcelOnClick: boolean = false;

    @Input()
    public highlightWellOnClick: boolean = false;

    @Input()
    public displayParcelLayerOnLoad: boolean = true;

    @Input()
    public displayWellLayerOnLoad: boolean = false;
    @Input()
    public displayGSABoundaryOnLoad: boolean = false;

    @Input()
    public mapHeight: string = "300px";

    @Input()
    public defaultFitBoundsOptions?: FitBoundsOptions = null;

    @Input()
    public selectedParcelLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px; display:inline-block;'> Selected Parcels";

    @Input()
    public geographyID: number;

    @Input()
    public collapsedLayerControl: boolean = false;

    //removes the ability to move the map with the cursor, zoom in/out and change layers
    @Input()
    public displayAllZoneGroupsOnLoad: boolean = false;

    @Input()
    public visibleZoneGroupIDsOnLoad: Array<number> = [];

    @Input()
    public isStatic: boolean = false;

    @Output()
    public afterSetControl: EventEmitter<Control.Layers> = new EventEmitter();

    @Output()
    public afterLoadMap: EventEmitter<LeafletEvent> = new EventEmitter();

    @Output()
    public mapMoveEnd: EventEmitter<LeafletEvent> = new EventEmitter();

    @Input()
    public returnParcelOnClick: boolean = false;
    @Input()
    public displayMonitoringWellsOnLoad: boolean = false;
    @Output()
    public clickedOnParcel: EventEmitter<any> = new EventEmitter();

    private _customGeoJSONLayers: CustomGeoJSONLayer[] = [];
    @Input() set customGeoJSONLayers(value: CustomGeoJSONLayer[]) {
        if (this.customGeoJSONLayers != value) {
            this._customGeoJSONLayers = value;
            if (this.map) {
                this.updateCustomGeoJSONLayers(this.customGeoJSONLayers);
            }
        }
    }
    get customGeoJSONLayers(): CustomGeoJSONLayer[] {
        return this._customGeoJSONLayers;
    }

    public parcelLayerName: string = "<img src='./assets/main/map-legend-images/parcel.png' style='height:16px; margin-bottom:3px; display:inline-block;'> Parcels";
    public wellLayerName: string = "<img src='./assets/main/map-legend-images/well-red.png' style='height:16px; margin-bottom:3px; display:inline-block;'> Wells";
    public component: any;

    public wells: WellMinimalDto[];
    public wellLocations: WellLocationDto[];
    public zoneGroups: ZoneGroupMinimalDto[];
    public selectedWellMarker: L.Marker;
    private wellLayer: L.MarkerClusterGroup;
    public selectedReferenceWell: WellMinimalDto;

    public map: Map;
    public featureLayer: any;
    public layerControl: Control.Layers;
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public overlayLayers: { [key: string]: any } = {};
    boundingBox: BoundingBoxDto;
    private selectedParcelLayer: any;
    private highlightedParcelLayer: any;
    private selectedWellLayer: any;
    private highlightedWellLayer: any;
    private externalMapLayers: ExternalMapLayerDto[];

    private wellIcon = this.leafletHelperService.blueIcon;
    private selectedWellIcon = this.leafletHelperService.yellowIconLarge;
    private defaultParcelsWMSOptions: WMSOptions = {
        layers: "Qanat:AllParcels",
        transparent: true,
        format: "image/png",
        tiled: true,
    } as WMSOptions;

    private defaultZoneGroupWMSOptions: WMSOptions = {
        layers: "Qanat:ZoneGroups",
        transparent: true,
        format: "image/png",
        tiled: true,
    } as WMSOptions;

    public visibleZoneGroupStyle: string = "zone_group";

    constructor(
        private wfsService: WfsService,
        private parcelService: ParcelService,
        private appRef: ApplicationRef,
        private compileService: CustomCompileService,
        private wellService: WellService,
        private leafletHelperService: LeafletHelperService,
        private externalMapLayerService: ExternalMapLayerService,
        private zoneGroupService: ZoneGroupService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private cdr: ChangeDetectorRef
    ) {}

    public ngOnInit(): void {
        this.compileService.configure(this.appRef);
    }

    ngOnDestroy(): void {
        if (this.map) {
            this.map.off();
            this.map.remove();
            this.map = null;
        }
    }

    public updateSelectedParcelsOverlayLayer(parcelIDs: Array<number>) {
        if (this.selectedParcelLayer) {
            this.layerControl.removeLayer(this.selectedParcelLayer);
            this.map.removeLayer(this.selectedParcelLayer);
        }

        const wmsParameters = Object.assign({ styles: this.selectedParcelStyle, cql_filter: this.createParcelMapFilter(parcelIDs) }, this.defaultParcelsWMSOptions);
        this.selectedParcelLayer = tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsParameters);
        this.layerControl.addOverlay(this.selectedParcelLayer, this.selectedParcelLayerName);

        this.selectedParcelLayer.addTo(this.map).bringToFront();
        if (this.highlightedParcelLayer) {
            this.highlightedParcelLayer.bringToFront();
        }
    }

    private fitBoundsToSelectedParcels(parcelIDs: Array<number>) {
        this.parcelService.parcelsBoundingBoxPost(parcelIDs).subscribe((boundingBox) => {
            this.boundingBox = boundingBox;
            this.map.fitBounds(
                [
                    [this.boundingBox.Bottom, this.boundingBox.Left],
                    [this.boundingBox.Top, this.boundingBox.Right],
                ],
                this.defaultFitBoundsOptions
            );
        });
    }

    private fitBoundsToSelectedZoneGroups(zoneGroupIDs: Array<number>) {
        this.zoneGroupService.zoneGroupBoundingBoxPost(zoneGroupIDs).subscribe((boundingBox) => {
            this.boundingBox = boundingBox;
            this.map.fitBounds(
                [
                    [this.boundingBox.Bottom, this.boundingBox.Left],
                    [this.boundingBox.Top, this.boundingBox.Right],
                ],
                this.defaultFitBoundsOptions
            );
        });
    }

    private fitBoundsToSelectedWells(wellIDs: Array<number>) {
        this.wellService.wellsBoundingBoxPost(wellIDs).subscribe((boundingBox) => {
            this.boundingBox = boundingBox;
            this.map.fitBounds(
                [
                    [this.boundingBox.Bottom, this.boundingBox.Left],
                    [this.boundingBox.Top, this.boundingBox.Right],
                ],
                this.defaultFitBoundsOptions
            );
        });
    }

    private createParcelMapFilter(parcelIDs: Array<number>): any {
        return "ParcelID in (" + parcelIDs.join(",") + ")";
    }

    private createZoneGroupFilter(zoneGroupIDs: Array<number>): any {
        return "ZoneGroupID in (" + zoneGroupIDs.join(",") + ")";
    }

    public ngAfterViewInit(): void {
        // Default bounding box
        this.boundingBox = new BoundingBoxDto();
        this.boundingBox.Left = environment.parcelBoundingBoxLeft;
        this.boundingBox.Bottom = environment.parcelBoundingBoxBottom;
        this.boundingBox.Right = environment.parcelBoundingBoxRight;
        this.boundingBox.Top = environment.parcelBoundingBoxTop;

        if (this.boundingBoxInput) {
            this.boundingBox = this.boundingBoxInput;
        }

        forkJoin({
            wells: this.wellService.geographiesGeographyIDWellsGet(this.geographyID),
            wellLocations: this.wellService.geographiesGeographyIDWellsLocationGet(this.geographyID),
            externalMapLayers: this.externalMapLayerService.geographiesGeographyIDExternalMapLayersActiveGet(this.geographyID),
            zoneGroups: this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID),
        }).subscribe(({ wells, wellLocations, externalMapLayers, zoneGroups }) => {
            this.wells = wells;
            this.wellLocations = wellLocations;
            this.externalMapLayers = externalMapLayers;
            this.zoneGroups = zoneGroups;
            this.initMap();
        });
    }

    public initMap(): void {
        if (this.isStatic) {
            const mapOptions: MapOptions = {
                minZoom: 6,
                maxZoom: 17,
                layers: [this.tileLayers.Aerial],
                fullscreenControl: true,
                zoomControl: false,
                dragging: false,
                scrollWheelZoom: false,
                doubleClickZoom: false,
                boxZoom: false,
                keyboard: false,
                touchZoom: false,
                gestureHandling: true,
            } as MapOptions;
            this.map = L.map(this.mapID, mapOptions);
        } else {
            const mapOptions: MapOptions = {
                minZoom: 6,
                maxZoom: 17,
                layers: [this.tileLayers.Aerial],
                fullscreenControl: true,
                gestureHandling: true,
            } as MapOptions;
            this.map = L.map(this.mapID, mapOptions);
        }

        this.map.on("load", (event: LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: LeafletEvent) => {
            this.mapMoveEnd.emit(event);
        });
        this.map.fitBounds(
            [
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ],
            this.defaultFitBoundsOptions
        );
        this.setupLayerControl();

        if (this.selectedParcelIDs.length > 0) {
            this.updateSelectedParcelsOverlayLayer(this.selectedParcelIDs);
            this.fitBoundsToSelectedParcels(this.selectedParcelIDs);
        }
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        if (this.highlightParcelOnClick) {
            const wfsService = this.wfsService;
            const self = this;
            this.map.on("click", (event: LeafletMouseEvent): void => {
                self.map.fireEvent("dataloading");
                wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat).subscribe((parcelFeatureCollection: FeatureCollection) => {
                    self.map.fireEvent("dataload");
                    if (!parcelFeatureCollection.features || parcelFeatureCollection.features.length == 0) {
                        return;
                    }

                    const parcelID = parcelFeatureCollection.features[0].properties.ParcelID;
                    if (self.highlightedParcelID != parcelID) {
                        self.highlightedParcelID = parcelID;
                        self.highlightedParcelIDChange.emit(self.highlightedParcelID);
                    }
                });
            });
        }

        if (this.returnParcelOnClick) {
            const wfsService = this.wfsService;
            const self = this;
            this.map.on("click", (event: LeafletMouseEvent): void => {
                self.map.fireEvent("dataloading");
                wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat, this.geographyID).subscribe((parcelFeatureCollection: FeatureCollection) => {
                    self.map.fireEvent("dataload");
                    if (!parcelFeatureCollection.features || parcelFeatureCollection.features.length == 0) {
                        return;
                    }

                    self.clickedOnParcel.emit(parcelFeatureCollection.features[0].properties);
                });
            });
        }

        if (this.customGeoJSONLayers.length > 0) {
            this.updateCustomGeoJSONLayers(this.customGeoJSONLayers);
        }
    }

    public highlightParcel() {
        if (this.highlightedParcelLayer) {
            this.map.removeLayer(this.highlightedParcelLayer);
            this.highlightedParcelLayer = null;
        }
        if (this.highlightedParcelID) {
            const wmsParameters = Object.assign({ styles: this.highlightedParcelStyle, cql_filter: `ParcelID = ${this.highlightedParcelID}` }, this.defaultParcelsWMSOptions);
            this.highlightedParcelLayer = tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsParameters);
            this.highlightedParcelLayer.addTo(this.map).bringToFront();
            this.fitBoundsToSelectedParcels([this.highlightedParcelID]);
        }
    }

    public highlightWell(wellID: number) {
        this.highlightedWellID = wellID;
        if (this.highlightedWellLayer) {
            this.map.removeLayer(this.highlightedWellLayer);
            this.highlightedWellLayer = null;
        }
        if (this.highlightedWellID) {
            this.fitBoundsToSelectedWells([this.highlightedWellID]);
            this.highlightedWellIDChange.emit(wellID);
        }
    }

    public setupLayerControl(): void {
        this.addParcelLayer();
        this.addZoneGroupLayers();
        this.addExternalMapLayers();
        if (this.displayMonitoringWellsOnLoad) {
            this.addMonitoringWells();
        }

        if (!this.isStatic) {
            this.layerControl = new Control.Layers(this.tileLayers, this.overlayLayers, { collapsed: this.collapsedLayerControl }).addTo(this.map);

            this.addCloseButtonToLayerControl(this.layerControl);

            this.updateWellLayer();

            if (!this.displayWellLayerOnLoad) {
                this.map.removeLayer(this.wellLayer);
            }

            this.afterSetControl.emit(this.layerControl);
            this.cdr.markForCheck();
        }
    }

    private addParcelLayer() {
        const parcelsWMSOptions = Object.assign({ styles: this.visibleParcelStyle }, this.defaultParcelsWMSOptions);
        if (this.visibleParcelIDs.length > 0) {
            parcelsWMSOptions.cql_filter = this.createParcelMapFilter(this.visibleParcelIDs);
            this.fitBoundsToSelectedParcels(this.visibleParcelIDs);
        }

        if (this.cqlFilter) {
            parcelsWMSOptions.cql_filter = this.cqlFilter;
        }
        this.overlayLayers = Object.assign(
            {
                [this.parcelLayerName]: tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", parcelsWMSOptions),
            },
            this.overlayLayers
        );

        if (this.displayParcelLayerOnLoad) {
            this.overlayLayers[this.parcelLayerName].addTo(this.map);
        }
    }

    private addZoneGroupLayers() {
        const zoneGroupsWMSOptions = Object.assign({ styles: this.visibleZoneGroupStyle }, this.defaultZoneGroupWMSOptions);

        if (this.visibleZoneGroupIDs && this.visibleZoneGroupIDs.length > 0) {
            const zoneGroupName = this.zoneGroups.filter((x) => x.ZoneGroupID == this.visibleZoneGroupIDs[0])[0].ZoneGroupName;
            zoneGroupsWMSOptions.cql_filter = this.createZoneGroupFilter(this.visibleZoneGroupIDs);
            this.fitBoundsToSelectedZoneGroups(this.visibleZoneGroupIDs);
            this.overlayLayers = Object.assign(
                {
                    [zoneGroupName]: tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", zoneGroupsWMSOptions),
                },
                this.overlayLayers
            );

            if (this.displayAllZoneGroupsOnLoad) {
                this.overlayLayers[zoneGroupName].addTo(this.map);
            }
        }

        if (!this.visibleZoneGroupIDs) {
            this.zoneGroups.reverse().forEach((zoneGroup) => {
                zoneGroupsWMSOptions.cql_filter = this.createZoneGroupFilter([zoneGroup.ZoneGroupID]);
                this.overlayLayers = Object.assign(
                    {
                        [zoneGroup.ZoneGroupName]: tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", zoneGroupsWMSOptions),
                    },
                    this.overlayLayers
                );
                if (this.displayAllZoneGroupsOnLoad || this.visibleZoneGroupIDsOnLoad.includes(zoneGroup.ZoneGroupID)) {
                    this.overlayLayers[zoneGroup.ZoneGroupName].addTo(this.map);
                }
            });
        }
    }

    private addMonitoringWells() {
        const geojsonCNRAWellMarkerOptions = {
            radius: 4,
            fillColor: "#ff7800",
            color: "#000",
            weight: 1,
            opacity: 1,
            fillOpacity: 0.8,
        };
        const cql_filter = `GeographyID = ${this.geographyID}`;

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:CNRAMonitoringWells", cql_filter).subscribe((response) => {
            const geoJson = L.geoJSON(response, {
                pointToLayer: (feature, latlng) => {
                    return L.circleMarker(latlng, geojsonCNRAWellMarkerOptions);
                },
            });
            geoJson.addTo(this.map);

            geoJson.on("click", (e) => {
                this.modalService
                    .open(MonitoringWellMeasurementChartComponent, null, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light, TopLayer: false }, {
                        GeographyID: this.geographyID,
                        SiteCode: e.layer.feature.properties.SiteCode,
                        MonitoringWellName: e.layer.feature.properties.MonitoringWellName,
                    } as MonitoringWellContext)
                    .instance.result.then((result) => {});
            });
            this.layerControl.addOverlay(geoJson, "CNRA Monitoring Wells");
        });

        if (this.geographyID != GeographyEnum.yolo) return;

        const geoJsonYoloWRIDMarkerOptions = Object.assign({}, geojsonCNRAWellMarkerOptions);
        geoJsonYoloWRIDMarkerOptions.fillColor = "#007fff";

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:YoloWRIDMonitoringWells", cql_filter).subscribe((response) => {
            const geoJson = L.geoJSON(response, {
                pointToLayer: (feature, latlng) => {
                    return L.circleMarker(latlng, geoJsonYoloWRIDMarkerOptions);
                },
            });
            geoJson.addTo(this.map);

            geoJson.on("click", (e) => {
                this.modalService
                    .open(
                        MonitoringWellMeasurementChartComponent,
                        this.viewContainerRef,
                        { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light, TopLayer: false },
                        {
                            GeographyID: this.geographyID,
                            SiteCode: e.layer.feature.properties.SiteCode,
                            MonitoringWellName: e.layer.feature.properties.MonitoringWellName,
                        } as MonitoringWellContext
                    )
                    .instance.result.then((result) => {});
            });
            this.layerControl.addOverlay(geoJson, "Yolo WRID Monitoring Wells");
        });
    }

    private addExternalMapLayers() {
        this.externalMapLayers.forEach((mapLayer) => {
            let newFeatureLayer;
            switch (mapLayer.ExternalMapLayerType.ExternalMapLayerTypeID) {
                case ExternalMapLayerTypeEnum.ESRIMapServer:
                    newFeatureLayer = esri.tiledMapLayer({
                        url: mapLayer.ExternalMapLayerURL,
                        useCors: false,
                        isModern: false,
                    });
                    break;
                case ExternalMapLayerTypeEnum.ESRIFeatureServer:
                    newFeatureLayer = esri.featureLayer({
                        url: mapLayer.ExternalMapLayerURL,
                        useCors: false,
                        interactive: true,
                        minZoom: mapLayer.MinZoom,
                        isModern: false, // set to false makes the request/response type 'application/javascript' instead of 'applicaiton/geo+json' which was having CORS/CORB issues
                        onEachFeature: (feature, layer) => {
                            if (mapLayer.PopUpField != null && mapLayer.PopUpField != "") {
                                layer.bindPopup(`${mapLayer.PopUpField}: ${feature.properties[mapLayer.PopUpField]}`);
                            }
                        },
                    });
                    break;
                default:
                    console.error(`Invalid ExternalMapLayerTypeEnum ${mapLayer.ExternalMapLayerType.ExternalMapLayerTypeID}.`);
            }

            this.overlayLayers[mapLayer.ExternalMapLayerDisplayName] = newFeatureLayer;
            if (mapLayer.LayerIsOnByDefault) {
                this.map.addLayer(this.overlayLayers[mapLayer.ExternalMapLayerDisplayName]);
            }
        });
    }

    private addCloseButtonToLayerControl(layerControl: any) {
        const closeButton = document.createElement("button");
        closeButton.textContent = "Collapse layers";
        closeButton.classList.add("leaflet-control-layers-close");
        closeButton.onclick = () => {
            this.closeLayerControl();
        };
        this.layerControl.getContainer().append(closeButton);
    }

    private updateSelectedWellLayer(wellID: number) {
        const highlightedWell = this.wells.find((x) => x.WellID == wellID);
        const latitude = this.wellLocations.find((x) => x.WellID == wellID).Latitude;
        const longitude = this.wellLocations.find((x) => x.WellID == wellID).Longitude;
        if (!this.highlightedWellID) return;

        if (this.selectedWellMarker) {
            this.map.removeLayer(this.selectedWellMarker);
            this.selectedWellMarker = null;
        }

        this.selectedWellMarker = L.marker([latitude, longitude], { icon: this.selectedWellIcon, zIndexOffset: 20 });
        this.selectedWellMarker.addTo(this.map);
        this.selectedWellMarker.bindPopup(
            `<b>${highlightedWell.WellName ?? "New Well"}</b>` + "<hr />" + `<b>Latitude</b>: ${latitude} <br />` + `<b>Longitude</b>: ${longitude} <br />`
        );
        this.selectedWellMarker.openPopup();
        this.map.panTo(this.selectedWellMarker.getLatLng());
    }

    private customLayerFeatureGroups = {};
    private updateCustomGeoJSONLayers(customGeoJsonLayers: CustomGeoJSONLayer[]) {
        const bounds = L.latLngBounds();
        customGeoJsonLayers.forEach((layer) => {
            // remove the layer and the layer control if it exists
            if (this.layerControl._layers.map((x) => x.name).includes(layer.name)) {
                this.customLayerFeatureGroups[layer.name].remove();
                this.layerControl.removeLayer(this.customLayerFeatureGroups[layer.name]);
            }

            this.customLayerFeatureGroups[layer.name] = L.geoJSON(layer.geometries, {
                style: layer.style,
            }).addTo(this.map);

            this.layerControl.addOverlay(this.customLayerFeatureGroups[layer.name], layer.name);
            bounds.extend(this.customLayerFeatureGroups[layer.name].getBounds());
        });
        this.map.fitBounds(bounds, this.defaultFitBoundsOptions);
    }

    private updateWellLayer() {
        if (this.wellLayer) {
            this.map.removeLayer(this.wellLayer);
            this.wellLayer = null;
        }
        this.wellLayer = L.markerClusterGroup({
            iconCreateFunction: this.leafletHelperService.clusterIconCreateFunction,
        });

        this.wells.forEach((well) => {
            if (well.WellID == this.highlightedWellID || !well.Latitude || !well.Longitude) return;
            // var wellDto = this.wellLocations.find(x => x.WellID == well.WellID)
            const latitude = well.Latitude;
            const longitude = well.Longitude;

            new L.marker([latitude, longitude], { referenceWellID: well.WellID, icon: this.wellIcon })
                .on("click", (event) => (this.highlightWellOnClick ? this.highlightWell(event.target.options.referenceWellID) : null))
                .addTo(this.wellLayer);
        });
        this.wellLayer.addTo(this.map);
        if (this.layerControl) {
            this.layerControl.addOverlay(this.wellLayer, "Wells");
        }
    }

    public closeLayerControl() {
        this.layerControl.collapse();
    }
}

export interface CustomGeoJSONLayer {
    style: any;
    geometries: any;
    name: string;
}
