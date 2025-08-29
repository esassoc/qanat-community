import { Component, inject, OnInit } from "@angular/core";
import { SupportTicketService } from "../../../../generated/api/support-ticket.service";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../../../forms/form-field/form-field.component";
import { WaterAccountContext } from "../../../water-account/modals/update-water-account-info/update-water-account-info.component";
import { SupportTicketUpsertDtoForm, SupportTicketUpsertDtoFormControls } from "../../../../generated/model/support-ticket-upsert-dto";
import { AlertService } from "../../../../services/alert.service";
import { Alert } from "../../../../models/alert";
import { AlertContext } from "../../../../models/enums/alert-context.enum";
import { Observable, map, tap } from "rxjs";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { UserService } from "../../../../generated/api/user.service";
import { GeographyService } from "../../../../generated/api/geography.service";
import { UserDto } from "../../../../generated/model/user-dto";
import { GeographyRoleEnum } from "../../../../generated/enum/geography-role-enum";
import { AuthorizationHelper } from "../../../../helpers/authorization-helper";
import { SupportTicketPrioritiesAsSelectDropdownOptions } from "../../../../generated/enum/support-ticket-priority-enum";
import { SupportTicketQuestionTypesAsSelectDropdownOptions } from "../../../../generated/enum/support-ticket-question-type-enum";
import { DialogRef } from "@ngneat/dialog";
import { FlagEnum } from "../../../../generated/enum/flag-enum";

@Component({
    selector: "create-support-modal",
    imports: [FormFieldComponent, ReactiveFormsModule, AsyncPipe],
    templateUrl: "./create-support-modal.component.html",
    styleUrl: "./create-support-modal.component.scss",
})
export class CreateSupportModalComponent implements OnInit {
    public ref: DialogRef<WaterAccountContext, boolean> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public SupportTicketPrioritiesSelectDropdownOptions = SupportTicketPrioritiesAsSelectDropdownOptions;
    public geographyOptions$: Observable<SelectDropdownOption[]>;
    public users$: Observable<UserDto[]>;
    public userOptions = {};

    public SupportTicketQuestionTypesSelectDropdownOptions = SupportTicketQuestionTypesAsSelectDropdownOptions;

    public formGroup = new FormGroup<SupportTicketUpsertDtoForm>({
        WaterAccountID: SupportTicketUpsertDtoFormControls.WaterAccountID(),
        Description: SupportTicketUpsertDtoFormControls.Description(),
        SupportTicketPriorityID: SupportTicketUpsertDtoFormControls.SupportTicketPriorityID(),
        GeographyID: SupportTicketUpsertDtoFormControls.GeographyID(),
        ContactFirstName: SupportTicketUpsertDtoFormControls.ContactFirstName(),
        ContactLastName: SupportTicketUpsertDtoFormControls.ContactLastName(),
        ContactEmail: SupportTicketUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: SupportTicketUpsertDtoFormControls.ContactPhoneNumber(),
        AssignedUserID: SupportTicketUpsertDtoFormControls.AssignedUserID(),
        SupportTicketQuestionTypeID: SupportTicketUpsertDtoFormControls.SupportTicketQuestionTypeID(),
    });

    constructor(
        private alertService: AlertService,
        private supportTicketService: SupportTicketService,
        private geographyService: GeographyService,
        private userService: UserService
    ) {}

    ngOnInit(): void {
        this.geographyOptions$ = this.geographyService.listForCurrentUserGeography().pipe(
            map((geographies) => {
                this.formGroup.controls.GeographyID.setValue(geographies[0].GeographyID);
                let options = geographies.map(
                    (x) =>
                        ({
                            Value: x.GeographyID,
                            Label: x.GeographyName,
                        }) as SelectDropdownOption
                );
                options = [{ Value: null, Label: "- Select -", disabled: true }, ...options]; // insert an empty option at the front
                return options;
            }),
            tap((geographies) => {
                // todo: add an endpoint to get admin & managers for selected geography
                this.users$ = this.userService.listUser().pipe(
                    tap((users) => {
                        geographies
                            .filter((x) => x.Value)
                            .forEach((geography) => {
                                this.userOptions[geography.Value] = [];

                                const systemAdmins = users
                                    .filter((x) => AuthorizationHelper.isSystemAdministrator(x))
                                    .map(
                                        (x) =>
                                            ({
                                                Value: x.UserID,
                                                Label: x.FullName,
                                            }) as SelectDropdownOption
                                    );

                                const geographyUsers = users
                                    .filter((x) => AuthorizationHelper.hasGeographyFlag(geography.Value, FlagEnum.HasManagerDashboard, x))
                                    .map((user) => {
                                        return { Value: user.UserID, Label: user.FullName } as SelectDropdownOption;
                                    });

                                this.userOptions[geography.Value] = [{ Value: null, Label: "Unassigned" } as SelectDropdownOption, ...geographyUsers, ...systemAdmins];
                            });
                        this.userOptions[-1] = [{ Value: null, Label: "- Select a geography -", Disabled: true }];
                    })
                );
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.supportTicketService.createSupportTicketSupportTicket(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Created new support ticket successfully.", AlertContext.Success));
                this.ref.close(true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new support ticket.", AlertContext.Danger));
                this.ref.close(null);
            }
        );
    }
}
