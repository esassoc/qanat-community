import { ChangeDetectorRef, Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { forkJoin } from "rxjs";
import { StateSimpleDto, WellRegistrationContactsUpsertDto } from "src/app/shared/generated/model/models";
import { WellRegistryWorkflowProgressService } from "src/app/shared/services/well-registry-workflow-progress.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { IDeactivateComponent } from "src/app/guards/unsaved-changes-guard";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WellRegistrationService } from "src/app/shared/generated/api/well-registration.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { FormsModule } from "@angular/forms";

import { CustomRichTextComponent } from "../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";
import { NgxMaskDirective, provideNgxMask } from "ngx-mask";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { NgSelectModule } from "@ng-select/ng-select";

@Component({
    selector: "well-contacts",
    templateUrl: "./well-contacts.component.html",
    styleUrls: ["./well-contacts.component.scss"],
    imports: [PageHeaderComponent, WorkflowBodyComponent, AlertDisplayComponent, CustomRichTextComponent, FormsModule, NgxMaskDirective, ButtonComponent, NgSelectModule],
    providers: [provideNgxMask()]
})
export class WellContactsComponent implements OnInit, IDeactivateComponent, OnDestroy {
    public customRichTextTypeID = CustomRichTextTypeEnum.WellRegistryContacts;
    public formAsteriskExplanationID = CustomRichTextTypeEnum.FormAsteriskExplanation;

    public isLoadingSubmit: boolean = false;
    public wellID: number;
    public model: WellRegistrationContactsUpsertDto;
    public states: StateSimpleDto[];
    public originalModel: string;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private cdr: ChangeDetectorRef,
        private wellRegistrationService: WellRegistrationService,
        private publicService: PublicService,
        private wellRegistryProgressService: WellRegistryWorkflowProgressService,
        private alertService: AlertService
    ) {}

    canExit() {
        return this.originalModel === JSON.stringify(this.model);
    }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get(routeParams.wellRegistrationID);
        if (id) {
            this.wellID = parseInt(id);
            forkJoin({
                wellRegistrationContacts: this.wellRegistrationService.getWellRegistrationContactsUpsertDtoWellRegistration(this.wellID),
                states: this.publicService.statesListPublic(),
            }).subscribe(({ wellRegistrationContacts, states }) => {
                this.model = wellRegistrationContacts;
                this.originalModel = JSON.stringify(wellRegistrationContacts);
                this.states = states;
            });
        }

        this.cdr.detectChanges();
    }

    updateLandownerSameAsOwnerOperator() {
        this.model.LandownerSameAsOwnerOperator = !this.model.LandownerSameAsOwnerOperator;
    }

    getLandownerSameAsOwnerOperator() {
        return this.model.LandownerSameAsOwnerOperator;
    }

    ngOnDestroy() {
        this.cdr.detach();
    }

    public save(andContinue: boolean = false): void {
        this.isLoadingSubmit = true;
        if (!this.model.LandownerSameAsOwnerOperator) {
            this.model.OwnerOperatorContactName = this.model.LandownerContactName;
            this.model.OwnerOperatorBusinessName = this.model.LandownerBusinessName;
            this.model.OwnerOperatorStreetAddress = this.model.LandownerStreetAddress;
            this.model.OwnerOperatorCity = this.model.LandownerCity;
            this.model.OwnerOperatorStateID = this.model.LandownerStateID;
            this.model.OwnerOperatorZipCode = this.model.LandownerZipCode;
            this.model.OwnerOperatorPhone = this.model.LandownerPhone;
            this.model.OwnerOperatorEmail = this.model.LandownerEmail;
        }

        this.wellRegistrationService.addWellRegistrationContactWellRegistration(this.wellID, this.model).subscribe({
            next: () => {
                this.originalModel = JSON.stringify(this.model);
                this.isLoadingSubmit = false;
                this.wellRegistryProgressService.updateProgress(this.wellID);
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Successfully saved Well Contacts", AlertContext.Success));
                if (andContinue) {
                    this.router.navigate(["../basic-information"], { relativeTo: this.route });
                }
            },
            error: () => {
                this.isLoadingSubmit = false;
            },
        });
    }
}
