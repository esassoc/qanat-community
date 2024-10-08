import { Component, OnInit } from '@angular/core';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { Observable } from 'rxjs';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { WellService } from 'src/app/shared/generated/api/well.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { GeographyDto } from 'src/app/shared/generated/model/geography-dto';
import { ReferenceWellManageGridDto } from 'src/app/shared/generated/model/reference-well-manage-grid-dto';
import { SelectedGeographyService } from 'src/app/shared/services/selected-geography.service';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { NgIf, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { AlertDisplayComponent } from 'src/app/shared/components/alert-display/alert-display.component';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';

@Component({
  selector: 'reference-wells-list',
  templateUrl: './reference-wells-list.component.html',
  styleUrls: ['./reference-wells-list.component.scss'],
  standalone: true,
  imports: [PageHeaderComponent, RouterLink, AlertDisplayComponent, NgIf, QanatGridComponent, AsyncPipe, IconComponent, RouterLink],
})
export class ReferenceWellsListComponent implements OnInit {
  public richTextID: number = CustomRichTextTypeEnum.ReferenceWellsList;
  public columnDefs: ColDef<ReferenceWellManageGridDto>[];
  public referenceWells$: Observable<ReferenceWellManageGridDto[]>;
  public geography: GeographyDto;
  private referenceWellGrid: GridApi;

  constructor(
    private utilityFunctionsService: UtilityFunctionsService,
    private wellService: WellService,
    private selectedGeographyService: SelectedGeographyService
  ) {}

  ngOnInit(): void {
    this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
      this.geography = geography;
      this.referenceWells$ = this.wellService.geographiesGeographyIDReferenceWellsGridGet(geography.GeographyID);
      this.createColumnDefs();
    });
  }

  public createColumnDefs() {
    this.columnDefs = [
      {
        headerName: 'Reference Well ID',
        field: 'ReferenceWellID',
        width: 80,
        sortable: false,
        filter: true,
      },
      {
        headerName: 'Well Name',
        field: 'WellName',
        filter: true,
      },
      {
        headerName: 'Well Depth',
        field: 'WellDepth',
        filter: true,
      },

      { headerName: 'County Permit No', field: 'CountyWellPermitNo', filter: true },
      { headerName: 'State WCR Number', field: 'StateWCRNumber', filter: true },
      this.utilityFunctionsService.createLatLonColumnDef('Latitude', 'Latitude'),
      this.utilityFunctionsService.createLatLonColumnDef('Longitude', 'Longitude'),
    ];
  }

  onGridReady(event: GridReadyEvent) {
    this.referenceWellGrid = event.api;
  }
}
