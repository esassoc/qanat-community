import { Component, ComponentRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { FormGroup, FormControl, Validators, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, map, tap } from "rxjs";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { WaterAccountDto } from "src/app/shared/generated/model/water-account-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ReportingPeriodService } from "src/app/shared/generated/api/reporting-period.service";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SearchWaterAccountsComponent } from "src/app/shared/components/search-water-accounts/search-water-accounts.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "parcel-update-water-account-modal",
    standalone: true,
    imports: [CommonModule, IconComponent, FormsModule, ReactiveFormsModule, SearchWaterAccountsComponent, FormFieldComponent, NoteComponent],
    templateUrl: "./parcel-update-water-account-modal.component.html",
    styleUrls: ["./parcel-update-water-account-modal.component.scss"],
})
export class ParcelUpdateWaterAccountModalComponent implements IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    public FormFieldType = FormFieldType;

    public modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;
    public isLoadingSubmit: boolean = false;
    public effectiveYearOptions$: Observable<SelectDropdownOption[]>;

    public formGroup: FormGroup<UpdateParcelWaterAccountForm> = new FormGroup<UpdateParcelWaterAccountForm>({
        WaterAccount: new FormControl<WaterAccountDto>(null, [Validators.required]),
        EffectiveYear: new FormControl<number>(null, [Validators.required]),
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

        const waterAccountID = this.formGroup.controls.WaterAccount.value.WaterAccountID;
        this.parcelService.parcelsParcelIDUpdateWaterAccountWaterAccountIDPost(this.modalContext.ParcelID, waterAccountID, this.formGroup.controls.EffectiveYear.value).subscribe(
            () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully added parcel to water account", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            (error: any) => {
                this.isLoadingSubmit = false;
            },
            () => {
                this.isLoadingSubmit = false;
            }
        );
    }
}

export class UpdateParcelWaterAccountForm {
    WaterAccount: FormControl<WaterAccountDto>;
    EffectiveYear: FormControl<number>;
}
