import { ChangeDetectorRef, Component, ComponentRef, OnInit } from "@angular/core";
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, switchMap, tap } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyForAdminEditorsDto, UserDto, WaterMeasurementTypeSimpleDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { Control, Map } from "leaflet";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { NoSelectedItemBoxComponent } from "src/app/shared/components/no-selected-item-box/no-selected-item-box.component";
import { FieldDefinitionComponent } from "../../../shared/components/field-definition/field-definition.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ManageZoneGroupCardComponent } from "src/app/shared/components/manage-zone-group-card/manage-zone-group-card.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "zone-group-data-uploader",
    templateUrl: "./zone-group-data-uploader.component.html",
    styleUrls: ["./zone-group-data-uploader.component.scss"],
    imports: [
        PageHeaderComponent,
        AlertDisplayComponent,
        FormFieldComponent,
        FormsModule,
        ReactiveFormsModule,
        ManageZoneGroupCardComponent,
        ButtonComponent,
        QanatMapComponent,
        ZoneGroupLayerComponent,
        NgSelectModule,
        FieldDefinitionComponent,
        AsyncPipe,
        NoSelectedItemBoxComponent,
        RouterLink,
    ],
})
export class ZoneGroupDataUploaderComponent implements OnInit {
    geographyZoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public FormFieldType = FormFieldType;
    public zoneGroupSelectOptions: FormInputOption[];

    public selectedZoneGroupField: FormControl<ZoneGroupMinimalDto> = new FormControl<ZoneGroupMinimalDto>(null, [Validators.required]);

    public selectedZoneGroupFieldValue$: Observable<ZoneGroupMinimalDto>;

    public geography: GeographyForAdminEditorsDto;
    public currentUser: UserDto;
    public zoneGroupSlug: string;

    public waterMeasurementTypes: WaterMeasurementTypeSimpleDto[];

    public fileUpload: File;
    public fileUploadHeaders: string[];
    public fileUploadElementID = "file-upload";
    public fileUploadElement: HTMLInputElement;

    public apnColumnName: string;
    public zoneColumnName: string;
    public displayFileInputPanel = true;
    public isLoadingSubmit: boolean = false;
    public richTextTypeID = CustomRichTextTypeEnum.ZoneGroupCSVUploader;

    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(
        public authenticationService: AuthenticationService,
        private confirmService: ConfirmService,
        private cdr: ChangeDetectorRef,
        private alertService: AlertService,
        private geographyService: GeographyService,
        private zoneGroupService: ZoneGroupService,
        private waterMeasurementService: WaterMeasurementService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geographyZoneGroups$ = this.geographyService.getByGeographyNameForAdminEditorGeography(geographyName).pipe(
            tap((geography) => {
                this.geography = geography;
            }),
            switchMap((geography) => {
                return this.zoneGroupService.listZoneGroup(geography.GeographyID);
            }),
            tap((zoneGroups) => {
                this.zoneGroupSelectOptions = zoneGroups.map((zoneGroup) => {
                    return {
                        Value: zoneGroup,
                        Label: zoneGroup.ZoneGroupName,
                    } as FormInputOption;
                });
                this.selectedZoneGroupFieldValue$ = this.selectedZoneGroupField.valueChanges.pipe(
                    tap(() => {
                        this.fileUpload = null;
                        this.cdr.markForCheck();
                        this.cdr.detectChanges();
                    })
                );
            })
        );
    }

    public onFileUploadChange(event: any) {
        if (!event.target.files || !event.target.files.length) {
            this.fileUpload = null;
            event.target.value = null;
        }

        const [file] = event.target.files;
        this.fileUpload = event.target.files.item(0);
    }

    public onClickFileUpload() {
        if (!this.fileUploadElement) {
            this.fileUploadElement = <HTMLInputElement>document.getElementById(this.fileUploadElementID);
        }
        this.fileUploadElement.click();
    }

    public getFileUploadHeaders() {
        if (!this.fileUpload) {
            this.alertService.pushAlert(new Alert("The File field is required.", AlertContext.Danger));
            return;
        }

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.waterMeasurementService.listCSVHeadersWaterMeasurement(this.geography.GeographyID, this.fileUpload).subscribe({
            next: (fileUploadHeaders) => {
                this.isLoadingSubmit = false;
                this.fileUploadHeaders = fileUploadHeaders;
                this.displayFileInputPanel = false;
            },
            error: (error) => {
                this.isLoadingSubmit = false;
            },
        });
    }

    private validateRequiredFields(): boolean {
        let isValid = true;

        if (!this.apnColumnName) {
            this.alertService.pushAlert(new Alert("The APN Column field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.zoneColumnName) {
            this.alertService.pushAlert(new Alert("The Zone Column field is required.", AlertContext.Danger));
            isValid = false;
        }

        return isValid;
    }

    public onSubmit(selectedZoneGroup: ZoneGroupMinimalDto) {
        this.alertService.clearAlerts();

        if (!this.validateRequiredFields()) return;

        this.isLoadingSubmit = true;
        const confirmOptions = {
            title: "Confirm Zone Group Data Updates",
            message: `You are about to modify the data for ${selectedZoneGroup.ZoneGroupName}.<br /><br />
      Are you sure you wish to proceed?`,
            buttonTextYes: "Confirm",
            buttonClassYes: "btn-primary",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;
        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.zoneGroupService
                    .uploadZoneGroupDataZoneGroup(this.geography.GeographyID, selectedZoneGroup.ZoneGroupID, this.fileUpload, this.apnColumnName, this.zoneColumnName)
                    .subscribe({
                        next: (response) => {
                            this.isLoadingSubmit = false;
                            const successMessage = this.createSuccessMessage(response);
                            this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success));
                            this.displayFileInputPanel = true;
                            this.fileUpload = null;
                        },
                        error: (error) => {
                            this.isLoadingSubmit = false;
                            this.cdr.detectChanges();

                            if (error.error?.UploadedFile) {
                                this.fileUpload = null;
                                this.fileUploadElement.value = null;
                            }
                        },
                    });
            }
        });
    }

    public createSuccessMessage(response) {
        let totalEntries = 0;
        let successMessage = "";
        for (const [key, value] of Object.entries(response)) {
            totalEntries += Number(value);
            if (key != "unassigned") {
                successMessage += `<li>${value} APNs added to ${key}.</li>`;
            } else {
                successMessage += `<li>${value} APNs were unassigned for ${this.selectedZoneGroupField.value.ZoneGroupName}.</li></ul>`;
            }
        }
        successMessage = `<ul><li>${totalEntries} APNs updated with zone group data for ${this.selectedZoneGroupField.value.ZoneGroupName}.</li>` + successMessage;
        return successMessage;
    }

    closeColumns(): void {
        this.fileUploadHeaders = null;
        this.displayFileInputPanel = true;
    }

    public map: Map;
    public layerControl: Control.Layers;
    public mapIsReady: boolean = false;
    handleMapReady(event: QanatMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }
}
