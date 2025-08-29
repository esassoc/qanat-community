import { AfterViewInit, Component, Input, OnChanges, SimpleChange } from "@angular/core";
import * as esri from "esri-leaflet";
import "esri-leaflet-renderers";

import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { ExternalMapLayerSimpleDto } from "src/app/shared/generated/model/external-map-layer-simple-dto";
import { ExternalMapLayerTypeEnum } from "src/app/shared/generated/enum/external-map-layer-type-enum";
@Component({
    selector: "geography-external-map-layer",
    templateUrl: "./geography-external-map-layer.component.html",
    styleUrls: ["./geography-external-map-layer.component.scss"],
})
export class GeographyExternalMapLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
    public isLoading: boolean = false;
    @Input({ required: true }) externalMapLayer: ExternalMapLayerSimpleDto;

    public layer;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngOnChanges(changes: any): void {
        if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)) {
            this.updateLayer();
        }
    }

    ngAfterViewInit(): void {
        this.setupLayer();
        // this.updateLayer();
    }

    updateLayer() {
        this.layer.clearLayers();

        // const cql_filter = `GeographyID = ${this.geographyID} and WaterAccountID = ${this.waterAccountID}`;

        // this.wfsService.getGeoserverWFSLayer(null, 'Qanat:AllParcels', cql_filter)
        //   .subscribe(response => {
        //     if (response.length > 0) {
        //       const geoJson = L.geoJSON(response, {
        //         style: (feature) => {
        //           if(this.highlightedParcelIDs?.includes(feature.properties.ParcelID)){
        //             return this.parcelHighlightedStyle;
        //           }
        //           return this.parcelStyle;
        //         }
        //       });
        //       geoJson.addTo(this.layer);
        //       const bounds = this.layer.getBounds();
        //       this.map.fitBounds(bounds);
        //     }
        //     this.isLoading = false;
        //   });
    }

    setupLayer() {
        switch (this.externalMapLayer.ExternalMapLayerTypeID) {
            case ExternalMapLayerTypeEnum.ESRIMapServer:
                this.layer = esri.tiledMapLayer({
                    url: this.externalMapLayer.ExternalMapLayerURL,
                    useCors: false,
                    isModern: false,
                });
                break;
            case ExternalMapLayerTypeEnum.ESRIFeatureServer:
                this.layer = esri.featureLayer({
                    url: this.externalMapLayer.ExternalMapLayerURL,
                    useCors: false,
                    interactive: true,
                    minZoom: this.externalMapLayer.MinZoom,
                    isModern: false, // set to false makes the request/response type 'application/javascript' instead of 'applicaiton/geo+json' which was having CORS/CORB issues
                    onEachFeature: (feature, layer) => {
                        if (this.externalMapLayer.PopUpField != null && this.externalMapLayer.PopUpField != "") {
                            layer.bindPopup(`${this.externalMapLayer.PopUpField}: ${feature.properties[this.externalMapLayer.PopUpField]}`);
                        }
                    },
                });
                break;
            default:
                console.error(`Invalid ExternalMapLayerTypeEnum ${this.externalMapLayer.ExternalMapLayerTypeID}.`);
        }

        this.initLayer();
    }
}
