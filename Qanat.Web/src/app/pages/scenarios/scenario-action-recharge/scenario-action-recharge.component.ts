import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { latlng, Map } from "leaflet";
import { AddAWellScenarioDto } from "src/app/shared/generated/model/add-a-well-scenario-dto";
import { LeafletHelperService } from "src/app/shared/services/leaflet-helper.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { tap } from "rxjs/operators";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { ModelSimpleDto, RechargeScenarioDto, ScenarioObservationPointDto, ScenarioRechargeSiteDto } from "src/app/shared/generated/model/models";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { Control } from "leaflet";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { ButtonLoadingDirective } from "../../../shared/directives/button-loading.directive";
import { ScenarioMapMarkerInputCardComponent } from "../../../shared/components/scenario-map-marker-input-card/scenario-map-marker-input-card.component";
import { ScenarioMapMarkerLayerComponent } from "../../../shared/components/scenario-map-marker-layer/scenario-map-marker-layer.component";
import { ScenarioMapComponent } from "../../../shared/components/maps/scenario-map/scenario-map.component";
import { FormsModule } from "@angular/forms";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { MonitoringWellsLayerComponent } from "src/app/shared/components/leaflet/layers/monitoring-wells-layer/monitoring-wells-layer.component";

@Component({
    selector: "scenario-action-recharge",
    templateUrl: "./scenario-action-recharge.component.html",
    styleUrls: ["./scenario-action-recharge.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        FormsModule,
        ScenarioMapComponent,
        ScenarioMapMarkerLayerComponent,
        MonitoringWellsLayerComponent,
        NgFor,
        ScenarioMapMarkerInputCardComponent,
        ButtonLoadingDirective,
        AsyncPipe,
    ],
})
export class ScenarioActionRechargeComponent implements OnInit, OnDestroy {
    public scenarioID = ScenarioEnum.Recharge;
    public model: RechargeScenarioDto;

    public modelShortName: string;
    public getModel$: Observable<ModelSimpleDto>;
    public richTextTypeID = CustomRichTextTypeEnum.RechargeScenario;
    public isLoadingSubmit: boolean = false;

    public map: Map;
    public mapLoading: boolean = true;
    public layerControl: Control;
    public isSelectingMapLocation: boolean = false;
    public mapSelectionEntity: "Recharge Site" | "Observation Point";

    public selectedRechargeSite: ScenarioRechargeSiteDto;
    public selectedObservationPoint: ScenarioObservationPointDto;

    public rechargeSiteMarkerIcon = this.leafletHelperService.blueIcon;
    public selectedRechargeSiteMarkerIcon = this.leafletHelperService.blueIconLarge;
    public observationPointMarkerIcon = this.leafletHelperService.yellowIcon;
    public selectedObservationPointMarker = this.leafletHelperService.yellowIconLarge;

    constructor(
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private leafletHelperService: LeafletHelperService,
        private getActionService: GETActionService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.initModel();

        const modelShortName = this.route.snapshot.paramMap.get(routeParams.modelShortName);
        if (modelShortName) {
            this.modelShortName = modelShortName;

            this.getModel$ = this.getActionService.modelsModelShortNameGet(modelShortName).pipe(
                tap((model) => {
                    this.model.ModelID = model.ModelID;
                })
            );
        }

        this.cdr.detectChanges();
    }

    initModel() {
        this.model = new AddAWellScenarioDto();
        this.model.ScenarioRechargeSites = [];
        this.model.ScenarioObservationPoints = [];
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    public canExit(): boolean {
        return !this.model.ScenarioRunName && this.model.ScenarioRechargeSites?.length == 0 && this.model.ScenarioObservationPoints?.length == 0;
    }

    public onMapReady(event: QanatMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapLoading = false;
    }

    public addRechargeSite() {
        this.isSelectingMapLocation = true;
        this.mapSelectionEntity = "Recharge Site";
    }

    public addObservationPoint() {
        this.isSelectingMapLocation = true;
        this.mapSelectionEntity = "Observation Point";
    }

    public onMapLocationSelection(latlng: latlng) {
        if (!this.isSelectingMapLocation) return;

        switch (this.mapSelectionEntity) {
            case "Recharge Site":
                var rechargeSiteName = `Recharge Site #${this.model.ScenarioRechargeSites.length + 1}`;
                var rechargeSite = new ScenarioRechargeSiteDto({ Latitude: latlng.lat, Longitude: latlng.lng, RechargeSiteName: rechargeSiteName });
                this.model.ScenarioRechargeSites = [...this.model.ScenarioRechargeSites, rechargeSite];
                break;

            case "Observation Point":
                var observationPointName = `Observation Point #${this.model.ScenarioObservationPoints.length + 1}`;
                var observationPoint = new ScenarioObservationPointDto({ Latitude: latlng.lat, Longitude: latlng.lng, ObservationPointName: observationPointName });
                this.model.ScenarioObservationPoints = [...this.model.ScenarioObservationPoints, observationPoint];
                break;
        }

        this.isSelectingMapLocation = false;
    }

    public onMapMarkerSelection(selectedMapObject: ScenarioRechargeSiteDto | ScenarioObservationPointDto) {
        if (selectedMapObject instanceof ScenarioRechargeSiteDto) {
            this.selectedRechargeSite = selectedMapObject;
            this.selectedObservationPoint = null;
        } else {
            this.selectedRechargeSite = null;
            this.selectedObservationPoint = selectedMapObject;
        }
    }

    public deleteRechargeSite(index: number) {
        if (this.rechargeSiteSelected(this.model.ScenarioRechargeSites[index])) {
            this.selectedRechargeSite = null;
        }

        this.model.ScenarioRechargeSites.splice(index, 1);
        this.model.ScenarioRechargeSites = [...this.model.ScenarioRechargeSites];
    }

    public deleteObservationPoint(index: number) {
        if (this.observationPointSelected(this.model.ScenarioObservationPoints[index])) {
            this.selectedObservationPoint = null;
        }

        this.model.ScenarioObservationPoints.splice(index, 1);
        this.model.ScenarioObservationPoints = [...this.model.ScenarioObservationPoints];
    }

    public rechargeSiteSelected(pumpingWell: ScenarioRechargeSiteDto): boolean {
        if (!this.selectedRechargeSite) return false;
        return pumpingWell.Latitude == this.selectedRechargeSite.Latitude && pumpingWell.Longitude == this.selectedRechargeSite.Longitude;
    }

    public observationPointSelected(observationPoint: ScenarioObservationPointDto): boolean {
        if (!this.selectedObservationPoint) return false;
        return observationPoint.Latitude == this.selectedObservationPoint.Latitude && observationPoint.Longitude == this.selectedObservationPoint.Longitude;
    }

    public selectRechargeSite(index: number) {
        this.selectedRechargeSite = this.model.ScenarioRechargeSites[index];
        this.selectedObservationPoint = null;
    }

    public selectObservationPoint(index: number) {
        this.selectedObservationPoint = this.model.ScenarioObservationPoints[index];
        this.selectedRechargeSite = null;
    }

    public runScenario() {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.getActionService.scenariosRechargePost(this.model).subscribe(
            () => {
                this.isLoadingSubmit = false;
                this.initModel();
                this.router.navigate(["../../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert("Scenario run successfully started.", AlertContext.Success));
                });
            },
            (error) => {
                this.isLoadingSubmit = false;
                window.scroll(0, 0);
            }
        );
    }
}
