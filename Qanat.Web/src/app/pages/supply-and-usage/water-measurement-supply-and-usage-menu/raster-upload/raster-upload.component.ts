import { NgIf, NgFor, DatePipe, AsyncPipe } from "@angular/common";
import { ChangeDetectorRef, Component } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router, ActivatedRoute, RouterLink } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { Observable, forkJoin, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UnitTypeService } from "src/app/shared/generated/api/unit-type.service";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { UnitTypeSimpleDto } from "src/app/shared/generated/model/unit-type-simple-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { WaterMeasurementTypeSimpleDto } from "src/app/shared/generated/model/water-measurement-type-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";

@Component({
    selector: "raster-upload",
    standalone: true,
    imports: [AsyncPipe, PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ButtonComponent, NgIf, NgSelectModule, FieldDefinitionComponent, NgFor, DatePipe],
    templateUrl: "./raster-upload.component.html",
    styleUrl: "./raster-upload.component.scss",
})
export class RasterUploadComponent {
    public geography$: Observable<GeographyMinimalDto>;
    public waterMeasurementTypes$: Observable<WaterMeasurementTypeSimpleDto[]>;
    public unitTypes$: Observable<UnitTypeSimpleDto[]>;

    public fileUpload: File;
    public fileUploadHeaders: string[];
    public fileUploadElementID = "file-upload";
    public fileUploadElement: HTMLInputElement;

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
    public richTextTypeID = CustomRichTextTypeEnum.RasterUploadGuidance;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private waterMeasurementService: WaterMeasurementService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private unitTypeService: UnitTypeService,
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

                for (let year = this.effectiveDateYear; year >= 2016; year--) {
                    this.years.push(year);
                }
            })
        );

        this.waterMeasurementTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterMeasurementTypeService.geographiesGeographyIDWaterMeasurementTypesActiveGet(geography.GeographyID);
            })
        );

        this.unitTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.unitTypeService.geographiesGeographyIDUnitTypesGet(geography.GeographyID);
            })
        );
    }

    // private getDataForGeographyID(geographyID: number): void {
    //     this.authenticationService.getCurrentUser().subscribe((currentUser) => {
    //         this.currentUser = currentUser;

    //         forkJoin({
    //             waterUseTypes: this.waterMeasurementTypeService.geographiesGeographyIDWaterMeasurementTypesActiveGet(geographyID),
    //             unitTypes: this.unitTypeService.geographiesGeographyIDUnitTypesGet(geographyID),
    //         }).subscribe(({ waterUseTypes, unitTypes }) => {
    //             this.waterMeasurementTypes = waterUseTypes;
    //             this.unitTypes = unitTypes;
    //         });

    //         const currentDate = new Date();

    //         this.effectiveDateMonth = currentDate.getMonth();
    //         this.effectiveDateYear = currentDate.getFullYear();
    //         this.updateEffectiveDate();

    //         for (let year = this.effectiveDateYear; year >= 2016; year--) {
    //             this.years.push(year);
    //         }

    //         this.cdr.detectChanges();
    //     });
    // }

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

    public getFileMetadata() {
        //MK 9/23/2024 -- Following the CSV pattern and leaving this as two steps on the front end for when we have multiband rasters to support. Add the call to get the metadata/bands here when we need to support that functionality.
        this.displayFileInputPanel = false;
        this.isLoadingSubmit = false;
    }

    public backToFileInputPanel() {
        this.displayFileInputPanel = true;
    }

    public updateEffectiveDate(): void {
        const date = new Date(this.effectiveDateYear, this.effectiveDateMonth + 1);
        this.effectiveDate = date;
    }

    public setEffectiveDateYear(year: number): void {
        this.effectiveDate.setFullYear(year);
    }

    private validateRequiredFields(): boolean {
        let isValid = true;

        if (!this.waterMeasurementTypeID) {
            this.alertService.pushAlert(new Alert("The Water Measurement Type field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.unitTypeID) {
            this.alertService.pushAlert(new Alert("The Unit Type field is required.", AlertContext.Danger));
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
            .geographiesGeographyIDWaterMeasurementsRasterUploadPost(geographyID, this.fileUpload, this.unitTypeID, this.waterMeasurementTypeID, effectiveDate)
            .subscribe({
                next: (response) => {
                    this.isLoadingSubmit = false;

                    this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                        const successMessage = `Raster succesfully uploaded, processing the data now. Please check back later to see the results.`;
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
