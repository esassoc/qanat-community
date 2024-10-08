import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Observable, Subject, catchError, debounceTime, distinctUntilChanged, filter, of, switchMap, tap } from "rxjs";
import { ParcelDisplayDto } from "src/app/shared/generated/model/parcel-display-dto";
import * as L from "leaflet";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import {
    ClickedOnHighlightedParcelEvent,
    ClickedOnNonHighlightedParcelEvent,
} from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { FormsModule } from "@angular/forms";
import { NgSelectModule } from "@ng-select/ng-select";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { GeographyParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/geography-parcels-layer/geography-parcels-layer.component";
import { NgIf, NgFor, NgClass, AsyncPipe } from "@angular/common";
import { QanatMapComponent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";

@Component({
    selector: "well-irrigated-parcels-edit-map",
    templateUrl: "./well-irrigated-parcels-edit-map.component.html",
    styleUrl: "./well-irrigated-parcels-edit-map.component.scss",
    standalone: true,
    imports: [QanatMapComponent, NgIf, GeographyParcelsLayerComponent, HighlightedParcelsLayerComponent, NgSelectModule, FormsModule, NgFor, NgClass, AsyncPipe],
})
export class WellIrrigatedParcelsEditMapComponent implements OnInit {
    @Input() geographyID: number;
    @Input() wellLatLng?: L.latlng;

    private _wellIrrigatedParcels: ParcelDisplayDto[];
    @Input() set wellIrrigatedParcels(value: ParcelDisplayDto[]) {
        this._wellIrrigatedParcels = value;
        this.reset();
    }

    @Output() selectionChanged = new EventEmitter<number[]>();

    public wellID: number;
    public irrigatedParcels: ParcelDisplayDto[];

    // parcels that have been selected from the typeahead
    public selectedParcels: ParcelDisplayDto[] = [];

    // recent activity variables
    public isIrrigatedParcel: { [ParcelID: number]: boolean } = {};
    public recentlyAddedCount = 0;

    // typeahead variables
    public parcelSearchModel: ParcelDisplayDto[];
    public parcels$: Observable<ParcelDisplayDto[]>;
    public parcelInputs$ = new Subject<string>();
    public searchLoading = false;

    // map variables
    public highlightedParcelID: number;
    public irrigatedParcelIDs: number[] = [];
    public map: L.Map;
    public mapIsReady = false;
    public layerControl: L.layerControl;

    constructor(
        private parcelService: ParcelService,
        private leafletHelperService: LeafletHelperService,
        private cdr: ChangeDetectorRef
    ) {}

    public ngOnInit(): void {
        this._wellIrrigatedParcels.forEach((parcel) => {
            this.isIrrigatedParcel[parcel.ParcelID] = true;
            this.irrigatedParcelIDs = [...this.irrigatedParcelIDs, parcel.ParcelID];
        });
        this.irrigatedParcels = this._wellIrrigatedParcels;

        // parcel typeahead search
        this.parcels$ = this.parcelInputs$.pipe(
            filter((searchTerm) => searchTerm != null),
            distinctUntilChanged(),
            tap(() => (this.searchLoading = true)),
            debounceTime(800),
            switchMap((searchTerm) =>
                this.parcelService.geographiesGeographyIDParcelsSearchSearchStringGet(this.geographyID, searchTerm).pipe(
                    catchError(() => of([])),
                    tap(() => (this.searchLoading = false))
                )
            )
        );
    }

    public addWellToMap() {
        if (this.mapIsReady) {
            const wellMarker = new L.marker(L.latLng(this.wellLatLng), { icon: this.leafletHelperService.yellowIcon, zIndexOffset: 1000, interactive: false }).addTo(this.map);

            if (this.selectedParcels.length == 0) {
                this.leafletHelperService.zoomToMarker(this.map, wellMarker);
            }
        }
    }

    public handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
        this.addWellToMap();
    }

    public onSelected(selectedParcel: ParcelDisplayDto) {
        // if parcel has already been selected, bump it to the top of the list
        const index = this.selectedParcels.findIndex((x) => x.ParcelID == selectedParcel.ParcelID);
        if (index > -1) {
            this.selectedParcels.splice(index, 1);
        }

        this.selectedParcels.splice(0, 0, selectedParcel);

        // prevent default select behavior, since typeahead is serving as a search bar
        this.parcelSearchModel = [];
    }

    public addIrrigatedParcel(parcel: ParcelDisplayDto, selectedParcelIndex: number) {
        if (selectedParcelIndex > -1) {
            this.selectedParcels.splice(selectedParcelIndex, 1);
        }

        this.irrigatedParcels.splice(0, 0, parcel);
        this.isIrrigatedParcel[parcel.ParcelID] = true;

        this.irrigatedParcelIDs.push(parcel.ParcelID);
        this.updateIrrigatedParcelIDs();

        this.recentlyAddedCount++;
    }

    public removeIrrigatedParcel(parcel: ParcelDisplayDto, associatedParcelIndex: number, associatedParcelIDsIndex?: number) {
        this.irrigatedParcels.splice(associatedParcelIndex, 1);
        this.selectedParcels.splice(0, 0, parcel);
        this.isIrrigatedParcel[parcel.ParcelID] = false;

        if (associatedParcelIndex < this.recentlyAddedCount) {
            this.recentlyAddedCount--;
        }

        if (associatedParcelIDsIndex == null) {
            associatedParcelIDsIndex = this.irrigatedParcelIDs.findIndex((x) => x == parcel.ParcelID);
        }

        this.irrigatedParcelIDs.splice(associatedParcelIDsIndex, 1);
        this.updateIrrigatedParcelIDs();
    }

    public clickedOnHighlightedParcel(e: ClickedOnHighlightedParcelEvent) {
        const associatedParcelIDsIndex = this.irrigatedParcelIDs.findIndex((x) => x == e.parcelID);
        const parcel = new ParcelDisplayDto({ ParcelID: e.parcelID, ParcelNumber: e.parcelNumber });
        if (associatedParcelIDsIndex > -1) {
            const associatedParcelIndex = this.irrigatedParcels.findIndex((x) => x.ParcelID == e.parcelID);
            this.removeIrrigatedParcel(parcel, associatedParcelIndex, associatedParcelIDsIndex);
        }
    }

    public clickedOnNonHighlightedParcel(e: ClickedOnNonHighlightedParcelEvent) {
        const parcel = new ParcelDisplayDto({ ParcelID: e.parcelID, ParcelNumber: e.parcelNumber });
        const selectedParcelIndex = this.selectedParcels.findIndex((x) => x.ParcelID == e.parcelID);
        this.addIrrigatedParcel(parcel, selectedParcelIndex);
    }

    public highlightParcel(parcelID: number) {
        this.highlightedParcelID = parcelID;
    }

    public updateIrrigatedParcelIDs() {
        this.irrigatedParcelIDs = [...this.irrigatedParcelIDs];

        this.selectionChanged.emit(this.irrigatedParcelIDs);
    }

    private reset() {
        this.selectedParcels = [];
        this.irrigatedParcelIDs = [];
        this.recentlyAddedCount = 0;
    }
}
