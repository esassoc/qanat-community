import { Component, OnInit } from '@angular/core';
import { ColDef, ValueGetterParams } from 'ag-grid-community';
import { Observable, switchMap, tap } from 'rxjs';
import { ConfirmService } from 'src/app/shared/services/confirm/confirm.service';
import { UtilityFunctionsService } from 'src/app/shared/services/utility-functions.service';
import { WellRegistrationService } from 'src/app/shared/generated/api/well-registration.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { GeographyDto } from 'src/app/shared/generated/model/geography-dto';
import { SubmittedWellRegistrationListItemDto } from 'src/app/shared/generated/model/submitted-well-registration-list-item-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { SelectedGeographyService } from 'src/app/shared/services/selected-geography.service';
import { QanatGridComponent } from 'src/app/shared/components/qanat-grid/qanat-grid.component';
import { NgIf, AsyncPipe } from '@angular/common';
import { AlertDisplayComponent } from '../../../../shared/components/alert-display/alert-display.component';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'review-submitted-wells',
  templateUrl: './review-submitted-wells.component.html',
  styleUrls: ['./review-submitted-wells.component.scss'],
  standalone: true,
  imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, QanatGridComponent, AsyncPipe, IconComponent, RouterLink],
})
export class ReviewSubmittedWellsComponent implements OnInit {
  public customRichTextId: number = CustomRichTextTypeEnum.ManageReviewSubmittedWells;
  public colDefs: ColDef<SubmittedWellRegistrationListItemDto>[];
  public wellsToReview$: Observable<SubmittedWellRegistrationListItemDto[]>;
  public geography: GeographyDto;

  constructor(
    private wellRegistrationService: WellRegistrationService,
    private selectedGeographyService: SelectedGeographyService,
    private utilityFunctionsService: UtilityFunctionsService,
    private alertService: AlertService,
    private confirmService: ConfirmService
  ) {}

  ngOnInit(): void {
    this.setupObservable();
  }

  setupObservable(): void {
    this.wellsToReview$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.pipe(
      switchMap((geography) => {
        this.geography = geography;
        return this.wellRegistrationService.geographiesGeographyIDWellRegistrationsSubmittedGet(geography.GeographyID);
      }),
      tap((x) => this.setupColDefs())
    );
  }

  setupColDefs(): void {
    this.colDefs = [
      this.utilityFunctionsService.createActionsColumnDef((params: any) => {
        return [
          {
            ActionName: 'Review',
            ActionIcon: 'fas fa-clipboard-check',
            ActionLink: `/well-registry/${this.geography.GeographyName.toLowerCase()}/well/${params.data.WellRegistrationID}/edit`,
          },
          { ActionName: 'Delete', ActionIcon: 'fa fa-times-circle text-danger', ActionHandler: () => this.deleteWellRegistry(params) },
        ];
      }),
      this.utilityFunctionsService.createLinkColumnDef('Well Name', 'WellName', 'WellRegistrationID', {
        InRouterLink: `/manage/${this.geography.GeographyName.toLowerCase()}/wells/well-registrations/`,
      }),
      this.utilityFunctionsService.createLinkColumnDef('APN', 'APN', 'ParcelID', {
        InRouterLink: `/manage/${this.geography.GeographyName.toLowerCase()}/parcels/`,
      }),
      this.utilityFunctionsService.createDateColumnDef('Date Submitted', 'DateSubmitted', 'short'),
      {
        headerName: 'Created By',
        field: 'CreatedBy',
      },
    ];
  }

  deleteWellRegistry(params: ValueGetterParams<SubmittedWellRegistrationListItemDto, any>) {
    const confirmOptions = {
      title: 'Delete Well Registration',
      message: `Are you sure you want to delete this well registration?`,
      buttonClassYes: 'btn btn-danger',
      buttonTextYes: 'Delete',
      buttonTextNo: 'Cancel',
    };
    this.confirmService.confirm(confirmOptions).then((confirmed) => {
      if (confirmed) {
        this.wellRegistrationService.wellRegistrationsWellRegistrationIDDelete(params.data.WellRegistrationID).subscribe(() => {
          this.alertService.pushAlert(new Alert('Successfully Deleted Well Registration', AlertContext.Success));
          params.api.applyTransaction({ remove: [params.data] });
        });
      }
    });
  }
}
