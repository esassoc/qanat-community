import { CommonModule } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from "@angular/forms";
import { Observable, map } from "rxjs";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { RoleService } from "src/app/shared/generated/api/role.service";
import { UserService } from "src/app/shared/generated/api/user.service";
import { UserDto, UserUpsertDto, UserUpsertDtoForm } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "update-user-information-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./update-user-information-modal.component.html",
    styleUrl: "./update-user-information-modal.component.scss",
})
export class UpdateUserInformationModalComponent implements OnInit, IModal {
    public FormFieldType = FormFieldType;
    modalComponentRef: ComponentRef<ModalComponent>;

    public modalContext: UserContext;
    public roleDropDownOptions$: Observable<SelectDropdownOption[]>;
    public formGroup: FormGroup<UserUpsertDtoForm> = new FormGroup<UserUpsertDtoForm>({
        RoleID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
        GETRunCustomerID: new FormControl(null, {
            nonNullable: false,
            validators: [],
        }),
        GETRunUserID: new FormControl(null, {
            nonNullable: false,
            validators: [],
        }),
        ReceiveSupportEmails: new FormControl(null),
    });

    public isLoadingSubmit = false;

    constructor(
        private modalService: ModalService,
        private roleService: RoleService,
        private userService: UserService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.roleDropDownOptions$ = this.roleService.rolesGet().pipe(
            map((roles) => {
                const roleOptions = roles.map((x) => {
                    return { Value: x.RoleID, Label: x.RoleDisplayName } as SelectDropdownOption;
                });

                // Find the matching role for the modal context
                const selectedRoleID = roleOptions.find((option) => option.Value === this.modalContext.User.Role.RoleID);
                if (selectedRoleID) {
                    this.formGroup.controls.RoleID.setValue(selectedRoleID.Value);
                }

                return roleOptions;
            })
        );

        this.formGroup.patchValue({
            GETRunCustomerID: this.modalContext.User.GETRunCustomerID,
            GETRunUserID: this.modalContext.User.GETRunUserID,
        });
    }

    save(): void {
        this.isLoadingSubmit = true;

        const userUpsertDto = new UserUpsertDto({
            RoleID: this.formGroup.controls.RoleID.value,
            GETRunCustomerID: this.formGroup.controls.GETRunCustomerID.value,
            GETRunUserID: this.formGroup.controls.GETRunUserID.value,
            ReceiveSupportEmails: this.modalContext.User.ReceiveSupportEmails,
        });

        this.userService.usersUserIDPut(this.modalContext.User.UserID, userUpsertDto).subscribe((response) => {
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert(`${this.modalContext.User.FullName} was successfully updated.`, AlertContext.Success));
            this.modalService.close(this.modalComponentRef, response);
        });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class UserContext {
    public User: UserDto;
}
