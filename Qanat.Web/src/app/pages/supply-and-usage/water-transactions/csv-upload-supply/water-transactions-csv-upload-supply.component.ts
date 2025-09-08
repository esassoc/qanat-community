import { Component, OnInit, Inject, ChangeDetectorRef, DOCUMENT } from "@angular/core";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Observable, switchMap, tap } from "rxjs";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "water-transactions-csv-upload-supply",
    templateUrl: "./water-transactions-csv-upload-supply.component.html",
    styleUrls: ["./water-transactions-csv-upload-supply.component.scss"],
    imports: [AsyncPipe, PageHeaderComponent, RouterLink, AlertDisplayComponent, FormsModule, ButtonComponent, NgSelectModule, FieldDefinitionComponent]
})
export class WaterTransactionsCsvUploadSupplyComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public waterTypes$: Observable<WaterTypeSimpleDto[]>;

    public fileUpload: File;
    public fileUploadElementID = "file-upload";
    public fileUploadElement: HTMLInputElement;
    public effectiveDate: string;
    public waterTypeID: number;

    public isLoadingSubmit: boolean = false;
    public richTextTypeID = CustomRichTextTypeEnum.WaterTransactionCSVUploadSupply;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private alertService: AlertService,
        private currentGeographyService: CurrentGeographyService,
        @Inject(DOCUMENT) private document: Document
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.waterTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterTypeByGeographyService.getWaterTypesWaterTypeByGeography(geography.GeographyID);
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

    private validateRequiredFields(): boolean {
        let isValid = true;

        if (!this.fileUpload) {
            this.alertService.pushAlert(new Alert("The File field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.waterTypeID) {
            this.alertService.pushAlert(new Alert("The Supply Type field is required.", AlertContext.Danger));
            isValid = false;
        }
        if (!this.effectiveDate) {
            this.alertService.pushAlert(new Alert("The Effective Date field is required.", AlertContext.Danger));
            isValid = false;
        }

        return isValid;
    }

    public onSubmit(geography: GeographyMinimalDto) {
        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        if (!this.validateRequiredFields()) {
            this.isLoadingSubmit = false;
            return;
        }

        this.parcelSupplyByGeographyService.newCSVUploadParcelSupplyByGeography(geography.GeographyID, this.fileUpload, this.effectiveDate, this.waterTypeID).subscribe(
            (response) => {
                this.isLoadingSubmit = false;

                this.router.navigate(["../../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
                });
            },
            (error) => {
                this.isLoadingSubmit = false;

                if (error.error?.UploadedFile) {
                    this.fileUpload = null;
                    this.fileUploadElement.value = null;
                }
            }
        );
    }
}
