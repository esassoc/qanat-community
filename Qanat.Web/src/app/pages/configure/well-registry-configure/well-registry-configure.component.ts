import { Component, OnInit } from "@angular/core";
import { Observable, switchMap, tap } from "rxjs";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographyConfigurationService } from "src/app/shared/generated/api/geography-configuration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { ActivatedRoute } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { routeParams } from "src/app/app.routes";

@Component({
    selector: "well-registry-configure",
    templateUrl: "./well-registry-configure.component.html",
    styleUrls: ["./well-registry-configure.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, FormsModule, AlertDisplayComponent, AsyncPipe],
})
export class WellRegistryConfigureComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryConfigurationPage;

    public geography$: Observable<GeographyMinimalDto>;
    public isEnabled: boolean;
    public geographyID: number;

    constructor(
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private confirmService: ConfirmService,
        private geographyConfigurationService: GeographyConfigurationService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.alertService.clearAlerts();
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
                this.isEnabled = geography.GeographyConfiguration.WellRegistryEnabled;
                if (!this.isEnabled) {
                    this.alertService.pushAlert(
                        new Alert(
                            "This feature is currently disabled. You can configure this feature, but changes will not take effect until the feature is enabled.",
                            AlertContext.Danger,
                            false
                        )
                    );
                }
            })
        );
    }

    onToggle() {
        if (this.isEnabled) {
            this.openEnableModal();
        } else {
            this.openDisableModal();
        }
    }

    public openDisableModal() {
        const options = {
            title: "Confirm: Disable Well Registry",
            message: "Are you sure you want to disable Well Registry?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.geographiesGeographyIDConfigurationMeterConfigurationPut(this.geographyID, false).subscribe((response) => {
                    this.alertService.pushAlert(
                        new Alert(
                            "This feature is currently disabled. You can configure this feature, but changes will not take effect until the feature is enabled.",
                            AlertContext.Danger,
                            false
                        )
                    );
                    this.alertService.pushAlert(new Alert("Disabled Well Registry.", AlertContext.Success));
                });
            } else {
                this.isEnabled = true;
            }
        });
    }

    public openEnableModal() {
        const options = {
            title: "Confirm: Enable Well Registry",
            message: "Are you sure you want to enable Well Registry?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.geographiesGeographyIDConfigurationToggleWellRegistryPut(this.geographyID, true).subscribe((response) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Enabled Well Registry.", AlertContext.Success));
                });
            } else {
                this.isEnabled = false;
            }
        });
    }
}
