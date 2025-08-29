import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { tap, switchMap } from "rxjs/operators";
import { latLng } from "leaflet";
import { ParcelDisplayDto, WellRegistrationIrrigatedParcelsRequestDto, WellRegistrationIrrigatedParcelsResponseDto } from "src/app/shared/generated/model/models";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { WellIrrigatedParcelsEditMapComponent } from "../../../shared/components/maps/well-irrigated-parcels-edit-map/well-irrigated-parcels-edit-map.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowHelpComponent } from "src/app/shared/components/workflow-help/workflow-help.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "irrigated-parcels-edit",
    templateUrl: "./irrigated-parcels-edit.component.html",
    styleUrls: ["./irrigated-parcels-edit.component.scss"],
    imports: [PageHeaderComponent, WorkflowHelpComponent, WorkflowBodyComponent, AlertDisplayComponent, WellIrrigatedParcelsEditMapComponent, ButtonComponent, AsyncPipe]
})
export class IrrigatedParcelsEditComponent implements OnInit, IDeactivateComponent {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryIrrigatedParcels;

    public wellID: number;
    public isLoadingSubmit: boolean = false;

    public wellRegistrationIrrigatedParcels$: Observable<WellRegistrationIrrigatedParcelsResponseDto>;
    public wellRegistrationIrrigatedParcels: ParcelDisplayDto[];

    public irrigatedParcelIDs: number[];
    public geographyID;
    public wellLatLng: latLng;

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private route: ActivatedRoute,
        private router: Router,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private alertService: AlertService
    ) {}

    canExit() {
        return true;
    }

    ngOnInit(): void {
        this.wellRegistrationIrrigatedParcels$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                this.wellID = parseInt(paramMap.get(routeParams.wellRegistrationID));
                return this.wellRegistrationService.getWellRegistrationIrrigatedParcelsWellRegistration(this.wellID);
            }),
            tap((wellIrrigatedParcelsDto) => {
                this.geographyID = wellIrrigatedParcelsDto.GeographyID;
                this.wellLatLng = new latLng(wellIrrigatedParcelsDto.Latitude, wellIrrigatedParcelsDto.Longitude);
                this.wellRegistrationIrrigatedParcels = wellIrrigatedParcelsDto.IrrigatedParcels;
            })
        );
    }

    public save(andContinue: boolean = false): void {
        this.isLoadingSubmit = true;

        const requestDto = new WellRegistrationIrrigatedParcelsRequestDto({ IrrigatedParcelIDs: this.irrigatedParcelIDs });
        this.wellRegistrationService.updateWellIrrigatedParcelsWellRegistration(this.wellID, requestDto).subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.wellRegistryProgressService.updateProgress(this.wellID);
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Well successfully updated", AlertContext.Success));

                if (andContinue) {
                    this.router.navigate([`../contacts`], { relativeTo: this.route });
                } else {
                    this.reset();
                }
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }

    public onSelectionChanged(irrigatedParcelIDs: number[]) {
        this.irrigatedParcelIDs = irrigatedParcelIDs;
    }

    public reset(): void {
        // resetting the irrigated parcels input triggers a refresh in the WellIrrigatedParcelsEdit component
        const tempWellRegistrationIrrigatedParcels = Object.assign({}, this.wellRegistrationIrrigatedParcels);
        this.wellRegistrationIrrigatedParcels = tempWellRegistrationIrrigatedParcels;
    }
}
