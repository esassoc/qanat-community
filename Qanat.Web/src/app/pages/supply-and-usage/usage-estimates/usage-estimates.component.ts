import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewContainerRef } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Subscription } from "rxjs";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import {
    RefreshWaterMeasurementCalculationsContext,
    RefreshWaterMeasurementCalculationsModalComponent,
} from "src/app/shared/components/refresh-water-measurement-calculations-modal/refresh-water-measurement-calculations-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";

@Component({
    selector: "usage-estimates",
    templateUrl: "./usage-estimates.component.html",
    styleUrls: ["./usage-estimates.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, ButtonComponent, RouterLink],
})
export class UsageEstimatesComponent implements OnInit, OnDestroy {
    private selectedGeography$: Subscription = Subscription.EMPTY;

    public geography: GeographyDto;
    private currentUser: UserDto;

    public richTextTypeID = CustomRichTextTypeEnum.UsageEstimates;
    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private selectedGeographyService: SelectedGeographyService,
        private geographyService: GeographyService,
        private waterMeasurementService: WaterMeasurementService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geography = geography;
            this.getDataForGeographyID(geography.GeographyID);
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    private getDataForGeographyID(geographyID: number): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;

            this.geographyService.geographiesGeographyIDGet(geographyID).subscribe((geography) => {
                this.geography = geography;
            });

            this.cdr.detectChanges();
        });
    }

    public canCreateTransactions(): boolean {
        const hasPermission = this.authenticationService.hasOverallPermission(
            this.currentUser,
            PermissionEnum.WaterTransactionRights,
            RightsEnum.Create,
            this.geography.GeographyID
        );
        return hasPermission;
    }

    public downloadWaterMeasurements() {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;

        this.waterMeasurementService.geographiesGeographyIDWaterMeasurementsGet(this.geography.GeographyID).subscribe(
            (result) =>
                this.handleDownloadSuccess(result, `${this.geography.GeographyName}_waterMeasurements`, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            (error) => this.handleDownloadError(error)
        );
    }

    private handleDownloadSuccess(result, fileName, contentType) {
        const blob = new Blob([result], {
            type: contentType,
        });

        //Create a fake object to trigger downloading the zip file that was returned
        const a: any = document.createElement("a");
        document.body.appendChild(a);

        a.style = "display: none";
        const url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isDownloading = false;
    }

    private handleDownloadError(error) {
        this.downloadError = true;
        //Because our return type is ArrayBuffer, the message will be ugly. Convert it and display
        const decodedString = String.fromCharCode.apply(null, new Uint8Array(error.error) as any);
        this.downloadErrorMessage = decodedString;
        this.isDownloading = false;
    }

    public refreshWaterMeasurementCalculations() {
        this.modalService
            .open(RefreshWaterMeasurementCalculationsModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: this.geography.GeographyID,
                GeographyName: this.geography.GeographyName,
                GeographyStartYear: this.geography.StartYear,
            } as RefreshWaterMeasurementCalculationsContext)
            .instance.result.then((result) => {});
    }
}
