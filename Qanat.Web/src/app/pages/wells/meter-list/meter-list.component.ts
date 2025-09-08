import { Component, OnInit } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent, RowNode } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { AddMeterModalComponent } from "src/app/shared/components/well/modals/add-meter-modal/add-meter-modal.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { MeterGridDto } from "src/app/shared/generated/model/meter-grid-dto";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { MeterByGeographyService } from "src/app/shared/generated/api/meter-by-geography.service";
import { DialogService } from "@ngneat/dialog";
import { UpdateMeterModalComponent } from "src/app/shared/components/well/modals/update-meter-modal/update-meter-modal.component";

@Component({
    selector: "meter-list",
    templateUrl: "./meter-list.component.html",
    styleUrl: "./meter-list.component.scss",
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, AsyncPipe, RouterLink],
})
export class MeterListComponent implements OnInit {
    public richTextTypeID: number = CustomRichTextTypeEnum.MeterList;

    public geography$: Observable<GeographyMinimalDto>;
    public meters$: Observable<MeterGridDto[]>;

    public columnDefs: ColDef[];
    private meterGrid: GridApi;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private geographyService: GeographyService,
        private meterByGeographyService: MeterByGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private currentGeographyService: CurrentGeographyService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.createColumnDefs(geography);
            })
        );

        this.meters$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.meterByGeographyService.getMetersMeterByGeography(geography.GeographyID);
            })
        );
    }

    public createColumnDefs(geography: GeographyMinimalDto) {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                var actions = [{ ActionName: "Update Meter", ActionHandler: () => this.updateMeterModal(params.data.MeterID, params.node, geography.GeographyID) }];

                if (params.data.WellID) {
                    actions.push({
                        ActionName: "Add Reading",
                        ActionHandler: () => this.router.navigate(["/wells", params.data.WellID, "new-meter-reading"], { relativeTo: this.route }),
                    });
                }

                return actions;
            }),
            this.utilityFunctionsService.createBasicColumnDef("Serial Number", "SerialNumber", {
                FieldDefinitionType: "SerialNumber",
                FieldDefinitionLabelOverride: "Serial Number",
            }),
            this.utilityFunctionsService.createBasicColumnDef("Device Name", "DeviceName"),
            this.utilityFunctionsService.createBasicColumnDef("Make", "Make"),
            this.utilityFunctionsService.createBasicColumnDef("Model Number", "ModelNumber"),
            this.utilityFunctionsService.createBasicColumnDef("Status", "MeterStatus.MeterStatusDisplayName", {
                CustomDropdownFilterField: "MeterStatus.MeterStatusDisplayName",
            }),
            this.utilityFunctionsService.createLinkColumnDef("Well", "WellID", "WellID", { InRouterLink: "/wells/" }),
        ];
    }

    onGridReady(event: GridReadyEvent) {
        this.meterGrid = event.api;
    }

    public addMeterModal(geography: GeographyMinimalDto) {
        const dialogRef = this.dialogService.open(AddMeterModalComponent, {
            data: {
                MeterID: -1,
                GeographyID: geography.GeographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.meterGrid.applyTransaction({ add: [result] });
            }
        });
    }

    public updateMeterModal(meterID: number, rowNode: RowNode, geographyID: number) {
        const dialogRef = this.dialogService.open(UpdateMeterModalComponent, {
            data: {
                MeterID: meterID,
                GeographyID: geographyID,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                rowNode.setData(result);
            }
        });
    }
}
