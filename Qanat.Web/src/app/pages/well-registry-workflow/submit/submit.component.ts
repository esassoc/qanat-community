import { Component, OnInit } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, tap } from "rxjs";
import { routeParams } from "src/app/app.routes";
import { FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { WellRegistryWorkflowProgressDto } from "src/app/shared/generated/model/well-registry-workflow-progress-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { NoteComponent } from "../../../shared/components/note/note.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { FormFieldComponent } from "../../../shared/components/forms/form-field/form-field.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";

@Component({
    selector: "submit",
    templateUrl: "./submit.component.html",
    styleUrls: ["./submit.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        WorkflowBodyComponent,
        AlertDisplayComponent,
        FormsModule,
        FormFieldComponent,
        ReactiveFormsModule,
        NgIf,
        NoteComponent,
        CustomRichTextComponent,
        ButtonComponent,
        AsyncPipe,
    ],
})
export class SubmitComponent implements OnInit {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistrySubmit;
    public incompleteCustomRichTextTypeID = CustomRichTextTypeEnum.WellRegistryIncompleteText;
    public isLoadingSubmit: boolean = false;
    public FormFieldType = FormFieldType;
    public registryProgress$: Observable<WellRegistryWorkflowProgressDto>;
    public formControl = new FormControl<boolean>(false);
    public wellID: number;
    public showIncompleteNote: boolean = false;

    constructor(
        private wellRegistrationService: WellRegistrationService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private router: Router,
        private route: ActivatedRoute,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.wellID = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID) ? parseInt(this.route.snapshot.paramMap.get(routeParams.wellRegistrationID)) : null;
        this.registryProgress$ = this.wellRegistryProgressService.progressObservable$.pipe(
            tap((progress) => {
                const everyStepComplete = Object.values(progress.Steps).every((complete) => complete);
                this.showIncompleteNote = !everyStepComplete;
            })
        );
    }

    submitWell(): void {
        this.isLoadingSubmit = true;
        this.wellRegistrationService.wellRegistrationsWellRegistrationIDSubmitWellPost(this.wellID).subscribe(() => {
            this.isLoadingSubmit = false;
            this.router.navigate(["profile"]).then(() => {
                this.alertService.pushAlert(new Alert("Well registration successfully submitted!", AlertContext.Success));
            });
        });
    }
}
