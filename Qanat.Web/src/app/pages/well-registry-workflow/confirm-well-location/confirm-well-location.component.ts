import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
// import * as L from 'leaflet';
import { latLng } from "leaflet";
import "leaflet.markercluster";
import { routeParams } from "src/app/app.routes";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { Observable, switchMap, tap } from "rxjs";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { ConfirmWellRegistrationLocationDto } from "src/app/shared/generated/model/confirm-well-registration-location-dto";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { WellLocationEditMapComponent } from "../../../shared/components/maps/well-location-edit-map/well-location-edit-map.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowHelpComponent } from "src/app/shared/components/workflow-help/workflow-help.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "confirm-well-location",
    templateUrl: "./confirm-well-location.component.html",
    styleUrls: ["./confirm-well-location.component.scss"],
    imports: [PageHeaderComponent, WorkflowHelpComponent, WorkflowBodyComponent, AlertDisplayComponent, WellLocationEditMapComponent, ButtonComponent, AsyncPipe]
})
export class ConfirmWellLocationComponent implements OnInit, OnDestroy {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryConfirmWellLocation;

    public confirmWellLocation$: Observable<ConfirmWellRegistrationLocationDto>;
    public model: ConfirmWellRegistrationLocationDto;
    public wellLatLng: latLng;

    public isLoadingSubmit = false;

    constructor(
        private cdr: ChangeDetectorRef,
        private router: Router,
        private route: ActivatedRoute,
        private wellRegistrationService: WellRegistrationService,
        private alertService: AlertService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService
    ) {}

    ngOnInit(): void {
        this.confirmWellLocation$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const wellRegistrationID = parseInt(paramMap.get(routeParams.wellRegistrationID));
                return this.wellRegistrationService.getConfirmLocationWellRegistration(wellRegistrationID);
            }),
            tap((wellLocation) => {
                this.model = { ...wellLocation };
                this.wellLatLng = new latLng(wellLocation.Latitude, wellLocation.Longitude);
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public onLocationChanged(latLng: latLng) {
        this.model.Latitude = latLng.lat;
        this.model.Longitude = latLng.lng;
    }

    public saveAndContinue() {
        this.isLoadingSubmit = true;

        this.wellRegistrationService.confirmLocationWellRegistration(this.model.WellRegistrationID, this.model).subscribe({
            next: () => {
                this.router.navigate([`../irrigated-parcels`], { relativeTo: this.route }).then(() => {
                    this.isLoadingSubmit = false;
                    this.wellRegistryProgressService.updateProgress(this.model.WellRegistrationID);
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
