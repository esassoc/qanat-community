import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, Subscription, switchMap, tap } from "rxjs";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { CustomAttributeSimpleDto } from "src/app/shared/generated/model/custom-attribute-simple-dto";
import { CustomAttributeTypeSimpleDto } from "src/app/shared/generated/model/custom-attribute-type-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ButtonLoadingDirective } from "../../shared/directives/button-loading.directive";
import { LoadingDirective } from "../../shared/directives/loading.directive";
import { CustomAttributesEditComponent } from "../../shared/components/custom-attributes-edit/custom-attributes-edit.component";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { FormFieldComponent } from "../../shared/components/forms/form-field/form-field.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";

@Component({
    selector: "custom-attributes-configure",
    templateUrl: "./custom-attributes-configure.component.html",
    styleUrl: "./custom-attributes-configure.component.scss",
    standalone: true,
    imports: [
        PageHeaderComponent,
        FormsModule,
        ReactiveFormsModule,
        NgIf,
        FormFieldComponent,
        AlertDisplayComponent,
        CustomAttributesEditComponent,
        LoadingDirective,
        ButtonLoadingDirective,
        AsyncPipe,
    ],
})
export class CustomAttributesConfigureComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public selectedGeography$ = Subscription.EMPTY;

    public customAttributeTypes$: Observable<CustomAttributeTypeSimpleDto[]>;
    public customAttributeTypeDropdownOptions: SelectDropdownOption[];

    public customAttributeData$: Observable<CustomAttributeSimpleDto[]>;
    public customAttributes: CustomAttributeSimpleDto[];
    public customAttributesOnLoad: string;

    public formGroup: FormGroup<CustomAttributePageControlsForm> = new FormGroup<CustomAttributePageControlsForm>({
        GeographyID: new FormControl(null),
        CustomAttributeTypeID: new FormControl(1),
    });

    public FormFieldType = FormFieldType;
    public isLoading: boolean = true;
    public isLoadingSubmit: boolean = false;
    public isReadonly: boolean = false;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private selectedGeographyService: SelectedGeographyService,
        private customAttributeService: CustomAttributeService,
        private alertService: AlertService
    ) {}

    public ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
            tap((currentUser) => {
                // set form to readonly for non-admin users
                this.isReadonly = !this.authenticationService.hasPermission(currentUser, PermissionEnum.GeographyRights, RightsEnum.Update);
            })
        );

        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.formGroup.controls.GeographyID.setValue(geography.GeographyID);
        });

        this.customAttributeTypes$ = this.customAttributeService.customAttributesTypesGet().pipe(
            tap((customAttributeTypes) => {
                this.formGroup.controls.CustomAttributeTypeID.setValue(customAttributeTypes[0].CustomAttributeTypeID);
                this.customAttributeTypeDropdownOptions = customAttributeTypes.map(
                    (x) => ({ Value: x.CustomAttributeTypeID, Label: x.CustomAttributeTypeDisplayName }) as SelectDropdownOption
                );
            })
        );

        // update route query params when form group values change
        this.formGroup.valueChanges.subscribe((x) => {
            this.setRouteQueryParams(x.GeographyID, x.CustomAttributeTypeID);
        });

        // refresh custom attributes when route query params change
        this.customAttributeData$ = this.route.queryParams.pipe(
            tap(() => (this.isLoading = true)),
            switchMap((x) => {
                if (!x.GeographyID || !x.CustomAttributeTypeID) {
                    // if route is reloaded from sidebar link, reset query params
                    if (this.formGroup?.controls.GeographyID.value && this.formGroup?.controls.CustomAttributeTypeID.value) {
                        this.setRouteQueryParams(this.formGroup.controls.GeographyID.value, this.formGroup.controls.CustomAttributeTypeID.value);
                    }
                    return [];
                }
                return this.customAttributeService.geographiesGeographyIDCustomAttributesCustomAttributeTypeIDGet(x.GeographyID, x.CustomAttributeTypeID);
            }),
            tap((customAttributes) => {
                this.customAttributes = customAttributes;
                this.customAttributesOnLoad = JSON.stringify(customAttributes);
                this.isLoading = false;
            })
        );
    }

    private setRouteQueryParams(geographyID: number, customAttributeTypeID: number) {
        this.router.navigate([], { relativeTo: this.route, queryParams: { GeographyID: geographyID, CustomAttributeTypeID: customAttributeTypeID } });
    }

    public save() {
        this.alertService.clearAlerts();
        this.isLoadingSubmit = true;

        const geographyID = this.formGroup.controls.GeographyID.value;
        const customAttributeTypeID = this.formGroup.controls.CustomAttributeTypeID.value;
        this.customAttributeService.geographiesGeographyIDCustomAttributesCustomAttributeTypeIDPut(geographyID, customAttributeTypeID, this.customAttributes).subscribe({
            next: () => {
                this.isLoadingSubmit = false;
                this.customAttributesOnLoad = JSON.stringify(this.customAttributes);

                this.alertService.pushAlert(new Alert("Custom attributes successfully saved!", AlertContext.Success));
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    public canExit(): boolean {
        return JSON.stringify(this.customAttributes) == this.customAttributesOnLoad;
    }
}

export interface CustomAttributePageControlsForm {
    GeographyID?: FormControl<number>;
    CustomAttributeTypeID?: FormControl<number>;
}
