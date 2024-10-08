import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, map, of, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { BeginWellRegistryRequestDto, GeographyDto, ParcelMinimalDto } from "src/app/shared/generated/model/models";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { ParcelTypeaheadComponent } from "../../../shared/components/parcel-typeahead/parcel-typeahead.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
@Component({
    selector: "select-parcel",
    templateUrl: "./select-parcel.component.html",
    styleUrls: ["./select-parcel.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        WorkflowBodyComponent,
        FormsModule,
        NgIf,
        ParcelTypeaheadComponent,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        ButtonComponent,
        AsyncPipe,
    ],
})
export class WellRegistrySelectParcelComponent implements OnInit, OnDestroy {
    public geography: GeographyDto;
    public selectedParcel$: Observable<ParcelMinimalDto>;

    public wellRegistrationID: number;
    public selectedParcel: ParcelMinimalDto;
    public selectedParcelIDOnLoad: number;

    public richTextTypeID = CustomRichTextTypeEnum.WellRegistryIntro;
    public isLoadingSubmit = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private wellRegistrationService: WellRegistrationService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private geographyRouteService: GeographyRouteService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.selectedParcel$ = this.geographyRouteService.geography$.pipe(
            tap((x) => {
                this.geography = x;
            }),
            map((x) => {
                return parseInt(this.route.snapshot.paramMap.get(routeParams.wellRegistrationID));
            }),
            switchMap((wellID) => {
                if (wellID) {
                    this.wellRegistrationID = wellID;
                    return this.wellRegistrationService.wellRegistrationsWellRegistrationIDParcelGet(wellID);
                } else {
                    return of(null);
                }
            }),
            tap((x) => {
                this.selectedParcel = x;
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public continue() {
        if (!this.wellRegistrationID) {
            this.createWell();
        } else {
            this.updateWellParcel();
        }
    }

    selectedParcelChanged(parcel: ParcelMinimalDto) {
        this.selectedParcel = parcel;
    }

    private createWell() {
        this.isLoadingSubmit = true;
        const parcelID = this.selectedParcel ? this.selectedParcel.ParcelID : null;

        const createWellRequest = new BeginWellRegistryRequestDto();
        createWellRequest.ParcelID = parcelID;

        this.wellRegistrationService.geographiesGeographyIDWellRegistrationsPost(this.geography.GeographyID, createWellRequest).subscribe({
            next: (wellRegistration) => {
                this.wellRegistryProgressService.updateProgress(this.wellRegistrationID);
                this.isLoadingSubmit = false;
                this.wellRegistrationID = wellRegistration.WellRegistrationID;
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully saved Parcel", AlertContext.Success));
                this.goToLocationStep();
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }

    private updateWellParcel() {
        this.isLoadingSubmit = true;

        this.wellRegistrationService.wellRegistrationsWellRegistrationIDParcelPut(this.wellRegistrationID, this.selectedParcel?.ParcelID).subscribe({
            next: () => {
                this.wellRegistryProgressService.updateProgress(this.wellRegistrationID);
                this.isLoadingSubmit = false;
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully saved Parcel", AlertContext.Success));
                this.goToLocationStep();
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }

    private goToLocationStep() {
        this.router.navigateByUrl(`/well-registry/${this.geography.GeographyName.toLowerCase()}/well/${this.wellRegistrationID}/edit/location`);
    }

    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;
    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
    }
}
