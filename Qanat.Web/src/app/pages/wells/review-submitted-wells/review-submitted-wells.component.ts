import { Component, OnInit } from "@angular/core";
import { ColDef, ValueGetterParams } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { SubmittedWellRegistrationListItemDto } from "src/app/shared/generated/model/submitted-well-registration-list-item-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { QanatGridComponent } from "src/app/shared/components/qanat-grid/qanat-grid.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { RouterLink } from "@angular/router";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "review-submitted-wells",
    templateUrl: "./review-submitted-wells.component.html",
    styleUrls: ["./review-submitted-wells.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, QanatGridComponent, AsyncPipe, RouterLink]
})
export class ReviewSubmittedWellsComponent implements OnInit {
    public customRichTextId: number = CustomRichTextTypeEnum.ManageReviewSubmittedWells;
    public colDefs: ColDef<SubmittedWellRegistrationListItemDto>[];
    public wellsToReview$: Observable<SubmittedWellRegistrationListItemDto[]>;
    public geography$: Observable<GeographyMinimalDto>;
    public geography: GeographyDto;

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private currentGeographyService: CurrentGeographyService,
        private utilityFunctionsService: UtilityFunctionsService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    ngOnInit(): void {
        this.setupObservable();
    }

    setupObservable(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();
        this.wellsToReview$ = this.geography$.pipe(
            switchMap((geography) => {
                this.geography = geography;
                return this.wellRegistrationService.listSubmittedWellRegistrationsWellRegistration(geography.GeographyID);
            }),
            tap((x) => this.setupColDefs())
        );
    }

    setupColDefs(): void {
        this.colDefs = [
            this.utilityFunctionsService.createActionsColumnDef((params: any) => {
                return [
                    {
                        ActionName: "Review",
                        ActionIcon: "fas fa-clipboard-check",
                        ActionLink: `/well-registry/${this.geography.GeographyName.toLowerCase()}/well/${params.data.WellRegistrationID}/edit`,
                    },
                    { ActionName: "Delete", ActionIcon: "fa fa-times-circle text-danger", ActionHandler: () => this.deleteWellRegistry(params) },
                ];
            }),
            this.utilityFunctionsService.createLinkColumnDef("Well Name", "WellName", "WellRegistrationID", {
                InRouterLink: `/wells/${this.geography.GeographyName.toLowerCase()}/well-registrations/`,
            }),
            this.utilityFunctionsService.createLinkColumnDef("APN", "APN", "ParcelID", {
                InRouterLink: `/parcels/`,
            }),
            this.utilityFunctionsService.createDateColumnDef("Date Submitted", "DateSubmitted", "short"),
            {
                headerName: "Created By",
                field: "CreatedBy",
            },
        ];
    }

    deleteWellRegistry(params: ValueGetterParams<SubmittedWellRegistrationListItemDto, any>) {
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
                    this.alertService.pushAlert(new Alert("Successfully Deleted Well Registration", AlertContext.Success));
                    params.api.applyTransaction({ remove: [params.data] });
                });
            }
        });
    }
}
