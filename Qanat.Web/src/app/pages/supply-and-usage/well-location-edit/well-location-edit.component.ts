import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { Observable, switchMap, tap } from "rxjs";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WellLocationDto } from "src/app/shared/generated/model/well-location-dto";
import { latLng } from "leaflet";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { WellService } from "src/app/shared/generated/api/well.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { routeParams } from "src/app/app.routes";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { WellLocationPreviewDto } from "src/app/shared/generated/model/well-location-preview-dto";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { WellLocationEditMapComponent } from "src/app/shared/components/maps/well-location-edit-map/well-location-edit-map.component";

@Component({
    selector: "well-location-edit",
    templateUrl: "./well-location-edit.component.html",
    styleUrl: "./well-location-edit.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, WellLocationEditMapComponent, RouterLink, AsyncPipe],
})
export class WellLocationEditComponent implements OnInit, OnDestroy {
    public geography$: Observable<GeographyDto>;
    public wellLocation$: Observable<WellLocationDto>;

    public model: WellLocationDto;
    public wellLatLng: latLng;

    public isLoadingSubmit = false;
    public customRichTextTypeID = CustomRichTextTypeEnum.UpdateWellLocation;

    constructor(
        private cdr: ChangeDetectorRef,
        private router: Router,
        private route: ActivatedRoute,
        private wellService: WellService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.wellLocation$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const wellID = parseInt(paramMap.get(routeParams.wellID));
                return this.wellService.wellsWellIDLocationGet(wellID);
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

    public previewChanges() {
        this.isLoadingSubmit = true;

        this.wellService.wellsWellIDLocationPreviewPut(this.model.WellID, this.model).subscribe((response) => {
            if (response.ParcelID == this.model.ParcelID) {
                // bypass confirmation modal if Parcel hasn't changed
                this.save();
            } else {
                this.confirmWellLocation(response);
            }
        });
    }

    public confirmWellLocation(response: WellLocationPreviewDto) {
        const message =
            (this.model.ParcelID != null
                ? `<p>This well is currently associated with APN <b>${this.model.ParcelNumber}</b>. `
                : "<p>This well is currently not associated with an APN. ") +
            (response.ParcelID != null ? `This update will move it to APN <b>${response.ParcelNumber}</b>.` : "This update will remove it from this parcel</b>.") +
            `</p></br><p>Are you sure you want to proceed?</p>`;

        console.log(message);

        const confirmOptions = {
            title: "Confirm Well Location",
            message: message,
            buttonClassYes: "btn-primary",
            buttonTextYes: "Update Well",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.save();
            } else {
                this.isLoadingSubmit = false;
            }
        });
    }

    public save() {
        this.wellService.wellsWellIDLocationPut(this.model.WellID, this.model).subscribe({
            next: () => {
                this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Well location updated successfully.", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            },
        });
    }
}
