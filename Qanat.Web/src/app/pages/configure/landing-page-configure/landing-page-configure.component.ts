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
import { AsyncPipe } from "@angular/common";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { ActivatedRoute } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "landing-page-configure",
    templateUrl: "./landing-page-configure.component.html",
    styleUrl: "./landing-page-configure.component.scss",
    imports: [PageHeaderComponent, FormsModule, AlertDisplayComponent, AsyncPipe]
})
export class LandingPageConfigureComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.LandingPageConfigure;

    public geography$: Observable<GeographyMinimalDto>;
    public geographyID: number;
    public isEnabled: boolean;

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
                const geographyName = params["geographyName"];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.alertService.clearAlerts();
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
                this.isEnabled = geography.GeographyConfiguration.LandingPageEnabled;
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
            title: "Confirm: Disable Account Sign-up",
            message: "Are you sure you want to disable Account Sign-up?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.toggleLandingPageConfigurationGeographyConfiguration(this.geographyID, false).subscribe((response) => {
                    this.alertService.pushAlert(
                        new Alert(
                            "This feature is currently disabled. You can configure this feature, but changes will not take effect until the feature is enabled.",
                            AlertContext.Danger,
                            false
                        )
                    );
                    this.alertService.pushAlert(new Alert("Disabled Account Sign-up.", AlertContext.Success));
                });
            } else {
                this.isEnabled = true;
            }
        });
    }

    public openEnableModal() {
        const options = {
            title: "Confirm: Enable Account Sign-up",
            message: "Are you sure you want to enable Account Sign-up?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.toggleLandingPageConfigurationGeographyConfiguration(this.geographyID, true).subscribe((response) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Enabled Account Sign-up.", AlertContext.Success));
                });
            } else {
                this.isEnabled = false;
            }
        });
    }
}
