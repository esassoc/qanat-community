import { ChangeDetectorRef, Component, OnInit, ViewContainerRef } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WellService } from "src/app/shared/generated/api/well.service";
import { WellDetailDto } from "src/app/shared/generated/model/well-detail-dto";
import { WellLocationDto } from "src/app/shared/generated/model/well-location-dto";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { Map, layerControl } from "leaflet";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { MeterContext, UpdateMeterModalComponent } from "src/app/shared/components/well/update-meter-modal/update-meter-modal.component";
import { AddWellMeterModalComponent } from "src/app/shared/components/well/add-well-meter-modal/add-well-meter-modal.component";
import { RemoveWellMeterModalComponent } from "src/app/shared/components/well/remove-well-meter-modal/remove-well-meter-modal.component";
import { UpdateWellInfoModalComponent } from "src/app/shared/components/well/update-well-info-modal/update-well-info-modal.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";

@Component({
    selector: "well-detail",
    templateUrl: "./well-detail.component.html",
    styleUrl: "./well-detail.component.scss",
    standalone: true,
    imports: [
        NgIf,
        PageHeaderComponent,
        RouterLink,
        IconComponent,
        AlertDisplayComponent,
        FieldDefinitionComponent,
        NgFor,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellsLayerComponent,
        AsyncPipe,
        DecimalPipe,
        DatePipe,
    ],
})
export class WellDetailComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public well$: Observable<WellDetailDto>;
    public wellLocation$: Observable<WellLocationDto>;
    public well: WellDetailDto;

    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;
    public highlightedParcelIDs: number[];

    public hasGeographyWellManagePermissions: boolean = false;

    constructor(
        private wellService: WellService,
        private route: ActivatedRoute,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((x) => {
                const wellID = parseInt(this.route.snapshot.paramMap.get(routeParams.wellID));
                this.well$ = this.wellService.wellsWellIDDetailsGet(wellID).pipe(
                    tap((well) => {
                        this.well = well;
                        this.highlightedParcelIDs = this.getHighlightedParcelIDs();

                        this.hasGeographyWellManagePermissions =
                            this.authenticationService.currentUserHasGeographyPermission(PermissionEnum.WellRights, RightsEnum.Update, well.Geography.GeographyID) ||
                            AuthorizationHelper.isSystemAdministrator(x);
                    })
                );

                this.wellLocation$ = this.wellService.wellsWellIDLocationGet(wellID);
            })
        );
    }

    public handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
        this.cdr.detectChanges();
    }

    private getHighlightedParcelIDs() {
        let highlightedParcelIDs = [];
        if (this.well.IrrigatedParcels?.length > 0) {
            highlightedParcelIDs = this.well.IrrigatedParcels.map((x) => x.ParcelID);
        }
        if (this.well.Parcel && !highlightedParcelIDs.includes(this.well.Parcel.ParcelID)) {
            highlightedParcelIDs.push(this.well.Parcel.ParcelID);
        }
        return highlightedParcelIDs;
    }

    public updateWellInfoModal() {
        this.modalService
            .open(UpdateWellInfoModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Large, ModalTheme: ModalThemeEnum.Light }, { WellID: this.well.WellID })
            .instance.result.then((result) => {
                if (result) {
                    this.well.StateWCRNumber = result.StateWCRNumber;
                    this.well.CountyWellPermitNumber = result.CountyWellPermitNumber;
                    this.well.DateDrilled = result.DateDrilled;
                    this.well.WellDepth = result.WellDepth;
                    this.well.WellStatus = { WellStatusID: result.WellStatusID, WellStatusDisplayName: result.WellStatusDisplayName };
                    this.well.Notes = result.Notes;
                }
            });
    }

    public addWellMeterModal() {
        this.modalService
            .open(
                AddWellMeterModalComponent,
                this.viewContainerRef,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light },
                { WellID: this.well.WellID, WellName: this.well.WellName, GeographyID: this.well.Geography.GeographyID }
            )
            .instance.result.then((result) => {
                if (result) {
                    this.well.Meter = result;
                }
            });
    }

    public removeWellMeterModal() {
        this.modalService
            .open(
                RemoveWellMeterModalComponent,
                this.viewContainerRef,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light },
                { WellID: this.well.WellID, WellName: this.well.WellName, MeterID: this.well.Meter?.MeterID, DeviceName: this.well.Meter?.DeviceName }
            )
            .instance.result.then((result) => {
                if (result) {
                    this.well.Meter = null;
                }
            });
    }

    public updateMeterModal() {
        this.modalService
            .open(UpdateMeterModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                MeterID: this.well.Meter?.MeterID,
                GeographyID: this.well.Geography.GeographyID,
            } as MeterContext)
            .instance.result.then((result) => {
                if (result) {
                    this.well.Meter = result;
                }
            });
    }
}
