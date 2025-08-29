import { Component, EventEmitter, Input, Output, SimpleChange } from "@angular/core";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";

@Component({
    selector: "usage-location-layer",
    imports: [],
    templateUrl: "./usage-location-layer.component.html",
    styleUrls: ["./usage-location-layer.component.scss"],
})
export class UsageLocationLayerComponent extends MapLayerBase {
    @Input({ required: true }) geographyID: number;
    @Input() reportingPeriodID: number;
    @Input() parcelIDs: number[];
    @Input() usageLocationIDs: number[];
    @Input() selectedUsageLocationID: number;
    @Input() displayFallowed: boolean = false;
    @Input() displayCoverCropped: boolean = false;

    @Output() usageLocationSelected = new EventEmitter<number>();

    @Output() layerStartedLoading = new EventEmitter();
    @Output() layerFinishedLoading = new EventEmitter();

    public isLoading: boolean = false;

    public wmsOptions: L.WMSOptions;
    public layer: L.featureGroup;

    public defaultStyle = { fillOpacity: 0.05 };
    public highlightStyle = { color: "#0d5b73", fillOpacity: 0.25 };
    public selectedFromMap: boolean = false;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngAfterViewInit(): void {
        this.layer = new L.geoJSON();
        this.initLayer();
        this.updateLayer();
    }

    ngOnChanges(changes: any): void {
        if (changes.selectedUsageLocationID) {
            if (this.selectedFromMap) {
                this.selectedFromMap = false;
                return;
            }

            if (changes.selectedUsageLocationID.previousValue == changes.selectedUsageLocationID.currentValue) {
                return;
            }
            this.selectedUsageLocationID = changes.selectedUsageLocationID.currentValue;
            this.highlightSelectedUsageLocation(true);
        } else if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)) {
            this.updateLayer();
        }
    }

    updateLayer() {
        this.layer.clearLayers();
        this.addUsageLocationsToLayer();
    }

    addUsageLocationsToLayer() {
        this.isLoading = true;
        this.layerStartedLoading.emit();

        let cql_filter = `GeographyID = ${this.geographyID}`;
        if (this.parcelIDs?.length > 0) {
            cql_filter += ` and ParcelID in(${this.parcelIDs.join(",")})`;
        }

        if (this.reportingPeriodID) {
            cql_filter += ` and ReportingPeriodID = ${this.reportingPeriodID}`;
        } else {
            cql_filter += ` and IsCurrent = 1`;
        }

        if (this.usageLocationIDs?.length > 0) {
            cql_filter += ` and UsageLocationID in(${this.usageLocationIDs.join(",")})`;
        }

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllUsageLocations", cql_filter).subscribe((response) => {
            for (let i = 0; i < response.length; i++) {
                const geoJSONFeature = response[i] as any;
                let style = { color: geoJSONFeature.properties.UsageLocationTypeColor, fillOpacity: this.defaultStyle.fillOpacity };
                const geoJSONLayer = new L.geoJson(geoJSONFeature, { style: style });

                // IMPORTANT: THIS ONLY WORKS BECAUSE I'VE INSTALLED @angular/elements AND CONFIGURED THIS IN THE app.module.ts bootstrapping
                geoJSONLayer.bindPopup(
                    `<usage-location-popup-custom-element geography-id="${geoJSONFeature.properties.GeographyID}" parcel-id="${geoJSONFeature.properties.ParcelID}" usage-location-id="${geoJSONFeature.properties.UsageLocationID}"></usage-location-popup-custom-element>`,
                    {
                        maxWidth: 475,
                        keepInView: false,
                        autoPan: false,
                    }
                );

                geoJSONLayer.on("mouseover", (e) => {
                    geoJSONLayer.setStyle({ fillOpacity: 0.5 });
                });

                geoJSONLayer.on("mouseout", (e) => {
                    geoJSONLayer.setStyle({ fillOpacity: 0.25 });
                });

                geoJSONLayer.on("click", (e) => {
                    this.selectedFromMap = true;
                    this.onUsageLocationSelected(e.layer.feature.properties.UsageLocationID, false);
                });

                geoJSONLayer.addTo(this.layer);
            }

            const bounds = this.layer.getBounds();
            this.map.fitBounds(bounds);
            this.isLoading = false;
            this.layerFinishedLoading.emit();
        });
    }

    private onUsageLocationSelected(usageLocationID: number, zoomToFeature: boolean = false) {
        this.selectedUsageLocationID = usageLocationID;
        this.highlightSelectedUsageLocation(zoomToFeature);
        this.usageLocationSelected.emit(usageLocationID);
    }

    private highlightSelectedUsageLocation(zoomToFeature: boolean = false) {
        this.map.closePopup();

        this.layer.eachLayer((layer) => {
            const geoJsonLayers = layer.getLayers();

            if (geoJsonLayers[0].feature.properties.UsageLocationID == this.selectedUsageLocationID) {
                layer.setStyle(this.highlightStyle);
                layer.openPopup();
                if (zoomToFeature) {
                    this.map.fitBounds(geoJsonLayers[0].getBounds());
                }
            } else {
                let style = { color: geoJsonLayers[0].feature.properties.UsageLocationTypeColor, fillOpacity: this.highlightStyle.fillOpacity };
                layer.setStyle(style);
            }
        });
    }
}
