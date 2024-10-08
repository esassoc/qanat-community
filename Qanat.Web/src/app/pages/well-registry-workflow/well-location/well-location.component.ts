import { ChangeDetectorRef, Component, OnDestroy, AfterViewInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { WellService } from "src/app/shared/generated/api/well.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import * as L from "leaflet";
import "leaflet.markercluster";
import { GestureHandling } from "leaflet-gesture-handling";
import { BoundingBoxDto } from "src/app/shared/generated/model/bounding-box-dto";
import { environment } from "src/environments/environment";
import { routeParams } from "src/app/app.routes";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { Observable, switchMap, tap } from "rxjs";
import { ReferenceWellMapMarkerDto, WellRegistrationLocationDto } from "src/app/shared/generated/model/models";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { NgIf, AsyncPipe, DecimalPipe } from "@angular/common";
import { ButtonGroupComponent } from "../../../shared/components/button-group/button-group.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowHelpComponent } from "src/app/shared/components/workflow-help/workflow-help.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "well-location",
    templateUrl: "./well-location.component.html",
    styleUrls: ["./well-location.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, WorkflowHelpComponent, WorkflowBodyComponent, AlertDisplayComponent, ButtonGroupComponent, NgIf, ButtonComponent, AsyncPipe, DecimalPipe],
})
export class WellLocationComponent implements AfterViewInit, OnDestroy {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryMapYourWell;
    public wellRegistrationLocation$: Observable<WellRegistrationLocationDto>;
    public referenceWells$: Observable<ReferenceWellMapMarkerDto[]>;

    public selectByReferenceWell: boolean = true;

    public wellRegistrationID: number;
    public wellRegistrationModel: WellRegistrationLocationDto;

    public mapID: string = "wellMap";
    public mapHeight: string = "500px";
    public selectedParcelStyle: string = "parcel_yellow";
    public wellMarker: L.Layer;

    public map: L.Map;
    public layerControl: L.Control.Layers;
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();
    public overlayLayers: { [key: string]: any } = {};
    public boundingBox: BoundingBoxDto;

    private wellIcon = L.icon({
        iconUrl: "/assets/main/map-icons/marker-icon-selected.png",
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        tooltipAnchor: [16, -28],
        shadowSize: [41, 41],
    });

    private selectedWellIcon = L.icon({
        iconUrl: "/assets/main/map-icons/yellow-pin.png",
        shadowUrl: "/assets/main/map-icons/shadow-skew.png",
        iconSize: [28, 45],
        iconAnchor: [15, 45],
        shadowAnchor: [5, 34],
        popupAnchor: [1, -45],
        tooltipAnchor: [16, -28],
        shadowSize: [35, 35],
    });

    public isLoadingSubmit = false;

    constructor(
        private cdr: ChangeDetectorRef,
        private router: Router,
        private route: ActivatedRoute,
        private wellService: WellService,
        private wellRegistrationService: WellRegistrationService,
        private alertService: AlertService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private geographyRouteService: GeographyRouteService
    ) {}

    toggleSelectType(selectByReferenceWell: boolean): void {
        this.selectByReferenceWell = selectByReferenceWell;
        if (!selectByReferenceWell) {
            if (this.clusteredReferenceWellLayer) this.map.removeLayer(this.clusteredReferenceWellLayer);
            if (this.selectedReferenceWellMarker) this.map.removeLayer(this.selectedReferenceWellMarker);
            if (this.wellMarker) this.map.addLayer(this.wellMarker);
            if (this.clusteredReferenceWellLayer) this.layerControl.removeLayer(this.clusteredReferenceWellLayer);
        } else {
            if (this.clusteredReferenceWellLayer) this.map.addLayer(this.clusteredReferenceWellLayer);
            if (this.wellMarker) this.map.removeLayer(this.wellMarker);
            if (this.selectedReferenceWellMarker) this.map.addLayer(this.selectedReferenceWellMarker);
            if (this.clusteredReferenceWellLayer) this.layerControl.addOverlay(this.clusteredReferenceWellLayer, "Well Inventory");
        }
    }

    ngAfterViewInit(): void {
        const defaultParcelsWMSOptions = {
            layers: "Qanat:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true,
        } as L.WMSOptions;

        const parcelsWMSOptions = Object.assign({ styles: "parcel" }, defaultParcelsWMSOptions);

        this.overlayLayers = Object.assign(
            {
                "All Parcels": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", parcelsWMSOptions),
            },
            this.overlayLayers
        );

        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            gestureHandling: true,
        } as L.MapOptions;
        this.map = L.map(this.mapID, mapOptions);
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        this.updateMapBoundingBox();
        this.setControl();

        // register click events
        this.map.on("click", (event: L.LeafletEvent) => this.placeWellMarker(event.latlng));

        this.wellRegistrationLocation$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const wellID = parseInt(paramMap.get(routeParams.wellRegistrationID));
                return this.wellRegistrationService.wellRegistrationsWellRegistrationIDLocationGet(wellID);
            }),
            tap((wellRegistrationLocation) => {
                this.wellRegistrationModel = wellRegistrationLocation;

                const toggleToSelectedWell = this.wellRegistrationModel.ReferenceWellID == null && this.wellRegistrationModel.Latitude && this.wellRegistrationModel.Longitude;
                if (toggleToSelectedWell) this.toggleSelectType(false);

                // this.toggleSelectType(this.wellModel.ReferenceWellID != null)
                if (wellRegistrationLocation.ParcelGeoJson) {
                    this.addParcelLayer(wellRegistrationLocation.ParcelGeoJson);
                }
                if (wellRegistrationLocation.Latitude && wellRegistrationLocation.Longitude) {
                    const wellLatLng = new L.latLng(wellRegistrationLocation.Latitude, wellRegistrationLocation.Longitude);
                    this.placeWellMarker(wellLatLng);

                    if (!wellRegistrationLocation.ParcelID) {
                        this.map.setView(wellLatLng, 16);
                    }
                }

                this.referenceWells$ = this.geographyRouteService.geography$.pipe(
                    switchMap((geography) => {
                        if (wellRegistrationLocation.ParcelID) {
                            this.updateMapBoundingBox(wellRegistrationLocation.BoundingBox);
                        } else {
                            this.updateMapBoundingBox(geography.BoundingBox);
                        }
                        return this.wellService.geographiesGeographyIDReferenceWellsGet(geography.GeographyID);
                    }),
                    tap((referenceWells) => {
                        this.addReferenceWellsToMap(referenceWells);
                        if (this.wellRegistrationModel.ReferenceWellID) {
                            this.updateSelectedWell(referenceWells.find((x) => x.ReferenceWellID == this.wellRegistrationModel.ReferenceWellID));
                        }
                    })
                );
            })
        );
    }

    private clusteredReferenceWellLayer;
    private wellInventoryMarkers;
    private addReferenceWellsToMap(referenceWells: ReferenceWellMapMarkerDto[]) {
        if (this.clusteredReferenceWellLayer) {
            this.map.removeLayer(this.clusteredReferenceWellLayer);
            this.clusteredReferenceWellLayer = null;
        }

        this.clusteredReferenceWellLayer = L.markerClusterGroup({
            iconCreateFunction: (cluster) => {
                const childCount = cluster.getChildCount();
                return new L.DivIcon({ html: "<div><span>" + childCount + "</span></div>", className: "marker-cluster marker-cluster-small", iconSize: new L.Point(40, 40) });
            },
        });

        this.wellInventoryMarkers = referenceWells.map((x) => {
            return new L.marker([x.Latitude, x.Longitude], { well: x, icon: this.wellIcon })
                .on("click", (event) => this.updateSelectedWell(event.target.options.well))
                .addTo(this.clusteredReferenceWellLayer);
        });
        if (this.selectByReferenceWell) {
            this.clusteredReferenceWellLayer.addTo(this.map);
            this.layerControl.addOverlay(this.clusteredReferenceWellLayer, "Well Inventory");
        }
    }

    private selectedReferenceWellMarker;
    public selectedReferenceWell: ReferenceWellMapMarkerDto;
    updateSelectedWell(referenceWell: ReferenceWellMapMarkerDto) {
        if (this.selectedReferenceWellMarker) {
            this.map.removeLayer(this.selectedReferenceWellMarker);
            this.selectedReferenceWellMarker = null;
            this.selectedReferenceWell = null;
        }
        this.selectedReferenceWell = referenceWell;
        this.selectedReferenceWellMarker = new L.Marker([referenceWell.Latitude, referenceWell.Longitude], { icon: this.selectedWellIcon, zIndexOffset: 100 })
            .bindPopup(
                `<b>State WCR No</b>: ${referenceWell.StateWCRNumber ?? "<em>Not Available</em>"} <br />` +
                    `<b>Permit No</b>: ${referenceWell.CountyWellPermitNo ?? "<em>Not Available</em>"} <br />` +
                    "<hr />" +
                    `<b>Latitude</b>: ${referenceWell.Latitude} <br />` +
                    `<b>Longitude</b>: ${referenceWell.Longitude} <br />`
            )
            .addTo(this.map);

        this.clusteredReferenceWellLayer.clearLayers();
        this.clusteredReferenceWellLayer.addLayers(this.wellInventoryMarkers.filter((x) => x.options.well.ReferenceWellID != referenceWell.ReferenceWellID));

        this.selectedReferenceWellMarker.openPopup();
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    private addParcelLayer(geoJsonString: string) {
        const parcelGeoJson = JSON.parse(geoJsonString);
        L.geoJSON(parcelGeoJson, { style: { color: "#FFFF85" }, interactive: false }).addTo(this.map);
    }

    private updateMapBoundingBox(boundingBox?: BoundingBoxDto) {
        if (!this.boundingBox) {
            this.boundingBox = new BoundingBoxDto();
        }

        this.boundingBox.Left = boundingBox ? boundingBox.Left : environment.parcelBoundingBoxLeft;
        this.boundingBox.Bottom = boundingBox ? boundingBox.Bottom : environment.parcelBoundingBoxBottom;
        this.boundingBox.Right = boundingBox ? boundingBox.Right : environment.parcelBoundingBoxRight;
        this.boundingBox.Top = boundingBox ? boundingBox.Top : environment.parcelBoundingBoxTop;

        this.map.fitBounds(
            [
                [this.boundingBox.Bottom, this.boundingBox.Left],
                [this.boundingBox.Top, this.boundingBox.Right],
            ],
            {}
        );
    }

    private setControl(): void {
        this.layerControl = new L.Control.Layers(this.tileLayers, this.overlayLayers).addTo(this.map);
    }

    private placeWellMarker(latlng: L.latLng) {
        if (this.selectByReferenceWell) return;

        if (this.wellMarker) {
            this.map.removeLayer(this.wellMarker);
        }

        this.wellMarker = new L.marker(latlng, { icon: this.wellIcon, zIndexOffset: 1000 });

        this.wellMarker.addTo(this.map);

        this.wellMarker.Latitude = latlng.lat;
        this.wellMarker.Longitude = latlng.lng;
    }

    public saveAndContinue() {
        this.isLoadingSubmit = true;

        this.wellRegistrationModel.Latitude = this.selectByReferenceWell ? this.selectedReferenceWell.Latitude : this.wellMarker.Latitude;
        this.wellRegistrationModel.Longitude = this.selectByReferenceWell ? this.selectedReferenceWell.Longitude : this.wellMarker.Longitude;
        this.wellRegistrationModel.ReferenceWellID = this.selectByReferenceWell ? this.selectedReferenceWell.ReferenceWellID : null;

        this.wellRegistrationService.wellRegistrationsWellRegistrationIDLocationPut(this.wellRegistrationModel.WellRegistrationID, this.wellRegistrationModel).subscribe({
            next: () => {
                this.router.navigate([`../confirm-location`], { relativeTo: this.route }).then(() => {
                    this.isLoadingSubmit = false;
                    this.wellRegistryProgressService.updateProgress(this.wellRegistrationModel.WellRegistrationID);
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Well successfully updated", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }
}
