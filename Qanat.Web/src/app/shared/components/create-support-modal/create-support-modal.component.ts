import { Component, ComponentRef, OnInit } from "@angular/core";
import { SupportTicketService } from "../../generated/api/support-ticket.service";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FormFieldType, FormFieldComponent } from "../forms/form-field/form-field.component";
import { ModalComponent } from "../modal/modal.component";
import { WaterAccountContext } from "../water-account/modals/update-water-account-info/update-water-account-info.component";
import { SupportTicketUpsertDtoForm, SupportTicketUpsertDtoFormControls } from "../../generated/model/support-ticket-upsert-dto";
import { AlertService } from "../../services/alert.service";
import { ModalService } from "../../services/modal/modal.service";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { Observable, map, tap } from "rxjs";
import { SelectDropdownOption } from "../inputs/select-dropdown/select-dropdown.component";
import { AsyncPipe, NgIf } from "@angular/common";
import { UserService } from "../../generated/api/user.service";
import { GeographyService } from "../../generated/api/geography.service";
import { UserDto } from "../../generated/model/user-dto";
import { GeographyRoleEnum } from "../../generated/enum/geography-role-enum";
import { SearchWaterAccountsComponent } from "../search-water-accounts/search-water-accounts.component";
import { AuthorizationHelper } from "../../helpers/authorization-helper";
import { SupportTicketPrioritiesAsSelectDropdownOptions } from "../../generated/enum/support-ticket-priority-enum";
import { SupportTicketQuestionTypesAsSelectDropdownOptions } from "../../generated/enum/support-ticket-question-type-enum";

@Component({
    selector: "create-support-modal",
    standalone: true,
    imports: [FormFieldComponent, ReactiveFormsModule, AsyncPipe, NgIf, SearchWaterAccountsComponent],
    templateUrl: "./create-support-modal.component.html",
    styleUrl: "./create-support-modal.component.scss",
})
export class CreateSupportModalComponent implements OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: WaterAccountContext;

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public SupportTicketPrioritiesSelectDropdownOptions = SupportTicketPrioritiesAsSelectDropdownOptions;
    public geographyOptions$: Observable<SelectDropdownOption[]>;
    public users$: Observable<UserDto[]>;
    public userOptions = {};

    public SupportTicketQuestionTypesSelectDropdownOptions = SupportTicketQuestionTypesAsSelectDropdownOptions;

    public formGroup = new FormGroup<SupportTicketUpsertDtoForm>({
        WaterAccount: SupportTicketUpsertDtoFormControls.WaterAccount(),
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
        private modalService: ModalService,
        private alertService: AlertService,
        private supportTicketService: SupportTicketService,
        private geographyService: GeographyService,
        private userService: UserService,
    ) {}

    ngOnInit(): void {
        this.geographyOptions$ = this.geographyService.geographiesCurrentUserGet().pipe(
            map((geographies) => {
                this.formGroup.controls.GeographyID.setValue(geographies[0].GeographyID);
                let options = geographies.map((x) => ({ Value: x.GeographyID, Label: x.GeographyName } as SelectDropdownOption));
                options = [{ Value: null, Label: "- Select -", Disabled: true }, ...options]; // insert an empty option at the front
                return options;
            }),
            tap((geographies) => {
                // todo: add an endpoint to get admin & managers for selected geography
                this.users$ = this.userService.usersGet().pipe(
                    tap((users) => {
                        geographies.forEach((geography) => {
                            this.userOptions[geography.Value] = [];
                            this.userOptions[geography.Value] = users
                                .filter((x) => AuthorizationHelper.isSystemAdministrator(x))
                                .map((x) => ({ Value: x.UserID, Label: x.FullName } as SelectDropdownOption));
                            const geographyUsers = users
                                .map((x) => x.GeographyUser.filter((y) => y.GeographyID == geography.Value && y.GeographyRoleID == GeographyRoleEnum.WaterManager))
                                .filter((x) => x.length > 0);
                            geographyUsers.forEach((user) => {
                                const entry = users.find((x) => x.UserID == user[0].UserID);
                                this.userOptions[geography.Value] = [{ Value: entry.UserID, Label: entry.FullName } as SelectDropdownOption, ...this.userOptions[geography.Value]];
                            });
                            this.userOptions[geography.Value] = [{ Value: null, Label: "Unassigned" } as SelectDropdownOption, ...this.userOptions[geography.Value]];
                        });
                        this.userOptions[-1] = [{ Value: null, Label: "- Select a geography -", Disabled: true }];
                    })
                );
            })
        );
    }

    close() {
        this.modalService.close(this.modalComponentRef, null);
    }

    save() {
        this.alertService.clearAlerts();
        this.supportTicketService.supportTicketsCreatePost(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Created new support ticket successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to create a new support ticket.", AlertContext.Danger));
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }
}
