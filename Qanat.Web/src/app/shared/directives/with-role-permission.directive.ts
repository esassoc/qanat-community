import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { UserDto } from "../generated/model/models";
import { PermissionEnum } from "../generated/enum/permission-enum";
import { RightsEnum } from "../models/enums/rights.enum";
import { AuthorizationHelper } from "../helpers/authorization-helper";

@Directive({
    selector: "[withRolePermission]",
    standalone: true,
})
export class WithRolePermissionDirective {
    @Input() set withRolePermission(rolePermissionCheck: RolePermissionCheck) {
        this.viewContainer.clear(); // always clear up front
        try {
            if (AuthorizationHelper.hasRolePermission(rolePermissionCheck.permission, rolePermissionCheck.rights, rolePermissionCheck.currentUser)) {
                this.viewContainer.createEmbeddedView(this.templateRef);
            } else {
                this.viewContainer.clear();
            }
        } catch (error) {
            console.error(`Error checking role permission.`);
            this.viewContainer.clear();
        }
    }

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) {}
}

export interface RolePermissionCheck {
    currentUser: UserDto;
    permission: PermissionEnum;
    rights: RightsEnum;
}
