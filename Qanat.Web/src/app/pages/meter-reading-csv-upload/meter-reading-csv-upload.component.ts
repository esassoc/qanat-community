import { AsyncPipe, Location } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { AgGridAngular } from "ag-grid-angular";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { BehaviorSubject, combineLatest, filter, map, Observable, switchMap, tap } from "rxjs";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "src/app/shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridHeaderComponent } from "src/app/shared/components/qanat-grid-header/qanat-grid-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { MeterReadingByGeographyService } from "src/app/shared/generated/api/meter-reading-by-geography.service";
import { MeterReadingCSVService } from "src/app/shared/generated/api/meter-reading-csv.service";
import { GeographyMinimalDto, MeterReadingGridDto, UserDto } from "src/app/shared/generated/model/models";
import { GeographyHelper } from "src/app/shared/helpers/geography-helper";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { MeterReadingUploadModalComponent } from "./meter-reading-upload-modal/meter-reading-upload-modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "meter-reading-csv-upload",
    imports: [
        PageHeaderComponent,
        ModelNameTagComponent,
        AsyncPipe,
        FormsModule,
        NgSelectModule,
        QanatGridComponent,
        QanatGridHeaderComponent,
        AlertDisplayComponent,
        LoadingDirective,
    ],
    templateUrl: "./meter-reading-csv-upload.component.html",
    styleUrl: "./meter-reading-csv-upload.component.scss",
})
export class MeterReadingCsvUploadComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public geography$: Observable<GeographyMinimalDto>;
    public currentGeography: GeographyMinimalDto;
    public onCurrentGeographySelected: BehaviorSubject<GeographyMinimalDto> = new BehaviorSubject<GeographyMinimalDto>(null);
    public selectedGeography$ = this.onCurrentGeographySelected.asObservable();

    public currentUserGeographies$: Observable<GeographyMinimalDto[]>;
    public compareGeography = GeographyHelper.compareGeography;

    public meterReadings$: Observable<MeterReadingGridDto[]>;
    public refreshMeterReadings: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
    public onRefreshMeterReadings$ = this.refreshMeterReadings.asObservable();

    public isLoading: boolean = true;

    public meterReadingColDefs: ColDef[];
    public meterReadingGridApi: GridApi;
    public meterReadingGridRef: AgGridAngular<any, ColDef<any, any>>;

    public richTextTypeID: number = CustomRichTextTypeEnum.BulkUploadMeterReadings;

    constructor(
        private authenticationService: AuthenticationService,
        private route: ActivatedRoute,
        private location: Location,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private meterReadingsByGeographyService: MeterReadingByGeographyService,
        private meterReadingCSVUploadService: MeterReadingCSVService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private dialogService: DialogService
    ) {
        this.meterReadingColDefs = [
            this.utilityFunctionsService.createLinkColumnDef("Well", "WellName", "WellID", {
                InRouterLink: "/wells/",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Meter", "SerialNumber"),
            this.utilityFunctionsService.createDateColumnDef("Date", "ReadingDate", "M/d/yyyy h:mm a", { IgnoreLocalTimezone: true }),
            this.utilityFunctionsService.createBasicColumnDef("Reader Initials", "ReaderInitials"),
            this.utilityFunctionsService.createDecimalColumnDef("Previous Reading", "PreviousReading", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createDecimalColumnDef("Current Reading", "CurrentReading", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createBasicColumnDef("Unit Type", "MeterReadingUnitTypeDisplayName"),
            this.utilityFunctionsService.createDecimalColumnDef("Volume", "Volume", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createDecimalColumnDef("Volume (ac-ft)", "VolumeInAcreFeet", { MinDecimalPlacesToDisplay: 0, MaxDecimalPlacesToDisplay: 4 }),
            this.utilityFunctionsService.createBasicColumnDef("Comment", "Comment"),
        ];
    }

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser();

        this.currentUserGeographies$ = this.currentUser$.pipe(
            switchMap((user) => {
                return this.geographyService.listForCurrentUserGeography().pipe(
                    map((geographies) => {
                        return geographies.filter((geography) => {
                            return geography.GeographyConfiguration.MetersEnabled;
                        });
                    })
                );
            })
        );

        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                let geographyName = params.geographyName;
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.onGeographySelected(geography);
            })
        );

        this.meterReadings$ = combineLatest({ geography: this.selectedGeography$, _: this.onRefreshMeterReadings$ }).pipe(
            switchMap(({ geography, _ }) => {
                return this.meterReadingsByGeographyService.listByGeographyIDMeterReadingByGeography(geography.GeographyID);
            }),
            tap(() => {
                if (this.meterReadingGridApi) {
                    this.meterReadingGridApi.sizeColumnsToFit();
                    this.meterReadingGridApi.setGridOption("loading", false);
                }
                this.isLoading = false;
            })
        );
    }

    onGeographySelected(geography: GeographyMinimalDto) {
        this.currentGeographyService.setCurrentGeography(geography);
        this.currentGeography = geography;
        this.onCurrentGeographySelected.next(geography);

        if (this.meterReadingGridApi) {
            this.meterReadingGridApi.setGridOption("loading", true);
        }

        this.location.replaceState(`/geographies/${geography.GeographyName.toLowerCase()}/bulk-upload-meter-readings`);
    }

    onGridReady($event: GridReadyEvent<any, any>) {
        this.meterReadingGridApi = $event.api;
        this.meterReadingGridApi.sizeColumnsToFit();
    }

    onGridRefReady($event: AgGridAngular<any, ColDef<any, any>>) {
        this.meterReadingGridRef = $event;
    }

    downloadTemplate(geography: GeographyMinimalDto) {
        this.meterReadingCSVUploadService.downloadCSVTemplateMeterReadingCSV(geography.GeographyID).subscribe((response) => {
            const blob = new Blob([response], {
                type: "text/csv;",
            });

            //Create a fake object to trigger downloading the zip file that was returned
            const a: any = document.createElement("a");
            document.body.appendChild(a);

            a.style = "display: none";
            const url = window.URL.createObjectURL(blob);
            a.href = url;
            a.download = "MeterReadingTemplate.csv";
            a.click();
            window.URL.revokeObjectURL(url);
        });
    }

    openUploadModal(geography: GeographyMinimalDto) {
        const dialogRef = this.dialogService.open(MeterReadingUploadModalComponent, {
            data: {
                GeographyID: geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                if (this.meterReadingGridApi) {
                    this.meterReadingGridApi.setGridOption("loading", true);
                }

                this.alertService.pushAlert(new Alert("Successfully uploaded Meter Readings", AlertContext.Success));
                this.refreshMeterReadings.next(true);
            }
        });
    }
}
