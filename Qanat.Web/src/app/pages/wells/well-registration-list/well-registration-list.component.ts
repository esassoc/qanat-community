import { Component, OnInit } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ColDef, ValueGetterParams } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { CustomDropdownFilterComponent } from "src/app/shared/components/custom-dropdown-filter/custom-dropdown-filter.component";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WellRegistrationStatusEnum } from "src/app/shared/generated/enum/well-registration-status-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { WellRegistrationGridRowDto } from "src/app/shared/generated/model/well-registration-grid-row-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { PhonePipe } from "src/app/shared/pipes/phone.pipe";
import { ZipCodePipe as ZipCodePipe } from "src/app/shared/pipes/zipcode.pipe";
import { AlertService } from "src/app/shared/services/alert.service";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { FuelTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/fuel-type-enum";
import { WellRegistrationWaterUseTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/well-registration-water-use-type-enum";

@Component({
    selector: "well-registration-list",
    templateUrl: "./well-registration-list.component.html",
    styleUrls: ["./well-registration-list.component.scss"],
    providers: [PhonePipe, ZipCodePipe],
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, LoadingDirective, AsyncPipe, RouterLink],
})
export class WellRegistrationListComponent implements OnInit {
    public customRichTextId: number = CustomRichTextTypeEnum.ManageAllWellRegistrations;
    public colDefs: ColDef<WellRegistrationGridRowDto>[];
    public geography$: Observable<GeographyMinimalDto>;
    public allWells$: Observable<WellRegistrationGridRowDto[]>;
    public geography: GeographyDto;
    public FuelTypesSelectDropdownOptions = FuelTypesAsSelectDropdownOptions;
    public WaterUseTypesSelectDropdownOptions = WellRegistrationWaterUseTypesAsSelectDropdownOptions;

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private zipCodePipe: ZipCodePipe,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.setupObservable();
    }

    setupObservable(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.allWells$ = this.geography$.pipe(
            switchMap((geography) => {
                this.geography = geography;
                return this.wellRegistrationService.listWellRegistration(geography.GeographyID);
            }),
            tap((x) => {
                this.setupColDefs();
            })
        );
    }

    setupColDefs(): void {
        this.colDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                const actions: any[] = [];

                if (params.data.WellID) {
                    actions.push({
                        ActionName: "View Well Details",
                        ActionLink: `/wells/${params.data.WellID}`,
                    });
                }

                actions.push({
                    ActionName: "View Registration",
                    ActionLink: `${params.data.WellRegistrationID}`,
                });

                if (params.data.WellRegistrationStatusID == WellRegistrationStatusEnum.Approved) return actions;

                actions.push(
                    {
                        ActionName: params.data.WellRegistrationStatusID == WellRegistrationStatusEnum.Submitted ? "Review" : "Edit",
                        ActionLink: `/well-registry/${this.geography.GeographyName.toLowerCase()}/well/${params.data.WellRegistrationID}/edit`,
                        ActionIcon: "fa fa-pencil",
                    },
                    { ActionName: "Delete", ActionIcon: "fa fa-times-circle text-danger", ActionHandler: () => this.deleteWellRegistry(params) }
                );
                return actions;
            }),
            this.utilityFunctionsService.createBasicColumnDef("Well Name", "WellName"),
            this.utilityFunctionsService.createLinkColumnDef("APN", "ParcelNumber", "ParcelID", { InRouterLink: "/parcels/" }),
            {
                headerName: "Status",
                valueGetter: (params) => WellRegistrationStatusEnum[params.data.WellRegistrationStatusID].toString(),
                filter: CustomDropdownFilterComponent,
            },
            this.utilityFunctionsService.createDateColumnDef("Date Submitted", "SubmitDate", "shortDate"),
            this.utilityFunctionsService.createDateColumnDef("Date Approved", "ApprovalDate", "shortDate"),
            { headerName: "Created By", field: "CreateUserName" },
            { headerName: "Created By Email", field: "CreateUserEmail" },
            this.utilityFunctionsService.createMultiLinkColumnDef("Irrigated Parcels", "IrrigatedParcels", "ParcelID", "ParcelNumber", {
                InRouterLink: "/parcels/",
            }),
            { headerName: "Landowner Name", field: "LandownerName" },
            { headerName: "Landowner Business Name", field: "LandownerBusinessName" },
            { headerName: "Landowner Street Address", field: "LandownerStreetAddress" },
            { headerName: "Landowner City", field: "LandownerCity" },
            { headerName: "Landowner State", field: "LandownerState" },
            {
                headerName: "Landowner Zip Code",
                field: "LandownerZipCode",
                valueFormatter: (params) => this.zipCodePipe.transform(params.data.LandownerZipCode),
                filterParams: {
                    textFormatter: this.zipCodePipe.gridFilterTextFormatter,
                },
            },
            this.utilityFunctionsService.createPhoneNumberColumnDef("Landowner Phone", "LandownerPhone"),
            { headerName: "Landowner Email", field: "LandownerEmail" },
            { headerName: "Operator Name", field: "OwnerOperatorName" },
            { headerName: "Operator Business Name", field: "OwnerOperatorBusinessName" },
            { headerName: "Operator Street Address", field: "OwnerOperatorStreetAddress" },
            { headerName: "Operator City", field: "OwnerOperatorCity" },
            { headerName: "Operator State", field: "OwnerOperatorState" },
            {
                headerName: "Operator Zip Code",
                field: "OwnerOperatorZipCode",
                valueFormatter: (params) => this.zipCodePipe.transform(params.data.OwnerOperatorZipCode),
                filterParams: {
                    textFormatter: this.zipCodePipe.gridFilterTextFormatter,
                },
            },
            this.utilityFunctionsService.createPhoneNumberColumnDef("Operator Phone", "OwnerOperatorPhone"),
            { headerName: "Operator Email", field: "OwnerOperatorEmail" },
            { headerName: "State Well No.", field: "WellRegistrationMetadatum.StateWellNumber" },
            { headerName: "State WCR No.", field: "WellRegistrationMetadatum.StateWellCompletionNumber" },
            { headerName: "County Well Permit", field: "WellRegistrationMetadatum.CountyWellPermit" },
            this.utilityFunctionsService.createDateColumnDef("Date Drilled", "DateDrilled", "shortDate"),
            { headerName: "Agricultural Water Use", valueGetter: (params) => (params.data.AgriculturalWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Agricultural Water Use Description", field: "AgriculturalWaterUseDescription" },
            { headerName: "Stock Watering Water Use", valueGetter: (params) => (params.data.StockWateringWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Stock Watering Water Use Description", field: "StockWateringWaterUseDescription" },
            { headerName: "Domestic Water Use", valueGetter: (params) => (params.data.DomesticWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Domestic Water Use Description", field: "DomesticWaterUseDescription" },
            { headerName: "Public Municipal Water Use", valueGetter: (params) => (params.data.PublicMunicipalWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Public Municipal Water Use Description", field: "PublicMunicipalWaterUseDescription" },
            { headerName: "Private Municipal Water Use", valueGetter: (params) => (params.data.PrivateMunicipalWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Private Municipal Water Use Description", field: "PrivateMunicipalWaterUseDescription" },
            { headerName: "Other Water Use", valueGetter: (params) => (params.data.OtherWaterUse ? "Yes" : "No"), filter: CustomDropdownFilterComponent },
            { headerName: "Other Water Use Description", field: "OtherWaterUseDescription" },
            this.utilityFunctionsService.createDecimalColumnDef("Well Depth (ft)", "WellRegistrationMetadatum.WellDepth"),
            this.utilityFunctionsService.createDecimalColumnDef("Casing Diameter (in)", "WellRegistrationMetadatum.CasingDiameter"),
            this.utilityFunctionsService.createDecimalColumnDef("Top of Perforations", "WellRegistrationMetadatum.TopOfPerforations"),
            this.utilityFunctionsService.createDecimalColumnDef("Bottom of Perforations", "WellRegistrationMetadatum.BottomOfPerforations"),
            { headerName: "Serial No. of Water Meter", field: "WellRegistrationMetadatum.SerialNumberOfWaterMeter" },
            { headerName: "Manufacturer of Water Meter", field: "WellRegistrationMetadatum.ManufacturerOfWaterMeter" },
            { headerName: "Electric Meter Number", field: "WellRegistrationMetadatum.ElectricMeterNumber" },
            this.utilityFunctionsService.createDecimalColumnDef("Pump Discharge Diameter (in)", "WellRegistrationMetadatum.PumpDischargeDiameter"),
            this.utilityFunctionsService.createDecimalColumnDef("Motor/Engine (hp)", "WellRegistrationMetadatum.MotorHorsePower"),
            {
                headerName: "Fuel Type",
                valueGetter: (params) => {
                    const fuelTypeDisplayName = this.FuelTypesSelectDropdownOptions.find((x) => params.data.WellRegistrationMetadatum?.FuelTypeID == x.Value)?.Label;
                    return fuelTypeDisplayName;
                },
                filter: CustomDropdownFilterComponent,
            },
            { headerName: "Fuel Type Description", field: "WellRegistrationMetadatum.FuelOther" },
            this.utilityFunctionsService.createDecimalColumnDef("Maximum Flow", "WellRegistrationMetadatum.MaximumFlow"),

            {
                headerName: "Maximum Flow Estimated?",
                valueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.WellRegistrationMetadatum?.IsEstimatedMax, true),
                filter: CustomDropdownFilterComponent,
            },
            this.utilityFunctionsService.createDecimalColumnDef("Typical Pump Flow", "WellRegistrationMetadatum.TypicalPumpFlow"),
            {
                headerName: "Typical Pump Flow Estimated?",
                valueGetter: (params) => this.utilityFunctionsService.booleanValueGetter(params.data.WellRegistrationMetadatum?.IsEstimatedMax, true),
                filter: CustomDropdownFilterComponent,
            },

            this.utilityFunctionsService.createDateColumnDef("Most Recent Pump Test Date", "WellRegistrationMetadatum.PumpTestDatePerformed", "shortDate"),

            { headerName: "Conducted By", field: "WellRegistrationMetadatum.PumpTestBy" },
            { headerName: "Pump Manufacturer", field: "WellRegistrationMetadatum.PumpManufacturer" },
            this.utilityFunctionsService.createDecimalColumnDef("Yield (gpm)", "WellRegistrationMetadatum.PumpYield"),
            this.utilityFunctionsService.createDecimalColumnDef("Static Level (ft)", "WellRegistrationMetadatum.PumpStaticLevel"),
            this.utilityFunctionsService.createDecimalColumnDef("Pumping Level (ft)", "WellRegistrationMetadatum.PumpingLevel"),

            this.utilityFunctionsService.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctionsService.createLatLonColumnDef("Longitude", "Longitude"),
        ];
    }

    deleteWellRegistry(params: ValueGetterParams<WellRegistrationGridRowDto, any>) {
        const confirmOptions = {
            title: "Delete Well Registration",
            message: `Are you sure you want to delete this well registration?`,
            buttonClassYes: "btn btn-danger",
            buttonTextYes: "Delete",
            buttonTextNo: "Cancel",
        };
        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.wellRegistrationService.deleteWellRegistrationWellRegistration(params.data.WellRegistrationID).subscribe(() => {
                    this.alertService.pushAlert(new Alert("Successfully deleted well registration", AlertContext.Success));
                    params.api.applyTransaction({ remove: [params.data] });
                });
            }
        });
    }
}
