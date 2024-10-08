import { Component, ComponentRef, OnInit } from "@angular/core";
import { Observable, map, tap } from "rxjs";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { SelectDropdownOption } from "src/app/shared/components/inputs/select-dropdown/select-dropdown.component";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { WaterAccountUserMinimalDto, WaterAccountUserMinimalDtoForm } from "src/app/shared/generated/model/water-account-user-minimal-dto";
import { IModal, ModalService } from "src/app/shared/services/modal/modal.service";
import { UserContext } from "../update-user-information-modal/update-user-information-modal.component";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from "@angular/forms";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { CommonModule } from "@angular/common";
import { SelectDropDownModule } from "ngx-select-dropdown";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "add-water-account-user-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, SelectDropDownModule, FormFieldComponent, LoadingDirective],
    templateUrl: "./add-water-account-user-modal.component.html",
    styleUrl: "./add-water-account-user-modal.component.scss",
})
export class AddWaterAccountUserModalComponent implements OnInit, IModal {
    public FormFieldType = FormFieldType;

    modalComponentRef: ComponentRef<ModalComponent>;

    public waterAccountDropDownFormControl: FormControl = new FormControl();

    public formGroup: FormGroup<WaterAccountUserMinimalDtoForm> = new FormGroup<WaterAccountUserMinimalDtoForm>({
        WaterAccountID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
        WaterAccountRoleID: new FormControl(null, {
            nonNullable: true,
            validators: [Validators.required],
        }),
    });

    public modalContext: UserContext;
    public waterAccountDropDownOptions$: Observable<SelectDropdownOption[]>;
    public waterAccountDropDownConfig = {
        search: true,
        height: "320px",
        placeholder: "Select a water account",
        searchFn: (option: SelectDropdownOption) => option.Label,
        displayFn: (option: SelectDropdownOption) => option.Label,
    };

    public isLoadingWaterAccounts: boolean = true;

    public roleDropDownOptions$: Observable<SelectDropdownOption[]>;

    public isLoadingSubmit = false;

    constructor(
        private modalService: ModalService,
        private waterAccountService: WaterAccountService,
        private waterAccountUserService: WaterAccountUserService
    ) {}

    ngOnInit(): void {
        this.waterAccountDropDownOptions$ = this.waterAccountService.waterAccountsGet().pipe(
            tap(() => (this.isLoadingWaterAccounts = false)),
            map((waterAccounts) => {
                const waterAccountOptions = waterAccounts.map((x) => {
                    return { Value: x.WaterAccountID, Label: "#" + x.WaterAccountNumber } as SelectDropdownOption;
                });
                return waterAccountOptions;
            })
        );

        this.roleDropDownOptions$ = this.waterAccountUserService.waterAccountRolesGet().pipe(
            map((roles) => {
                const roleOptions = roles.map((x) => {
                    return { Value: x.WaterAccountRoleID, Label: x.WaterAccountRoleDisplayName } as SelectDropdownOption;
                });
                return roleOptions;
            })
        );
    }

    changedWaterAccount(event: any) {
        const newValue = event.value.Value;
        this.formGroup.controls.WaterAccountID.patchValue(newValue);
    }

    save(): void {
        this.isLoadingSubmit = true;

        const waterAccountID = this.formGroup.get("WaterAccountID").value;
        const waterAccountUsers = [
            new WaterAccountUserMinimalDto({
                UserID: this.modalContext.User.UserID,
                User: this.modalContext.User,
                WaterAccountRoleID: this.formGroup.get("WaterAccountRoleID").value,
                ClaimDate: new Date(),
            }),
        ];

        this.waterAccountService.waterAccountsWaterAccountIDEditUsersPut(waterAccountID, waterAccountUsers).subscribe({
            next: (result) => {
                this.isLoadingSubmit = false;
                this.modalService.close(this.modalComponentRef, result);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close(): void {
        this.modalService.close(this.modalComponentRef);
    }
}
