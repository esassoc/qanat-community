import { ChangeDetectorRef, Component } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import saveAs from "file-saver";
import * as L from "leaflet";
import { Observable, Subscription, forkJoin, map, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { FuelTypeEnum, FuelTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/fuel-type-enum";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { FileResourceSimpleDto } from "src/app/shared/generated/model/file-resource-simple-dto";
import { WellRegistrationDetailedDto, WellRegistrationLocationDto } from "src/app/shared/generated/model/models";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe, DatePipe } from "@angular/common";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { WellRegistrationsLayerComponent } from "src/app/shared/components/leaflet/layers/well-registrations-layer/well-registrations-layer.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";

@Component({
    selector: "well-registration-detail",
    templateUrl: "./well-registration-detail.component.html",
    styleUrls: ["./well-registration-detail.component.scss"],
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        NgFor,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellRegistrationsLayerComponent,
        LoadingDirective,
        AsyncPipe,
        DecimalPipe,
        DatePipe,
        IconComponent,
    ],
})
export class WellRegistrationDetailComponent {
    public FuelTypeEnum = FuelTypeEnum;
    public WellRegistrationStatusEnum = WellRegistrationStatusEnum;
    public currentUser: UserDto;

    public wellRegistration$: Observable<WellRegistrationDetailedDto>;
    public irrigatedParcelNumbers: string[];
    public irrigatedParcelIDs: number[];
    public wellRegistrationAsArray: WellRegistrationLocationDto[];
    public FuelTypesSelectDropdownOptions = FuelTypesAsSelectDropdownOptions;
    public wellRegistrationFuelType: SelectDropdownOption;
    private fileResourceRequestSubscription: Subscription = Subscription.EMPTY;
    public showUpdateButton: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private wellRegistrationService: WellRegistrationService,
        private fileResourceService: FileResourceService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.wellRegistration$ = this.route.paramMap.pipe(
            switchMap((paramMap) => {
                const wellID = parseInt(paramMap.get(routeParams.wellRegistrationID));
                return this.wellRegistrationService.wellRegistrationsWellRegistrationIDGet(wellID);
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

                this.wellRegistrationFuelType =
                    wellRegistration.WellRegistrationMetadatum?.FuelTypeID != null
                        ? this.FuelTypesSelectDropdownOptions.find((x) => x.Value == wellRegistration.WellRegistrationMetadatum.FuelTypeID)
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
        this.cdr.detectChanges();
    }
}
