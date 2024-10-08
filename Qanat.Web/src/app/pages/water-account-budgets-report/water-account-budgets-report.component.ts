import { Component, OnDestroy, OnInit } from '@angular/core';
import { ColDef } from 'ag-grid-community';
import { Subscription } from 'rxjs';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { WaterTypeService } from 'src/app/shared/generated/api/water-type.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { WaterAccountBudgetReportDto } from 'src/app/shared/generated/model/water-account-budget-report-dto';
import { WaterTypeSimpleDto } from 'src/app/shared/generated/model/water-type-simple-dto';
import { SelectedGeographyService } from 'src/app/shared/services/selected-geography.service';
import { ReportingPeriodSelectComponent } from '../../shared/components/reporting-period-select/reporting-period-select.component';
import { NgIf } from '@angular/common';
import { AlertDisplayComponent } from '../../shared/components/alert-display/alert-display.component';
import { LoadingDirective } from '../../shared/directives/loading.directive';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { WaterAccountByGeographyService } from 'src/app/shared/generated/api/water-account-by-geography.service';

@Component({
  selector: 'water-account-budgets-report',
  templateUrl: './water-account-budgets-report.component.html',
  styleUrls: ['./water-account-budgets-report.component.scss'],
  standalone: true,
  imports: [LoadingDirective, PageHeaderComponent, AlertDisplayComponent, NgIf, ReportingPeriodSelectComponent, QanatGridComponent],
})
export class WaterAccountBudgetsReportComponent implements OnInit, OnDestroy {
  private selectedGeography$: Subscription = Subscription.EMPTY;
  public geographyID: number;

  public selectedYear: number;
  public waterTypes: WaterTypeSimpleDto[];

  public waterAccountBudgetReports: WaterAccountBudgetReportDto[];
  public columnDefs: ColDef[];

  public richTextTypeID = CustomRichTextTypeEnum.WaterAccountBudgetReport;
  public isLoading = true;

  constructor(
    private selectedGeographyService: SelectedGeographyService,
    private waterTypeService: WaterTypeService,
    private waterAccountByGeographyService: WaterAccountByGeographyService,
    private utilityFunctionsService: UtilityFunctionsService
  ) {}

  ngOnInit(): void {
    this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
      this.geographyID = geography.GeographyID;
      this.selectedYear = geography.DefaultDisplayYear;
      this.getDataForGeographyID();
    });
  }

  private getDataForGeographyID() {
    this.updateReportForSelectedYear(this.selectedYear);

    this.waterTypeService.geographiesGeographyIDWaterTypesActiveGet(this.geographyID).subscribe((waterTypes) => {
      this.waterTypes = waterTypes;
      this.isLoading = false;

      this.createColumnDefs();
    });
  }

  ngOnDestroy(): void {
    this.selectedGeography$.unsubscribe();
  }

  private createColumnDefs(): void {
    this.columnDefs = [
      this.utilityFunctionsService.createLinkColumnDef('Water Account #', 'WaterAccountNumber', 'WaterAccountUrl', {
        FieldDefinitionType: 'WaterAccount',
        ValueGetter: (params) => {
          return { LinkValue: `${params.data.WaterAccountID}/water-budget`, LinkDisplay: params.data.WaterAccountNumber };
        },
        InRouterLink: '../../../water-dashboard/water-accounts/',
      }),
      { headerName: 'Water Account Name', field: 'WaterAccountName', width: 155 },
      this.utilityFunctionsService.createDecimalColumnDef('Total Supply (ac-ft)', 'TotalSupply'),
      ...this.waterTypes.map((waterType) => {
        const fieldName = 'WaterSupplyByWaterType.' + waterType.WaterTypeID;
        return this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, fieldName, { WaterType: waterType });
      }),
      this.utilityFunctionsService.createDecimalColumnDef('Total Usage (ac-ft)', 'UsageToDate'),
      this.utilityFunctionsService.createDecimalColumnDef('Current Available (ac-ft)', 'CurrentAvailable'),
      this.utilityFunctionsService.createDecimalColumnDef('Acres Managed', 'AcresManaged'),
    ];
  }

  updateReportForSelectedYear(selectedYear: number) {
    this.selectedYear = selectedYear;

    this.waterAccountByGeographyService
      .geographiesGeographyIDWaterAccountsBudgetReportsYearsYearGet(this.geographyID, this.selectedYear)
      .subscribe((waterAccountBudgetReport) => {
        this.waterAccountBudgetReports = waterAccountBudgetReport;
      });
  }
}
