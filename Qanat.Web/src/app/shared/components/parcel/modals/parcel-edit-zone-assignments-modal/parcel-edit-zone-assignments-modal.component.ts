import { Component, ComponentRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Observable, tap, map } from "rxjs";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ParcelContext } from "src/app/shared/components/water-account/modals/add-parcel-to-water-account/add-parcel-to-water-account.component";
import { ParcelService } from "src/app/shared/generated/api/parcel.service";
import { ParcelMinimalDto } from "src/app/shared/generated/model/parcel-minimal-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import {
    ParcelDetailDto,
    ParcelZoneAssignmentDto,
    ParcelZoneAssignmentDtoForm,
    ParcelZoneAssignmentFormDtoFormControls,
    ZoneGroupMinimalDto,
} from "src/app/shared/generated/model/models";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "../../../icon/icon.component";

@Component({
    selector: "parcel-edit-zone-assignments-modal",
    standalone: true,
    imports: [CommonModule, IconComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, NoteComponent],
    templateUrl: "./parcel-edit-zone-assignments-modal.component.html",
    styleUrls: ["./parcel-edit-zone-assignments-modal.component.scss"],
})
export class ParcelEditZoneAssignmentsModalComponent implements IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    public FormFieldType = FormFieldType;

    public modalContext: ParcelContext;

    public parcel$: Observable<ParcelMinimalDto>;

    public isLoadingSubmit: boolean = false;
    public zoneGroups$: Observable<any[]>;

    public formGroup: FormGroup<ParcelZoneAssignmentDtoCustomForm> = new FormGroup<ParcelZoneAssignmentDtoCustomForm>({
        ParcelID: ParcelZoneAssignmentFormDtoFormControls.ParcelID(),
        ParcelZoneAssignments: new FormArray<FormGroup<ParcelZoneAssignmentDtoForm>>([]),
    });

    constructor(
        private modalService: ModalService,
        private parcelService: ParcelService,
        private zoneGroupService: ZoneGroupService,
        private alertService: AlertService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.parcel$ = this.parcelService.parcelsParcelIDZonesGet(this.modalContext.ParcelID).pipe(
            tap((parcel) => {
                this.formGroup.controls.ParcelID.patchValue(parcel.ParcelID);
                this.zoneGroups$ = this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.modalContext.GeographyID).pipe(
                    tap((x) => {
                        const zoneGroups = x.map((zoneGroup) => this.createZoneGroupType(zoneGroup, parcel));
                        this.formGroup.setControl("ParcelZoneAssignments", this.formBuilder.array(zoneGroups));
                    }),
                    map((zoneGroups) => {
                        return zoneGroups.map((zoneGroup) => {
                            let options: FormInputOption[] = zoneGroup.ZoneList.map((x) => {
                                return { Value: x.ZoneID, Label: x.ZoneName } as FormInputOption;
                            });
                            options = [{ Value: null, Label: "- None -" } as FormInputOption, ...options];
                            return { ...zoneGroup, Options: options };
                        });
                    })
                );
            })
        );
    }

    createZoneGroupType(zoneGroup: ZoneGroupMinimalDto, parcel: ParcelDetailDto): FormGroup {
        const parcelZoneGroupZoneID = parcel.Zones?.find((x) => x.ZoneGroupID == zoneGroup.ZoneGroupID)?.ZoneID;
        return this.formBuilder.group<ParcelZoneAssignmentDto>({
            ZoneGroupID: zoneGroup.ZoneGroupID,
            ZoneID: parcelZoneGroupZoneID,
        });
    }

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    save() {
        this.isLoadingSubmit = true;
        this.parcelService.parcelsParcelIDEditZoneAssignmentsPost(this.modalContext.ParcelID, this.formGroup.value).subscribe(
            () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully updated parcel Zone Assignments", AlertContext.Success));
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

export class ParcelZoneAssignmentDtoCustomForm {
    ParcelID: FormControl<number>;
    ParcelZoneAssignments: FormArray<FormGroup<ParcelZoneAssignmentDtoForm>>;
    // ParcelZoneAssignments?: FormArray<FormControl<ParcelZoneAssignmentDto[]>>;
}
