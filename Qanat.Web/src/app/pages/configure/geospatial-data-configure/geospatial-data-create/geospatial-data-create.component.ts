import { Component, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, combineLatest, of, switchMap, tap } from "rxjs";
import { FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ExternalMapLayerTypeEnum, ExternalMapLayerTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/external-map-layer-type-enum";
import { ExternalMapLayerSimpleDto, ExternalMapLayerSimpleDtoForm, ExternalMapLayerSimpleDtoFormControls } from "src/app/shared/generated/model/external-map-layer-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { FormFieldComponent } from "../../../../shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "geospatial-data-create",
    templateUrl: "./geospatial-data-create.component.html",
    styleUrls: ["./geospatial-data-create.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, FormsModule, ReactiveFormsModule, FormFieldComponent, RouterLink, AsyncPipe],
})
export class GeospatialDataCreateComponent implements OnInit {
    public geography$: Observable<GeographyMinimalDto>;
    public externalMapLayer$: Observable<ExternalMapLayerSimpleDto>;
    public ExternalMapLayerTypesSelectDropdownOptions = ExternalMapLayerTypesAsSelectDropdownOptions;
    public isCreating: boolean = true;

    public isLoadingSubmit: boolean;
    public richTextTypeID: number = CustomRichTextTypeEnum.ExternalMapLayers;
    public layerIsOnByDefaultOptions: FormInputOption[] = [
        { Value: true, Label: "Yes", disabled: false },
        { Value: false, Label: "No", disabled: false },
    ];
    public isActiveOptions: FormInputOption[] = [
        { Value: true, Label: "Yes", disabled: false },
        { Value: false, Label: "No", disabled: false },
    ];
    public esriFeatureServerID = ExternalMapLayerTypeEnum.ESRIFeatureServer;

    public formGroup: FormGroup<ExternalMapLayerSimpleDtoForm> = new FormGroup<ExternalMapLayerSimpleDtoForm>({
        ExternalMapLayerID: ExternalMapLayerSimpleDtoFormControls.ExternalMapLayerID(),
        GeographyID: ExternalMapLayerSimpleDtoFormControls.GeographyID(),
        ExternalMapLayerDisplayName: ExternalMapLayerSimpleDtoFormControls.ExternalMapLayerDisplayName(),
        ExternalMapLayerTypeID: ExternalMapLayerSimpleDtoFormControls.ExternalMapLayerTypeID(),
        ExternalMapLayerURL: ExternalMapLayerSimpleDtoFormControls.ExternalMapLayerURL(),
        LayerIsOnByDefault: ExternalMapLayerSimpleDtoFormControls.LayerIsOnByDefault(),
        IsActive: ExternalMapLayerSimpleDtoFormControls.IsActive(),
        ExternalMapLayerDescription: ExternalMapLayerSimpleDtoFormControls.ExternalMapLayerDescription(),
        PopUpField: ExternalMapLayerSimpleDtoFormControls.PopUpField(),
        MinZoom: ExternalMapLayerSimpleDtoFormControls.MinZoom(),
    });

    public FormFieldType = FormFieldType;

    constructor(
        private route: ActivatedRoute,
        private externalMapLayerService: ExternalMapLayerService,
        private router: Router,
        private alertService: AlertService,
        private currentGeographyService: CurrentGeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.externalMapLayer$ = combineLatest({ geography: this.geography$, routeParams: this.route.params }).pipe(
            switchMap(({ geography, routeParams }) => {
                const isCreating = routeParams.create;

                if (isCreating || !routeParams.externalMapLayerID) {
                    return of(
                        new ExternalMapLayerSimpleDto({
                            IsActive: true,
                            LayerIsOnByDefault: true,
                            GeographyID: geography.GeographyID,
                            ExternalMapLayerID: 0,
                        })
                    );
                }

                this.isCreating = false;
                const externalMapLayerID = parseInt(routeParams.externalMapLayerID);
                return this.externalMapLayerService.getExternalMapLayerByIDExternalMapLayer(geography.GeographyID, externalMapLayerID);
            }),
            tap((dto) => {
                this.formGroup.patchValue(dto);
                this.isLoadingSubmit = false;
            })
        );
    }

    onSubmit(geography: GeographyMinimalDto): void {
        this.isLoadingSubmit = true;

        const submitRequest = this.isCreating
            ? this.externalMapLayerService.addExternalMapLayer(geography.GeographyID, this.formGroup.value)
            : this.externalMapLayerService.updateExternalMapLayer(geography.GeographyID, this.formGroup.value);

        const successMessage = this.isCreating ? "Successfully added new Geospatial Data Layer" : "Successfully updated Geospatial Data Layer";

        const returnPath = this.isCreating ? ["../"] : ["../../"];

        submitRequest.subscribe({
            next: (response) => {
                this.router.navigate(returnPath, { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success));
                });
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
