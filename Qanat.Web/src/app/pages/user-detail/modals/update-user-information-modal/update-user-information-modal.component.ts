import { CommonModule } from "@angular/common";
import { Component, ComponentRef, OnInit } from "@angular/core";
import { FormGroup, FormControl, Validators, ReactiveFormsModule, FormsModule } from "@angular/forms";
import { NgSelectComponent, NgSelectModule } from "@ng-select/ng-select";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { UserService } from "src/app/shared/generated/api/user.service";
import { ModelsAsSelectDropdownOptions } from "src/app/shared/generated/enum/model-enum";
import { RolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/role-enum";
import { ScenarioPlannerRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/scenario-planner-role-enum";
import { UserDto, UserUpsertDto, UserUpsertDtoForm } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "update-user-information-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, FormFieldComponent, NgSelectModule],
    templateUrl: "./update-user-information-modal.component.html",
    styleUrl: "./update-user-information-modal.component.scss",
})
export class UpdateUserInformationModalComponent implements OnInit, IModal {
    public FormFieldType = FormFieldType;
    modalComponentRef: ComponentRef<ModalComponent>;
    public ModelsAsSelectDropdownOptions = ModelsAsSelectDropdownOptions.sort((a, b) => a.Label.localeCompare(b.Label));
    public selectedModels: SelectDropdownOption[] = [];
    public modalContext: UserContext;
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
        private modalService: ModalService,
        private userService: UserService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.formGroup.patchValue({
            RoleID: this.modalContext.User.RoleID,
            ModelIDs: this.modalContext.User.Models.map((model) => model.ModelID),
            ScenarioPlannerRoleID: this.modalContext.User.ScenarioPlannerRoleID,
            GETRunCustomerID: this.modalContext.User.GETRunCustomerID,
            GETRunUserID: this.modalContext.User.GETRunUserID,
        });

        this.selectedModels = this.modalContext.User.Models.map((model) => {
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
            ReceiveSupportEmails: this.modalContext.User.ReceiveSupportEmails,
        });

        this.userService.usersUserIDPut(this.modalContext.User.UserID, userUpsertDto).subscribe(
            (response) => {
                this.alertService.clearAlerts();
                this.alertService.pushAlert(new Alert(`${this.modalContext.User.FullName} was successfully updated.`, AlertContext.Success));
                this.modalService.close(this.modalComponentRef, response);
            },
            (error) => {
                this.modalService.close(this.modalComponentRef, null);
            }
        );
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}

export class UserContext {
    public User: UserDto;
    public SystemRoleEdit: boolean;
}
