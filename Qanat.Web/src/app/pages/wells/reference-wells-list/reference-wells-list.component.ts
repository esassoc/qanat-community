import { Component, OnInit } from "@angular/core";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { WellService } from "src/app/shared/generated/api/well.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ReferenceWellManageGridDto } from "src/app/shared/generated/model/reference-well-manage-grid-dto";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";

@Component({
    selector: "reference-wells-list",
    templateUrl: "./reference-wells-list.component.html",
    styleUrls: ["./reference-wells-list.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, NgIf, QanatGridComponent, AsyncPipe, RouterLink],
})
export class ReferenceWellsListComponent implements OnInit {
    public richTextID: number = CustomRichTextTypeEnum.ReferenceWellsList;
    public columnDefs: ColDef<ReferenceWellManageGridDto>[];
    public referenceWells$: Observable<ReferenceWellManageGridDto[]>;
    public geography$: Observable<GeographyMinimalDto>;
    private referenceWellGrid: GridApi;

    constructor(
        private route: ActivatedRoute,
        private utilityFunctionsService: UtilityFunctionsService,
        private wellService: WellService,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.route.params.pipe(
            switchMap((params) => {
                const geographyName = params.geographyName;
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
            })
        );

        this.referenceWells$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.wellService.geographiesGeographyIDReferenceWellsGridGet(geography.GeographyID);
            })
        );

        this.createColumnDefs();
    }

    public createColumnDefs() {
        this.columnDefs = [
            {
                headerName: "Reference Well ID",
                field: "ReferenceWellID",
                width: 80,
                sortable: false,
                filter: true,
            },
            {
                headerName: "Well Name",
                field: "WellName",
                filter: true,
            },
            {
                headerName: "Well Depth",
                field: "WellDepth",
                filter: true,
            },

            { headerName: "County Permit No", field: "CountyWellPermitNo", filter: true },
            { headerName: "State WCR Number", field: "StateWCRNumber", filter: true },
            this.utilityFunctionsService.createLatLonColumnDef("Latitude", "Latitude"),
            this.utilityFunctionsService.createLatLonColumnDef("Longitude", "Longitude"),
        ];
    }

    onGridReady(event: GridReadyEvent) {
        this.referenceWellGrid = event.api;
    }
}
