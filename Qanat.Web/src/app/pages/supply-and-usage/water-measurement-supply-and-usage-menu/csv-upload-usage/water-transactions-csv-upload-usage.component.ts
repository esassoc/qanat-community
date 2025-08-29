import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, switchMap, tap } from "rxjs";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { GeographyMinimalDto, WaterMeasurementTypeSimpleDto } from "src/app/shared/generated/model/models";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { DatePipe, AsyncPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { UnitTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/unit-type-enum";

@Component({
    selector: "water-transactions-csv-upload-usage",
    templateUrl: "./water-transactions-csv-upload-usage.component.html",
    styleUrls: ["./water-transactions-csv-upload-usage.component.scss"],
    imports: [AsyncPipe, PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ButtonComponent, NgSelectModule, FieldDefinitionComponent, DatePipe]
})
export class WaterTransactionsCsvUploadUsageComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public waterMeasurementTypes$: Observable<WaterMeasurementTypeSimpleDto[]>;
    public unitTypeSelectDropdownOptions: SelectDropdownOption[] = UnitTypesAsSelectDropdownOptions;

    public fileUpload: File;
    public fileUploadHeaders: string[];
    public fileUploadElementID = "file-upload";
    public fileUploadElement: HTMLInputElement;

    public apnColumnName: string;
    public quantityColumnName: string;
    public commentColumnName: string;
    public waterMeasurementTypeID: number;
    public unitTypeID: number;

    public effectiveDate: Date;
    public effectiveDateMonth: number;
    public effectiveDateYear: number;
    public years = new Array<number>();
    public months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public displayFileInputPanel = true;
    public isLoadingSubmit: boolean = false;
    public richTextTypeID = CustomRichTextTypeEnum.WaterTransactionCsvUploadUsage;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private waterMeasurementService: WaterMeasurementService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private alertService: AlertService,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                const currentDate = new Date();
                this.effectiveDateMonth = currentDate.getMonth();
                this.effectiveDateYear = currentDate.getFullYear();
                this.updateEffectiveDate();

                this.years = [];
                for (let year = this.effectiveDateYear; year >= 2016; year--) {
                    this.years.push(year);
                }
            })
        );

        this.waterMeasurementTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterMeasurementTypeService.getActiveAndEditableWaterMeasurementTypesWaterMeasurementType(geography.GeographyID);
            })
        );
    }

    ngOnDestroy() {
        this.cdr.detach();
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

    public getFileUploadHeaders(geographyID: number) {
        if (!this.fileUpload) {
            this.alertService.pushAlert(new Alert("The File field is required.", AlertContext.Danger));
            return;
        }

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.waterMeasurementService.listCSVHeadersWaterMeasurement(geographyID, this.fileUpload).subscribe(
            (fileUploadHeaders) => {
                this.isLoadingSubmit = false;
                this.fileUploadHeaders = fileUploadHeaders;
                this.displayFileInputPanel = false;
            },
            (error) => {
                this.isLoadingSubmit = false;
            }
        );
    }

    public backToFileInputPanel() {
        this.displayFileInputPanel = true;
    }

    public updateEffectiveDate(): void {
        const date = new Date(this.effectiveDateYear, this.effectiveDateMonth + 1, 0);
        this.effectiveDate = date;
    }

    public setEffectiveDateYear(year: number): void {
        this.effectiveDate.setFullYear(year);
    }

    private validateRequiredFields(): boolean {
        let isValid = true;

        if (!this.waterMeasurementTypeID) {
            this.alertService.pushAlert(new Alert("The Water Use Type field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.unitTypeID) {
            this.alertService.pushAlert(new Alert("The Unit Type field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.apnColumnName) {
            this.alertService.pushAlert(new Alert("The APN Column field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.quantityColumnName) {
            this.alertService.pushAlert(new Alert("The Quantity Column field is required.", AlertContext.Danger));
            isValid = false;
        }

        return isValid;
    }

    public onSubmit(geographyID: number): void {
        this.alertService.clearAlerts();

        if (!this.validateRequiredFields()) return;

        this.isLoadingSubmit = true;

        const effectiveDate = this.utilityFunctionsService.formatDate(this.effectiveDate, "yyyy-MM-dd");
        this.waterMeasurementService
            .newCSVUploadWaterMeasurement(
                geographyID,
                this.fileUpload,
                effectiveDate,
                this.waterMeasurementTypeID,
                this.unitTypeID,
                this.apnColumnName,
                this.quantityColumnName,
                this.commentColumnName
            )
            .subscribe({
                next: (response) => {
                    this.isLoadingSubmit = false;

                    this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                        const successMessage =
                            `${response.TransactionCount} records successfully created.` +
                            (response.UnmatchedParcelNumbers?.length > 0
                                ? ` ${
                                      response.UnmatchedParcelNumbers.length
                                  } records skipped because the following APN/Field ID's were not found in the database: ${response.UnmatchedParcelNumbers.join(", ")}.`
                                : "");
                        this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success));
                    });
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
}
