import { CommonModule } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { Observable, map } from "rxjs";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { WaterAccountUserMinimalDto, WaterAccountUserMinimalDtoForm } from "src/app/shared/generated/model/water-account-user-minimal-dto";
import { WaterAccountRoleSimpleDto } from "src/app/shared/generated/model/water-account-role-simple-dto";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "update-water-account-user-role-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, SelectDropDownModule, FormFieldComponent, IconComponent],
    templateUrl: "./update-water-account-user-role-modal.component.html",
    styleUrl: "./update-water-account-user-role-modal.component.scss",
})
export class UpdateWaterAccountUserRoleModalComponent implements OnInit, IModal {
    public FormFieldType = FormFieldType;

    modalComponentRef: ComponentRef<ModalComponent>;

    public waterAccountDropDownFormControl: FormControl = new FormControl();

    public formGroup: FormGroup<WaterAccountUserMinimalDtoForm> = new FormGroup<WaterAccountUserMinimalDtoForm>({
        WaterAccountRole: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
    });

    public modalContext: UpdateWaterAccountUserRoleContext;
    public roleDropDownOptions$: Observable<SelectDropdownOption[]>;
    public waterAccountRoles: WaterAccountRoleSimpleDto[];

    public isLoadingSubmit = false;
    public userDisplayName: string;

    constructor(
        private modalService: ModalService,
        private waterAccountUserService: WaterAccountUserService
    ) {}

    ngOnInit(): void {
        this.roleDropDownOptions$ = this.waterAccountUserService.waterAccountRolesGet().pipe(
            map((roles) => {
                const roleOptions = roles.map((x) => {
                    return { Value: x, Label: x.WaterAccountRoleDisplayName } as SelectDropdownOption;
                });

                // Find the matching role for the modal context
                const selectedRole = roleOptions.find(
                    (option) => option.Value && option.Value.WaterAccountRoleID === this.modalContext.WaterAccountUser.WaterAccountRole.WaterAccountRoleID
                );
                if (selectedRole) {
                    this.formGroup.controls.WaterAccountRole.setValue(selectedRole.Value);
                }

                this.waterAccountRoles = roles;
                return roleOptions;
            })
        );

        this.userDisplayName =
            this.modalContext.WaterAccountUser.User.RoleID == RoleEnum.PendingLogin
                ? this.modalContext.WaterAccountUser.User.Email
                : this.modalContext.WaterAccountUser.User.FullName;
    }

    save(): void {
        this.isLoadingSubmit = true;

        const selectedWaterAccountRoleID = this.formGroup.get("WaterAccountRole").value.WaterAccountRoleID;
        const waterAccountRole = this.waterAccountRoles.find((x) => x.WaterAccountRoleID === selectedWaterAccountRoleID);

        const waterAccountUser = new WaterAccountUserMinimalDto({
            UserID: this.modalContext.WaterAccountUser.User.UserID,
            User: this.modalContext.WaterAccountUser.User,
            WaterAccountRole: waterAccountRole,
            WaterAccountRoleID: selectedWaterAccountRoleID,
            ClaimDate: new Date(),
        });

        const waterAccountID = this.modalContext.WaterAccountUser.WaterAccount.WaterAccountID;
        this.waterAccountUserService.waterAccountsWaterAccountIDUserUserIDPut(waterAccountID, this.modalContext.WaterAccountUser.User.UserID, waterAccountUser).subscribe({
            next: (result) => {
                this.isLoadingSubmit = false;
                this.modalService.close(this.modalComponentRef, true); //MK 7/12/2024 - API returns an empty body, we probably should make sure POSTs/PUTs return the added/updated object.
            },
            error: () => {
                this.isLoadingSubmit = false;
                this.modalService.close(this.modalComponentRef, false);
            },
        });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class UpdateWaterAccountUserRoleContext {
    WaterAccountUser: WaterAccountUserMinimalDto;
}
