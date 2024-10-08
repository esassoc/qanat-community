import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { Map } from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { WfsService } from "src/app/shared/services/wfs.service";
import { ReplaySubject, Subscription, debounceTime } from "rxjs";
@Component({
    selector: "highlighted-parcels-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./highlighted-parcels-layer.component.html",
    styleUrls: ["./highlighted-parcels-layer.component.scss"],
})
export class HighlightedParcelsLayerComponent extends MapLayerBase implements OnChanges, OnDestroy {
    public isLoading: boolean = false;
    @Input() controlTitle: string = "Selected Parcel(s)";
    @Input() geographyID: number;
    @Output() onHighlightedParcelClick: EventEmitter<ClickedOnHighlightedParcelEvent> = new EventEmitter();
    @Output() onNonHighlightedParcelClick: EventEmitter<ClickedOnNonHighlightedParcelEvent> = new EventEmitter();

    private highlightedParcelIDsSubject = new ReplaySubject<number[]>();
    private _highlightedParcelIDs: number[];
    @Input() set highlightedParcelIDs(value: number[]) {
        this._highlightedParcelIDs = value;
        this.highlightedParcelIDsSubject.next(value);
    }
    get highlightedParcelIDs(): number[] {
        return this._highlightedParcelIDs;
    }
    public geoJsonStyle = { color: "#ffff00" };
    public layer;
    private updateSubscriptionDebounoced = Subscription.EMPTY;

    constructor(private wfsService: WfsService) {
        super();
    }

    ngOnDestroy() {
        this.updateSubscriptionDebounoced.unsubscribe();
    }

    ngAfterViewInit(): void {
        this.map.on("click", (e) => {
            L.DomEvent.stop(e);
            const latLng = e.latlng;
            this.wfsService.getParcelByCoordinate(latLng.lng, latLng.lat, this.geographyID).subscribe((response) => {
                if (response.features.length > 0) {
                    this.clickedOnNonHighlightedParcel(response.features[0].properties.ParcelID, response.features[0].properties.ParcelNumber);
                }
            });
        });

        this.updateSubscriptionDebounoced = this.highlightedParcelIDsSubject
            .asObservable()
            .pipe(debounceTime(100))
            .subscribe((value: number[]) => {
                this._highlightedParcelIDs = value;
                if (!this.layer) this.setupLayer();
                this.updateLayer();
            });
    }

    updateLayer() {
        this.layer.clearLayers();

        let cql_filter = `GeographyID = ${this.geographyID}`;
        cql_filter += this.highlightedParcelIDs.length > 0 ? ` and ParcelID in(${this.highlightedParcelIDs.join(",")})` : " and ParcelID is null";

        this.wfsService.getGeoserverWFSLayer(null, "Qanat:AllParcels", cql_filter).subscribe((response) => {
            if (response.length > 0) {
                const geoJson = L.geoJSON(response, { style: this.geoJsonStyle }).on("click", (e) => this.clickedOnHighlightedParcel(e));
                geoJson.addTo(this.layer);
                const bounds = this.layer.getBounds();
                this.map.fitBounds(bounds);
            }
            this.isLoading = false;
        });
    }

    setupLayer() {
        this.layer = L.geoJSON();
        this.initLayer();
    }

    clickedOnHighlightedParcel(event: any) {
        L.DomEvent.stop(event);
        this.onHighlightedParcelClick.emit(
            new ClickedOnHighlightedParcelEvent(this.map, this.layerControl, event.layer.feature.properties.ParcelID, event.layer.feature.properties.ParcelNumber)
        );
    }

    clickedOnNonHighlightedParcel(parcelID: any, parcelNumber: string) {
        this.onNonHighlightedParcelClick.emit(new ClickedOnNonHighlightedParcelEvent(this.map, this.layerControl, parcelID, parcelNumber));
    }
}

export class ClickedOnHighlightedParcelEvent {
    public map: Map;
    public layerControl: any;
    public parcelID: number;
    public parcelNumber: string;
    constructor(map: Map, layerControl: any, parcelID: number, parcelNumber: string) {
        this.map = map;
        this.layerControl = layerControl;
        this.parcelID = parcelID;
        this.parcelNumber = parcelNumber;
    }
}

export class ClickedOnNonHighlightedParcelEvent {
    public map: Map;
    public layerControl: any;
    public parcelID: number;
    public parcelNumber: string;
    constructor(map: Map, layerControl: any, parcelID: number, parcelNumber: string) {
        this.map = map;
        this.layerControl = layerControl;
        this.parcelID = parcelID;
        this.parcelNumber = parcelNumber;
    }
}
