import { CommonModule } from "@angular/common";
import { AfterViewInit, Component, Input, OnChanges, OnDestroy } from "@angular/core";
import * as L from "leaflet";
import { MapLayerBase } from "../map-layer-base.component";
import { ReplaySubject, Subscription, debounceTime } from "rxjs";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { WellRegistrationLocationDto } from "src/app/shared/generated/model/well-registration-location-dto";
@Component({
    selector: "well-registrations-layer",
    standalone: true,
    imports: [CommonModule, MapLayerBase],
    templateUrl: "./well-registrations-layer.component.html",
    styleUrls: ["./well-registrations-layer.component.scss"],
})
export class WellRegistrationsLayerComponent extends MapLayerBase implements AfterViewInit, OnChanges, OnDestroy {
    public isLoading: boolean = false;

    private wellRegistrationsSubject = new ReplaySubject<WellRegistrationLocationDto[]>();
    private _wellRegistrations: WellRegistrationLocationDto[];
    @Input() set wellRegistrations(value: WellRegistrationLocationDto[]) {
        this._wellRegistrations = value;
        this.wellRegistrationsSubject.next(value);
    }
    get wellRegistrations(): WellRegistrationLocationDto[] {
        return this._wellRegistrations;
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
        this.updateSubscriptionDebounoced = this.wellRegistrationsSubject
            .asObservable()
            .pipe(debounceTime(100))
            .subscribe((value: WellRegistrationLocationDto[]) => {
                this._wellRegistrations = value;
                if (!this.layer) this.setupLayer();
                this.updateLayer();
            });
    }

    updateLayer() {
        this.layer.clearLayers();

        const markers = this.wellRegistrations.map((well) => {
            const latLng = L.latLng(well.Latitude, well.Longitude);
            return new L.marker(latLng, { icon: this.wellIcon, zIndexOffset: 1000, interactive: true, title: well.WellRegistrationID });
        });

        markers.forEach((marker) => {
            marker.addTo(this.layer);
        });
        this.map.fitBounds(this.layer.getBounds());
    }

    setupLayer() {
        this.layer = L.featureGroup();
        this.initLayer();
    }
}
