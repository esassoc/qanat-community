import { Component, OnInit } from "@angular/core";
import { Observable, tap } from "rxjs";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { GeographyConfigurationService } from "src/app/shared/generated/api/geography-configuration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, AsyncPipe } from "@angular/common";

@Component({
    selector: "landing-page-configure",
    templateUrl: "./landing-page-configure.component.html",
    styleUrl: "./landing-page-configure.component.scss",
    standalone: true,
    imports: [NgIf, PageHeaderComponent, FormsModule, AlertDisplayComponent, AsyncPipe],
})
export class LandingPageConfigureComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.LandingPageConfigure;
    public currentGeography$: Observable<GeographyDto>;
    public isEnabled: boolean;
    public geographyID: number;
    constructor(
        private selectedGeographyService: SelectedGeographyService,
        private confirmService: ConfirmService,
        private geographyConfigurationService: GeographyConfigurationService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.pipe(
            tap((geography) => {
                this.isEnabled = geography.LandingPageEnabled;
                this.geographyID = geography.GeographyID;
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
            title: "Confirm: Disable Landing Page",
            message: "Are you sure you want to disable Landing Page?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.geographiesGeographyIDConfigurationToggleLandingPagePut(this.geographyID, false).subscribe((response) => {
                    this.alertService.pushAlert(
                        new Alert(
                            "This feature is currently disabled. You can configure this feature, but changes will not take effect until the feature is enabled.",
                            AlertContext.Danger,
                            false
                        )
                    );
                    this.alertService.pushAlert(new Alert("Disabled Landing Page.", AlertContext.Success));
                });
            } else {
                this.isEnabled = true;
            }
        });
    }

    public openEnableModal() {
        const options = {
            title: "Confirm: Enable Landing Page",
            message: "Are you sure you want to enable Landing Page?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.geographyConfigurationService.geographiesGeographyIDConfigurationToggleLandingPagePut(this.geographyID, true).subscribe((response) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Enabled Landing Page.", AlertContext.Success));
                });
            } else {
                this.isEnabled = false;
            }
        });
    }
}
