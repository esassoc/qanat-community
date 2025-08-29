import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { GeographyForAdminEditorsDto } from "src/app/shared/generated/model/geography-for-admin-editors-dto";
import { UsageLocationByGeographyService } from "src/app/shared/generated/api/usage-location-by-geography.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";

@Component({
    selector: "upload-usage-location-gdb",
    templateUrl: "./upload-usage-location-gdb.component.html",
    styleUrl: "./upload-usage-location-gdb.component.scss",
    imports: [PageHeaderComponent, AlertDisplayComponent, FormsModule, FormFieldComponent, ReactiveFormsModule, ButtonLoadingDirective, AsyncPipe, LoadingDirective],
})
export class UploadUsageLocationGdbComponent implements OnInit {
    public FormFieldType = FormFieldType;
    public isLoadingSubmit: boolean = false;
    public geography$: Observable<GeographyForAdminEditorsDto>;
    public geographyID: number;
    public geographySlug: string;
    public customRichTextType: CustomRichTextTypeEnum = CustomRichTextTypeEnum.UsageLocationGdbUpload;

    public isLoading: boolean = true;

    public uploadFormField: FormControl<Blob> = new FormControl<Blob>(null, [Validators.required]);
    public formGroup: FormGroup<{
        usageLocationOption: FormControl<boolean>;
    }> = new FormGroup({
        usageLocationOption: new FormControl<boolean>(true, [Validators.required]),
    });

    constructor(
        private usageLocationByGeographyService: UsageLocationByGeographyService,
        private geographyService: GeographyService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.geographySlug = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.geographyService.getByGeographyNameForAdminEditorGeography(this.geographySlug).pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.isLoading = false;
            })
        );
    }

    onSubmit(): void {
        this.usageLocationByGeographyService.uploadGDBUsageLocationByGeography(this.geographyID, this.uploadFormField.value).subscribe((response) => {
            this.isLoadingSubmit = false;

            this.router.navigate(["/supply-and-usage", this.geographySlug, "usage-locations", "upload-usage-location-gdb", "review"]).then(() => {});
        });
    }
}
