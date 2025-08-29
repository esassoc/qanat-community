import { AsyncPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { map, Observable, tap } from "rxjs";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ParcelByGeographyService } from "src/app/shared/generated/api/parcel-by-geography.service";
import { UsageLocationService } from "src/app/shared/generated/api/usage-location.service";
import { ReportingPeriodDto } from "src/app/shared/generated/model/models";
import { ParcelIndexGridDto } from "src/app/shared/generated/model/parcel-index-grid-dto";
import { ParcelLinkDisplayDto } from "src/app/shared/generated/model/parcel-link-display-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "migrate-usage-locations-modal",
    imports: [FormsModule, IconComponent, FormFieldComponent, ReactiveFormsModule, AsyncPipe, LoadingDirective, NoteComponent],
    templateUrl: "./migrate-usage-locations-modal.component.html",
    styleUrl: "./migrate-usage-locations-modal.component.scss",
})
export class MigrateUsageLocationsModalComponent {
    public ref: DialogRef<MigrateUsageLocationsModalContext, boolean> = inject(DialogRef);
    public migrateUsageLocationsForm: FormGroup<{ ParcelID: FormControl<number | null> }> = new FormGroup({
        ParcelID: new FormControl<number | null>(null, Validators.required),
    });

    public isLoading: boolean = true;
    public parcelSelectDropdownOptions$: Observable<SelectDropdownOption[]>;
    public FormFieldType = FormFieldType;
    public constructor(
        private parcelByGeographyService: ParcelByGeographyService,
        private usageLocationService: UsageLocationService,
        private alertService: AlertService
    ) {
        let geographyID = this.ref.data.GeographyID;
        let reportingPeriodID = this.ref.data.ReportingPeriod.ReportingPeriodID;
        this.parcelSelectDropdownOptions$ = this.parcelByGeographyService
            .listByGeographyIDByReportingPeriodForCurrentUserAsDisplayLinkParcelByGeography(geographyID, reportingPeriodID)
            .pipe(
                tap(() => {
                    this.isLoading = false;
                }),
                map((parcels: ParcelLinkDisplayDto[]) => {
                    return parcels.map((parcel) => ({
                        Label: parcel.LinkDisplay,
                        Value: parcel.ParcelID,
                    })) as SelectDropdownOption[];
                })
            );
    }

    public save(): void {
        let usageLocationMigrationDto = { UsageLocationIDs: this.ref.data.UsageLocationIDs };
        this.usageLocationService
            .migrateUsageLocationsUsageLocation(this.ref.data.GeographyID, this.migrateUsageLocationsForm.value.ParcelID, usageLocationMigrationDto)
            .subscribe(() => {
                this.alertService.pushAlert(new Alert("Usage Locations have been successfully migrated to the selected Parcel.", AlertContext.Success));
                this.ref.close(true);
            });
    }

    public close(): void {
        this.ref.close(false);
    }
}

export class MigrateUsageLocationsModalContext {
    GeographyID: number;
    ReportingPeriod: ReportingPeriodDto;
    UsageLocationIDs: number[];
}
