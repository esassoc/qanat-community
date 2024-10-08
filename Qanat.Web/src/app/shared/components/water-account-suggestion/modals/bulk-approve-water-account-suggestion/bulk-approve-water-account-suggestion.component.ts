import { Component, ComponentRef, OnInit } from "@angular/core";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../../modal/modal.component";
import { CreateWaterAccountFromSuggestionDto, ParcelDisplayDto, ParcelWithGeoJSONDto } from "src/app/shared/generated/model/models";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { CustomGeoJSONLayer } from "../../../parcel-map/parcel-map.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { UntypedFormGroup, UntypedFormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgFor, NgIf, DecimalPipe } from "@angular/common";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { WaterAccountByGeographyService } from "src/app/shared/generated/api/water-account-by-geography.service";

@Component({
    selector: "bulk-approve-water-account-suggestion",
    templateUrl: "./bulk-approve-water-account-suggestion.component.html",
    styleUrls: ["./bulk-approve-water-account-suggestion.component.scss"],
    standalone: true,
    imports: [IconComponent, FormsModule, ReactiveFormsModule, NgFor, NgIf, DecimalPipe],
})
export class BulkApproveWaterAccountSuggestionComponent implements OnInit, IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: BulkApproveWaterAccountSuggestionContext;
    public availableYears: number[];

    public formGroup = new UntypedFormGroup({
        waterYearSelection: new UntypedFormControl("", [Validators.required]),
    });

    public selectedParcel: ParcelDisplayDto;
    public originalWaterAccountParcels: ParcelWithGeoJSONDto[];
    public waterAccountParcels: ParcelWithGeoJSONDto[];
    public isLoadingSubmit = false;

    public customGeoJSONLayers: CustomGeoJSONLayer[] = [];

    constructor(
        private modalService: ModalService,
        private waterAccountByGeographyService: WaterAccountByGeographyService,
        private reportingPeriodService: ReportingPeriodService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(this.modalContext.GeographyID).subscribe((availableYears) => {
            this.availableYears = availableYears.reverse();
        });
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        const effectiveYear = this.formGroup.get("waterYearSelection").value;
        this.modalContext.WaterAccountSuggestions.forEach((x) => (x.EffectiveYear = effectiveYear));
        this.waterAccountByGeographyService
            .geographiesGeographyIDWaterAccountsSuggestedBulkCreatePost(this.modalContext.GeographyID, this.modalContext.WaterAccountSuggestions)
            .subscribe((response) => {
                this.alertService.pushAlert(
                    new Alert(
                        `Successfully approved ${this.modalContext.WaterAccountSuggestions.length} water account${this.modalContext.WaterAccountSuggestions.length == 1 ? "" : "s"}.`,
                        AlertContext.Success
                    )
                );
                this.modalService.close(this.modalComponentRef, true);
                this.isLoadingSubmit = false;
            });
    }
}

export interface BulkApproveWaterAccountSuggestionContext {
    GeographyID: number;
    WaterAccountSuggestions: CreateWaterAccountFromSuggestionDto[];
}
