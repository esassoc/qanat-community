import { ChangeDetectorRef, Component, OnInit, ViewContainerRef } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { Observable, switchMap, tap } from "rxjs";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { AsyncPipe, NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import {
    RefreshWaterMeasurementCalculationsContext,
    RefreshWaterMeasurementCalculationsModalComponent,
} from "src/app/pages/supply-and-usage/water-measurement-supply-and-usage-menu/refresh-water-measurement-calculations-modal/refresh-water-measurement-calculations-modal.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyPublicDto } from "src/app/shared/generated/model/geography-public-dto";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/geography-minimal-dto";
import { DownloadWaterMeasurementsContext, DownloadWaterMeasurementsModalComponent } from "./download-water-measurements-modal/download-water-measurements-modal.component";

@Component({
    selector: "water-measurement-supply-and-usage-menu",
    templateUrl: "./water-measurement-supply-and-usage-menu.html",
    styleUrls: ["./water-measurement-supply-and-usage-menu.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, ButtonComponent, RouterLink, AsyncPipe],
})
export class WaterMeasurementSupplyAndUsageMenu implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;

    public currentUser$: Observable<UserDto>;
    private currentUser: UserDto;

    public richTextTypeID = CustomRichTextTypeEnum.UsageEstimates;
    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(
        private authenticationService: AuthenticationService,
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementService: WaterMeasurementService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.currentUser = user;
            })
        );
    }

    public canCreateTransactions(geography: GeographyMinimalDto): boolean {
        const hasPermission = this.authenticationService.hasOverallPermission(this.currentUser, PermissionEnum.WaterTransactionRights, RightsEnum.Create, geography.GeographyID);
        return hasPermission;
    }

    public openDownloadWaterMeasurementsModal(geography: GeographyMinimalDto) {
        this.modalService
            .open(DownloadWaterMeasurementsModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: geography.GeographyID,
                GeographyName: geography.GeographyName,
                GeographyStartYear: geography.DefaultDisplayYear,
            } as DownloadWaterMeasurementsContext)
            .instance.result.then((result) => {
                if (result) {
                    this.downloadWaterMeasurements(geography, result.year);
                }
            });
    }

    private downloadWaterMeasurements(geography: GeographyMinimalDto, year: number) {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;

        this.waterMeasurementService.geographiesGeographyIDWaterMeasurementsYearsYearExcelDownloadGet(geography.GeographyID, year).subscribe(
            (result) =>
                this.handleDownloadSuccess(result, `${geography.GeographyName}_${year}_waterMeasurements`, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
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

    public refreshWaterMeasurementCalculations(geography: GeographyMinimalDto) {
        this.modalService
            .open(RefreshWaterMeasurementCalculationsModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                GeographyID: geography.GeographyID,
                GeographyName: geography.GeographyName,
                GeographyStartYear: geography.DefaultDisplayYear,
            } as RefreshWaterMeasurementCalculationsContext)
            .instance.result.then((result) => {});
    }
}
