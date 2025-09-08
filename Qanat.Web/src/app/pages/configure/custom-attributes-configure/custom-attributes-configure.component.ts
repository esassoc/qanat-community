import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { combineLatest, Observable, Subscription, switchMap, tap } from "rxjs";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { CustomAttributeService } from "src/app/shared/generated/api/custom-attribute.service";
import { CustomAttributeSimpleDto } from "src/app/shared/generated/model/custom-attribute-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ButtonLoadingDirective } from "../../../shared/directives/button-loading.directive";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { CustomAttributesEditComponent } from "../../../shared/components/custom-attributes-edit/custom-attributes-edit.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { routeParams } from "src/app/app.routes";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { CustomAttributeTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/custom-attribute-type-enum";

@Component({
    selector: "custom-attributes-configure",
    templateUrl: "./custom-attributes-configure.component.html",
    styleUrl: "./custom-attributes-configure.component.scss",
    imports: [
        PageHeaderComponent,
        FormsModule,
        ReactiveFormsModule,
        FormFieldComponent,
        AlertDisplayComponent,
        CustomAttributesEditComponent,
        LoadingDirective,
        ButtonLoadingDirective,
        AsyncPipe,
    ]
})
export class CustomAttributesConfigureComponent implements OnInit, OnDestroy {
    public initialData$: Observable<CustomAttributeConfigureInitialData>;
    public geographyID: number;
    public isLoading: boolean;

    public customAttributeTypeDropdownOptions = CustomAttributeTypesAsSelectDropdownOptions;

    public customAttributeData$: Observable<CustomAttributeSimpleDto[]>;
    public customAttributes: CustomAttributeSimpleDto[];
    public customAttributesOnLoad: string;

    public formGroup: FormGroup<CustomAttributePageControlsForm> = new FormGroup<CustomAttributePageControlsForm>({
        CustomAttributeTypeID: new FormControl(1),
    });

    public FormFieldType = FormFieldType;
    public isLoadingSubmit: boolean = false;
    public isReadonly: boolean = false;

    private formSubscription: Subscription;

    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService,
        private customAttributeService: CustomAttributeService,
        private alertService: AlertService
    ) {}
    public ngOnInit(): void {
        this.initialData$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return combineLatest({
                    currentUser: this.authenticationService.getCurrentUser(),
                    geography: this.geographyService.getByNameAsMinimalDtoGeography(geographyName),
                });
            }),
            tap(({ currentUser, geography }) => {
                this.isReadonly = !this.authenticationService.hasPermission(currentUser, PermissionEnum.GeographyRights, RightsEnum.Update);

                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;

                const parsedCustomAttributeTypeID = parseInt(this.route.snapshot.queryParams.CustomAttributeTypeID);
                const customAttributeTypeID = isNaN(parsedCustomAttributeTypeID) ? this.customAttributeTypeDropdownOptions[0].Value : parsedCustomAttributeTypeID;
                this.formGroup.controls.CustomAttributeTypeID.setValue(customAttributeTypeID);

                this.isLoading = false;
            })
        );

        this.customAttributeData$ = combineLatest([this.initialData$, this.route.queryParams]).pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap(([{ geography }, queryParams]) => {
                const customAttributeTypeID = queryParams.CustomAttributeTypeID as number;
                return this.customAttributeService.listCustomAttributesForGeographyCustomAttribute(geography.GeographyID, customAttributeTypeID);
            }),
            tap((customAttributes) => {
                this.customAttributes = customAttributes;
                this.customAttributesOnLoad = JSON.stringify(customAttributes);
                this.isLoading = false;
            })
        );

        // update route query params when form group values change
        this.formSubscription = this.formGroup.valueChanges.subscribe((x) => {
            this.setRouteQueryParams(x.CustomAttributeTypeID);
        });
    }

    public ngOnDestroy(): void {
        if (this.formSubscription && this.formSubscription.unsubscribe) {
            this.formSubscription.unsubscribe();
        }
    }

    private setRouteQueryParams(customAttributeTypeID: number) {
        this.router.navigate([], { relativeTo: this.route, queryParams: { CustomAttributeTypeID: customAttributeTypeID } });
    }

    public save() {
        this.alertService.clearAlerts();
        this.isLoadingSubmit = true;
        const customAttributeTypeID = this.formGroup.controls.CustomAttributeTypeID.value;
        this.customAttributeService.mergeCustomAttributesCustomAttribute(this.geographyID, customAttributeTypeID, this.customAttributes).subscribe({
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

class CustomAttributeConfigureInitialData {
    public currentUser: UserDto;
    public geography: GeographyMinimalDto;
}

export interface CustomAttributePageControlsForm {
    CustomAttributeTypeID?: FormControl<number>;
}
