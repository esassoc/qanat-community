import { AsyncPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormsModule, ReactiveFormsModule, FormGroup } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { map, Observable, tap } from "rxjs";
import { FormFieldComponent, SelectDropdownOption, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UsageLocationByGeographyService } from "src/app/shared/generated/api/usage-location-by-geography.service";
import { UsageLocationTypeService } from "src/app/shared/generated/api/usage-location-type.service";
import { ReportingPeriodDto } from "src/app/shared/generated/model/models";
import {
    UsageLocationBulkUpdateUsageLocationTypeDtoForm,
    UsageLocationBulkUpdateUsageLocationTypeDtoFormControls,
} from "src/app/shared/generated/model/usage-location-bulk-update-usage-location-type-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "bulk-update-usage-location-type-modal",
    imports: [FormsModule, IconComponent, FormFieldComponent, ReactiveFormsModule, AsyncPipe, LoadingDirective, NoteComponent, ButtonLoadingDirective],
    templateUrl: "./bulk-update-usage-location-type-modal.component.html",
    styleUrl: "./bulk-update-usage-location-type-modal.component.scss",
})
export class BulkUpdateUsageLocationTypeModalComponent {
    public ref: DialogRef<BulkUpdateUsageLocationTypeModalContext, boolean> = inject(DialogRef);

    public bulkUpdateUsageLocationTypeForm = new FormGroup<UsageLocationBulkUpdateUsageLocationTypeDtoForm>({
        UsageLocationTypeID: UsageLocationBulkUpdateUsageLocationTypeDtoFormControls.UsageLocationTypeID(),
        UsageLocationIDs: UsageLocationBulkUpdateUsageLocationTypeDtoFormControls.UsageLocationIDs(),
        Note: UsageLocationBulkUpdateUsageLocationTypeDtoFormControls.Note(),
    });

    public isLoading: boolean = true;
    public isLoadingSubmit: boolean = false;
    public usageLocationTypeSelectDropdownOptions$: Observable<SelectDropdownOption[]>;
    public FormFieldType = FormFieldType;
    public constructor(
        private usageLocationTypeService: UsageLocationTypeService,
        private usageLocationByGeographyService: UsageLocationByGeographyService,
        private alertService: AlertService
    ) {
        let geographyID = this.ref.data.GeographyID;

        this.usageLocationTypeSelectDropdownOptions$ = this.usageLocationTypeService.listUsageLocationType(geographyID).pipe(
            map((usageLocationTypes) => {
                return usageLocationTypes.map(
                    (type) =>
                        ({
                            Label: type.Name,
                            Value: type.UsageLocationTypeID,
                        }) as SelectDropdownOption
                );
            }),
            tap(() => {
                this.isLoading = false;
            })
        );

        this.bulkUpdateUsageLocationTypeForm.patchValue({
            UsageLocationIDs: this.ref.data.UsageLocationIDs,
        });
    }

    public save(): void {
        this.isLoadingSubmit = true;
        let updateUsageLocationTypeDto = this.bulkUpdateUsageLocationTypeForm.getRawValue();
        this.usageLocationByGeographyService
            .bulkUpdateUsageLocationTypeUsageLocationByGeography(this.ref.data.GeographyID, this.ref.data.ReportingPeriod.ReportingPeriodID, updateUsageLocationTypeDto)
            .subscribe(() => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("Usage Locations have been successfully updated to the selected Usage Location Type.", AlertContext.Success));
                this.ref.close(true);
            });
    }

    public close(): void {
        this.ref.close(false);
    }
}

export class BulkUpdateUsageLocationTypeModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
    UsageLocationIDs: number[];
}
