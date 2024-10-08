import { Component, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    AdminGeographyUpdateRequestDto,
    AdminGeographyUpdateRequestDtoForm,
    AdminGeographyUpdateRequestDtoFormControls,
} from "src/app/shared/generated/model/admin-geography-update-request-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ButtonLoadingDirective } from "../../shared/directives/button-loading.directive";
import { FormFieldComponent } from "../../shared/components/forms/form-field/form-field.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";

@Component({
    selector: "geography-setup",
    templateUrl: "./geography-setup.component.html",
    styleUrl: "./geography-setup.component.scss",
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, FormsModule, FormFieldComponent, ReactiveFormsModule, ButtonLoadingDirective, RouterLink, AsyncPipe],
})
export class GeographySetupComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public geography$: Observable<AdminGeographyUpdateRequestDto>;
    public FormFieldType = FormFieldType;
    public modelOnLoad: string;
    public apnRegexPatternOnLoad: string;
    public richTextTypeID = CustomRichTextTypeEnum.AdminGeographyEditForm;

    public formGroup: FormGroup<AdminGeographyUpdateRequestDtoForm> = new FormGroup<AdminGeographyUpdateRequestDtoForm>({
        GeographyID: AdminGeographyUpdateRequestDtoFormControls.GeographyID(),
        GeographyDisplayName: AdminGeographyUpdateRequestDtoFormControls.GeographyDisplayName(),
        StartYear: AdminGeographyUpdateRequestDtoFormControls.StartYear(),
        DefaultDisplayYear: AdminGeographyUpdateRequestDtoFormControls.DefaultDisplayYear(),
        APNRegexPattern: AdminGeographyUpdateRequestDtoFormControls.APNRegexPattern(),
        APNRegexDisplay: AdminGeographyUpdateRequestDtoFormControls.APNRegexDisplay(),
        LandownerDashboardSupplyLabel: AdminGeographyUpdateRequestDtoFormControls.LandownerDashboardSupplyLabel(),
        LandownerDashboardUsageLabel: AdminGeographyUpdateRequestDtoFormControls.LandownerDashboardUsageLabel(),
        ContactEmail: AdminGeographyUpdateRequestDtoFormControls.ContactEmail(),
        ContactPhoneNumber: AdminGeographyUpdateRequestDtoFormControls.ContactPhoneNumber(),
        DisplayUsageGeometriesAsField: AdminGeographyUpdateRequestDtoFormControls.DisplayUsageGeometriesAsField(),
        AllowLandownersToRequestAccountChanges: AdminGeographyUpdateRequestDtoFormControls.AllowLandownersToRequestAccountChanges(),
    });

    public isLoadingSubmit: boolean = false;
    public isReadonly: boolean = true;

    constructor(
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private route: ActivatedRoute,
        private router: Router,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    public ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                // set form to readonly for non-admin users
                this.isReadonly = !this.authenticationService.hasPermission(currentUser, PermissionEnum.GeographyRights, RightsEnum.Update);
                if (this.isReadonly) {
                    Object.keys(this.formGroup.controls).forEach((control) => {
                        this.formGroup.get(control).disable();
                    });
                }
            })
        );

        const geographySlug = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.geographyService.geographyGeographyNameEditGet(geographySlug).pipe(
            tap((geography) => {
                this.formGroup.patchValue(geography);
                this.modelOnLoad = JSON.stringify(geography);
                this.apnRegexPatternOnLoad = geography.APNRegexPattern;
            })
        );
    }

    canExit() {
        if (!this.modelOnLoad) return true;
        return JSON.stringify(this.formGroup.value) == this.modelOnLoad;
    }

    public onSubmit(): void {
        // present confirmation modal if user has changed APN  regex pattern
        if (this.formGroup.controls.APNRegexPattern.value == this.apnRegexPatternOnLoad) {
            this.saveGeographyChanges();
            return;
        }

        const confirmOptions = {
            title: "Update APN Regex Pattern",
            message: `Are you sure you want to update this geography's APN regex pattern? This will affect the importing of parcels to the platform, and could cause errors if not configured correctly.`,
            buttonClassYes: "btn btn-primary",
            buttonTextYes: "Continue",
            buttonTextNo: "Cancel",
        };

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.saveGeographyChanges();
            }
        });
    }

    private saveGeographyChanges() {
        this.isLoadingSubmit = true;
        this.geographyService.geographiesGeographyIDEditPut(this.formGroup.controls.GeographyID.value, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.modelOnLoad = null;
                this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                    this.alertService.pushAlert(new Alert("Geography attributed successfully updated.", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
