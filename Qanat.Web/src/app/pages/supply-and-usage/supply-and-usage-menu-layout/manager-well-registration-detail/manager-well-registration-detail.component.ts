import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import saveAs from "file-saver";
import { Observable, Subscription, switchMap, forkJoin, map, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { FuelTypeEnum } from "src/app/shared/generated/enum/fuel-type-enum";
import { FileResourceSimpleDto } from "src/app/shared/generated/model/file-resource-simple-dto";
import { FuelTypeSimpleDto } from "src/app/shared/generated/model/fuel-type-simple-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { WellRegistrationDetailedDto } from "src/app/shared/generated/model/well-registration-detailed-dto";
import { WellRegistrationLocationDto } from "src/app/shared/generated/model/well-registration-location-dto";
import { LoadingDirective } from "../../../../shared/directives/loading.directive";
import { WellRegistrationsLayerComponent } from "src/app/shared/components/leaflet/layers/well-registrations-layer/well-registrations-layer.component";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { QanatMapComponent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe, DatePipe } from "@angular/common";

@Component({
    selector: "manager-well-detail",
    templateUrl: "./manager-well-registration-detail.component.html",
    styleUrls: ["./manager-well-registration-detail.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        RouterLink,
        IconComponent,
        AlertDisplayComponent,
        NgFor,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellRegistrationsLayerComponent,
        LoadingDirective,
        AsyncPipe,
        DecimalPipe,
        DatePipe,
    ],
})
export class ManagerWellRegistrationDetailComponent implements OnInit, OnDestroy {
    public FuelTypeEnum = FuelTypeEnum;
    public WellRegistrationStatusEnum = WellRegistrationStatusEnum;
    public currentUser: UserDto;

    public wellRegistration$: Observable<WellRegistrationDetailedDto>;
    public irrigatedParcelNumbers: string[];
    public irrigatedParcelIDs: number[];
    public wellRegistrationAsArray: WellRegistrationLocationDto[];
    public fuelTypes: FuelTypeSimpleDto[];
    public wellFuelType: FuelTypeSimpleDto;
    private fileResourceRequestSubscription: Subscription = Subscription.EMPTY;
    public hasMostRecentPumpTest: boolean;
    public showUpdateButton: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private wellRegistrationService: WellRegistrationService,
        private fileResourceService: FileResourceService
    ) {}

    ngOnInit(): void {
        this.fuelTypes;
        this.wellRegistration$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const wellID = parseInt(paramMap.get(routeParams.wellRegistrationID));
                return forkJoin({
                    well: this.wellRegistrationService.wellRegistrationsWellRegistrationIDGet(wellID),
                    fuelTypes: this.wellRegistrationService.wellRegistrationsPumpFuelTypesGet(),
                });
            }),
            map(({ well, fuelTypes }) => {
                this.fuelTypes = fuelTypes;
                return well;
            }),
            tap((wellRegistration) => {
                this.irrigatedParcelNumbers = wellRegistration.IrrigatedParcels.map((x) => x.ParcelNumber);
                this.irrigatedParcelIDs = wellRegistration.IrrigatedParcels.map((x) => x.ParcelID);
                this.showUpdateButton = ![WellRegistrationStatusEnum.Submitted, WellRegistrationStatusEnum.Approved].includes(
                    wellRegistration.WellRegistrationStatus.WellRegistrationStatusID
                );

                const wellRegistrationLocationDto = new WellRegistrationLocationDto();
                wellRegistrationLocationDto.WellRegistrationID = wellRegistration.WellRegistrationID;
                wellRegistrationLocationDto.Longitude = wellRegistration.Longitude;
                wellRegistrationLocationDto.Latitude = wellRegistration.Latitude;
                wellRegistrationLocationDto.ParcelID = wellRegistration.Parcel.ParcelID;
                wellRegistrationLocationDto.ParcelNumber = wellRegistration.Parcel.ParcelNumber;

                this.wellRegistrationAsArray = [wellRegistrationLocationDto];

                this.wellFuelType =
                    wellRegistration.WellRegistrationMetadatum?.FuelTypeID != null
                        ? this.fuelTypes.find((x) => x.FuelTypeID == wellRegistration.WellRegistrationMetadatum.FuelTypeID)
                        : null;
            })
        );
    }

    ngOnDestroy() {
        this.fileResourceRequestSubscription.unsubscribe();
    }

    public openFileResourceLink(fileResource: FileResourceSimpleDto) {
        this.fileResourceRequestSubscription = this.fileResourceService.fileResourcesFileResourceGuidAsStringGet(fileResource.FileResourceGUID).subscribe((response) => {
            saveAs(response, `${fileResource.OriginalBaseFilename}.${fileResource.OriginalFileExtension}`);
        });
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
