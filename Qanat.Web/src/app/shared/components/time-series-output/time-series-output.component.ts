import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Map } from "leaflet";
import * as L from "leaflet";
import GestureHandling from "leaflet-gesture-handling";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { TimeSeriesOutputChartComponent } from "../time-series-output-chart/time-series-output-chart.component";
import { ScenarioOutputStatComponent } from "../scenario-output-stat/scenario-output-stat.component";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { ScenarioRunDto, ScenarioRunResult, ScenarioRunResultPointOfInterest } from "src/app/shared/generated/model/models";
import { ModelService } from "../../generated/api/model.service";

@Component({
    selector: "time-series-output",
    imports: [CommonModule, TimeSeriesOutputChartComponent, ScenarioOutputStatComponent],
    templateUrl: "./time-series-output.component.html",
    styleUrls: ["./time-series-output.component.scss"],
})
export class TimeSeriesOutputComponent implements OnInit {
    @Input() getScenarioRunResult: ScenarioRunResult;
    @Input() scenarioRun: ScenarioRunDto;
    public ScenarioEnum = ScenarioEnum;
    public mapLoading: boolean = true;
    public map: L.map;
    public mapID: string = "getScenarioMap";
    public tileLayers: { [key: string]: any } = LeafletHelperService.GetDefaultTileLayers();

    public averageChangeInWaterLevel: number;
    public totalChangeInAquiferStorage: number;
    public totalChangeInPumping: number;
    public totalChangeInRecharge: number;
    public totalChangeInGainFromStreams: number;

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
        private modelService: ModelService,
        private leafletHelperService: LeafletHelperService
    ) {}

    ngOnInit(): void {
        this.modelService.getModelBoundaryByModelShortNameModel(this.scenarioRun.Model.ModelShortName).subscribe((modelBoundary) => {
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
        this.averageChangeInWaterLevel = this.getScenarioRunResult.AverageChangeInWaterLevel;
        this.totalChangeInAquiferStorage = this.getScenarioRunResult.TotalChangeInAquiferStorage;
        this.totalChangeInPumping = this.getScenarioRunResult.TotalChangeInPumping;
        this.totalChangeInRecharge = this.getScenarioRunResult.TotalChangeInRecharge;
        this.totalChangeInGainFromStreams = this.getScenarioRunResult.TotalChangeInGainFromStream;
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
        this.getScenarioRunResult.PointsOfInterest.forEach((input) => {
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

    popupContentForInput(input: ScenarioRunResultPointOfInterest): string {
        const isRecharge = this.scenarioRun.Scenario.ScenarioID == ScenarioEnum.Recharge;
        const pointType = input.AverageValue == null ? "Observation Point" : this.scenarioRun.Scenario.ScenarioID == ScenarioEnum.Recharge ? "Recharge Site" : "Pumping Well";
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
