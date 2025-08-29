import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output, SimpleChange } from "@angular/core";
import * as L from "leaflet";

import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { ReplaySubject } from "rxjs";
@Component({
    selector: "parcel-layer",
    templateUrl: "./parcel-layer.component.html",
    styleUrls: ["./parcel-layer.component.scss"],
})
export class ParcelLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    @Input() controlTitle: string = "My Parcels";
    @Input({ required: true }) geographyID: number;
    @Input() reportingPeriodID: number;
    @Input() parcelIDs: number[];
    @Input() selectedParcelID: number;

    @Output() layerBoundsCalculated = new EventEmitter();
    @Output() parcelSelected = new EventEmitter<number>();

    @Output() layerStartedLoading = new EventEmitter();
    @Output() layerFinishedLoading = new EventEmitter();

    public isLoading: boolean = false;
    public layer;

    private defaultStyle = {
        color: "#3388ff",
        weight: 2,
        opacity: 0.65,
        fillOpacity: 0.1,
        zIndex: 9999,
    };

    private highlightStyle = {
        color: "#fcfc12",
        weight: 2,
        opacity: 0.65,
        fillOpacity: 0.1,
        zIndex: 9999,
    };

    private selectedFromMap: boolean = false;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngOnChanges(changes: any): void {
        if (changes.selectedParcelID) {
            if (this.selectedFromMap) {
                this.selectedFromMap = false;
                return;
            }

            if (changes.selectedParcelID.previousValue == changes.selectedParcelID.currentValue) {
                return;
            }

            this.highlightSelectedParcel(true);
        } else if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)) {
            this.updateLayer(true);
        }
    }

    ngAfterViewInit(): void {
        this.setupLayer();
        this.updateLayer(true);
    }

    updateLayer(firstLoad: boolean) {
        this.layerStartedLoading.emit();
        this.isLoading = true;
        this.layer.clearLayers();

        let cql_filter = `GeographyID = ${this.geographyID}`;
        if (this.parcelIDs?.length > 0) {
            cql_filter += ` and ParcelID in (${this.parcelIDs.join(",")})`;
        }

        if (this.reportingPeriodID) {
            cql_filter += ` and ReportingPeriodID = ${this.reportingPeriodID}`;
        } else {
            cql_filter += ` and IsCurrent = 1`;
        }

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllParcels", cql_filter).subscribe((response) => {
            if (response.length == 0) return;

            response.forEach((feature: any) => {
                const geoJson = L.geoJSON(feature, {
                    style: this.defaultStyle,
                });

                //IMPORTANT: THIS ONLY WORKS BECAUSE I'VE INSTALLED @angular/elements AND CONFIGURED THIS IN THE app.module.ts bootstrapping
                geoJson.bindPopup(
                    `<parcel-popup-custom-element parcel-id="${feature.properties.ParcelID}" reporting-period-id="${feature.properties.ReportingPeriodID}"></parcel-popup-custom-element>`,
                    {
                        maxWidth: 475,
                        keepInView: false,
                        autoPan: false,
                    }
                );

                geoJson.on("mouseover", (e) => {
                    geoJson.setStyle({ fillOpacity: 0.5 });
                });

                geoJson.on("mouseout", (e) => {
                    geoJson.setStyle({ fillOpacity: 0.1 });
                });

                geoJson.on("click", (e) => {
                    this.selectedFromMap = true;
                    this.onParcelSelected(Number(feature.properties.ParcelID));
                });

                this.layer.addLayer(geoJson);
            });

            if (!firstLoad || this.displayOnLoad) {
                this.layer.addTo(this.map);
                this.map.fitBounds(this.layer.getBounds());
            }

            if (this.selectedParcelID) {
                this.highlightSelectedParcel();
            }

            this.isLoading = false;
            this.layerFinishedLoading.emit();
        });
    }

    private onParcelSelected(parcelID: number) {
        this.selectedParcelID = parcelID;
        this.highlightSelectedParcel();

        this.parcelSelected.emit(parcelID);
    }

    private highlightSelectedParcel(zoomToFeature: boolean = false) {
        if (!this.layer) return;

        // clear styles
        this.layer.setStyle(this.defaultStyle);
        this.map.closePopup();

        // loop through the allWaterAccountsFeatureGroup
        this.layer.eachLayer((layer) => {
            const geoJsonLayers = layer.getLayers();
            if (geoJsonLayers[0].feature.properties.ParcelID == this.selectedParcelID) {
                layer.setStyle(this.highlightStyle);

                if (zoomToFeature) {
                    this.map.fitBounds(geoJsonLayers[0].getBounds());
                }

                layer.openPopup();
            }
        });
    }

    private setupLayer() {
        this.layer = L.geoJSON();
        this.initLayer();
    }
}
