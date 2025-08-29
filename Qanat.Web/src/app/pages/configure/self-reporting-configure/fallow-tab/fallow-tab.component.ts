import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { DialogService } from "@ngneat/dialog";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { GeographyMinimalDto, ReportingPeriodDto, UsageLocationTypeDto } from "src/app/shared/generated/model/models";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { EditFallowingSelfReportVisibilityModalComponent } from "./edit-fallowing-self-report-visibility-modal/edit-fallowing-self-report-visibility-modal.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { FormsModule } from "@angular/forms";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { EditFallowingUsageLocationTypeModalComponent } from "./edit-fallowing-usage-location-type-modal/edit-fallowing-usage-location-type-modal.component";

@Component({
    selector: "fallow-tab",
    imports: [QanatGridComponent, FormsModule],
    templateUrl: "./fallow-tab.component.html",
    styleUrl: "./fallow-tab.component.scss",
})
export class FallowTabComponent implements OnChanges {
    @Input() currentGeography: GeographyMinimalDto;
    @Input() reportingPeriods: ReportingPeriodDto[];
    @Input() usageLocationTypes: UsageLocationTypeDto[];

    @Output() public updatedGeography: EventEmitter<void> = new EventEmitter<void>();
    @Output() public updatedReportingPeriods: EventEmitter<void> = new EventEmitter<void>();
    @Output() public updatedUsageLocationTypes: EventEmitter<void> = new EventEmitter<void>();

    public fallowingEnabled: boolean = false;

    public reportingPeriodColumnDefs: ColDef[];
    public usageLocationTypeColumnDefs: ColDef[];
    private gridApi: GridApi;

    public constructor(
        private geographyService: GeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
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
                            const dialogRef = this.dialogService.open(EditFallowingSelfReportVisibilityModalComponent, {
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
            this.utilityFunctionsService.createDateColumnDef("Fallowing Self Report Start Date", "FallowSelfReportStartDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createDateColumnDef("Fallowing Self Report End Date", "FallowSelfReportEndDate", "M/d/yyyy", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createBasicColumnDef("Ready for Account Holders", "FallowSelfReportReadyForAccountHolders", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.FallowSelfReportReadyForAccountHolders),
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
                            const dialogRef = this.dialogService.open(EditFallowingUsageLocationTypeModalComponent, {
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
            this.utilityFunctionsService.createBasicColumnDef("Can Be Selected", "CanBeSelectedInFallowForm", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.CanBeSelectedInFallowForm),
                UseCustomDropdownFilter: true,
            }),
            this.utilityFunctionsService.createBasicColumnDef("Counts as Fallowed", "CountsAsFallowed", {
                ValueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.CountsAsFallowed),
                UseCustomDropdownFilter: true,
            }),
        ];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.currentGeography) {
            this.fallowingEnabled = changes.currentGeography.currentValue.AllowFallowSelfReporting;
        }
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        this.gridApi.sizeColumnsToFit();
    }

    public onEnabledToggle() {
        const confirmOptions = {
            title: `Confirm: ${!this.fallowingEnabled ? "Disable" : "Enable"} Fallow Self Reporting`,
            message: `Are you sure you want to ${!this.fallowingEnabled ? "disable" : "enable"} fallow self reporting?`,
            buttonClassYes: "btn btn-primary",
            buttonTextYes: "Continue",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.saveGeographyChanges();
            } else {
                this.fallowingEnabled = !this.fallowingEnabled;
            }
        });
    }

    public saveGeographyChanges(): void {
        let updateDto = {
            AllowCoverCropSelfReporting: this.currentGeography.AllowCoverCropSelfReporting,
            AllowFallowSelfReporting: this.fallowingEnabled,
            AllowWaterMeasurementSelfReporting: this.currentGeography.AllowWaterMeasurementSelfReporting,
        };

        this.geographyService.updateGeographySelfReportingGeography(this.currentGeography.GeographyID, updateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Fallow self reporting ${this.fallowingEnabled ? "enabled" : "disabled"}.`, AlertContext.Success));
                this.updatedGeography.emit();
            },
        });
    }
}
