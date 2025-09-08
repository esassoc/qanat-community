import { Component, EventEmitter, Input, Output, SimpleChanges } from "@angular/core";
import { MapLayerBase } from "../map-layer-base.component";
import * as L from "leaflet";
import { WfsService } from "src/app/shared/services/wfs.service";
import { WaterMeasurementQualityAssuranceDto } from "src/app/shared/generated/model/water-measurement-quality-assurance-dto";
import { environment } from "src/environments/environment";

@Component({
    selector: "water-measurement-quality-assurance-layer",
    imports: [],
    templateUrl: "./water-measurement-quality-assurance-layer.component.html",
    styleUrl: "./water-measurement-quality-assurance-layer.component.scss"
})
export class WaterMeasurementQualityAssuranceLayerComponent extends MapLayerBase {
    @Input({ required: true }) geographyID: number;
    @Input({ required: true }) reportingPeriodID: number;
    @Input({ required: true }) waterMeasurementTypeName: string;
    @Input({ required: true }) waterMeasurements: WaterMeasurementQualityAssuranceDto[];
    @Input() selectedUsageLocationIDs: number[] = [];

    @Output() usageLocationSelected = new EventEmitter<number>();
    @Output() layerStartedLoading = new EventEmitter();
    @Output() layerFinishedLoading = new EventEmitter();

    public styles = [
        { color: "#ffffb2", fillOpacity: 0.5, weight: 0.5, label: "Very Low" },
        { color: "#fed976", fillOpacity: 0.5, weight: 0.5, label: "Low" },
        { color: "#feb24c", fillOpacity: 0.5, weight: 0.5, label: "Moderate" },
        { color: "#fd8d3c", fillOpacity: 0.5, weight: 0.5, label: "High" },
        { color: "#f03b20", fillOpacity: 0.5, weight: 0.5, label: "Very High" },
        { color: "#bd0026", fillOpacity: 0.5, weight: 0.5, label: "Extreme" },
    ];

    public noDataStyle = { color: "#000000", fillOpacity: 0.5, weight: 0.5 };
    public selectedStyle = { color: "#0d5b73", fillOpacity: 0.5, weight: 0.5 };

    public isLoading: boolean = false;
    private layerAddedToMap = false;

    public wmsOptions: L.WMSOptions;
    public layer: L.featureGroup;
    private labelLayer: L.TileLayer;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngAfterViewInit(): void {
        this.layer = new L.geoJSON();
        this.initLayer();

        this.updateLayer();
        this.addLegend();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.geographyID || changes.reportingPeriodID || changes.waterMeasurementTypeName || changes.waterMeasurements) {
            this.updateLayer();
        }

        if (changes.selectedUsageLocationIDs) {
            this.updateSelectedFeatures();
        }
    }

    addLegend() {
        let existingLegend = document.getElementById("water-measurement-legend");
        if (existingLegend) {
            existingLegend.remove();
        }

        let legend = L.control({ position: "bottomright" });
        let styles = this.styles;
        legend.onAdd = function () {
            let div = L.DomUtil.create("div", "info legend");
            div.id = "water-measurement-legend";

            div.innerHTML = `<h4>Depth (ac-ft/ac)</h4>`;

            for (let i = 0; i < styles.length; i++) {
                div.innerHTML += `<div><i style="background:${styles[i].color}"></i> ${styles[i].label}</div>`;
            }

            // Add no-data box
            div.innerHTML += `<i style="background:black"></i> No Data`;
            return div;
        };

        setTimeout(() => {
            legend.addTo(this.map);
        }, 0);
    }

    updateLayer() {
        if (this.layer) {
            this.addUsageLocationsToLayer();
        }
    }

    addUsageLocationsToLayer() {
        this.isLoading = true;
        this.layerStartedLoading.emit();

        let cql_filter = `GeographyID = ${this.geographyID} and ReportingPeriodID = ${this.reportingPeriodID}`;

        this.map.createPane("labelsPane");
        this.map.getPane("labelsPane").style.zIndex = "650";

        this.wmsOptions = {
            layers: "Qanat:AllUsageLocations",
            transparent: true,
            format: "image/png",
            tiled: true,
            styles: "UsageLocationLabels",
            pane: "labelsPane",
        };

        this.wmsOptions.cql_filter = cql_filter;

        if (this.labelLayer) {
            this.map.removeLayer(this.labelLayer);
        }

        this.labelLayer = L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", this.wmsOptions);
        this.map.addLayer(this.labelLayer);
        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllUsageLocations", cql_filter).subscribe((response) => {
            this.layer.clearLayers();
            for (let i = 0; i < response.length; i++) {
                let geoJSONFeature = response[i] as any;
                let waterMeasurement = this.waterMeasurements.find((wm) => wm.UsageLocationID === geoJSONFeature.properties.UsageLocationID);

                let styleToUse = this.noDataStyle;
                if (waterMeasurement != null && (waterMeasurement.PercentileBucket || waterMeasurement.PercentileBucket === 0)) {
                    styleToUse = this.styles[waterMeasurement.PercentileBucket];
                }

                let geoJSONLayer = new L.geoJson(geoJSONFeature, { style: styleToUse });

                geoJSONLayer.on("click", (e) => {
                    this.onUsageLocationSelected(e.layer.feature.properties.UsageLocationID);
                });

                geoJSONLayer.addTo(this.layer);
            }

            let bounds = this.layer.getBounds();
            if (bounds.isValid()) {
                this.map.fitBounds(bounds);
            }

            this.isLoading = false;
            this.layerFinishedLoading.emit();
            // Force label layer to top
            this.labelLayer.bringToFront();
            this.labelLayer.setZIndex(1001);
            this.layer.setZIndex(1000);
        });

        this.map.on("layeradd", (e: any) => {
            if (e.layer === this.layer) {
                this.map.addLayer(this.labelLayer);
            }
        });

        this.map.on("layerremove", (e: any) => {
            if (e.layer === this.layer) {
                this.labelLayer.remove();
            }
        });
    }

    private onUsageLocationSelected(usageLocationID: number) {
        this.usageLocationSelected.emit(usageLocationID);
    }

    private updateSelectedFeatures() {
        if (!this.layer || !this.selectedUsageLocationIDs) {
            return;
        }

        this.layer.eachLayer((layer: any) => {
            let geoJSONLayers = layer.getLayers();
            geoJSONLayers.forEach((geoJSONLayer: any) => {
                let feature = geoJSONLayer.feature;
                if (!feature?.properties?.UsageLocationID) {
                    return;
                }

                let usageLocationID = feature.properties.UsageLocationID;
                let isSelected = this.selectedUsageLocationIDs.includes(usageLocationID);
                if (isSelected) {
                    geoJSONLayer.setStyle(this.selectedStyle);
                    return;
                }

                let waterMeasurement = this.waterMeasurements.find((wm) => wm.UsageLocationID === usageLocationID);
                let styleToUse = this.noDataStyle;
                if (waterMeasurement != null && (waterMeasurement.PercentileBucket || waterMeasurement.PercentileBucket === 0)) {
                    styleToUse = this.styles[waterMeasurement.PercentileBucket];
                }

                geoJSONLayer.setStyle(styleToUse);
            });
        });
    }
}
