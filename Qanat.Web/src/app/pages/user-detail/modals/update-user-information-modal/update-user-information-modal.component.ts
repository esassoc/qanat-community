import { Component, inject, OnInit } from "@angular/core";
import { FormGroup, FormControl, Validators, ReactiveFormsModule, FormsModule } from "@angular/forms";
import { NgSelectModule } from "@ng-select/ng-select";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { UserService } from "src/app/shared/generated/api/user.service";
import { ModelsAsSelectDropdownOptions } from "src/app/shared/generated/enum/model-enum";
import { RolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/role-enum";
import { ScenarioPlannerRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/scenario-planner-role-enum";
import { UserDto, UserUpsertDto, UserUpsertDtoForm } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";

@Component({
    selector: "update-user-information-modal",
    imports: [ReactiveFormsModule, FormsModule, FormFieldComponent, NgSelectModule],
    templateUrl: "./update-user-information-modal.component.html",
    styleUrl: "./update-user-information-modal.component.scss",
})
export class UpdateUserInformationModalComponent implements OnInit {
    public ref: DialogRef<UserContext, UserDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public ModelsAsSelectDropdownOptions = ModelsAsSelectDropdownOptions.sort((a, b) => a.Label.localeCompare(b.Label));
    public selectedModels: SelectDropdownOption[] = [];
    public formGroup: FormGroup<UserUpsertDtoForm> = new FormGroup<UserUpsertDtoForm>({
        RoleID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
        ScenarioPlannerRoleID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
        ModelIDs: new FormControl([], {
            nonNullable: false,
            validators: [],
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
    public RolesAsSelectDropdownOptions = RolesAsSelectDropdownOptions;
    public ScenarioPlannerRolesAsSelectDropdownOptions = ScenarioPlannerRolesAsSelectDropdownOptions;

    constructor(
        private userService: UserService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({
            RoleID: this.ref.data.User.RoleID,
            ModelIDs: this.ref.data.User.Models.map((model) => model.ModelID),
            ScenarioPlannerRoleID: this.ref.data.User.ScenarioPlannerRoleID,
            GETRunCustomerID: this.ref.data.User.GETRunCustomerID,
            GETRunUserID: this.ref.data.User.GETRunUserID,
        });

        this.selectedModels = this.ref.data.User.Models.map((model) => {
            return this.ModelsAsSelectDropdownOptions.find((x) => x.Value === model.ModelID);
        });
    }

    onSelectedModelsChanged(event) {
        this.selectedModels = event;
    }

    save(): void {
        this.isLoadingSubmit = true;

        const userUpsertDto = new UserUpsertDto({
            RoleID: this.formGroup.controls.RoleID.value,
            ScenarioPlannerRoleID: this.formGroup.controls.ScenarioPlannerRoleID.value,
            ModelIDs: this.selectedModels.map((x) => x.Value),
            GETRunCustomerID: this.formGroup.controls.GETRunCustomerID.value,
            GETRunUserID: this.formGroup.controls.GETRunUserID.value,
            ReceiveSupportEmails: this.ref.data.User.ReceiveSupportEmails,
        });

        this.userService.updateUser(this.ref.data.User.UserID, userUpsertDto).subscribe({
            next: (response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`${this.ref.data.User.FullName} was successfully updated.`, AlertContext.Success));
                this.ref.close(response);
            },
            error: () => this.ref.close(null),
        });
    }

    close(): void {
        this.ref.close(null);
    }
}

export class UserContext {
    public User: UserDto;
    public SystemRoleEdit: boolean;
}
