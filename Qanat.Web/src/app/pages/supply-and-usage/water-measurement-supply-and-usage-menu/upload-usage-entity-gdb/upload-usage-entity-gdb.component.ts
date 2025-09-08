import { Component, OnInit } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { UsageEntityByGeographyService } from "src/app/shared/generated/api/usage-entity-by-geography.service";
import { GeographyForAdminEditorsDto } from "src/app/shared/generated/model/geography-for-admin-editors-dto";

@Component({
    selector: "upload-usage-entity-gdb",
    templateUrl: "./upload-usage-entity-gdb.component.html",
    styleUrl: "./upload-usage-entity-gdb.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, FormsModule, FormFieldComponent, ReactiveFormsModule, ButtonLoadingDirective, RouterLink, AsyncPipe],
})
export class UploadUsageEntityGdbComponent implements OnInit {
    public FormFieldType = FormFieldType;
    public isLoadingSubmit: boolean = false;
    public geography$: Observable<GeographyForAdminEditorsDto>;
    public geographyID: number;

    public uploadFormField: FormControl<Blob> = new FormControl<Blob>(null);

    constructor(
        private usageEntityByGeographyService: UsageEntityByGeographyService,
        private geographyService: GeographyService,
        private route: ActivatedRoute,
        private confirmService: ConfirmService,
        private router: Router
    ) {}

    ngOnInit(): void {
        const geographySlug = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.geographyService.geographiesGeographyNameGeographyNameForAdminEditorGet(geographySlug).pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
            })
        );
    }

    onSubmit(): void {
        // todo: confirm service for this upload
        this.isLoadingSubmit = true;
        const options = {
            title: "Confirm: Upload Usage Entities",
            message: "Are you sure you want to upload new Usage Entities? This will erase existing usage entities for this geography and replace them with what is uploaded.",
            buttonClassYes: "btn-danger",
            buttonTextYes: "Confirm",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;

        this.confirmService.confirm(options).then((confirmed) => {
            if (confirmed) {
                this.usageEntityByGeographyService.geographiesGeographyIDUsageEntitiesPost(this.geographyID, this.uploadFormField.value).subscribe((response) => {
                    this.isLoadingSubmit = false;
                    this.router.navigate(["../../"], { relativeTo: this.route });
                });
            }
        });
    }
}
