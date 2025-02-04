import { Component, OnInit, ViewChild, AfterViewChecked } from "@angular/core";
import { SafeHtml } from "@angular/platform-browser";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { combineLatest, Observable, switchMap, tap } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AllocationPlanPreviewChangesDto } from "src/app/shared/generated/model/allocation-plan-preview-changes-dto";
import { GeographyAllocationPlanConfigurationDto, GeographyMinimalDto, WaterTypeSimpleDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { NgxMaskDirective, provideNgxMask } from "ngx-mask";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "allocation-plans-configure",
    templateUrl: "./allocation-plans-configure.component.html",
    styleUrls: ["./allocation-plans-configure.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, FormsModule, LoadingDirective, AlertDisplayComponent, NgFor, NgxMaskDirective, EditorComponent, RouterLink, AsyncPipe],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }, provideNgxMask()],
})
export class AllocationPlansConfigureComponent implements OnInit, AfterViewChecked {
    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object;

    public geography$: Observable<GeographyMinimalDto>;
    private geographyID: number;

    public model: GeographyAllocationPlanConfigurationDto;
    public modelOnLoad: string;
    public customRichTextContent: SafeHtml;

    public zoneGroups: ZoneGroupMinimalDto[];
    public waterTypes: WaterTypeSimpleDto[];
    public waterTypeSelected: { [waterTypeID: number]: boolean } = {};

    public richTextTypeID = CustomRichTextTypeEnum.AllocationPlansConfigure;
    public isLoadingSubmit: boolean = false;
    public allocationPlan$: Observable<any>;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private allocationPlanService: AllocationPlanService,
        private zoneGroupService: ZoneGroupService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.allocationPlan$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                this.model = null;
                this.modelOnLoad = null;
                this.zoneGroups = null;
                this.waterTypes = null;
                this.waterTypeSelected = {};
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
            }),
            switchMap((geography) => {
                return combineLatest({
                    allocationPlan: this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationGet(geography.GeographyID),
                    zoneGroups: this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geography.GeographyID),
                    waterTypes: this.waterTypeByGeographyService.geographiesGeographyIDWaterTypesActiveGet(geography.GeographyID),
                });
            }),
            tap(({ allocationPlan, zoneGroups, waterTypes }) => {
                this.zoneGroups = zoneGroups;
                this.waterTypes = waterTypes;
                this.waterTypes.forEach((x) => (this.waterTypeSelected[x.WaterTypeID] = false));
                if (!allocationPlan) {
                    this.model = new GeographyAllocationPlanConfigurationDto();
                    this.model.GeographyID = this.geographyID;
                    this.model.WaterTypeIDs = [];
                    this.model.IsActive = false;
                    this.model.IsVisibleToLandowners = false;
                } else {
                    this.model = allocationPlan;
                    this.model.WaterTypeIDs.forEach((x) => (this.waterTypeSelected[x] = true));
                }

                this.modelOnLoad = JSON.stringify(this.model);
            })
        );
    }

    ngAfterViewChecked(): void {
        // We need to use ngAfterViewInit because the image upload needs a reference to the component
        // to setup the blobCache for image base64 encoding
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    canExit() {
        if (!this.modelOnLoad) return true;

        return JSON.stringify(this.model) == this.modelOnLoad;
    }

    public onSubmit() {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.model.WaterTypeIDs = [];
        this.waterTypes.forEach((x) => {
            if (this.waterTypeSelected[x.WaterTypeID]) this.model.WaterTypeIDs.push(x.WaterTypeID);
        });

        if (this.model.GeographyAllocationPlanConfigurationID > 0) {
            this.getConfigurationChangesPreview();
        } else {
            this.saveConfiguration();
        }
    }

    public getConfigurationChangesPreview() {
        this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationPreviewPut(this.geographyID, this.model).subscribe((previewChangesDtos) => {
            if (previewChangesDtos.length == 0) {
                this.saveConfiguration();
            } else {
                this.confirmConfigurationChanges(previewChangesDtos);
            }
        });
    }

    public confirmConfigurationChanges(previewChangesDtos: AllocationPlanPreviewChangesDto[]) {
        const deletingAllocationPlanChanges = previewChangesDtos.filter((x) => x.ToDelete);
        const dateRangeConflictChanges = previewChangesDtos.filter((x) => !x.ToDelete && x.PeriodsToDeleteCount > 0);

        const message =
            (deletingAllocationPlanChanges.length == 0
                ? ""
                : "This update will delete the following Allocation Plans and all associated Allocation Periods:" +
                  `<ul class="mt-3">${deletingAllocationPlanChanges.map(
                      (x) => `<li><b>${x.AllocationPlanDisplayName}</b> (${x.TotalPeriodsCount} Allocation Period${x.TotalPeriodsCount == 1 ? "" : "s"})</li>`
                  )}</ul>` +
                  "<br /><br />") +
            (dateRangeConflictChanges.length == 0
                ? ""
                : `<p>This update will ${
                      deletingAllocationPlanChanges.length > 0 ? "also " : ""
                  } delete Allocation Periods from the following Allocation Plans due to conflicting date ranges:</p>` +
                  `<ul class="mt-3">${dateRangeConflictChanges.map(
                      (x) => `<li><b>${x.AllocationPlanDisplayName}</b> (Deleting ${x.PeriodsToDeleteCount} of ${x.TotalPeriodsCount} Allocation Periods)</li>`
                  )}</ul>` +
                  "<br /><br />") +
            "Are you sure you wish to proceed?";

        this.confirmService
            .confirm({
                title: "Update Configuration",
                message: message,
                buttonTextYes: "Save",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.saveConfiguration();
                } else {
                    this.isLoadingSubmit = false;
                }
            });
    }

    public saveConfiguration() {
        const request =
            this.model.GeographyAllocationPlanConfigurationID > 0
                ? this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationPut(this.geographyID, this.model)
                : this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationPost(this.geographyID, this.model);

        request.subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.modelOnLoad = null;

                this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                    this.alertService.pushAlert(new Alert("Allocation plan configuration successfully saved.", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }

    onToggle() {
        if (this.model.IsActive) {
            this.openEnableModal();
        } else {
            this.openDisableModal();
        }
    }

    public openDisableModal() {
        const options = {
            title: "Confirm: Disable Allocation Plans",
            message: "Are you sure you want to disable Allocation Plans?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.model.IsActive = false;
                this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationPut(this.geographyID, this.model).subscribe((response) => {
                    this.alertService.pushAlert(
                        new Alert(
                            "This feature is currently disabled. You can configure this feature, but changes will not take effect until the feature is enabled.",
                            AlertContext.Danger,
                            false
                        )
                    );
                    this.alertService.pushAlert(new Alert("Disabled Allocation Plans.", AlertContext.Success));
                });
            } else {
                this.model.IsActive = true;
            }
        });
    }

    public openEnableModal() {
        const options = {
            title: "Confirm: Enable Allocation Plan",
            message: "Are you sure you want to enable Allocation Plan?",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.model.IsActive = true;
                this.allocationPlanService.geographiesGeographyIDAllocationPlanConfigurationPut(this.geographyID, this.model).subscribe((response) => {
                    this.alertService.clearAlerts();
                    this.alertService.pushAlert(new Alert("Enabled Allocation Plans.", AlertContext.Success));
                });
            } else {
                this.model.IsActive = false;
            }
        });
    }
}
