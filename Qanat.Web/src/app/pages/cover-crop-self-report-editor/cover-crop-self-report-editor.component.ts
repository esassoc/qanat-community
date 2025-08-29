import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ColDef, GridApi, GridOptions, GridReadyEvent } from "ag-grid-community";
import { Observable, BehaviorSubject, Subscription, combineLatest, switchMap, tap, shareReplay, of, map } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { UsageLocationLayerComponent } from "src/app/shared/components/leaflet/layers/usage-location-layer/usage-location-layer.component";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterAccountCoverCropStatusService } from "src/app/shared/generated/api/water-account-cover-crop-status.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UsageLocationDto } from "src/app/shared/generated/model/usage-location-dto";
import { WaterAccountCoverCropStatusDto } from "src/app/shared/generated/model/water-account-cover-crop-status-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import * as L from "leaflet";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { CoverCropSubmitAttestationModalComponent } from "./cover-crop-submit-attestation-modal/cover-crop-submit-attestation-modal.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { WaterAccountCoverCropStatusReviewService } from "src/app/shared/generated/api/water-account-cover-crop-status-review.service";
import { DialogService } from "@ngneat/dialog";
import { UsageLocationService } from "src/app/shared/generated/api/usage-location.service";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";
import { UsageLocationUpdateCoverCropDtoForm, UsageLocationUpdateCoverCropDtoFormControls } from "src/app/shared/generated/model/usage-location-update-cover-crop-dto";

@Component({
    selector: "cover-crop-self-report-editor",
    imports: [
        PageHeaderComponent,
        CustomRichTextComponent,
        IconComponent,
        RouterLink,
        AsyncPipe,
        LoadingDirective,
        QanatGridComponent,
        QanatMapComponent,
        UsageLocationLayerComponent,
        ReactiveFormsModule,
        FormsModule,
        FormFieldComponent,
        AlertDisplayComponent,
        ButtonLoadingDirective,
    ],
    templateUrl: "./cover-crop-self-report-editor.component.html",
    styleUrl: "./cover-crop-self-report-editor.component.scss",
})
export class CoverCropSelfReportEditorComponent {
    public richTextTypeID = CustomRichTextTypeEnum.CoverCropSelfReportingInstructions;

    public coverCropSelfReport$: Observable<WaterAccountCoverCropStatusDto>;
    public refreshCoverCropSelfReport: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public currentUser$: Observable<UserDto>;
    public currentUserIsWaterManagerOrAdmin: boolean = false;

    public usageLocationTypeSelectOptions$: Observable<SelectDropdownOption[]>;

    public usageLocationIDs: number[] = [];
    public selectedUsageLocation: UsageLocationDto;

    public usageLocationColDefs$: Observable<ColDef[]>;
    public usageLocationGridApi: GridApi;
    public FormFieldType = FormFieldType;

    public formGroup = new FormGroup<UsageLocationUpdateCoverCropDtoForm>({
        UsageLocationTypeID: UsageLocationUpdateCoverCropDtoFormControls.UsageLocationTypeID(),
        CoverCropNote: UsageLocationUpdateCoverCropDtoFormControls.CoverCropNote(),
    });

    public enableEditing: boolean = false;
    public enableSubmit: boolean = false;
    public SelfReportStatusEnum = SelfReportStatusEnum;

    public isLoadingSubmit: boolean = false;

    private subscriptions: Subscription[] = [];

    public constructor(
        private coverCropSelfReportService: WaterAccountCoverCropStatusService,
        private coverCropSelfReportReviewService: WaterAccountCoverCropStatusReviewService,
        private activatedRoute: ActivatedRoute,
        private readonly title: Title,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private dialogService: DialogService,
        private usageLocationService: UsageLocationService,
        private usageLocationTypeService: UsageLocationTypeService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = combineLatest({ currentUser: this.authenticationService.getCurrentUser(), params: this.activatedRoute.params }).pipe(
            switchMap(({ currentUser, params }) => {
                let geographyID = params[routeParams.geographyID];
                let currentUserIsAdmin = AuthorizationHelper.isSystemAdministrator(currentUser);
                let currentUserIsWaterManager = geographyID ? AuthorizationHelper.hasGeographyFlag(geographyID, FlagEnum.HasManagerDashboard, currentUser) : false;
                this.currentUserIsWaterManagerOrAdmin = currentUserIsAdmin || currentUserIsWaterManager;
                return of(currentUser);
            })
        );

        this.coverCropSelfReport$ = combineLatest({ params: this.activatedRoute.params, _: this.refreshCoverCropSelfReport }).pipe(
            switchMap(({ params }) => {
                let geographyID = params[routeParams.geographyID];
                let reportingPeriodID = params[routeParams.reportingPeriodID];
                let waterAccountCoverCropStatusID = params[routeParams.coverCropSelfReportID];

                return this.coverCropSelfReportService.getWaterAccountCoverCropStatus(geographyID, reportingPeriodID, waterAccountCoverCropStatusID).pipe(
                    tap((coverCropSelfReport) => {
                        this.title.setTitle(`Cover Crop Self Report - ${coverCropSelfReport.Geography.GeographyDisplayName} | Groundwater Accounting Platform`);
                        this.currentGeographyService.setCurrentGeography(coverCropSelfReport.Geography);
                        this.usageLocationIDs = coverCropSelfReport.UsageLocations.map((location) => location.UsageLocationID);
                        if (this.usageLocationGridApi) {
                            setTimeout(() => {
                                this.usageLocationGridApi.sizeColumnsToFit();
                            }, 100);
                        }

                        this.enableEditing =
                            coverCropSelfReport.SelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Draft ||
                            coverCropSelfReport.SelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Returned;

                        this.enableSubmit = this.enableEditing && coverCropSelfReport.UsageLocations.some((location) => location.UsageLocationType.CountsAsCoverCropped);

                        if (this.selectedUsageLocation) {
                            let selectedLocation = coverCropSelfReport.UsageLocations.find((location) => location.UsageLocationID === this.selectedUsageLocation.UsageLocationID);
                            if (selectedLocation) {
                                this.onUsageLocationSelected(selectedLocation);
                            } else {
                                this.selectedUsageLocation = null;
                                this.formGroup.reset();
                            }
                        }
                    })
                );
            }),
            shareReplay(1)
        );

        this.usageLocationColDefs$ = this.coverCropSelfReport$.pipe(
            map((coverCropSelfReport) => {
                let sourceOfRecordLabel = `${coverCropSelfReport.UsageLocations.map((location) => location.SourceOfRecordWaterMeasurementTypeName)[0]} (ac-ft)`;
                let usageLocationColDefs = [
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location", "Name", { Sort: "asc" }),
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationType.Name", { UseCustomDropdownFilter: true }),
                    this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "Area"),
                    this.utilityFunctionsService.createDecimalColumnDef(sourceOfRecordLabel, "SourceOfRecordValueInAcreFeet"),
                    this.utilityFunctionsService.createBasicColumnDef("Cover Crop Note", "CoverCropNote"),
                ];

                return usageLocationColDefs;
            })
        );

        this.usageLocationTypeSelectOptions$ = this.coverCropSelfReport$.pipe(
            switchMap((coverCropSelfReport) => {
                return this.usageLocationTypeService.listUsageLocationType(coverCropSelfReport.Geography.GeographyID).pipe(
                    map((usageLocationTypes) => {
                        return usageLocationTypes
                            .filter((ult) => ult.CanBeSelectedInCoverCropForm)
                            .map((ult) => {
                                return {
                                    Label: ult.Name,
                                    Value: ult.UsageLocationTypeID,
                                } as SelectDropdownOption;
                            });
                    })
                );
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
        this.subscriptions = [];
    }

    onGridReady($event: GridReadyEvent<any, any>) {
        this.usageLocationGridApi = $event.api;

        setTimeout(() => {
            this.usageLocationGridApi.sizeColumnsToFit();
        }, 100);
    }

    onGridSelection() {
        let selectedNode = this.usageLocationGridApi.getSelectedNodes()[0];
        if (selectedNode && selectedNode.data) {
            this.onUsageLocationSelected(selectedNode.data);
        }
    }

    // the map stuff
    public map: L.Map;
    public layerControl: L.layerControl;
    public mapIsReady: boolean = false;
    public layerLoading: boolean = true;
    onLayerLoadStarted() {
        this.layerLoading = true;
    }

    onLayerLoadFinished() {
        this.layerLoading = false;
    }

    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    onMapSelected(usageLocationID: number) {
        if (this.usageLocationGridApi) {
            this.usageLocationGridApi.forEachNode((node) => {
                if (node.data.UsageLocationID === usageLocationID) {
                    this.onUsageLocationSelected(node.data);
                    node.setSelected(true);
                }
            });
        }
    }

    onUsageLocationSelected(usageLocation: UsageLocationDto): void {
        this.selectedUsageLocation = usageLocation;

        if (!this.selectedUsageLocation.UsageLocationType.CanBeSelectedInCoverCropForm) {
            this.formGroup.disable();
        } else {
            this.formGroup.enable();
        }

        this.formGroup.patchValue({
            UsageLocationTypeID: usageLocation.UsageLocationType.UsageLocationTypeID,
            CoverCropNote: usageLocation.CoverCropNote,
        });

        if (!this.enableEditing) {
            this.formGroup.disable();
        }
    }

    save(): void {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        let updateSubscription = this.usageLocationService
            .updateCoverCropUsageLocation(
                this.selectedUsageLocation.Geography.GeographyID,
                this.selectedUsageLocation.Parcel.ParcelID,
                this.selectedUsageLocation.UsageLocationID,
                this.formGroup.getRawValue()
            )
            .subscribe((next) => {
                this.alertService.pushAlert(new Alert(`Usage location updated successfully.`, AlertContext.Success));
                this.refreshCoverCropSelfReport.next();
                this.isLoadingSubmit = false;
            });

        this.subscriptions.push(updateSubscription);
    }

    submit(coverCropStatusDto: WaterAccountCoverCropStatusDto): void {
        this.isLoadingSubmit = true;
        const dialogRef = this.dialogService.open(CoverCropSubmitAttestationModalComponent, {
            data: {
                GeographyID: coverCropStatusDto.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                let submitSubscription = this.coverCropSelfReportService
                    .submitWaterAccountCoverCropStatus(
                        coverCropStatusDto.Geography.GeographyID,
                        coverCropStatusDto.ReportingPeriod.ReportingPeriodID,
                        coverCropStatusDto.WaterAccountCoverCropStatusID
                    )
                    .subscribe({
                        next: (v) => {
                            this.alertService.clearAlerts();
                            this.alertService.pushAlert(new Alert(`This cover crop self report has been submitted successfully.`, AlertContext.Success));
                            this.refreshCoverCropSelfReport.next();
                            this.isLoadingSubmit = false;
                        },
                        error: (e) => {
                            this.isLoadingSubmit = false;
                            if (e.error) {
                                for (let errorKey of Object.keys(e.error)) {
                                    this.alertService.pushAlert(new Alert(e.error[errorKey], AlertContext.Danger));
                                }
                                return;
                            }
                            this.alertService.pushAlert(new Alert("There was an error submitting the cover crop self report.", AlertContext.Danger));
                        },
                    });

                this.subscriptions.push(submitSubscription);
            } else {
                this.isLoadingSubmit = false;
            }
        });
    }

    approve(coverCropStatusDto: WaterAccountCoverCropStatusDto): void {
        let coverCropStatusIDs = [coverCropStatusDto.WaterAccountCoverCropStatusID];

        let approveSubscription = this.coverCropSelfReportReviewService
            .approveWaterAccountCoverCropStatusReview(coverCropStatusDto.Geography.GeographyID, coverCropStatusIDs)
            .subscribe({
                next: (v) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(
                        new Alert(`The cover crop self report for ${coverCropStatusDto.Geography.GeographyDisplayName} has been approved.`, AlertContext.Success)
                    );
                    this.refreshCoverCropSelfReport.next();
                },
                error: () => {
                    this.isLoadingSubmit = false;
                },
            });

        this.subscriptions.push(approveSubscription);
    }

    return(coverCropStatusDto: WaterAccountCoverCropStatusDto): void {
        let coverCropStatusIDs = [coverCropStatusDto.WaterAccountCoverCropStatusID];

        let returnSubscription = this.coverCropSelfReportReviewService
            .returnWaterAccountCoverCropStatusReview(coverCropStatusDto.Geography.GeographyID, coverCropStatusIDs)
            .subscribe({
                next: (v) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(
                        new Alert(`The cover crop self report for ${coverCropStatusDto.Geography.GeographyDisplayName} has been returned.`, AlertContext.Success)
                    );
                    this.refreshCoverCropSelfReport.next();
                },
                error: () => {
                    this.isLoadingSubmit = false;
                },
            });

        this.subscriptions.push(returnSubscription);
    }
}
