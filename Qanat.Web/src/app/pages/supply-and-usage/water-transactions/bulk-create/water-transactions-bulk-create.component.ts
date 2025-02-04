import { ChangeDetectorRef, Component, OnInit, OnDestroy } from "@angular/core";
import { BehaviorSubject, combineLatest, forkJoin, Observable, of, switchMap, tap } from "rxjs";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ParcelSupplyUpsertDto } from "src/app/shared/generated/model/parcel-supply-upsert-dto";
import { ParcelWaterSupplyDto, UserDto, GeographyMinimalDto, WaterTypeSimpleDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { AsyncPipe, NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { ParcelSupplyByGeographyService } from "src/app/shared/generated/api/parcel-supply-by-geography.service";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "water-transactions-bulk-create",
    templateUrl: "./water-transactions-bulk-create.component.html",
    styleUrls: ["./water-transactions-bulk-create.component.scss"],
    standalone: true,
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        RouterLink,
        AlertDisplayComponent,
        ReportingPeriodSelectComponent,
        QanatGridComponent,
        NgIf,
        FormsModule,
        NgSelectModule,
        FieldDefinitionComponent,
        ButtonComponent,
    ],
})
export class WaterTransactionsBulkCreateComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public waterTypes$: Observable<WaterTypeSimpleDto[]>;
    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public parcelSupply$: Observable<ParcelWaterSupplyDto[]>;
    public columnDefs$: Observable<ColDef<ParcelWaterSupplyDto>[]>;

    private selectedYearSubject: BehaviorSubject<number> = new BehaviorSubject<number>(null);
    public selectedYear$: Observable<number> = this.selectedYearSubject.asObservable();

    public model: ParcelSupplyUpsertDto;
    public selectedParcels: ParcelWaterSupplyDto[];

    public noParcelsSelected: boolean = true;
    public isLoadingSubmit: boolean = false;
    public richTextTypeID: number = CustomRichTextTypeEnum.WaterTransactionBulkCreate;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private alertService: AlertService,
        private parcelByGeographyService: ParcelByGeographyService,
        private parcelSupplyByGeographyService: ParcelSupplyByGeographyService,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService,
        private zoneGroupService: ZoneGroupService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography().pipe(
            tap((geography) => {
                this.selectedYearSubject.next(geography.DefaultDisplayYear);
                this.model = new ParcelSupplyUpsertDto();
            })
        );

        this.waterTypes$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.waterTypeByGeographyService.geographiesGeographyIDWaterTypesActiveGet(geography.GeographyID);
            })
        );

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geography.GeographyID);
            })
        );

        this.columnDefs$ = combineLatest({ waterTypes: this.waterTypes$, zoneGroups: this.zoneGroups$ }).pipe(
            switchMap(({ waterTypes, zoneGroups }) => {
                const colDefs = this.createColumnDefs(waterTypes, zoneGroups);
                return of(colDefs);
            })
        );

        this.parcelSupply$ = combineLatest({ geography: this.geography$, selectedYear: this.selectedYear$ }).pipe(
            switchMap(({ geography, selectedYear }) => {
                return this.parcelByGeographyService.geographiesGeographyIDParcelsWaterSupplyYearGet(geography.GeographyID, selectedYear);
            })
        );
    }

    public onSelectionChanged(event: SelectionChangedEvent) {
        this.selectedParcels = event.api.getSelectedRows();
        this.noParcelsSelected = this.selectedParcels.length == 0;
    }

    private createColumnDefs(waterTypes: WaterTypeSimpleDto[], zoneGroups: ZoneGroupMinimalDto[]): ColDef<ParcelWaterSupplyDto>[] {
        const columnDefs = [
            this.utilityFunctionsService.createCheckboxSelectionColumnDef(),
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.ParcelID}`, LinkDisplay: params.data.ParcelNumber };
                },
                InRouterLink: "/parcels/",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "ParcelArea"),
            this.utilityFunctionsService.createBasicColumnDef("Parcel Status", "ParcelStatusDisplayName", {
                FieldDefinitionType: "ParcelStatus",
                CustomDropdownFilterField: "ParcelStatusDisplayName",
            }),
        ];

        zoneGroups.forEach((zoneGroup) => {
            columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "ZoneIDs"));
        });

        waterTypes.forEach((waterType) => {
            const waterTypeFieldName = "WaterSupplyByWaterType." + waterType.WaterTypeID;
            columnDefs.push(this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, waterTypeFieldName));
        });

        return columnDefs;
    }

    public onSubmit(geography: GeographyMinimalDto): void {
        this.model.ParcelIDs = this.selectedParcels.map((x) => x.ParcelID);

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.parcelSupplyByGeographyService.geographiesGeographyIDParcelSuppliesBulkPost(geography.GeographyID, this.model).subscribe(
            (response) => {
                this.isLoadingSubmit = false;

                this.router.navigate(["../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
                });
            },
            (error) => {
                this.isLoadingSubmit = false;
            }
        );
    }

    public changeSelectedYear(selectedYear: number) {
        this.selectedYearSubject.next(selectedYear);
    }
}
