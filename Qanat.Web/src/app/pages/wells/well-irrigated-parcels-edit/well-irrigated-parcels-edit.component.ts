import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { tap, switchMap } from "rxjs/operators";
import { latLng } from "leaflet";
import { ParcelDisplayDto, WellIrrigatedParcelsRequestDto, WellIrrigatedParcelsResponseDto } from "src/app/shared/generated/model/models";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { WellService } from "src/app/shared/generated/api/well.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { WellIrrigatedParcelsEditMapComponent } from "src/app/shared/components/maps/well-irrigated-parcels-edit-map/well-irrigated-parcels-edit-map.component";

@Component({
    selector: "well-irrigated-parcels-edit",
    templateUrl: "./well-irrigated-parcels-edit.component.html",
    styleUrls: ["./well-irrigated-parcels-edit.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, WellIrrigatedParcelsEditMapComponent, RouterLink, AsyncPipe],
})
export class WellIrrigatedParcelsEditComponent implements OnInit, IDeactivateComponent {
    public wellID: number;
    public isLoadingSubmit: boolean = false;

    public wellIrrigatedParcels$: Observable<WellIrrigatedParcelsResponseDto>;
    public wellIrrigatedParcels: ParcelDisplayDto[];

    public irrigatedParcelIDs: number[];
    public geographyID;
    public wellLatLng: latLng;
    public customRichTextTypeID: CustomRichTextTypeEnum.UpdateWellIrrigatedParcels;

    constructor(
        private wellService: WellService,
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService
    ) {}

    canExit() {
        return true;
    }

    ngOnInit(): void {
        this.wellIrrigatedParcels$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                this.wellID = parseInt(paramMap.get(routeParams.wellID));
                return this.wellService.wellsWellIDIrrigatedParcelsGet(this.wellID);
            }),
            tap((wellIrrigatedParcelsDto) => {
                this.geographyID = wellIrrigatedParcelsDto.GeographyID;
                this.wellLatLng = new latLng(wellIrrigatedParcelsDto.Latitude, wellIrrigatedParcelsDto.Longitude);
                this.wellIrrigatedParcels = wellIrrigatedParcelsDto.IrrigatedParcels;
                this.irrigatedParcelIDs = wellIrrigatedParcelsDto.IrrigatedParcels.map((x) => x.ParcelID);
            })
        );
    }

    public onSelectionChanged(irrigatedParcelIDs: number[]) {
        this.irrigatedParcelIDs = irrigatedParcelIDs;
    }

    public reset(): void {
        // resetting the irrigated parcels input triggers a refresh in the WellIrrigatedParcelsEdit component
        const tempWellIrrigatedParcels = Object.assign({}, this.wellIrrigatedParcels);
        this.wellIrrigatedParcels = tempWellIrrigatedParcels;
    }

    public save(): void {
        this.isLoadingSubmit = true;

        const requestDto = new WellIrrigatedParcelsRequestDto({ IrrigatedParcelIDs: this.irrigatedParcelIDs });
        this.wellService.wellsWellIDIrrigatedParcelsPut(this.wellID, requestDto).subscribe({
            next: () => {
                this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                    this.isLoadingSubmit = false;
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Irrigated parcels successfully updated", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
