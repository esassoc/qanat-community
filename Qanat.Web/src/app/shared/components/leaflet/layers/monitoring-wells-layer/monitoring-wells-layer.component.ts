import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import {
    MonitoringWellContext,
    MonitoringWellMeasurementChartComponent,
} from "src/app/shared/components/monitoring-wells/modal/monitoring-well-measurement-chart/monitoring-well-measurement-chart.component";
import { AfterViewInit, Component, Input, OnChanges } from "@angular/core";

@Component({
    selector: "monitoring-wells-layer",
    standalone: true,
    imports: [],
    templateUrl: "./monitoring-wells-layer.component.html",
    styleUrl: "./monitoring-wells-layer.component.scss",
})
export class MonitoringWellsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges {
    @Input() layerControl: L.Control;
    @Input() editingMap: boolean = false;

    public layer: L.Layer;

    constructor(
        private wfsService: WfsService,
        private modalService: ModalService
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
        const cqlFilter = "GeographyID is not null";

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:MonitoringWells", cqlFilter).subscribe((response) => {
            const geoJson = L.geoJSON(response, {
                pointToLayer: (feature, latlng) => {
                    return L.circleMarker(latlng, geojsonMonitoringWellMarkerOptions);
                },
                interactive: !this.editingMap,
            });

            geoJson.on("click", (e) => {
                if (this.editingMap) return;

                this.modalService
                    .open(MonitoringWellMeasurementChartComponent, null, { ModalSize: ModalSizeEnum.ExtraLarge, ModalTheme: ModalThemeEnum.Light, TopLayer: false }, {
                        GeographyID: e.layer.feature.properties.GeographyID,
                        SiteCode: e.layer.feature.properties.SiteCode,
                        MonitoringWellName: e.layer.feature.properties.MonitoringWellName,
                    } as MonitoringWellContext)
                    .instance.result.then((result) => {});
            });

            geoJson.addTo(this.layer);
        });
    }

    setupLayer() {
        this.layer = L.featureGroup();
        this.initLayer();
    }
}
