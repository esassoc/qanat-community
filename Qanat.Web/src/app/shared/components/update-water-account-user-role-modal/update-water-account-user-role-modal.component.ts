import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { WaterAccountUserMinimalDto, WaterAccountUserMinimalDtoForm } from "src/app/shared/generated/model/water-account-user-minimal-dto";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { WaterAccountRolesAsSelectDropdownOptions } from "../../generated/enum/water-account-role-enum";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "update-water-account-user-role-modal",
    imports: [ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./update-water-account-user-role-modal.component.html",
    styleUrl: "./update-water-account-user-role-modal.component.scss",
})
export class UpdateWaterAccountUserRoleModalComponent implements OnInit {
    public ref: DialogRef<UpdateWaterAccountUserRoleContext, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public waterAccountDropDownFormControl: FormControl = new FormControl();

    public formGroup: FormGroup<WaterAccountUserMinimalDtoForm> = new FormGroup<WaterAccountUserMinimalDtoForm>({
        WaterAccountRoleID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
    });

    public WaterAccountRolesSelectDropdownOptions = WaterAccountRolesAsSelectDropdownOptions;

    public isLoadingSubmit = false;
    public userDisplayName: string;

    constructor(private waterAccountUserService: WaterAccountUserService) {}

    ngOnInit(): void {
        this.formGroup.patchValue({
            WaterAccountRoleID: this.ref.data.WaterAccountUser.WaterAccountRoleID,
        });

        this.userDisplayName =
            this.ref.data.WaterAccountUser.User.RoleID == RoleEnum.PendingLogin ? this.ref.data.WaterAccountUser.User.Email : this.ref.data.WaterAccountUser.User.FullName;
    }

    save(): void {
        this.isLoadingSubmit = true;

        const selectedWaterAccountRoleID = this.formGroup.get("WaterAccountRoleID").value;
        const waterAccountRole = this.WaterAccountRolesSelectDropdownOptions.find((x) => x.Value === selectedWaterAccountRoleID);

        const waterAccountUser = new WaterAccountUserMinimalDto({
            UserID: this.ref.data.WaterAccountUser.User.UserID,
            User: this.ref.data.WaterAccountUser.User,
            WaterAccountRole: waterAccountRole,
            WaterAccountRoleID: selectedWaterAccountRoleID,
            ClaimDate: new Date(),
        });

        const waterAccountID = this.ref.data.WaterAccountUser.WaterAccount.WaterAccountID;
        this.waterAccountUserService.updateWaterAccountUserWaterAccountUser(waterAccountID, this.ref.data.WaterAccountUser.User.UserID, waterAccountUser).subscribe({
            next: (result) => {
                this.isLoadingSubmit = false;
                this.ref.close(true); //MK 7/12/2024 - API returns an empty body, we probably should make sure POSTs/PUTs return the added/updated object.
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.ref.close(false);
            },
        });
    }

    close(): void {
        this.ref.close();
    }
}

export class UpdateWaterAccountUserRoleContext {
    WaterAccountUser: WaterAccountUserMinimalDto;
}
