import { CommonModule } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { AddUserByEmailDto, AddUserByEmailDtoForm } from "src/app/shared/generated/model/add-user-by-email-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { InviteToWaterAccountContext } from "../../users-and-settings.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WaterAccountRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/water-account-role-enum";

@Component({
    selector: "invite-user-to-water-account-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, SelectDropDownModule, FormFieldComponent],
    templateUrl: "./invite-user-to-water-account-modal.component.html",
    styleUrl: "./invite-user-to-water-account-modal.component.scss",
})
export class InviteUserToWaterAccountModalComponent implements OnInit, IModal {
    public FormFieldType = FormFieldType;

    modalComponentRef: ComponentRef<ModalComponent>;

    public formGroup: FormGroup<AddUserByEmailDtoForm> = new FormGroup<AddUserByEmailDtoForm>({
        Email: new FormControl(null),
        WaterAccountRoleID: new FormControl(null),
    });

    public modalContext: InviteToWaterAccountContext;
    public WaterAccountRolesSelectDropDownOptions = WaterAccountRolesAsSelectDropdownOptions;

    public isLoadingWaterAccounts: boolean = true;

    public isLoadingSubmit = false;

    constructor(
        private modalService: ModalService,
        private waterAccountUserService: WaterAccountUserService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {

    }

    save(): void {
        this.isLoadingSubmit = true;

        const addUserByEmailDto = new AddUserByEmailDto({
            Email: this.formGroup.get("Email").value,
            WaterAccountRoleID: this.formGroup.get("WaterAccountRoleID").value,
        });

        this.waterAccountUserService
            .waterAccountsWaterAccountIDInvitingUserInvitingUserIDPost(this.modalContext.WaterAccountID, this.modalContext.CurrentUserID, addUserByEmailDto)
            .subscribe({
                next: (result) => {
                    this.isLoadingSubmit = false;
                    this.alertService.pushAlert(new Alert("User successfully added! An email has been sent to notify them.", AlertContext.Success, true));
                    this.modalService.close(this.modalComponentRef, result);
                },
                error: () => {
                    this.isLoadingSubmit = false;
                    this.modalService.close(this.modalComponentRef);
                },
            });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}
