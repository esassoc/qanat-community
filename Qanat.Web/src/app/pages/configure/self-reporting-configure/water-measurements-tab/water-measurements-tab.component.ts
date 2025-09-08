import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

@Component({
    selector: "water-measurements-tab",
    imports: [FormsModule],
    templateUrl: "./water-measurements-tab.component.html",
    styleUrl: "./water-measurements-tab.component.scss",
})
export class WaterMeasurementsTabComponent implements OnChanges {
    @Input() currentGeography: GeographyMinimalDto;

    @Output() public updatedGeography: EventEmitter<void> = new EventEmitter<void>();

    public waterMeasurementsEnabled: boolean = false;

    public constructor(
        private confirmService: ConfirmService,
        private geographyService: GeographyService,
        private alertService: AlertService
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.currentGeography) {
            this.waterMeasurementsEnabled = changes.currentGeography.currentValue.AllowWaterMeasurementSelfReporting;
        }
    }

    public onEnabledToggle() {
        const confirmOptions = {
            title: `Confirm: ${!this.waterMeasurementsEnabled ? "Disable" : "Enable"} Surface Water Self Reporting`,
            message: `Are you sure you want to ${!this.waterMeasurementsEnabled ? "disable" : "enable"} surface water self reporting?`,
            buttonClassYes: "btn btn-primary",
            buttonTextYes: "Continue",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.saveGeographyChanges();
            } else {
                this.waterMeasurementsEnabled = !this.waterMeasurementsEnabled;
            }
        });
    }

    public saveGeographyChanges(): void {
        let updateDto = {
            AllowCoverCropSelfReporting: this.currentGeography.AllowCoverCropSelfReporting,
            AllowFallowSelfReporting: this.currentGeography.AllowFallowSelfReporting,
            AllowWaterMeasurementSelfReporting: this.waterMeasurementsEnabled,
        };

        this.geographyService.updateGeographySelfReportingGeography(this.currentGeography.GeographyID, updateDto).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`Water measurement self reporting ${this.waterMeasurementsEnabled ? "enabled" : "disabled"}.`, AlertContext.Success));
                this.updatedGeography.emit();
            },
        });
    }
}
