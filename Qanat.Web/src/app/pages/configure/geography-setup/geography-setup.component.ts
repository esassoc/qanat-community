import { AfterViewInit, ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { combineLatest, Observable, of, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    GeographyForAdminEditorsDto,
    GeographyForAdminEditorsDtoForm,
    GeographyForAdminEditorsDtoFormControls,
} from "src/app/shared/generated/model/geography-for-admin-editors-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ButtonLoadingDirective } from "../../../shared/directives/button-loading.directive";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { WaterMeasurementTypeService } from "src/app/shared/generated/api/water-measurement-type.service";
import { WaterMeasurementTypeSimpleDto } from "src/app/shared/generated/model/water-measurement-type-simple-dto";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "geography-setup",
    templateUrl: "./geography-setup.component.html",
    styleUrl: "./geography-setup.component.scss",
    imports: [PageHeaderComponent, LoadingDirective, AlertDisplayComponent, FormsModule, FormFieldComponent, ReactiveFormsModule, ButtonLoadingDirective, RouterLink, AsyncPipe],
})
export class GeographySetupComponent implements OnInit, AfterViewInit {
    public currentUser$: Observable<UserDto>;
    public geography$: Observable<GeographyForAdminEditorsDto>;
    public isLoading: boolean = false;

    public waterMeasurementTypes$: Observable<WaterMeasurementTypeSimpleDto[]>;

    public FormFieldType = FormFieldType;
    public modelOnLoad: string;
    public apnRegexPatternOnLoad: string;
    public richTextTypeID = CustomRichTextTypeEnum.AdminGeographyEditForm;

    public formGroup: FormGroup<GeographyForAdminEditorsDtoForm> = new FormGroup<GeographyForAdminEditorsDtoForm>({
        GeographyID: GeographyForAdminEditorsDtoFormControls.GeographyID(),
        GeographyDisplayName: GeographyForAdminEditorsDtoFormControls.GeographyDisplayName(),
        APNRegexPattern: GeographyForAdminEditorsDtoFormControls.APNRegexPattern(),
        APNRegexDisplay: GeographyForAdminEditorsDtoFormControls.APNRegexDisplay(),
        LandownerDashboardSupplyLabel: GeographyForAdminEditorsDtoFormControls.LandownerDashboardSupplyLabel(),
        LandownerDashboardUsageLabel: GeographyForAdminEditorsDtoFormControls.LandownerDashboardUsageLabel(),
        ContactEmail: GeographyForAdminEditorsDtoFormControls.ContactEmail(),
        ContactPhoneNumber: GeographyForAdminEditorsDtoFormControls.ContactPhoneNumber(),
        ContactAddressLine1: GeographyForAdminEditorsDtoFormControls.ContactAddressLine1(),
        ContactAddressLine2: GeographyForAdminEditorsDtoFormControls.ContactAddressLine2(),
        AllowLandownersToRequestAccountChanges: GeographyForAdminEditorsDtoFormControls.AllowLandownersToRequestAccountChanges(),
        ShowSupplyOnWaterBudgetComponent: GeographyForAdminEditorsDtoFormControls.ShowSupplyOnWaterBudgetComponent(),
        WaterBudgetSlotAHeader: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotAHeader(),
        WaterBudgetSlotAWaterMeasurementTypeID: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotAWaterMeasurementTypeID(),
        WaterBudgetSlotBHeader: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotBHeader(),
        WaterBudgetSlotBWaterMeasurementTypeID: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotBWaterMeasurementTypeID(),
        WaterBudgetSlotCHeader: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotCHeader(),
        WaterBudgetSlotCWaterMeasurementTypeID: GeographyForAdminEditorsDtoFormControls.WaterBudgetSlotCWaterMeasurementTypeID(),
        BoundingBox: GeographyForAdminEditorsDtoFormControls.BoundingBox(),
        WaterManagers: GeographyForAdminEditorsDtoFormControls.WaterManagers(),
    });

    public isLoadingSubmit: boolean = false;
    public isReadonly: boolean = true;
    public showSupplyOnWaterBudgetComponentFormOptions: boolean;
    public waterMeasurementTypeOptions: FormInputOption[];

    constructor(
        private authenticationService: AuthenticationService,
        private geographyService: GeographyService,
        private currentGeographyService: CurrentGeographyService,
        private waterMeasurementTypeService: WaterMeasurementTypeService,
        private route: ActivatedRoute,
        private router: Router,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
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
    }

    ngAfterViewInit(): void {
        this.geography$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
                this.formGroup.reset();
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];

                //MK 1/9/2025 -- This is a bit of a hack to get the minimal and admin geography data in one call, the data is almost duplicated but its not quite there.
                const geographyMinimal = this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
                const geographyForAdminEdit = this.geographyService.getByGeographyNameForAdminEditorGeography(geographyName);
                return combineLatest({ geographyMinimal: geographyMinimal, geographyForAdminEdit: geographyForAdminEdit });
            }),
            tap((combo) => {
                this.currentGeographyService.setCurrentGeography(combo.geographyMinimal);
            }),
            switchMap((combo) => {
                return of(combo.geographyForAdminEdit);
            }),
            tap((geography) => {
                this.formGroup.patchValue(geography);
                this.modelOnLoad = JSON.stringify(this.formGroup.getRawValue());
                this.apnRegexPatternOnLoad = geography.APNRegexPattern;
                this.waterMeasurementTypes$ = this.waterMeasurementTypeService.getWaterMeasurementTypesWaterMeasurementType(geography.GeographyID).pipe(
                    tap((waterMeasurementTypes) => {
                        this.waterMeasurementTypeOptions = waterMeasurementTypes.map(
                            (type) =>
                                ({
                                    Label: type.WaterMeasurementTypeName,
                                    Value: type.WaterMeasurementTypeID,
                                }) as FormInputOption
                        );
                    })
                );

                this.showSupplyOnWaterBudgetComponentFormOptions = !geography.ShowSupplyOnWaterBudgetComponent;
                this.isLoading = false;
            })
        );

        this.formGroup.get("ShowSupplyOnWaterBudgetComponent")?.valueChanges.subscribe((value) => {
            this.showSupplyOnWaterBudgetComponentFormOptions = !value;
            this.cdr.markForCheck();
        });
    }

    canExit() {
        if (!this.modelOnLoad) {
            return true;
        }

        return JSON.stringify(this.formGroup.getRawValue()) == this.modelOnLoad;
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
        this.geographyService.updateGeographyGeography(this.formGroup.controls.GeographyID.value, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.modelOnLoad = null;
                this.formGroup.reset();
                this.router.navigate([".."], { relativeTo: this.route }).then(() => {
                    this.alertService.pushAlert(new Alert("Geography successfully updated.", AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
