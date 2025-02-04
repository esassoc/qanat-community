import { CommonModule } from "@angular/common";
import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from "@angular/core";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { ReplaySubject, Subscription, debounceTime } from "rxjs";
import { WellLocationDto } from "src/app/shared/generated/model/well-location-dto";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { WellMinimalDto } from "src/app/shared/generated/model/well-minimal-dto";
@Component({
    selector: "wells-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./wells-layer.component.html",
    styleUrls: ["./wells-layer.component.scss"],
})
export class WellsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges, OnDestroy {
    @Input() controlTitle: string = "My Wells";

    public isLoading: boolean = false;
    @Output() popupOpened: EventEmitter<OpenedWellPopupEvent> = new EventEmitter();

    private wellsSubject = new ReplaySubject<WellLocationDto[] | WellMinimalDto[]>();
    private _wells: WellLocationDto[] | WellMinimalDto[];
    @Input() set wells(value: WellLocationDto[] | WellMinimalDto[]) {
        this._wells = value;
        this.wellsSubject.next(value);
    }
    get wells(): WellLocationDto[] | WellMinimalDto[] {
        return this._wells;
    }
    private _highlightedWellID: number = null;
    @Input() set highlightedWellID(value: number) {
        if (this.highlightedWellID != value) {
            this._highlightedWellID = value;
            this.popupOpened.emit(new OpenedWellPopupEvent(this.map, this.layerControl, value));
            this.changedWell(value);
        }
    }
    get highlightedWellID(): number {
        return this._highlightedWellID;
    }
    private wellIcon = this.leafletHelperService.blueIconLarge;
    public layer: L.Layer;
    private updateSubscriptionDebounoced = Subscription.EMPTY;

    constructor(private leafletHelperService: LeafletHelperService) {
        super();
    }

    ngOnDestroy() {
        this.updateSubscriptionDebounoced.unsubscribe();
    }

    ngAfterViewInit(): void {
        this.updateSubscriptionDebounoced = this.wellsSubject
            .asObservable()
            .pipe(debounceTime(100))
            .subscribe((value: WellLocationDto[]) => {
                this._wells = value;
                if (!this.layer) this.setupLayer();
                this.updateLayer();
            });
    }

    updateLayer() {
        this.layer.clearLayers();

        if (this.wells.length == 0) return;

        const markers = this.wells.map((well) => {
            const latLng = L.latLng(well.Latitude, well.Longitude);
            return new L.marker(latLng, { icon: this.wellIcon, zIndexOffset: 1000, interactive: true, title: well.WellID })
                .bindPopup(`<well-popup-custom-element well-id="${well.WellID}"></well-popup-custom-element>`, {
                    maxWidth: 475,
                    keepInView: true,
                })
                .on("popupopen", (e) => {
                    this.popupOpened.emit(new OpenedWellPopupEvent(this.map, this.layerControl, well.WellID));
                    this.highlightedWellID = well.WellID;
                    this.map.fitBounds(latLng);
                })
                .on("click", (e) => {
                    this.changedWell(Number(well.WellID));
                });
        });

        markers.forEach((marker) => {
            marker.addTo(this.layer);
        });
        this.map.fitBounds(this.layer.getBounds());
    }

    changedWell(wellID: number) {
        this.map.closePopup();
        this.highlightedWellID = wellID;

        this.layer.eachLayer((layer) => {
            if (layer.options.title == wellID) {
                layer.openPopup();
                this.map.fitBounds(layer.getBounds());
            }
        });
    }

    setupLayer() {
        this.layer = L.featureGroup();
        this.initLayer();
    }
}

export class OpenedWellPopupEvent {
    public map: L.Map;
    public layerControl: L.LayerControl;
    public wellID: number;
    constructor(map: L.Map, layerControl: L.LayerControl, wellID: number) {
        this.map = map;
        this.layerControl = layerControl;
        this.wellID = wellID;
    }
}
