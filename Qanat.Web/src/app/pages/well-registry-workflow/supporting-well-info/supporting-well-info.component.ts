import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { FuelTypeEnum } from "src/app/shared/generated/enum/fuel-type-enum";
import {
    FuelTypeSimpleDto,
    WellRegistrySupportingInfoDto,
    WellRegistrySupportingInfoDtoForm,
    WellRegistrySupportingInfoDtoFormControls,
} from "src/app/shared/generated/model/models";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "supporting-well-info",
    templateUrl: "./supporting-well-info.component.html",
    styleUrls: ["./supporting-well-info.component.scss"],
    animations: [inOutAnimation],
    standalone: true,
    imports: [
        PageHeaderComponent,
        WorkflowBodyComponent,
        AlertDisplayComponent,
        CustomRichTextComponent,
        NgIf,
        FormsModule,
        ReactiveFormsModule,
        FormFieldComponent,
        ButtonComponent,
        AsyncPipe,
    ],
})
export class SupportingWellInfoComponent implements OnInit, OnDestroy, IDeactivateComponent {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistrySupportingInformation;
    public formAsteriskExplanationID = CustomRichTextTypeEnum.FormAsteriskExplanation;

    public isLoadingSubmit: boolean = false;
    public wellID;
    public FormFieldType = FormFieldType;
    public supportingWellInfo$: Observable<WellRegistrySupportingInfoDto>;
    public fuelTypes$: Observable<FuelTypeSimpleDto[]>;

    public formGroup: FormGroup<WellRegistrySupportingInfoDtoForm> = new FormGroup<WellRegistrySupportingInfoDtoForm>({
        WellDepth: WellRegistrySupportingInfoDtoFormControls.WellDepth(),
        CasingDiameter: WellRegistrySupportingInfoDtoFormControls.CasingDiameter(),
        TopOfPerforations: WellRegistrySupportingInfoDtoFormControls.TopOfPerforations(),
        BottomOfPerforations: WellRegistrySupportingInfoDtoFormControls.BottomOfPerforations(),
        SerialNumberOfWaterMeter: WellRegistrySupportingInfoDtoFormControls.SerialNumberOfWaterMeter(),
        ManufacturerOfWaterMeter: WellRegistrySupportingInfoDtoFormControls.ManufacturerOfWaterMeter(),
        ElectricMeterNumber: WellRegistrySupportingInfoDtoFormControls.ElectricMeterNumber(),

        PumpDischargeDiameter: WellRegistrySupportingInfoDtoFormControls.PumpDischargeDiameter(),
        MotorHorsePower: WellRegistrySupportingInfoDtoFormControls.MotorHorsePower(),

        FuelTypeID: WellRegistrySupportingInfoDtoFormControls.FuelTypeID(),
        FuelOther: WellRegistrySupportingInfoDtoFormControls.FuelOther(),

        MaximumFlow: WellRegistrySupportingInfoDtoFormControls.MaximumFlow(),
        IsEstimatedMax: WellRegistrySupportingInfoDtoFormControls.IsEstimatedMax(),
        TypicalPumpFlow: WellRegistrySupportingInfoDtoFormControls.TypicalPumpFlow(),
        IsEstimatedTypical: WellRegistrySupportingInfoDtoFormControls.IsEstimatedTypical(),

        PumpTestBy: WellRegistrySupportingInfoDtoFormControls.PumpTestBy(),
        PumpTestDatePerformed: WellRegistrySupportingInfoDtoFormControls.PumpTestDatePerformed(),
        PumpManufacturer: WellRegistrySupportingInfoDtoFormControls.PumpManufacturer(),
        PumpYield: WellRegistrySupportingInfoDtoFormControls.PumpYield(),
        PumpStaticLevel: WellRegistrySupportingInfoDtoFormControls.PumpStaticLevel(),
        PumpingLevel: WellRegistrySupportingInfoDtoFormControls.PumpingLevel(),
    });

    public fuelTypeOptions: FormInputOption[] = [];

    public isEstimatedOptions: FormInputOption[] = [
        { Value: false, Label: "Measured", Disabled: false },
        { Value: true, Label: "Estimated", Disabled: false },
    ];

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private formBuilder: FormBuilder,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private alertService: AlertService
    ) {}

    canExit() {
        return !this.formGroup.dirty;
    }

    ngOnInit(): void {
        this.wellID = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID) ? parseInt(this.route.snapshot.paramMap.get(routeParams.wellRegistrationID)) : null;

        this.fuelTypes$ = this.wellRegistrationService.wellRegistrationsPumpFuelTypesGet().pipe(
            tap((x) => {
                this.fuelTypeOptions = x.map((y) => ({ Value: y.FuelTypeID, Disabled: false, Label: y.FuelTypeDisplayName }) as FormInputOption);
            })
        );

        this.formGroup.controls.FuelTypeID.valueChanges.subscribe((x) => {
            if (x != FuelTypeEnum.Other) {
                this.formGroup.controls.FuelOther.disable();
            } else {
                this.formGroup.controls.FuelOther.enable();
            }
        });
        this.supportingWellInfo$ = this.wellRegistrationService.wellRegistrationsWellRegistrationIDSupportingInfoGet(this.wellID).pipe(
            tap((x) => {
                this.formGroup.patchValue(x);
            })
        );
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public save(andContinue: boolean = false) {
        this.isLoadingSubmit = true;
        this.wellRegistrationService.wellRegistrationsWellRegistrationIDSupportingInfoPut(this.wellID, this.formGroup.value).subscribe((response) => {
            this.isLoadingSubmit = false;
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Successfully saved supporting information", AlertContext.Success));
            this.wellRegistryProgressService.updateProgress(this.wellID);
            this.formGroup.patchValue(response);
            this.formGroup.markAsPristine();
            if (andContinue) {
                this.router.navigate(["../attachments"], { relativeTo: this.route });
            }
        });
    }
}
