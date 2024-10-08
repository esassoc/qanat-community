import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { Map } from "leaflet";
import * as L from "leaflet";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import GestureHandling from "leaflet-gesture-handling";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { TimeSeriesOutputChartComponent } from "../time-series-output-chart/time-series-output-chart.component";
import { ScenarioOutputStatComponent } from "../scenario-output-stat/scenario-output-stat.component";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { GetActionResult, GetActionResultPointOfInterest } from "src/app/shared/generated/model/models";

@Component({
    selector: "time-series-output",
    standalone: true,
    imports: [CommonModule, TimeSeriesOutputChartComponent, ScenarioOutputStatComponent],
    templateUrl: "./time-series-output.component.html",
    styleUrls: ["./time-series-output.component.scss"],
})
export class TimeSeriesOutputComponent implements OnInit {
    @Input() getActionResult: GetActionResult;
    @Input() getAction: GETActionDto;
    public ScenarioEnum = ScenarioEnum;
    public mapLoading: boolean = true;
    public map: L.map;
    public mapID: string = "getScenarioMap";
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();

    public averageChangeInWaterLevel: number;
    public totalChangeInAquiferStorage: number;
    public totalChangeInPumping: number;
    public totalChangeInRecharge: number;

    private markers: L.marker[] = [];

    // two-way binding selected marker functionality
    private _selectedMarker: string = null;
    set selectedMarker(value) {
        this._selectedMarker = value;
        this.markers.forEach((marker) => {
            if (!value) {
                marker.setOpacity(1);
            } else if (marker.name == value) {
                marker.setOpacity(1);
            } else {
                marker.setOpacity(0.4);
            }
        });
    }
    get selectedMarker() {
        return this._selectedMarker;
    }

    constructor(
        private getActionService: GETActionService,
        private leafletHelperService: LeafletHelperService
    ) {}

    ngOnInit(): void {
        this.getActionService.modelsModelShortNameBoundaryGet(this.getAction.Model.ModelShortName).subscribe((modelBoundary) => {
            const geoJsonObject = JSON.parse(modelBoundary.GeoJson);

            if (geoJsonObject) {
                const leafletGeoJson = L.geoJson(geoJsonObject, { interactive: false });
                this.map.fitBounds(leafletGeoJson.getBounds());
                leafletGeoJson.addTo(this.map);
            } else {
                this.leafletHelperService.fitMapToDefaultBoundingBox(this.map);
            }
        });

        // get the stats that we want to show...
        this.setStats();
    }

    public setStats() {
        this.averageChangeInWaterLevel = this.getActionResult.AverageChangeInWaterLevel;
        this.totalChangeInAquiferStorage = this.getActionResult.TotalChangeInAquiferStorage;
        this.totalChangeInPumping = this.getActionResult.TotalChangeInPumping;
        this.totalChangeInRecharge = this.getActionResult.TotalChangeInRecharge;
    }

    public onMapReady(map: Map) {
        this.map = map;
        this.mapLoading = false;
    }

    ngAfterViewInit(): void {
        this.initMap();
    }

    initMap(): void {
        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [this.tileLayers.Aerial],
            fullscreenControl: true,
            gestureHandling: true,
        } as L.MapOptions;

        this.map = L.map(this.mapID, mapOptions);
        L.Map.addInitHook("addHandler", "gestureHandling", GestureHandling);

        let currentColorIndex = 0;
        this.getActionResult.PointsOfInterest.forEach((input) => {
            const icon = this.leafletHelperService.createDivIcon(this.leafletHelperService.markerColors[currentColorIndex], input.AverageValue == null);

            const popupContent = this.popupContentForInput(input);

            const marker = L.marker([input.Latitude, input.Longitude], { icon: icon })
                .bindPopup(popupContent, { offset: [0, -28] })
                .addTo(this.map);

            marker.name = input.Name; // IMPORTANT: adding this name field to the marker that is the input name
            marker.on("click", (event) => {
                if (this.selectedMarker == event.target.name) {
                    this.selectedMarker = null;
                } else {
                    this.selectedMarker = event.target.name;
                }
            });

            this.markers = [...this.markers, marker];

            if (currentColorIndex > this.leafletHelperService.markerColors.length - 1) {
                currentColorIndex = 0;
            } else {
                currentColorIndex++;
            }
        });
    }

    popupContentForInput(input: GetActionResultPointOfInterest): string {
        const isRecharge = this.getAction.Scenario.ScenarioID == ScenarioEnum.Recharge;
        const pointType = input.AverageValue == null ? "Observation Point" : this.getAction.Scenario.ScenarioID == ScenarioEnum.Recharge ? "Recharge Site" : "Pumping Well";
        let popupContent = `
      <p>
        <strong>${pointType}: </strong>
        ${input.Name}
      <p>
    `;
        if (input.AverageValue != null) {
            popupContent += `<p><strong>${isRecharge ? "Recharge Volume" : "Pumping Volume"}:</strong> ${isRecharge ? input.AverageValue : -input.AverageValue} (ac-ft/mo)</p>`;
        }
        return popupContent;
    }
}
