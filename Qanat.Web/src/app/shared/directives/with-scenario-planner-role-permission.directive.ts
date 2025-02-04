import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { RolePermissionCheck } from "./with-role-permission.directive";
import { AuthorizationHelper } from "../helpers/authorization-helper";

@Directive({
    selector: "[withScenarioPlannerRolePermission]",
    standalone: true,
})
export class WithScenarioPlannerRolePermissionDirective {
    @Input() set withScenarioPlannerRolePermission(rolePermissionCheck: RolePermissionCheck) {
        this.viewContainer.clear(); // always clear up front
        try {
            if (rolePermissionCheck == null || rolePermissionCheck.currentUser == null) {
                this.viewContainer.clear();
                return;
            }

            var isAdmin = AuthorizationHelper.isSystemAdministrator(rolePermissionCheck.currentUser);
            if (isAdmin) {
                this.viewContainer.createEmbeddedView(this.templateRef);
                return;
            }

            if (AuthorizationHelper.hasScenarioPlannerRolePermission(rolePermissionCheck.permission, rolePermissionCheck.rights, rolePermissionCheck.currentUser)) {
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
