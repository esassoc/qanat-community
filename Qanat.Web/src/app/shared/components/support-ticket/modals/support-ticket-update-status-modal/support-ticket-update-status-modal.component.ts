import { AsyncPipe } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { ReactiveFormsModule, FormGroup } from "@angular/forms";
import { Observable, tap } from "rxjs";
import { FormFieldComponent, FormFieldType } from "../../../forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { SupportTicketService } from "../../../../generated/api/support-ticket.service";
import { UserService } from "../../../../generated/api/user.service";
import { Alert } from "../../../../models/alert";
import { AlertContext } from "../../../../models/enums/alert-context.enum";
import { AlertService } from "../../../../services/alert.service";
import { WaterAccountMinimalDto } from "../../../../generated/model/water-account-minimal-dto";
import { UserDto } from "../../../../generated/model/user-dto";
import { AuthorizationHelper } from "../../../../helpers/authorization-helper";
import { SupportTicketPrioritiesAsSelectDropdownOptions } from "../../../../generated/enum/support-ticket-priority-enum";
import { SupportTicketUpdateStatusContext } from "src/app/pages/support-ticket-admin-pages/support-ticket-detail/support-ticket-detail.component";
import { DialogRef } from "@ngneat/dialog";
import { SupportTicketStatusUpdateDtoForm, SupportTicketStatusUpdateDtoFormControls } from "../../../../generated/model/support-ticket-status-update-dto";
import { SupportTicketStatusEnum, SupportTicketStatusesAsSelectDropdownOptions } from "../../../../generated/enum/support-ticket-status-enum";
import { FlagEnum } from "../../../../generated/enum/flag-enum";
import { LoadingDirective } from "../../../../directives/loading.directive";

@Component({
    selector: "support-ticket-update-modal",
    imports: [FormFieldComponent, ReactiveFormsModule, AsyncPipe, LoadingDirective],
    templateUrl: "./support-ticket-update-status-modal.component.html",
    styleUrl: "./support-ticket-update-status-modal.component.scss",
})
export class SupportTicketUpdateStatusModalComponent implements OnInit {
    public ref: DialogRef<SupportTicketUpdateStatusContext, boolean> = inject(DialogRef);

    public geographyID: number;
    public geographyOptions$: Observable<SelectDropdownOption[]>;

    public waterAccounts$: Observable<WaterAccountMinimalDto>;
    public waterAccountOptions = {};

    public users$: Observable<UserDto[]>;
    public userOptions = [];

    public FormFieldType = FormFieldType;
    public supportTicketPrioritiesSelectDropdownOptions = SupportTicketPrioritiesAsSelectDropdownOptions;
    public supportTicketStatusSelectDropdownOptions = SupportTicketStatusesAsSelectDropdownOptions;

    public isLoading: boolean = true;

    public formGroup = new FormGroup<SupportTicketStatusUpdateDtoForm>({
        SupportTicketPriorityID: SupportTicketStatusUpdateDtoFormControls.SupportTicketPriorityID(),
        SupportTicketStatusID: SupportTicketStatusUpdateDtoFormControls.SupportTicketStatusID(),
        AssignedUserID: SupportTicketStatusUpdateDtoFormControls.AssignedUserID(),
    });

    constructor(
        private alertService: AlertService,
        private supportTicketService: SupportTicketService,
        private userService: UserService
    ) {}

    ngOnInit(): void {
        this.geographyID = this.ref.data.GeographyID;

        // disable "Unassigned" status option if an assigned user is selected
        this.formGroup.controls.AssignedUserID.valueChanges.subscribe((assignedUserID) => {
            if (assignedUserID && this.formGroup.controls.SupportTicketStatusID.getRawValue() == SupportTicketStatusEnum.Unassigned) {
                this.formGroup.controls.SupportTicketStatusID.patchValue(SupportTicketStatusEnum.Active);
            }
        });

        this.formGroup.controls.SupportTicketPriorityID.setValue(this.ref.data.SupportTicketPriorityID);
        this.formGroup.controls.SupportTicketStatusID.setValue(this.ref.data.SupportTicketStatusID);
        this.formGroup.controls.AssignedUserID.setValue(this.ref.data.AssignedUserID);

        this.users$ = this.userService.listUser().pipe(
            tap((users) => {
                var systemAdmins = users
                    .filter((x) => AuthorizationHelper.isSystemAdministrator(x))
                    .map(
                        (x) =>
                            ({
                                Value: x.UserID,
                                Label: x.FullName,
                                Group: "System Administrators",
                            }) as SelectDropdownOption
                    );

                const geographyWaterManagers = users
                    .filter(
                        (x) =>
                            x.GeographyUser.findIndex(
                                (y) => y.GeographyID == this.geographyID && AuthorizationHelper.hasGeographyFlag(this.geographyID, FlagEnum.HasManagerDashboard, x)
                            ) > -1
                    )
                    .map(
                        (x) =>
                            ({
                                Value: x.UserID,
                                Label: x.FullName,
                                Group: "Water Managers",
                            }) as SelectDropdownOption
                    );

                this.userOptions = [...systemAdmins, ...geographyWaterManagers];
                //Sort by label, then by descending group
                this.userOptions = this.userOptions.sort((a, b) => a.Label.localeCompare(b.Label)).sort((a, b) => b.Group.localeCompare(a.Group));

                //Add "Unassigned" option
                this.userOptions.unshift({ Value: null, Label: "Unassigned" } as SelectDropdownOption);
                this.isLoading = false;
            })
        );
    }

    close() {
        this.ref.close(null);
    }

    save() {
        this.alertService.clearAlerts();
        this.supportTicketService.updateSupportTicketStatusSupportTicket(this.ref.data.SupportTicketID, this.formGroup.getRawValue()).subscribe({
            next: () => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert("Support request status successfully updated.", AlertContext.Success));
                this.ref.close(true);
            },
            error: () => {
                this.ref.close(null);
            },
        });
    }
}
