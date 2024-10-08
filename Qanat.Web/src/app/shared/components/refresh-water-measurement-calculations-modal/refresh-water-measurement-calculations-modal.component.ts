import { NgFor } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { NgSelectModule } from "@ng-select/ng-select";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { WaterMeasurementService } from "src/app/shared/generated/api/water-measurement.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "refresh-water-measurement-calculations-modal",
    standalone: true,
    imports: [FormsModule, NgSelectModule, ButtonLoadingDirective, NgFor],
    templateUrl: "./refresh-water-measurement-calculations-modal.component.html",
    styleUrl: "./refresh-water-measurement-calculations-modal.component.scss",
})
export class RefreshWaterMeasurementCalculationsModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: RefreshWaterMeasurementCalculationsContext;

    public selectedMonth: number;
    public selectedYear: number;
    public years: number[];
    public months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    public isLoadingSubmit: boolean = false;

    constructor(
        private waterMeasurementService: WaterMeasurementService,
        private alertService: AlertService,
        private modalService: ModalService
    ) {}

    public ngOnInit(): void {
        const currentDate = new Date();

        this.selectedMonth = currentDate.getMonth();
        this.selectedYear = currentDate.getFullYear();

        this.years = [];
        for (let year = this.selectedYear; year >= 2016; year--) {
            this.years.push(year);
        }
    }

    public close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.alertService.clearAlerts();
        this.isLoadingSubmit = true;

        this.waterMeasurementService
            .geographiesGeographyIDWaterMeasurementsCalculationsYearMonthPost(this.modalContext.GeographyID, this.selectedYear, this.selectedMonth + 1)
            .subscribe({
                next: () => {
                    this.isLoadingSubmit = false;
                    this.modalService.close(this.modalComponentRef, null);
                    this.alertService.pushAlert(
                        new Alert(
                            `${this.modalContext.GeographyName} water measurements successfully recalculated for ${this.months[this.selectedMonth]} ${this.selectedYear}`,
                            AlertContext.Success
                        )
                    );
                },
                error: () => {
                    this.isLoadingSubmit = false;
                    this.modalComponentRef.instance.close();
                },
            });
    }
}

export class RefreshWaterMeasurementCalculationsContext {
    public GeographyID: number;
    public GeographyName: string;
    public GeographyStartYear: number;
}
