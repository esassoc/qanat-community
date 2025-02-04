import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { inOutAnimation } from "src/app/shared/animations/in-out.animation";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import {
    WaterUseTypesUsed,
    WellRegistrationBasicInfoFormDto,
    WellRegistrationBasicInfoFormDtoFormControls,
} from "src/app/shared/generated/model/models";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";
import { WellRegistrationWaterUseTypes } from "src/app/shared/generated/enum/well-registration-water-use-type-enum";

@Component({
    selector: "basic-well-info",
    templateUrl: "./basic-well-info.component.html",
    styleUrls: ["./basic-well-info.component.scss"],
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
        NgFor,
        ButtonComponent,
        AsyncPipe,
    ],
})
export class BasicWellInfoComponent implements OnInit, OnDestroy, IDeactivateComponent {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryBasicInformation;
    public formAsteriskExplanationID = CustomRichTextTypeEnum.FormAsteriskExplanation;
    public CustomRichTextTypeEnum = CustomRichTextTypeEnum;

    public isLoadingSubmit: boolean = true;
    public wellRegistrationID: number;
    public FormFieldType = FormFieldType;
    public basicWellInfo$: Observable<WellRegistrationBasicInfoFormDto>;
    public WellRegistrationWaterUseTypes = WellRegistrationWaterUseTypes;

    public formGroup: FormGroup<{
        WellName: FormControl<string>;
        StateWellNumber: FormControl<string>;
        StateWellCompletionNumber: FormControl<string>;
        CountyWellPermit: FormControl<string>;
        DateDrilled: FormControl<Date>;
        WaterUseTypes: FormArray<FormGroup<{ Checked: FormControl<boolean>; Description: FormControl }>>; // Replace `any` with the specific type if available
    }> = new FormGroup<any>({
        WellName: WellRegistrationBasicInfoFormDtoFormControls.WellName(),
        StateWellNumber: WellRegistrationBasicInfoFormDtoFormControls.StateWellNumber(),
        StateWellCompletionNumber: WellRegistrationBasicInfoFormDtoFormControls.StateWellCompletionNumber(),
        CountyWellPermit: WellRegistrationBasicInfoFormDtoFormControls.CountyWellPermit(),
        DateDrilled: WellRegistrationBasicInfoFormDtoFormControls.DateDrilled(),
        WaterUseTypes: this.formBuilder.array<FormGroup<any>>([]),
    });

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
        this.wellRegistrationID = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID)
            ? parseInt(this.route.snapshot.paramMap.get(routeParams.wellRegistrationID))
            : null;
        this.basicWellInfo$ = this.wellRegistrationService.wellRegistrationsWellRegistrationIDBasicInfoGet(this.wellRegistrationID).pipe(
            tap((x) => {
                this.formGroup.patchValue(x as any);

                if (!x.StateWellCompletionNumber && x.ReferenceWell?.StateWCRNumber) {
                    this.formGroup.controls.StateWellCompletionNumber.patchValue(x.ReferenceWell?.StateWCRNumber);
                }
                if (!x.CountyWellPermit && x.ReferenceWell?.CountyWellPermitNo) {
                    this.formGroup.controls.CountyWellPermit.patchValue(x.ReferenceWell.CountyWellPermitNo);
                }

                const waterUseTypes = x.WaterUseTypes.map((useType) => this.createWaterUseType(useType));
                this.formGroup.setControl("WaterUseTypes", this.formBuilder.array(waterUseTypes));
                this.isLoadingSubmit = false;
            })
        );
    }

    createWaterUseType(useType: WaterUseTypesUsed): FormGroup {
        return this.formBuilder.group<WaterUseTypesUsed>({
            WaterUseTypeID: useType.WaterUseTypeID,
            Checked: useType.Checked,
            Description: useType.Description,
        });
    }

    get waterUseTypes(): FormArray {
        return this.formGroup.get("WaterUseTypes") as FormArray<FormControl<WaterUseTypesUsed>>;
    }

    ngOnDestroy(): void {
        this.cdr.detach();
    }

    public save(andContinue: boolean = false) {
        this.isLoadingSubmit = true;

        const formDto = new WellRegistrationBasicInfoFormDto();
        formDto.WellName = this.formGroup.controls.WellName.value;
        formDto.StateWellNumber = this.formGroup.controls.StateWellNumber.value;
        formDto.StateWellCompletionNumber = this.formGroup.controls.StateWellCompletionNumber.value;
        formDto.CountyWellPermit = this.formGroup.controls.CountyWellPermit.value;
        formDto.DateDrilled = this.formGroup.controls.DateDrilled.value.toDateString();
        formDto.WaterUseTypes = this.formGroup.controls.WaterUseTypes.value;
        this.wellRegistrationService.wellRegistrationsWellRegistrationIDBasicInfoPut(this.wellRegistrationID, formDto).subscribe((response) => {
            this.isLoadingSubmit = false;
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Successfully saved basic information", AlertContext.Success));
            this.wellRegistryProgressService.updateProgress(this.wellRegistrationID);
            this.formGroup.patchValue(response as any);
            this.formGroup.markAsPristine();
            if (andContinue) {
                this.router.navigate(["../supporting-information"], { relativeTo: this.route });
            }
        });
    }
}
