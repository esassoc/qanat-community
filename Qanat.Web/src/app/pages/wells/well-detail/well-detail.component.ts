import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, IsActiveMatchOptions, Router, RouterLink } from "@angular/router";
import { BehaviorSubject, combineLatest, filter, map, Observable, of, shareReplay, Subscription, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { WellService } from "src/app/shared/generated/api/well.service";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { Map, layerControl } from "leaflet";
import { UpdateMeterModalComponent } from "src/app/shared/components/well/modals/update-meter-modal/update-meter-modal.component";
import { AddWellMeterModalComponent } from "src/app/shared/components/well/modals/add-well-meter-modal/add-well-meter-modal.component";
import { RemoveWellMeterModalComponent } from "src/app/shared/components/well/modals/remove-well-meter-modal/remove-well-meter-modal.component";
import { UpdateWellInfoModalComponent } from "src/app/shared/components/well/modals/update-well-info-modal/update-well-info-modal.component";
import { AsyncPipe, DecimalPipe, DatePipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { HighlightedParcelsLayerComponent } from "src/app/shared/components/leaflet/layers/highlighted-parcels-layer/highlighted-parcels-layer.component";
import { WellsLayerComponent } from "src/app/shared/components/leaflet/layers/wells-layer/wells-layer.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { OverrideWellParcelComponent } from "src/app/shared/components/well/modals/override-well-parcel-modal/override-well-parcel-modal.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ColDef } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { DashboardMenu, DashboardMenuComponent } from "src/app/shared/components/dashboard-menu/dashboard-menu.component";
import { GeographyLogoComponent } from "src/app/shared/components/geography-logo/geography-logo.component";
import { MeterReadingByWellService } from "src/app/shared/generated/api/meter-reading-by-well.service";
import { QanatGridHeaderComponent } from "../../../shared/components/qanat-grid-header/qanat-grid-header.component";
import { AgGridAngular } from "ag-grid-angular";
import { MeterReadingMonthlyInterpolationSimpleDto } from "src/app/shared/generated/model/meter-reading-monthly-interpolation-simple-dto";
import { VegaMeterReadingMonthlyChartComponent } from "src/app/shared/components/vega/vega-meter-reading-monthly-chart/vega-meter-reading-monthly-chart.component";
import { WellFileResourceDto } from "src/app/shared/generated/model/well-file-resource-dto";
import { WellFileResourceService } from "src/app/shared/generated/api/well-file-resource.service";
import { FileResourceListComponent } from "../../../shared/components/file-resource-list/file-resource-list.component";
import saveAs from "file-saver";
import { IFileResourceUpload } from "src/app/shared/components/file-resource-list/file-upload-modal/file-upload-modal.component";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import {
    FileResourceSimpleDto,
    MeterReadingDto,
    ParcelMinimalDto,
    UserDto,
    WellDetailDto,
    WellFileResourceUpdateDto,
    WellInstanceDto,
    WellLocationDto,
    WellTypeDto,
} from "src/app/shared/generated/model/models";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { DialogService } from "@ngneat/dialog";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { Alert } from "src/app/shared/models/alert";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { WellTypeService } from "src/app/shared/generated/api/well-type.service";
import { InstanceDisplayComponent } from "src/app/shared/components/schemoto/instance-display/instance-display.component";

@Component({
    selector: "well-detail",
    templateUrl: "./well-detail.component.html",
    styleUrl: "./well-detail.component.scss",
    imports: [
        PageHeaderComponent,
        RouterLink,
        IconComponent,
        AlertDisplayComponent,
        FieldDefinitionComponent,
        QanatMapComponent,
        HighlightedParcelsLayerComponent,
        WellsLayerComponent,
        AsyncPipe,
        DecimalPipe,
        DatePipe,
        QanatGridComponent,
        GeographyLogoComponent,
        DashboardMenuComponent,
        QanatGridHeaderComponent,
        VegaMeterReadingMonthlyChartComponent,
        FileResourceListComponent,
        InstanceDisplayComponent,
    ],
})
export class WellDetailComponent implements OnInit, OnDestroy {
    public currentUser$: Observable<UserDto>;
    public well$: Observable<WellDetailDto>;
    public wellLocation$: Observable<WellLocationDto>;
    public well: WellDetailDto;

    public wellType$: Observable<WellTypeDto>;
    public wellInstance$: Observable<WellInstanceDto>;

    public refreshMeterReadings$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public meterReadings$: Observable<MeterReadingDto[]>;

    public monthlyInterpolations$: Observable<MeterReadingMonthlyInterpolationSimpleDto[]>;

    public map: Map;
    public layerControl: layerControl;
    public mapIsReady: boolean = false;
    public highlightedParcelIDs: number[];

    public hasGeographyWellManagePermissions$: Observable<boolean> = of(false);
    public hasGeographyMeterManagePermissions$: Observable<boolean> = of(false);

    public meterReadingColumnDefs: ColDef<MeterReadingDto>[];
    public meterReadingGridRef: AgGridAngular;

    public monthlyInterpolationsColumnDefs: ColDef<MeterReadingDto>[];
    public monthlyInterpolationsGridRef: AgGridAngular;

    public fileResources$: Observable<WellFileResourceDto[]>;
    public refreshFileResources$ = new BehaviorSubject<null>(null);
    private subscriptions: Subscription[] = [];

    public pageMenu: DashboardMenu;

    public selectedMeterPanel: "MeterReadings" | "MonthlyInterpolations" = "MeterReadings";

    constructor(
        private wellService: WellService,
        private wellTypeService: WellTypeService,
        private meterReadingByWellService: MeterReadingByWellService,
        private wellFileResourceService: WellFileResourceService,
        private fileResourceService: FileResourceService,
        private route: ActivatedRoute,
        private router: Router,
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private confirmService: ConfirmService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser();

        this.well$ = this.route.paramMap.pipe(
            filter((paramMap) => {
                return paramMap.has(routeParams.wellID);
            }),
            switchMap((paramMap) => {
                return this.wellService.getWellAsDetailDtoWell(parseInt(paramMap.get(routeParams.wellID)));
            }),
            tap((well) => {
                this.well = well;
                this.highlightedParcelIDs = this.getHighlightedParcelIDs();
                this.buildMenu(well);

                if (this.well.MetersEnabled && well.Meter) {
                    this.refreshMeterReadings$.next(well.WellID);
                }
            }),
            shareReplay(1)
        );

        this.wellType$ = this.well$.pipe(
            filter((well) => !!well && well.WellType.HasSchomotoSchema),
            switchMap((well) => {
                return this.wellTypeService.getWellType(well.Geography.GeographyID, well.WellType.WellTypeID);
            })
        );

        this.wellInstance$ = combineLatest({ well: this.well$, wellType: this.wellType$ }).pipe(
            filter(({ well, wellType }) => !!well && !!wellType),
            switchMap(({ well, wellType }) => {
                return this.wellService.getWellInstanceWell(well.WellID);
            })
        );

        this.hasGeographyWellManagePermissions$ = combineLatest({ currentUser: this.currentUser$, well: this.well$ }).pipe(
            filter(({ currentUser, well }) => !!currentUser && !!well),
            map(({ currentUser, well }) => {
                return (
                    this.authenticationService.currentUserHasGeographyPermission(PermissionEnum.WellRights, RightsEnum.Update, well.Geography.GeographyID) ||
                    AuthorizationHelper.isSystemAdministrator(currentUser)
                );
            }),
            shareReplay(1)
        );

        this.hasGeographyMeterManagePermissions$ = combineLatest({ currentUser: this.currentUser$, well: this.well$ }).pipe(
            filter(({ currentUser, well }) => !!currentUser && !!well),
            map(({ currentUser, well }) => {
                return (
                    this.authenticationService.currentUserHasGeographyPermission(PermissionEnum.MeterRights, RightsEnum.Update, well.Geography.GeographyID) ||
                    AuthorizationHelper.isSystemAdministrator(currentUser)
                );
            }),
            tap((hasGeographyMeterManagePermissions) => {
                this.createMeterReadingColumnDefs(hasGeographyMeterManagePermissions);
            }),
            shareReplay(1)
        );

        this.wellLocation$ = this.well$.pipe(
            switchMap((well) => {
                return this.wellService.getLocationByWellIDWell(well.WellID);
            }),
            shareReplay(1)
        );

        this.meterReadings$ = this.refreshMeterReadings$.pipe(
            filter((wellID) => !!wellID),
            switchMap((wellID) => {
                return this.meterReadingByWellService.listMeterReadingsByWellMeterReadingByWell(this.well.Geography.GeographyID, wellID);
            }),
            shareReplay(1)
        );

        this.monthlyInterpolations$ = this.refreshMeterReadings$.pipe(
            filter((wellID) => !!wellID),
            switchMap((wellID) => {
                return this.meterReadingByWellService.listMonthlyInterpolationsByWellMeterReadingByWell(this.well.Geography.GeographyID, wellID);
            }),
            shareReplay(1)
        );

        this.fileResources$ = combineLatest({ well: this.well$, _: this.refreshFileResources$ }).pipe(
            switchMap(({ well, _ }) => {
                return this.wellFileResourceService.listWellFileResource(well.WellID);
            }),
            shareReplay(1)
        );

        this.createMonthlyInterpolationsColumnDefs();
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => {
            subscription.unsubscribe();
        });
    }

    private buildMenu(well: WellDetailDto) {
        var menu = {
            menuItems: [
                {
                    title: well.WellName ?? `#${well.WellID}`,
                    icon: "Wells",
                    routerLink: ["/wells", well.WellID],
                    isDropdown: true,
                    routerLinkActiveOptions: {
                        matrixParams: "ignored",
                        queryParams: "ignored",
                        fragment: "exact",
                        paths: "subset",
                    },
                    isExpanded: true,
                    preventCollapse: true,
                    menuItems: [
                        {
                            title: "Well Details",
                            routerLink: ["/wells", well.WellID],
                            fragment: "details",
                        },
                        {
                            title: "Meter",
                            routerLink: ["/wells", well.WellID],
                            fragment: "meter",
                            hidden: !well.MetersEnabled,
                        },
                        {
                            title: "Meter Readings",
                            routerLink: ["/wells", well.WellID],
                            fragment: "meter-readings",
                            hidden: !well.MetersEnabled || !well.Meter,
                        },
                        {
                            title: "Documents",
                            routerLink: ["/wells", well.WellID],
                            fragment: "documents",
                        },
                        {
                            title: "Back to All Wells",
                            icon: "ArrowLeft",
                            routerLink: ["/wells"],
                            cssClasses: "border-top",
                        },
                    ],
                },
                {
                    title: "Water Accounts",
                    icon: "WaterAccounts",
                    routerLink: ["/water-dashboard/water-accounts"],
                },
                {
                    title: "Parcels",
                    icon: "Parcels",
                    routerLink: ["/water-dashboard/parcels"],
                },
                {
                    title: "Wells",
                    icon: "Wells",
                    routerLink: ["/water-dashboard/wells"],
                },
            ],
        } as DashboardMenu;

        menu.menuItems.forEach((menuItem) => {
            menuItem.menuItems?.forEach((childItem) => {
                const urltree = this.router.createUrlTree(childItem.routerLink as any[]);
                const childRouteIsActive = this.router.isActive(
                    urltree,
                    childItem.routerLinkActiveOptions
                        ? childItem.routerLinkActiveOptions
                        : ({ paths: "exact", queryParams: "ignored", matrixParams: "ignored" } as IsActiveMatchOptions)
                );
                if (childRouteIsActive) {
                    menuItem.isExpanded = true;
                }
            });
        });

        this.pageMenu = menu;
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

    private createMeterReadingColumnDefs(hasGeographyMeterManagePermissions: boolean = false) {
        this.meterReadingColumnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit Reading",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => this.router.navigate(["edit-meter-reading", params.data.MeterReadingID], { relativeTo: this.route }),
                    },
                ];
            }, !hasGeographyMeterManagePermissions),
            this.utilityFunctionsService.createBasicColumnDef("Meter", "MeterSerialNumber"),
            this.utilityFunctionsService.createDateColumnDef("Date", "ReadingDate", "short", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume", "Volume", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createBasicColumnDef("Units", "MeterReadingUnitType.MeterReadingUnitTypeDisplayName", {
                CustomDropdownFilterField: "MeterReadingUnitType.MeterReadingUnitTypeDisplayName",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume in Acre Feet", "VolumeInAcreFeet", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createDecimalColumnDef("Previous Reading", "PreviousReading", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createDecimalColumnDef("Current Reading", "CurrentReading", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createBasicColumnDef("Reader Initials", "ReaderInitials"),
            this.utilityFunctionsService.createBasicColumnDef("Comment", "Comment"),
        ];
    }

    public onMeterReadingGridRefReady(event: AgGridAngular) {
        this.meterReadingGridRef = event;
    }

    private createMonthlyInterpolationsColumnDefs() {
        this.monthlyInterpolationsColumnDefs = [
            this.utilityFunctionsService.createDateColumnDef("Date", "Date", "MMMM yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume", "InterpolatedVolume", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume in Acre Feet", "InterpolatedVolumeInAcreFeet", {
                MinDecimalPlacesToDisplay: 0,
                MaxDecimalPlacesToDisplay: 4,
            }),
        ];
    }

    public onMonthlyInterpolationsGridRefReady(event: AgGridAngular) {
        this.monthlyInterpolationsGridRef = event;
    }

    public updateWellInfoModal() {
        const dialogRef = this.dialogService.open(UpdateWellInfoModalComponent, {
            data: {
                WellID: this.well.WellID,
                UpdatingTechnicalInfo: false,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.StateWCRNumber = result.StateWCRNumber;
                this.well.CountyWellPermitNumber = result.CountyWellPermitNumber;
                this.well.DateDrilled = result.DateDrilled;
                this.well.WellStatus = { WellStatusID: result.WellStatusID, WellStatusDisplayName: result.WellStatusDisplayName };
                this.well.Notes = result.Notes;
            }
        });
    }

    public overrideWellParcelModal() {
        const dialogRef = this.dialogService.open(OverrideWellParcelComponent, {
            data: { WellID: this.well.WellID },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.Parcel = new ParcelMinimalDto({ ParcelID: result.ParcelID, ParcelNumber: result.ParcelNumber });
            }
        });
    }

    public addWellMeterModal() {
        const dialogRef = this.dialogService.open(AddWellMeterModalComponent, {
            data: { WellID: this.well.WellID, WellName: this.well.WellName, GeographyID: this.well.Geography.GeographyID },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.Meter = result;
                this.refreshMeterReadings$.next(this.well.WellID);
            }
        });
    }

    public removeWellMeterModal() {
        const dialogRef = this.dialogService.open(RemoveWellMeterModalComponent, {
            data: { WellID: this.well.WellID, WellName: this.well.WellName, MeterID: this.well.Meter?.MeterID, DeviceName: this.well.Meter?.DeviceName },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.Meter = null;
            }
        });
    }

    public updateMeterModal() {
        const dialogRef = this.dialogService.open(UpdateMeterModalComponent, {
            data: {
                MeterID: this.well.Meter?.MeterID,
                GeographyID: this.well.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.Meter = result;
            }
        });
    }

    public updateTechnicalInfoModal() {
        const dialogRef = this.dialogService.open(UpdateWellInfoModalComponent, {
            data: {
                WellID: this.well.WellID,
                UpdatingTechnicalInfo: true,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.well.WellDepth = result.WellDepth;
                this.well.CasingDiameter = result.CasingDiameter;
                this.well.TopOfPerforations = result.TopOfPerforations;
                this.well.BottomOfPerforations = result.BottomOfPerforations;
                this.well.ElectricMeterNumber = result.ElectricMeterNumber;
            }
        });
    }

    public onFileResourceUploaded(fileResource: IFileResourceUpload) {
        this.wellFileResourceService.createWellFileResourceWellFileResource(this.well.WellID, fileResource.File, fileResource.FileDescription).subscribe((result) => {
            this.alertService.pushAlert(new Alert("Successfully uploaded file.", AlertContext.Success));
            this.refreshFileResources$.next(null);
        });
    }

    public downloadFileResource(fileResource: FileResourceSimpleDto) {
        let downloadFileSubscription = this.fileResourceService.downloadFileResourceFileResource(fileResource.FileResourceGUID).subscribe((response) => {
            saveAs(response, `${fileResource.OriginalBaseFilename}.${fileResource.OriginalFileExtension}`);
        });

        this.subscriptions.push(downloadFileSubscription);
    }

    public onFileResourceUpdated(fileResource: any) {
        this.alertService.clearAlerts();

        let fileResourceUpdateDto = new WellFileResourceUpdateDto({
            FileDescription: fileResource.FileDescription,
        });

        let updateFileResource = this.wellFileResourceService
            .updateWellFileResourceWellFileResource(this.well.WellID, fileResource.WellFileResourceID, fileResourceUpdateDto)
            .subscribe(() => {
                this.alertService.pushAlert(new Alert("Successfully updated file.", AlertContext.Success));
                this.refreshFileResources$.next(null);
            });

        this.subscriptions.push(updateFileResource);
    }

    public deleteFileResource(file: WellFileResourceDto) {
        this.alertService.clearAlerts();

        const message = `You are about to delete <b>${file.FileResource.OriginalBaseFilename}</b>. Are you sure you wish to proceed?`;
        this.confirmService.confirm({ title: "Delete File", message: message, buttonTextYes: "Delete", buttonClassYes: "btn-danger", buttonTextNo: "Cancel" }).then((confirmed) => {
            if (!confirmed) {
                return;
            }

            let deleteFileResource = this.wellFileResourceService.deleteWellFileResourceWellFileResource(this.well.WellID, file.WellFileResourceID).subscribe(() => {
                this.alertService.pushAlert(new Alert("Successfully deleted file.", AlertContext.Success));
                this.refreshFileResources$.next(null);
            });

            this.subscriptions.push(deleteFileResource);
        });
    }
}
