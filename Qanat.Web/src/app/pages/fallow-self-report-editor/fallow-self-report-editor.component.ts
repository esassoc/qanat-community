import { AsyncPipe } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ColDef, GridApi, GridOptions, GridReadyEvent, SelectionChangedEvent } from "ag-grid-community";
import { BehaviorSubject, combineLatest, map, Observable, of, shareReplay, Subscription, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterAccountFallowStatusService } from "src/app/shared/generated/api/water-account-fallow-status.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    UsageLocationDto,
    UsageLocationUpdateFallowingDtoForm,
    UsageLocationUpdateFallowingDtoFormControls,
    UserDto,
    WaterAccountFallowStatusDto,
} from "src/app/shared/generated/model/models";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatGridComponent } from "../../shared/components/qanat-grid/qanat-grid.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { QanatMapComponent, QanatMapInitEvent } from "../../shared/components/leaflet/qanat-map/qanat-map.component";
import * as L from "leaflet";
import { UsageLocationLayerComponent } from "../../shared/components/leaflet/layers/usage-location-layer/usage-location-layer.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FormControl, FormGroup, ReactiveFormsModule, RequiredValidator, Validators } from "@angular/forms";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FallowTypeEnum, FallowTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/fallow-type-enum";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { AuthorizationHelper } from "src/app/shared/helpers/authorization-helper";
import { SelfReportStatusEnum } from "src/app/shared/generated/enum/self-report-status-enum";
import { FallowSubmitAttestationModalComponent } from "./fallow-submit-attestation-modal/fallow-submit-attestation-modal.component";
import { WaterAccountFallowStatusReviewService } from "src/app/shared/generated/api/water-account-fallow-status-review.service";
import { DialogService } from "@ngneat/dialog";
import { UsageLocationService } from "src/app/shared/generated/api/usage-location.service";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";

@Component({
    selector: "fallow-self-report-editor",
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
        FormFieldComponent,
        AlertDisplayComponent,
        ButtonLoadingDirective,
    ],
    templateUrl: "./fallow-self-report-editor.component.html",
    styleUrl: "./fallow-self-report-editor.component.scss",
})
export class FallowSelfReportEditorComponent implements OnInit, OnDestroy {
    public richTextTypeID = CustomRichTextTypeEnum.FallowSelfReportingInstructions;

    public fallowSelfReport$: Observable<WaterAccountFallowStatusDto>;
    public refreshFallowSelfReport: BehaviorSubject<void> = new BehaviorSubject<void>(null);

    public currentUser$: Observable<UserDto>;
    public currentUserIsWaterManagerOrAdmin: boolean = false;

    public usageLocationTypeSelectOptions$: Observable<SelectDropdownOption[]>;

    public usageLocationIDs: number[] = [];
    public selectedUsageLocation: UsageLocationDto;

    public usageLocationColDefs$: Observable<ColDef[]>;
    public usageLocationGridApi: GridApi;

    public formGroup = new FormGroup<UsageLocationUpdateFallowingDtoForm>({
        UsageLocationTypeID: UsageLocationUpdateFallowingDtoFormControls.UsageLocationTypeID(),
        FallowingNote: UsageLocationUpdateFallowingDtoFormControls.FallowingNote(),
    });

    public FormFieldType = FormFieldType;
    public FallowTypeSelectOptions = FallowTypesAsSelectDropdownOptions;

    public SelfReportStatusEnum = SelfReportStatusEnum;

    public enableEditing: boolean = false;
    public enableSubmit: boolean = false;

    public isLoadingSubmit: boolean = false;

    private subscriptions: Subscription[] = [];

    public constructor(
        private fallowSelfReportService: WaterAccountFallowStatusService,
        private fallowSelfReportReviewService: WaterAccountFallowStatusReviewService,
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

        this.fallowSelfReport$ = combineLatest({ params: this.activatedRoute.params, _: this.refreshFallowSelfReport }).pipe(
            switchMap(({ params }) => {
                let geographyID = params[routeParams.geographyID];
                let reportingPeriodID = params[routeParams.reportingPeriodID];
                let waterAccountFallowStatusID = params[routeParams.fallowSelfReportID];

                return this.fallowSelfReportService.getWaterAccountFallowStatus(geographyID, reportingPeriodID, waterAccountFallowStatusID).pipe(
                    tap((fallowSelfReport) => {
                        this.title.setTitle(`Fallow Self Report - ${fallowSelfReport.Geography.GeographyDisplayName} | Groundwater Accounting Platform`);
                        this.currentGeographyService.setCurrentGeography(fallowSelfReport.Geography);
                        this.usageLocationIDs = fallowSelfReport.UsageLocations.map((location) => location.UsageLocationID);
                        if (this.usageLocationGridApi) {
                            setTimeout(() => {
                                this.usageLocationGridApi.sizeColumnsToFit();
                            }, 100);
                        }

                        this.enableEditing =
                            fallowSelfReport.SelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Draft ||
                            fallowSelfReport.SelfReportStatus.SelfReportStatusID === SelfReportStatusEnum.Returned;

                        this.enableSubmit = this.enableEditing && fallowSelfReport.UsageLocations.some((location) => location.UsageLocationType.CountsAsFallowed);

                        if (this.selectedUsageLocation) {
                            let selectedLocation = fallowSelfReport.UsageLocations.find((location) => location.UsageLocationID === this.selectedUsageLocation.UsageLocationID);
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

        this.usageLocationColDefs$ = this.fallowSelfReport$.pipe(
            map((fallowSelfReport) => {
                let sourceOfRecordLabel = `${fallowSelfReport.UsageLocations.map((location) => location.SourceOfRecordWaterMeasurementTypeName)[0]} (ac-ft)`;
                let usageLocationColDefs = [
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location", "Name", { Sort: "asc" }),
                    this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "UsageLocationType.Name", { UseCustomDropdownFilter: true }),
                    this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "Area"),
                    this.utilityFunctionsService.createDecimalColumnDef(sourceOfRecordLabel, "SourceOfRecordValueInAcreFeet"),
                    this.utilityFunctionsService.createBasicColumnDef("Fallowing Note", "FallowNote"),
                ];

                return usageLocationColDefs;
            })
        );

        this.usageLocationTypeSelectOptions$ = this.fallowSelfReport$.pipe(
            switchMap((fallowSelfReport) => {
                return this.usageLocationTypeService.listUsageLocationType(fallowSelfReport.Geography.GeographyID).pipe(
                    map((usageLocationTypes) => {
                        return usageLocationTypes
                            .filter((ult) => ult.CanBeSelectedInFallowForm)
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

        if (!this.selectedUsageLocation.UsageLocationType.CanBeSelectedInFallowForm) {
            this.formGroup.disable();
        } else {
            this.formGroup.enable();
        }

        this.formGroup.patchValue({
            UsageLocationTypeID: usageLocation.UsageLocationType.UsageLocationTypeID,
            FallowingNote: usageLocation.FallowNote,
        });

        if (!this.enableEditing) {
            this.formGroup.disable();
        }
    }

    save(): void {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        let updateSubscription = this.usageLocationService
            .updateFallowingUsageLocation(
                this.selectedUsageLocation.Geography.GeographyID,
                this.selectedUsageLocation.Parcel.ParcelID,
                this.selectedUsageLocation.UsageLocationID,
                this.formGroup.getRawValue()
            )
            .subscribe((next) => {
                this.alertService.pushAlert(new Alert(`Usage location updated successfully.`, AlertContext.Success));
                this.refreshFallowSelfReport.next();
                this.isLoadingSubmit = false;
            });
    }

    submit(fallowStatusDto: WaterAccountFallowStatusDto): void {
        this.isLoadingSubmit = true;
        const dialogRef = this.dialogService.open(FallowSubmitAttestationModalComponent, {
            data: {
                GeographyID: fallowStatusDto.Geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                let submitSubscription = this.fallowSelfReportService
                    .submitWaterAccountFallowStatus(
                        fallowStatusDto.Geography.GeographyID,
                        fallowStatusDto.ReportingPeriod.ReportingPeriodID,
                        fallowStatusDto.WaterAccountFallowStatusID
                    )
                    .subscribe({
                        next: (v) => {
                            this.alertService.clearAlerts();
                            this.alertService.pushAlert(new Alert(`This fallow self report has been submitted successfully.`, AlertContext.Success));
                            this.refreshFallowSelfReport.next();
                            this.isLoadingSubmit = false;
                        },
                        error: (e) => {
                            if (e.error) {
                                for (let errorKey of Object.keys(e.error)) {
                                    this.alertService.pushAlert(new Alert(e.error[errorKey], AlertContext.Danger));
                                }
                                return;
                            }
                            this.alertService.pushAlert(new Alert("There was an error submitting the fallow self report.", AlertContext.Danger));
                            this.isLoadingSubmit = false;
                        },
                    });

                this.subscriptions.push(submitSubscription);
            } else {
                this.isLoadingSubmit = false;
            }
        });
    }

    approve(fallowStatusDto: WaterAccountFallowStatusDto): void {
        let fallowStatusIDs = [fallowStatusDto.WaterAccountFallowStatusID];

        let approveSubscription = this.fallowSelfReportReviewService.approveWaterAccountFallowStatusReview(fallowStatusDto.Geography.GeographyID, fallowStatusIDs).subscribe({
            next: (v) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`The fallow self report for ${fallowStatusDto.Geography.GeographyDisplayName} has been approved.`, AlertContext.Success));
                this.refreshFallowSelfReport.next();
            },
        });

        this.subscriptions.push(approveSubscription);
    }

    return(fallowStatusDto: WaterAccountFallowStatusDto): void {
        let fallowStatusIDs = [fallowStatusDto.WaterAccountFallowStatusID];

        let returnSubscription = this.fallowSelfReportReviewService.returnWaterAccountFallowStatusReview(fallowStatusDto.Geography.GeographyID, fallowStatusIDs).subscribe({
            next: (v) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`The fallow self report for ${fallowStatusDto.Geography.GeographyDisplayName} has been returned.`, AlertContext.Success));
                this.refreshFallowSelfReport.next();
            },
        });

        this.subscriptions.push(returnSubscription);
    }
}
