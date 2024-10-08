import { Component, ComponentRef, OnInit } from "@angular/core";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "../../modal/modal.component";
import { WellService } from "src/app/shared/generated/api/well.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormGroup, FormsModule } from "@angular/forms";
import { AddWellMeterRequestDtoForm, AddWellMeterRequestDtoFormControls } from "src/app/shared/generated/model/add-well-meter-request-dto";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { Alert } from "src/app/shared/models/alert";
import { Observable } from "rxjs";
import { MeterLinkDisplayDto } from "src/app/shared/generated/model/meter-link-display-dto";
import { MeterService } from "src/app/shared/generated/api/meter.service";
import { NoteComponent } from "../../note/note.component";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { NgIf, AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../alert-display/alert-display.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

@Component({
    selector: "add-well-meter-modal",
    templateUrl: "./add-well-meter-modal.component.html",
    styleUrl: "./add-well-meter-modal.component.scss",
    standalone: true,
    imports: [IconComponent, AlertDisplayComponent, NgIf, FormsModule, SelectDropDownModule, NoteComponent, AsyncPipe],
})
export class AddWellMeterModalComponent implements OnInit {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: AddWellMeterContext;

    public meters$: Observable<MeterLinkDisplayDto[]>;

    public meter: MeterLinkDisplayDto;
    public startDate: Date;

    public meterDropdownConfig = {
        search: true,
        height: "320px",
        placeholder: "Select a Meter",
        searchOnKey: "LinkDisplay",
        displayKey: "LinkDisplay",
    };

    public formGroup = new FormGroup<AddWellMeterRequestDtoForm>({
        MeterID: AddWellMeterRequestDtoFormControls.MeterID(),
        WellID: AddWellMeterRequestDtoFormControls.WellID(),
        StartDate: AddWellMeterRequestDtoFormControls.StartDate(),
    });
    public isLoadingSubmit: boolean = false;

    constructor(
        private modalService: ModalService,
        private wellService: WellService,
        private meterService: MeterService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.meters$ = this.meterService.geographiesGeographyIDMetersDropdownItemsGet(this.modalContext.GeographyID);
        this.formGroup.patchValue({ WellID: this.modalContext.WellID });
    }

    onMeterSelected() {
        if (!this.meter) return;
        this.formGroup.patchValue({ MeterID: this.meter.MeterID });
    }

    onStartDateSelected() {
        if (!this.startDate) return;
        this.formGroup.patchValue({ StartDate: this.startDate.toString() });
    }

    save() {
        this.isLoadingSubmit = true;
        this.wellService.wellsWellIDMetersPost(this.modalContext.WellID, this.formGroup.getRawValue()).subscribe({
            next: (response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Meter successfully assigned to well.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, response);
                this.isLoadingSubmit = false;
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }
}

export class AddWellMeterContext {
    WellID: number;
    WellName: string;
    GeographyID: number;
}
