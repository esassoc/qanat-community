import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { forkJoin, Subscription } from "rxjs";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ColDef, SelectionChangedEvent } from "ag-grid-community";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { ParcelSupplyUpsertDto } from "src/app/shared/generated/model/parcel-supply-upsert-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { ParcelWaterSupplyDto, UserDto, WaterTypeSimpleDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelSupplyService } from "src/app/shared/generated/api/parcel-supply.service";
import { WaterTypeService } from "src/app/shared/generated/api/water-type.service";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { NgSelectModule } from "@ng-select/ng-select";
import { FormsModule } from "@angular/forms";
import { NgIf } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonComponent } from "src/app/shared/components/button/button.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { ReportingPeriodSelectComponent } from "src/app/shared/components/reporting-period-select/reporting-period-select.component";

@Component({
    selector: "water-transactions-bulk-create",
    templateUrl: "./water-transactions-bulk-create.component.html",
    styleUrls: ["./water-transactions-bulk-create.component.scss"],
    standalone: true,
    imports: [
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
    private currentUser: UserDto;
    private selectedGeography$: Subscription = Subscription.EMPTY;
    public geographyID: number;
    public geographySlug: string;

    public model: ParcelSupplyUpsertDto;
    public selectedParcels: ParcelWaterSupplyDto[];
    public waterTypes: WaterTypeSimpleDto[];
    public parcelWaterSupplyAndUsages: ParcelWaterSupplyDto[];
    public zoneGroups: ZoneGroupMinimalDto[];
    public columnDefs: ColDef<ParcelWaterSupplyDto>[];

    public noParcelsSelected: boolean = true;
    public isLoadingSubmit: boolean = false;
    public richTextTypeID: number = CustomRichTextTypeEnum.WaterTransactionBulkCreate;
    public selectedYear: number;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private parcelService: ParcelService,
        private ParcelSupplyService: ParcelSupplyService,
        private waterTypeService: WaterTypeService,
        private utilityFunctionsService: UtilityFunctionsService,
        private selectedGeographyService: SelectedGeographyService,
        private zoneGroupService: ZoneGroupService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.geographySlug = geography.GeographyName.replace(" ", "-").toLowerCase();
            this.selectedYear = geography.DefaultDisplayYear;
            this.getDataForGeographyID(this.geographyID);

            this.model = new ParcelSupplyUpsertDto();
        });
    }

    private getDataForGeographyID(geographyID: number): void {
        forkJoin({
            currentUser: this.authenticationService.getCurrentUser(),
            zoneGroups: this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID),
        }).subscribe(({ currentUser, zoneGroups }) => {
            this.currentUser = currentUser;
            this.zoneGroups = zoneGroups;

            forkJoin({
                waterTypes: this.waterTypeService.geographiesGeographyIDWaterTypesActiveGet(geographyID),
                parcelWaterSupplyAndUsages: this.parcelService.geographiesGeographyIDParcelsWaterSupplyYearGet(geographyID, this.selectedYear),
            }).subscribe(
                ({ waterTypes, parcelWaterSupplyAndUsages }) => {
                    this.waterTypes = waterTypes;
                    this.parcelWaterSupplyAndUsages = parcelWaterSupplyAndUsages;

                    this.createColumnDefs();
                },
                (error) => {
                    this.waterTypes = [];
                    this.parcelWaterSupplyAndUsages = [];
                }
            );
        });
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.selectedGeography$.unsubscribe();
    }

    public onSelectionChanged(event: SelectionChangedEvent) {
        this.selectedParcels = event.api.getSelectedRows();
        this.noParcelsSelected = this.selectedParcels.length == 0;
    }

    private createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createCheckboxSelectionColumnDef(),
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", {
                ValueGetter: (params) => {
                    return { LinkValue: `${params.data.ParcelID}`, LinkDisplay: params.data.ParcelNumber };
                },
                InRouterLink: "../../../../water-dashboard/parcels/",
            }),
            this.utilityFunctionsService.createDecimalColumnDef("Area (acres)", "ParcelArea"),
            this.utilityFunctionsService.createBasicColumnDef("Parcel Status", "ParcelStatusDisplayName", {
                FieldDefinitionType: "ParcelStatus",
                CustomDropdownFilterField: "ParcelStatusDisplayName",
            }),
        ];

        this.zoneGroups.forEach((zoneGroup) => {
            this.columnDefs.push(this.utilityFunctionsService.createZoneGroupColumnDef(zoneGroup, "Zones"));
        });

        this.waterTypes.forEach((waterType) => {
            const waterTypeFieldName = "WaterSupplyByWaterType." + waterType.WaterTypeID;
            this.columnDefs.push(this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, waterTypeFieldName));
        });
    }

    private insertWaterTypeColDefs() {
        const colDefsWithWaterTypes = this.columnDefs;

        this.waterTypes.forEach((waterType) => {
            const waterTypeFieldName = "WaterSupplyByWaterType." + waterType.WaterTypeID;
            colDefsWithWaterTypes.push(this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, waterTypeFieldName));
        });
    }

    public onSubmit(): void {
        this.model.ParcelIDs = this.selectedParcels.map((x) => x.ParcelID);

        this.isLoadingSubmit = true;
        this.alertService.clearAlerts();

        this.ParcelSupplyService.geographiesGeographyIDParcelSuppliesBulkPost(this.geographyID, this.model).subscribe(
            (response) => {
                this.isLoadingSubmit = false;

                this.router.navigate(["../"], { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
                });
            },
            (error) => {
                this.isLoadingSubmit = false;
                this.cdr.detectChanges();
            }
        );
    }

    public changeSelectedYear(selectedYear: number) {
        this.selectedYear = selectedYear;

        this.parcelService.geographiesGeographyIDParcelsWaterSupplyYearGet(this.geographyID, this.selectedYear).subscribe((parcelWaterSupplyAndUsages) => {
            this.parcelWaterSupplyAndUsages = parcelWaterSupplyAndUsages;
        });
    }
}
