import { Component, OnDestroy, OnInit, ViewContainerRef } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent, RowNode } from "ag-grid-community";
import { Observable, Subscription } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { AddMeterModalComponent } from "src/app/shared/components/well/add-meter-modal/add-meter-modal.component";
import { MeterContext, UpdateMeterModalComponent } from "src/app/shared/components/well/update-meter-modal/update-meter-modal.component";
import { MeterService } from "src/app/shared/generated/api/meter.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { MeterGridDto } from "src/app/shared/generated/model/meter-grid-dto";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "src/app/shared/services/modal/modal.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";

@Component({
    selector: "meter-list",
    templateUrl: "./meter-list.component.html",
    styleUrl: "./meter-list.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, QanatGridComponent, AsyncPipe],
})
export class MeterListComponent implements OnInit, OnDestroy {
    public richTextTypeID: number = CustomRichTextTypeEnum.MeterList;
    public meters$: Observable<MeterGridDto[]>;
    public columnDefs: ColDef[];
    private meterGrid: GridApi;

    private geographyID: number;
    private selectedGeography$: Subscription = Subscription.EMPTY;

    constructor(
        private meterService: MeterService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private utilityFunctionsService: UtilityFunctionsService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
        });
        this.meters$ = this.meterService.geographiesGeographyIDMetersGet(this.geographyID);
        this.createColumnDefs();
    }

    ngOnDestroy() {
        this.selectedGeography$.unsubscribe();
    }

    public createColumnDefs() {
        this.columnDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [{ ActionName: "Update Meter", ActionIcon: "fas fa-map", ActionHandler: () => this.updateMeterModal(params.data.MeterID, params.node) }];
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

            this.utilityFunctionsService.createMultiLinkColumnDef("Wells", "WellIDs", "WellID", "WellID", {
                InRouterLink: "../../wells",
                MaxWidth: 300,
            }),
        ];
    }

    onGridReady(event: GridReadyEvent) {
        this.meterGrid = event.api;
    }

    public addMeterModal(e) {
        this.modalService
            .open(AddMeterModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                MeterID: -1,
                GeographyID: this.geographyID,
            } as MeterContext)
            .instance.result.then((result) => {
                if (result) {
                    this.meterGrid.applyTransaction({ add: [result] });
                }
            });
    }

    public updateMeterModal(meterID: number, rowNode: RowNode) {
        this.modalService
            .open(UpdateMeterModalComponent, this.viewContainerRef, { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light }, {
                MeterID: meterID,
                GeographyID: this.geographyID,
            } as MeterContext)
            .instance.result.then((result) => {
                if (result) {
                    rowNode.setData(result);
                }
            });
    }
}
