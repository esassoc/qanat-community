import { Component, OnInit } from "@angular/core";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ReportingPeriodSimpleDto } from "src/app/shared/generated/model/reporting-period-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { forkJoin } from "rxjs";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { UtilityFunctionsService } from "src/app/shared/services/utility-functions.service";
import { FormsModule } from "@angular/forms";
import { NgIf, NgFor } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: "reporting-period-configure",
    templateUrl: "./reporting-period-configure.component.html",
    styleUrls: ["./reporting-period-configure.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, NgIf, FormsModule, NgFor, RouterLink],
})
export class ReportingPeriodConfigureComponent implements OnInit {
    public geographyID: number;
    public currentUser: UserDto;
    public geography: GeographyDto;
    public reportingPeriod: ReportingPeriodSimpleDto;

    public intervals = ["Monthly"];
    public originalReportingPeriod: string;

    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private reportingPeriodService: ReportingPeriodService,
        private alertService: AlertService,
        private geographyService: GeographyService,
        private authenticationService: AuthenticationService,
        private selectedGeographyService: SelectedGeographyService,
        private utilityFunctionService: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });

        this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    public getDataForGeographyID(geographyID: number) {
        forkJoin({
            reportingPeriod: this.reportingPeriodService.geographiesGeographyIDReportingPeriodGet(geographyID),
            geography: this.geographyService.geographiesGeographyIDGet(geographyID),
        }).subscribe(({ reportingPeriod, geography }) => {
            this.geography = geography;
            this.reportingPeriod = reportingPeriod;

            if (!reportingPeriod) {
                this.reportingPeriod = new ReportingPeriodSimpleDto();
            }

            this.originalReportingPeriod = JSON.stringify(reportingPeriod);
        });
    }

    canExit() {
        return this.originalReportingPeriod == JSON.stringify(this.reportingPeriod);
    }

    saveReportingPeriod() {
        this.reportingPeriodService.geographiesGeographyIDReportingPeriodUpdatePost(this.geographyID, this.reportingPeriod).subscribe((response) => {
            this.reportingPeriod = response;
            this.originalReportingPeriod = JSON.stringify(this.reportingPeriod);
            this.alertService.pushAlert(new Alert("Successfully saved!", AlertContext.Success, true));
        });
    }

    getNumberFromMonth(month: string) {
        return this.utilityFunctionService.getNumberFromMonth(month);
    }
}
