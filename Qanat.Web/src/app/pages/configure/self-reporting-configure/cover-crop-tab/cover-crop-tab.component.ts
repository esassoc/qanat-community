import { Component, EventEmitter, Input, OnChanges, output, Output, SimpleChanges } from "@angular/core";
import { GeographyMinimalDto, ReportingPeriodDto, UsageLocationTypeDto } from "src/app/shared/generated/model/models";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { DialogService } from "@ngneat/dialog";
import { EditCoverCropSelfReportVisibilityModalComponent } from "./edit-cover-crop-self-report-visibility-modal/edit-cover-crop-self-report-visibility-modal.component";
import { FormsModule } from "@angular/forms";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { EditCoverCropUsageLocationTypeModalComponent } from "./edit-cover-crop-usage-location-type-modal/edit-cover-crop-usage-location-type-modal.component";

@Component({
    selector: "cover-crop-tab",
    imports: [QanatGridComponent, FormsModule],
    templateUrl: "./cover-crop-tab.component.html",
    styleUrl: "./cover-crop-tab.component.scss",
})
export class CoverCropTabComponent implements OnChanges {
    @Input() currentGeography: GeographyMinimalDto;
    @Input() reportingPeriods: ReportingPeriodDto[];
    @Input() usageLocationTypes: UsageLocationTypeDto[];

    @Output() public updatedGeography: EventEmitter<void> = new EventEmitter<void>();
    @Output() public updatedReportingPeriods: EventEmitter<void> = new EventEmitter<void>();
    @Output() public updatedUsageLocationTypes: EventEmitter<void> = new EventEmitter<void>();

    public coverCropEnabled: boolean = false;

    public reportingPeriodColumnDefs: ColDef[];
    public usageLocationTypeColumnDefs: ColDef[];
    private gridApi: GridApi;

    public constructor(
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private geographyService: GeographyService,
        private alertService: AlertService
    ) {
        this.reportingPeriodColumnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => {
                            const dialogRef = this.dialogService.open(EditCoverCropSelfReportVisibilityModalComponent, {
                                data: {
                                    GeographyID: this.currentGeography.GeographyID,
                                    ReportingPeriod: params.data,
                                },
                            });

                            dialogRef.afterClosed$.subscribe((result) => {
                                if (result) {
                                    this.updatedReportingPeriods.emit();
                                }
                            });
                        },
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("Reporting Period", "Name"),
            this.utilityFunctionsService.createDateColumnDef("Cover Crop Self Report Start Date", "CoverCropSelfReportStartDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDateColumnDef("Cover Crop Self Report End Date", "CoverCropSelfReportEndDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createBasicColumnDef("Ready for Account Holders", "CoverCropSelfReportReadyForAccountHolders", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.CoverCropSelfReportReadyForAccountHolders),
                UseCustomDropdownFilter: true,
            }),
        ];

        this.usageLocationTypeColumnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Edit",
                        CssClasses: "btn btn-primary btn-sm",
                        ActionIcon: null,
                        ActionHandler: () => {
                            const dialogRef = this.dialogService.open(EditCoverCropUsageLocationTypeModalComponent, {
                                data: {
                                    GeographyID: this.currentGeography.GeographyID,
                                    UsageLocationType: params.data,
                                },
                            });

                            dialogRef.afterClosed$.subscribe((result) => {
                                if (result) {
                                    this.updatedUsageLocationTypes.emit();
                                }
                            });
                        },
                    },
                ];
            }),
            this.utilityFunctionsService.createBasicColumnDef("Usage Location Type", "Name"),
            this.utilityFunctionsService.createBasicColumnDef("Can Be Selected", "CanBeSelectedInCoverCropForm", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.CanBeSelectedInCoverCropForm),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Counts as Cover Cropped", "CountsAsCoverCropped", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.CountsAsCoverCropped),
                UseCustomDropdownFilter: true,
            }),
        ];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.currentGeography) {
            this.coverCropEnabled = changes.currentGeography.currentValue.AllowCoverCropSelfReporting;
        }
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridApi.sizeColumnsToFit();
    }

    public onEnabledToggle() {
        const confirmOptions = {
            title: `Confirm: ${!this.coverCropEnabled ? "Disable" : "Enable"} Cover Crop Self Reporting`,
            message: `Are you sure you want to ${!this.coverCropEnabled ? "disable" : "enable"} cover crop self reporting?`,
            buttonClassYes: "btn btn-primary",
            buttonTextYes: "Continue",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.saveGeographyChanges();
            } else {
                this.coverCropEnabled = !this.coverCropEnabled;
            }
        });
    }

    public saveGeographyChanges(): void {
        let updateDto = {
            AllowCoverCropSelfReporting: this.coverCropEnabled,
            AllowFallowSelfReporting: this.currentGeography.AllowFallowSelfReporting,
            AllowWaterMeasurementSelfReporting: this.currentGeography.AllowWaterMeasurementSelfReporting,
        };

        this.geographyService.updateGeographySelfReportingGeography(this.currentGeography.GeographyID, updateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Cover crop self reporting ${this.coverCropEnabled ? "enabled" : "disabled"}.`, AlertContext.Success));
                this.updatedGeography.emit();
            },
        });
    }
}
