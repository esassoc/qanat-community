import { Component, inject, OnInit } from "@angular/core";
import { Observable, map, tap } from "rxjs";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { WaterAccountUserMinimalDto, WaterAccountUserMinimalDtoForm } from "src/app/shared/generated/model/water-account-user-minimal-dto";
import { UserContext } from "../update-user-information-modal/update-user-information-modal.component";
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from "@angular/forms";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";

import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { WaterAccountRolesAsSelectDropdownOptions } from "src/app/shared/generated/enum/water-account-role-enum";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { WaterAccountMinimalDto } from "src/app/shared/generated/model/models";

@Component({
    selector: "add-water-account-user-modal",
    imports: [ReactiveFormsModule, FormFieldComponent, LoadingDirective, AsyncPipe],
    templateUrl: "./add-water-account-user-modal.component.html",
    styleUrl: "./add-water-account-user-modal.component.scss",
})
export class AddWaterAccountUserModalComponent implements OnInit {
    public ref: DialogRef<UserContext, WaterAccountMinimalDto> = inject(DialogRef);
    public FormFieldType = FormFieldType;

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

    public waterAccountDropDownOptions$: Observable<FormInputOption[]>;

    public isLoadingWaterAccounts: boolean = true;

    public WaterAccountRolesSelectDropDownOptions = WaterAccountRolesAsSelectDropdownOptions;

    public isLoadingSubmit = false;

    constructor(
        private waterAccountService: WaterAccountService,
        private waterAccountUserService: WaterAccountUserService
    ) {}

    ngOnInit(): void {
        this.waterAccountDropDownOptions$ = this.waterAccountService.listByCurrentUserWaterAccount().pipe(
            tap(() => (this.isLoadingWaterAccounts = false)),
            map((waterAccounts) => {
                const waterAccountOptions = waterAccounts.map((x) => {
                    return { Value: x.WaterAccountID, Label: "#" + x.WaterAccountNumber } as FormInputOption;
                });
                return waterAccountOptions;
            })
        );
    }

    save(): void {
        this.isLoadingSubmit = true;

        const waterAccountID = this.formGroup.get("WaterAccountID").value;
        const waterAccountUser = new WaterAccountUserMinimalDto({
            UserID: this.ref.data.User.UserID,
            User: this.ref.data.User,
            WaterAccountRoleID: this.formGroup.get("WaterAccountRoleID").value,
            ClaimDate: new Date(),
        });

        this.waterAccountUserService.addUserWaterAccountUser(waterAccountID, waterAccountUser).subscribe({
            next: (result) => {
                this.isLoadingSubmit = false;
                this.ref.close(result);
            },
            error: () => (this.isLoadingSubmit = false),
        });
    }

    close(): void {
        this.ref.close();
    }
}
