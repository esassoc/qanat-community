import { Component, ComponentRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { Observable, tap, map } from "rxjs";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ParcelStatusEnum } from "src/app/shared/generated/enum/parcel-status-enum";
import { ParcelBulkUpdateParcelStatusDto, ParcelBulkUpdateParcelStatusDtoForm } from "src/app/shared/generated/model/models";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "parcel-modify-parcel-status-modal",
    standalone: true,
    imports: [CommonModule, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
    templateUrl: "./parcel-modify-parcel-status-modal.component.html",
    styleUrls: ["./parcel-modify-parcel-status-modal.component.scss"],
})
export class ParcelModifyParcelStatusModalComponent implements IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    public FormFieldType = FormFieldType;

    public modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;
    public isLoadingSubmit: boolean = false;
    public effectiveYearOptions$: Observable<SelectDropdownOption[]>;

    public statusOptions: SelectDropdownOption[] = [
        { Value: null, Label: "Select Parcel Status:", Disabled: true },
        { Value: ParcelStatusEnum.Excluded, Label: "Excluded", Disabled: false },
        { Value: ParcelStatusEnum.Inactive, Label: "Inactive", Disabled: false },
        { Value: ParcelStatusEnum.Unassigned, Label: "Unassigned", Disabled: false },
    ];

    public formGroup: FormGroup<ParcelBulkUpdateParcelStatusDtoForm> = new FormGroup<ParcelBulkUpdateParcelStatusDtoForm>({
        ParcelStatusID: new FormControl<ParcelStatusEnum>(null, [Validators.required]),
        EndYear: new FormControl<number>(null, [Validators.required]),
    });

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private parcelService: ParcelService,
        private reportingPeriodService: ReportingPeriodService
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDGet(this.modalContext.ParcelID).pipe(
            tap((x) => {
                this.effectiveYearOptions$ = this.reportingPeriodService.geographiesGeographyIDReportingPeriodYearsGet(x.GeographyID).pipe(
                    map((years) => {
                        let options = years.map((x) => ({ Value: x, Label: x.toString() }) as SelectDropdownOption);
                        options = [{ Value: null, Label: "- Select -", Disabled: true }, ...options]; // insert an empty option at the front
                        return options;
                    })
                );
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;

        const submitDto = new ParcelBulkUpdateParcelStatusDto(this.formGroup.value);
        submitDto.ParcelIDs = [this.modalContext.ParcelID];
        if (!submitDto.EndYear) {
            submitDto.EndYear = -1;
        }

        this.parcelService.geographiesGeographyIDParcelsBulkUpdateParcelStatusPost(this.modalContext.GeographyID, submitDto).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Parcels successfully updated!", AlertContext.Success));
            this.modalService.close(this.modalComponentRef, true);
        });
    }
}
