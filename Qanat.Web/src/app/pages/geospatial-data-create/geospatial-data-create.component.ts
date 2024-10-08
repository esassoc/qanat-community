import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Observable, Subscription, map, of, switchMap, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ExternalMapLayerTypeService } from "src/app/shared/generated/api/external-map-layer-type.service";
import { ExternalMapLayerService } from "src/app/shared/generated/api/external-map-layer.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ExternalMapLayerTypeEnum } from "src/app/shared/generated/enum/external-map-layer-type-enum";
import { ExternalMapLayerSimpleDto, ExternalMapLayerSimpleDtoForm, ExternalMapLayerSimpleDtoFormControls } from "src/app/shared/generated/model/external-map-layer-simple-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { FormFieldComponent } from "../../shared/components/forms/form-field/form-field.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { AlertDisplayComponent } from "../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "geospatial-data-create",
    templateUrl: "./geospatial-data-create.component.html",
    styleUrls: ["./geospatial-data-create.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, AlertDisplayComponent, NgIf, FormsModule, ReactiveFormsModule, FormFieldComponent, RouterLink, AsyncPipe],
})
export class GeospatialDataCreateComponent implements OnInit, OnDestroy {
    public selectedGeography$ = Subscription.EMPTY;

    private geographyID: number;
    public isLoadingSubmit: boolean;
    public externalMapLayer$: Observable<ExternalMapLayerSimpleDto>;

    public externalMapLayerTypes$: Observable<FormInputOption[]>;
    public richTextTypeID: number = CustomRichTextTypeEnum.ExternalMapLayers;
    public layerIsOnByDefaultOptions: FormInputOption[] = [
        { Value: true, Label: "Yes", Disabled: false },
        { Value: false, Label: "No", Disabled: false },
    ];
    public isActiveOptions: FormInputOption[] = [
        { Value: true, Label: "Yes", Disabled: false },
        { Value: false, Label: "No", Disabled: false },
    ];
    public isCreating: boolean;
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
        private externalMapLayerTypeService: ExternalMapLayerTypeService,
        private router: Router,
        private alertService: AlertService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID();
        });
    }

    getDataForGeographyID() {
        this.isLoadingSubmit = true;
        this.externalMapLayerTypes$ = this.externalMapLayerTypeService.externalMapLayerTypesGet().pipe(
            map((types) => {
                return types.map((x) => ({ Value: x.ExternalMapLayerTypeID, Label: x.ExternalMapLayerTypeDisplayName }) as FormInputOption);
            })
        );

        this.externalMapLayer$ = this.route.data.pipe(
            switchMap((routeData) => {
                this.isCreating = routeData.create;

                if (this.isCreating) {
                    return of(
                        new ExternalMapLayerSimpleDto({
                            IsActive: true,
                            LayerIsOnByDefault: true,
                            GeographyID: this.geographyID,
                            ExternalMapLayerID: 0,
                        })
                    );
                }

                const id = parseInt(this.route.snapshot.paramMap.get(routeParams.externalMapLayerID));
                return this.externalMapLayerService.geographiesGeographyIDExternalMapLayerExternalMapLayerIDGet(this.geographyID, id);
            }),
            tap((dto) => {
                this.formGroup.patchValue(dto);
                this.isLoadingSubmit = false;
            })
        );
    }

    onSubmit() {
        this.isLoadingSubmit = true;

        const submitRequest = this.isCreating
            ? this.externalMapLayerService.geographiesGeographyIDExternalMapLayersPost(this.geographyID, this.formGroup.value)
            : this.externalMapLayerService.geographiesGeographyIDExternalMapLayersPut(this.geographyID, this.formGroup.value);

        const successMessage = this.isCreating ? "Successfully added new Geospatial Data Layer" : "Successfully updated Geospatial Data Layer";

        const returnPath = this.isCreating ? ["../"] : ["../../"];

        submitRequest.subscribe({
            next: (response) => {
                this.router.navigate(returnPath, { relativeTo: this.route }).then((x) => {
                    this.alertService.pushAlert(new Alert(successMessage, AlertContext.Success));
                });
            },
            error: (e) => {
                this.isLoadingSubmit = false;
                console.error(e);
            },
        });
    }
}
