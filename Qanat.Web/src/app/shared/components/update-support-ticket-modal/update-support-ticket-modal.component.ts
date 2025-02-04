import { AsyncPipe, NgIf } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormGroup } from "@angular/forms";
import { Observable, map, tap } from "rxjs";
import { FormFieldComponent, FormFieldType } from "../forms/form-field/form-field.component";
import { SelectDropdownOption } from "../inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "../modal/modal.component";
import { SupportTicketService } from "../../generated/api/support-ticket.service";
import { UserService } from "../../generated/api/user.service";
import { SupportTicketUpsertDtoForm, SupportTicketUpsertDtoFormControls } from "../../generated/model/support-ticket-upsert-dto";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { AlertService } from "../../services/alert.service";
import { ModalService } from "../../services/modal/modal.service";
import { GeographyService } from "../../generated/api/geography.service";
import { SupportTicketContext } from "../../../pages/support-ticket-list/support-ticket-list.component";
import { WaterAccountService } from "../../generated/api/water-account.service";
import { WaterAccountMinimalDto } from "../../generated/model/water-account-minimal-dto";
import { GeographyRoleEnum } from "../../generated/enum/geography-role-enum";
import { UserDto } from "../../generated/model/user-dto";
import { SearchWaterAccountsComponent } from "../search-water-accounts/search-water-accounts.component";
import { WaterAccountSimpleDto } from "../../generated/model/water-account-simple-dto";
import { AuthorizationHelper } from "../../helpers/authorization-helper";
import { SupportTicketPrioritiesAsSelectDropdownOptions } from "../../generated/enum/support-ticket-priority-enum";

@Component({
    selector: "update-support-ticket-modal",
    standalone: true,
    imports: [FormFieldComponent, ReactiveFormsModule, AsyncPipe, NgIf, SearchWaterAccountsComponent],
    templateUrl: "./update-support-ticket-modal.component.html",
    styleUrl: "./update-support-ticket-modal.component.scss",
})
export class UpdateSupportTicketModalComponent implements OnInit {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: SupportTicketContext;

    public FormFieldType = FormFieldType;
    public geographyID: number;
    public SupportTicketPrioritiesSelectDropdownOptions = SupportTicketPrioritiesAsSelectDropdownOptions;
    public geographyOptions$: Observable<SelectDropdownOption[]>;
    public waterAccounts$: Observable<WaterAccountMinimalDto>;
    public users$: Observable<UserDto[]>;
    public userOptions = {};
    public waterAccountOptions = {};

    public formGroup = new FormGroup<SupportTicketUpsertDtoForm>({
        SupportTicketID: SupportTicketUpsertDtoFormControls.SupportTicketID(),
        WaterAccount: SupportTicketUpsertDtoFormControls.WaterAccount(),
        Description: SupportTicketUpsertDtoFormControls.Description(),
        SupportTicketPriorityID: SupportTicketUpsertDtoFormControls.SupportTicketPriorityID(),
        GeographyID: SupportTicketUpsertDtoFormControls.GeographyID(),
        ContactFirstName: SupportTicketUpsertDtoFormControls.ContactFirstName(),
        ContactLastName: SupportTicketUpsertDtoFormControls.ContactLastName(),
        ContactEmail: SupportTicketUpsertDtoFormControls.ContactEmail(),
        ContactPhoneNumber: SupportTicketUpsertDtoFormControls.ContactPhoneNumber(),
        AssignedUserID: SupportTicketUpsertDtoFormControls.AssignedUserID(),
    });

    constructor(
        private modalService: ModalService,
        private alertService: AlertService,
        private supportTicketService: SupportTicketService,
        private geographyService: GeographyService,
        private waterAccountService: WaterAccountService,
        private userService: UserService
    ) {}

    ngOnInit(): void {
        this.formGroup.controls.SupportTicketID.setValue(this.modalContext.SupportTicketID);
        this.formGroup.controls.Description.setValue(this.modalContext.Description);
        this.formGroup.controls.SupportTicketPriorityID.setValue(this.modalContext.SupportTicketPriorityID);
        this.formGroup.controls.GeographyID.setValue(this.modalContext.GeographyID);
        this.formGroup.controls.ContactFirstName.setValue(this.modalContext.ContactFirstName);
        this.formGroup.controls.ContactLastName.setValue(this.modalContext.ContactLastName);
        this.formGroup.controls.ContactEmail.setValue(this.modalContext.ContactEmail);
        this.formGroup.controls.ContactPhoneNumber.setValue(this.modalContext.ContactPhoneNumber);
        this.formGroup.controls.AssignedUserID.setValue(this.modalContext.AssignedUserID);
        this.geographyOptions$ = this.geographyService.geographiesCurrentUserGet().pipe(
            map((geographies) => {
                let options = geographies.map((x) => ({ Value: x.GeographyID, Label: x.GeographyName }) as SelectDropdownOption);
                options = [{ Value: null, Label: "- Select -", Disabled: true }, ...options]; // insert an empty option at the front
                return options;
            }),
            tap((geographies) => {
                if (this.modalContext.WaterAccountID) {
                    this.waterAccountService.waterAccountsWaterAccountIDGet(this.modalContext.WaterAccountID).subscribe((waterAccounts) => {
                        this.formGroup.controls.WaterAccount.setValue(waterAccounts);
                    });
                } else {
                    this.formGroup.controls.WaterAccount.setValue(new WaterAccountSimpleDto());
                }
            }),
            tap((geographies) => {
                this.users$ = this.userService.usersGet().pipe(
                    tap((users) => {
                        geographies.forEach((geography) => {
                            this.userOptions[geography.Value] = users
                                .filter((x) => AuthorizationHelper.isSystemAdministrator(x))
                                .map((x) => ({ Value: x.UserID, Label: x.FullName }) as SelectDropdownOption);
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
        this.supportTicketService.supportTicketsEditPut(this.formGroup.getRawValue()).subscribe(
            (result) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Edit support ticket successfully.", AlertContext.Success));
                this.modalService.close(this.modalComponentRef, true);
            },
            (error) => {
                this.alertService.pushAlert(new Alert("Error occurred while attempting to edit a support ticket.", AlertContext.Danger));
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }
}
