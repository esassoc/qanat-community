import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { AddUserByEmailDto, AddUserByEmailDtoForm } from "src/app/shared/generated/model/add-user-by-email-dto";
import { AlertService } from "src/app/shared/services/alert.service";
import { InviteToWaterAccountContext } from "../../users-and-settings.component";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WaterAccountRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/water-account-role-enum";
import { DialogRef } from "@ngneat/dialog";
import { WaterAccountUserMinimalDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "invite-user-to-water-account-modal",
    imports: [ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./invite-user-to-water-account-modal.component.html",
    styleUrl: "./invite-user-to-water-account-modal.component.scss",
})
export class InviteUserToWaterAccountModalComponent implements OnInit {
    public ref: DialogRef<InviteToWaterAccountContext, WaterAccountUserMinimalDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<AddUserByEmailDtoForm> = new FormGroup<AddUserByEmailDtoForm>({
        Email: new FormControl(null),
        WaterAccountRoleID: new FormControl(null),
    });
    public WaterAccountRolesSelectDropDownOptions = WaterAccountRolesAsSelectDropdownOptions;

    public isLoadingWaterAccounts: boolean = true;

    public isLoadingSubmit = false;

    constructor(
        private waterAccountUserService: WaterAccountUserService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {}

    save(): void {
        this.isLoadingSubmit = true;

        const addUserByEmailDto = new AddUserByEmailDto({
            Email: this.formGroup.get("Email").value,
            WaterAccountRoleID: this.formGroup.get("WaterAccountRoleID").value,
        });

        this.waterAccountUserService.addUserOnWaterAccountByEmailWaterAccountUser(this.ref.data.WaterAccountID, this.ref.data.CurrentUserID, addUserByEmailDto).subscribe({
            next: (result) => {
                this.isLoadingSubmit = false;
                this.alertService.pushAlert(new Alert("User successfully added! An email has been sent to notify them.", AlertContext.Success, true));
                this.ref.close(result);
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.ref.close(null);
            },
        });
    }

    close(): void {
        this.ref.close(null);
    }
}
