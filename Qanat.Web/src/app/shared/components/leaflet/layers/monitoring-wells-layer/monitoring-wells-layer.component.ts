import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { MonitoringWellMeasurementChartComponent } from "src/app/shared/components/monitoring-wells/modal/monitoring-well-measurement-chart/monitoring-well-measurement-chart.component";
import { AfterViewInit, Component, Input, OnChanges } from "@angular/core";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "monitoring-wells-layer",
    imports: [],
    templateUrl: "./monitoring-wells-layer.component.html",
    styleUrl: "./monitoring-wells-layer.component.scss",
})
export class MonitoringWellsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges {
    @Input() layerControl: L.Control;
    @Input() editingMap: boolean = false;
    @Input() geographyID?: number;

    public layer: L.Layer;

    constructor(
        private wfsService: WfsService,
        private dialogService: DialogService
    ) {
        super();
    }

    ngAfterViewInit(): void {
        if (!this.layer) this.setupLayer();
        this.updateLayer();
    }

    ngOnChanges(changes: any): void {
        if (changes.editingMap) {
            this.updateLayer();
        }
    }

    updateLayer() {
        this.layer?.clearLayers();

        const geojsonMonitoringWellMarkerOptions = {
            radius: 4,
            fillColor: "#ff7800",
            color: "#000",
            weight: 1,
            opacity: 1,
            fillOpacity: 0.8,
            interactive: !this.editingMap,
        };
        const cqlFilter = this.geographyID ? `GeographyID = ${this.geographyID}` : "GeographyID is not null";

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:MonitoringWells", cqlFilter).subscribe((response) => {
            const geoJson = L.geoJSON(response, {
                pointToLayer: (feature, latlng) => {
                    return L.circleMarker(latlng, geojsonMonitoringWellMarkerOptions);
                },
                interactive: !this.editingMap,
            });

            geoJson.on("click", (e) => {
                if (this.editingMap) return;
                const dialogRef = this.dialogService.open(MonitoringWellMeasurementChartComponent, {
                    data: {
                        GeographyID: e.layer.feature.properties.GeographyID,
                        SiteCode: e.layer.feature.properties.SiteCode,
                        MonitoringWellName: e.layer.feature.properties.MonitoringWellName,
                    },
                    size: "lg",
                });

                dialogRef.afterClosed$.subscribe((result) => {
                    if (result) {
                    }
                });
            });

            geoJson.addTo(this.layer);
        });
    }

    setupLayer() {
        this.layer = L.featureGroup();
        this.initLayer();
    }
}
